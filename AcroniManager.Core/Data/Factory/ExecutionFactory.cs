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
    internal static class ExecutionFactory
    {
        internal static Execution GetExecution(ExecutionElement executionConfiguration, Crawler crawler)
        {
            IQueryable<Execution> executions = DataContext.Instance.ExecutionSet
                                                    .Where(e => e.Crawler.CrawlerId == crawler.CrawlerId
                                                                && e.LanguageCode == executionConfiguration.LanguageCode
                                                                && e.Parameters.Count == executionConfiguration.Parameters.Count
                                                                && e.Parameters.All(p => executionConfiguration.Parameters.AllKeys.Contains(p.Name)));

            Execution execution = null;

            foreach (Execution dbExecution in executions)
            {
                if (dbExecution.Parameters.All(p => executionConfiguration.Parameters[p.Name].Value == p.Value))
                {
                    execution = dbExecution;
                    break;
                }
            }

            bool pendingChanges = false;

            if (execution == null)
            {
                execution = DataContext.Instance.ExecutionSet.Add(new Execution 
                                                    {
                                                        Crawler = crawler,
                                                        LanguageCode = executionConfiguration.LanguageCode,
                                                        Description = executionConfiguration.Description
                                                    });

                foreach (NameValueConfigurationElement parameter in executionConfiguration.Parameters)
                {
                    execution.Parameters.Add(new Parameter { Name = parameter.Name, Value = parameter.Value });
                }

                pendingChanges = true;
            }
            else if (execution.Description != executionConfiguration.Description)
            {
                execution.Description = executionConfiguration.Description;
                pendingChanges = true;
            }

            if (pendingChanges)
            {
                DataContext.Instance.SaveChanges();
            }

            return execution;
        }
    }
}
