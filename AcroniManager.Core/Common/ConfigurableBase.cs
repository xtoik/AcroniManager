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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;

namespace AcroniManager.Core.Common
{
    public abstract class ConfigurableBase
    {
        #region Static

        internal static T CreateConfigurableItem<T>(Type itemType, NameValueConfigurationCollection parameters) where T : ConfigurableBase
        {
            T configurableItem = Activator.CreateInstance(itemType) as T;

            configurableItem.Configure(parameters);

            return configurableItem;
        }

        internal static Type GetType(string className, Type baseClass, string objectType)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ConfigurationErrorsException(
                    String.Format("The configuration of the \"{0}\" is not correctly defined.", objectType));
            }
            Type type = Type.GetType(className);
            if (type == null)
            {
                throw new ConfigurationErrorsException(
                    String.Format("The {0} type \"{1}\" hasn't been found.", objectType, className));
            }
            if (!type.IsSubclassOf(baseClass))
            {
                throw new ConfigurationErrorsException(
                    String.Format("The {0} type \"{1}\" doesn´t derive from \"{2}\".", objectType, type.FullName, baseClass.Name));
            }
            return type;
        }

        #endregion Static 

        #region Fields

        private readonly NameValueCollection _parameters = new NameValueCollection();

        #endregion Fields

        #region Internal Members

        public void Configure(NameValueConfigurationCollection parameters)
        {
            if (parameters != null)
            {
                foreach (NameValueConfigurationElement parameter in parameters)
                {
                    _parameters.Add(parameter.Name, parameter.Value);
                    SetParameter(parameter.Name, parameter.Value);
                }                
            }

            List<string> notProvidedParameters = RequiredParameterNames.FindAll(x => string.IsNullOrWhiteSpace(_parameters[x]));
            if (notProvidedParameters != null && notProvidedParameters.Count > 0)
            {
                StringBuilder notProvidedParametersSB = new StringBuilder();
                notProvidedParametersSB.AppendFormat("The following parameters are required by the class \"{0}\" and has not been provided: ",
                                                     this.GetType().Name);
                foreach (string notProvidedParameter in notProvidedParameters)
                {
                    notProvidedParametersSB.AppendFormat("{0}, ", notProvidedParameter);
                }

                throw new ConfigurationErrorsException(notProvidedParametersSB.Remove(notProvidedParametersSB.Length - 2, 2).Append(".").ToString());
            }
        }

        #endregion Internal Members

        #region Virtual Members

        protected virtual void SetParameter(string name, string value) 
        {
            throw new ArgumentException("The parameter " + name + " has not been found in the class " + this.GetType().Name);
        }

        protected virtual List<string> RequiredParameterNames { get { return new List<string>(); } }

        #endregion Virtual Members

        #region Protected Members

        protected NameValueCollection Parameters
        {
            get { return _parameters; }
        }

        protected static string CheckIfNotEmpty(string parameterName, string parameterValue)
        {
            if (string.IsNullOrWhiteSpace(parameterValue))
            {
                throw new ArgumentException("The value for the configuration parameter " + parameterName + " is not valid. Must be a string not null or empty");
            }
            return parameterValue;
        }

        protected static bool CheckBoolean(string parameterName, string value)
        {
            bool cleanContents;
            if (!bool.TryParse(value, out cleanContents))
            {
                throw new ArgumentException("The value for the configuration parameter " + parameterName + " is not valid. Must be boolean!");
            }
            return cleanContents;
        }

        protected static int CheckInteger(string parameterName, string value, bool positive = false)
        {
            int cleanContents;
            if (!int.TryParse(value, out cleanContents))
            {
                throw new ArgumentException("The value for the configuration parameter " + parameterName + " is not valid. Must be integer!");
            }
            else if (positive && cleanContents < 0)
            {
                throw new ArgumentException("The value for the configuration parameter " + parameterName + " is not valid. Must be greater or equal than 0!");
            }
            return cleanContents;
        }

        #endregion Protected Members
    }
}
