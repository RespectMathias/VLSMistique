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
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using VLSMCalculator.Interfaces;
using VLSMCalculator.Model;

namespace VLSMCalculator.ViewModel
{
    /// <summary> View model for the VLSM calculator. </summary>
    public class VLSMCalculatorViewModel : INotifyPropertyChanged
    {
        private string _ipAddress;
        private int _subnetAmount;
        private ObservableCollection<SubnetModel> _subnets;

        /// <summary> Gets the IPAddress and checks if its changed. </summary>
        public string IPAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets the SubnetAmount and checks if its changed. </summary>
        public int SubnetAmount
        {
            get => _subnetAmount;
            set
            {
                _subnetAmount = value;
                OnPropertyChanged();
                AddSubnets(value);
            }
        }

        /// <summary> Gets the HostAmount from the subnet grid and orders it in decending order. </summary>
        public List<int> HostAmounts
        {
            get => Subnets.Select(subnet => subnet.HostAmount).OrderByDescending(amount => amount).ToList();
        }

        /// <summary> Gets the Subnets and checks if its changed. </summary>
        public ObservableCollection<SubnetModel> Subnets
        {
            get => _subnets;
            set
            {
                _subnets = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets the command to calculate the subnets from the view. </summary>
        public ICommand CalculateSubnetsCommand { get; }

        /// <summary> Constructor. </summary>
        public VLSMCalculatorViewModel()
        {
            _subnets = new ObservableCollection<SubnetModel>();
            CalculateSubnetsCommand = new RelayCommand(CalculateSubnets);
        }

        /// <summary> Checks if the input is valid. </summary>
        private bool isValidInput()
        {
            if (SubnetAmount <= 0 || string.IsNullOrEmpty(IPAddress) || HostAmounts.Any(h => h <= 0))
                return false;
            else
                return true;
        }

        /// <summary> Adds the subnets to the subnet grid. </summary>
        private void AddSubnets(int count)
        {
            Subnets.Clear();
            for (int i = 1; i <= count; i++)
                Subnets.Add(new SubnetModel(null, null, null, null, 0, 0));
        }

        /// <summary> Calculates the subnets using the model. </summary>
        private void CalculateSubnets()
        {
            if (!isValidInput())
                return;

            IVLSMCalculatorModel calculatorModel = new VLSMCalculatorModel();
            var subnets = calculatorModel.CalculateSubnets(IPAddress, SubnetAmount, HostAmounts);
            Subnets.Clear();

            foreach (var subnet in subnets)
                Subnets.Add(subnet);
        }

        /// <summary> Event handler for the PropertyChanged event. Used to notify the view of changes. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> The method that notifies the view of changes. </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}