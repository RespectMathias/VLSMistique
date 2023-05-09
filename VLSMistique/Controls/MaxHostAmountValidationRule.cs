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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VLSMistique.Controls
{
    /// <summary> Validates if hosts exceed the subnet. </summary>
    public class MaxHostAmountValidationRule : ValidationRule
    {
        /// <summary> Defines the maximum number of hosts per subnet allowed. Is set in the view. </summary>
        public int MaxHostAmount { get; set; }

        /// <summary> Validates if the required number of hosts exceeds the maximum number of hosts per subnet allowed. </summary>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if ((int.TryParse(value?.ToString(), out int hostAmount)) && hostAmount > MaxHostAmount)
                return new ValidationResult(false, $"The maximum number of hosts per subnet allowed is {MaxHostAmount}.");

            return ValidationResult.ValidResult;
        }
    }
}
