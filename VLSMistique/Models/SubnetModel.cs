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

using CommunityToolkit.Mvvm.ComponentModel;
using System.Net;

namespace VLSMistique.Models
{
    /// <summary> The class that represents a subnet. </summary>
    public partial class SubnetModel : ObservableObject
    {
        private int _hostAmount;
        /// <summary> The required amount of hosts of the subnet. </summary>
        public int HostAmount
        {
            get => _hostAmount;
            set
            {
                if (SetProperty(ref _hostAmount, value))
                {
                    HostAmountChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler HostAmountChanged;

        /// <summary> The Broadcast Address of the subnet. </summary>
        [ObservableProperty]
        public IPAddress _broadcastAddress;

        /// <summary> The Network Address of the subnet. </summary>
        [ObservableProperty]
        public IPAddress _networkAddress;

        /// <summary> The Subnet Mask of the subnet. </summary>
        [ObservableProperty]
        public IPAddress _mask;

        /// <summary> The IP-range of the subnet. Meaning the first and last IP-address of the subnet. </summary>
        [ObservableProperty]
        public string _range;

        /// <summary> The maximum amount of hosts in the subnet. </summary>
        [ObservableProperty]
        public int _maxSubnetHosts;

        /// <summary> Contains the values of the subnet. </summary>
        public SubnetModel(IPAddress broadcastAddress, IPAddress networkAddress, IPAddress mask, string range, int hostAmount, int maxSubnetHosts)
        {
            BroadcastAddress = broadcastAddress;
            NetworkAddress = networkAddress;
            Mask = mask;
            Range = range;
            HostAmount = hostAmount;
            MaxSubnetHosts = maxSubnetHosts;
        }
    }
}
