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
using System;
using System.Linq;

namespace AcroniManager.Core.Data.Factory
{
    internal static class CrawlerFactory
    {
        internal static Crawler GetCrawler(LeecherElement leecherConfiguration)
        {
            Crawler crawler = DataContext.Instance.CrawlerSet.FirstOrDefault(c => c.ClassName == leecherConfiguration.Class);
            Boolean pendingChanges = false;

            Source source = getSource(leecherConfiguration.Source, ref pendingChanges);

            if (crawler == null)
            {
                crawler = DataContext.Instance.CrawlerSet.Add(new Crawler { ClassName = leecherConfiguration.Class, Source = source });
                pendingChanges = true;
            }
            else if (!crawler.Source.Name.Equals(leecherConfiguration.Source, StringComparison.InvariantCultureIgnoreCase))
            {
                crawler.Source = source;
                pendingChanges = true;
            }

            if (pendingChanges)
            {
                DataContext.Instance.SaveChanges();
            }

            return crawler;
        }

        private static Source getSource(string name, ref Boolean pendingChanges)
        {
            Source source = DataContext.Instance.SourceSet.FirstOrDefault(s => s.Name == name);

            if (source == null)
            {
                source = new Source { Name = name };
                DataContext.Instance.SourceSet.Add(source);
                DataContext.Instance.SaveChanges();
                pendingChanges = true;
            }

            return source;
        }
    }
}
