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

namespace AcroniManager.AcroBrowser
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
            this.tabControlContent = new System.Windows.Forms.TabControl();
            this.tabPageBrowser = new System.Windows.Forms.TabPage();
            this.lblMeaningCaption = new System.Windows.Forms.Label();
            this.lvwMeaning = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgLstAcronymIcon = new System.Windows.Forms.ImageList(this.components);
            this.lblAcronymCaption = new System.Windows.Forms.Label();
            this.lbxAcronym = new System.Windows.Forms.ListBox();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.rdoValidations = new System.Windows.Forms.RadioButton();
            this.rdoMatches = new System.Windows.Forms.RadioButton();
            this.rdoResources = new System.Windows.Forms.RadioButton();
            this.rdoCategories = new System.Windows.Forms.RadioButton();
            this.lblNoOfCategories = new System.Windows.Forms.Label();
            this.lblNoOfCategoriesCaption = new System.Windows.Forms.Label();
            this.lblNoOfValidations = new System.Windows.Forms.Label();
            this.lblNoOfMatches = new System.Windows.Forms.Label();
            this.lblNoOfResources = new System.Windows.Forms.Label();
            this.lblNoOfValidationsCaption = new System.Windows.Forms.Label();
            this.lblNoOfMatchesCaption = new System.Windows.Forms.Label();
            this.lblNoOfResourcesCaption = new System.Windows.Forms.Label();
            this.lblDetailsCaption = new System.Windows.Forms.Label();
            this.lbxDetails = new System.Windows.Forms.ListBox();
            this.lblSearchStringCaption = new System.Windows.Forms.Label();
            this.txtSearchString = new System.Windows.Forms.TextBox();
            this.tabPageReplacer = new System.Windows.Forms.TabPage();
            this.btnReplaceAll = new System.Windows.Forms.Button();
            this.rtxtInput = new System.Windows.Forms.RichTextBox();
            this.ttpAcronym = new System.Windows.Forms.ToolTip(this.components);
            this.chkShowExplanation = new System.Windows.Forms.CheckBox();
            this.tabControlContent.SuspendLayout();
            this.tabPageBrowser.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.tabPageReplacer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlContent
            // 
            this.tabControlContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlContent.Controls.Add(this.tabPageBrowser);
            this.tabControlContent.Controls.Add(this.tabPageReplacer);
            this.tabControlContent.Location = new System.Drawing.Point(0, 0);
            this.tabControlContent.Name = "tabControlContent";
            this.tabControlContent.SelectedIndex = 0;
            this.tabControlContent.Size = new System.Drawing.Size(623, 442);
            this.tabControlContent.TabIndex = 100;
            this.tabControlContent.SelectedIndexChanged += new System.EventHandler(this.tabControlContent_SelectedIndexChanged);
            // 
            // tabPageBrowser
            // 
            this.tabPageBrowser.Controls.Add(this.lblMeaningCaption);
            this.tabPageBrowser.Controls.Add(this.lvwMeaning);
            this.tabPageBrowser.Controls.Add(this.lblAcronymCaption);
            this.tabPageBrowser.Controls.Add(this.lbxAcronym);
            this.tabPageBrowser.Controls.Add(this.pnlDetails);
            this.tabPageBrowser.Controls.Add(this.lblSearchStringCaption);
            this.tabPageBrowser.Controls.Add(this.txtSearchString);
            this.tabPageBrowser.Location = new System.Drawing.Point(4, 22);
            this.tabPageBrowser.Name = "tabPageBrowser";
            this.tabPageBrowser.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBrowser.Size = new System.Drawing.Size(615, 416);
            this.tabPageBrowser.TabIndex = 0;
            this.tabPageBrowser.Text = "Acronyms Browser";
            // 
            // lblMeaningCaption
            // 
            this.lblMeaningCaption.AutoSize = true;
            this.lblMeaningCaption.Location = new System.Drawing.Point(158, 45);
            this.lblMeaningCaption.Name = "lblMeaningCaption";
            this.lblMeaningCaption.Size = new System.Drawing.Size(51, 13);
            this.lblMeaningCaption.TabIndex = 10;
            this.lblMeaningCaption.Text = "Meaning:";
            // 
            // lvwMeaning
            // 
            this.lvwMeaning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwMeaning.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvwMeaning.FullRowSelect = true;
            this.lvwMeaning.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwMeaning.HideSelection = false;
            this.lvwMeaning.Location = new System.Drawing.Point(161, 61);
            this.lvwMeaning.MultiSelect = false;
            this.lvwMeaning.Name = "lvwMeaning";
            this.lvwMeaning.Size = new System.Drawing.Size(447, 264);
            this.lvwMeaning.SmallImageList = this.imgLstAcronymIcon;
            this.lvwMeaning.TabIndex = 20;
            this.lvwMeaning.UseCompatibleStateImageBehavior = false;
            this.lvwMeaning.View = System.Windows.Forms.View.Details;
            this.lvwMeaning.SelectedIndexChanged += new System.EventHandler(this.lvwMeaning_SelectedIndexChanged);
            this.lvwMeaning.KeyDown += new System.Windows.Forms.KeyEventHandler(this.navigationControl_KeyDown);
            this.lvwMeaning.Resize += new System.EventHandler(this.lvwMeaning_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 443;
            // 
            // imgLstAcronymIcon
            // 
            this.imgLstAcronymIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLstAcronymIcon.ImageStream")));
            this.imgLstAcronymIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imgLstAcronymIcon.Images.SetKeyName(0, "Blank Icon.jpg");
            this.imgLstAcronymIcon.Images.SetKeyName(1, "Green Tick Icon.jpg");
            // 
            // lblAcronymCaption
            // 
            this.lblAcronymCaption.AutoSize = true;
            this.lblAcronymCaption.Location = new System.Drawing.Point(6, 45);
            this.lblAcronymCaption.Name = "lblAcronymCaption";
            this.lblAcronymCaption.Size = new System.Drawing.Size(51, 13);
            this.lblAcronymCaption.TabIndex = 8;
            this.lblAcronymCaption.Text = "Acronym:";
            // 
            // lbxAcronym
            // 
            this.lbxAcronym.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxAcronym.DisplayMember = "Caption";
            this.lbxAcronym.FormattingEnabled = true;
            this.lbxAcronym.Location = new System.Drawing.Point(9, 61);
            this.lbxAcronym.Name = "lbxAcronym";
            this.lbxAcronym.Size = new System.Drawing.Size(145, 264);
            this.lbxAcronym.TabIndex = 10;
            this.lbxAcronym.DataSourceChanged += new System.EventHandler(this.lbxAcronym_SelectedValueChanged);
            this.lbxAcronym.SelectedValueChanged += new System.EventHandler(this.lbxAcronym_SelectedValueChanged);
            this.lbxAcronym.KeyDown += new System.Windows.Forms.KeyEventHandler(this.navigationControl_KeyDown);
            // 
            // pnlDetails
            // 
            this.pnlDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDetails.Controls.Add(this.rdoValidations);
            this.pnlDetails.Controls.Add(this.rdoMatches);
            this.pnlDetails.Controls.Add(this.rdoResources);
            this.pnlDetails.Controls.Add(this.rdoCategories);
            this.pnlDetails.Controls.Add(this.lblNoOfCategories);
            this.pnlDetails.Controls.Add(this.lblNoOfCategoriesCaption);
            this.pnlDetails.Controls.Add(this.lblNoOfValidations);
            this.pnlDetails.Controls.Add(this.lblNoOfMatches);
            this.pnlDetails.Controls.Add(this.lblNoOfResources);
            this.pnlDetails.Controls.Add(this.lblNoOfValidationsCaption);
            this.pnlDetails.Controls.Add(this.lblNoOfMatchesCaption);
            this.pnlDetails.Controls.Add(this.lblNoOfResourcesCaption);
            this.pnlDetails.Controls.Add(this.lblDetailsCaption);
            this.pnlDetails.Controls.Add(this.lbxDetails);
            this.pnlDetails.Location = new System.Drawing.Point(9, 331);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(600, 81);
            this.pnlDetails.TabIndex = 30;
            this.pnlDetails.Visible = false;
            // 
            // rdoValidations
            // 
            this.rdoValidations.AutoSize = true;
            this.rdoValidations.Location = new System.Drawing.Point(9, 64);
            this.rdoValidations.Name = "rdoValidations";
            this.rdoValidations.Size = new System.Drawing.Size(14, 13);
            this.rdoValidations.TabIndex = 52;
            this.rdoValidations.UseVisualStyleBackColor = true;
            this.rdoValidations.CheckedChanged += new System.EventHandler(this.rdoDetails_CheckedChanged);
            // 
            // rdoMatches
            // 
            this.rdoMatches.AutoSize = true;
            this.rdoMatches.Location = new System.Drawing.Point(9, 44);
            this.rdoMatches.Name = "rdoMatches";
            this.rdoMatches.Size = new System.Drawing.Size(14, 13);
            this.rdoMatches.TabIndex = 51;
            this.rdoMatches.UseVisualStyleBackColor = true;
            this.rdoMatches.CheckedChanged += new System.EventHandler(this.rdoDetails_CheckedChanged);
            // 
            // rdoResources
            // 
            this.rdoResources.AutoSize = true;
            this.rdoResources.Location = new System.Drawing.Point(9, 24);
            this.rdoResources.Name = "rdoResources";
            this.rdoResources.Size = new System.Drawing.Size(14, 13);
            this.rdoResources.TabIndex = 50;
            this.rdoResources.UseVisualStyleBackColor = true;
            this.rdoResources.CheckedChanged += new System.EventHandler(this.rdoDetails_CheckedChanged);
            // 
            // rdoCategories
            // 
            this.rdoCategories.AutoSize = true;
            this.rdoCategories.Checked = true;
            this.rdoCategories.Location = new System.Drawing.Point(9, 4);
            this.rdoCategories.Name = "rdoCategories";
            this.rdoCategories.Size = new System.Drawing.Size(14, 13);
            this.rdoCategories.TabIndex = 49;
            this.rdoCategories.TabStop = true;
            this.rdoCategories.UseVisualStyleBackColor = true;
            this.rdoCategories.CheckedChanged += new System.EventHandler(this.rdoDetails_CheckedChanged);
            // 
            // lblNoOfCategories
            // 
            this.lblNoOfCategories.AutoSize = true;
            this.lblNoOfCategories.Location = new System.Drawing.Point(127, 4);
            this.lblNoOfCategories.Name = "lblNoOfCategories";
            this.lblNoOfCategories.Size = new System.Drawing.Size(97, 13);
            this.lblNoOfCategories.TabIndex = 48;
            this.lblNoOfCategories.Text = "999999999999999";
            // 
            // lblNoOfCategoriesCaption
            // 
            this.lblNoOfCategoriesCaption.AutoSize = true;
            this.lblNoOfCategoriesCaption.Location = new System.Drawing.Point(29, 4);
            this.lblNoOfCategoriesCaption.Name = "lblNoOfCategoriesCaption";
            this.lblNoOfCategoriesCaption.Size = new System.Drawing.Size(92, 13);
            this.lblNoOfCategoriesCaption.TabIndex = 47;
            this.lblNoOfCategoriesCaption.Text = "No. of Categories:";
            // 
            // lblNoOfValidations
            // 
            this.lblNoOfValidations.AutoSize = true;
            this.lblNoOfValidations.Location = new System.Drawing.Point(127, 64);
            this.lblNoOfValidations.Name = "lblNoOfValidations";
            this.lblNoOfValidations.Size = new System.Drawing.Size(97, 13);
            this.lblNoOfValidations.TabIndex = 44;
            this.lblNoOfValidations.Text = "999999999999999";
            // 
            // lblNoOfMatches
            // 
            this.lblNoOfMatches.AutoSize = true;
            this.lblNoOfMatches.Location = new System.Drawing.Point(127, 44);
            this.lblNoOfMatches.Name = "lblNoOfMatches";
            this.lblNoOfMatches.Size = new System.Drawing.Size(97, 13);
            this.lblNoOfMatches.TabIndex = 45;
            this.lblNoOfMatches.Text = "999999999999999";
            // 
            // lblNoOfResources
            // 
            this.lblNoOfResources.AutoSize = true;
            this.lblNoOfResources.Location = new System.Drawing.Point(127, 24);
            this.lblNoOfResources.Name = "lblNoOfResources";
            this.lblNoOfResources.Size = new System.Drawing.Size(97, 13);
            this.lblNoOfResources.TabIndex = 46;
            this.lblNoOfResources.Text = "999999999999999";
            // 
            // lblNoOfValidationsCaption
            // 
            this.lblNoOfValidationsCaption.AutoSize = true;
            this.lblNoOfValidationsCaption.Location = new System.Drawing.Point(29, 64);
            this.lblNoOfValidationsCaption.Name = "lblNoOfValidationsCaption";
            this.lblNoOfValidationsCaption.Size = new System.Drawing.Size(93, 13);
            this.lblNoOfValidationsCaption.TabIndex = 43;
            this.lblNoOfValidationsCaption.Text = "No. of Validations:";
            // 
            // lblNoOfMatchesCaption
            // 
            this.lblNoOfMatchesCaption.AutoSize = true;
            this.lblNoOfMatchesCaption.Location = new System.Drawing.Point(29, 44);
            this.lblNoOfMatchesCaption.Name = "lblNoOfMatchesCaption";
            this.lblNoOfMatchesCaption.Size = new System.Drawing.Size(83, 13);
            this.lblNoOfMatchesCaption.TabIndex = 42;
            this.lblNoOfMatchesCaption.Text = "No. of Matches:";
            // 
            // lblNoOfResourcesCaption
            // 
            this.lblNoOfResourcesCaption.AutoSize = true;
            this.lblNoOfResourcesCaption.Location = new System.Drawing.Point(29, 24);
            this.lblNoOfResourcesCaption.Name = "lblNoOfResourcesCaption";
            this.lblNoOfResourcesCaption.Size = new System.Drawing.Size(93, 13);
            this.lblNoOfResourcesCaption.TabIndex = 41;
            this.lblNoOfResourcesCaption.Text = "No. of Resources:";
            // 
            // lblDetailsCaption
            // 
            this.lblDetailsCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDetailsCaption.AutoSize = true;
            this.lblDetailsCaption.Location = new System.Drawing.Point(241, 34);
            this.lblDetailsCaption.Name = "lblDetailsCaption";
            this.lblDetailsCaption.Size = new System.Drawing.Size(60, 13);
            this.lblDetailsCaption.TabIndex = 9;
            this.lblDetailsCaption.Text = "Categories:";
            this.lblDetailsCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbxDetails
            // 
            this.lbxDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxDetails.FormattingEnabled = true;
            this.lbxDetails.Location = new System.Drawing.Point(307, 4);
            this.lbxDetails.Name = "lbxDetails";
            this.lbxDetails.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbxDetails.Size = new System.Drawing.Size(290, 69);
            this.lbxDetails.TabIndex = 40;
            // 
            // lblSearchStringCaption
            // 
            this.lblSearchStringCaption.AutoSize = true;
            this.lblSearchStringCaption.Location = new System.Drawing.Point(6, 20);
            this.lblSearchStringCaption.Name = "lblSearchStringCaption";
            this.lblSearchStringCaption.Size = new System.Drawing.Size(64, 13);
            this.lblSearchStringCaption.TabIndex = 2;
            this.lblSearchStringCaption.Text = "Search text:";
            // 
            // txtSearchString
            // 
            this.txtSearchString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchString.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearchString.Location = new System.Drawing.Point(76, 17);
            this.txtSearchString.Name = "txtSearchString";
            this.txtSearchString.Size = new System.Drawing.Size(532, 20);
            this.txtSearchString.TabIndex = 0;
            this.txtSearchString.TextChanged += new System.EventHandler(this.txtSearchString_TextChanged);
            this.txtSearchString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.navigationControl_KeyDown);
            // 
            // tabPageReplacer
            // 
            this.tabPageReplacer.Controls.Add(this.chkShowExplanation);
            this.tabPageReplacer.Controls.Add(this.btnReplaceAll);
            this.tabPageReplacer.Controls.Add(this.rtxtInput);
            this.tabPageReplacer.Location = new System.Drawing.Point(4, 22);
            this.tabPageReplacer.Name = "tabPageReplacer";
            this.tabPageReplacer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageReplacer.Size = new System.Drawing.Size(615, 416);
            this.tabPageReplacer.TabIndex = 1;
            this.tabPageReplacer.Text = "Acronyms Replacer";
            // 
            // btnReplaceAll
            // 
            this.btnReplaceAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReplaceAll.Location = new System.Drawing.Point(533, 387);
            this.btnReplaceAll.Name = "btnReplaceAll";
            this.btnReplaceAll.Size = new System.Drawing.Size(75, 23);
            this.btnReplaceAll.TabIndex = 2;
            this.btnReplaceAll.Text = "Replace All";
            this.btnReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
            // 
            // rtxtInput
            // 
            this.rtxtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtInput.Location = new System.Drawing.Point(7, 7);
            this.rtxtInput.Name = "rtxtInput";
            this.rtxtInput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtInput.Size = new System.Drawing.Size(601, 374);
            this.rtxtInput.TabIndex = 0;
            this.rtxtInput.Text = "";
            this.rtxtInput.Click += new System.EventHandler(this.rtxtInput_Click);
            this.rtxtInput.TextChanged += new System.EventHandler(this.rtxtInput_TextChanged);
            this.rtxtInput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rtxtInput_MouseMove);
            // 
            // chkShowExplanation
            // 
            this.chkShowExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowExplanation.AutoSize = true;
            this.chkShowExplanation.Location = new System.Drawing.Point(9, 388);
            this.chkShowExplanation.Name = "chkShowExplanation";
            this.chkShowExplanation.Size = new System.Drawing.Size(115, 17);
            this.chkShowExplanation.TabIndex = 1;
            this.chkShowExplanation.Text = "Show explanations";
            this.chkShowExplanation.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.tabControlContent);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "FrmMain";
            this.Text = "AcroBrowser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.tabControlContent.ResumeLayout(false);
            this.tabPageBrowser.ResumeLayout(false);
            this.tabPageBrowser.PerformLayout();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            this.tabPageReplacer.ResumeLayout(false);
            this.tabPageReplacer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlContent;
        private System.Windows.Forms.TabPage tabPageBrowser;
        private System.Windows.Forms.Label lblSearchStringCaption;
        private System.Windows.Forms.TextBox txtSearchString;
        private System.Windows.Forms.TabPage tabPageReplacer;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.Label lblMeaningCaption;
        private System.Windows.Forms.ListView lvwMeaning;
        private System.Windows.Forms.Label lblAcronymCaption;
        private System.Windows.Forms.ListBox lbxAcronym;
        private System.Windows.Forms.ImageList imgLstAcronymIcon;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListBox lbxDetails;
        private System.Windows.Forms.Label lblDetailsCaption;
        private System.Windows.Forms.Label lblNoOfMatchesCaption;
        private System.Windows.Forms.Label lblNoOfResourcesCaption;
        private System.Windows.Forms.Label lblNoOfValidationsCaption;
        private System.Windows.Forms.Label lblNoOfValidations;
        private System.Windows.Forms.Label lblNoOfMatches;
        private System.Windows.Forms.Label lblNoOfResources;
        private System.Windows.Forms.Label lblNoOfCategories;
        private System.Windows.Forms.Label lblNoOfCategoriesCaption;
        private System.Windows.Forms.RadioButton rdoValidations;
        private System.Windows.Forms.RadioButton rdoMatches;
        private System.Windows.Forms.RadioButton rdoResources;
        private System.Windows.Forms.RadioButton rdoCategories;
        private System.Windows.Forms.Button btnReplaceAll;
        private System.Windows.Forms.RichTextBox rtxtInput;
        private System.Windows.Forms.ToolTip ttpAcronym;
        private System.Windows.Forms.CheckBox chkShowExplanation;
    }
}

