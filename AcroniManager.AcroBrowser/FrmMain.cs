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

using AcroniManager.Core.Data;
using AcroniManager.Core.Information;
using AcroniManager.Core.Inquisitor;
using AcroniManager.Core.MeaningSelector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AcroniManager.AcroBrowser
{    
    public partial class FrmMain : Form
    {
        private const string _tabPageBrowserName = "tabPageBrowser";
        private const string _tabPageReplacerName = "tabPageReplacer";

        AcroInquisitor _acroInquisitor;

        public FrmMain()
        {
            InitializeComponent();
            _acroInquisitor = new AcroInquisitor();
        }

        public new void Dispose()
        {
            _acroInquisitor.Dispose();
            base.Dispose();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            tabControlContent_SelectedIndexChanged(sender, e);
        }

        private void tabControlContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlContent.SelectedTab.Name)
            {
                case _tabPageBrowserName:
                    txtSearchString.Select();
                    break;
                case _tabPageReplacerName:
                    rtxtInput.Select();
                    break;
            }
        }

        #region Browsing Tab

        private void txtSearchString_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (string.IsNullOrWhiteSpace(txtSearchString.Text)
                    || txtSearchString.Text.Trim().Length == 1)
                {
                    lbxAcronym.DataSource = new List<Acronym>();
                    lbxAcronym_SelectedValueChanged(sender, e);
                }
                else
                {
                    var acronyms = _acroInquisitor.SearchAcronyms(txtSearchString.Text.Trim().Replace(".", string.Empty))
                                                 .OrderBy(x => x.Caption)
                                                 .AsEnumerable()
                                                 .ToList();
                    lbxAcronym.DataSource = acronyms;

                    if (!acronyms.Any())
                    {
                        lbxAcronym_SelectedValueChanged(sender, e);
                    }
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void lbxAcronym_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                lvwMeaning.Items.Clear();
                if (lbxAcronym.SelectedItem != null)
                {
                    Acronym acronymSelected = lbxAcronym.SelectedItem as Acronym;
                    lvwMeaning.Items.AddRange(acronymSelected.Meanings
                                                             .OrderBy(x => x.Caption)
                                                             .Select(x => new ListViewItem(x.Caption,
                                                                                           x.Validations.Any(y => y.Validated) ? 1 : 0))
                                                             .ToArray());
                }
                if (lvwMeaning.Items.Count > 0)
                {
                    lvwMeaning.Items[0].Selected = true;
                }
                else
                {
                    lvwMeaning_SelectedIndexChanged(sender, e);
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void lvwMeaning_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                pnlDetails.Visible = lvwMeaning.SelectedItems.Count > 0;

                if (lvwMeaning.SelectedItems.Count > 0)
                {
                    Acronym acronymSelected = lbxAcronym.SelectedItem as Acronym;
                    Meaning selectedMeaning = acronymSelected.Meanings.First(x => x.Caption == lvwMeaning.SelectedItems[0].Text);
                    lblNoOfCategories.Text = selectedMeaning.Categories.Count.ToString("n0");
                    lblNoOfResources.Text = selectedMeaning.Resources.Count.ToString("n0");
                    lblNoOfMatches.Text = selectedMeaning.Configurations.Count.ToString("n0");
                    lblNoOfValidations.Text = selectedMeaning.Validations.Count(x => x.Validated).ToString("n0");
                    showDetails();
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void rdoDetails_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    showDetails();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void showDetails()
        {
            Acronym acronymSelected = lbxAcronym.SelectedItem as Acronym;
            if (acronymSelected != null)
            {
                Meaning selectedMeaning = acronymSelected.Meanings.First(x => x.Caption == lvwMeaning.SelectedItems[0].Text);
                if (rdoCategories.Checked)
                {
                    lblDetailsCaption.Text = "Categories:";
                    lbxDetails.DataSource = selectedMeaning.Categories.OrderBy(x => x.Caption).Select(x => x.Caption).ToList();
                }
                else if (rdoResources.Checked)
                {
                    lblDetailsCaption.Text = "Sources:";
                    lbxDetails.DataSource = selectedMeaning.Resources.SelectMany(x => x.Executions.Select(y => y.Description)).Distinct().OrderBy(x => x).ToList();
                }
                else if (rdoMatches.Checked)
                {
                    lblDetailsCaption.Text = "Matchers:";
                    lbxDetails.DataSource = selectedMeaning.Configurations.Select(x => x.Description).OrderBy(x => x).ToList();
                }
                else if (rdoValidations.Checked)
                {
                    lblDetailsCaption.Text = "Validators:";
                    lbxDetails.DataSource = selectedMeaning.Validations.Select(x => x.Arrangement.Description).OrderBy(x => x).ToList();
                }
            }
        }

        private void lvwMeaning_Resize(object sender, EventArgs e)
        {
            lvwMeaning.Columns[0].Width = lvwMeaning.Width - 4;
        }

        private void navigationControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up
                || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                if (e.KeyCode == Keys.Up && lbxAcronym.SelectedIndex > 0)
                {
                    lbxAcronym.SelectedIndex--;
                }
                else if (e.KeyCode == Keys.Down && lbxAcronym.SelectedIndex >= 0 && lbxAcronym.SelectedIndex < lbxAcronym.Items.Count - 1)
                {
                    lbxAcronym.SelectedIndex++;
                }
            }
            else if (e.KeyCode == Keys.Right
                     || e.KeyCode == Keys.Left)
            {
                e.Handled = true;

                if (lvwMeaning.SelectedIndices.Count == 0 && lvwMeaning.Items.Count > 0)
                {
                    lvwMeaning.SelectedIndices.Add(0);
                }
                else if (lvwMeaning.SelectedIndices.Count > 0
                         && lvwMeaning.SelectedIndices[0] > 0
                         && e.KeyCode == Keys.Left)
                {
                    int selectedIndex = lvwMeaning.SelectedIndices[0];
                    lvwMeaning.SelectedIndices.Clear();
                    lvwMeaning.SelectedIndices.Add(selectedIndex - 1);
                }
                else if (lvwMeaning.SelectedIndices.Count > 0
                         && lvwMeaning.SelectedIndices[0] < lvwMeaning.Items.Count - 1
                         && e.KeyCode == Keys.Right)
                {
                    int selectedIndex = lvwMeaning.SelectedIndices[0];
                    lvwMeaning.SelectedIndices.Clear();
                    lvwMeaning.SelectedIndices.Add(selectedIndex + 1);
                }
            }
        }

        #endregion Browsing Tab        

        #region Replacing tab

        #region P/Invoke

        [DllImport("user32.dll")]
        private static extern bool LockWindowUpdate(IntPtr hWndLock);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwndLock, Int32 wMsg, Int32 wParam, ref Point pt);

        #endregion P/Invoke

        private List<FoundAcronym> _acronyms;
        private FoundAcronym _pointedAcronym = null;
        private Point _lastPoint = new Point(int.MinValue, int.MinValue);
        private bool _updatingText;
        private Color _acronymDefaultColor = Color.DarkOrange;
        private Color _acronymUnknownColor = Color.Red;
        private Color _acronymKnownColor = Color.Green;
        private Point _initialPoint;
        private int _initialIndex;

        private void rtxtInput_TextChanged(object sender, EventArgs e)
        {
            if (!_updatingText)
            {   
                highlightAcronyms();
                _lastPoint = new Point(int.MinValue, int.MinValue);
                setCursor(_lastPoint);
            }
        }

        private void rtxtInput_MouseMove(object sender, MouseEventArgs e)
        {
            setCursor(e.Location);            
        }

        private void rtxtInput_Click(object sender, EventArgs e)
        {
            if (_pointedAcronym != null)
            {                
                ttpAcronym.Hide(rtxtInput);
                txtSearchString.Text = _pointedAcronym.Caption.Replace(".", string.Empty);
                tabControlContent.SelectedTab = tabControlContent.TabPages[_tabPageBrowserName];
            }
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            if (_acronyms != null)
            {                
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    _updatingText = true;
                    using (FrmWait frmWait = new FrmWait { Text = "Replacing acronyms..." })
                    {
                        frmWait.AsyncProcess = replaceAcronyms;
                        frmWait.InputParameter = rtxtInput.Text;
                        frmWait.ShowDialog(this);
                        if (frmWait.HasFailed)
                        {
                            MessageBox.Show("Exception occurred: " + frmWait.ThrownException.ToString());
                        }
                        else
                        {
                            rtxtInput.Text = frmWait.OutputParameter as string;
                            highlightAcronyms(_acronymUnknownColor);
                        }
                    }                    
                }
                finally
                {                    
                    _updatingText = false;
                    Cursor.Current = Cursors.Default;
                    rtxtInput.Select();
                }
            }
        }

        private object replaceAcronyms(FrmWait frmWait, object objInputText)
        {
            string ret = null;

            frmWait.SetupProgressBar(_acronyms.Count);
            _acronyms.Reverse();
            string originalText = objInputText as string;
            StringBuilder text = new StringBuilder(objInputText as string);
            int acronymsTreated = 0;
            foreach (FoundAcronym acronym in _acronyms)
            {
                SelectedMeaningBase meaning;
                if (acronym.IsMeaningSet)
                {
                    meaning = acronym.Meaning;
                }
                else
                {
                    meaning = _acroInquisitor.GetAcronymMeaning(acronym, originalText);
                }

                if (meaning != null)
                {
                    text.Remove(acronym.Index, acronym.Caption.Length);
                    text.Insert(acronym.Index, meaning);
                }
                frmWait.UpdateProgressBar(++acronymsTreated);
            }
            ret = text.ToString();

            return ret;
        }

        private void highlightAcronyms()
        {
            highlightAcronyms(_acronymDefaultColor);
        }

        private void highlightAcronyms(Color defaultColor)
        {
            try
            {
                _updatingText = true;
                Cursor.Current = Cursors.WaitCursor;
                LockWindowUpdate(rtxtInput.Handle);
                rtxtInput.SuspendLayout();
                storeInitialPosition();
                rtxtInput.SelectionStart = 0;
                rtxtInput.SelectionLength = rtxtInput.Text.Length;
                rtxtInput.SelectionColor = rtxtInput.ForeColor;
                rtxtInput.SelectionFont = rtxtInput.Font;

                _acronyms = string.IsNullOrEmpty(rtxtInput.Text)
                                ? new List<FoundAcronym>()
                                : _acroInquisitor.MatchAcronyms(rtxtInput.Text).ToList();

                foreach (FoundAcronym acronym in _acronyms)
                {
                    if (defaultColor == _acronymUnknownColor)
                    {
                        acronym.Meaning = null;
                    }

                    rtxtInput.SelectionStart = acronym.Index;
                    rtxtInput.SelectionLength = acronym.Caption.Length;
                    rtxtInput.SelectionColor = defaultColor;
                    rtxtInput.SelectionFont = new Font(rtxtInput.Font, FontStyle.Bold);
                }                
            }
            finally
            {
                restoreInitialPosition();
                rtxtInput.ResumeLayout();
                LockWindowUpdate(IntPtr.Zero);
                Cursor.Current = Cursors.Default;
                _updatingText = false;
            }
        }

        private void setCursor(Point cursorLocation)
        {
            try
            {
                _updatingText = true;
                if (_lastPoint == null || _lastPoint != cursorLocation)
                {
                    bool isOnAcronym = false;

                    int mouseIndex = rtxtInput.GetCharIndexFromPosition(cursorLocation);

                    if (_acronyms != null)
                    {
                        FoundAcronym pointedAcronym = _acronyms.FirstOrDefault(x => x.Index <= mouseIndex && x.Index + x.Caption.Length >= mouseIndex);

                        if (pointedAcronym != null)
                        {
                            Point firstAcronymPosition = rtxtInput.GetPositionFromCharIndex(pointedAcronym.Index);
                            Point lastAcronymPosition = rtxtInput.GetPositionFromCharIndex(pointedAcronym.Index + pointedAcronym.Caption.Length);

                            if (pointedAcronym != null
                                && lastAcronymPosition.X >= cursorLocation.X
                                && firstAcronymPosition.X <= cursorLocation.X
                                && firstAcronymPosition.Y + 2 * txtSearchString.Font.SizeInPoints >= cursorLocation.Y)
                            {
                                List<FoundAcronym> acronymsToPaint = null;
                                if (!pointedAcronym.IsMeaningSet)
                                {
                                    pointedAcronym.Meaning = getAcronymMeaning(pointedAcronym);

                                    acronymsToPaint = new List<FoundAcronym>();
                                    acronymsToPaint.Add(pointedAcronym);
                                    acronymsToPaint.AddRange(_acronyms.Where(x => x.Caption.Replace(".", string.Empty) == pointedAcronym.Caption.Replace(".", string.Empty)
                                                                                  && !x.IsMeaningSet
                                                                                  && x.Index != pointedAcronym.Index));
                                }

                                Point tooltipLocation = new Point(cursorLocation.X + 2, cursorLocation.Y + 2);

                                if (pointedAcronym.Meaning != null)
                                {
                                    paint(acronymsToPaint, _acronymKnownColor);                                    
                                    ttpAcronym.Show(string.Format("{0}: {1}{2}", 
                                                                  pointedAcronym.Caption.Replace(".", string.Empty), 
                                                                  pointedAcronym.Meaning,
                                                                  chkShowExplanation.Checked ? Environment.NewLine + pointedAcronym.Meaning.Explanation 
                                                                                             : string.Empty),
                                                    rtxtInput, tooltipLocation);
                                    _pointedAcronym = pointedAcronym;
                                    rtxtInput.Cursor = Cursors.Hand;
                                }
                                else
                                {
                                    paint(acronymsToPaint, _acronymUnknownColor);
                                    ttpAcronym.Show("Acronym not found", rtxtInput, tooltipLocation);
                                    _pointedAcronym = null;
                                    rtxtInput.Cursor = Cursors.Default;
                                }
                                isOnAcronym = true;
                            }
                        }
                    }

                    if (!isOnAcronym)
                    {
                        ttpAcronym.Hide(rtxtInput);
                        _pointedAcronym = null;
                        rtxtInput.Cursor = Cursors.Default;
                    }

                    _lastPoint = cursorLocation;
                }
            }
            finally
            {
                _updatingText = false;
            }
        }

        private SelectedMeaningBase getAcronymMeaning(FoundAcronym acronym)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                return _acroInquisitor.GetAcronymMeaning(acronym, rtxtInput.Text);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void paint(List<FoundAcronym> acronyms, Color foreColor)
        {
            if (acronyms != null)
            {
                btnReplaceAll.Focus();
                rtxtInput.SuspendLayout();
                LockWindowUpdate(rtxtInput.Handle);                
                foreach (FoundAcronym acronym in acronyms)
                {
                    rtxtInput.SelectionStart = acronym.Index;
                    rtxtInput.SelectionLength = acronym.Caption.Length;
                    rtxtInput.SelectionColor = foreColor;
                    if (foreColor == _acronymUnknownColor)
                    {
                        acronym.Meaning = null;
                    }
                }
                LockWindowUpdate(IntPtr.Zero);
                rtxtInput.ResumeLayout();
                rtxtInput.SelectionStart = acronyms.First().Index;
                rtxtInput.Focus();                
                rtxtInput.SelectionLength = 0;
            }
        }

        private void storeInitialPosition()
        {
            _initialIndex = rtxtInput.SelectionStart;
        }

        private void restoreInitialPosition()
        {
            rtxtInput.SelectionStart = _initialIndex;
            rtxtInput.SelectionLength = 0;
        }        

        #endregion Replacing tab        
    }
}
