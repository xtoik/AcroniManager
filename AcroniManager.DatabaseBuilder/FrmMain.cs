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

using AcroniManager.Core.Executor;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace AcroniManager.DatabaseBuilder
{
    public partial class FrmMain : Form
    {
        private enum WorkType
        {
            LeechResources,
            ValidateAcronyms
        }

        private DatabaseStatus databaseStatus;
        private WorkType currentWork;
        private DateTime startTime;
        private long initialNumberOfMeaningsChecked;
        
        private bool FormEnabled
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new ReceiveBooleanDelegate(SetFormEnabled), value);
                }
                else
                {
                    SetFormEnabled(value);
                }
            }
        }

        private void SetFormEnabled(bool enable)
        {
            btnStartLeeching.Enabled = enable;
            btnStartValidating.Enabled = enable;
        }

        private bool EstimationsVisible
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new ReceiveBooleanDelegate(SetEstimationsVisible), value);
                }
                else
                {
                    SetEstimationsVisible(value);
                }
            }
        }

        private void SetEstimationsVisible(bool visible)
        {
            lblTimeEstimated.Visible = visible;
            lblTimeEstimatedCaption.Visible = visible;
            barProgress.Visible = visible;
        }

        private bool TimerEnabled
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new ReceiveBooleanDelegate(SetTimerEnabled), value);
                }
                else
                {
                    SetTimerEnabled(value);
                }
            }
        }

        private void SetTimerEnabled(bool enable)
        {
            refreshTimer.Enabled = enable;
        }

        public FrmMain()
        {
            InitializeComponent();

            EstimationsVisible = false;

            lblTimeElapsed.Text = string.Empty;
            lblTimeEstimated.Text = string.Empty;
            lblResourcesLeeched.Text = string.Empty;
            lblAcronymsExtracted.Text = string.Empty;
            lblMeaningsParsed.Text = string.Empty;
            lblMeaningsChecked.Text = string.Empty;
            lblMeaningsValidated.Text = string.Empty;
        }

        private void btnStartLeeching_Click(object sender, EventArgs e)
        {            
            backgroundWorker.RunWorkerAsync(WorkType.LeechResources);
        }

        private void btnStartValidating_Click(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync(WorkType.ValidateAcronyms);
        }

        private void getDatabaseStatus()
        {
            databaseStatus = AcroManager.Instance.GetDatabaseStatus();
        }

        private void printDatabaseStatus()
        {
            if (databaseStatus != null)
            {
                lblAcronymsExtracted.Text = databaseStatus.NumberOfAcronymsExtracted.ToString("n0");
                lblMeaningsChecked.Text = databaseStatus.NumberOfMeaningsChecked.ToString("n0");
                lblMeaningsParsed.Text = databaseStatus.NumberOfMeaningsParsed.ToString("n0");
                lblMeaningsValidated.Text = databaseStatus.NumberOfMeaningsValidated.ToString("n0");
                lblResourcesLeeched.Text = databaseStatus.NumberOfResourcesLeeched.ToString("n0");
            }
        }

        private delegate void ReceiveBooleanDelegate(bool enable);
        private delegate void ReceiveNothingDelegate();

        private void RefreshDatabaseStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new ReceiveNothingDelegate(printDatabaseStatus));
            }
            else
            {
                printDatabaseStatus();
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Exception exceptionHappened = null;
            try
            {
                FormEnabled = false;

                currentWork = (WorkType)e.Argument;
                Cursor.Current = Cursors.WaitCursor;

                startTime = DateTime.UtcNow;
                getDatabaseStatus();

                RefreshDatabaseStatus();
                TimerEnabled = true;

                switch (currentWork)
                {
                    case WorkType.LeechResources:
                        AcroManager.Instance.ResourceLeeched += resourceLeeched;
                        AcroManager.Instance.AcronymCreated += acronymCreated;
                        AcroManager.Instance.MeaningCreated += meaningCreated;
                        AcroManager.Instance.LeechResources();
                        break;
                    case WorkType.ValidateAcronyms:
                        initialNumberOfMeaningsChecked = databaseStatus.NumberOfMeaningsChecked;
                        EstimationsVisible = true;
                        AcroManager.Instance.MeaningChecked += meaningChecked;
                        AcroManager.Instance.MeaningValidated += meaningValidated;
                        AcroManager.Instance.ValidateMeanings();
                        break;
                }
            }
            catch (Exception ex)
            {
                exceptionHappened = ex;
            }
            finally
            {
                switch (currentWork)
                {
                    case WorkType.LeechResources:
                        AcroManager.Instance.ResourceLeeched -= resourceLeeched;
                        AcroManager.Instance.AcronymCreated -= acronymCreated;
                        AcroManager.Instance.MeaningCreated -= meaningCreated;
                        break;
                    case WorkType.ValidateAcronyms:
                        EstimationsVisible = false;
                        AcroManager.Instance.MeaningChecked -= meaningChecked;
                        AcroManager.Instance.MeaningValidated -= meaningValidated;
                        break;
                }

                TimerEnabled = false;

                getDatabaseStatus();
                RefreshDatabaseStatus();
                
                Cursor.Current = Cursors.Default;

                if (exceptionHappened != null)
                {
                    MessageBox.Show(string.Format("There has been an error in the process:{0}{1}",
                                                  Environment.NewLine,
                                                  exceptionHappened),
                                    "Error");
                }

                FormEnabled = true;
            }
        }

        private void meaningCreated(string acronym, string meaning)
        {
            databaseStatus.NumberOfMeaningsParsed++;
            RefreshDatabaseStatus();
        }

        private void meaningChecked(string acronym, string meaning)
        {
            databaseStatus.NumberOfMeaningsChecked++; ;
            RefreshDatabaseStatus();
        }

        private void meaningValidated(string acronym, string meaning)
        {
            databaseStatus.NumberOfMeaningsValidated++;
            RefreshDatabaseStatus();
        }

        private void acronymCreated(string acronym)
        {
            databaseStatus.NumberOfAcronymsExtracted++;
            RefreshDatabaseStatus();
        }

        void resourceLeeched(AcroniManager.Core.Information.ResourceInformation information)
        {
            databaseStatus.NumberOfResourcesLeeched++;
            RefreshDatabaseStatus();
        }

        const string timespanFormatString = @"d\d\ hh\h\ mm\m\ ss\s";

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - startTime.Ticks);
            lblTimeElapsed.Text = elapsedTime.ToString(timespanFormatString);
            DatabaseStatus currentStatus = databaseStatus;

            if (currentWork == WorkType.ValidateAcronyms)
            {
                long checksPerformed = currentStatus.NumberOfMeaningsChecked - initialNumberOfMeaningsChecked;

                if (currentStatus.NumberOfMeaningsParsed - initialNumberOfMeaningsChecked > 0)
                {
                    int value = Convert.ToInt32(Convert.ToDouble(checksPerformed)
                                                        / Convert.ToDouble(currentStatus.NumberOfMeaningsParsed - initialNumberOfMeaningsChecked)
                                                        * Convert.ToDouble(barProgress.Maximum));

                    barProgress.Value = value > barProgress.Maximum ? barProgress.Maximum : value;
                }
                else
                {
                    barProgress.Value = barProgress.Maximum;
                }

                barProgress.Refresh();

                if (checksPerformed == 0)
                {
                    lblTimeEstimated.Text = "Infinite";
                }
                else
                {
                    lblTimeEstimated.Text = TimeSpan.FromTicks(Convert.ToInt64(elapsedTime.Ticks
                                                                               / checksPerformed
                                                                               * (currentStatus.NumberOfMeaningsParsed - currentStatus.NumberOfMeaningsChecked))
                                                              ).ToString(timespanFormatString);
                }
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            getDatabaseStatus();
            printDatabaseStatus();
        }        
    }
}
