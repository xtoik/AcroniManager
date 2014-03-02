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
using AcroniManager.Core.MeaningSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AcroniManager.MeaningSelectors.Rank
{
    public enum RankingCriteria
    {
        Score,
        ValidationsCount,
        ResourcesCount,
        PatternsCount
    }

    public enum ConfigurationParameter
    {
        CriteriasOrder,
        AcronymPartWeight,
        CategoryWeight,
        ParagraphsAround,
        ParagraphDistanceModifier,
        RelevantWordMinimumLength
    }

    public class RankMeaningSelector : MeaningSelectorBase
    {
        private const string _wordRegularExpresion = @"((\b[^\s]+\b)((?<=\.\w).)?)";

        private static readonly string _criteriaOrderErrorFormatString = "The parameter {0} contains an invalid value: {1}. "
                                                + "It´s expected a sequence of at lease one valid RankingCriterias separed by \",\". The valid values are: "
                                                + RankingCriteria.PatternsCount.ToString() + ", " + RankingCriteria.ResourcesCount.ToString() + ", "
                                                + RankingCriteria.Score.ToString() + ", " + RankingCriteria.ValidationsCount.ToString() + ".";

        private List<RankingCriteria> _criteriaOrder;
        private int _acronymPartWeight = 1;
        private int _categoryWeight = 1;
        private int _paragraphsAround = 0;
        private int _relevantWordMinimumLength = 5;
        private int _paragraphDistanceModifier = 1;
                
        public override SelectedMeaningBase SelectMeaning(FoundAcronym acronym, string text)
        {
            List<RankedSelectedMeaning> meanings = acronym.Acronym.Meanings.Select(x => new RankedSelectedMeaning(x)).ToList();

            if (_criteriaOrder.Contains(RankingCriteria.Score))
            {
                TextSplitted textSplitted = getTextSplitted(acronym, text);
                foreach (RankedSelectedMeaning meaning in meanings)
                {
                    calculateScore(meaning, textSplitted);
                }
            }

            return sortByCriteriaOrder(meanings).FirstOrDefault();
        }

        protected override List<string> RequiredParameterNames
        {
            get
            {
                base.RequiredParameterNames.AddRange(new List<string> { ConfigurationParameter.CriteriasOrder.ToString() });
                return base.RequiredParameterNames;
            }
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
                    case ConfigurationParameter.CriteriasOrder:
                        _criteriaOrder = parseCriteriaOrder(name, value);
                        break;
                    case ConfigurationParameter.AcronymPartWeight:
                        _acronymPartWeight = CheckInteger(name, value, true);
                        break;
                    case ConfigurationParameter.CategoryWeight:
                        _categoryWeight = CheckInteger(name, value, true);
                        break;
                    case ConfigurationParameter.ParagraphsAround:
                        _paragraphsAround = CheckInteger(name, value, true);
                        break;
                    case ConfigurationParameter.RelevantWordMinimumLength:
                        _relevantWordMinimumLength = CheckInteger(name, value, true);
                        break;
                    case ConfigurationParameter.ParagraphDistanceModifier:
                        _paragraphDistanceModifier = CheckInteger(name, value, true);
                        break;
                    default:
                        base.SetParameter(name, value);
                        break;
                }
            }
        }

        private static List<RankingCriteria> parseCriteriaOrder(string parameterName, string criteriaOrder)
        {
            List<RankingCriteria> ret = new List<RankingCriteria>();

            foreach (string criteriaText in criteriaOrder.Split(','))
            {
                RankingCriteria criteria;
                if (!Enum.TryParse<RankingCriteria>(criteriaText.Trim(), true, out criteria))
                {
                    throw new ArgumentException(string.Format(_criteriaOrderErrorFormatString, parameterName, criteriaText.Trim()));
                }
                else if (ret.Contains(criteria))
                {
                    throw new ArgumentException("The parameter " + parameterName + " contains a repeated value: " + criteriaText + ".");
                }
                else
                {
                    ret.Add(criteria);
                }
            }

            if (ret.Count == 0)
            {
                throw new ArgumentException(string.Format(_criteriaOrderErrorFormatString, parameterName, criteriaOrder));
            }

            return ret;
        }

        private IOrderedEnumerable<RankedSelectedMeaning> sortByCriteriaOrder(List<RankedSelectedMeaning> meanings)
        {
            IOrderedEnumerable<RankedSelectedMeaning> orderedList = meanings.OrderBy(x => 1);

            foreach (RankingCriteria criteria in _criteriaOrder)
            {
                switch (criteria)
                {
                    case RankingCriteria.PatternsCount:
                        orderedList = orderedList.ThenByDescending(x => x.PatternsCount);
                        break;
                    case RankingCriteria.ValidationsCount:
                        orderedList = orderedList.ThenByDescending(x => x.ValidationsCount);
                        break;
                    case RankingCriteria.ResourcesCount:
                        orderedList = orderedList.ThenByDescending(x => x.ResourcesCount);
                        break;
                    case RankingCriteria.Score:
                        orderedList = orderedList.ThenByDescending(x => x.Score);
                        break;
                }
            }

            return orderedList;
        }

        private class TextSplitted
        {
            internal List<string> LeftParagraphs = new List<string>();
            internal string MainParagraph;
            internal List<string> RightParagraphs = new List<string>();
        }

        private TextSplitted getTextSplitted(FoundAcronym acronym, string text)
        {
            TextSplitted ret = new TextSplitted();

            string leftText = StringTreatment.NormalizeText(text.Substring(0, acronym.Index));
            MatchCollection newLines = Regex.Matches(leftText, Environment.NewLine, RegexOptions.RightToLeft);
            if (newLines.Count == 0)
            {
                ret.MainParagraph = leftText.Trim();
            }
            else
            {
                ret.MainParagraph = leftText.Substring(newLines[0].Index).Trim();
                leftText = leftText.Substring(0, newLines[0].Index);
                for (int i = 1; i <= _paragraphsAround && i <= newLines.Count; i++)
                {
                    string nextParagraph = leftText.Substring(i < newLines.Count ? newLines[i].Index : 0);
                    if (!string.IsNullOrWhiteSpace(nextParagraph))
                    {
                        ret.LeftParagraphs.Add(nextParagraph.Trim());
                    }
                    if (i < newLines.Count)
                    {
                        leftText = leftText.Substring(0, newLines[i].Index);
                    }
                }
            }

            ret.MainParagraph += " "; // +acronym.Caption + " ";

            string rightText = StringTreatment.NormalizeText(text.Substring(acronym.Index + acronym.Caption.Length));
            newLines = Regex.Matches(rightText, Environment.NewLine);
            if (newLines.Count == 0)
            {
                ret.MainParagraph += rightText.Trim();
            }
            else
            {
                ret.MainParagraph += rightText.Substring(0, newLines[0].Index).Trim();
                int indexOfLastLineBreak = newLines[0].Index;
                for (int i = 1; i <= _paragraphsAround && i <= newLines.Count; i++)
                {
                    string nextParagraph = rightText.Substring(indexOfLastLineBreak, i < newLines.Count ? (newLines[i].Index - indexOfLastLineBreak) 
                                                                                                        : rightText.Length - indexOfLastLineBreak);
                    if (!string.IsNullOrWhiteSpace(nextParagraph))
                    {
                        ret.RightParagraphs.Add(nextParagraph.Trim());
                    }
                    if (i < newLines.Count)
                    {
                        indexOfLastLineBreak = newLines[i].Index;
                    }
                }
            }

            return ret;
        }

        private void calculateScore(RankedSelectedMeaning meaning, TextSplitted textSplitted)
        {
            meaning.Score = 0.0;
            StringBuilder explanationSB = new StringBuilder();

            explanationSB.Append("Main paragraph:");
            addParagraphScore(meaning, textSplitted.MainParagraph, 0, explanationSB);

            List<List<string>> paragraphsToCheck = new List<List<string>> { textSplitted.LeftParagraphs, textSplitted.RightParagraphs };
            bool leftParagraph = true;

            foreach (List<string> paragraphsList in paragraphsToCheck)
            {
                int distance = 0;
                foreach (string paragraph in paragraphsList)
                {
                    distance++;
                    explanationSB.AppendFormat("{2}{0} paragraph {1}:", leftParagraph ? "Left" : "Right", distance, Environment.NewLine);
                    addParagraphScore(meaning, paragraph, distance, explanationSB);
                }
                leftParagraph = false;
            }

            explanationSB.AppendFormat("{1}Total score: {0}", meaning.Score, Environment.NewLine);
            meaning.Explanation = explanationSB.ToString();
        }

        private void addParagraphScore(RankedSelectedMeaning meaning, string paragraph, int paragraphDistance, StringBuilder explanationSB)
        {
            double distanceModifier = paragraphDistance == 0 ? 1.0 : (double) paragraphDistance * (double) _paragraphDistanceModifier;
            bool nothingCounted = true;
            foreach (Match word in Regex.Matches(paragraph, _wordRegularExpresion))
            {
                if (word.Length > _relevantWordMinimumLength)
                {
                    double initialScore = meaning.Score;                    
                    int acronymPartsCount = getNumOfMatches(meaning.MeaningCaption, word.Value);
                    meaning.Score += acronymPartsCount * _acronymPartWeight / distanceModifier;
                    int categoriesPartCount = getNumOfMatches(string.Join(", ", meaning.Meaning.Categories), word.Value);
                    meaning.Score +=  categoriesPartCount * _categoryWeight / distanceModifier;
                    if (acronymPartsCount > 0 || categoriesPartCount > 0)
                    {
                        explanationSB.AppendFormat("{4}\tWord: {0}\t\t\tAcronym Parts: {1}\t\tCategories Part: {2}\t\tScore:{3}",
                                                   word.Value, acronymPartsCount, categoriesPartCount, meaning.Score - initialScore, Environment.NewLine);
                        nothingCounted = false;
                    }
                }
            }
            if (nothingCounted)
            {
                explanationSB.AppendFormat("\t\tnothing relevant");
            }
        }

        private int getNumOfMatches(string text, string pattern)
        {
            int count = 0;

            for (int i = text.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase); 
                 i > -1; 
                 i = text.IndexOf(pattern, i + 1 , StringComparison.InvariantCultureIgnoreCase))
            {
                count++;
            }

            return count;
        }
    }
}
