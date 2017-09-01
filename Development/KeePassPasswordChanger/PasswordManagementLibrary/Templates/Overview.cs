using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CefBrowserControl;
using CefBrowserControl.BrowserActions.Elements;
using CefBrowserControl.BrowserActions.Helper;
using CefBrowserControl.BrowserCommands;
using CefBrowserControl.Resources;
using KeePassLib;
using KeePassLib.Security;

namespace KeePassPasswordChanger.Templates
{
    public partial class Overview : Form
    {
        private PwGroup _editPwGroup;
        private List<PwGroup> _allGroups = new List<PwGroup>();
        private List<Template> _currentTemplates;
        private bool _isPrivateMode;

        public Overview()
        {
            InitializeComponent();
        }

        public Overview(bool privateMode) : this()
        {
            _isPrivateMode = privateMode;
            if (privateMode)
            {
                _editPwGroup = KeePassPasswordChangerExt.GroupPrivateTemplates;
                labelViewMode.Text = "private";
            }
            else
            {
                _editPwGroup = KeePassPasswordChangerExt.GroupTemplates;
                labelViewMode.Text = "public";
            }
            _allGroups.Add(KeePassPasswordChangerExt.GroupTemplates);
            _allGroups.Add(KeePassPasswordChangerExt.GroupPrivateTemplates);
        }

