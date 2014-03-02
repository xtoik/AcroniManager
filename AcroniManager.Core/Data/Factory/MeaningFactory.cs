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

using AcroniManager.Core.Information;
using System.Collections.Generic;
using System.Linq;

namespace AcroniManager.Core.Data.Factory
{
    public delegate void AcronymCreatedEventHandler(string acronym);
    public delegate void MeaningCreatedEventHandler(string acronym, string meaning);

    internal class MeaningFactory
    {
        internal static event AcronymCreatedEventHandler AcronymCreated;
        internal static event MeaningCreatedEventHandler MeaningCreated;

        internal static Meaning GetMeaning(string match, string definition, List<ResourceCategory> categories)
        {
            Acronym acronym = DataContext.Instance.AcronymSet.FirstOrDefault(a => a.Caption == match);

            bool pendingChanges = false;

            if (acronym == null)
            {
                acronym = DataContext.Instance.AcronymSet.Add(new Acronym { Caption = match });
                if (AcronymCreated != null) AcronymCreated(acronym.Caption);
                pendingChanges = true;
            }

            Meaning meaning = acronym.Meanings.FirstOrDefault(m => m.Caption.ToUpper() == definition.ToUpper());
            
            if (meaning == null)
            {
                meaning = acronym.Meanings.FirstOrDefault(m => definition.ToUpper().Contains(m.Caption.ToUpper()));
            }

            if (meaning == null)
            {
                meaning = acronym.Meanings.FirstOrDefault(m => m.Caption.ToUpper().Contains(definition.ToUpper()));
                if (meaning != null)
                {
                    meaning.Caption = definition;
                    pendingChanges = true;
                }
            }

            if (meaning == null)
            {
                meaning = DataContext.Instance.MeaningSet.Add(new Meaning { Caption = definition, Acronym = acronym });
                if (MeaningCreated != null) MeaningCreated(acronym.Caption, meaning.Caption);
                pendingChanges = true;
            }

            foreach (ResourceCategory resourceCategory in categories)
            {
                resourceCategory.Category = DataContext.Instance.CategorySet
                                                       .FirstOrDefault(c => c.Caption.ToUpper() == resourceCategory.Name.ToUpper());
                if (resourceCategory.Category == null)
                {
                    resourceCategory.Category = DataContext.Instance.CategorySet.Add(new Category { Caption = resourceCategory.Name });
                    pendingChanges = true;
                }

                if (!meaning.Categories.Contains(resourceCategory.Category))
                {
                    meaning.Categories.Add(resourceCategory.Category);
                    pendingChanges = true;
                }
            }

            if (pendingChanges)
            {
                DataContext.Instance.SaveChanges();
            }

            return meaning;
        }
    }
}
