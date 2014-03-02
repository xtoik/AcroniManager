/** Copyright 2014 Álvaro Rodríguez Otero and Álvaro Rodrigo Yuste 
*
* Licensed under the EUPL, Version 1.1 or – as soon they will be
* approved by the European Commission – subsequent versions of the
* EUPL (the "Licence");* you may not use this work except in
* compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://www.osor.eu/eupl/european-union-public-licence-eupl-v.1.1
*
* Unless required by applicable law or agreed to in writing,
* software distributed under the Licence is distributed on an "AS
* IS" BASIS, * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
* express or implied.
* See the Licence for the specific language governing permissions
* and limitations under the Licence.
*/

using AcroniManager.Core.Data;
using AcroniManager.Core.MeaningSelector;
using System.Linq;

namespace AcroniManager.MeaningSelectors.Rank
{
    public class RankedSelectedMeaning : SelectedMeaningBase
    {
        public RankedSelectedMeaning(Meaning meaning) : base(meaning.Caption)
        {
            Meaning = meaning;
        }

        public double Score { get; internal set; }

        public Meaning Meaning { get; private set; }

        public int ValidationsCount 
        {
            get
            {
                return Meaning.Validations.Count(y => y.Validated);
            }
        }

        public int ResourcesCount
        {
            get
            {
                return Meaning.Resources.Count();
            }
        }

        public int PatternsCount
        {
            get
            {
                return Meaning.Configurations.Count();
            }
        }
    }
}
