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

using AcroniManager.Core.Checker;
using AcroniManager.Core.Common;
using AcroniManager.Core.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace AcroniManager.Checkers.Web
{
    public class WebBasedChecker : CheckerBase
    {
        private const string _parameterNameUrlFormatString = "URLFORMATSTRING";
        private const string _parameterNameSetUrlAcronymToLowerCase = "SETACRONYMTOLOWERCASE";
        private const string _parameterNameMeaningFormatString = "MEANINGFORMATSTRING";
        private const string _parameterNameUserAgent = "USERAGENT";

        private const string _groupNameDefinition = "definition";
        private const string _acronymReplacer = "{acronym}";

        private string _urlFormatString;
        private string _meaningFormatString;
        private string _userAgent;
        private bool _setAcronymToLowerCase = false;

        protected override List<string> RequiredParameterNames
        {
            get
            {
                return new List<string> 
                            { 
                                _parameterNameUrlFormatString, 
                                _parameterNameMeaningFormatString 
                            };
            }
        }

        protected override List<Meaning> ValidateMeanings(Acronym acronym)
        {
            string acronymCaption = _setAcronymToLowerCase
                                        ? acronym.Caption.ToLowerInvariant()
                                        : acronym.Caption;

            acronymCaption = HttpUtility.UrlEncode(StringTreatment.RemoveDiacritics(acronymCaption)
                                                                  .Replace(".", string.Empty)
                                                                  .Replace("/", string.Empty));

            List<string> meanings = getMeanings(acronymCaption);

            if (meanings != null && meanings.Count == 0 && acronymCaption.Contains("-"))
            {
                meanings = getMeanings(acronymCaption.Replace("-", string.Empty));
            }

            List<Meaning> validatedMeanings = null;

            if (meanings != null)
            {
                validatedMeanings = acronym.Meanings.Where(x => meanings.Any(y => y.IndexOf(x.Caption.ToUpperInvariant()) >= 0
                                                                                  || x.Caption.ToUpperInvariant().IndexOf(y) >= 0)).ToList();
            }

            return validatedMeanings;
        }

        private List<string> getMeanings(string acronymCaption)
        {
            string url = _urlFormatString.Replace(_acronymReplacer, acronymCaption);
            HttpWebRequest request = null;

            List<string> meanings = null;

            try
            {
                request = WebRequest.CreateHttp(url);
                if (!string.IsNullOrWhiteSpace(_userAgent))
                {
                    request.UserAgent = _userAgent;
                }
                string responseText;
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                    {
                        responseText = responseReader.ReadToEnd();
                    }
                }

                meanings = new List<string>();
                MatchCollection matches = Regex.Matches(responseText, _meaningFormatString);
                foreach (Match match in matches)
                {
                    foreach (Capture capture in match.Groups[_groupNameDefinition].Captures)
                    {
                        meanings.Add(HttpUtility.HtmlDecode(capture.Value.Trim()).ToUpperInvariant());
                    }
                }
            }
            catch
            {
                meanings = null;
            }

            return meanings;
        }

        protected override void SetParameter(string name, string value)
        {
            switch (name.ToUpperInvariant())
            {
                case _parameterNameUrlFormatString:
                    _urlFormatString = CheckIfNotEmpty(name, value);
                    break;
                case _parameterNameMeaningFormatString:
                    _meaningFormatString = CheckIfNotEmpty(name, value);
                    break;
                case _parameterNameUserAgent:
                    _userAgent = CheckIfNotEmpty(name, value);
                    break;
                case _parameterNameSetUrlAcronymToLowerCase:
                    _setAcronymToLowerCase = CheckBoolean(name, value);
                    break;
                default:
                    base.SetParameter(name, value);
                    break;
            }
        }
    }
}
