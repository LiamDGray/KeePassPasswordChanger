using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CefBrowserControl;
using KeePassLib;
using KeePassLib.Security;
using KeePassPasswordChanger.Templates;
using Timer = System.Timers.Timer;

namespace KeePassPasswordChanger
{
    public partial class Working : Form
    {
        private Timer _listUpdates, _dottedTimer;
        private int _dotcounter = 0;

        private bool started = false;

        private DateTime DateTimeNotLaterThan = DateTime.MinValue;
        

        public Working()
        {
            InitializeComponent();
            _listUpdates = new Timer();
            _listUpdates.Elapsed += _listUpdates_Elapsed;
            _listUpdates.Interval = 500;
            _listUpdates.AutoReset = false;

            _dottedTimer = new Timer();
            _dottedTimer.Elapsed += _dottedTimer_Elapsed;
            _dottedTimer.Interval = 1000;
            _dottedTimer.AutoReset = false;
        }

        public Working(DateTime selectDateTimeValue) : this()
        {
            DateTimeNotLaterThan = selectDateTimeValue;
        }

        private void _dottedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _dottedTimer.Stop();
            string text = "Changing your passwords, please wait ";
            for (int i = 0; i <= _dotcounter; i++)
                text += ".";
            _dotcounter++;
            _dotcounter = _dotcounter %3;
            this.BeginInvoke((MethodInvoker) delegate()
            {
                labelDescription.Text = text;
            });
            //possible async
                _dottedTimer.Start();
        }

