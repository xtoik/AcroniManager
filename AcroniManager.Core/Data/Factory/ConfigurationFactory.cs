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
    internal static class ConfigurationFactory
    {
        internal static Configuration GetConfiguration(ParameterizableConfigurationElement patternConfiguration, Matcher matcher)
        {
            if (patternConfiguration == null)
            {
                patternConfiguration = new ParameterizableConfigurationElement();
            }

            IQueryable<Configuration> configurations = DataContext.Instance.ConfigurationSet
                                                        .Where(c => c.Matcher.MatcherId == matcher.MatcherId
                                                                    && c.Parameters.Count == patternConfiguration.Parameters.Count
                                                                    && c.Parameters.All(p => patternConfiguration.Parameters.AllKeys.Contains(p.Name)));

            Configuration configuration = null;

            foreach (Configuration dbConfiguration in configurations)
            {
                if (dbConfiguration.Parameters.All(p => patternConfiguration.Parameters[p.Name].Value == p.Value))
                {
                    configuration = dbConfiguration;
                    break;
                }
            }

            bool pendingChanges = false;

            if (configuration == null)
            {
                configuration = DataContext.Instance.ConfigurationSet.Add(new Configuration
                                                                            {  
                                                                                Matcher = matcher,
                                                                                Description = patternConfiguration.Description
                                                                            });

                foreach (NameValueConfigurationElement parameter in patternConfiguration.Parameters)
                {
                    configuration.Parameters.Add(new Parameter { Name = parameter.Name, Value = parameter.Value });
                }

                pendingChanges = true;
            }
            else if (configuration.Description != patternConfiguration.Description)
            {
                configuration.Description = patternConfiguration.Description;
                pendingChanges = true;
            }

            if (pendingChanges)
            {
                DataContext.Instance.SaveChanges();
            }

            return configuration;
        }
    }
}
