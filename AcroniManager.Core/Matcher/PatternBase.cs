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

using AcroniManager.Core.Common;
using AcroniManager.Core.Configuration;
using AcroniManager.Core.Information;
using AcroniManager.Core.Matcher;
using System.Collections.Generic;

namespace AcroniManager.Core.Pattern
{
    public abstract class PatternBase : ConfigurableBase
    {
        #region Protected Internal Methods

        public abstract IEnumerable<FoundMatchBase> FindMatches(ResourceInformation information);

        #endregion Protected Internal Methods

        #region Properties

        internal Data.Configuration Configuration { get; set; }

        protected virtual string AcronymRegularExpression
        {
            get
            {
                return AcroniManagerConfigurationSection.Instance.AcronymRegularExpression;
            }
        }

        protected virtual string AcronymGroupName
        {
            get
            {
                return AcroniManagerConfigurationSection.Instance.AcronymGroupName;
            }
        }

        #endregion Properties
    }
}
