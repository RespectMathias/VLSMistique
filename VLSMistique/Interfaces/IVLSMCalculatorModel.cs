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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VLSMistique.Models;

namespace VLSMistique.Interfaces
{
    /// <summary> Interface for the VLSM calculator model. </summary>
    public interface IVLSMCalculatorModel
    {
        /// <summary> Calculates the subnets based on the given IP addres, the amont of subnets, and a list of host per subnet. </summary>
        List<SubnetModel> CalculateSubnets(string ipAddress, int subnetAmount, List<int> hostAmounts);
    }
}
