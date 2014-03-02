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
using AcroniManager.Core.Matcher;
using AcroniManager.Core.Pattern;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace AcroniManager.Patterns.UnitTests.TestAdapter
{
    internal class PatternTestAdapter<T> where T : PatternBase
    {
        private PatternBase _pattern;
        private ResourceInformation _input;
        private List<FoundMatchBase> _expectedOutput;

        internal PatternTestAdapter(NameValueCollection config, ResourceInformation input, List<FoundMatchBase> expectedOutput)
        {
            _pattern = Activator.CreateInstance<T>();

            if (config != null)
            {
                NameValueConfigurationCollection properConfig = new NameValueConfigurationCollection();
                foreach (string key in config.AllKeys) 
                {
                    properConfig.Add(new NameValueConfigurationElement(key, config[key]));
                }
                _pattern.Configure(properConfig);
            }
            _input = input;
            _expectedOutput = expectedOutput;
        }

        internal void ExecuteTest()
        {
            IEnumerable<FoundMatchBase> actualOutput = _pattern.FindMatches(_input);

            foreach (FoundMatchBase output in actualOutput)
            {
                if (!_expectedOutput.Any(x => x.Match == output.Match
                                              && x.Definition == output.Definition))
                {
                    Assert.Fail("The output got was not expected:{2}\"{0}\": \"{1}\".", output.Match, output.Definition, Environment.NewLine);
                }
            }

            foreach (FoundMatchBase output in _expectedOutput)
            {
                if (!actualOutput.Any(x => x.Match == output.Match
                                           && x.Definition == output.Definition))
                {
                    Assert.Fail("The expected output was not got:{2}\"{0}\": \"{1}\".", output.Match, output.Definition, Environment.NewLine);
                }
            }
        }
    }
}
