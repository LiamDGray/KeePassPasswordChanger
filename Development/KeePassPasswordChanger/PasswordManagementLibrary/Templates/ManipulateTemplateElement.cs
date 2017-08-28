using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CefBrowserControl;
using CefBrowserControl.BrowserActions.Elements;
using CefBrowserControl.BrowserActions.Helper;
using CefBrowserControl.Resources;
using KeePassLib;

namespace KeePassPasswordChanger.Templates
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public partial class ManipulateTemplateElement : Form
    {
        private readonly List<KeyValuePair<object, object>> _parameterToInputList = new List<KeyValuePair<object, object>>();
        private const int VerticalSpace = 6;
        private int _currentY;
        private int _currentTabIndex;

        public TemplateElement TemplateElement;
        public Template Template;

        public ManipulateTemplateElement()
        {
            InitializeComponent();
            _currentY = groupBoxName.Location.Y + groupBoxName.Height + VerticalSpace;
        }

        private BaseObject GetBaseObject()
        {
            if (TemplateElement == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("TemplateElement should be initialized!"));
            }
            Debug.Assert(TemplateElement != null, "TemplateElement != null");
            BaseObject baseObject = TemplateElement.BrowserActionOrCommand is BrowserCommand ?
                 (BaseObject)TemplateElement.BrowserActionOrCommand :
                 (BaseObject)((BrowserAction)TemplateElement.BrowserActionOrCommand).ActionObject;
            return baseObject;
        }

        private List<KeyValuePairEx<string, object>> GetParameters()
        {
            return GetBaseObject().InputParameterAvailable;
        }

        private List<string> GetOutputKeyList()
        {
            return GetBaseObject().ReturnedOutputKeysList;
        }

        public ManipulateTemplateElement(TemplateElement templateElement, Template template) : this()
        {
            Text = Text + " " + Manipulate.GenerateTemplateElementString(templateElement, true);
            TemplateElement = templateElement;
            Template = template;

            textBoxName.Text = TemplateElement.Name;
            textBoxName.TextChanged+= TextBoxNameOnTextChanged;

            textBoxUEID.Text = TemplateElement.UEID;
            textBoxUEID.TextChanged += TextBoxUeidOnTextChanged;

            if (((BaseObject)TemplateElement.BrowserActionOrCommand) is BrowserCommand)
            {
                if (((BaseObject) TemplateElement.BrowserActionOrCommand).TimeoutInSec != null)
                    textBoxTimeOut.Text = ((BaseObject) TemplateElement.BrowserActionOrCommand).TimeoutInSec.Value.ToString();
                else
                    textBoxTimeOut.Text = "";
            }
            else if (((BaseObject)TemplateElement.BrowserActionOrCommand) is BrowserAction)
            {
                if (((BaseObject)((BrowserAction)((BaseObject)TemplateElement.BrowserActionOrCommand)).ActionObject).TimeoutInSec != null)
                    textBoxTimeOut.Text = ((BaseObject)((BrowserAction)((BaseObject)TemplateElement.BrowserActionOrCommand)).ActionObject).TimeoutInSec.Value.ToString();
                else
                    textBoxTimeOut.Text = "";
            }

            List<KeyValuePairEx<string, object>> parameters = GetParameters();
            foreach (var identifierToObjectKeyValuePair in parameters)
            {
                if (identifierToObjectKeyValuePair.Value is InsecureText)
                {
                    GroupBoxCreateString(identifierToObjectKeyValuePair.Key, identifierToObjectKeyValuePair.Value);
                }
                else if (identifierToObjectKeyValuePair.Value is Selector)
                {
                    GroupBoxCreateSelector(identifierToObjectKeyValuePair.Key, identifierToObjectKeyValuePair.Value);
                }
                else if (identifierToObjectKeyValuePair.Value is StringOrRegex)
                {
                    GroupBoxCreateStringOrRegex(identifierToObjectKeyValuePair.Key, identifierToObjectKeyValuePair.Value);
                }
                else if (identifierToObjectKeyValuePair.Value is InsecureInt)
                {
                    GroupBoxCreateInteger(identifierToObjectKeyValuePair.Key, identifierToObjectKeyValuePair.Value);
                }
                else if (identifierToObjectKeyValuePair.Value is InsecureHttpAuthSchemaType)
                {
                    GroupBoxCreateSchemaType(identifierToObjectKeyValuePair.Key, identifierToObjectKeyValuePair.Value);
                }
                else if (identifierToObjectKeyValuePair.Value is InsecureDialogType)
                {
                    GroupBoxCreateJsPromptType(identifierToObjectKeyValuePair.Key, identifierToObjectKeyValuePair.Value);
                }
                else if (identifierToObjectKeyValuePair.Value is InsecureBool)
                {
                    GroupBoxCreateBool(identifierToObjectKeyValuePair.Key, identifierToObjectKeyValuePair.Value);
                }
                else if (identifierToObjectKeyValuePair.Value is List<object>)
                {
                    GroupBoxCreateInsecureDisplayObjects(identifierToObjectKeyValuePair.Key);
                }
            }

            GroupBox groupBoxConditions = GroupboxCreate("Conditions", "", 223);
            Controls.Add(groupBoxConditions);
            TabControl tabControlTemplateElementConditions = new TabControl();
            tabControlTemplateElementConditions.Location = new System.Drawing.Point(6, 19);
            //tabControlConditions.Name = "tabControlSuccessConditionsDemo";
            tabControlTemplateElementConditions.SelectedIndex = 0;
            tabControlTemplateElementConditions.Size = new System.Drawing.Size(644, 173);
            tabControlTemplateElementConditions.TabIndex = _currentTabIndex++;

            groupBoxConditions.Controls.Add(tabControlTemplateElementConditions);

            GroupBoxAddTemplateElementCondition(true, tabControlTemplateElementConditions, TemplateElement);
            foreach (var conditionsToTemplateElement in TemplateElement.ConditionBasedAppendedTemplateElements)
            {
                GroupBoxAddTemplateElementCondition(false, tabControlTemplateElementConditions, conditionsToTemplateElement.Value);
            }

            Button buttonAddConditionBasedTemplateElement = new Button();
            buttonAddConditionBasedTemplateElement.Location = new System.Drawing.Point(6, 194);
            //buttonAddConditionBasedTemplateElement.Name = "buttonAddConditionBasedTemplateElementDemo";
            buttonAddConditionBasedTemplateElement.Size = new System.Drawing.Size(644, 23);
            buttonAddConditionBasedTemplateElement.TabIndex = _currentTabIndex++;
            buttonAddConditionBasedTemplateElement.Text = "Add Condition-Based Template-Element";
            buttonAddConditionBasedTemplateElement.UseVisualStyleBackColor = true;

            buttonAddConditionBasedTemplateElement.Click += ButtonAddConditionBasedTemplateElementOnClick;
            groupBoxConditions.Controls.Add(buttonAddConditionBasedTemplateElement);


            buttonNewPassword.Click +=
                delegate(object sender, EventArgs args)
                {
                    Clipboard.SetText(BaseObject.ConvertStringToPlaceholderString(""));
                };
            buttonCompleted.Click += delegate(object sender, EventArgs args)
            {
                    Clipboard.SetText(BaseObject.ConvertStringToPlaceholderString("completed"));
            };
            buttonSuccessfull.Click += delegate(object sender, EventArgs args)
            {
                    Clipboard.SetText(BaseObject.ConvertStringToPlaceholderString("successfull"));

            };

            foreach (var standardField in PwDefs.GetStandardFields())
            {
                listBoxPwDefs.Items.Add(standardField);
            }
            listBoxPwDefs.SelectedValueChanged += delegate(object sender, EventArgs args)
            {
                ListBox listBox = (ListBox)sender;
                foreach (var standardField in PwDefs.GetStandardFields())
                {
                    if (listBox.Text == standardField)
                    {
                        Clipboard.SetText(BaseObject.ConvertStringToPlaceholderString(standardField));
                    }
                }
            };

            foreach (var outputKey in GetOutputKeyList())
            {
                listBoxOutputTemplateElements.Items.Add(outputKey);
            }
            listBoxOutputTemplateElements.SelectedValueChanged += delegate(object sender, EventArgs args)
            {
                ListBox listBox = (ListBox)sender;
                foreach (var outputKey in GetOutputKeyList())
                {
                    if (listBox.Text == outputKey)
                    {
                        Clipboard.SetText(Template.EncodeTemplateElementIdWithOutputName(TemplateElement.UEID, outputKey));
                    }
                }
            };
        }

        private void TextBoxUeidOnTextChanged(object sender, EventArgs eventArgs)
        {
            TemplateElement.UEID = ((TextBox) sender).Text;
        }

        private void TextBoxNameOnTextChanged(object sender, EventArgs eventArgs)
        {
            TemplateElement.Name = ((TextBox) sender).Text;
            Text = "Template Element: " + " " + Manipulate.GenerateTemplateElementString(TemplateElement, true);
        }

        private void ButtonAddConditionBasedTemplateElementOnClick(object sender, EventArgs eventArgs)
        {
            Button button = (Button) sender;
            GroupBox groupBoxConditionBasedTemplateelements = (GroupBox) button.Parent;
            TabControl tabControlConditionBasedTemplateElements = null;
            foreach (Control control in groupBoxConditionBasedTemplateelements.Controls)
            {
                if (control is TabControl)
                    tabControlConditionBasedTemplateElements = (TabControl)control;
            }
            if (tabControlConditionBasedTemplateElements == null)
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("Tab Control Conditionbased Template Elements should be set!"));
            NewTemplateElement newTemplateElement = new NewTemplateElement();
            newTemplateElement.ShowDialog();
            if (newTemplateElement.TemplateElement != null)
            {
                TemplateElement.ConditionBasedAppendedTemplateElements.Add(
                    new KeyValuePairEx<List<Condition>, TemplateElement>(new List<Condition>() {new Condition()},
                        newTemplateElement.TemplateElement
                    ));
                GroupBoxAddTemplateElementCondition(false, tabControlConditionBasedTemplateElements,
                    newTemplateElement.TemplateElement);
            }
        }

        private void GroupBoxAddTemplateElementCondition(bool succesCondition, TabControl tabControlTemplateElementConditions, TemplateElement templateElement)
        {
            TabPage tabPageConditions = new TabPage(succesCondition ? "Success Conditions" : "Condition-Based Template-Element " + (tabControlTemplateElementConditions.TabPages.Count));
            tabPageConditions.Location = new System.Drawing.Point(4, 22);
            //tabPageSuccessConditions.Name = "tabPage4";
            tabPageConditions.Padding = new System.Windows.Forms.Padding(3);
            tabPageConditions.Size = new System.Drawing.Size(636, 147);
            tabPageConditions.TabIndex = _currentTabIndex++;
            tabPageConditions.Text = succesCondition ? "Success Conditions" : "Condition-Based Template-Element " + (tabControlTemplateElementConditions.TabPages.Count);
            tabPageConditions.UseVisualStyleBackColor = true;

            tabControlTemplateElementConditions.TabPages.Add(tabPageConditions);

            TabControl tabControlConditions = new TabControl();
            tabControlConditions.Location = new System.Drawing.Point(3, 6);
            //tabControlConditions.Name = "tabControlCondition";
            tabControlConditions.SelectedIndex = 0;
            tabControlConditions.Size = new System.Drawing.Size(623, 85);
            tabControlConditions.TabIndex = _currentTabIndex++;

            tabPageConditions.Controls.Add(tabControlConditions);

            Button buttonAddCondition = new Button();
            buttonAddCondition.Location = new System.Drawing.Point(3, 93);
            //buttonAddCondition.Name = "buttonAddConditionDemo";
            buttonAddCondition.Size = new System.Drawing.Size(143, 23);
            buttonAddCondition.TabIndex = _currentTabIndex++;
            buttonAddCondition.Text = "Add Condition";
            buttonAddCondition.UseVisualStyleBackColor = true;

            buttonAddCondition.Click += ButtonAddCondition_Click;
            tabPageConditions.Controls.Add(buttonAddCondition);

            Label labelAppendedTemplateElementDescriber = new Label();
            labelAppendedTemplateElementDescriber.AutoSize = true;
            labelAppendedTemplateElementDescriber.Location = new System.Drawing.Point(4, 119);
            //labelAppendedTemplateElement.Name = "labelAppendedTemplateElementDemo";
            labelAppendedTemplateElementDescriber.Size = new System.Drawing.Size(147, 13);
            labelAppendedTemplateElementDescriber.TabIndex = 999999999;
            labelAppendedTemplateElementDescriber.Text = "Appended Template Element:";

            tabPageConditions.Controls.Add(labelAppendedTemplateElementDescriber);

            Label labelAppendedTemplateElement = new Label();
            labelAppendedTemplateElement.AutoSize = true;
            labelAppendedTemplateElement.Location = new System.Drawing.Point(157, 119);
            labelAppendedTemplateElement.Name = "labelAppendedTemplateelement" + _currentTabIndex;
            labelAppendedTemplateElement.Size = new System.Drawing.Size(33, 13);
            labelAppendedTemplateElement.TabIndex = 999999999;
            labelAppendedTemplateElement.Text = GetAppendedTemplateElementStringForLabel(succesCondition ? templateElement.AppendedTemplateElement : templateElement);

            tabPageConditions.Controls.Add(labelAppendedTemplateElement);

            Label labelAppendedTemplateElementUeid = new Label();
            labelAppendedTemplateElementUeid.Visible = false;
            labelAppendedTemplateElementUeid.Text =  succesCondition ? (templateElement.AppendedTemplateElement == null ? "" : templateElement.AppendedTemplateElement.UEID) : (templateElement == null ? "" : templateElement.UEID);
            
            tabPageConditions.Controls.Add(labelAppendedTemplateElementUeid);

            if (succesCondition)
            {
                foreach (var successCondition in templateElement.SuccessConditions)
                {
                    AddTabPageCondition(tabControlConditions, successCondition);
                }
            }
            else
            {
                foreach (var conditionsToTemplateElement in TemplateElement.ConditionBasedAppendedTemplateElements)
                {
                    if (conditionsToTemplateElement.Value.UEID == labelAppendedTemplateElementUeid.Text)
                    {
                        foreach (var condition in conditionsToTemplateElement.Key)
                        {
                            AddTabPageCondition(tabControlConditions, condition);
                        }
                    }
                }
            }

            Button buttonAddOrEditAppendedTemplateElement = new Button();
            buttonAddOrEditAppendedTemplateElement.Location = new System.Drawing.Point(393, 93);
            //buttonAddOrEditAppendedTemplateElement.Name = "buttonAddOrEditAppendedTemplateelement";
            buttonAddOrEditAppendedTemplateElement.Size = new System.Drawing.Size(112, 49);
            buttonAddOrEditAppendedTemplateElement.TabIndex = _currentTabIndex++;
            buttonAddOrEditAppendedTemplateElement.Text = "Add/Edit appended Template Element";
            buttonAddOrEditAppendedTemplateElement.UseVisualStyleBackColor = true;

            buttonAddOrEditAppendedTemplateElement.Click += ButtonAddOrEditAppendedTemplateElementOnClick;
            tabPageConditions.Controls.Add(buttonAddOrEditAppendedTemplateElement);

            Button buttonRemoveAppendedTemplateElement = new Button();
            buttonRemoveAppendedTemplateElement.Location = new System.Drawing.Point(511, 93);
            //buttonRemoveAppendedTemplateElement.Name = "buttonRemoveAppendedTemplateelementDemo";
            buttonRemoveAppendedTemplateElement.Size = new System.Drawing.Size(118, 49);
            buttonRemoveAppendedTemplateElement.TabIndex = _currentTabIndex++;
            buttonRemoveAppendedTemplateElement.Text = "Remove appended Template Element";
            buttonRemoveAppendedTemplateElement.UseVisualStyleBackColor = true;

            buttonRemoveAppendedTemplateElement.Click += ButtonRemoveAppendedTemplateElement_Click;
            tabPageConditions.Controls.Add(buttonRemoveAppendedTemplateElement);

            if (!succesCondition)
            {
                Button buttonRemoveConditionBasedTemplateElement = new Button();
                buttonRemoveConditionBasedTemplateElement.Location = new System.Drawing.Point(152, 93);
                buttonRemoveConditionBasedTemplateElement.Name = "buttonRemoveConditionBasedTemplateElement" + _currentTabIndex;
                buttonRemoveConditionBasedTemplateElement.Size = new System.Drawing.Size(235, 23);
                buttonRemoveConditionBasedTemplateElement.TabIndex = _currentTabIndex++;
                buttonRemoveConditionBasedTemplateElement.Text = "Remove condition-based Template Element";
                buttonRemoveConditionBasedTemplateElement.UseVisualStyleBackColor = true;

                buttonRemoveConditionBasedTemplateElement.Click += ButtonRemoveConditionBasedTemplateElement_Click;
                tabPageConditions.Controls.Add(buttonRemoveConditionBasedTemplateElement);
                tabControlTemplateElementConditions.SelectedIndex = tabControlTemplateElementConditions.TabCount-1;
            }
        }

        private void ButtonRemoveConditionBasedTemplateElement_Click(object sender, EventArgs e)
        {
            Button button = (Button) sender;
            TabPage tabPageConditionBasedTemplateElementCondition = (TabPage) button.Parent;
            TabControl tabControlConditionTemplateElements =
                (TabControl) tabPageConditionBasedTemplateElementCondition.Parent;
            Label labelAppendedTemplateElement = null, labelAppendedTemplateElementUeid = null;
            foreach (Control control in tabPageConditionBasedTemplateElementCondition.Controls)
            {
                if (control is Label && control.Name.Contains("labelAppendedTemplateelement"))
                {
                    labelAppendedTemplateElement = (Label)control;
                }
                else if (control is Label && control.Visible == false)
                {
                    labelAppendedTemplateElementUeid = (Label)control;
                }
            }
            if (labelAppendedTemplateElement == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The appended template element label should be set!"));
            }
            if (labelAppendedTemplateElementUeid == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The appended template element label ued should be set!"));
            }

            if (labelAppendedTemplateElementUeid.Text != "")
            {
                foreach (
                    var conditionsToAppendedTemplateElement in TemplateElement.ConditionBasedAppendedTemplateElements)
                {
                    if (conditionsToAppendedTemplateElement.Value.UEID == labelAppendedTemplateElementUeid.Text)
                    {
                        TemplateElement.ConditionBasedAppendedTemplateElements.Remove(
                            conditionsToAppendedTemplateElement);
                        break;
                    }
                }
            }
            tabControlConditionTemplateElements.TabPages.Remove(
                            tabPageConditionBasedTemplateElementCondition);
            tabControlConditionTemplateElements.SelectedIndex =
                tabControlConditionTemplateElements.TabPages.Count - 1;
            tabControlConditionTemplateElements.TabPages.Remove(tabPageConditionBasedTemplateElementCondition);

        }

        private void ButtonRemoveAppendedTemplateElement_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            TabPage tabPageTemplateElementsCondition = (TabPage)button.Parent;
            Label labelAppendedTemplateElement = null, labelAppendedTemplateElementUeid = null;
            bool foundRemoveConditionButton = false;
            foreach (Control control in tabPageTemplateElementsCondition.Controls)
            {
                if (control is Label && control.Name.Contains("labelAppendedTemplateelement"))
                {
                    labelAppendedTemplateElement = (Label)control;
                }
                else if (control is Label && control.Visible == false)
                {
                    labelAppendedTemplateElementUeid = (Label)control;
                }
            }
            if (labelAppendedTemplateElement == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The appended template element label should be set!"));
            }
            if (labelAppendedTemplateElementUeid == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The appended template element label ued should be set!"));
            }
            foreach (Control control in tabPageTemplateElementsCondition.Controls)
            {
                if (control.Name.Contains("buttonRemoveConditionBasedTemplateElement"))
                {
                    foundRemoveConditionButton = true;
                    break;
                }
            }
            if (!foundRemoveConditionButton)
            {
                //if (TemplateElement.AppendedTemplateElement.UEID == labelAppendedTemplateElementUeid.Text)
                    TemplateElement.AppendedTemplateElement = null;
                labelAppendedTemplateElement.Text =
                        GetAppendedTemplateElementStringForLabel(TemplateElement.AppendedTemplateElement);
                labelAppendedTemplateElementUeid.Text = "";
            }
            else
            {
                foreach (var conditionsToTemplateElement in TemplateElement.ConditionBasedAppendedTemplateElements)
                {
                    if (conditionsToTemplateElement.Value.UEID == labelAppendedTemplateElementUeid.Text)
                    {
                        NewTemplateElement newTemplateElement = new NewTemplateElement();
                        newTemplateElement.ShowDialog();
                        if (newTemplateElement.TemplateElement != null)
                        {
                            KeyValuePairEx<List<Condition>, TemplateElement> newEntry =
                                new KeyValuePairEx<List<Condition>, TemplateElement>(conditionsToTemplateElement.Key,
                                    newTemplateElement.TemplateElement);
                            TemplateElement.ConditionBasedAppendedTemplateElements.Remove(conditionsToTemplateElement);
                            TemplateElement.ConditionBasedAppendedTemplateElements.Add(newEntry);
                            labelAppendedTemplateElement.Text =
                            GetAppendedTemplateElementStringForLabel(newTemplateElement.TemplateElement);
                            labelAppendedTemplateElementUeid.Text = newTemplateElement.TemplateElement.UEID;
                        }
                        break;
                    }
                }
            }
            
        }

        private void ButtonAddOrEditAppendedTemplateElementOnClick(object sender, EventArgs eventArgs)
        {
            Button button = (Button) sender;
            TabPage tabPageTemplateElementsCondition = (TabPage) button.Parent;
            Label labelAppendedTemplateElement = null, labelAppendedTemplateElementUeid = null;
            bool foundRemoveConditionButton = false;
            foreach (Control control in tabPageTemplateElementsCondition.Controls)
            {
                if (control is Label && control.Name.Contains("labelAppendedTemplateelement"))
                {
                    labelAppendedTemplateElement = (Label)control;
                }
                else if (control is Label && control.Visible == false)
                {
                    labelAppendedTemplateElementUeid = (Label)control;
                }
            }
            if (labelAppendedTemplateElement == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The appended template element label should be set!"));
            }
            if (labelAppendedTemplateElementUeid == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The appended template element label ued should be set!"));
            }
            foreach (Control control in tabPageTemplateElementsCondition.Controls)
            {
                if (control.Name.Contains("buttonRemoveConditionBasedTemplateElement"))
                {
                    foundRemoveConditionButton = true;
                    break;
                }
            }
            if (!foundRemoveConditionButton)
            {
                if (TemplateElement.AppendedTemplateElement == null)
                {
                    NewTemplateElement newTemplateElement = new NewTemplateElement();
                    newTemplateElement.ShowDialog();
                    if (newTemplateElement.TemplateElement != null)
                    {
                        TemplateElement.AppendedTemplateElement = newTemplateElement.TemplateElement;
                        labelAppendedTemplateElement.Text =
                            GetAppendedTemplateElementStringForLabel(TemplateElement.AppendedTemplateElement);
                        labelAppendedTemplateElementUeid.Text = TemplateElement.AppendedTemplateElement.UEID;

                        ManipulateTemplateElement manipulateTemplateElement =
                            new ManipulateTemplateElement(newTemplateElement.TemplateElement, Template);
                        manipulateTemplateElement.ShowDialog();

                        labelAppendedTemplateElement.Text =
                            GetAppendedTemplateElementStringForLabel(TemplateElement.AppendedTemplateElement);
                        labelAppendedTemplateElementUeid.Text = TemplateElement.AppendedTemplateElement.UEID;
                    }
                }
                else
                {
                    ManipulateTemplateElement manipulateTemplateElement =
                        new ManipulateTemplateElement(TemplateElement.AppendedTemplateElement, Template);
                    manipulateTemplateElement.ShowDialog();

                    labelAppendedTemplateElement.Text =
                        GetAppendedTemplateElementStringForLabel(TemplateElement.AppendedTemplateElement);
                    labelAppendedTemplateElementUeid.Text = TemplateElement.AppendedTemplateElement.UEID;

                }
            }
            else
            {
                if (labelAppendedTemplateElementUeid.Text != "")
                {
                    List<Condition> conditionsList;
                    //TODO: create new entry when keyvaluepair is protected
                    foreach (var conditionsToTemplateElement in TemplateElement.ConditionBasedAppendedTemplateElements)
                    {
                        if (conditionsToTemplateElement.Value.UEID == labelAppendedTemplateElementUeid.Text)
                        {
                            ManipulateTemplateElement manipulateTemplateElement =
                        new ManipulateTemplateElement(conditionsToTemplateElement.Value, Template);
                            manipulateTemplateElement.ShowDialog();

                            labelAppendedTemplateElement.Text =
                                GetAppendedTemplateElementStringForLabel(conditionsToTemplateElement.Value);
                            labelAppendedTemplateElementUeid.Text = conditionsToTemplateElement.Value.UEID;
                            break;
                        }
                    }
                }
                else
                {
                    NewTemplateElement newTemplateElement = new NewTemplateElement();
                    newTemplateElement.ShowDialog();
                    if (newTemplateElement.TemplateElement != null)
                    {
                        TemplateElement.ConditionBasedAppendedTemplateElements.Add(
                            new KeyValuePairEx<List<Condition>, TemplateElement>(
                                new List<Condition>() {new Condition()}, newTemplateElement.TemplateElement));
                        labelAppendedTemplateElement.Text =
                            GetAppendedTemplateElementStringForLabel(TemplateElement.AppendedTemplateElement);
                        labelAppendedTemplateElementUeid.Text = newTemplateElement.TemplateElement.UEID;

                        ManipulateTemplateElement manipulateTemplateElement =
                            new ManipulateTemplateElement(newTemplateElement.TemplateElement, Template);
                        manipulateTemplateElement.ShowDialog();

                        labelAppendedTemplateElement.Text =
                            GetAppendedTemplateElementStringForLabel(newTemplateElement.TemplateElement);
                        labelAppendedTemplateElementUeid.Text = newTemplateElement.TemplateElement.UEID;
                    }
                }
            }
        }

        private string GetAppendedTemplateElementStringForLabel(TemplateElement templateElement)
        {
            return templateElement == null ? "None" : Manipulate.GenerateTemplateElementString(templateElement, false);
        }

        private void AddTabPageCondition(TabControl tabControlConditions, Condition condition)
        {
            TabPage tabPageCondition = new TabPage("Condition " + (tabControlConditions.TabPages.Count + 1));
            tabPageCondition.Location = new System.Drawing.Point(4, 22);
            //tabPageCondition.Name = "tabPage6";
            tabPageCondition.Padding = new System.Windows.Forms.Padding(3);
            tabPageCondition.Size = new System.Drawing.Size(615, 59);
            tabPageCondition.TabIndex = _currentTabIndex++;
            tabPageCondition.Text = "Condition " + (tabControlConditions.TabPages.Count + 1);
            tabPageCondition.UseVisualStyleBackColor = true;

            tabControlConditions.TabPages.Add(tabPageCondition);
            tabControlConditions.SelectedIndex = tabControlConditions.TabPages.Count - 1;
            _parameterToInputList.Add(new KeyValuePair<object, object>(condition, tabPageCondition));

            Label labelConditionUcid = new Label();
            labelConditionUcid.Text = condition.UCID;
            labelConditionUcid.Visible = false;
            labelConditionUcid.Name = "labelConditionUcid" + _currentTabIndex;

            tabPageCondition.Controls.Add(labelConditionUcid);

            Label labelConditionOperator = new Label();
            labelConditionOperator.AutoSize = true;
            labelConditionOperator.Location = new System.Drawing.Point(6, 8);
            //labelConditionOperator.Name = "labelConditionalOperatorDemo";
            labelConditionOperator.Size = new System.Drawing.Size(106, 13);
            labelConditionOperator.TabIndex = 999999999;
            labelConditionOperator.Text = "Conditional Operator:";

            tabPageCondition.Controls.Add(labelConditionOperator);

            ComboBox comboBoxConditionOperator = new ComboBox();
            comboBoxConditionOperator.FormattingEnabled = true;
            comboBoxConditionOperator.Location = new System.Drawing.Point(118, 5);
            comboBoxConditionOperator.Name = "comboBoxConditionOperator" + _currentTabIndex;
            comboBoxConditionOperator.Size = new System.Drawing.Size(47, 21);
            comboBoxConditionOperator.TabIndex = _currentTabIndex++;
            comboBoxConditionOperator.TextChanged += TabPageConditionChanged;

            foreach (var allowedOperator in Condition.AllowedOperators)
            {
                comboBoxConditionOperator.Items.Add(allowedOperator);
                if (allowedOperator == condition.UsedOperator)
                    comboBoxConditionOperator.SelectedItem = allowedOperator;
            }
            tabPageCondition.Controls.Add(comboBoxConditionOperator);

            Button buttonRemoveCondition = new Button();
            buttonRemoveCondition.Location = new System.Drawing.Point(6, 30);
            //buttonRemoveCondition.Name = "buttonRemoveSuccessConditionDemo";
            buttonRemoveCondition.Size = new System.Drawing.Size(159, 23);
            buttonRemoveCondition.TabIndex = _currentTabIndex++;
            buttonRemoveCondition.Text = "Remove Condition";
            buttonRemoveCondition.UseVisualStyleBackColor = true;

            buttonRemoveCondition.Click += ButtonRemoveConditionOnClick;
            tabPageCondition.Controls.Add(buttonRemoveCondition);

            Label labelOperand1 = new Label();
            labelOperand1.AutoSize = true;
            labelOperand1.Location = new System.Drawing.Point(169, 9);
            //labelOperand1.Name = "labelOperand1Demo";
            labelOperand1.Size = new System.Drawing.Size(60, 13);
            labelOperand1.TabIndex = 999999999;
            labelOperand1.Text = "Operand 1:";

            tabPageCondition.Controls.Add(labelOperand1);

            Label labelOperand2 = new Label();
            labelOperand2.AutoSize = true;
            labelOperand2.Location = new System.Drawing.Point(169, 35);
            //labelOperand2.Name = "labelOperand2Demo";
            labelOperand2.Size = new System.Drawing.Size(60, 13);
            labelOperand2.TabIndex = 999999999;
            labelOperand2.Text = "Operand 2:";

            tabPageCondition.Controls.Add(labelOperand2);

            TextBox textBoxOperand1 = new TextBox();
            textBoxOperand1.Location = new System.Drawing.Point(229, 6);
            textBoxOperand1.Name = "textBoxOperand1" + _currentTabIndex;
            textBoxOperand1.Size = new System.Drawing.Size(380, 20);
            textBoxOperand1.TabIndex = _currentTabIndex++;
            textBoxOperand1.TextChanged += TabPageConditionChanged;

            textBoxOperand1.Text = condition.FirstOperand;
            tabPageCondition.Controls.Add(textBoxOperand1);

            TextBox textBoxOperand2 = new TextBox();
            textBoxOperand2.Location = new System.Drawing.Point(229, 32);
            textBoxOperand2.Name = "textBoxOperand2" + _currentTabIndex;
            textBoxOperand2.Size = new System.Drawing.Size(380, 20);
            textBoxOperand2.TabIndex = _currentTabIndex++;
            textBoxOperand2.TextChanged += TabPageConditionChanged;

            textBoxOperand2.Text = condition.SecondOperand;
            tabPageCondition.Controls.Add(textBoxOperand2);

        }

        private void TabPageConditionChanged(object sender, EventArgs eventArgs)
        {
            TabPage tabPage = (TabPage) ((Control) sender).Parent;
            foreach (var parameterToInput in _parameterToInputList)
            {
                if (parameterToInput.Value is TabPage && parameterToInput.Value == tabPage)
                {
                    Condition condition = (Condition)parameterToInput.Key;
                    foreach (Control control in tabPage.Controls)
                    {
                        if (control is TextBox && control.Name.Contains("textBoxOperand1"))
                            condition.FirstOperand = ((TextBox)control).Text;
                        else if (control is TextBox && control.Name.Contains("textBoxOperand2"))
                            condition.SecondOperand = ((TextBox)control).Text;
                        else if (control is ComboBox && control.Name.Contains("comboBoxConditionOperator"))
                        {
                            ComboBox comboBox = (ComboBox)control;
                            foreach (var value in Condition.AllowedOperators)
                            {
                                if (value == comboBox.Text)
                                {
                                    condition.UsedOperator = comboBox.Text;
                                }
                            }
                        }
                    }
                    TemplateElement.ReadAvailableInputParameters();
                }
            }
        }

        private void ButtonAddCondition_Click(object sender, EventArgs e)
        {
            Button button = (Button) sender;
            TabPage tabPageTemplateElementCondition = (TabPage) button.Parent;
            TabControl tabControlConditions = null;
            TabPage tabPageConditionBasedTemplateElement = null;
            Label labelAppendedTemplateElement = null, labelAppendedTemplateElementUeid = null;
            bool foundRemoveConditionButton = false;
            foreach (var control in tabPageTemplateElementCondition.Controls)
            {
                if (control is TabControl)
                {
                    tabControlConditions = (TabControl) control;
                    break;
                }
            }
            if (tabControlConditions == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("TabControlConditions should be set!"));
                return;
            }
            tabPageConditionBasedTemplateElement = (TabPage) tabControlConditions.Parent;
            foreach (Control control in tabPageConditionBasedTemplateElement.Controls)
            {
                if (control is Label && control.Name.Contains("labelAppendedTemplateelement"))
                {
                    labelAppendedTemplateElement = (Label)control;
                }
                else if (control is Label && control.Visible == false)
                {
                    labelAppendedTemplateElementUeid = (Label)control;
                }
            }
            if (labelAppendedTemplateElement == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The appended template element label should be set!"));
            }
            if (labelAppendedTemplateElementUeid == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The appended template element label ued should be set!"));
            }
            foreach (Control control in tabPageConditionBasedTemplateElement.Controls)
            {
                if (control.Name.Contains("buttonRemoveConditionBasedTemplateElement"))
                {
                    foundRemoveConditionButton = true;
                    break;
                }
            }
            Condition condition = new Condition();

            if (!foundRemoveConditionButton)
            {
                TemplateElement.SuccessConditions.Add(condition);
            }
            else
            {
                foreach (var conditionsToTemplateElement in TemplateElement.ConditionBasedAppendedTemplateElements)
                {
                    if(conditionsToTemplateElement.Value.UEID == labelAppendedTemplateElementUeid.Text)
                        conditionsToTemplateElement.Key.Add(condition);
                }
            }
            AddTabPageCondition(tabControlConditions, condition);
        }

        private void ButtonRemoveConditionOnClick(object sender, EventArgs eventArgs)
        {
            Button button = (Button) sender;
            TabPage tabPageCondition = (TabPage) button.Parent;
            TabControl tabControlConditions = (TabControl) tabPageCondition.Parent;
            //--
            TabPage tabPageConditionBasedTemplateElement = (TabPage)tabControlConditions.Parent;
            Label labelAppendedTemplateElement = null, labelAppendedTemplateElementUeid = null, labelConditionUcid = null;
            bool foundRemoveConditionButton = false;

            foreach (Control control in tabPageConditionBasedTemplateElement.Controls)
            {
                if (control is Label && control.Name.Contains("labelAppendedTemplateelement"))
                {
                    labelAppendedTemplateElement = (Label)control;
                }
                else if (control is Label && control.Visible == false)
                {
                    labelAppendedTemplateElementUeid = (Label)control;
                }
            }
            foreach (Control control in tabPageCondition.Controls)
            {
                 if (control is Label && control.Visible == false && control.Name.Contains("labelConditionUcid"))
                {
                    labelConditionUcid = (Label)control;
                }
            }
           
            foreach (Control control in tabPageConditionBasedTemplateElement.Controls)
            {
                if (control.Name.Contains("buttonRemoveConditionBasedTemplateElement"))
                {
                    foundRemoveConditionButton = true;
                    break;
                }
            }

            if (!foundRemoveConditionButton)
            {
                foreach (var condition in TemplateElement.SuccessConditions)
                {
                    if (condition.UCID == labelConditionUcid.Text)
                    {
                        TemplateElement.SuccessConditions.Remove(condition);
                        break;
                    }
                }
            }
            else
            {
                if (labelAppendedTemplateElement == null)
                {
                    ExceptionHandling.Handling.GetException("Unexpected",
                        new Exception("The appended template element label should be set!"));
                }
                if (labelAppendedTemplateElementUeid == null)
                {
                    ExceptionHandling.Handling.GetException("Unexpected",
                        new Exception("The appended template element label ued should be set!"));
                }
                foreach (var conditionsToTemplateElement in TemplateElement.ConditionBasedAppendedTemplateElements)
                {
                    if (conditionsToTemplateElement.Value.UEID == labelAppendedTemplateElementUeid.Text)
                    {
                        foreach (var condition in conditionsToTemplateElement.Key)
                        {
                            if (condition.UCID == labelConditionUcid.Text)
                            {
                                conditionsToTemplateElement.Key.Remove(condition);
                                break;
                            }
                        }
                    }
                }
            }

            tabControlConditions.TabPages.Remove(tabPageCondition);
            tabControlConditions.SelectedIndex = tabControlConditions.TabPages.Count - 1;
        }

        private void ManipulateTemplateElement_Load(object sender, EventArgs e)
        {

        }

        private GroupBox GroupboxCreate(string name, string type, int height)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Location = new Point(12, _currentY);
            //groupBox.Name = "groupBoxBool";
            groupBox.Size = new Size(656, height);
            groupBox.TabIndex = _currentTabIndex++;
            groupBox.TabStop = false;
            groupBox.Text = name + ": " + type;
            _currentY += groupBox.Size.Height + VerticalSpace;
            Controls.Add(groupBox);
            Height = _currentY + 19*2;
            return groupBox;
        }

        private void GroupBoxCreateBool(string name, object boolValue)
        {
            GroupBox groupBox = GroupboxCreate(name, "Bool",  47);

            CheckBox checkbox = new CheckBox();
            checkbox.AutoSize = true;
            checkbox.Location = new Point(6, 19);
            //checkbox.Name = "checkBox1";
            checkbox.Size = new Size(277, 17);
            checkbox.TabIndex = _currentTabIndex++;
            checkbox.Text = name;
            checkbox.UseVisualStyleBackColor = true;
            checkbox.CheckedChanged += GroupBoxBoolChanged;

            checkbox.Checked = ((InsecureBool) boolValue).Value;
            groupBox.Controls.Add(checkbox);
            _parameterToInputList.Add(new KeyValuePair<object, object>(boolValue, checkbox));

        }

        private void GroupBoxBoolChanged(object sender, EventArgs eventArgs)
        {
            CheckBox checkBox = (CheckBox) sender;
            foreach (var parameterToInputObject in _parameterToInputList)
            {
                if (parameterToInputObject.Value is CheckBox)
                {
                    if (((CheckBox) parameterToInputObject.Value) == checkBox)
                    {
                        InsecureBool insecureBool = (InsecureBool) parameterToInputObject.Key;
                        insecureBool.Value = checkBox.Checked;
                        TemplateElement.ReadAvailableInputParameters();
                    }
                }
            }
        }

        private void GroupBoxCreateString(string name, object stringValue)
        {
            GroupBox groupBox = GroupboxCreate(name, "String", 47);

            TextBox textBox = new TextBox();
            textBox.Location = new Point(6, 19);
            //textBox.Name = "textBox4";
            textBox.Size = new Size(644, 20);
            textBox.TabIndex = _currentTabIndex++;
            textBox.TextChanged += GroupBoxStringChanged;

            textBox.Text = ((InsecureText) stringValue).Value;
            groupBox.Controls.Add(textBox);
            _parameterToInputList.Add(new KeyValuePair<object, object>(stringValue, textBox));
        }

        private void GroupBoxStringChanged(object sender, EventArgs eventArgs)
        {
            TextBox textBox = (TextBox) sender;
            foreach (var parameterToInputObject in _parameterToInputList)
            {
                if (parameterToInputObject.Value is TextBox)
                {
                    if (((TextBox)parameterToInputObject.Value) == textBox)
                    {
                        InsecureText insecureText = (InsecureText)parameterToInputObject.Key;
                        insecureText.Value = textBox.Text;
                        TemplateElement.ReadAvailableInputParameters();
                    }
                }
            }
        }

        private void GroupBoxCreateInteger(string name, object integerValue)
        {
            GroupBox groupBox = GroupboxCreate(name, "Integer", 47);

            TextBox textBox = new TextBox();
            textBox.Location = new Point(6, 19);
            //textBox.Name = "textBox4";
            textBox.Size = new Size(644, 20);
            textBox.TabIndex = _currentTabIndex++;
            textBox.KeyPress += TextBoxOnKeyPress;
            textBox.TextChanged += GroupBoxIntegerChanged;

            textBox.Text = ((InsecureInt)integerValue).Value.ToString();
            groupBox.Controls.Add(textBox);

            _parameterToInputList.Add(new KeyValuePair<object, object>(integerValue, textBox));
        }

        private void GroupBoxIntegerChanged(object sender, EventArgs eventArgs)
        {
            TextBox textBox = (TextBox)sender;
            foreach (var parameterToInputObject in _parameterToInputList)
            {
                if (parameterToInputObject.Value is TextBox)
                {
                    if (((TextBox)parameterToInputObject.Value) == textBox)
                    {
                        InsecureInt insecureText = (InsecureInt)parameterToInputObject.Key;
                        if (textBox.Text == "")
                            insecureText.Value = null;
                        else
                            insecureText.Value = Convert.ToInt32(textBox.Text);
                        TemplateElement.ReadAvailableInputParameters();
                    }
                }
            }
        }

        private void GroupBoxCreateStringOrRegex(string name, object stringOrRegexObject)
        {
            GroupBox groupBox = GroupboxCreate(name, "String or Regex", 47);
            _parameterToInputList.Add(new KeyValuePair<object, object>((StringOrRegex)stringOrRegexObject, groupBox));


            CheckBox checkBox = new CheckBox();
            checkBox.AutoSize = true;
            checkBox.Location = new Point(6, 20);
            checkBox.Name = "StringOrRegexBool" + _currentTabIndex;
            checkBox.Size = new Size(67, 17);
            checkBox.TabIndex = _currentTabIndex++;
            checkBox.Text = "is Regex";
            checkBox.UseVisualStyleBackColor = true;
            checkBox.CheckedChanged += GroupBoxStringOrRegexChanged;

            checkBox.Checked = ((StringOrRegex) stringOrRegexObject).IsRegex.Value;
            groupBox.Controls.Add(checkBox);
           
            TextBox textBox = new TextBox();
            textBox.Location = new Point(79, 19);
            textBox.Name = "StringOrRegexTextBox" + _currentTabIndex;
            textBox.Size = new Size(571, 20);
            textBox.TabIndex = _currentTabIndex++;
            textBox.TextChanged += GroupBoxStringOrRegexChanged;

            textBox.Text = ((StringOrRegex) stringOrRegexObject).Value.Value;
            groupBox.Controls.Add(textBox);
        }

        private void GroupBoxStringOrRegexChanged(object sender, EventArgs eventArgs)
        {
            GroupBox groupBox = (GroupBox)((Control)sender).Parent;
            foreach (var parameterToInputObject in _parameterToInputList)
            {
                if (parameterToInputObject.Value is GroupBox)
                {
                    if (((GroupBox)parameterToInputObject.Value) == groupBox)
                    {
                        StringOrRegex stringOrRegex = (StringOrRegex)parameterToInputObject.Key;
                        foreach (Control control in groupBox.Controls)
                        {
                            if (control is CheckBox && control.Name.Contains("StringOrRegexBool"))
                                stringOrRegex.IsRegex.Value = ((CheckBox) control).Checked;
                            else if(control is TextBox && control.Name.Contains("StringOrRegexTextBox"))
                                stringOrRegex.Value.Value = ((TextBox)control).Text;
                        }

                        TemplateElement.ReadAvailableInputParameters();
                    }
                }
            }
        }

        private void GroupBoxCreateSelector(string name, object selector)
        {
            GroupBox groupBox = GroupboxCreate(name, "Selector", 104);
            _parameterToInputList.Add(new KeyValuePair<object, object>(
                    (Selector)selector, groupBox));

            {
                //Select ON
                ComboBox comboBoxSelectOn = new ComboBox();
                comboBoxSelectOn.FormattingEnabled = true;
                comboBoxSelectOn.Location = new Point(69, 18);
                comboBoxSelectOn.Name = "comboBoxSelectOn"+_currentTabIndex;
                comboBoxSelectOn.Size = new Size(581, 21);
                comboBoxSelectOn.TabIndex = _currentTabIndex++;
                comboBoxSelectOn.TextChanged += GroupBoxSelectorChanged;

                foreach (var value in Enum.GetValues(typeof(BrowserAction.ExecuteActionOn)))
                {
                    comboBoxSelectOn.Items.Add(value);
                }
                for (var i = 0; i < comboBoxSelectOn.Items.Count; i++)
                {
                    if (((Selector) selector).SelectorExecuteActionOn.ToString() == comboBoxSelectOn.Items[i].ToString())
                        comboBoxSelectOn.SelectedIndex = i;
                }
                
                groupBox.Controls.Add(comboBoxSelectOn);
                comboBoxSelectOn.Parent = groupBox;

                Label labelSelectOn = new Label();
                labelSelectOn.AutoSize = true;
                labelSelectOn.Location = new Point(6, 22);
                //label3.Name = "label1";
                labelSelectOn.Size = new Size(57, 13);
                labelSelectOn.TabIndex = 999999999;
                labelSelectOn.Text = "Select On:";

                groupBox.Controls.Add(labelSelectOn);
            }

            {
                //Identifier to select
                Label labelIdentifier = new Label();
                labelIdentifier.AutoSize = true;
                labelIdentifier.Location = new Point(6, 49);
                //label2.Name = "label2";
                labelIdentifier.Size = new Size(93, 13);
                labelIdentifier.TabIndex = 999999999;
                labelIdentifier.Text = "Identifier to select:";

                groupBox.Controls.Add(labelIdentifier);

                TextBox textBoxIdentifier = new TextBox();
                textBoxIdentifier.Location = new Point(105, 45);
                textBoxIdentifier.Name = "textBoxIdentifier"+_currentTabIndex;
                textBoxIdentifier.Size = new Size(545, 20);
                textBoxIdentifier.TabIndex = _currentTabIndex++;
                textBoxIdentifier.TextChanged += GroupBoxSelectorChanged;

                textBoxIdentifier.Text = ((Selector)selector).SelectorString;
                groupBox.Controls.Add(textBoxIdentifier);
            }

            {
//Expected number of elements
                Label labelExpectedNumber = new Label();
                labelExpectedNumber.AutoSize = true;
                labelExpectedNumber.Location = new Point(6, 75);
                //label1.Name = "label3";
                labelExpectedNumber.Size = new Size(150, 13);
                labelExpectedNumber.TabIndex = 999999999;
                labelExpectedNumber.Text = "Expected number of elements:";

                groupBox.Controls.Add(labelExpectedNumber);

                TextBox textBoxExpectedNumber = new TextBox();
                textBoxExpectedNumber.Location = new Point(162, 71);
                textBoxExpectedNumber.Name = "textBoxExpectedNumber"+_currentTabIndex;
                textBoxExpectedNumber.Size = new Size(488, 20);
                textBoxExpectedNumber.TabIndex = _currentTabIndex++;
                textBoxExpectedNumber.KeyPress += TextBoxOnKeyPress;
                textBoxExpectedNumber.TextChanged += GroupBoxSelectorChanged;

                textBoxExpectedNumber.Text = ((Selector) selector).ExpectedNumberOfElements.Value.ToString();
                groupBox.Controls.Add(textBoxExpectedNumber);
            }
        }

        private void GroupBoxSelectorChanged(object sender, EventArgs eventArgs)
        {
            GroupBox groupBox = (GroupBox)((Control)sender).Parent;
            foreach (var parameterToInputObject in _parameterToInputList)
            {
                if (parameterToInputObject.Value is GroupBox)
                {
                    if (((GroupBox)parameterToInputObject.Value) == groupBox)
                    {
                        Selector selector = (Selector)parameterToInputObject.Key;
                        foreach (Control control in groupBox.Controls)
                        {
                            if (control is TextBox && control.Name.Contains("textBoxExpectedNumber"))
                            {
                                if (((TextBox) control).Text == "")
                                    ((TextBox) control).Text = "0";
                                selector.ExpectedNumberOfElements.Value = Convert.ToInt32(((TextBox) control).Text);

                            }
                            else if (control is TextBox && control.Name.Contains("textBoxIdentifier"))
                                selector.SelectorString = ((TextBox) control).Text;
                            else if (control is ComboBox && control.Name.Contains("comboBoxSelectOn"))
                            {
                                ComboBox comboBox = (ComboBox) control;
                                foreach (var value in Enum.GetValues(typeof(BrowserAction.ExecuteActionOn)))
                                {
                                    if (value.ToString() == comboBox.Text)
                                    {
                                        selector.SelectorExecuteActionOn =
                                            (BrowserAction.ExecuteActionOn)
                                            Enum.Parse(typeof(BrowserAction.ExecuteActionOn), comboBox.Text);
                                    }
                                }
                            }
                        }
                        TemplateElement.ReadAvailableInputParameters();
                    }
                }
            }
        }

        private void GroupBoxCreateSchemaType(string name, object schemaType)
        {
            GroupBox groupBox = GroupboxCreate(name, "Schema Type", 47);
            _parameterToInputList.Add(new KeyValuePair<object, object>(
                   schemaType, groupBox));

            {
                //Select ON
                ComboBox comboBoxSchemaType = new ComboBox();
                comboBoxSchemaType.FormattingEnabled = true;
                comboBoxSchemaType.Location = new Point(136, 18);
                //comboBox.Name = "comboBoxExecuteActionOn";
                comboBoxSchemaType.Size = new Size(514, 21);
                comboBoxSchemaType.TabIndex = _currentTabIndex++;
                comboBoxSchemaType.TextChanged += GroupBoxSchemaTypeChanged;

                foreach (var value in Enum.GetValues(typeof(GetHttpAuth.SchemaTypes)))
                {
                    comboBoxSchemaType.Items.Add(value);
                }
                for (var i = 0; i < comboBoxSchemaType.Items.Count; i++)
                {
                    if (((InsecureHttpAuthSchemaType)schemaType).Value.ToString() == comboBoxSchemaType.Items[i].ToString())
                        comboBoxSchemaType.SelectedIndex = i;
                }
               
                groupBox.Controls.Add(comboBoxSchemaType);

                Label labelSchemaType = new Label();
                labelSchemaType.AutoSize = true;
                labelSchemaType.Location = new Point(6, 22);
                //label3.Name = "label1";
                labelSchemaType.Size = new Size(124, 13);
                labelSchemaType.TabIndex = 999999999;
                labelSchemaType.Text = "Expected Schema Type:";

                groupBox.Controls.Add(labelSchemaType);
            }
        }

        private void GroupBoxSchemaTypeChanged(object sender, EventArgs eventArgs)
        {
            GroupBox groupBox = (GroupBox)((Control)sender).Parent;
            foreach (var parameterToInputObject in _parameterToInputList)
            {
                if (parameterToInputObject.Value is GroupBox)
                {
                    if (((GroupBox)parameterToInputObject.Value) == groupBox)
                    {
                        InsecureHttpAuthSchemaType schema = (InsecureHttpAuthSchemaType)parameterToInputObject.Key;
                        foreach (Control control in groupBox.Controls)
                        {
                            if (control is ComboBox)
                            {
                                ComboBox comboBox = (ComboBox)control;
                                foreach (var value in Enum.GetValues(typeof(GetHttpAuth.SchemaTypes)))
                                {
                                    if (value.ToString() == comboBox.Text)
                                    {
                                        schema.Value =
                                           (GetHttpAuth.SchemaTypes)
                                           Enum.Parse(typeof(GetHttpAuth.SchemaTypes), comboBox.Text);
                                    }
                                }
                            }
                        }
                        TemplateElement.ReadAvailableInputParameters();
                    }
                }
            }
        }

        private void GroupBoxCreateJsPromptType(string name, object jsPromptType)
        {
            GroupBox groupBox = GroupboxCreate(name, "Schema Type", 47);
            _parameterToInputList.Add(new KeyValuePair<object, object>(
                    jsPromptType, groupBox));
            {
                //Select ON
                ComboBox comboBoxDialogType = new ComboBox();
                comboBoxDialogType.FormattingEnabled = true;
                comboBoxDialogType.Location = new Point(181, 18);
                //comboBox.Name = "comboBoxExecuteActionOn";
                comboBoxDialogType.Size = new Size(469, 21);
                comboBoxDialogType.TabIndex = _currentTabIndex++;
                comboBoxDialogType.TextChanged += GroupBoxJsPromptTypechanged;

                foreach (var value in Enum.GetValues(typeof(GetJsPrompt.DialogTypes)))
                {
                    comboBoxDialogType.Items.Add(value);
                }
                for (var i = 0; i < comboBoxDialogType.Items.Count; i++)
                {
                    if (((InsecureDialogType)jsPromptType).Value.ToString() == comboBoxDialogType.Items[i].ToString())
                        comboBoxDialogType.SelectedIndex = i;
                }
                
                groupBox.Controls.Add(comboBoxDialogType);

                Label labelDialogType = new Label();
                labelDialogType.AutoSize = true;
                labelDialogType.Location = new Point(6, 22);
                //label3.Name = "label1";
                labelDialogType.Size = new Size(169, 13);
                labelDialogType.TabIndex = 999999999;
                labelDialogType.Text = "Expected JS Dialog Type:";

                groupBox.Controls.Add(labelDialogType);
            }
        }

        private void GroupBoxJsPromptTypechanged(object sender, EventArgs eventArgs)
        {
            GroupBox groupBox = (GroupBox)((Control)sender).Parent;
            foreach (var parameterToInputObject in _parameterToInputList)
            {
                if (parameterToInputObject.Value is GroupBox)
                {
                    if (((GroupBox)parameterToInputObject.Value) == groupBox)
                    {
                        InsecureDialogType dialogType = (InsecureDialogType)parameterToInputObject.Key;
                        foreach (Control control in groupBox.Controls)
                        {
                            if (control is ComboBox)
                            {
                                ComboBox comboBox = (ComboBox)control;
                                foreach (var value in Enum.GetValues(typeof(GetJsPrompt.DialogTypes)))
                                {
                                    if (value.ToString() == comboBox.Text)
                                    {
                                        dialogType.Value =
                                          (GetJsPrompt.DialogTypes)
                                          Enum.Parse(typeof(GetJsPrompt.DialogTypes), comboBox.Text);
                                    }
                                }
                            }
                        }
                        TemplateElement.ReadAvailableInputParameters();
                    }
                }
            }
        }

        private void GroupBoxCreateInsecureDisplayObjects(string name)
        {
            GroupBox groupBox = GroupboxCreate(name, "Insecure Display Objects", 190);

            TabControl tabControl = new TabControl();
            tabControl.Location = new Point(6, 19);
            //tabControl.Name = "tabControl1";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(644, 138);
            tabControl.TabIndex = _currentTabIndex++;
            
            groupBox.Controls.Add(tabControl);
            //_parameterToInputList.Add(new KeyValuePair<object, object>(InsecureDisplayObjectList, tabControl));

            Button buttonAddInsecureTextObject = new Button();
            buttonAddInsecureTextObject.Location = new Point(6, 159);
            //buttonAddSingleObject.Name = "buttonAddSingleInsecureDisplayObject";
            buttonAddInsecureTextObject.Size = new Size(320, 23);
            buttonAddInsecureTextObject.TabIndex = _currentTabIndex++;
            buttonAddInsecureTextObject.Text = "Add new Text Object";
            buttonAddInsecureTextObject.UseVisualStyleBackColor = true;

            buttonAddInsecureTextObject.Click += ButtonAddInsecureTextObject_Click;
            groupBox.Controls.Add(buttonAddInsecureTextObject);

            Button buttonAddInsecureImageObject = new Button();
            buttonAddInsecureImageObject.Location = new Point(330, 159);
            //buttonAddSingleObject.Name = "buttonAddSingleInsecureDisplayObject";
            buttonAddInsecureImageObject.Size = new Size(320, 23);
            buttonAddInsecureImageObject.TabIndex = _currentTabIndex++;
            buttonAddInsecureImageObject.Text = "Add new Image Object";
            buttonAddInsecureImageObject.UseVisualStyleBackColor = true;

            buttonAddInsecureImageObject.Click += ButtonAddInsecureImageObjectOnClick;
            groupBox.Controls.Add(buttonAddInsecureImageObject);

            List<KeyValuePairEx<string, object>> parameters = GetParameters();
            List<object> insecureObjectsList = null;
            foreach (var identifierToObjectKeyValuePair in parameters)
            {
                if (identifierToObjectKeyValuePair.Value is List<object>)
                {
                    insecureObjectsList = (List<object>)identifierToObjectKeyValuePair.Value;
                }
            }
            if (insecureObjectsList == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The should be an insecure objectlist!"));
            }
            Debug.Assert(insecureObjectsList != null, "insecureObjectsList != null");
            foreach (var insecureObject in insecureObjectsList)
            {
                if(insecureObject is InsecureText)
                    AddInsecureTextTabToTabControl(tabControl, (InsecureText)insecureObject);
                else if (insecureObject is InsecureImage)
                    AddInsecureImageTabToTabControl(tabControl, (InsecureImage)insecureObject);
            }
        }

        private void ButtonAddInsecureImageObjectOnClick(object sender, EventArgs eventArgs)
        {
            Button button = (Button)sender;
            GroupBox groupBox = (GroupBox)button.Parent;
            List<KeyValuePairEx<string, object>> parameters = GetParameters();
            List<object> insecureObjectsList = null;
            foreach (var identifierToObjectKeyValuePair in parameters)
            {
                if (identifierToObjectKeyValuePair.Value is List<object>)
                {
                    insecureObjectsList = (List<object>)identifierToObjectKeyValuePair.Value;
                }
            }
            if (insecureObjectsList == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The should be an insecure objectlist!"));
            }

            InsecureImage insecureImage = new InsecureImage();
            Debug.Assert(insecureObjectsList != null, "insecureObjectsList != null");
            insecureObjectsList.Add(insecureImage);

            foreach (Control control in groupBox.Controls)
            {
                if (control is TabControl)
                {
                    AddInsecureImageTabToTabControl((TabControl)control, insecureImage);
                }
            }
        }

        private void ButtonAddInsecureTextObject_Click(object sender, EventArgs e)
        {
            Button button = (Button) sender;
            GroupBox groupBox = (GroupBox)button.Parent;
            List<KeyValuePairEx<string,object>> parameters = GetParameters();
            List<object> insecureObjectsList = null;
            foreach (var identifierToObjectKeyValuePair in parameters)
            {
                if (identifierToObjectKeyValuePair.Value is List<object>)
                {
                    insecureObjectsList = (List<object>) identifierToObjectKeyValuePair.Value;
                }
            }
            if (insecureObjectsList == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The should be an insecure objectlist!"));
            }

            InsecureText insecureText = new InsecureText();
            Debug.Assert(insecureObjectsList != null, "insecureObjectsList != null");
            insecureObjectsList.Add(insecureText);

            foreach (Control control in groupBox.Controls)
            {
                if (control is TabControl)
                {
                    AddInsecureTextTabToTabControl((TabControl)control, insecureText);
                }
            }
        }

        //TODO: insert existing data....
        private void AddInsecureTextTabToTabControl(TabControl tabControl, InsecureText insecureText)
        {
            TabPage tabPage = new TabPage("Text " + (tabControl.TabPages.Count +1));
            tabPage.Padding = new Padding(3);
            tabPage.Size = new Size(636, 112);
            tabPage.Location = new Point(4, 22);
            tabPage.TabIndex = _currentY++;
            //tabPage.Text = "Object 1";
            tabPage.UseVisualStyleBackColor = true;
            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedIndex = tabControl.TabPages.Count - 1;

            _parameterToInputList.Add(new KeyValuePair<object, object>(insecureText, tabPage));

            Button buttonRemove = new Button();
            buttonRemove.Location = new Point(6, 6);
            //buttonRemove.Name = "buttonRemoveSingleInsecureDisplayObject";
            buttonRemove.Size = new Size(58, 100);
            buttonRemove.TabIndex = _currentTabIndex;
            buttonRemove.Text = "Remove this object";
            buttonRemove.UseVisualStyleBackColor = true;

            tabPage.Controls.Add(buttonRemove);
            buttonRemove.Click += ButtonRemove_Click;

            Label labelText = new Label(); 
            labelText.AutoSize = true;
            labelText.Location = new Point(73, 9);
            //labelText.Name = "labelInsecureObjectText";
            labelText.Size = new Size(31, 13);
            labelText.TabIndex = 999999999;
            labelText.Text = "Text:";

            tabPage.Controls.Add(labelText);
            
            TextBox textBoxText = new TextBox();
            textBoxText.Location = new Point(107, 6);
            textBoxText.Multiline = true;
            //textBoxText.Name = "textBoxSingleInsecureObjectText";
            textBoxText.Size = new Size(523, 100);
            textBoxText.TabIndex = _currentTabIndex++;
            textBoxText.Text = insecureText.Value;
            textBoxText.TextChanged += TabControlInsecureObjectsTextChanged;

            tabPage.Controls.Add(textBoxText);
            _parameterToInputList.Add(new KeyValuePair<object, object>(insecureText, tabPage));
        }

        private void TabControlInsecureObjectsTextChanged(object sender, EventArgs eventArgs)
        {
            TabPage tabPage = (TabPage) ((Control) sender).Parent;
            TextBox textBox = (TextBox) sender;
            foreach (var parameterToInput in _parameterToInputList)
            {
                if (parameterToInput.Value is TabPage && parameterToInput.Value == tabPage)
                {
                    InsecureText insecureText = (InsecureText) parameterToInput.Key;
                    insecureText.Value = textBox.Text;
                    TemplateElement.ReadAvailableInputParameters();
                }
            }
        }

        private void AddInsecureImageTabToTabControl(TabControl tabControl, InsecureImage insecureImage)
        {
            TabPage tabPage = new TabPage("Image " + (tabControl.TabPages.Count + 1));
            tabPage.Padding = new Padding(3);
            tabPage.Size = new Size(636, 112);
            tabPage.Location = new Point(4, 22);
            tabPage.TabIndex = _currentY++;
            //tabPage.Text = "Object 1";
            tabPage.UseVisualStyleBackColor = true;
            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedIndex = tabControl.TabPages.Count - 1;

            _parameterToInputList.Add(new KeyValuePair<object, object>(insecureImage, tabPage));

            Button buttonRemove = new Button();
            buttonRemove.Location = new Point(6, 6);
            //buttonRemove.Name = "buttonRemoveSingleInsecureDisplayObject";
            buttonRemove.Size = new Size(58, 100);
            buttonRemove.TabIndex = _currentTabIndex;
            buttonRemove.Text = "Remove this object";
            buttonRemove.UseVisualStyleBackColor = true;

            tabPage.Controls.Add(buttonRemove);
            buttonRemove.Click += ButtonRemove_Click;

            Label labelImage = new Label();
            labelImage.AutoSize = true;
            labelImage.Location = new Point(68, 10);
            //labelImage.Name = "labelSingleInsecureObjectImage";
            labelImage.Size = new Size(39, 13);
            labelImage.TabIndex = 999999999;
            labelImage.Text = "Image:";

            tabPage.Controls.Add(labelImage);

            PictureBox pictureBoxImage = new PictureBox();
            pictureBoxImage.Location = new Point(106, 7);
            //pictureBoxImage.Name = "pictureBoxSingleInsecureImage";
            pictureBoxImage.Size = new Size(106, 100);
            pictureBoxImage.TabIndex = _currentTabIndex++;
            pictureBoxImage.TabStop = false;
            pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImage.BackColor = Color.WhiteSmoke;
            if (BaseObject.ExtractPlaceholdersToListWithoutOutputConcat(insecureImage.Base64EncodedImage).Count <= 1)
            {
                try
                {
                    pictureBoxImage.Image = EncodingEx.Base64.Decoder.DecodeImage(insecureImage.Base64EncodedImage);
                }
                catch (Exception ex)
                {
                    
                }
            }

            tabPage.Controls.Add(pictureBoxImage);

            Label labelImageBase64 = new Label();
            labelImageBase64.AutoSize = true;
            labelImageBase64.Location = new Point(384, 11);
            //labelImageBase64.Name = "labelSingleInsecureObjectImageBase64";
            labelImageBase64.Size = new Size(46, 13);
            labelImageBase64.TabIndex = 999999999;
            labelImageBase64.Text = "Base64:";

            tabPage.Controls.Add(labelImageBase64);

            TextBox textBoxBase64Image = new TextBox();
            textBoxBase64Image.Location = new Point(436, 8);
            textBoxBase64Image.Multiline = true;
            textBoxBase64Image.Name = "textBoxSingleInsecureBase64Image" + _currentTabIndex;
            textBoxBase64Image.Size = new Size(197, 98);
            textBoxBase64Image.TabIndex = _currentTabIndex++;
            textBoxBase64Image.Text = insecureImage.Base64EncodedImage;
            textBoxBase64Image.TextChanged += TextBoxBase64ImageOnTextChanged;
            textBoxBase64Image.TextChanged +=TabPanelInsecureObjectsImageChanged;

            tabPage.Controls.Add(textBoxBase64Image);

            Button copyBase64Button = new Button();
            copyBase64Button.Location = new Point(470, 52);
            //copyBase64Button.Name = "buttonCopyBase64";
            copyBase64Button.Size = new Size(46, 54);
            copyBase64Button.TabIndex = _currentTabIndex++;
            copyBase64Button.Text = "Copy Base 64";
            copyBase64Button.UseVisualStyleBackColor = true;
            copyBase64Button.Click += CopyBase64ButtonOnClick;

            tabPage.Controls.Add(copyBase64Button);

            Button buttonSelectLocalImage = new Button();
            buttonSelectLocalImage.Location = new Point(218, 6);
            //buttonSelectLocalImage.Name = "buttonSelectSingleLocalImage";
            buttonSelectLocalImage.Size = new Size(160, 23);
            buttonSelectLocalImage.TabIndex = _currentTabIndex++;
            buttonSelectLocalImage.Text = "Select local image";
            buttonSelectLocalImage.UseVisualStyleBackColor = true;

            buttonSelectLocalImage.Click += ButtonSelectLocalImageOnClick;
            tabPage.Controls.Add(buttonSelectLocalImage);

            _parameterToInputList.Add(new KeyValuePair<object, object>(insecureImage, tabPage));

        }

        private void TabPanelInsecureObjectsImageChanged(object sender, EventArgs eventArgs)
        {
            TabPage tabPage = (TabPage)((Control)sender).Parent;
            TextBox textBox = (TextBox)sender;
            foreach (var parameterToInput in _parameterToInputList)
            {
                if (parameterToInput.Value is TabPage && parameterToInput.Value == tabPage)
                {
                    InsecureImage insecureImage = (InsecureImage)parameterToInput.Key;
                    insecureImage.Base64EncodedImage = textBox.Text;
                    TemplateElement.ReadAvailableInputParameters();
                }
            }
        }

        private void TextBoxBase64ImageOnTextChanged(object sender, EventArgs eventArgs)
        {
            TextBox textBox = (TextBox)sender;
            TabPage tabPage = (TabPage)textBox.Parent;
            PictureBox pictureBox = null;
            TextBox base64TextBox = null;
            foreach (Control control in tabPage.Controls)
            {
                if (control is PictureBox)
                    pictureBox = (PictureBox) control;
                else if (control is TextBox && control.Name.Contains("textBoxSingleInsecureBase64Image"))
                    base64TextBox = (TextBox) control;

            }
            if (pictureBox == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected", new Exception("There should be an pictureBox!"));
            }
            if (base64TextBox == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected", new Exception("There should be an base64 textBox!"));
            }

            Debug.Assert(base64TextBox != null, "base64TextBox != null");
            if (BaseObject.ExtractPlaceholdersToListWithoutOutputConcat(base64TextBox.Text).Count <= 1)
            {
                try
                {
                    Debug.Assert(pictureBox != null, "pictureBox != null");
                    pictureBox.Image = EncodingEx.Base64.Decoder.DecodeImage(base64TextBox.Text);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void CopyBase64ButtonOnClick(object sender, EventArgs eventArgs)
        {
            Button button = (Button) sender;
            TabPage tabPage = (TabPage)button.Parent;
            TextBox textBox = null;
            foreach (Control control in tabPage.Controls)
            {
                if (control is TextBox && control.Name.Contains("textBoxSingleInsecureBase64Image"))
                    textBox = (TextBox)control;
            }
            if (textBox == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("Could not get the base64 textbox from the tabpage"));
                return;
            }
            Clipboard.SetText(textBox.Text);
        }

        private void ButtonSelectLocalImageOnClick(object sender, EventArgs eventArgs)
        {
            Button button = (Button) sender;
            TabPage tabPage = (TabPage) button.Parent;
            PictureBox pictureBox = null;
            TextBox textBox = null;
            foreach (Control control in tabPage.Controls)
            {
                if (control is PictureBox)
                    pictureBox = (PictureBox)control;
                else if (control is TextBox && control.Name.Contains("textBoxSingleInsecureBase64Image"))
                    textBox = (TextBox) control;
            }
            if (pictureBox == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("Could not get the picturebox from the tabpage"));
                return;
            }
            if (textBox == null)
            {
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("Could not get the base64 textbox from the tabpage"));
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.Drawing.Image image = System.Drawing.Image.FromFile(ofd.FileName);
                    pictureBox.Image = image;
                    string base64 = EncodingEx.Base64.Encoder.EncodeImage(image, image.RawFormat);
                    textBox.Text = base64;
                }
                catch (OutOfMemoryException ex)
                {
                    MessageBox.Show("This was not a valid image");
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show("The file could not be found");
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show("Please do not select an URI");
                }
            }
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            Button button = (Button) sender;
            TabPage tabPage = (TabPage) button.Parent;
            TabControl tabControl = (TabControl) tabPage.Parent;

            foreach (var keyValuePair in _parameterToInputList)
            {
                if (keyValuePair.Value is TabPage)
                {
                    if (keyValuePair.Value == tabPage)
                    {
                        _parameterToInputList.Remove(keyValuePair);
                        break;
                    }
                }
            }


            tabControl.TabPages.Remove(tabPage);
            tabPage.Dispose();
        }

        private void TextBoxOnKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
            if (keyPressEventArgs.KeyChar == '\b')
                return;
            int isNumber = 0;
            keyPressEventArgs.Handled = !int.TryParse(keyPressEventArgs.KeyChar.ToString(), out isNumber);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {

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

        private void textBoxTimeOut_KeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
            if (keyPressEventArgs.KeyChar == '\b')
                return;
            int isNumber = 0;
            keyPressEventArgs.Handled = !int.TryParse(keyPressEventArgs.KeyChar.ToString(), out isNumber);
        }

        private void textBoxTimeOut_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            BaseObject baseObject = (BaseObject) TemplateElement.BrowserActionOrCommand;
            if (textBox.Text == "")
            {
                if (((BaseObject)TemplateElement.BrowserActionOrCommand) is BrowserCommand)
                {
                    ((BaseObject) TemplateElement.BrowserActionOrCommand).TimeoutInSec = null;
                }
                else if (((BaseObject)TemplateElement.BrowserActionOrCommand) is BrowserAction)
                {
                    ((BaseObject) ((BrowserAction) ((BaseObject) TemplateElement.BrowserActionOrCommand)).ActionObject).TimeoutInSec = null;
                }
                return;
            }
            try
            {
                if (((BaseObject)TemplateElement.BrowserActionOrCommand) is BrowserCommand)
                {
                    ((BaseObject)TemplateElement.BrowserActionOrCommand).TimeoutInSec = Convert.ToInt32(textBox.Text);
                }
                else if (((BaseObject)TemplateElement.BrowserActionOrCommand) is BrowserAction)
                {
                    ((BaseObject)((BrowserAction)((BaseObject)TemplateElement.BrowserActionOrCommand)).ActionObject).TimeoutInSec = Convert.ToInt32(textBox.Text);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
