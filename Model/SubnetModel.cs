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

using System.ComponentModel;
using System.Net;
using VLSMCalculator.Interfaces;

namespace VLSMCalculator.Model
{
    /// <summary> The class that represents a subnet. Implements INotifyPropertyChanged to notify the view of changes. </summary>
    public class SubnetModel : INotifyPropertyChanged
    {
        /// <summary> The required amount of hosts of the subnet. Used to notify the view of changes. </summary>
        private int _hostAmount;

        /// <summary> The Broadcast Address of the subnet. </summary>
        public IPAddress BroadcastAddress { get; set; }

        /// <summary> The Network Address of the subnet. </summary>
        public IPAddress NetworkAddress { get; set; }

        /// <summary> The Subnet Mask of the subnet. </summary>
        public string Mask { get; set; }

        /// <summary> The IP-range of the subnet. Meaning the first and last IP-address of the subnet. </summary>
        public string Range { get; set; }

        /// <summary> The maximum amount of hosts in the subnet. </summary>
        public int MaxSubnetHosts { get; set; }

        /// <summary> The required amount of hosts of the subnet. </summary>
        public int HostAmount
        {
            get => _hostAmount;
            set
            {
                _hostAmount = value;
                OnPropertyChanged(nameof(HostAmount));
            }
        }
        /// <summary> Contains the values of the subnet. </summary>
        public SubnetModel(IPAddress broadcastAddress, IPAddress networkAddress, string mask, string range, int hostAmount, int maxSubnetHosts)
        {
            BroadcastAddress = broadcastAddress;
            NetworkAddress = networkAddress;
            Mask = mask;
            Range = range;
            HostAmount = hostAmount;
            MaxSubnetHosts = maxSubnetHosts;
        }

        /// <summary> Event handler for the PropertyChanged event. Used to notify the view of changes. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> The method that notifies the view of changes. </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
