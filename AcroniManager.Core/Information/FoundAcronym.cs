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
namespace AcroniManager.Core.Information
{
    public class FoundAcronym
    {
        private SelectedMeaningBase _meaning;

        public int Index { get; internal set; }
        public string Caption { get; internal set; }
        public bool IsMeaningSet { get; private set; } 
        public Acronym Acronym { get; internal set; }
        
        public SelectedMeaningBase Meaning 
        {
            get
            {
                return _meaning;
            }

            set
            {
                IsMeaningSet = true;
                _meaning = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Caption, Index);
        }
    }
}
