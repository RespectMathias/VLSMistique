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

namespace VLSMCalculator.Validation
{
    /// <summary> Validates the amount of subnets. </summary>
    public class MaxSubnetsValidationRule : ValidationRule
    {
        /// <summary> Defines the maximum amount of subnets allowed. Is set in the view. </summary>
        public int MaxSubnets { get; set; }

        /// <summary> Validates if the amount of subnets exceeds the maximum allowed. </summary>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if ((int.TryParse(value?.ToString(), out int subnet) && subnet > MaxSubnets))
                return new ValidationResult(false, $"The maximum number of subnets allowed is {MaxSubnets}.");

            return ValidationResult.ValidResult;
        }
    }
}