        private void _listUpdates_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckValues();
            _listUpdates.Enabled = true;
        }

        private void Working_Load(object sender, EventArgs e)
        {
        }

        private string GenerateEntryLine(Template template, bool includeUid = true)
        {
            return "\"" + template.UsedEntryName + "\" with template " + (includeUid ? ( template.Name + "(UID: " +
                   template.PwUuid.ToString() + ")") : "") ;
        }

        private string GenerateFailureEntryLine(Template template)
        {
            return GenerateEntryLine(template) + " | " + template.LastTemplateElement + " | " + template.LastTemplateElementFailureReason;
        }

        private string GenerateActiveEntryLine(Template template)
        {
            return GenerateEntryLine(template, false) + " @Position: " + template.LastTemplateElement;
        }

        private void CheckValues()
        {
            while (true)
            {
                try
                {
                    TemplateManagement.LockTemplates.AcquireWriterLock(Options.LockTimeOut);
                    bool ready = false;

                    Dictionary<string, Template> templatesReady = new Dictionary<string, Template>();
                    Dictionary<string, Template> templatesRemoved = new Dictionary<string, Template>();
                    Dictionary<string, Template> templatesInTransit = new Dictionary<string, Template>();
                    Dictionary<string, Template> templatesCompleted = new Dictionary<string, Template>();

                    foreach (var template in TemplateManagement.TemplatesReady)
                    {
                        templatesReady.Add(template.Key, template.Value);
                    }
                    foreach (var template in TemplateManagement.TemplatesRemoved)
                    {
                        templatesRemoved.Add(template.Key, template.Value);
                    }
                    foreach (var template in TemplateManagement.TemplatesInTransit)
                    {
                        templatesInTransit.Add(template.Key, template.Value);
                    }
                    foreach (var template in TemplateManagement.TemplatesCompleted)
                    {
                        templatesCompleted.Add(template.Key, template.Value);
                    }

                    if (this.InvokeRequired)
                        this.BeginInvoke((MethodInvoker) delegate()
                        {
                            listBoxRemainingTemplates.Items.Clear();
                            foreach (var template in templatesReady)
                            {
                                string line = "";
                                line += "\"" + template.Value.UsedEntryName + "\" with template " + template.Value.Name;
                                listBoxRemainingTemplates.Items.Add(line);
                                ready = true;
                            }
                            //if (ready && !_listUpdates.Enabled)
                            //    buttonStartChange.Enabled = true;

                            listBoxRemovedTemplates.Items.Clear();
                            foreach (var template in templatesRemoved)
                            {
                                string line = "";
                                line += GenerateEntryLine(template.Value);
                                listBoxRemovedTemplates.Items.Add(line);
                            }

                            listBoxActiveTemplates.Items.Clear();
                            foreach (var template in templatesInTransit)
                            {
                                string line = "";
                                line += GenerateActiveEntryLine(template.Value) ;
                                listBoxActiveTemplates.Items.Add(line);
                            }

                            listBoxTemplatesCompleted.Items.Clear();
                            foreach (var template in templatesCompleted)
                            {
                                if (!template.Value.Successful)
                                    continue;
                                string line = "";
                                line += GenerateEntryLine(template.Value);
                                listBoxTemplatesCompleted.Items.Add(line);
                            }
                            listBoxFailedTemplates.Items.Clear();
                            foreach (var template in templatesCompleted)
                            {
                                if (template.Value.Successful)
                                    continue;
                                string line = "";
                                line += GenerateFailureEntryLine(template.Value);
                                listBoxFailedTemplates.Items.Add(line);
                            }

                            if (templatesReady.Count == 0 &&
                                templatesInTransit.Count == 0)
                            {
                                ControlBox = true;
                                _dottedTimer.Stop();
                                _listUpdates.Stop();
                                this.BeginInvoke((MethodInvoker) delegate()
                                {
                                    labelDescription.Text = "Finished with changing";
                                });
                                TemplateManagement.StopTemplateManagement();
                                KeePassPasswordChangerExt.SaveCurrentDb();

                                FinishPasswordChangeProcess();
                            }
                        });
                    else/* if (!started)*/
                    {
                        started = true;

                        listBoxRemainingTemplates.Items.Clear();
                        foreach (var template in templatesReady)
                        {
                            string line = "";
                            line += GenerateEntryLine(template.Value);
                            listBoxRemainingTemplates.Items.Add(line);
                            ready = true;
                        }
                        if (ready && !_listUpdates.Enabled)
                            buttonStartChange.Enabled = true;

                        listBoxRemovedTemplates.Items.Clear();
                        foreach (var template in templatesRemoved)
                        {
                            string line = "";
                            line += GenerateEntryLine(template.Value);
                            listBoxRemovedTemplates.Items.Add(line);
                        }
                    }



                    break;
                }
                catch (ApplicationException ex)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
                    if (_listUpdates.Enabled)
                        break;
                }
                finally
                {
                    if (TemplateManagement.LockTemplates.IsWriterLockHeld)
                        TemplateManagement.LockTemplates.ReleaseWriterLock();
                }
                Thread.Sleep(100);
            }
        }

        public void PrepareForm()
        {
            if (KeePassPasswordChangerExt.PrepareDatabase())
            {
                DialogResult allEntriesResult =
                    MessageBox.Show("Notice: Please read the about section of the plugin. This is an alpha release. Backup your Database before you use this plugin.\r\n\r\nDo you want to proceed?",
                        "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (allEntriesResult == DialogResult.No)
                {
                    KeePassPasswordChangerExt.ExtentEntryNote("User aborted...");
                    this.Close();
                }
                else
                {
                    foreach (var pwEntry in KeePassPasswordChangerExt._mHost.MainWindow.ActiveDatabase.RootGroup.GetEntries(true))
                    {
                        try
                        {

                            if (KeePassPasswordChangerExt.GroupPlugin.FindEntry(pwEntry.Uuid, true) != null)
                                continue;
                            if(pwEntry.Strings.Get(PwDefs.UrlField) == null)
                                continue;

                            string uri = pwEntry.Strings.Get(PwDefs.UrlField).ReadString();
                            int count = 0;
                            Template template = null;
                            TemplateManagement.LoadTemplates();
                            foreach (var availableTemplate in TemplateManagement.AvailableTemplates)
                            {
                                if (availableTemplate.Value.BoundUrl.IsRegex.Value)
                                {
                                    if (Regex.IsMatch(uri, availableTemplate.Value.BoundUrl.Value.Value))
                                    {
                                        template = availableTemplate.Value;
                                        count++;
                                    }
                                }
                                else
                                {
                                    if (uri.Contains(availableTemplate.Value.BoundUrl.Value.Value) ||
                                        availableTemplate.Value.BoundUrl.Value.Value.Contains(uri))
                                    {
                                        template = availableTemplate.Value;
                                        count++;
                                    }
                                }

                            }
                            switch (count)
                            {
                                case 0:
                                    KeePassPasswordChangerExt.ExtentEntryNote("Ignoring entry " +
                                                        pwEntry.Strings.Get(PwDefs.TitleField).ReadString() + "(UID: " +
                                                        pwEntry.Uuid.ToString() + ") it has no template");
                                    continue;
                                case 1:
                                    {
                                        PwGroup dustbin =
                                            KeePassPasswordChangerExt._mHost.MainWindow.ActiveDatabase.RootGroup.FindGroup(
                                                KeePassPasswordChangerExt._mHost.MainWindow.ActiveDatabase.RecycleBinUuid, true);
                                        if (dustbin != null)
                                        {
                                            if (dustbin.FindEntry(pwEntry.Uuid, true) != null)
                                            {
                                                KeePassPasswordChangerExt.ExtentEntryNote("Ignoring entry " +
                                                                                             pwEntry.Strings.Get(
                                                                                                     PwDefs.TitleField)
                                                                                                 .ReadString() +
                                                                                             "(UID: " +
                                                                                             pwEntry.Uuid.ToString() +
                                                                                             ") because it is dustbinned");
                                                continue;
                                            }
                                            if (DateTimeNotLaterThan != DateTime.MinValue &&
                                                pwEntry.LastModificationTime >= DateTimeNotLaterThan)
                                            {
                                                KeePassPasswordChangerExt.ExtentEntryNote("Ignoring entry " +
                                                                                             pwEntry.Strings.Get(
                                                                                                     PwDefs.TitleField)
                                                                                                 .ReadString() +
                                                                                             "(UID: " +
                                                                                             pwEntry.Uuid.ToString() +
                                                                                             ") because was modified @ " +
                                                                                             pwEntry
                                                                                                 .LastModificationTime
                                                                                                 .ToString() +
                                                                                             "(UTC) and Limit is: " +
                                                                                             DateTimeNotLaterThan
                                                                                                 .ToString() + "(UTC)");
                                                continue;
                                            }
                                        }

                                        KeePassPasswordChangerExt.ExtentEntryNote("Entry " + pwEntry.Strings.Get(PwDefs.TitleField).ReadString() + "(UID: " +
                                                            pwEntry.Uuid.ToString() + ") matched on the template " + template.Name);
                                        Template templateSibling = (Template)template.Clone();
                                        List<KeyValuePairEx<string, ProtectedString>> parameters =
                                            new List<KeyValuePairEx<string, ProtectedString>>();
                                        List<string> requiredParameters = template.GetRequiredKeepassVariables();
                                        foreach (var requiredKeepassVariable in requiredParameters)
                                        {
                                            try
                                            {
                                                if (
                                                    BaseObject.ExtractSinglePlaceholderToString(requiredKeepassVariable) ==
                                                    "")
                                                {
                                                    ProtectedString newPassword = KeePassPasswordChangerExt
                                                        .GeneratePassword(
                                                            template.PasswordCreationPolicy);
                                                    parameters.Add(
                                                        new KeyValuePairEx<string, ProtectedString>(
                                                            BaseObject.ConvertStringToPlaceholderString(""), newPassword));
                                                    continue;
                                                }
                                                bool foundPwDef = false;
                                                foreach (var pwdef in PwDefs.GetStandardFields())
                                                {
                                                    if (pwdef ==
                                                        BaseObject.ExtractSinglePlaceholderToString(
                                                            requiredKeepassVariable))
                                                    {
                                                        parameters.Add(
                                                            new KeyValuePairEx<string, ProtectedString>(
                                                                requiredKeepassVariable,
                                                                pwEntry.Strings.Get(
                                                                    BaseObject.ExtractSinglePlaceholderToString(
                                                                        requiredKeepassVariable))));
                                                        foundPwDef = true;
                                                        break;
                                                    }
                                                }
                                                if(!foundPwDef)
                                                    parameters.Add(
                                                    new KeyValuePairEx<string, ProtectedString>(
                                                        requiredKeepassVariable,
                                                        new ProtectedString(true, requiredKeepassVariable)));

                                            }
                                            catch (Exception ex)
                                            {
                                                ExceptionHandling.Handling.GetException("Unexpected", ex);
                                            }
                                        }
                                        if (requiredParameters.Count != parameters.Count)
                                        {
                                            KeePassPasswordChangerExt.ExtentEntryNote(
                                                "Sorry, i could not gather all required parameters from the password entry " +
                                                pwEntry.Strings.Get(PwDefs.TitleField).ReadString());
                                        }
                                        else
                                        {
                                            while (true)
                                            {
                                                try
                                                {
                                                    templateSibling.GenerateNewUtid(KeePassPasswordChangerExt.Counter++.ToString());
                                                    TemplateManagement.LockTemplates.AcquireWriterLock(Options.LockTimeOut);
                                                    templateSibling.InitializeTemplate(parameters, KeePassPasswordChangerExt.CefControl, pwEntry.Strings.Get(PwDefs.TitleField).ReadString(), pwEntry.Uuid);
                                                    TemplateManagement.TemplatesReady.Add(templateSibling.UTID,
                                                        templateSibling);
                                                    break;
                                                }
                                                catch (ApplicationException ex)
                                                {
                                                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
                                                }
                                                finally
                                                {
                                                    if (TemplateManagement.LockTemplates.IsWriterLockHeld)
                                                        TemplateManagement.LockTemplates.ReleaseWriterLock();
                                                }
                                            }
                                        }

                                    }
                                    break;
                                default: //Write detailed list!
                                    KeePassPasswordChangerExt.ExtentEntryNote("Entry \"" + pwEntry.Strings.Get(PwDefs.TitleField).ReadString() +
                                                    "\" matches on too many templates, ignoring this one, sorry");
                                    continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandling.Handling.GetException("Unexpected", ex);
                        }
                    }
                }
            }
            else
            {
                return;
            }
            KeePassPasswordChangerExt.UnlockDatabase();
            CheckValues();
        }

        private void listBoxRemainingTemplates_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RemoveSelected();
        }

        private void RemoveSelected()
        {
            while (true)
            {
                try
                {
                    TemplateManagement.LockTemplates.AcquireWriterLock(Options.LockTimeOut);
                    for (int i = 0; i < listBoxRemainingTemplates.Items.Count; i++)
                    {
                        if (listBoxRemainingTemplates.GetSelected(i))
                        {
                            int count = 0;
                            string forRemoving = "";
                            foreach (var UtidTotemplate in TemplateManagement.TemplatesReady)
                            {
                                if (listBoxRemainingTemplates.Items[i].ToString() ==
                                    GenerateEntryLine(UtidTotemplate.Value))
                                {
                                    forRemoving = UtidTotemplate.Key;
                                    break;
                                }
                                count++;
                            }
                            if (forRemoving != "")
                            {
                                TemplateManagement.TemplatesRemoved.Add(forRemoving,
                                    TemplateManagement.TemplatesReady[forRemoving]);
                                TemplateManagement.TemplatesReady.Remove(forRemoving);
                            }
                            else
                            {
                                ExceptionHandling.Handling.GetException("Unexpected", new Exception("This should not happen!"));
                            }
                        }
                    }
                    break;
                }
                catch (ApplicationException ex)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
                }
                finally
                {
                    if (TemplateManagement.LockTemplates.IsWriterLockHeld)
                        TemplateManagement.LockTemplates.ReleaseWriterLock();
                }
                Thread.Sleep(100);
            }
            CheckValues();
        }

        private void AddSelected()
        {
            while (true)
            {
                try
                {
                    TemplateManagement.LockTemplates.AcquireWriterLock(Options.LockTimeOut);
                    for (int i = 0; i < listBoxRemovedTemplates.Items.Count; i++)
                    {
                        if (listBoxRemovedTemplates.GetSelected(i))
                        {
                            int count = 0;
                            string forRemoving = "";
                            foreach (var UtidTotemplate in TemplateManagement.TemplatesRemoved)
                            {
                                if (listBoxRemovedTemplates.Items[i].ToString() ==
                                    GenerateEntryLine(UtidTotemplate.Value))
                                {
                                    forRemoving = UtidTotemplate.Key;
                                    break;
                                }
                                count++;
                            }
                            if (forRemoving != "")
                            {
                                TemplateManagement.TemplatesReady.Add(forRemoving,
                                    TemplateManagement.TemplatesRemoved[forRemoving]);
                                TemplateManagement.TemplatesRemoved.Remove(forRemoving);
                            }
                            else
                            {
                                ExceptionHandling.Handling.GetException("Unexpected", new Exception("This should not happen!"));
                            }
                        }
                    }
                    break;
                }
                catch (ApplicationException ex)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
                }
                finally
                {
                    if (TemplateManagement.LockTemplates.IsWriterLockHeld)
                        TemplateManagement.LockTemplates.ReleaseWriterLock();
                }
                Thread.Sleep(100);
            }
            CheckValues();
        }

        private void listBoxRemovedTemplates_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddSelected();
        }

        private void buttonStartChange_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("All the selected passwords will be changed. Are you really sure?", "Last chance before process starts...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                buttonStartChange.Enabled = false;
                ControlBox = false;
                buttonRemovedEntry.Enabled = false;
                buttonEnqueueEntry.Enabled = false;
                _listUpdates.Start();
                _dottedTimer.Start();
                TemplateManagement.StartTemplateManagement();
                listBoxRemainingTemplates.Enabled = false;
                listBoxRemovedTemplates.Enabled = false;
            }

        }

        private void FinishPasswordChangeProcess()
        {
        }

        private void buttonRemovedEntry_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        private void buttonEnqueueEntry_Click(object sender, EventArgs e)
        {
            AddSelected();
        }

        private void listBoxFailedTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    TemplateManagement.LockTemplates.AcquireWriterLock(Options.LockTimeOut);
                    for (int i = 0; i < listBoxFailedTemplates.Items.Count; i++)
                    {
                        if (listBoxFailedTemplates.GetSelected(i))
                        {
                            int count = 0;
                            foreach (var UtidTotemplate in TemplateManagement.TemplatesCompleted)
                            {
                                if (listBoxFailedTemplates.Items[i].ToString() ==
                                    GenerateFailureEntryLine(UtidTotemplate.Value))
                                {
                                    Clipboard.SetText(GenerateFailureEntryLine(UtidTotemplate.Value));
                                    break;
                                }
                                count++;
                            }
                        }
                    }
                    break;
                }
                catch (ApplicationException ex)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
                }
                finally
                {
                    if (TemplateManagement.LockTemplates.IsWriterLockHeld)
                        TemplateManagement.LockTemplates.ReleaseWriterLock();
                }
                Thread.Sleep(100);
            }
        }

        private void listBoxActiveTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    TemplateManagement.LockTemplates.AcquireWriterLock(Options.LockTimeOut);
                    for (int i = 0; i < listBoxActiveTemplates.Items.Count; i++)
                    {
                        if (listBoxActiveTemplates.GetSelected(i))
                        {
                            int count = 0;
                            foreach (var UtidTotemplate in TemplateManagement.TemplatesInTransit)
                            {
                                string currentEntry = listBoxActiveTemplates.Items[i].ToString(),
                                    checkingEntry = GenerateActiveEntryLine(UtidTotemplate.Value);
                                if (currentEntry == checkingEntry)
                                {
                                    Clipboard.SetText(GenerateActiveEntryLine(UtidTotemplate.Value));
                                    break;
                                }
                                count++;
                            }
                        }
                    }
                    break;
                }
                catch (ApplicationException ex)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
                }
                finally
                {
                    if (TemplateManagement.LockTemplates.IsWriterLockHeld)
                        TemplateManagement.LockTemplates.ReleaseWriterLock();
                }
                Thread.Sleep(100);
            }
        }

        private void Working_FormClosing(object sender, FormClosingEventArgs e)
        {
            CefControl.ClearLists();
            TemplateManagement.ClearLists();
        }
    }
}
