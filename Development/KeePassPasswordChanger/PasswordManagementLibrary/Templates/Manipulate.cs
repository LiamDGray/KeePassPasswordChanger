using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CefBrowserControl;

namespace KeePassPasswordChanger.Templates
{
    public partial class Manipulate : Form
    {
        public Template Template;

        public Manipulate()
        {
            InitializeComponent();
            textBoxPasswordLength.KeyPress += TextBoxIntOnKeyPress;
            textBoxTemplateVersion.KeyPress += TextBoxIntOnKeyPress;
        }

        private void TextBoxIntOnKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
            if (keyPressEventArgs.KeyChar == '\b')
                return;
            int isNumber = 0;
            keyPressEventArgs.Handled = !int.TryParse(keyPressEventArgs.KeyChar.ToString(), out isNumber);
        }

        public Manipulate(Template template) : this()
        {
            this.Text = "Template: " + template.Name + " (UTID: " + template.UTID + ")";
            this.Template = template;
            RefreshUi();
        }

        public static string GenerateTemplateElementString(TemplateElement templateElement, bool includeUeid = true)
        {
            if (templateElement == null)
                return "";
            string text = "";
            if (templateElement.BrowserActionOrCommand is BrowserCommand)
            {
                text = templateElement.BrowserActionOrCommand.GetType().Name;
            }
            else
            {
                text = templateElement.BrowserActionOrCommand.GetType().Name + ": " +
                       ((BrowserAction) templateElement.BrowserActionOrCommand).ActionObject.GetType().Name;
            }
            return templateElement.Name + " | " + text + (includeUeid
                       ? (" (UEID: " +
                          templateElement.UEID + ")")
                       : "");
        }

        private void RefreshUi()
        {
            checkBoxAutomaticallyGenerateAndSavePassword.Checked = Template.SetNewPasswordWhenSuccess;
            textBoxUTID.Text = Template.UTID;
            textBoxName.Text = Template.Name;
            textBoxTemplateVersion.Text = Template.TemplateVersion.ToString();
            textBoxSelectorString.Text = Template.BoundUrl.Value.Value;
            checkBoxSelectorIsRegex.Checked = Template.BoundUrl.IsRegex.Value;
            if (Template.PasswordCreationPolicy.LowerCase)
                checkBoxLowerCase.Checked = true;
            if (Template.PasswordCreationPolicy.UpperCase)
                checkBoxUpperCase.Checked = true;
            if (Template.PasswordCreationPolicy.Digits)
                checkBoxDigits.Checked = true;
            if (Template.PasswordCreationPolicy.SpecialAscii)
                checkBoxAscii.Checked = true;
            if (Template.PasswordCreationPolicy.Punctuation)
                checkBoxPunctuation.Checked = true;
            if (Template.PasswordCreationPolicy.Brackets)
                checkBoxBrackets.Checked = true;
            if (Template.PasswordCreationPolicy.NoRepeatingCharacters)
                checkBoxNoRepeating.Checked = true;
            if (Template.PasswordCreationPolicy.ExcludeLookAlike)
                checkBoxExcludeLookAlikes.Checked = true;
            textBoxPasswordLength.Text = Template.PasswordCreationPolicy.Length.ToString();

            listBoxTemplateElements.Items.Clear();
            foreach (var templateElement in Template.TemplateElements)
            {
                listBoxTemplateElements.Items.Add(GenerateTemplateElementString(templateElement, false));
            }
        }

        private void ReArrangeTemplateelements(bool up = true)
        {
            int selectedIndex = listBoxTemplateElements.SelectedIndex;
            if (selectedIndex >= 0)
            {
                if (up && selectedIndex == 0)
                    return;
                if (!up && selectedIndex == listBoxTemplateElements.Items.Count - 1)
                    return;
                List<TemplateElement> newTemplateElements = new List<TemplateElement>();
                for (int i = 0; i < Template.TemplateElements.Count; i++)
                {
                    if (up)
                    {
                        if (i == selectedIndex - 1)
                        {
                            newTemplateElements.Add(Template.TemplateElements[i + 1]);
                            newTemplateElements.Add(Template.TemplateElements[i]);
                            i++;
                            continue;
                        }
                    }
                    else
                    {
                        if (i == selectedIndex)
                        {
                            newTemplateElements.Add(Template.TemplateElements[i + 1]);
                            newTemplateElements.Add(Template.TemplateElements[i]);
                            i++;
                            continue;
                        }
                    }
                    newTemplateElements.Add(Template.TemplateElements[i]);
                }
                Template.TemplateElements = newTemplateElements;
            }
            RefreshUi();
            listBoxTemplateElements.SelectedIndex = up ? --selectedIndex : ++selectedIndex;
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
           ReArrangeTemplateelements();
        }

        private void buttonAddTemplateElement_Click(object sender, EventArgs e)
        {
            NewTemplateElement newTemplateElement = new NewTemplateElement();
            newTemplateElement.ShowDialog();
            if (newTemplateElement.TemplateElement != null)
            {
                Template.TemplateElements.Add(newTemplateElement.TemplateElement);

                RefreshUi();

                ManipulateTemplateElement manipulateTemplateElement = new ManipulateTemplateElement(newTemplateElement.TemplateElement, Template);
                manipulateTemplateElement.ShowDialog();

                RefreshUi();
            }
        }

