﻿/*
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
using System.Windows;
using System.Threading;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using VLSMistique.Interfaces;
using VLSMistique.Models;
using VLSMistique.Services;

namespace VLSMistique.ViewModels
{
    /// <summary> View model for the main page. </summary>
    public partial class MainPageViewModel : ObservableRecipient
    {
        // Fields and dependencies

        private string _address;
        private int _subnetAmount;
        private bool _validateInput;
        private ObservableCollection<SubnetModel> _subnets = new();
        private readonly IFileSaver _fileSaver = FileSaver.Default;
        private readonly IVLSMCalculatorModel _calculatorModel;
        private readonly IInputValidator _inputValidator;
        private readonly ICsvConverter _csvConverter;

        /// <summary> Initializes a new instance of the <see cref="MainPageViewModel"/> class. </summary>
        public MainPageViewModel()
        {
            ValidateInput = false;
            _calculatorModel = new VLSMCalculatorModel(); // Instantiate the VLSM calculator model
            _inputValidator = new InputValidator(); // Instantiate the input validator
            _csvConverter = new CsvConverter(); // Instantiate the CSV converter
            for (int i = 1; i <= 3; i++)
                AddSubnets(); // Add initial subnet
        }

        // Properties

        /// <summary> Gets or sets a value indicating whether the input is valid. </summary>
        public bool ValidateInput
        {
            get => _validateInput;
            set => SetProperty(ref _validateInput, value);
        }

        /// <summary> Gets or sets the collection of subnets. </summary>
        public ObservableCollection<SubnetModel> Subnets
        {
            get => _subnets;
            set
            {
                SetProperty(ref _subnets, value);
                UpdateValidateInput(); // Update the validation flag
            }
        }

        /// <summary> Gets or sets the address. </summary>
        public string Address
        {
            get => _address;
            set
            {
                SetProperty(ref _address, value); 
                UpdateValidateInput(); // Update the validation flag
            } 
        }

        /// <summary> Gets or sets the SubnetAmount . </summary>
        public int SubnetAmount
        {
            get => _subnetAmount;
            set
            {
                if (value >= 0 && SetProperty(ref _subnetAmount, value)) // Ensure positive value and changed
                {
                    var difference = value - Subnets.Count;
                    if (difference > 0)
                        for (int i = 0; i < difference; i++) // If the new SubnetAmount is greater than the current count, add subnets
                            AddSubnets();
                    else if (difference < 0)  
                        for (int i = 0; i < -difference; i++) // If the new SubnetAmount is less than the current count, remove subnets
                            RemoveSubnets();
                }
            }
        }

        #region Commands

        /// <summary> Exports the subnets to a CSV file. </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [RelayCommand]
        public async Task Export(CancellationToken cancellationToken)
        {
            // Convert subnets to CSV data
            string csvData = _csvConverter.Convert(Subnets);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            try
            {
                var promptTitle = "Export Subnets";
                var promptMessage = "Enter a filename (CSV format):";
                var defaultFileName = $"Network_{DateTime.Now:yyyyMMHHmmss}";
                var fileExtension = ".csv";

                // Prompt user for the filename
                var fileNameInput = await Application.Current.MainPage.DisplayPromptAsync(promptTitle, promptMessage, cancel: "Cancel", initialValue: defaultFileName);

                // Handle cancel and empty filename
                if (string.Equals(fileNameInput?.Trim(), "Cancel", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(fileNameInput))
                    throw new Exception("Export cancelled by user.");

                var fileName = $"{fileNameInput.Trim()}{fileExtension}";
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

        /// <summary> Calculates the subnets based on the host amounts. </summary>
        [RelayCommand]
        public void CalculateSubnets()
        {
            var hostAmounts = Subnets.Select(subnet => subnet.HostAmount).OrderByDescending(amount => amount).ToList();
            var newSubnets = _calculatorModel.CalculateSubnets(Address, Subnets.Count, hostAmounts);

            if (newSubnets.Count == Subnets.Count)
            {
                for (int i = 0; i < Subnets.Count; i++)
                {
                    Subnets[i] = newSubnets[i];
                    Subnets[i].HostAmountChanged += (s, e) => UpdateValidateInput(); // Update the validation flag for HostAmount
                }
                UpdateValidateInput(); // Update the validation flag
            }
        }

        /// <summary> Removes subnets. </summary>
        [RelayCommand]
        public void RemoveSubnets()
        {
            if (Subnets.Count > 0)
            {
                Subnets.RemoveAt(Subnets.Count - 1);
                SubnetAmount = Subnets.Count; // Update Subnet Amount
                UpdateValidateInput(); // Update the validation flag
            }
        }

        /// <summary> Adds subnets. </summary>
        [RelayCommand]
        private void AddSubnets()
        {
            var newSubnet = new SubnetModel(null, null, null, null, null, 0);
            newSubnet.HostAmountChanged += (s, e) => UpdateValidateInput(); // Update the validation flag for HostAmount
            Subnets.Add(newSubnet);
            SubnetAmount = Subnets.Count; // Update Subnet Amount
            UpdateValidateInput(); // Update the validation flag
        }

        #endregion Commands

        #region Private methods

        /// <summary> Updates the validation flag. </summary>
        private void UpdateValidateInput()
        {
            ValidateInput = _inputValidator.Validate(Address, Subnets.Count, Subnets);
        }

        #endregion Private methods
    }
}