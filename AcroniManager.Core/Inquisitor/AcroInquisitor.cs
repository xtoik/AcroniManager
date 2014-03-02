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

using AcroniManager.Core.Common;
using AcroniManager.Core.Configuration;
using AcroniManager.Core.Data;
using AcroniManager.Core.Information;
using AcroniManager.Core.MeaningSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AcroniManager.Core.Inquisitor
{
    public class AcroInquisitor : IDisposable
    {
        private MeaningSelectorBase _meaningSelector;

        private MeaningSelectorBase MeaningSelector
        {
            get
            {
                if (_meaningSelector == null)
                {
                    _meaningSelector = ConfigurableBase
                                        .CreateConfigurableItem<MeaningSelectorBase>(
                                            ConfigurableBase.GetType(AcroniManagerConfigurationSection.Instance.MeaningSelector.Class,
                                                                     typeof(MeaningSelectorBase),
                                                                     "meaningSelector"),
                                            AcroniManagerConfigurationSection.Instance.MeaningSelector.Parameters);
                }
                return _meaningSelector;
            }
        }

        public AcroInquisitor()
        {
            DataContext.Instance = new AcroniManagerDatabaseModelContainerWrapped();
        }

        public void Dispose()
        {
            DataContext.Instance.Dispose();
            DataContext.Instance = null;
        }

        public IQueryable<Acronym> SearchAcronyms(string acronymCaption)
        {
            return DataContext.Instance.AcronymSet.Where(x => x.Caption.StartsWith(acronymCaption));
        }

        public IEnumerable<FoundAcronym> MatchAcronyms(string text)
        {
            foreach (Match match in Regex.Matches(text, AcroniManagerConfigurationSection.Instance.AcronymRegularExpression))
            {
                yield return new FoundAcronym 
                                    {
                                        Index = match.Index,
                                        Caption = match.Groups[AcroniManagerConfigurationSection.Instance.AcronymGroupName].Value
                                    };
            }
        }

        public SelectedMeaningBase GetAcronymMeaning(FoundAcronym foundAcronym, string wholeText)
        {
            foundAcronym.Acronym = DataContext.Instance.AcronymSet.FirstOrDefault(x => x.Caption == foundAcronym.Caption.Replace(".", string.Empty));
            return foundAcronym.Acronym == null ? null : MeaningSelector.SelectMeaning(foundAcronym, wholeText);
        }
    }
}