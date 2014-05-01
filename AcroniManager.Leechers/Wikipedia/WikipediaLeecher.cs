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
using AcroniManager.Core.Leecher;
using LinqToWiki;
using LinqToWiki.Generated;
using LinqToWiki.Generated.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AcroniManager.Leechers.Wikipedia
{
    public enum ConfigurationParameter
    {
        InitialCategory,
        WikipediaUrl,
        UserAgent,
        MaximumDepth,
        CleanContents,
        FileTag,
        QuoteTag,
        RedirectTag,
        CategoryNamePrefix,
        BrowsableCategories
    }

    public class WikipediaLeecher : LeecherBase
    {
        #region Fields

        private Wiki _wiki;
        private string _initialCategory;
        private string _wikipediaUrl;
        private string _userAgent;
        private List<WikipediaItem> browsedCategories;
        private int _maximumDepth = 1;
        private bool _cleanContents = true;
        private string _fileTag = "File";
        private string _quoteTag = "quote";
        private string _redirectTag = "Redirect";
        private List<string> _browsableCatergories = new List<string>();

        #endregion Fields

        #region Private Helpers

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void initialize()
        {
            if (_wiki == null)
            {
                _wiki = new Wiki(_userAgent, _wikipediaUrl);
            }
            browsedCategories = new List<WikipediaItem>();
        }

        private bool categoryNameBrowsable(string categoryName)
        {
            return !_browsableCatergories.Any()  
                   || _browsableCatergories.Any(bcn => categoryName.ToUpperInvariant().Contains(bcn));
        }

        private IEnumerable<LeechedResourceBase> leechPages(PagesSource<Page> pagesSource, WikipediaItem parent)
        {
            IEnumerator<infoResult> pagesEnumerator = null;
            try
            {
                pagesEnumerator = pagesSource.Select(p => p.info).ToEnumerable().GetEnumerator();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception encountered enumerating the pages of \"{0}\": {1}", parent.Title, ex);
            }

            if (pagesEnumerator != null)
            {
                infoResult page = null;
                do
                {
                    try
                    {
                        if (!pagesEnumerator.MoveNext())
                        {
                            page = null;
                        }
                        else
                        {
                            page = pagesEnumerator.Current;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception encountered leeching the childs of \"{0}\": {1}", parent.Title, ex);
                    }
                    if (page != null)
                    {
                        WikipediaItem pageItem = new WikipediaItem(page, parent);
                        yield return pageItem;

                        if (pageItem.Links != null
                            && pageItem.Links.Count > 0)
                        {
                            foreach (string link in pageItem.Links)
                            {
                                IEnumerator<LeechedResourceBase> childPagesEnumerator = null;
                                try
                                {
                                    var childPages = _wiki.Query.allpages()
                                                                .Where(c => c.from == link
                                                                            && c.to == link)
                                                                .Pages;

                                    childPagesEnumerator = leechPages(childPages, pageItem).GetEnumerator();
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("Exception encountered enumerating the link \"{2}\" of \"{0}\": {1}", pageItem.Title, ex, link);
                                }

                                if (childPagesEnumerator != null)
                                {
                                    LeechedResourceBase childPage = null;

                                    do
                                    {
                                        try
                                        {
                                            if (!childPagesEnumerator.MoveNext())
                                            {
                                                childPage = null;
                                            }
                                            else
                                            {
                                                childPage = childPagesEnumerator.Current;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("Exception encountered leeching the link \"{2}\" of \"{0}\": {1}", pageItem.Title, ex, link);
                                        }
                                        if (childPage != null)
                                        {
                                            yield return childPage;
                                        }
                                    } while (childPage != null);
                                }
                            }
                        }
                    }
                } while (page != null);
            }
        }

        private IEnumerable<LeechedResourceBase> leechContents(WikipediaItem parent)
        {
            IEnumerator<LeechedResourceBase> pagesEnumerator = null;
            try
            {
                var pages = _wiki.Query.categorymembers()
                             .Where(c => c.title == parent.Title
                                         && c.type == categorymemberstype.page)
                             .Pages;

                pagesEnumerator = leechPages(pages, parent).GetEnumerator();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception encountered enumerating the members of \"{0}\": {1}", parent.Title, ex);
            }

            if (pagesEnumerator != null)
            {
                LeechedResourceBase page = null;

                do
                {
                    try
                    {
                        page = pagesEnumerator.MoveNext() ? pagesEnumerator.Current : null;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception encountered leeching the members of \"{0}\": {1}", parent.Title, ex);
                    }
                    if (page != null)
                    {
                        yield return page;
                    }
                } while (page != null);
            }

            IEnumerator<categorymembersSelect> categoriesEnumerator = null;

            try
            {
                categoriesEnumerator = _wiki.Query.categorymembers()
                                                  .Where(c => c.title == parent.Title && c.type == categorymemberstype.subcat)
                                                  .ToEnumerable()
                                                  .GetEnumerator();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception encountered enumerating the subcategories of \"{0}\": {1}", parent.Title, ex);
            }

            if (categoriesEnumerator != null)
            {
                categorymembersSelect category = null;

                do 
                {
                    try
                    {
                        category = categoriesEnumerator.MoveNext() ? categoriesEnumerator.Current : null;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception encountered leeching the subcategories of \"{0}\": {1}", parent.Title, ex);
                    }

                    if (category != null)
                    {
                        WikipediaItem categoryItem = new WikipediaItem(category, parent);
                        if (categoryNameBrowsable(categoryItem.Title)
                            && browsedCategories.All(bc => bc.PageId != categoryItem.PageId))
                        {
                            browsedCategories.Add(categoryItem);
                            foreach (LeechedResourceBase leechedResource in leechContents(categoryItem))
                            {
                                yield return leechedResource;
                            }                        
                        }
                    }
                } while (category != null);
            }
        }

        private ResourceInformation cleanContent(ResourceInformation dirtyContent)
        {
            if (_cleanContents)
            {
                try
                {
                    // remove all the comments
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"<!--.*-->",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // remove tables
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\|.*\|\}",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // remove all the references
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"<ref[^>]*>(?:[^<]*</ref>)?",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep only the text for all the links not archives:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\[\[(?!" + _fileTag + @":)(?:[^\|\]]*\|)*(?<text>[^\]]*)\]\]",
                                                         "${text}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // remove the rest of the links:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\[\[[^\]]*\]\]",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep only the text for the textual quotes:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\{" + _quoteTag + @"\|(?<text>[^\}\|]*)(?:\|[^\}]*)*\}\}",
                                                         "${text}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // delete the rest of the quotes: 
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\{" + _quoteTag + @"[^\|]*(?:\|[^\}]*)\}\}",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep the text of the ord templates:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\{ord\|(?<text1>[^\|\}]*)(?:\|(?<text2>[^\}]*))?\}\}",
                                                         "${text1}${text2}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep the text of the overline templates:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\{overline\|(?<text>[^\}]*)\}\}",
                                                         "${text}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep the text of the generic lang templates:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\{lang\|[^\|]*\|(?<text>[^\}]*)\}\}",
                                                         "${text}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep the text of the culture-specific lang templates:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\{lang\-[^\|]*\|(?<text>[^\}]*)\}\}",
                                                         "${text}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep the text of the Commons links templates:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\{commons[^\|\}]*(?:\|(?<text>[^\|\}]*)(?:\|[^\}\|]*)*)?\}\}",
                                                         "${text}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep the text of the news links templates:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\{iprnoticias[^\|\}]*(?:\|[^\|\}]*)?\|(?<text>[^\|\}]*)\}\}",
                                                         "${text}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // remove the rest of templates
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\{\{.*\}\}",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep the text of the external links:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\[\w+:\/\/[\w@][\w.:@]+\/?[\w\.~?=%&#=\-:@/$,]*(?:\s+(?<text>[^\]]*))?\]",
                                                         "${text}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // remove the URLs
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\w+:\/\/[\w@][\w.:@]+\/?[\w\.~?=%&#=\-:@/$,]*",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // keep just the text in the redirections:
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"#" + _redirectTag + @"\w*\s*(?<text>[^#\r]*)(?:#.*)?",
                                                         "${text}", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));

                    // reformat titles
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"=====\s*(?<text>[^\r=]*)\s*=====",
                                                         "${text}", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"====\s*(?<text>[^\r=]*)\s*====",
                                                         "${text}", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"===\s*(?<text>[^\r=]*)\s*===",
                                                         "${text}", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"==\s*(?<text>[^\r=]*)\s*==",
                                                         "${text}", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"=\s*(?<text>[^\r=]*)\s*=",
                                                         "${text}", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));

                    // remove triple quotes
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"'''",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // remove double quotes
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"''",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));                    

                    // remove latex mathfrak function keeping the text parameter
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\\mathfrak\{(?<text>[^\}]+)\}",
                                                         "${text}", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // remove all html tags
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"\<\/?[^\>]+\>",
                                                         string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline,
                                                         new TimeSpan(0, 0, 60));

                    // special characters
                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"&nbsp;",
                                                         " ", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));

                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"&mdash;",
                                                         "-", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));

                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"&amp;",
                                                         "&", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));

                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"&gt;",
                                                         ">", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));

                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"&lt;",
                                                         "<", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));

                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"&oplus;",
                                                         "⊕", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));

                    dirtyContent.Content = Regex.Replace(dirtyContent.Content, @"&times;",
                                                         "×", RegexOptions.IgnoreCase,
                                                         new TimeSpan(0, 0, 60));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error occurred cleaning the contents of the resource: {0}{1} * Resource Content: {2}",
                                    ex,
                                    Environment.NewLine,
                                    dirtyContent.Content);
                }
            }

            return dirtyContent;
        }

        private WikipediaResourceInformation extractResourceInformation(WikipediaItem resource, PagesSource<Page> page)
        {
            WikipediaResourceInformation ret;
            try
            {
                ret = page.Select(p => new WikipediaResourceInformation
                                            {
                                                Content = resource.Title + Environment.NewLine
                                                            + p.revisions().Select(r => r.value).ToEnumerable().FirstOrDefault(),

                                                Categories = p.categories().Where(c => c.show == categoriesshow.not_hidden)
                                                                            .Select(c => new WikipediaCategory { Name = c.title })
                                                                            .ToList().Cast<ResourceCategory>().ToList(),

                                                Links = resource.Depth < _maximumDepth
                                                            ? p.links().Where(l => l.ns == Namespace.Article).Select(l => l.title).ToList()
                                                            : new List<string>()
                                            })
                                .ToEnumerable().FirstOrDefault();

                if (ret != null && (ret.Categories == null || ret.Categories.Count == 0))
                {
                    ret.Categories = new List<ResourceCategory> { new WikipediaCategory { Name = resource.ParentItem.Title } };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occurred extracting the Resource Information: {0}{1} * Resource Title: {2}", 
                                ex,
                                Environment.NewLine,
                                resource.Title);
                ret = null;
            }

            return ret;
        }

        #endregion Private Helpers

        #region Overrides

        protected override List<string> RequiredParameterNames
        {
            get
            {
                return new List<string>
                            {
                                ConfigurationParameter.InitialCategory.ToString(),
                                ConfigurationParameter.UserAgent.ToString(),
                                ConfigurationParameter.WikipediaUrl.ToString(),
                                ConfigurationParameter.CategoryNamePrefix.ToString()
                            };
            }
        }

        protected override IEnumerable<LeechedResourceBase> LeechedResources
        {
            get 
            {
                initialize();

                var parent = new WikipediaItem
                {
                    Type = "supercat",
                    Title = _initialCategory
                };

                return leechContents(parent);
            }
        }        

        protected override ResourceInformation GetResourceInformation(LeechedResourceBase leechedResource)
        {
            WikipediaItem resource = leechedResource as WikipediaItem;

            WikipediaResourceInformation ret = extractResourceInformation(resource, 
                                                                          _wiki.Query
                                                                               .allpages()
                                                                               .Where(p => p.from == resource.Title 
                                                                                                     && p.to == resource.Title).Pages);

            // it´s not a page, so try with a different search mode
            if (ret == null)
            {
                ret = extractResourceInformation(resource, 
                                                 _wiki.Query.search(resource.Title).Pages);                                 
            }

            // failsafe! if cannot get the resource info form the wiki, make it up with what we have already.
            if (ret == null)
            {
                ret = new WikipediaResourceInformation
                {
                    Content = resource.Title,
                    Categories = new List<ResourceCategory> { new ResourceCategory { Name = resource.ParentItem.Title } }
                };
            }            

            resource.Links = ret.Links;

            return cleanContent(ret);
        }

        protected override void SetParameter(string name, string value)
        {
            ConfigurationParameter parameter;
            if (!Enum.TryParse<ConfigurationParameter>(name, true, out parameter))
            {
                base.SetParameter(name, value);
            }
            else
            {
                switch (parameter)
                {
                    case ConfigurationParameter.InitialCategory:
                        _initialCategory = CheckIfNotEmpty(name, value);
                        break;
                    case ConfigurationParameter.WikipediaUrl:
                        _wikipediaUrl = CheckIfNotEmpty(name, value);
                        break;
                    case ConfigurationParameter.UserAgent:
                        _userAgent = CheckIfNotEmpty(name, value);
                        break;
                    case ConfigurationParameter.FileTag:
                        _fileTag = CheckIfNotEmpty(name, value);
                        break;
                    case ConfigurationParameter.QuoteTag:
                        _quoteTag = CheckIfNotEmpty(name, value);
                        break;
                    case ConfigurationParameter.RedirectTag:
                        _redirectTag = CheckIfNotEmpty(name, value);
                        break;
                    case ConfigurationParameter.MaximumDepth:
                        _maximumDepth = CheckInteger(name, value, true);
                        break;
                    case ConfigurationParameter.CleanContents:
                        _cleanContents = CheckBoolean(name, value);
                        break;
                    case ConfigurationParameter.CategoryNamePrefix:
                        WikipediaCategory.CategoryNamePrefix = CheckIfNotEmpty(name, value);
                        break;
                    case ConfigurationParameter.BrowsableCategories:
                        _browsableCatergories.AddRange(CheckIfNotEmpty(name, value)
                                                            .Split(',')
                                                            .Select(x => x.Trim().ToUpperInvariant())
                                                            .Where(x => !string.IsNullOrWhiteSpace(x)));
                        break;
                    default:
                        base.SetParameter(name, value);
                        break;
                }
            }
        }

        #endregion Overrides
    }
}
