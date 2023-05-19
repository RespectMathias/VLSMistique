/*
 *    Copyright 2023 Mathias Lund-Hansen
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
 *  limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Net;
using VLSMistique.Interfaces;

namespace VLSMistique.Models
{
    /// <summary> Model for VLSM calculation. Implements the <see cref="IVLSMCalculatorModel" /> interface. </summary>
    public class VLSMCalculatorModel : IVLSMCalculatorModel
    {
        #region Public Methods

        /// <summary> Generates a list of Subnets based on a given IP address, subnet amount and the amount of hosts per subnet. </summary>
        /// <param name="ipAddress">The IP address to be subnetted.</param>
        /// <param name="subnetAmount">The number of subnets to create.</param>
        /// <param name="hostAmounts">List containing the amounts of hosts for each subnet.</param>
        /// <returns>A List of type <see cref="SubnetModel" /> representing each subnet.</returns>
        public List<SubnetModel> CalculateSubnets(string ipAddress, int subnetAmount, List<int> hostAmounts)
        {
            var subnets = new List<SubnetModel>();
            var ip = InitializeAddress(ipAddress);

            for (var i = 0; i < subnetAmount; i++)
            {
                var hostAmount = hostAmounts[i];
                var subnetModel = CreateSubnet(ip, hostAmount);
                subnets.Add(subnetModel);
                ip = GetNextAddress(subnetModel.BroadcastAddress);
            }

            return subnets;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary> Initializes the IP address and set the last octet to 0. </summary>
        /// <param name="ipAddress">The IP address to be initialized.</param>
        /// <returns>A new <see cref="IPAddress" /> instance with the last octet set to 0.</returns>
        private static IPAddress InitializeAddress(string ipAddress)
        {
            var ip = IPAddress.Parse(ipAddress);
            byte[] ipBytes = ip.GetAddressBytes();
            ipBytes[3] = 0;

            return new IPAddress(ipBytes);
        }

        /// <summary> Creates a new subnet based on an IP address and the host amount.</summary>
        /// <param name="ip">The base IP address for the subnet.</param>
        /// <param name="hostAmount">The amount of hosts the subnet should be able to hold.</param>
        /// <returns>A new instance of <see cref="SubnetModel" /> representing the subnet.</returns>
        private static SubnetModel CreateSubnet(IPAddress ip, int hostAmount)
        {
            var maxSubnetHosts = CalculateMaxSubnetHosts(hostAmount);
            var cidr = CalculateCidrFromHosts(hostAmount);
            var mask = CalculateSubnetMask(cidr);
            var networkAddress = CalculateNetworkAddress(ip, mask);
            var broadcastAddress = CalculateBroadcastAddress(ip, mask);
            var range = CalculateRange(networkAddress, broadcastAddress);

            return new SubnetModel(broadcastAddress, networkAddress, mask, range, maxSubnetHosts, hostAmount);
        }

        /// <summary> Calculates the maximum amount of hosts a subnet can hold. </summary>
        /// <param name="hostAmount">The desired amount of hosts.</param>
        /// <returns>The maximum amount of hosts the subnet can hold as a string.</returns>
        private static string CalculateMaxSubnetHosts(int hostAmount)
        {
            var subnetHosts = (int)Math.Ceiling(Math.Log(hostAmount + 2, 2));
            subnetHosts = (int)Math.Pow(2, subnetHosts);
            subnetHosts -= 2;

            return subnetHosts.ToString();
        }

        /// <summary> Calculates the CIDR notation based on the host amount. </summary>
        /// <param name="hostAmount">The desired amount of hosts.</param>
        /// <returns>The CIDR notation as an integer.</returns>
        private static int CalculateCidrFromHosts(int hostAmount)
        {
            return 32 - (int)Math.Ceiling(Math.Log(hostAmount + 2, 2));
        }

        /// <summary> Calculates the subnet mask based on the CIDR notation. </summary>
        /// <param name="cidr">The CIDR notation.</param>
        /// <returns>The subnet mask as an <see cref="IPAddress" />.</returns>
        private static IPAddress CalculateSubnetMask(int cidr)
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

        /// <summary> Calculates the network address based on the IP address and the subnet mask. </summary>
        /// <param name="ip">The IP address.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <returns>The network address as an <see cref="IPAddress" />.</returns>
        private static IPAddress CalculateNetworkAddress(IPAddress ip, IPAddress subnetMask)
        {
            var ipBytes = ip.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();
            var networkAddressBytes = new byte[4];
            
            for (var i = 0; i < 4; i++)
                networkAddressBytes[i] = (byte)(ipBytes[i] & subnetMaskBytes[i]);
            
            return new IPAddress(networkAddressBytes);
        }

        /// <summary> Calculates the broadcast address based on the IP address and the subnet mask. </summary>
        /// <param name="ip">The IP address.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <returns>The broadcast address as an <see cref="IPAddress" />.</returns>
        private static IPAddress CalculateBroadcastAddress(IPAddress ip, IPAddress subnetMask)
        {
            var ipBytes = ip.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();
            var broadcastAddressBytes = new byte[4];
            
            for (var i = 0; i < 4; i++)
                broadcastAddressBytes[i] = (byte)(ipBytes[i] | ~subnetMaskBytes[i]);
            
            return new IPAddress(broadcastAddressBytes);
        }

        /// <summary> Calculates the first usable IP address in a subnet. </summary>
        /// <param name="ip">The network address of the subnet.</param>
        /// <returns>The first usable IP address as an <see cref="IPAddress" />.</returns>
        private static IPAddress CalculateFirstAddress(IPAddress ip)
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

        /// <summary> Calculates the last usable IP address in a subnet. </summary>
        /// <param name="ip">The broadcast address of the subnet.</param>
        /// <returns>The last usable IP address as an <see cref="IPAddress" />.</returns>
        private static IPAddress CalculateLastAddress(IPAddress ip)
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

        /// <summary> Calculates the range of usable IP addresses in a subnet. </summary>
        /// <param name="networkAddress">The network address of the subnet.</param>
        /// <param name="broadcastAddress">The broadcast address of the subnet.</param>
        /// <returns>The range of usable IP addresses as a string.</returns>
        private static string CalculateRange(IPAddress networkAddress, IPAddress broadcastAddress)
        {
            var firstAddress = CalculateFirstAddress(networkAddress);
            var lastAddress = CalculateLastAddress(broadcastAddress);

            return $"{firstAddress} - {lastAddress}";
        }

        /// <summary> Gets the next IP address after a given IP address. </summary>
        /// <param name="ip">The IP address to get the next address for.</param>
        /// <returns>The next IP address as an <see cref="IPAddress" />.</returns>
        private static IPAddress GetNextAddress(IPAddress ip)
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

        #endregion Private Methods
    }
}