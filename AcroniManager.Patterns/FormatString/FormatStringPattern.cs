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
using AcroniManager.Core.Information;
using AcroniManager.Core.Matcher;
using AcroniManager.Core.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AcroniManager.Patterns.FormatString
{
    public class FormatStringPattern : PatternBase
    {
        private const string _parameterNameFormatString = "FORMATSTRING";
        private const string _parameterNameCheckDefinition = "CHECKDEFINITION";
        private const string _parameterNameCutBeginOfDefinition = "CUTBEGINOFDEFINITION";
        private const string _parameterNameCutEndOfDefinition = "CUTENDOFDEFINITION";
        private const string _parameterNameBeginOfDefinitionBoundaries = "BEGINOFDEFINITIONBOUNDARIES";
        private const string _parameterNameNotAllowed = "BEGINOFDEFINITIONBOUNDARIES";

        private const string _groupNameDefinition = "definition";
        private const string _acronymRegularExpressionContentPlaceHolder = "{acronymRE}";

        private char[] _wordSeparators = { ',', ' ', '-', '/', '.', '\'', '(', ')', '"', '<', '>' };

        private char[] _definitionBoundariesChars = { ':', '"', ')', '(', '<', '>' };

        private const string _dotAndContinue = @"[\p{L}\p{N}]{2,}\.";
        private const string _word = @"[^\s]+(?:\s|$)";

        private const int _numberOfWordsAllowedPerMatchLetter = 3;

        private string _formatString;
        private bool _checkDefinition = false;
        private bool _cutBeginOfDefinition = false;
        private bool _cutEndOfDefinition = false;
        private List<string> _beginOfDefinitionBoundaries = null;

        public override IEnumerable<FoundMatchBase> FindMatches(ResourceInformation information)
        {
            information.Content = StringTreatment.NormalizeText(information.Content);

            return FindMatches(information.Content);
        }

        private IEnumerable<FoundMatchBase> FindMatches(string information)
        {
            
            MatchCollection matches = Regex.Matches(information, _formatString);
            foreach (Match match in matches)
            {
                foreach (Capture capture in match.Groups[_groupNameDefinition].Captures)
                {
                    string matchText = match.Groups[AcronymGroupName].Value.Trim().Replace(".", "");
                    string captureText = capture.Value.Trim();
                    if (validateAndCutDefinition(matchText, ref captureText))
                    {
                        yield return new FoundMatchBase
                                            {
                                                Match = matchText,
                                                Definition = captureText
                                            };
                    }
                    else
                    {
                        foreach (FoundMatchBase recursiveMatch in FindMatches(captureText))
                        {
                            yield return recursiveMatch;
                        }
                    }
                }
            }
        }

        #region Private Methods        

        private string preCutDefinition(string match, string definition)
        {
            string definitionTrimmed = definition;

            int numberOfWordsAllowed =  match.Replace(".", "").Length * _numberOfWordsAllowedPerMatchLetter;

            if (_cutBeginOfDefinition)
            {
                definitionTrimmed = definitionTrimmed.TrimEnd(_definitionBoundariesChars);
                int startIndexOfDefinition = definitionTrimmed.LastIndexOfAny(_definitionBoundariesChars) + 1;
                if (definitionTrimmed.Length > startIndexOfDefinition)
                {
                    definitionTrimmed = definitionTrimmed.Substring(startIndexOfDefinition);
                }
                else
                {
                    definitionTrimmed = string.Empty;
                }

                if (_beginOfDefinitionBoundaries != null
                    && _beginOfDefinitionBoundaries.Count > 0)
                {
                    foreach (string definitionBoundary in _beginOfDefinitionBoundaries)
                    {
                        MatchCollection matches = Regex.Matches(definitionTrimmed, "\\b" + definitionBoundary + "\\b", RegexOptions.IgnoreCase);
                        if (matches.Count > 0)
                        {
                            startIndexOfDefinition = matches[matches.Count - 1].Index + definitionBoundary.Length + 1;
                            if (definitionTrimmed.Length > startIndexOfDefinition)
                            {
                                definitionTrimmed = definitionTrimmed.Substring(startIndexOfDefinition);
                            }
                            else
                            {
                                definitionTrimmed = string.Empty;
                            }
                        }
                    }
                }

                Match lastDotAndContinue = Regex.Match(definitionTrimmed.TrimEnd().TrimEnd('.'), _dotAndContinue, RegexOptions.RightToLeft);
                if (lastDotAndContinue.Success)
                {
                    definitionTrimmed = definitionTrimmed.Substring(lastDotAndContinue.Index + lastDotAndContinue.Value.Length).TrimStart();
                }                

                MatchCollection words = Regex.Matches(definitionTrimmed, _word, RegexOptions.RightToLeft);
                if (words.Count > numberOfWordsAllowed)
                {
                    definitionTrimmed = definitionTrimmed.Substring(words[numberOfWordsAllowed - 1].Index);
                }
            }

            if (_cutEndOfDefinition)
            {
                definitionTrimmed = definitionTrimmed.TrimStart(_definitionBoundariesChars);
                int endIndexOfDefinition = definitionTrimmed.IndexOfAny(_definitionBoundariesChars);
                if (endIndexOfDefinition >= 0 && definitionTrimmed.Length > endIndexOfDefinition)
                {
                    definitionTrimmed = definitionTrimmed.Substring(0, endIndexOfDefinition);
                }
                else if (endIndexOfDefinition >= 0)
                {
                    definitionTrimmed = string.Empty;
                }

                Match firstDotAndContinue = Regex.Match(definitionTrimmed, _dotAndContinue);
                if (firstDotAndContinue.Success)
                {
                    definitionTrimmed = definitionTrimmed.Substring(0, firstDotAndContinue.Index + firstDotAndContinue.Value.Length).TrimEnd();
                }

                MatchCollection words = Regex.Matches(definitionTrimmed, _word);
                if (words.Count > numberOfWordsAllowed)
                {
                    definitionTrimmed = definitionTrimmed.Substring(0, words[numberOfWordsAllowed - 1].Index);
                }
            }

            return definitionTrimmed;
        }

        

        private string cleanMatch(string match)
        {
            StringBuilder cleanMatch = new StringBuilder(StringTreatment.RemoveDiacritics(match));

            cleanMatch.Replace("-", string.Empty);
            cleanMatch.Replace("/", string.Empty);

            return cleanMatch.ToString();
        }

        private bool validateAndCutDefinition(string match, ref string treatedDefinition)
        {
            bool definitionValid = true;

            if (_checkDefinition 
                || _cutBeginOfDefinition
                || _cutEndOfDefinition)
            {
                string definition = preCutDefinition(match, treatedDefinition);

                if (definition.Contains(Environment.NewLine))
                {
                    if (_cutBeginOfDefinition)
                    {
                        definition = definition.Substring(definition.LastIndexOf(Environment.NewLine));
                    }
                    else if (_cutEndOfDefinition)
                    {
                        definition = definition.Substring(0, definition.IndexOf(Environment.NewLine));
                    }
                }                

                string explicitDefinition = definition;
                string implicitDefinition = definition;

                bool definitionExplicitValid = cutDefinitionExplicit(match, ref explicitDefinition);
                bool definitionImplicitValid = cutDefinitionImplicit(match, ref implicitDefinition);

                definitionValid = definitionExplicitValid || definitionImplicitValid;

                if (definitionValid && (_cutBeginOfDefinition || _cutEndOfDefinition))
                {
                    definition = definitionExplicitValid ? explicitDefinition : implicitDefinition;
                    treatedDefinition = definition.TrimEnd(_wordSeparators.Except(new[] { '.' }).ToArray());                    
                }
                else if (definitionValid)
                {
                    definitionValid = Regex.Matches(treatedDefinition, _word).Count <= match.Replace(".", "").Length * _numberOfWordsAllowedPerMatchLetter;
                }

                treatedDefinition = trimDefinition(treatedDefinition);

                definitionValid = definitionValid && !treatedDefinition.Replace(".", "").Contains(match);
            }

            return !_checkDefinition || definitionValid;
        }

        private string trimDefinition(string definition)
        {
            string[] trimChars = { "\"", "'" };
            foreach (string trimChar in trimChars)
            {
                if (definition.StartsWith(trimChar)
                    && definition.EndsWith(trimChar))
                {
                    definition = definition.Trim(trimChar.ToCharArray());
                }
            }

            if (definition.StartsWith("<")
                && definition.EndsWith(">"))
            {
                definition = definition.Trim('<', '>');
            }

            return definition;
        }

        private bool cutDefinitionExplicit(string match, ref string definition)
        {
            bool ret = false;

            MatchCollection upperCaseLetters = Regex.Matches(StringTreatment.RemoveDiacritics(definition),
                                                             @"\p{Lu}", 
                                                             _cutBeginOfDefinition 
                                                                ? RegexOptions.RightToLeft
                                                                : RegexOptions.None);
            List<char> matchParts = new List<char>(cleanMatch(match).ToUpperInvariant().ToCharArray());
            foreach (Match upperCaseLetter in upperCaseLetters)
            {
                matchParts.Remove(upperCaseLetter.Value.ToCharArray()[0]);
                if (matchParts.Count == 0)
                {
                    ret = true;
                    if (_cutBeginOfDefinition)
                    {
                        string leftOfUpperCaseLetter = definition.Substring(0, upperCaseLetter.Index);

                        Match restOfAcronym = Regex.Match(leftOfUpperCaseLetter, "(^|\\s)(?<restOfAcronym>(?:[A-Z]\\.?\\-?\\/?)+)$", RegexOptions.RightToLeft);
                        if (restOfAcronym.Success)
                        {
                            if (restOfAcronym.Groups["restOfAcronym"].Value.StartsWith(upperCaseLetter.Value))
                            {
                                definition = definition.Substring(restOfAcronym.Index).TrimStart();
                            }
                            else
                            {
                                ret = false;
                            }
                        }
                        else
                        {
                            Match beginOfWord = Regex.Match(leftOfUpperCaseLetter, "(^|\\s)", RegexOptions.RightToLeft);
                            definition = definition.Substring(beginOfWord.Success
                                                                ? beginOfWord.Index + (beginOfWord.Value.Length > 0 ? 1 : 0)
                                                                : upperCaseLetter.Index);
                        }
                    }
                    else if (_cutEndOfDefinition)
                    {
                        string rightOfUpperCaseLetter = definition.Substring(upperCaseLetter.Index);
                        Match endOfWord = Regex.Match(rightOfUpperCaseLetter, "($|\\s)");

                        definition = definition.Substring(0, upperCaseLetter.Index + (endOfWord.Success ? endOfWord.Index : 0));
                    }
                    break;
                }
            }

            return ret;
        }

        private bool cutDefinitionImplicit(string match, ref string definition)
        {
            int numberOfCharacters = 0;
            int numberOfSeparatorsSinceLastIncludedWord = 0;
            List<char> matchParts = new List<char>(cleanMatch(match).ToUpperInvariant().ToCharArray());
            List<char> removedMatchParts = new List<char>(matchParts.Count);
            List<string> shorterMatchedDefinitionParts = new List<string>();
            IEnumerable<string> definitionParts = StringTreatment.RemoveDiacritics(definition).ToUpperInvariant().Split(_wordSeparators);

            if (_cutBeginOfDefinition)
            {
                definitionParts = definitionParts.Reverse();
            }

            foreach (string definitionPart in definitionParts)
            {
                if (definitionPart.Length == 0
                    && matchParts.Count == 0)
                {
                    numberOfSeparatorsSinceLastIncludedWord++;
                }
                if (definitionPart.Length > 0)
                {
                    char definitionPartFirstLetter = definitionPart.Substring(0, 1).ToCharArray()[0];
                    if (matchParts.Count == 0)
                    {
                        if (!removedMatchParts.Contains(definitionPartFirstLetter)
                            || definitionPart.Length <= 3
                            || shorterMatchedDefinitionParts.Where(x => x.StartsWith(definitionPart.Substring(0, 1))).All(x => x.Length > 3))
                        {
                            break;
                        }
                        else
                        {
                            numberOfCharacters = numberOfCharacters + 1 + numberOfSeparatorsSinceLastIncludedWord;
                            numberOfSeparatorsSinceLastIncludedWord = 0;
                        }
                    }
                    numberOfCharacters += definitionPart.Length;

                    if (matchParts.Remove(definitionPartFirstLetter))
                    {
                        removedMatchParts.Add(definitionPartFirstLetter);
                        shorterMatchedDefinitionParts.Add(definitionPart);
                    }
                    else if (removedMatchParts.Contains(definitionPartFirstLetter))
                    {
                        string longerMatchedDefinitionPart = shorterMatchedDefinitionParts.OrderBy(x => x.Length)
                                                                                          .FirstOrDefault(x => x.Length < definitionPart.Length
                                                                                                               && x.StartsWith(definitionPart.Substring(0, 1)));

                        if (longerMatchedDefinitionPart != null)
                        {
                            shorterMatchedDefinitionParts.Remove(longerMatchedDefinitionPart);
                            shorterMatchedDefinitionParts.Add(definitionPart);
                        }
                    }
                }
                if (matchParts.Count > 0)
                {
                    numberOfCharacters++;
                }
            }
            bool ret = matchParts.Count == 0;
            if (ret)
            {
                if (_cutBeginOfDefinition)
                {
                    definition = definition.Substring(definition.Length - numberOfCharacters);
                }
                else if (_cutEndOfDefinition)
                {
                    definition = definition.Substring(0, numberOfCharacters);
                }
            }
            return ret;
        }

        #endregion Private Methods

        #region Overrides

        protected override List<string> RequiredParameterNames
        {
            get
            {
                return new List<string> 
                            { 
                                _parameterNameFormatString
                            };
            }
        }

        protected override void SetParameter(string name, string value)
        {
            switch (name.ToUpperInvariant())
            {
                case _parameterNameFormatString:
                    _formatString = CheckIfNotEmpty(_parameterNameFormatString,
                                                    value).Replace(_acronymRegularExpressionContentPlaceHolder, AcronymRegularExpression);
                    break;
                case _parameterNameBeginOfDefinitionBoundaries:
                    _beginOfDefinitionBoundaries = new List<string>(CheckIfNotEmpty(_parameterNameBeginOfDefinitionBoundaries, value).Split(','));
                    break;
                case _parameterNameCheckDefinition:
                    _checkDefinition = CheckBoolean(_parameterNameCheckDefinition, value);
                    break;
                case _parameterNameCutBeginOfDefinition:
                    _cutBeginOfDefinition = CheckBoolean(_parameterNameCutBeginOfDefinition, value);
                    break;
                case _parameterNameCutEndOfDefinition:
                    _cutEndOfDefinition = CheckBoolean(_parameterNameCutEndOfDefinition, value);
                    break;
                default:
                    base.SetParameter(name, value);
                    break;
            }
        }

        #endregion Overrides
    }
}
