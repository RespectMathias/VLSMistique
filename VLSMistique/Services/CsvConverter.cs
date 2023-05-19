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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using VLSMistique.Models;
using VLSMistique.Interfaces;

namespace VLSMistique.Services
{
    public class CsvConverter : ICsvConverter
    {
        private readonly string _delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator == "," ? "," : ";";

        /// <summary> Converts a collection of subnet models to a CSV string. </summary>
        /// <param name="subnets">The collection of subnet models.</param>
        /// <returns>A CSV string representing the subnet data.</returns>
        public string Convert(ObservableCollection<SubnetModel> subnets)
        {
            var csv = new StringBuilder();

            // Add headers to the CSV string
            csv.AppendLine("Required Host" + _delimiter +
                           "Available Hosts" + _delimiter +
                           "Subnet Mask" + _delimiter +
                           "Network Address" + _delimiter +
                           "Range" + _delimiter +
                           "Broadcast Address");

            // Add subnet data to the CSV string
            foreach (var subnet in subnets)
            {
                csv.AppendLine(subnet.HostAmount + _delimiter +
                               subnet.MaxSubnetHosts + _delimiter +
                               subnet.Mask + _delimiter +
                               subnet.BroadcastAddress + _delimiter +
                               subnet.Range + _delimiter +
                               subnet.NetworkAddress);
            }

            return csv.ToString();
        }
    }
}
