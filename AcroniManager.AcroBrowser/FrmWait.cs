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
using System.ComponentModel;
using System.Windows.Forms;

namespace AcroniManager.AcroBrowser
{
    public delegate void AsyncAction(FrmWait frmWait);

    public partial class FrmWait : Form
    {
        private Func<FrmWait, object, object> _loadAction;
        
        public Func<FrmWait, object, object> AsyncProcess
        {
            set
            {
                _loadAction = value;
            }
        }

        public object InputParameter { get; set; }
        public object OutputParameter { get; internal set; }
        public bool HasFailed { get; private set; }
        public Exception ThrownException { get; private set; }

        public FrmWait()
        {
            InitializeComponent();
        }

        private void FrmWait_TextChanged(object sender, EventArgs e)
        {
            lblInfo.Text = Text;
        }

        private void FrmWait_Load(object sender, EventArgs e)
        {
            bgwAsyncProcess.RunWorkerAsync();
        }

        private void bgwAsyncProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (_loadAction != null)
                {
                    OutputParameter = _loadAction(this, InputParameter);
                }
            }
            catch (Exception ex)
            {
                ThrownException = ex;
                HasFailed = true;
            }
        }

        private void bgwAsyncProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = HasFailed ? DialogResult.No : DialogResult.Yes;
        }

        public void SetupProgressBar(int maximum)
        {
            if (this.InvokeRequired)
            {
                Invoke(new NotifyProgressBar(setupProgressBar), maximum);
            }
            else
            {
                setupProgressBar(maximum);
            }
        }

        public void UpdateProgressBar(int value)
        {
            if (this.InvokeRequired)
            {
                Invoke(new NotifyProgressBar(updateProgressBar), value);
            }
            else
            {
                updateProgressBar(value);
            }
        }

        private delegate void NotifyProgressBar(int quantity);

        private void setupProgressBar(int maximum)
        {
            barProgress.Minimum = 0;
            barProgress.Maximum = maximum;
            barProgress.Value = 0;
            barProgress.Refresh();
        }

        private void updateProgressBar(int value)
        {
            barProgress.Value = value;
            barProgress.Refresh();
        }                      
    }
}
