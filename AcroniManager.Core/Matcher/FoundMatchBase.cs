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
using AcroniManager.Core.Information;
using System.Collections.Generic;

namespace AcroniManager.Core.Matcher
{
    public class FoundMatchBase
    {
        #region Internal Members

        internal Meaning Meaning { get; set; }

        #endregion Internal Members

        #region Public members

        public string Match { get; set; }
        public string Definition { get; set; }
        public List<ResourceCategory> Categories = new List<ResourceCategory>();

        public override string ToString()
        {
            return string.Format("\"{0}\": \"{1}\".", Match, Definition);
        }

        #endregion Public members
    }
}
