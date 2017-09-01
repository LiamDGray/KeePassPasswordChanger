using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;
using KeePassPasswordChanger.Templates;
using DateTimePicker = KeePassPasswordChanger.DateTimePicker;
using Options = KeePassPasswordChanger.Options;
using Timer = System.Timers.Timer;

namespace KeePassPasswordChanger
{
    public sealed class KeePassPasswordChangerExt : Plugin
    {
        public override string UpdateUrl { get; } = Options.UpdateUrl;

        private const string MessageDatabaseNotOpened = "Database should be opened and unlocked";

        public static ReaderWriterLock DatabaseLock = new ReaderWriterLock();

        //For specific manipulations
        private ToolStripMenuItem _menuItemSelectedEntries;

        private ToolStripMenuItem _menuItemPlugin;
        private const string MenuString = "Password Management Extension";

        private ToolStripMenuItem _menuItemChangeAllPasswords;
        private const string MenuChangeAllPasswords = "Change all passwords";
        private ToolStripMenuItem _menuItemChangeOldEntries;
        private const string MenuChangeOldEntries = "Change passwords which have not been updated before ...";


        private ToolStripMenuItem _menuItemAbout;
        private const string MenuAbout = "About";

        private ToolStripMenuItem _menuItemTemplateManagement;
        private const string MenuTemplateManagement = "Template Management...";

        private ToolStripMenuItem _menuItemManagePublicTemplates;
        private const string MenuManagePublicTemplates = "Manage Public Templates";
        private ToolStripMenuItem _menuItemManagePrivateTemplates;
        private const string MenuManagePrivateTemplates = "Manage Private Templates";

        private const string GroupNamePlugin = "KeePassPasswordChanger";
        private const string GroupNameLogging = "Logging";
        private const string GroupNameTemplates = "Templates";
        private const string GroupNamePrivateTemplates = "Private";

        private static readonly ReaderWriterLock Lock = new ReaderWriterLock();
        public static int Counter = 0;

        private List<ToolStripSeparator> _helloSeparator = null;

        public static Timer TimerTemplateRun;


        //SESSION DEPENDED PARAMETERS
        public static IPluginHost _mHost;
        public static string PluginTurnId = "";
        public static PwGroup GroupPlugin = null, GroupLogging = null, GroupTemplates = null, GroupPrivateTemplates = null;
        private static PwEntry _currentLogginEntry;

        private Thread controlThread;
        public static CefControl CefControl;


        public override bool Initialize(IPluginHost host)
        {
            _mHost = host;
            //lockSlim = new ReaderWriterLockSlim();
            _helloSeparator = new List<ToolStripSeparator>();

            ToolStripItemCollection tsMenu = _mHost.MainWindow.MainMenuStrip.Items;

            _menuItemPlugin = new ToolStripMenuItem {Text = MenuString};
            tsMenu.Add(_menuItemPlugin);

            ToolStripItemCollection pluginMenu = _menuItemPlugin.DropDown.Items;

            _menuItemChangeAllPasswords = new ToolStripMenuItem {Text = MenuChangeAllPasswords};
            _menuItemChangeAllPasswords.Click += MenuItemChangeAllPasswordsClick;
            pluginMenu.Add(_menuItemChangeAllPasswords);

            _menuItemChangeOldEntries = new ToolStripMenuItem { Text = MenuChangeOldEntries };
            _menuItemChangeOldEntries.Click += MenuItemChangeOldEntriesOnClick;

            pluginMenu.Add(_menuItemChangeOldEntries);

            {
                var seperator = new ToolStripSeparator();
                _helloSeparator.Add(seperator);
                pluginMenu.Add(seperator);
            }

            _menuItemTemplateManagement = new ToolStripMenuItem { Text = MenuTemplateManagement };
            _menuItemTemplateManagement.Click +=MenuItemTemplateManagementOnClick;
            pluginMenu.Add(_menuItemTemplateManagement);

            ToolStripItemCollection templateMenu = _menuItemTemplateManagement.DropDown.Items;

            _menuItemManagePrivateTemplates = new ToolStripMenuItem { Text = MenuManagePrivateTemplates };
            _menuItemManagePrivateTemplates.Click += MenuItemManagePrivateTemplatesOnClick;
            templateMenu.Add(_menuItemManagePrivateTemplates);

            _menuItemManagePublicTemplates = new ToolStripMenuItem { Text = MenuManagePublicTemplates };
            _menuItemManagePublicTemplates.Click += MenuItemManagePublicTemplatesOnClick;
            templateMenu.Add(_menuItemManagePublicTemplates);

            {
                var seperator = new ToolStripSeparator();
                _helloSeparator.Add(seperator);
                pluginMenu.Add(seperator);
            }

            _menuItemAbout = new ToolStripMenuItem { Text = MenuAbout };
            _menuItemAbout.Click += MenuItemAboutOnClick;
            pluginMenu.Add(_menuItemAbout);






            controlThread = new Thread(new ThreadStart(StartControl));
            controlThread.Start();

            //_helloSeparator = new ToolStripSeparator();
            //tsMenu.Add(_helloSeparator);
            return true;
        }

