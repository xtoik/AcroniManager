﻿/** Copyright 2014 Álvaro Rodríguez Otero and Álvaro Rodrigo Yuste 
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

using AcroniManager.Core.Information;

namespace AcroniManager.Leechers.Wikipedia
{
    internal class WikipediaCategory : ResourceCategory
    {
        internal static string CategoryNamePrefix;

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                if (value.StartsWith(CategoryNamePrefix))
                {
                    value = value.Substring(CategoryNamePrefix.Length);
                }
                base.Name = value;
            }
        }
    }
}
