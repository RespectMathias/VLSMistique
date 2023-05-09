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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net;
using VLSMistique.Interfaces;
using VLSMistique.Models;

namespace VLSMistique.ViewModels
{
    /// <summary> View model for the VLSM calculator. </summary>
    public partial class MainWindowViewModel : ObservableRecipient
    {
        private string _address;
        private int _subnetAmount;
        private ObservableCollection<SubnetModel> _subnets = new ObservableCollection<SubnetModel>();

        /// <summary> Gets or sets the IP address. </summary>
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        /// <summary> Gets or sets the amount of subnets. </summary>
        public int SubnetAmount
        {
            get => _subnetAmount;
            set
            {
                SetProperty(ref _subnetAmount, value);
                AddSubnets(value);
            }
        }

        /// <summary> Gets the subnets. </summary>
        public ObservableCollection<SubnetModel> Subnets
        {
            get => _subnets;
            private set => SetProperty(ref _subnets, value);
        }

        /// <summary> Gets the host amounts from the subnets and orders them in descending order. </summary>
        public List<int> HostAmounts => Subnets.Select(subnet => subnet.HostAmount).OrderByDescending(amount => amount).ToList();

        [RelayCommand]
        private void CalculateSubnets()
        {
            if (!IsValidInput())
                return;

            IVLSMCalculatorModel calculatorModel = new VLSMCalculatorModel();
            var subnets = calculatorModel.CalculateSubnets(Address, SubnetAmount, HostAmounts);
            Subnets.Clear();

            foreach (var subnet in subnets)
                Subnets.Add(subnet);
        }

        /// <summary> Checks if the input is valid. </summary>
        private bool IsValidInput()
        {
            return !(SubnetAmount <= 0 || string.IsNullOrEmpty(Address) || HostAmounts.Any(h => h <= 0));
        }

        /// <summary> Adds the subnets to the subnet grid. </summary>
        private void AddSubnets(int count)
        {
            Subnets.Clear();
            for (int i = 1; i <= count; i++)
                Subnets.Add(new SubnetModel(null, null, null, null, 0, 0));
        }
    }
}
