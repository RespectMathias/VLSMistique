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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VLSMistique.Models;
using VLSMistique.Interfaces;

namespace VLSMistique.Services
{
    /// <summary> InputValidator class validates the inputs provided by the user. Implements the <see cref="IInputValidator" /> interface. </summary>
    public class InputValidator : IInputValidator
    {
        // Fields and dependencies

        // Minimum and maximum valid subnet amounts. Chosen based on typical subnet configurations.
        private const int MinSubnetAmount = 1;

        // Minimum and maximum valid host amounts. In a subnet, there can be up to 254 usable host addresses.
        private const int MinHostAmount = 1;
        private const int MaxHostAmount = 254;
        private const string AddressPattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"; // Pattern used to validate the IP address.

        /// <summary> Validates user-provided address, subnet Amount, and host amounts contained in subnets. </summary>
        /// <param name="address">The IP address to be validated.</param>
        /// <param name="subnetAmount">The subnet amount to be validated.</param>
        /// <param name="subnets">The collection of subnets with host amounts to be validated.</param>
        /// <returns>Returns true if all inputs are valid; otherwise, false.</returns>
        public bool Validate(string address, int subnetAmount, ObservableCollection<SubnetModel> subnets)
        {
            return subnetAmount >= MinSubnetAmount // Validates the subnet amount. 
                && !string.IsNullOrEmpty(address) && Regex.IsMatch(address, AddressPattern) // Validates the IPAdress. 
                && subnets.All(subnet => subnet.HostAmount > MinHostAmount && subnet.HostAmount < MaxHostAmount); // Validates the Host amounts.
        }
    }
}