        private void Overview_Load(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private string GenerateTemplateString(Template template)
        {
            return template.Name + " (" + template.UTID + ")";
        }

        private void RefreshUI(bool reload = true)
        {   
            if(_currentTemplates == null || reload)
                _currentTemplates = LoadTemplates();
            listBoxSavedTemplates.Items.Clear();
            foreach (var template in _currentTemplates)
            {
                listBoxSavedTemplates.Items.Add(GenerateTemplateString(template));
            }
        }

        private List<Template> LoadTemplates()
        {
            List<Template> templates = new List<Template>();
            foreach (var pwEntry in _editPwGroup.GetEntries(false))
            {
                string encoded = pwEntry.Strings.Get(PwDefs.NotesField).ReadString();
                string serialized = EncodingEx.Base64.Decoder.DecodeString(Encoding.UTF8, encoded);
                Template template = (Template)SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(serialized,
                    typeof(Template));
                templates.Add(template);
            }
            return templates;
        }

        private void buttonCreateTemplate_Click(object sender, EventArgs e)
        {
            Template template = new Template();
            while (true)
            {
                Manipulate manipulate = new Manipulate(template);
                manipulate.ShowDialog();
                if (manipulate.Template != null)
                {
                    if (AddTemplate(manipulate.Template))
                        break;
                    if (
                        MessageBox.Show("Do you want to reopen your last template?", "Reopen Template?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        break;
                }
            }
        }

        private Template GetSelectedTemplate()
        {
            Template template = null;
            foreach (var cTemplates in _currentTemplates)
            {
                for (int i = 0; i < listBoxSavedTemplates.Items.Count; i++)
                {
                    if (listBoxSavedTemplates.GetSelected(i) &&
                        listBoxSavedTemplates.Items[i].ToString() == GenerateTemplateString(cTemplates))
                    {
                        template = cTemplates;
                    }
                }
            }
            return template;
        }

        private void buttonEditSelectedTemplate_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBoxSavedTemplates.SelectedIndex;
            Template template = GetSelectedTemplate();
            if (template != null)
            {
                while (true)
                {
                    Manipulate manipulate = new Manipulate(template);
                    manipulate.ShowDialog();

                    if (UpdateTemplate(template))
                        break;
                    if (
                        MessageBox.Show("Do you want to reopen your last template?", "Reopen Template?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        break;
                }
            }
            listBoxSavedTemplates.SelectedIndex = selectedIndex;
        }

        private void buttonCloneSelectedTemplate_Click(object sender, EventArgs e)
        {
            Template template = GetSelectedTemplate();
            int selectedIndex = listBoxSavedTemplates.SelectedIndex;
            if (template != null)
            {
                Template newTemplate = (Template) template.Clone();
                newTemplate.GenerateNewUtid(DateTime.Now.ToString());
                newTemplate.Name = "Clone " + newTemplate.Name +" "+DateTime.Now.ToString() + Guid.NewGuid();
                newTemplate.BoundUrl.Value.Value = "Clone " + newTemplate.Name + " " + DateTime.Now.ToString() + Guid.NewGuid();
                newTemplate.BoundUrl.IsRegex.Value = false;
                AddTemplate(newTemplate);
            }
            listBoxSavedTemplates.SelectedIndex = selectedIndex;
        }

        private void buttonDeleteSelectedTemplates_Click(object sender, EventArgs e)
        {
            Template template = GetSelectedTemplate();
            if (template != null)
            {
                DeleteTemplate(template);
            }
            if(listBoxSavedTemplates.Items.Count > 0)
                listBoxSavedTemplates.SelectedIndex = 0;
        }

        private void buttonImportTemplates_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Select a destination folder for your " +
                                                 (_isPrivateMode ? "private" : "public") +
                                                 " templates to export to (subfolder will be created)";
                folderBrowserDialog.ShowNewFolderButton = true;
                //folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
                folderBrowserDialog.SelectedPath = "Exported " + (_isPrivateMode ? "Private" : "Public") + " Templates";
                int count = 0;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    if (Directory.Exists(folderBrowserDialog.SelectedPath))
                    {
                        foreach (var file in Directory.GetFiles(folderBrowserDialog.SelectedPath))
                        {
                            if(!file.EndsWith(Options.ExportExtension))
                                continue;
                            try
                            {
                                string content = File.ReadAllText(file, Encoding.UTF8);
                                try
                                {
                                    string decoded = EncodingEx.Base64.Decoder.DecodeString(Encoding.UTF8, content);
                                    try
                                    {
                                        Template template = (Template)
                                            SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(decoded,
                                                typeof(Template));
                                        bool foundAnotherEntry = false;
                                        PwEntry oldEntry = null;
                                        foreach (var pwGroup in _allGroups)
                                        {
                                            foreach (var pwEntry in pwGroup.GetEntries(false))
                                            {
                                                string ser = pwEntry.Strings.Get(PwDefs.NotesField).ReadString();
                                                if (ser != null)
                                                {
                                                    Template entryTemplate = (Template)
                                                        SerializationDotNet2.Xml.Deserializer
                                                            .DeserializeObjectFromString(
                                                                EncodingEx.Base64.Decoder.DecodeString(Encoding.UTF8,
                                                                    ser), typeof(Template));
                                                    if (
                                                        entryTemplate.BoundUrl.Value.Value.Contains(
                                                            template.BoundUrl.Value.Value) ||
                                                        template.BoundUrl.Value.Value.Contains(
                                                            entryTemplate.BoundUrl.Value.Value))
                                                    {
                                                        if (entryTemplate.UTID == template.UTID)
                                                        {
                                                            if (template.TemplateVersion > entryTemplate.TemplateVersion)
                                                            {
                                                                oldEntry = pwEntry;
                                                                continue;
                                                            }
                                                        }
                                                        MessageBox.Show("Cannot add template '" + template.Name +
                                                                        "' (UTID: '" + template.UTID +
                                                                        "') because of the existing template '" +
                                                                        entryTemplate.Name + "' in the group '" +
                                                                        pwGroup.Name + "', which maps on this URI.");
                                                        foundAnotherEntry = true;
                                                    }
                                                    if (entryTemplate.UTID == template.UTID)
                                                    {
                                                        if (template.TemplateVersion > entryTemplate.TemplateVersion)
                                                        {
                                                            oldEntry = pwEntry;
                                                            continue;
                                                        }
                                                        MessageBox.Show("Cannot add template '" + template.Name +
                                                                        "' Version '" + template.TemplateVersion +
                                                                        "' (UTID: '" + template.UTID +
                                                                        ")' because of the existing template '" +
                                                                        entryTemplate.Name + "' has with Version '" +
                                                                        template.TemplateVersion +
                                                                        "' a higher^or equal number in the group '" +
                                                                        pwGroup.Name + "', which has the same UTID");
                                                        foundAnotherEntry = true;
                                                    }
                                                }
                                            }
                                        }
                                        if (foundAnotherEntry)
                                            continue;

                                        if (_isPrivateMode)
                                        {
                                            DialogResult dialogResult = MessageBox.Show(
                                                "Do you want to export the template '" + template.Name + "' Version '" +
                                                template.TemplateVersion.ToString() + "'", "Export Template?",
                                                MessageBoxButtons.YesNoCancel,
                                                MessageBoxIcon.Question);
                                            if (dialogResult == DialogResult.No)
                                                continue;
                                            if (dialogResult == DialogResult.Cancel)
                                                break;
                                        }

                                        PwEntry entry = new PwEntry(true, true);
                                        entry.Strings.Set(PwDefs.NotesField, new ProtectedString(true, content));
                                        entry.Strings.Set(PwDefs.TitleField,
                                            new ProtectedString(true,
                                                template.Name + " Version " + template.TemplateVersion + " UTID: " +
                                                template.UTID));
                                        entry.Strings.Set(PwDefs.UrlField,
                                            new ProtectedString(true, template.BoundUrl.Value.Value));
                                        _editPwGroup.AddEntry(entry, true);
                                        KeePassPasswordChangerExt.RefreshUiEntry(entry);
                                        if (oldEntry != null)
                                        {
                                            PwGroup pwGroup = oldEntry.ParentGroup;
                                            pwGroup.Entries.Remove(oldEntry);
                                            KeePassPasswordChangerExt.RefreshUiGroup(pwGroup);
                                        }
                                        //KeePassPasswordChangerExt.SaveCurrentDb();
                                        count++;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Could not deserialize plaintext\n\n" + ex.Message + "\n\n" +
                                                        ex.StackTrace);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Could not base 64 decode file\n\n" + ex.Message + "\n\n" +
                                                    ex.StackTrace);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Could not read file\n\n" + ex.Message + "\n\n" +
                                                ex.StackTrace);
                            }
                        }
                    }
                }
                MessageBox.Show("Completed importing " + count + " Templates", "Completed");
                if (count > 0)
                {
                    RefreshUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        //https://stackoverflow.com/questions/146134/how-to-remove-illegal-characters-from-path-and-filenames
        public string GetSafeFileName(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public string GetSafeFolderName(string path)
        {
            return string.Join("_", path.Split(Path.GetInvalidPathChars()));
        }


        private void buttonExportSelectedTemplate_Click(object sender, EventArgs e)
        {
            Template template = GetSelectedTemplate();
            ExportTemplates(template);
        }

        private void buttonExportSelectedTemplates_Click(object sender, EventArgs e)
        {
            ExportTemplates();
        }

        private void ExportTemplates(Template templateToExport = null)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Select a destination folder for your " +
                                                  (_isPrivateMode ? "private" : "public") +
                                                  " templates to export to (subfolder will be created)";
                folderBrowserDialog.ShowNewFolderButton = true;
                //folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
                folderBrowserDialog.SelectedPath = "Exported " + (_isPrivateMode ? "Private" : "Public") + " Templates";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!Directory.Exists(folderBrowserDialog.SelectedPath))
                    {
                        MessageBox.Show("The folder does not exists");
                        return;
                    }
                    string newFolderName = folderBrowserDialog.SelectedPath + "\\Exported " +
                                           (_isPrivateMode ? "Private" : "Public") + " Templates @ " +
                                           DateTime.Now.ToString().Replace(':', '.');
                    newFolderName = GetSafeFolderName(newFolderName);
                    Directory.CreateDirectory(newFolderName);
                    int count = 0;
                    foreach (var template in LoadTemplates())
                    {
                        if(templateToExport != null && template.UTID != templateToExport.UTID)
                            continue;
                        if (_isPrivateMode)
                        {
                            DialogResult dialogResult = MessageBox.Show(
                                "Do you want to export the template '" + template.Name + "' Version '" +
                                template.TemplateVersion.ToString() + "'", "Export Template?",
                                MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question);
                            if (dialogResult == DialogResult.No)
                                continue;
                            if (dialogResult == DialogResult.Cancel)
                                break;
                        }

                        string serialized = SerializationDotNet2.Xml.Serializer.SerializeObjectToString(template,
                            typeof(Template));
                        string encoded = EncodingEx.Base64.Encoder.EncodeString(Encoding.UTF8, serialized);
                        string filename =
                            GetSafeFileName(template.Name + " v" + template.TemplateVersion + " " + /* TOOLONG FOR FILES SOMETIMES template.UTID+ */Options.ExportExtension);
                        string fullname = newFolderName + "\\" + filename;
                        File.WriteAllText(fullname, encoded, Encoding.UTF8);
                        count++;
                    }
                    MessageBox.Show("Completed exporting " + count + " Templates", "Completed");
                }
            }
            catch (NotSupportedException)
            {
                MessageBox.Show("Please don't contain any special chars...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        private void buttonCreateDemoTemplate_Click(object sender, EventArgs e)
        {
            List<TemplateElement> templateElements = new List<TemplateElement>();


            SwitchWindowVisibility visibility = new SwitchWindowVisibility();
            visibility.NewInstance();
            visibility.Visible.Value = true;
            templateElements.Add(new TemplateElement("show window", visibility));

            //Dont forget to change identifier to real one!
            LoadUrl url = new LoadUrl();
            url.NewInstance();
            url.Url.Value = "http://keepass.info/help/kb/testform.html";
            templateElements.Add(new TemplateElement("Load Site", url));

            BrowserAction actionLoaded = new BrowserAction();
            SiteLoaded siteLoaded = new SiteLoaded();
            siteLoaded.NewInstance();
            siteLoaded.ExpectedSiteToLoad.Value = url.Url;
            siteLoaded.ExpectedSiteToLoad.IsRegex.Value = false;
            actionLoaded.ActionObject = siteLoaded;
            templateElements.Add(new TemplateElement("Check if Site has loaded", actionLoaded));

            BrowserAction checkNumberOfElements1 = new CefBrowserControl.BrowserAction();
            ElementToLoad loadElement = new ElementToLoad();
            loadElement.NewInstance();
            loadElement.Selector.SelectorExecuteActionOn = BrowserAction.ExecuteActionOn.Id;
            loadElement.Selector.SelectorString = "LoginFormUser";
            loadElement.Selector.ExpectedNumberOfElements = new InsecureInt(1);
            checkNumberOfElements1.ActionObject = loadElement;
            templateElements.Add(new TemplateElement("Check if Form exists", checkNumberOfElements1));


            BrowserAction imageAction = new BrowserAction();
            GetImage getImage1 = new GetImage();
            getImage1.NewInstance();
            imageAction.ActionObject = getImage1;
            getImage1.Selector.SelectorString = "/html/body/table/tbody/tr[1]/td/table/tbody/tr/td[1]/img";
            getImage1.Selector.SelectorExecuteActionOn = BrowserAction.ExecuteActionOn.Xpath;
            TemplateElement getImageTemplateElement = new TemplateElement("Download KeePass Image", imageAction);
            templateElements.Add(getImageTemplateElement);


            GetInputFromUser input = new GetInputFromUser();
            input.NewInstance();
            input.InputNeeded.Value = true;
            input.InsecureDisplayObjects.Add(new InsecureText("Please type in 'OK'. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."));
            input.InsecureDisplayObjects.Add(new InsecureImage(Template.EncodeTemplateElementIdWithOutputName(getImageTemplateElement.UEID, GetImage.KeyList.Base64String.ToString())));
            TemplateElement templateElement1 = new TemplateElement("Get OK from User", input);
            templateElement1.SuccessConditions.Add(new Condition("Contains", Template.EncodeTemplateElementIdWithOutputName("", GetInputFromUser.KeyList.UserInputResult.ToString()), "OK"));
            templateElements.Add(templateElement1);


            //----------------------------


            BrowserAction checkNumberOfElements2 = new CefBrowserControl.BrowserAction();
            ElementToLoad elementToLoad2 = new ElementToLoad();
            elementToLoad2.NewInstance();
            checkNumberOfElements2.ActionObject = elementToLoad2;
            elementToLoad2.Selector.SelectorExecuteActionOn = BrowserAction.ExecuteActionOn.Name;
            elementToLoad2.Selector.SelectorString = "pwd";
            elementToLoad2.Selector.ExpectedNumberOfElements.Value = 1;
            templateElements.Add(new TemplateElement("Check if Form Field 'pwd' exists", checkNumberOfElements2));

            BrowserAction checkNumberOfElements3 = new CefBrowserControl.BrowserAction();
            ElementToLoad elementToLoad3 = new ElementToLoad();
            elementToLoad3.NewInstance();
            checkNumberOfElements3.ActionObject = elementToLoad3;
            elementToLoad3.Selector.SelectorExecuteActionOn = BrowserAction.ExecuteActionOn.Name;
            elementToLoad3.Selector.SelectorString = "LoginForm";
            elementToLoad3.Selector.ExpectedNumberOfElements.Value = 1;
            templateElements.Add(new TemplateElement("Check if Form Field 'LoginForm' exists", checkNumberOfElements3));

            BrowserAction enteryUserName = new BrowserAction();
            SetAttribute setAttributeUserName = new SetAttribute();
            setAttributeUserName.NewInstance();
            enteryUserName.ActionObject = setAttributeUserName;
            setAttributeUserName.AttributeName.Value = "Value";
            setAttributeUserName.ValueToSet.Value =BaseObject.ConvertStringToPlaceholderString(PwDefs.UserNameField);
            setAttributeUserName.Selector.ExpectedNumberOfElements.Value = 1;
            setAttributeUserName.Selector.SelectorExecuteActionOn = BrowserAction.ExecuteActionOn.Id;
            setAttributeUserName.Selector.SelectorString = "LoginFormUser";
            templateElements.Add(new TemplateElement("enter username", enteryUserName));

            BrowserAction enterPassword = new BrowserAction();
            SetAttribute setAttributePassword = new SetAttribute();
            setAttributePassword.NewInstance();
            enterPassword.ActionObject = setAttributePassword;
            setAttributePassword.AttributeName.Value = "Value";
            setAttributePassword.ValueToSet.Value = BaseObject.ConvertStringToPlaceholderString(PwDefs.PasswordField) + " " + BaseObject.ConvertStringToPlaceholderString("");
            setAttributePassword.Selector.ExpectedNumberOfElements.Value = 1;
            setAttributePassword.Selector.SelectorExecuteActionOn = BrowserAction.ExecuteActionOn.Name;
            setAttributePassword.Selector.SelectorString = "pwd";
            templateElements.Add(new TemplateElement("enter password", enterPassword));

            BrowserAction handleJsDialog = new BrowserAction();
            SetJsPrompt setJsPrompt = new SetJsPrompt();
            setJsPrompt.NewInstance();
            handleJsDialog.ActionObject = setJsPrompt;
            setJsPrompt.ExpectedDialogType.Value = GetJsPrompt.DialogTypes.Alert;
            setJsPrompt.ExpectedMessageText.IsRegex.Value = true;
            setJsPrompt.ExpectedMessageText.Value.Value = "The following data would have been submitted";
            templateElements.Add(new TemplateElement("set jsdialog handler", handleJsDialog));

            SwitchWindowVisibility visibility2 = new SwitchWindowVisibility();
            visibility2.NewInstance();
            visibility2.Visible.Value = false;
            templateElements.Add(new TemplateElement("hide window", visibility2));

            BrowserAction clickSubmit = new BrowserAction();
            InvokeSubmit invokeSubmit = new InvokeSubmit();
            invokeSubmit.NewInstance();
            clickSubmit.ActionObject = invokeSubmit;
            invokeSubmit.Selector.SelectorExecuteActionOn = BrowserAction.ExecuteActionOn.Name;
            invokeSubmit.Selector.SelectorString = "LoginForm";
            invokeSubmit.Selector.ExpectedNumberOfElements.Value = 1;
            templateElements.Add(new TemplateElement("submit form", clickSubmit));

            BrowserAction checkJsDialog = new BrowserAction();
            GetJsPrompt getJsPrompt = new GetJsPrompt();
            getJsPrompt.NewInstance();
            checkJsDialog.ActionObject = getJsPrompt;
            getJsPrompt.ExpectedDialogType.Value = GetJsPrompt.DialogTypes.Alert;
            getJsPrompt.ExpectedMessageText.IsRegex.Value = true;
            getJsPrompt.ExpectedMessageText.Value.Value = "The following data would have been submitted";
            TemplateElement templateElement2 = new TemplateElement("get jsdialog", checkJsDialog);
            templateElement2.SuccessConditions.Add(new Condition("Contains",
                Template.EncodeTemplateElementIdWithOutputName("", GetJsPrompt.KeyList.MessageText.ToString()),
                "The following data would have been submitted"));
            templateElements.Add(templateElement2);

            Template template = new Template(1,
                new StringOrRegex() { IsRegex = new InsecureBool(true), Value = new InsecureText("keepass") },
                "KeePass TestForm, some illegal chars: \"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?", new PasswordCreationPolicy(), templateElements);
            AddTemplate(template);
            RefreshUI();
        }

        private void DeleteTemplate(Template template)
        {
            int foundEntries = 0;
            PwEntry singlePwEntry = null;
            foreach (var pwGroup in _allGroups)
            {
                foreach (var pwEntry in pwGroup.GetEntries(false))
                {
                    string ser = pwEntry.Strings.Get(PwDefs.NotesField).ReadString();
                    if (ser != null)
                    {
                        Template entryTemplate = (Template)
                            SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(
                                EncodingEx.Base64.Decoder.DecodeString(Encoding.UTF8, ser), typeof(Template));

                        if (entryTemplate.UTID == template.UTID)
                        {
                            singlePwEntry = pwEntry;
                            foundEntries++;
                        }
                        else if (entryTemplate.BoundUrl.Value.Value.Contains(template.BoundUrl.Value.Value) ||
                            template.BoundUrl.Value.Value.Contains(entryTemplate.BoundUrl.Value.Value))
                        {
                            foundEntries++;
                        }
                    }
                }
            }

            if (singlePwEntry == null)
            {
                MessageBox.Show("Could not find any template to delete, sorry");
                return;
            }
            if (foundEntries > 1)
            {
                MessageBox.Show("There are too many templates that match on the bound URL or share the same UTID");
                return;
            }
            PwGroup pwgroup = singlePwEntry.ParentGroup;
            pwgroup.Entries.Remove(singlePwEntry);

            KeePassPasswordChangerExt.RefreshUiGroup(pwgroup);
            //KeePassPasswordChangerExt.SaveCurrentDb();
            RefreshUI();
        }

        private void CleanTemplateElement(TemplateElement templateElement)
        {
            BaseObject baseObject = ((BaseObject) templateElement.BrowserActionOrCommand);
            baseObject.InputParameterAvailable = null;
            if (templateElement.AppendedTemplateElement != null)
                CleanTemplateElement(templateElement.AppendedTemplateElement);
            foreach (var conditionsToTemplateElement in templateElement.ConditionBasedAppendedTemplateElements)
            {
                if (conditionsToTemplateElement.Value != null)
                    CleanTemplateElement(conditionsToTemplateElement.Value);
            }
        }

        private Template CleanTemplate(Template template)
        {
            template.AvailableResources = null;
            template.InputParameterAvailable = null;
            foreach (var templateTemplateElement in template.TemplateElements)
            {
                CleanTemplateElement(templateTemplateElement);
            }
            return template;
        }

        private bool AddTemplate(Template template)
        {
            bool foundAnotherEntry = false;
            foreach (var pwGroup in _allGroups)
            {
                foreach (var pwEntry in pwGroup.GetEntries(false))
                {
                    string ser = pwEntry.Strings.Get(PwDefs.NotesField).ReadString();
                    if (ser != null)
                    {
                        Template entryTemplate = (Template)
                            SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(
                                EncodingEx.Base64.Decoder.DecodeString(Encoding.UTF8, ser), typeof(Template));
                        if (entryTemplate.BoundUrl.Value.Value.Contains(template.BoundUrl.Value.Value) ||
                            template.BoundUrl.Value.Value.Contains(entryTemplate.BoundUrl.Value.Value))
                        {
                            MessageBox.Show("Cannot add template '" + template.Name + "' (UTID: '" + template.UTID +
                                            "') because of the existing template '"+entryTemplate.Name+"' in the group '"+pwGroup.Name+"', which maps on this URI.");
                            foundAnotherEntry = true;
                        }
                        if (entryTemplate.UTID == template.UTID)
                        {
                            MessageBox.Show("Cannot add template '" + template.Name + "' (UTID: '" + template.UTID +
                                            "') because of the existing template '" + entryTemplate.Name + "' in the group '" + pwGroup.Name + "', which has the same UTID");
                            foundAnotherEntry = true;
                        }
                    }
                }
            }
            
            if (foundAnotherEntry)
                return false;

            string serializedEntry = SerializationDotNet2.Xml.Serializer.SerializeObjectToString(CleanTemplate(template),
                template.GetType());
            string encodedEntry = EncodingEx.Base64.Encoder.EncodeString(Encoding.UTF8, serializedEntry);
            PwEntry entry = new PwEntry(true, true);
            entry.Strings.Set(PwDefs.NotesField, new ProtectedString(true, encodedEntry));
            entry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, template.Name + " Version " + template.TemplateVersion + " UTID: " + template.UTID ));
            entry.Strings.Set(PwDefs.UrlField, new ProtectedString(true, template.BoundUrl.Value.Value));
            _editPwGroup.AddEntry(entry, true);
            KeePassPasswordChangerExt.RefreshUiEntry(entry);
            //KeePassPasswordChangerExt.SaveCurrentDb();
            RefreshUI();
            return true;
        }

        private bool UpdateTemplate(Template template)
        {
            int foundEntries = 0;
            PwEntry singlePwEntry = null;
            foreach (var pwGroup in _allGroups)
            {
                foreach (var pwEntry in pwGroup.GetEntries(false))
                {
                    string ser = pwEntry.Strings.Get(PwDefs.NotesField).ReadString();
                    if (ser != null)
                    {
                        Template entryTemplate = (Template)
                            SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(
                                EncodingEx.Base64.Decoder.DecodeString(Encoding.UTF8, ser), typeof(Template));
                        
                        if (entryTemplate.UTID == template.UTID)
                        {
                            singlePwEntry = pwEntry;
                            foundEntries++;
                        }
                        else if (entryTemplate.BoundUrl.Value.Value.Contains(template.BoundUrl.Value.Value) ||
                            template.BoundUrl.Value.Value.Contains(entryTemplate.BoundUrl.Value.Value))
                        {
                            foundEntries++;
                        }
                    }
                }
            }

            if (singlePwEntry == null)
            {
                
                return AddTemplate(template);
            }
            if (foundEntries > 1)
            {
                RefreshUI(false);
                MessageBox.Show("There are too many templates that match on the bound URL or share the same UTID");
                return false;
            }
            string serializedEntry = SerializationDotNet2.Xml.Serializer.SerializeObjectToString(CleanTemplate(template),
                template.GetType());
            string encodedEntry = EncodingEx.Base64.Encoder.EncodeString(Encoding.UTF8, serializedEntry);
            singlePwEntry.Strings.Set(PwDefs.NotesField, new ProtectedString(true, encodedEntry));
            singlePwEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, template.Name + " Version " + template.TemplateVersion + " UTID: " + template.UTID));
            singlePwEntry.Strings.Set(PwDefs.UrlField, new ProtectedString(true, template.BoundUrl.Value.Value));
            KeePassPasswordChangerExt.RefreshUiEntry(singlePwEntry);
            //KeePassPasswordChangerExt.SaveCurrentDb();
            RefreshUI();
            return true;
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

        private void buttonVisitTemplatesPage_Click(object sender, EventArgs e)
        {
            Process.Start(Options.PublicTemplatesUrl);
        }

        private void Overview_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeePassPasswordChangerExt.SaveCurrentDb();
        }
    }
}
