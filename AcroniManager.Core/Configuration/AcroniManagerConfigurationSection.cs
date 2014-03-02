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
using System.Configuration;
using System.Runtime.CompilerServices;

namespace AcroniManager.Core.Configuration
{
    public class AcroniManagerConfigurationSection : ConfigurationSection
    {
        #region static members

        private static AcroniManagerConfigurationSection _instance;

        internal static AcroniManagerConfigurationSection Instance
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (_instance == null)
                {
                    _instance = ConfigurationManager.GetSection("acroniManager") as AcroniManagerConfigurationSection;
                }
                if (_instance == null)
                {
                    throw new ConfigurationErrorsException("The \"acroniManager\" section is not defined properly in the configuration file");
                }
                return _instance;
            }
        }

        #endregion static members

        [ConfigurationProperty("leechers", IsRequired = true)]
        public LeechersCollection Leechers
        {
            get { return (LeechersCollection)this["leechers"]; }
        }

        [ConfigurationProperty("patterns", IsRequired = true)]
        public PatternsCollection Patterns
        {
            get { return (PatternsCollection)this["patterns"]; }
        }

        [ConfigurationProperty("checkers", IsRequired = false)]
        public CheckersCollection Checkers
        {
            get { return (CheckersCollection)this["checkers"]; }
        }

        [ConfigurationProperty("meaningSelector", IsRequired = false)]
        public MeaningSelectorElement MeaningSelector
        {
            get { return (MeaningSelectorElement)this["meaningSelector"]; }
        }

        [ConfigurationProperty("xmlns", IsRequired = false)]
        public string Schema
        {
            get { return (string)this["xmlns"]; }
        }

        [ConfigurationProperty("testCount", IsRequired = false)]
        public string TestCount
        {
            get { return (string)base["testCount"]; }
        }

        [ConfigurationProperty("storeResourceInformation", IsRequired = false)]
        public string StoreResourceInformation
        {
            get { return (string)this["storeResourceInformation"]; }
        }

        [ConfigurationProperty("acronymRegularExpression", IsRequired = true)]
        public string AcronymRegularExpression
        {
            get { return (string)this["acronymRegularExpression"]; }
        }

        [ConfigurationProperty("acronymGroupName", IsRequired = true)]
        public string AcronymGroupName
        {
            get { return (string)this["acronymGroupName"]; }
        }
    }

    [ConfigurationCollection(typeof(LeecherElement), AddItemName = "leecher")]
    public class LeechersCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LeecherElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return ((LeecherElement)element).Class;
        }
    }

    public class LeecherElement : ConfigurationElement
    {
        [ConfigurationProperty("executions", IsRequired = true)]
        public ExecutionsCollection Executions
        {
            get { return (ExecutionsCollection)this["executions"]; }
        }

        [ConfigurationProperty("class", IsRequired = true)]
        public string Class
        {
            get { return (string)base["class"]; }
        }

        [ConfigurationProperty("source", IsRequired = true)]
        public string Source
        {
            get { return (string)base["source"]; }
        }
    }

    [ConfigurationCollection(typeof(ExecutionElement), AddItemName = "execution")]
    public class ExecutionsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExecutionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return ((ExecutionElement)element).Description;
        }
    }

    public class ExecutionElement : ParameterizableConfigurationElement
    {
        [ConfigurationProperty("languageCode", IsRequired = true)]
        public string LanguageCode
        {
            get { return (string)base["languageCode"]; }
        }        
    }

    [ConfigurationCollection(typeof(ConfigurableElement), AddItemName = "pattern")]
    public class PatternsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurableElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return ((ConfigurableElement)element).Class;
        }
    }

    [ConfigurationCollection(typeof(ConfigurableElement), AddItemName = "checker")]
    public class CheckersCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurableElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return ((ConfigurableElement)element).Class;
        }
    }

    public class ConfigurableElement : ConfigurationElement
    {
        [ConfigurationProperty("instanceConfigurations", IsRequired = false)]
        public ConfigurationsCollection Configurations
        {
            get { return (ConfigurationsCollection)this["instanceConfigurations"]; }
        }

        [ConfigurationProperty("class", IsRequired = true)]
        public string Class
        {
            get { return (string)base["class"]; }
        }
    }

    [ConfigurationCollection(typeof(ParameterizableConfigurationElement), AddItemName = "instanceConfiguration")]
    public class ConfigurationsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ParameterizableConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return ((ParameterizableConfigurationElement)element).Description;
        }
    }

    public class ParameterizableConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("description", IsRequired = false)]
        public string Description
        {
            get { return (string)base["description"]; }
        }

        [ConfigurationProperty("parameters", IsRequired = false)]
        [ConfigurationCollection(typeof(NameValueConfigurationElement), AddItemName = "parameter")]
        public NameValueConfigurationCollection Parameters
        {
            get { return (NameValueConfigurationCollection)this["parameters"]; }
        }        
    }

    public class MeaningSelectorElement : ParameterizableConfigurationElement
    {
        [ConfigurationProperty("class", IsRequired = true)]
        public string Class
        {
            get { return (string)base["class"]; }
        }
    }
}
