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

using AcroniManager.Core.Configuration;
using System.Configuration;
using System.Linq;

namespace AcroniManager.Core.Data.Factory
{
    internal static class ArrangementFactory
    {
        internal static Arrangement GetArrangement(ParameterizableConfigurationElement checkerConfiguration, Validator validator)
        {
            if (checkerConfiguration == null)
            {
                checkerConfiguration = new ParameterizableConfigurationElement();
            }

            IQueryable<Arrangement> arrangements = DataContext.Instance.ArrangementSet
                                                              .Where(c => c.Validator.ValidatorId == validator.ValidatorId
                                                                          && c.Parameters.Count == checkerConfiguration.Parameters.Count
                                                                          && c.Parameters.All(p => checkerConfiguration.Parameters.AllKeys.Contains(p.Name)));

            Arrangement arrangement = null;

            foreach (Arrangement dbArrangement in arrangements)
            {
                if (dbArrangement.Parameters.All(p => checkerConfiguration.Parameters[p.Name].Value == p.Value))
                {
                    arrangement = dbArrangement;
                    break;
                }
            }

            bool pendingChanges = false;

            if (arrangement == null)
            {
                arrangement = DataContext.Instance.ArrangementSet.Add(new Arrangement
                                                                            {  
                                                                                Validator = validator,
                                                                                Description = checkerConfiguration.Description
                                                                            });

                foreach (NameValueConfigurationElement parameter in checkerConfiguration.Parameters)
                {
                    arrangement.Parameters.Add(new Parameter { Name = parameter.Name, Value = parameter.Value });
                }

                pendingChanges = true;
            }
            else if (arrangement.Description != checkerConfiguration.Description)
            {
                arrangement.Description = checkerConfiguration.Description;
                pendingChanges = true;
            }

            if (pendingChanges)
            {
                DataContext.Instance.SaveChanges();
            }

            return arrangement;
        }
    }
}