        private void MenuItemManagePublicTemplatesOnClick(object sender, EventArgs eventArgs)
        {
            if (!PrepareDatabase())
                return;
            Overview overview = new Overview(false);
            overview.ShowDialog();
        }

        private void MenuItemManagePrivateTemplatesOnClick(object sender, EventArgs eventArgs)
        {
            if (!PrepareDatabase())
                return;
            DialogResult result = MessageBox.Show("Please confirm that you want to edit your private templates",
                "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                Overview overview = new Overview(true);
                overview.ShowDialog();
            }
        }

        private void MenuItemAboutOnClick(object sender, EventArgs eventArgs)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void MenuItemChangeOldEntriesOnClick(object sender, EventArgs eventArgs)
        {
            if (!PrepareDatabase())
                return;
            TemplateManagement.GenerateTestTemplate();

            DateTimePicker picker = new DateTimePicker();
            picker.ShowDialog();

            if (picker.Success)
            {
                DateTime convertedDateTime = TimeUtil.ToUtc(picker.dateTimePicker1.Value, false);
                Working working = new Working(convertedDateTime);
                working.PrepareForm();
                if(!working.IsDisposed)
                    working.ShowDialog();
            }
        }

        private void StartControl()
        {
            CefControl = new CefControl();
        }

        private void MenuItemTemplateManagementOnClick(object sender, EventArgs eventArgs)
        {

            
        }


        private static string GeneratePluginTurnId()
        {
            return "" + DateTime.Now + " " +Counter++;
        }

        public static void RefreshUiGroup(PwGroup group = null)
        {
            if (group == null)
                group = _mHost.MainWindow.ActiveDatabase.RootGroup;
            _mHost.MainWindow.UpdateUI(false, null, true, group, true, null, true);
        }

        public static void RefreshUiEntry(PwEntry entry)
        {
            _mHost.MainWindow.UpdateUI(false, null, false, null, true, entry.ParentGroup, true);
        }

        private static PwEntry GetEntryFromGroup(string title, PwGroup group, bool includeSubGroups = true)
        {
            PwObjectList<PwEntry> entries = group.GetEntries(includeSubGroups);
            foreach (PwEntry entry in entries)
            {
                if (entry.Strings.Get(PwDefs.TitleField).ReadString() == title)
                    return entry;
            }
            return null;
        }

        public static void ExtentEntryNote(string additionalNote, bool enhanceSeperator = false)
        {
            if (enhanceSeperator)
                additionalNote = "---- " + DateTime.Now + "----\n" + additionalNote;
            _currentLogginEntry.Strings.Set(PwDefs.NotesField,
                new ProtectedString(true,
                    _currentLogginEntry.Strings.Get(PwDefs.NotesField).ReadString() + Environment.NewLine +
                    additionalNote));
            _currentLogginEntry.LastModificationTime = TimeUtil.ToUtc(DateTime.Now, false);
            RefreshUiEntry(_currentLogginEntry);
        }

        public static void SaveCurrentDb()
        {
            while (true)
            {
                try
                {
                    DatabaseLock.AcquireWriterLock(Options.LockTimeOut);

                    if (_mHost.MainWindow.InvokeRequired)
                    {
                        _mHost.MainWindow.BeginInvoke((MethodInvoker) delegate()
                        {
                            //ExtentEntryNote("Database is going to be saved", true);
                            _mHost.MainWindow.ActiveDatabase.Save(new NullStatusLogger());
                            _mHost.MainWindow.UpdateUI(false, null, false, null, false,
                                null, false, null);
                        });
                    }
                    else
                    {
                        //ExtentEntryNote("Database is going to be saved", true);
                        _mHost.MainWindow.ActiveDatabase.Save(new NullStatusLogger());
                        _mHost.MainWindow.UpdateUI(false, null, false, null, false,
                            null, false, null);
                    }
                    break;
                }
                catch (ApplicationException ex)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
                }
                finally
                {
                    if (DatabaseLock.IsWriterLockHeld)
                        DatabaseLock.ReleaseWriterLock();
                }
                Thread.Sleep(100);
            }
        }

