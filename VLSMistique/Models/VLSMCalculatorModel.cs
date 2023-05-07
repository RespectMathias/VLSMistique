/*   Copyright 2023 Mathias Lund-Hansen
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License. */

using System;
using System.Collections.Generic;
using System.Net;
using VLSMistique.Interfaces;

namespace VLSMistique.Models
{

    /// <summary> The class that calculates the subnets. </summary>
    public class VLSMCalculatorModel : IVLSMCalculatorModel
    {
        /// <summary> A list that calculates the subnets based on the given IP address, the amont of subnets, and a list of host per subnet. </summary>
        public List<SubnetModel> CalculateSubnets(string ipAddress, int subnetAmount, List<int> hostAmounts)
        {
            var subnets = new List<SubnetModel>();

            var ip = GetAddress(ipAddress);

            // Calculate each subnet
            for (var i = 0; i < subnetAmount; i++)
            {
                // Get the required number of hosts
                var hostAmount = hostAmounts[i];

                var maxSubnetHosts = GetMaxSubnetHosts(hostAmount);

                var cidr = GetCidrFromHosts(hostAmount);

                var subnetMask = GetSubnetMask(cidr);
                var mask = subnetMask.ToString();

                var networkAddress = GetNetworkAddress(ip, subnetMask);

                // Calculate the broadcast
                var broadcastAddress = GetBroadcastAddress(ip, subnetMask);

                
                var FirstAddress = GetFirstAddress(networkAddress);
                var LastAddress = GetLastAddress(broadcastAddress);

                // Calculate the range for subnet in the form of "FirstAddress - LastAddress"
                var range = $"{FirstAddress} - {LastAddress}";

                // Add the subnet to the list
                var subnetModel = new SubnetModel(broadcastAddress, networkAddress, mask, range, hostAmount, maxSubnetHosts);
                subnets.Add(subnetModel);

                ip = GetNextAddress(broadcastAddress);
            }

            return subnets;
        }
        
        /// <summary> Parses the IPAdress from a string to an IP and sets the last octet to 0. </summary>
        static IPAddress GetAddress(string ipAddress)
        {
            var ip = IPAddress.Parse(ipAddress);

            // Set the last octet to 0 to always start with the first subnet
            byte[] ipBytes = ip.GetAddressBytes();
            ipBytes[3] = 0;
            
            return new IPAddress(ipBytes);
        }

        /// <summary> Calculates the maximum amount of hosts from the required hosts. </summary>
        private static int GetMaxSubnetHosts(int hostAmount)
        {
            var subnetHosts = (int)Math.Ceiling(Math.Log(hostAmount + 2, 2));
            subnetHosts = (int)Math.Pow(2, subnetHosts);
            
            return subnetHosts - 2;
        }

        /// <summary> Calculates the CIDR notation from the required hosts. </summary>
        private static int GetCidrFromHosts(int hostAmount)
        {
            return 32 - (int)Math.Ceiling(Math.Log(hostAmount + 2, 2));
        }

        /// <summary> Converts the CIDR notation into a subnet mask. </summary>
        private static IPAddress GetSubnetMask(int cidr)
        {
            var subnetMask = new byte[4];

            for (int i = 0; i < subnetMask.Length; i++)
            {
                if (cidr >= 8)
                {
                    subnetMask[i] = 255;
                    cidr -= 8;
                }
                else if (cidr > 0)
                {
                    subnetMask[i] = (byte)(256 - Math.Pow(2, 8 - cidr));
                    cidr = 0;
                }
            }

            return new IPAddress(subnetMask);
        }

        /// <summary> Calculates the network address from the IP address and the subnet mask. </summary>
        private static IPAddress GetNetworkAddress(IPAddress ip, IPAddress subnetMask)
        {
            var ipBytes = ip.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();
            var networkAddressBytes = new byte[4];
            
            for (var i = 0; i < 4; i++)
                networkAddressBytes[i] = (byte)(ipBytes[i] & subnetMaskBytes[i]);
            
            return new IPAddress(networkAddressBytes);
        }

        /// <summary> Calculates the broadcast address from the IP address and the subnet mask. </summary>
        private static IPAddress GetBroadcastAddress(IPAddress ip, IPAddress subnetMask)
        {
            var ipBytes = ip.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();
            var broadcastAddressBytes = new byte[4];
            
            for (var i = 0; i < 4; i++)
                broadcastAddressBytes[i] = (byte)(ipBytes[i] | ~subnetMaskBytes[i]);
            
            return new IPAddress(broadcastAddressBytes);
        }

        /// <summary> Calculates the first availible IP address from the network address. </summary>
        private static IPAddress GetFirstAddress(IPAddress ip)
        {
            var ipBytes = ip.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(ipBytes);
            
            var FirstAddressBytes = new byte[4];
            var carry = true;
            for (var i = 0; i < 4; i++)
            {
                if (carry)
                {
                    ipBytes[i]++;
                    carry = false;
                }
                if ((int)ipBytes[i] == 256)
                {
                    ipBytes[i] = 0;
                    carry = true;
                }
                FirstAddressBytes[i] = ipBytes[i];
            }
            if (BitConverter.IsLittleEndian)
                Array.Reverse(FirstAddressBytes);
            
            return new IPAddress(FirstAddressBytes);
        }

        /// <summary> Calculates the last availible IP address from the broadcast address. </summary>
        private static IPAddress GetLastAddress(IPAddress ip)
        {
            var ipBytes = ip.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(ipBytes);

            var LastAddressBytes = new byte[4];
            var carry = true;
            for (var i = 0; i < 4; i++)
            {
                if (carry)
                {
                    ipBytes[i]--;
                    carry = false;
                }
                if ((int)ipBytes[i] == -1)
                {
                    ipBytes[i] = 255;
                    carry = true;
                }
                LastAddressBytes[i] = (byte)ipBytes[i];
            }
            if (BitConverter.IsLittleEndian)
                Array.Reverse(LastAddressBytes);
            
            return new IPAddress(LastAddressBytes);
        }

        /// <summary> Calculates the next IP address for the next subnet using the broadcast address. </summary>
        static IPAddress GetNextAddress(IPAddress ip)
        {
            byte[] ipBytes = ip.GetAddressBytes();
            for (int i = ipBytes.Length - 1; i >= 0; i--)
            {
                if (ipBytes[i] < 255)
                {
                    ipBytes[i]++;
                    break;
                }
                else
                {
                    ipBytes[i] = 0;
                    if (i == 0)
                    {
                        Array.Resize(ref ipBytes, ipBytes.Length + 1);
                        ipBytes[0] = 1;
                    }
                }
            }
            return new IPAddress(ipBytes);
        }
    }
}
