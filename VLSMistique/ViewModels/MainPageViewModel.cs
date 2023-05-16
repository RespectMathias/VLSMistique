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
using System.Threading;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Extensions;
using VLSMistique.Interfaces;
using VLSMistique.Models;
using System.Globalization;

namespace VLSMistique.ViewModels
{
    /// <summary> View model for the VLSM calculator. </summary>
    public partial class MainPageViewModel : ObservableRecipient
    {
        private string _address;
        private int _subnetAmount;
        private ObservableCollection<SubnetModel> _subnets = new ObservableCollection<SubnetModel>();
        private readonly IFileSaver _fileSaver = FileSaver.Default;
        private readonly IVLSMCalculatorModel _calculatorModel;
        private bool _validateInput;
        
        public bool ValidateInput
        {
            get => _validateInput;
            set => SetProperty(ref _validateInput, value);
        }

        /// <summary> Gets or sets the IP address. </summary>
        public string Address
        {
            get => _address;
            set
            {
                SetProperty(ref _address, value);
                UpdateValidateInput();
            } 
        }

        /// <summary> Gets the subnets. </summary>
        public ObservableCollection<SubnetModel> Subnets
        {
            get => _subnets;
            set => SetProperty(ref _subnets, value);
        }

        /// <summary> Gets or sets the amount of subnets. </summary>
        public int SubnetAmount
        {
            get => _subnetAmount;
            set
            {
                SetProperty(ref _subnetAmount, value);
                AddSubnets(value);
                UpdateValidateInput();
            }
        }

        public MainPageViewModel()
        {
            ValidateInput = false;
            _calculatorModel = new VLSMCalculatorModel() ?? throw new ArgumentNullException(nameof(_calculatorModel));
        }

        /// <summary> Calculates the Subnets. </summary>
        [RelayCommand]
        private void CalculateSubnets()
        {
            var hostAmounts = Subnets.Select(subnet => subnet.HostAmount).OrderByDescending(amount => amount).ToList();
            var newSubnets = _calculatorModel.CalculateSubnets(Address, SubnetAmount, hostAmounts);
            Subnets.Clear();

            foreach (var newSubnet in newSubnets)
            {
                newSubnet.HostAmountChanged += (s, e) => UpdateValidateInput();
                Subnets.Add(newSubnet);
            }
        }

        /// <summary> Validates the input for the calculation. </summary>
        private void UpdateValidateInput()
        {
            var isSubnetAmountValid = SubnetAmount > 0 && SubnetAmount < 100;
            var isAddressValid = !string.IsNullOrEmpty(Address) && Regex.IsMatch(Address, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.(\d{1,3})$");
            var areAllHostAmountsValid = Subnets.All(subnet => subnet.HostAmount > 0 && subnet.HostAmount < 255);

            ValidateInput = isSubnetAmountValid && isAddressValid && areAllHostAmountsValid;
        }


        /// <summary> Populates Subnets. </summary>
        private void AddSubnets(int count)
        {
            //Input limitations
            if (count <= 0 || count >= 100)
                return;

            Subnets.Clear();
            for (int i = 1; i <= count; i++)
            {
                var newSubnet = new SubnetModel(null, null, null, null, null, 0);
                newSubnet.HostAmountChanged += (s, e) => UpdateValidateInput();
                Subnets.Add(newSubnet);
            }
        }

        [RelayCommand]
        public async Task Export(CancellationToken cancellationToken)
        {
            string csvData = ConvertSubnetsToCsv();
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            try
            {
                //Variables
                var promptTitle = "Export Subnets";
                var promptMessage = "Enter a filename (CSV format):";
                var defaultFileName = $"Network_{DateTime.Now:yyyyMMHHmmss}";
                var fileExtension = ".csv";

                //User Input
                var fileNameInput = await Application.Current.MainPage.DisplayPromptAsync(promptTitle, promptMessage, cancel: "Cancel", initialValue: defaultFileName);

                //If user cancels
                if (string.Equals(fileNameInput?.Trim(), "Cancel", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(fileNameInput))
                    throw new Exception("Export cancelled by user.");

                var fileName = fileNameInput.Trim() + fileExtension;
                var fileLocationResult = await _fileSaver.SaveAsync(fileName, stream, cancellationToken);
                fileLocationResult.EnsureSuccess();

                await Toast.Make($"File is Exported: {fileLocationResult.FilePath}").Show(cancellationToken);
            }
            catch (Exception ex)
            {
                await Toast.Make($"File is not Exported, {ex.Message}").Show(cancellationToken);
                return;
            }
        }

        /// <summary> Turns List of Subnet information into a csv file. </summary>
        private string ConvertSubnetsToCsv()
        {
            StringBuilder csv = new();

            var listSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            var delimiter = listSeparator == "," ? "," : ";";

            csv.AppendLine($"Required Host{delimiter}Available Hosts{delimiter}Subnet Mask{delimiter}Network Address{delimiter}Range{delimiter}Broadcast Address");

            foreach (var subnet in Subnets)
            {
                csv.AppendLine($"{subnet.HostAmount}{delimiter}{subnet.MaxSubnetHosts}{delimiter}{subnet.Mask}{delimiter}{subnet.BroadcastAddress}{delimiter}{subnet.Range}{delimiter}{subnet.NetworkAddress}");
            }
            return csv.ToString();
        }
    }
}