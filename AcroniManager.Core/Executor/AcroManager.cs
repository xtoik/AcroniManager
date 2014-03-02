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
using AcroniManager.Core.Configuration;
using AcroniManager.Core.Data;
using AcroniManager.Core.Data.Factory;
using AcroniManager.Core.Information;
using AcroniManager.Core.Leecher;
using AcroniManager.Core.Matcher;
using AcroniManager.Core.Pattern;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AcroniManager.Core.Executor
{
    public delegate void ResourceLeechedEventHandler(ResourceInformation information);
    public delegate void MeaningCheckedEventHandler(string acronym, string meaning);
    public delegate void MeaningValidatedEventHandler(string acronym, string meaning);

    public class DatabaseStatus
    {
        public long NumberOfResourcesLeeched;
        public long NumberOfAcronymsExtracted;
        public long NumberOfMeaningsParsed;
        public long NumberOfMeaningsChecked;
        public long NumberOfMeaningsValidated;
    }

    public sealed class AcroManager
    {
        #region Static Members

        private static volatile AcroManager _instance;
        private static object _lock = new Object();        

        public static AcroManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new AcroManager();
                    }
                }

                return _instance;
            }
        }

        #endregion Static Members

        #region Fields

        private bool _isRunning;
        private List<LeecherBase> _leechers;
        private List<PatternBase> _patterns;
        private List<CheckerBase> _checkers;
        private int? _testCount;
        private TextWriter _resourceInformationStoreTW;

        #endregion Fields

        #region Constructor

        private AcroManager() 
        {
            if (!string.IsNullOrWhiteSpace(AcroniManagerConfigurationSection.Instance.TestCount))
            {
                int testCount;
                if (int.TryParse(AcroniManagerConfigurationSection.Instance.TestCount, out testCount))
                {
                    _testCount = testCount;
                }
            }
        }

        #endregion Constructor

        #region Public Members

        public event ResourceLeechedEventHandler ResourceLeeched;
        public event AcronymCreatedEventHandler AcronymCreated;
        public event MeaningCreatedEventHandler MeaningCreated;
        public event MeaningCheckedEventHandler MeaningChecked;
        public event MeaningValidatedEventHandler MeaningValidated;

        public DatabaseStatus GetDatabaseStatus()
        {
            DatabaseStatus ret = null;
            if (!_isRunning)
            {
                using (DataContext.Instance = new AcroniManagerDatabaseModelContainerWrapped())
                {
                    ret = new DatabaseStatus
                                {
                                    NumberOfResourcesLeeched = DataContext.Instance.ResourceSet.LongCount(),
                                    NumberOfAcronymsExtracted = DataContext.Instance.AcronymSet.LongCount(),
                                    NumberOfMeaningsParsed = DataContext.Instance.MeaningSet.LongCount(),
                                    NumberOfMeaningsChecked = DataContext.Instance.MeaningSet.LongCount(x => x.Validations.Any()),
                                    NumberOfMeaningsValidated = DataContext.Instance.MeaningSet.LongCount(x => x.Validations.Any(y => y.Validated))
                                };
                }
            }
            return ret;
        }
        
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LeechResources()
        {
            if (!_isRunning)
            {
                try
                {
                    _isRunning = true;
                    MeaningFactory.AcronymCreated += MeaningFactory_AcronymCreated;
                    MeaningFactory.MeaningCreated += MeaningFactory_MeaningCreated;

                    using (DataContext.Instance = new AcroniManagerDatabaseModelContainerWrapped())
                    {
                        initializeLeechers();
                        initializePatterns();

                        if (!string.IsNullOrWhiteSpace(AcroniManagerConfigurationSection.Instance.StoreResourceInformation))
                        {
                            _resourceInformationStoreTW = File.CreateText(AcroniManagerConfigurationSection.Instance.StoreResourceInformation);
                        }

                        foreach (LeecherBase leecher in Leechers)
                        {
                            Debug.WriteLine("Starting to leech the source \"{0}\" using the leecher \"{1}\".",
                                            leecher.Execution.Crawler.Source.Name,
                                            leecher.GetType().Name);
                            Stopwatch stopWatch = Stopwatch.StartNew();
                            long resourcesLeeched = 0;
                            IEnumerable<LeechedResourceBase> leechedResources = !_testCount.HasValue
                                                                                    ? leecher.LeechedResources
                                                                                    : leecher.LeechedResources.Take(_testCount.Value);
                            foreach (LeechedResourceBase leechedResource in leechedResources)
                            {
                                leechedResource.Resource = ResourceFactory.GetResource(leechedResource.ResourceKey, leecher.Execution);

                                List<PatternBase> notUsedPatterns = Patterns.Where(p => !leechedResource.Resource.Configurations.Contains(p.Configuration)).ToList();

                                if (notUsedPatterns.Count > 0)
                                {
                                    leechedResource.ResourceInformation = leecher.GetResourceInformation(leechedResource);

                                    if (_resourceInformationStoreTW != null)
                                    {
                                        _resourceInformationStoreTW.WriteLine(leechedResource.ResourceInformation.Content);
                                        _resourceInformationStoreTW.Flush();
                                    }

                                    resourcesLeeched++;
                                    if (ResourceLeeched != null)
                                    {
                                        ResourceLeeched(leechedResource.ResourceInformation);
                                    }

                                    foreach (PatternBase pattern in notUsedPatterns)
                                    {
                                        leechedResource.Resource.Configurations.Add(pattern.Configuration);
                                        DataContext.Instance.SaveChanges();

                                        foreach (FoundMatchBase match in pattern.FindMatches(leechedResource.ResourceInformation))
                                        {
                                            match.Meaning = MeaningFactory.GetMeaning(match.Match, match.Definition, leechedResource.ResourceInformation.Categories);
                                            bool pendingChanges = false;
                                            if (!match.Meaning.Configurations.Contains(pattern.Configuration))
                                            {
                                                match.Meaning.Configurations.Add(pattern.Configuration);
                                                pendingChanges = true;
                                            }
                                            if (!match.Meaning.Resources.Contains(leechedResource.Resource))
                                            {
                                                match.Meaning.Resources.Add(leechedResource.Resource);
                                                pendingChanges = true;
                                            }
                                            if (pendingChanges)
                                            {
                                                DataContext.Instance.SaveChanges();
                                            }
                                            Debug.WriteLine(match);
                                        }
                                    }
                                }
                            }
                            stopWatch.Stop();
                            Debug.WriteLine("Finished leeching the source \"{0}\" using the leecher \"{1}\".{4}Leeched {2} resources in {3}!",
                                            leecher.Execution.Crawler.Source.Name,
                                            leecher.GetType().Name,
                                            resourcesLeeched,
                                            stopWatch.Elapsed.ToString(),
                                            Environment.NewLine);
                        }
                    }
                }
                finally
                {
                    if (_resourceInformationStoreTW != null)
                    {
                        _resourceInformationStoreTW.Close();
                        _resourceInformationStoreTW.Dispose();
                    }

                    MeaningFactory.AcronymCreated -= MeaningFactory_AcronymCreated;
                    MeaningFactory.MeaningCreated -= MeaningFactory_MeaningCreated;

                    _leechers = null;
                    _patterns = null;
                    DataContext.Instance = null;
                    _isRunning = false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ValidateMeanings()
        {
            if (!_isRunning)
            {
                try
                {
                    _isRunning = true;

                    using (DataContext.Instance = new AcroniManagerDatabaseModelContainerWrapped())
                    {
                        List<int> arrangementsIDs = Checkers.Select(x => x.Arrangement.ArrangementId).Distinct().ToList();

                        foreach (Acronym acronym in DataContext.Instance.AcronymSet
                                                                        .Where(x => x.Meanings
                                                                                     .Any(y => !y.Validations.Any()
                                                                                               || y.Validations
                                                                                                   .All(z => !arrangementsIDs.Contains(z.Arrangement.ArrangementId))
                                                                                         )
                                                                              ).ToList())
                        {
                            foreach (CheckerBase checker in Checkers.Where(x => !acronym.Meanings
                                                                                        .SelectMany(y => y.Validations.Select(z => z.Arrangement.ArrangementId))
                                                                                        .Contains(x.Arrangement.ArrangementId)))
                            {
                                List<Meaning> validatedMeanings = checker.ValidateMeanings(acronym);
                                if (validatedMeanings != null)
                                {
                                    foreach (Meaning meaning in acronym.Meanings)
                                    {
                                        Validation validation = meaning.Validations
                                                                       .FirstOrDefault(x => x.Arrangement.ArrangementId == checker.Arrangement.ArrangementId);

                                        bool pendingChanges = false;
                                                                                               
                                        if (validation == null)
                                        {
                                            validation = new Validation
                                                                {
                                                                    Arrangement = checker.Arrangement,
                                                                    Meaning = meaning
                                                                };
                                            meaning.Validations.Add(validation);

                                            if (MeaningChecked != null && meaning.Validations.Count == 1)
                                            {
                                                MeaningChecked(acronym.Caption, meaning.Caption);
                                            }

                                            pendingChanges = true;
                                        }

                                        bool validated = validatedMeanings.Any(x => x.MeaningId == meaning.MeaningId);

                                        if (validation.Validated != validated)
                                        {
                                            validation.Validated = validated;

                                            if (MeaningValidated != null && validated && meaning.Validations.Count(x => x.Validated) == 1)
                                            {
                                                MeaningValidated(acronym.Caption, meaning.Caption);
                                            }

                                            pendingChanges = true;
                                        }

                                        if (pendingChanges)
                                        {
                                            DataContext.Instance.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    _checkers = null;
                    DataContext.Instance = null;
                    _isRunning = false;
                }
            }
        }

        #endregion Public Members        

        #region Helpers

        private List<LeecherBase> Leechers
        {            
            get
            {
                initializeLeechers();
                return _leechers;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void initializeLeechers()
        {
            if (_leechers == null)
            {
                _leechers = new List<LeecherBase>();

                foreach (LeecherElement leecherConfiguration in AcroniManagerConfigurationSection.Instance.Leechers)
                {
                    Type leecherType = ConfigurableBase.GetType(leecherConfiguration.Class, typeof(LeecherBase), "leecher");

                    Crawler crawler = CrawlerFactory.GetCrawler(leecherConfiguration);

                    foreach (ExecutionElement executionConfiguration in leecherConfiguration.Executions)
                    {
                        ValidateConfiguration(executionConfiguration);
                        LeecherBase leecher = ConfigurableBase.CreateConfigurableItem<LeecherBase>(leecherType, executionConfiguration.Parameters);
                        leecher.Execution = ExecutionFactory.GetExecution(executionConfiguration, crawler);
                        _leechers.Add(leecher);
                    }
                }
            }
        }

        private List<PatternBase> Patterns
        {            
            get
            {
                initializePatterns();
                return _patterns;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void initializePatterns()
        {
            if (_patterns == null)
            {
                _patterns = new List<PatternBase>();

                foreach (ConfigurableElement patternConfiguration in AcroniManagerConfigurationSection.Instance.Patterns)
                {
                    Type patternType = ConfigurableBase.GetType(patternConfiguration.Class, typeof(PatternBase), "pattern");

                    Data.Matcher matcher = MatcherFactory.GetMatcher(patternConfiguration);

                    if (patternConfiguration.Configurations == null
                        || patternConfiguration.Configurations.Count == 0)
                    {
                        CreatePattern(patternType, null, matcher);
                    }
                    else
                    {
                        foreach (ParameterizableConfigurationElement executionConfiguration in patternConfiguration.Configurations)
                        {
                            CreatePattern(patternType, executionConfiguration, matcher);
                        }
                    }
                }
            }
        }

        private List<CheckerBase> Checkers
        {
            get
            {
                initializeCheckers();
                return _checkers;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void initializeCheckers()
        {
            if (_checkers == null)
            {
                _checkers = new List<CheckerBase>();
                foreach (ConfigurableElement checkerConfiguration in AcroniManagerConfigurationSection.Instance.Checkers)
                {
                    Type checkerType = ConfigurableBase.GetType(checkerConfiguration.Class, typeof(CheckerBase), "checker");
                    Validator validator = ValidatorFactory.GetValidator(checkerConfiguration);

                    if (checkerConfiguration.Configurations == null
                        || checkerConfiguration.Configurations.Count == 0)
                    {
                        CreateChecker(checkerType, null, validator);
                    }
                    else
                    {
                        foreach (ParameterizableConfigurationElement executionConfiguration in checkerConfiguration.Configurations)
                        {
                            CreateChecker(checkerType, executionConfiguration, validator);
                        }
                    }
                }
            }
        }        

        private void CreatePattern(Type patternType, ParameterizableConfigurationElement patternConfiguration, Data.Matcher matcher)
        {
            PatternBase pattern = ConfigurableBase.CreateConfigurableItem<PatternBase>(patternType, patternConfiguration != null ? patternConfiguration.Parameters : null); 

            pattern.Configuration = ConfigurationFactory.GetConfiguration(patternConfiguration, matcher);

            _patterns.Add(pattern);
        }

        private void CreateChecker(Type checkerType, ParameterizableConfigurationElement checkerConfiguration, Validator validator)
        {
            CheckerBase checker = ConfigurableBase.CreateConfigurableItem<CheckerBase>(checkerType, checkerConfiguration != null ? checkerConfiguration.Parameters : null);

            checker.Arrangement = ArrangementFactory.GetArrangement(checkerConfiguration, validator);

            _checkers.Add(checker);
        }        

        private void ValidateConfiguration(ExecutionElement executionConfiguration)
        {
            if (string.IsNullOrWhiteSpace(executionConfiguration.LanguageCode))
            {
                throw new ConfigurationErrorsException("The language code cannot be null ot white space.");
            }

            CultureInfo selectedLang = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                                                  .FirstOrDefault(x => x.Name.Equals(executionConfiguration.LanguageCode,
                                                                                     StringComparison.InvariantCultureIgnoreCase));
            if (selectedLang == null)
            {
                throw new ConfigurationErrorsException(String.Format("The language code \"{0}\" is not defined as a neutral culture in the framework.",
                                                                     selectedLang.Name));
            }
            if (selectedLang.Equals(CultureInfo.InvariantCulture))
            {
                throw new ConfigurationErrorsException("The language code specified cannot be the Invariant culture.");
            }
        }

        private void MeaningFactory_MeaningCreated(string acronym, string meaning)
        {
            if (MeaningCreated != null)
            {
                MeaningCreated(acronym, meaning);
            }
        }

        private void MeaningFactory_AcronymCreated(string acronym)
        {
            if (AcronymCreated != null)
            {
                AcronymCreated(acronym);
            }
        }

        #endregion Helpers
    }
}
