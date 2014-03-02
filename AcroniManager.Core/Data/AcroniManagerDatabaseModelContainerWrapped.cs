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
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AcroniManager.Core.Data
{
    internal class AcroniManagerDatabaseModelContainerWrapped : AcroniManagerDatabaseModelContainer
    {
        public static bool _databaseInitialized = false;

        public AcroniManagerDatabaseModelContainerWrapped() : base()
        {
            initializeDatabase();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void initializeDatabase()
        {
            if (!_databaseInitialized)
            {
                string efConnectionString = ConfigurationManager.ConnectionStrings["AcroniManagerDatabaseModelContainer"].ConnectionString;
                EntityConnectionStringBuilder efConnectionStringBuilder = new EntityConnectionStringBuilder(efConnectionString);
                if (efConnectionStringBuilder.Provider.ToUpperInvariant().Contains("SqlServerCe".ToUpperInvariant()))
                {
                    string databaseConnectionString = efConnectionStringBuilder.ProviderConnectionString;
                    string dataSource = databaseConnectionString.Split(';').First(x => x.ToUpperInvariant().Contains("data source".ToUpperInvariant()));
                    string fileName = dataSource.Substring(dataSource.IndexOf('=') + 1);

                    string databaseInitializationScript;
                    using (Stream stream = typeof(AcroniManagerDatabaseModelContainerWrapped)
                                                .Assembly.GetManifestResourceStream("AcroniManager.Core.Data.AcroniManagerDatabaseModel.edmx.sqlce"))
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        databaseInitializationScript = streamReader.ReadToEnd();
                    }

                    if (!File.Exists(fileName))
                    {
                        using (SqlCeEngine sqlCeEngine = new SqlCeEngine(databaseConnectionString))
                        {
                            sqlCeEngine.CreateDatabase();
                        }

                        using (SqlCeConnection sqlCeConnection = new SqlCeConnection(databaseConnectionString))
                        {
                            sqlCeConnection.Open();
                            using (SqlCeCommand sqlCeCommand = sqlCeConnection.CreateCommand())
                            {
                                foreach (string rawCommandText in databaseInitializationScript.Split(new [] { "GO", ";" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    StringBuilder commandTextBuilder = new StringBuilder();
                                    foreach (string commandPart in rawCommandText.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        if (!string.IsNullOrWhiteSpace(commandPart)
                                            && !commandPart.StartsWith("--"))
                                        {
                                            commandTextBuilder.Append(commandPart);
                                        }
                                    }
                                    if (!string.IsNullOrWhiteSpace(commandTextBuilder.ToString())
                                        && !commandTextBuilder.ToString().Contains("DROP"))
                                    {
                                        sqlCeCommand.CommandText = commandTextBuilder.ToString();
                                        sqlCeCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                            sqlCeConnection.Close();
                        }
                    }
                }

                _databaseInitialized = true;
            }
        }
    }
}