        private static PwGroup GetPwGroup(PwObjectList<PwGroup> containingPwGroup, string identifier)
        {
            foreach (PwGroup group in containingPwGroup)
            {
                if (group.Name == identifier && !group.ParentGroup.Uuid.Equals(_mHost.MainWindow.ActiveDatabase.RecycleBinUuid))
                {
                    return (group);
                }
            }
            return null;
        }

        private static PwObjectList<PwGroup> GetPwGroups()
        {
            return new PwObjectList<PwGroup>
                {
                    _mHost.MainWindow.ActiveDatabase.RootGroup,
                    _mHost.MainWindow.ActiveDatabase.RootGroup.GetGroups(true)
                };
        }

        private static PwGroup AddPwGroup(PwGroup containingPwGroup, string newGroupnam)
        {
            PwGroup group = new PwGroup(true, true, newGroupnam, PwIcon.Archive);
            containingPwGroup.AddGroup(group, true);
            RefreshUiGroup(containingPwGroup);
            return group;
        }

        private static bool CheckDatabase()
        {

            if (!Lock.IsWriterLockHeld)
                return false;
            PwObjectList<PwGroup> pwGroups = GetPwGroups();
            //-------CHECK FOR THE MAIN PLUGIN GROUP IN KEEPASS DB
            GroupPlugin = GetPwGroup(pwGroups, GroupNamePlugin);
            bool createdPluginGroup = GroupPlugin == null;
            if (GroupPlugin == null)
                GroupPlugin = AddPwGroup(_mHost.MainWindow.ActiveDatabase.RootGroup, GroupNamePlugin);
            //------CHECK FOR THE LOGGING GROUP IN KEEPASS DB
            GroupLogging = GetPwGroup(pwGroups, GroupNameLogging);
            bool createdPluginLoggingGroup = GroupLogging == null;
            if (GroupLogging == null)
                GroupLogging = AddPwGroup(GroupPlugin, GroupNameLogging);
            //------CHECK FOR THE TEMPLATES GROUP IN KEEPASS DB
            GroupTemplates = GetPwGroup(pwGroups, GroupNameTemplates);
            bool createdPluginTemplateGroup = GroupTemplates == null;
            if (GroupTemplates == null)
                GroupTemplates = AddPwGroup(GroupPlugin, GroupNameTemplates);
            //------CHECK FOR THE TEMPLATES GROUP IN KEEPASS DB
            GroupPrivateTemplates = GetPwGroup(pwGroups, GroupNamePrivateTemplates);
            bool createdPluginPrivateTemplateGroup = GroupPrivateTemplates == null;
            if (GroupPrivateTemplates == null)
                GroupPrivateTemplates = AddPwGroup(GroupPlugin, GroupNamePrivateTemplates);

            //Generate Unique Turn Identifier and write basic log to notes entry
            //Refresh must be made because strings are directly set
            PluginTurnId = GeneratePluginTurnId();
            _currentLogginEntry = new PwEntry(true, true);
            _currentLogginEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, PluginTurnId));
            _currentLogginEntry.Strings.Set(PwDefs.NotesField, new ProtectedString(true, ""));
            GroupLogging.AddEntry(_currentLogginEntry, true);
            RefreshUiEntry(_currentLogginEntry);

            ExtentEntryNote(_mHost.MainWindow.ActiveDatabase.Name + ": checking the Database", true);


            //Writing group creation to log
            if (createdPluginGroup)
                ExtentEntryNote("Created Plugin Group");
            if (createdPluginLoggingGroup)
                ExtentEntryNote("Created Plugin Logging Group");
            if (createdPluginTemplateGroup)
                ExtentEntryNote("Created Plugin Templates Group");
            if (createdPluginPrivateTemplateGroup)
                ExtentEntryNote("Created private Plugin Templates Group");
            return true;
        }

        public static bool PrepareDatabase()
        {
            PwDatabase pwd = _mHost.MainWindow.ActiveDatabase;
            if (!pwd.IsOpen)
            {
                MessageBox.Show(MessageDatabaseNotOpened);
                return false;
            }
            try
            {
                Lock.AcquireWriterLock(Options.LockTimeOut);
                bool enter = PluginTurnId == "";


                CheckDatabase();

                //Simple log entry for each pw db initialization
                ExtentEntryNote(pwd.Name + " is opened", true);
                SaveCurrentDb();
                return true;
            }
            catch(ApplicationException ex)
            {
                ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
            }
            finally
            {
                if (Lock.IsWriterLockHeld)
                    Lock.ReleaseWriterLock();
            }
            return false;
        }

        public static void UnlockDatabase()
        {
            PluginTurnId = "";
            GroupPlugin = null;
            GroupLogging = null;
            GroupTemplates = null;
            _currentLogginEntry = null;
            if (Lock.IsWriterLockHeld)
                Lock.ReleaseWriterLock();
        }

        public static ProtectedString GeneratePassword(PasswordCreationPolicy policy)
        {
            return GeneratePassword(policy.Length, policy.LowerCase, policy.UpperCase, policy.Digits, policy.Punctuation,
                policy.Brackets, policy.SpecialAscii, policy.ExcludeLookAlike, policy.NoRepeatingCharacters);
        }

        public static ProtectedString GeneratePassword(int passwordLength, bool lowerCase, bool upperCase, bool digits,
        bool punctuation, bool brackets, bool specialAscii, bool excludeLookAlike, bool noRepeating)
        {
            var ps = new ProtectedString();
            var profile = new PwProfile();
            profile.CharSet = new PwCharSet();
            profile.CharSet.Clear();

            if (lowerCase)
                profile.CharSet.AddCharSet('l');
            if (upperCase)
                profile.CharSet.AddCharSet('u');
            if (digits)
                profile.CharSet.AddCharSet('d');
            if (punctuation)
                profile.CharSet.AddCharSet('p');
            if (brackets)
                profile.CharSet.AddCharSet('b');
            if (specialAscii)
                profile.CharSet.AddCharSet('s');

            profile.ExcludeLookAlike = excludeLookAlike;
            profile.Length = (uint)passwordLength;
            profile.NoRepeatingCharacters = noRepeating;

            KeePassLib.Cryptography.PasswordGenerator.PwGenerator.Generate(out ps, profile, null, _mHost.PwGeneratorPool );

            return ps;
        }

        private void MenuItemChangeAllPasswordsClick(object sender, System.EventArgs e)
        {
            if (!PrepareDatabase())
                return;
            TemplateManagement.GenerateTestTemplate();

            Working working = new Working();
            working.PrepareForm();
            if(!working.IsDisposed)
                working.ShowDialog();
        }

        public override void Terminate()
        {
            if (CefControl != null)
            {
                CefControl.KillRpc("CefBrowser");
            }

            ToolStripItemCollection tsMenu = _mHost.MainWindow.MainMenuStrip.Items;

            tsMenu.Remove(_menuItemPlugin);

            ToolStripItemCollection pluginMenu = _menuItemPlugin.DropDown.Items;

            _menuItemChangeAllPasswords.Click -= MenuItemChangeAllPasswordsClick;
            pluginMenu.Remove(_menuItemChangeAllPasswords);

            _menuItemChangeOldEntries.Click -= MenuItemChangeOldEntriesOnClick;
            pluginMenu.Remove(_menuItemChangeOldEntries);

            _menuItemTemplateManagement.Click -= MenuItemTemplateManagementOnClick;
            pluginMenu.Remove(_menuItemTemplateManagement);

            _menuItemTemplateManagement.Click -=MenuItemTemplateManagementOnClick;
            pluginMenu.Remove(_menuItemTemplateManagement);

            _menuItemAbout.Click -= MenuItemAboutOnClick;
            pluginMenu.Remove(_menuItemAbout);

            ToolStripItemCollection templateMenu = _menuItemTemplateManagement.DropDown.Items;

            foreach (var toolStripSeparator in _helloSeparator)
            {
                pluginMenu.Remove(toolStripSeparator);
            }

            _menuItemManagePrivateTemplates.Click -= MenuItemManagePrivateTemplatesOnClick;
            templateMenu.Remove(_menuItemManagePrivateTemplates);

            _menuItemManagePublicTemplates.Click -= MenuItemManagePublicTemplatesOnClick;
            templateMenu.Remove(_menuItemManagePublicTemplates);
        }
    }
}
