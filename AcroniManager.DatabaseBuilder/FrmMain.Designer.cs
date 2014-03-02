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

namespace AcroniManager.DatabaseBuilder
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.btnStartLeeching = new System.Windows.Forms.Button();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.lblResourcesLeechedCaption = new System.Windows.Forms.Label();
            this.lblResourcesLeeched = new System.Windows.Forms.Label();
            this.lblAcronymsExtractedCaption = new System.Windows.Forms.Label();
            this.lblAcronymsExtracted = new System.Windows.Forms.Label();
            this.lblMeaningsParsedCaption = new System.Windows.Forms.Label();
            this.lblMeaningsParsed = new System.Windows.Forms.Label();
            this.lblMeaningsCheckedCaption = new System.Windows.Forms.Label();
            this.lblMeaningsChecked = new System.Windows.Forms.Label();
            this.lblMeaningsValidatedCaption = new System.Windows.Forms.Label();
            this.lblMeaningsValidated = new System.Windows.Forms.Label();
            this.barProgress = new System.Windows.Forms.ProgressBar();
            this.lblTimeElapsedCaption = new System.Windows.Forms.Label();
            this.lblTimeElapsed = new System.Windows.Forms.Label();
            this.lblTimeEstimatedCaption = new System.Windows.Forms.Label();
            this.lblTimeEstimated = new System.Windows.Forms.Label();
            this.btnStartValidating = new System.Windows.Forms.Button();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnStartLeeching
            // 
            this.btnStartLeeching.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartLeeching.Location = new System.Drawing.Point(12, 206);
            this.btnStartLeeching.Name = "btnStartLeeching";
            this.btnStartLeeching.Size = new System.Drawing.Size(107, 23);
            this.btnStartLeeching.TabIndex = 0;
            this.btnStartLeeching.Text = "Leech";
            this.btnStartLeeching.UseVisualStyleBackColor = true;
            this.btnStartLeeching.Click += new System.EventHandler(this.btnStartLeeching_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoWork);
            // 
            // lblResourcesLeechedCaption
            // 
            this.lblResourcesLeechedCaption.AutoSize = true;
            this.lblResourcesLeechedCaption.Location = new System.Drawing.Point(13, 13);
            this.lblResourcesLeechedCaption.Name = "lblResourcesLeechedCaption";
            this.lblResourcesLeechedCaption.Size = new System.Drawing.Size(106, 13);
            this.lblResourcesLeechedCaption.TabIndex = 3;
            this.lblResourcesLeechedCaption.Text = "Resources Leeched:";
            // 
            // lblResourcesLeeched
            // 
            this.lblResourcesLeeched.AutoSize = true;
            this.lblResourcesLeeched.Location = new System.Drawing.Point(125, 13);
            this.lblResourcesLeeched.Name = "lblResourcesLeeched";
            this.lblResourcesLeeched.Size = new System.Drawing.Size(97, 13);
            this.lblResourcesLeeched.TabIndex = 4;
            this.lblResourcesLeeched.Text = "999999999999999";
            // 
            // lblAcronymsExtractedCaption
            // 
            this.lblAcronymsExtractedCaption.AutoSize = true;
            this.lblAcronymsExtractedCaption.Location = new System.Drawing.Point(15, 35);
            this.lblAcronymsExtractedCaption.Name = "lblAcronymsExtractedCaption";
            this.lblAcronymsExtractedCaption.Size = new System.Drawing.Size(104, 13);
            this.lblAcronymsExtractedCaption.TabIndex = 3;
            this.lblAcronymsExtractedCaption.Text = "Acronyms Extracted:";
            // 
            // lblAcronymsExtracted
            // 
            this.lblAcronymsExtracted.AutoSize = true;
            this.lblAcronymsExtracted.Location = new System.Drawing.Point(125, 35);
            this.lblAcronymsExtracted.Name = "lblAcronymsExtracted";
            this.lblAcronymsExtracted.Size = new System.Drawing.Size(97, 13);
            this.lblAcronymsExtracted.TabIndex = 4;
            this.lblAcronymsExtracted.Text = "999999999999999";
            // 
            // lblMeaningsParsedCaption
            // 
            this.lblMeaningsParsedCaption.AutoSize = true;
            this.lblMeaningsParsedCaption.Location = new System.Drawing.Point(27, 57);
            this.lblMeaningsParsedCaption.Name = "lblMeaningsParsedCaption";
            this.lblMeaningsParsedCaption.Size = new System.Drawing.Size(92, 13);
            this.lblMeaningsParsedCaption.TabIndex = 3;
            this.lblMeaningsParsedCaption.Text = "Meanings Parsed:";
            // 
            // lblMeaningsParsed
            // 
            this.lblMeaningsParsed.AutoSize = true;
            this.lblMeaningsParsed.Location = new System.Drawing.Point(125, 57);
            this.lblMeaningsParsed.Name = "lblMeaningsParsed";
            this.lblMeaningsParsed.Size = new System.Drawing.Size(97, 13);
            this.lblMeaningsParsed.TabIndex = 4;
            this.lblMeaningsParsed.Text = "999999999999999";
            // 
            // lblMeaningsCheckedCaption
            // 
            this.lblMeaningsCheckedCaption.AutoSize = true;
            this.lblMeaningsCheckedCaption.Location = new System.Drawing.Point(17, 79);
            this.lblMeaningsCheckedCaption.Name = "lblMeaningsCheckedCaption";
            this.lblMeaningsCheckedCaption.Size = new System.Drawing.Size(102, 13);
            this.lblMeaningsCheckedCaption.TabIndex = 3;
            this.lblMeaningsCheckedCaption.Text = "Meanings Checked:";
            // 
            // lblMeaningsChecked
            // 
            this.lblMeaningsChecked.AutoSize = true;
            this.lblMeaningsChecked.Location = new System.Drawing.Point(125, 79);
            this.lblMeaningsChecked.Name = "lblMeaningsChecked";
            this.lblMeaningsChecked.Size = new System.Drawing.Size(97, 13);
            this.lblMeaningsChecked.TabIndex = 4;
            this.lblMeaningsChecked.Text = "999999999999999";
            // 
            // lblMeaningsValidatedCaption
            // 
            this.lblMeaningsValidatedCaption.AutoSize = true;
            this.lblMeaningsValidatedCaption.Location = new System.Drawing.Point(16, 102);
            this.lblMeaningsValidatedCaption.Name = "lblMeaningsValidatedCaption";
            this.lblMeaningsValidatedCaption.Size = new System.Drawing.Size(103, 13);
            this.lblMeaningsValidatedCaption.TabIndex = 3;
            this.lblMeaningsValidatedCaption.Text = "Meanings Validated:";
            // 
            // lblMeaningsValidated
            // 
            this.lblMeaningsValidated.AutoSize = true;
            this.lblMeaningsValidated.Location = new System.Drawing.Point(125, 102);
            this.lblMeaningsValidated.Name = "lblMeaningsValidated";
            this.lblMeaningsValidated.Size = new System.Drawing.Size(97, 13);
            this.lblMeaningsValidated.TabIndex = 4;
            this.lblMeaningsValidated.Text = "999999999999999";
            // 
            // barProgress
            // 
            this.barProgress.Location = new System.Drawing.Point(12, 171);
            this.barProgress.Maximum = 100000;
            this.barProgress.Name = "barProgress";
            this.barProgress.Size = new System.Drawing.Size(282, 23);
            this.barProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.barProgress.TabIndex = 5;
            this.barProgress.Visible = false;
            // 
            // lblTimeElapsedCaption
            // 
            this.lblTimeElapsedCaption.AutoSize = true;
            this.lblTimeElapsedCaption.Location = new System.Drawing.Point(45, 124);
            this.lblTimeElapsedCaption.Name = "lblTimeElapsedCaption";
            this.lblTimeElapsedCaption.Size = new System.Drawing.Size(74, 13);
            this.lblTimeElapsedCaption.TabIndex = 3;
            this.lblTimeElapsedCaption.Text = "Elapsed Time:";
            // 
            // lblTimeElapsed
            // 
            this.lblTimeElapsed.AutoSize = true;
            this.lblTimeElapsed.Location = new System.Drawing.Point(125, 124);
            this.lblTimeElapsed.Name = "lblTimeElapsed";
            this.lblTimeElapsed.Size = new System.Drawing.Size(89, 13);
            this.lblTimeElapsed.TabIndex = 4;
            this.lblTimeElapsed.Text = "99d 23h 59m 59s";
            // 
            // lblTimeEstimatedCaption
            // 
            this.lblTimeEstimatedCaption.AutoSize = true;
            this.lblTimeEstimatedCaption.Location = new System.Drawing.Point(37, 146);
            this.lblTimeEstimatedCaption.Name = "lblTimeEstimatedCaption";
            this.lblTimeEstimatedCaption.Size = new System.Drawing.Size(82, 13);
            this.lblTimeEstimatedCaption.TabIndex = 3;
            this.lblTimeEstimatedCaption.Text = "Estimated Time:";
            this.lblTimeEstimatedCaption.Visible = false;
            // 
            // lblTimeEstimated
            // 
            this.lblTimeEstimated.AutoSize = true;
            this.lblTimeEstimated.Location = new System.Drawing.Point(125, 146);
            this.lblTimeEstimated.Name = "lblTimeEstimated";
            this.lblTimeEstimated.Size = new System.Drawing.Size(89, 13);
            this.lblTimeEstimated.TabIndex = 4;
            this.lblTimeEstimated.Text = "99d 23h 59m 59s";
            this.lblTimeEstimated.Visible = false;
            // 
            // btnStartValidating
            // 
            this.btnStartValidating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartValidating.Location = new System.Drawing.Point(187, 206);
            this.btnStartValidating.Name = "btnStartValidating";
            this.btnStartValidating.Size = new System.Drawing.Size(107, 23);
            this.btnStartValidating.TabIndex = 0;
            this.btnStartValidating.Text = "Validate";
            this.btnStartValidating.UseVisualStyleBackColor = true;
            this.btnStartValidating.Click += new System.EventHandler(this.btnStartValidating_Click);
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // FrmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(306, 241);
            this.Controls.Add(this.barProgress);
            this.Controls.Add(this.lblTimeEstimated);
            this.Controls.Add(this.lblTimeElapsed);
            this.Controls.Add(this.lblMeaningsValidated);
            this.Controls.Add(this.lblMeaningsChecked);
            this.Controls.Add(this.lblMeaningsParsed);
            this.Controls.Add(this.lblAcronymsExtracted);
            this.Controls.Add(this.lblResourcesLeeched);
            this.Controls.Add(this.lblTimeEstimatedCaption);
            this.Controls.Add(this.lblTimeElapsedCaption);
            this.Controls.Add(this.lblMeaningsValidatedCaption);
            this.Controls.Add(this.lblMeaningsCheckedCaption);
            this.Controls.Add(this.lblMeaningsParsedCaption);
            this.Controls.Add(this.lblAcronymsExtractedCaption);
            this.Controls.Add(this.lblResourcesLeechedCaption);
            this.Controls.Add(this.btnStartValidating);
            this.Controls.Add(this.btnStartLeeching);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.Text = "AcroniManager Database Builder";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartLeeching;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Label lblResourcesLeechedCaption;
        private System.Windows.Forms.Label lblResourcesLeeched;
        private System.Windows.Forms.Label lblAcronymsExtractedCaption;
        private System.Windows.Forms.Label lblAcronymsExtracted;
        private System.Windows.Forms.Label lblMeaningsParsedCaption;
        private System.Windows.Forms.Label lblMeaningsParsed;
        private System.Windows.Forms.Label lblMeaningsCheckedCaption;
        private System.Windows.Forms.Label lblMeaningsChecked;
        private System.Windows.Forms.Label lblMeaningsValidatedCaption;
        private System.Windows.Forms.Label lblMeaningsValidated;
        private System.Windows.Forms.ProgressBar barProgress;
        private System.Windows.Forms.Label lblTimeElapsedCaption;
        private System.Windows.Forms.Label lblTimeElapsed;
        private System.Windows.Forms.Label lblTimeEstimatedCaption;
        private System.Windows.Forms.Label lblTimeEstimated;
        private System.Windows.Forms.Button btnStartValidating;
        private System.Windows.Forms.Timer refreshTimer;
    }
}

