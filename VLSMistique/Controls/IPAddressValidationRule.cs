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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VLSMistique.Controls
{
    /// <summary> Validates IP address from string. </summary>
    public class IPAddressValidationRule : ValidationRule
    {
        /// <summary> Validates the amount of octets and size of each octet in the IP address. </summary>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string ipAddress)
            {
                Regex regex = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.(\d{1,3})$");
                Match match = regex.Match(ipAddress);
                if (!match.Success)
                    return new ValidationResult(false, "Invalid IP address.");

                foreach (var octetString in match.Groups[0].Value.Split('.'))
                {
                    if (!int.TryParse(octetString, out int octet) || octet > 255)
                        return new ValidationResult(false, "Invalid IP address. The octet " + octetString + " must be between 0 and 255.");
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
