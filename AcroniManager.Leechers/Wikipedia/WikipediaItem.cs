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

using AcroniManager.Core.Leecher;
using LinqToWiki.Generated;
using LinqToWiki.Generated.Entities;
using System.Collections.Generic;

namespace AcroniManager.Leechers.Wikipedia
{
    public class WikipediaItem : LeechedResourceBase
    {
        public WikipediaItem() { }

        public WikipediaItem(categorymembersSelect cm, WikipediaItem parent)
        {            
            PageId = cm.pageid;
            Type = cm.type.Value;
            Title = cm.title;
            ParentItem = parent;
        }

        public WikipediaItem(infoResult page, WikipediaItem parent)
        {
            PageId = page.pageid.GetValueOrDefault();
            Type = categorymemberstype.page.Value;
            Title = page.title;
            ParentItem = parent;
        }

        public long PageId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public long Category { get; set; }
        public WikipediaItem ParentItem { get; set; }

        #region Internal 

        #endregion Internal

        internal int Depth
        {
            get
            {
                WikipediaItem parentItem = this;
                int depth = 0;
                do
                {
                    parentItem = parentItem.ParentItem;
                    depth++;
                } while (parentItem != null
                         && parentItem.Type == "page");
                return depth;
            }
        }

        internal List<string> Links { get; set; }

        #region Overrides

        public override string ResourceKey { get { return PageId.ToString(); } }

        public override string ToString() { return Title; }

        #endregion Overrides
    }
}