        private void buttonRemoveTemplateElement_Click(object sender, EventArgs e)
        {
            TemplateElement templateElement = GetCurrentSelectedTemplateElement();
            if (templateElement != null)
            {
                Template.TemplateElements.Remove(templateElement);
                RefreshUi();
            }
            if (listBoxTemplateElements.Items.Count > 0)
                listBoxTemplateElements.SelectedIndex = 0;
        }

        private TemplateElement GetCurrentSelectedTemplateElement()
        {
            if (listBoxTemplateElements.SelectedIndex == -1)
                return null;
            TemplateElement templateElement = null;
            foreach (var cTemplateElement in Template.TemplateElements)
            {
                for (int i = 0; i < listBoxTemplateElements.Items.Count; i++)
                {
                    if (listBoxTemplateElements.GetSelected(i) &&
                        listBoxTemplateElements.Items[i].ToString() == GenerateTemplateElementString(cTemplateElement, false))
                    {
                        templateElement = cTemplateElement;
                    }
                }
            }
            return templateElement;
        }

        private void buttonCloneTemplateElement_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBoxTemplateElements.SelectedIndex;
            if (selectedIndex >= 0)
            {
                TemplateElement templateElement = GetCurrentSelectedTemplateElement();
                if (templateElement == null)
                    return;
                TemplateElement newTemplateElement = (TemplateElement) templateElement.Clone();
                newTemplateElement.Name = "Clone " + newTemplateElement.Name + " " + DateTime.Now.ToString() + Guid.NewGuid();
                newTemplateElement.GenerateNewUeid();
                Template.TemplateElements.Insert(selectedIndex+1, newTemplateElement);

                RefreshUi();
                listBoxTemplateElements.SelectedIndex = selectedIndex;
            }
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            ReArrangeTemplateelements(false);
        }

        private void buttonEditTemplateElement_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBoxTemplateElements.SelectedIndex;
            TemplateElement templateElement = GetCurrentSelectedTemplateElement();
            if (templateElement == null)
                return;
            RefreshUi();

            ManipulateTemplateElement manipulateTemplateElement = new ManipulateTemplateElement(templateElement, Template);
            manipulateTemplateElement.ShowDialog();

            RefreshUi();
            listBoxTemplateElements.SelectedIndex = selectedIndex;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Manipulate_Load(object sender, EventArgs e)
        {

        }

        private void textBoxUTID_TextChanged(object sender, EventArgs e)
        {
            Template.UTID = textBoxUTID.Text;
        }

        private void checkBoxAutomaticallyGenerateAndSavePassword_CheckedChanged(object sender, EventArgs e)
        {
            Template.SetNewPasswordWhenSuccess = checkBoxAutomaticallyGenerateAndSavePassword.Checked;
        }

        private void textBoxSelectorString_TextChanged(object sender, EventArgs e)
        {
            Template.BoundUrl.Value.Value = textBoxSelectorString.Text;
        }

        private void checkBoxSelectorIsRegex_CheckedChanged(object sender, EventArgs e)
        {
            Template.BoundUrl.IsRegex.Value = checkBoxSelectorIsRegex.Checked;
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            Template.Name = textBoxName.Text;
        }

        private void textBoxPasswordLength_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Template.PasswordCreationPolicy.Length = Convert.ToInt32(textBoxPasswordLength.Text);
            }
            catch
            {
                
            }
        }

        private void checkBoxLowerCase_CheckedChanged(object sender, EventArgs e)
        {
            Template.PasswordCreationPolicy.LowerCase = checkBoxLowerCase.Checked;
        }

        private void checkBoxExcludeLookAlikes_CheckedChanged(object sender, EventArgs e)
        {
            Template.PasswordCreationPolicy.ExcludeLookAlike = checkBoxExcludeLookAlikes.Checked;
        }

        private void checkBoxDigits_CheckedChanged(object sender, EventArgs e)
        {
            Template.PasswordCreationPolicy.Digits = checkBoxDigits.Checked;
        }

        private void checkBoxAscii_CheckedChanged(object sender, EventArgs e)
        {
            Template.PasswordCreationPolicy.SpecialAscii = checkBoxAscii.Checked;
        }

        private void checkBoxPunctuation_CheckedChanged(object sender, EventArgs e)
        {
            Template.PasswordCreationPolicy.Punctuation = checkBoxPunctuation.Checked;
        }

        private void checkBoxBrackets_CheckedChanged(object sender, EventArgs e)
        {
            Template.PasswordCreationPolicy.Brackets = checkBoxBrackets.Checked;
        }

        private void checkBoxNoRepeating_CheckedChanged(object sender, EventArgs e)
        {
            Template.PasswordCreationPolicy.NoRepeatingCharacters = checkBoxNoRepeating.Checked;
        }

        private void checkBoxUpperCase_CheckedChanged(object sender, EventArgs e)
        {
            Template.PasswordCreationPolicy.UpperCase = checkBoxUpperCase.Checked;
        }

        private void Manipulate_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void textBoxTemplateVersion_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Template.TemplateVersion = Convert.ToInt32(textBoxTemplateVersion.Text);
            }
            catch
            {

            }
        }
    }
}
