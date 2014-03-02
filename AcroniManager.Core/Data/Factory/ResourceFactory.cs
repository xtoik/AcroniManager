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

using System.Linq;

namespace AcroniManager.Core.Data.Factory
{
    internal static class ResourceFactory
    {
        internal static Resource GetResource(string resourceKey, Execution execution)
        {
            Resource resource = DataContext.Instance.ResourceSet
                                    .FirstOrDefault(r => r.ResourceKey == resourceKey);
            
            bool pendingChanges = false;

            if (resource == null)
            {
                resource = DataContext.Instance.ResourceSet.Add(new Resource { ResourceKey = resourceKey });
                pendingChanges = true;                
            }

            if (pendingChanges
                || !resource.Executions.Contains(execution))
            {
                resource.Executions.Add(execution);
                pendingChanges = true;
            }

            if (pendingChanges)
            {
                DataContext.Instance.SaveChanges();
            }

            return resource;
        }
    }
}
