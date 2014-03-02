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

using System;
using System.Globalization;
using System.Text;

namespace AcroniManager.Core.Common
{
    public static class StringTreatment
    {
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string NormalizeText(string text)
        {
            StringBuilder textNormalizer = new StringBuilder(text);
            textNormalizer = textNormalizer.Replace("\r\n", "\n")
                                                 .Replace("\n\r", "\n")
                                                 .Replace("\r", "\n")
                                                 .Replace("\n", Environment.NewLine)
                                                 .Replace("\t", " ");

            int lengthBefore;
            do
            {
                lengthBefore = textNormalizer.Length;
                textNormalizer = textNormalizer.Replace("  ", " ")
                                               .Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            } while (textNormalizer.Length != lengthBefore);

            return textNormalizer.ToString();
        }
    }
}
