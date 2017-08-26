using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ExceptionHandling;
using KeePassLib;
using Timer = System.Timers.Timer;

namespace KeePassPasswordChanger.Templates
{
    public class TemplateManagement
    {
        public static Dictionary<string, Template> AvailableTemplates = new Dictionary<string, Template>();

        //public Dictionary<string, Queue<string>> LastTemplateElement = new Dictionary<string, Queue<string>>();

        private static System.Timers.Timer _timerTemplateManagement;
        public static Dictionary<string, System.Timers.Timer> TemplateTimers = new Dictionary<string, Timer>();

        public static ReaderWriterLock LockTemplates = new ReaderWriterLock();
        public static Dictionary<string, Template> TemplatesReady = new Dictionary<string, Template>();
        public static Dictionary<string, Template> TemplatesRemoved = new Dictionary<string, Template>();
        public static Dictionary<string, Template> TemplatesInTransit = new Dictionary<string, Template>();
        public static Dictionary<string, Template> TemplatesCompleted = new Dictionary<string, Template>();

        private static bool _shouldBeEnabled;

        private static bool testTemplatesGenerated;

        public TemplateManagement()
        {
            
        }

        public static void StartTemplateManagement()
        {
            if (_timerTemplateManagement == null)
            {
                _timerTemplateManagement = new System.Timers.Timer();
                _timerTemplateManagement.Interval = 100;
                _timerTemplateManagement.Elapsed += _timerTemplateManagement_Elapsed;
                _timerTemplateManagement.AutoReset = false;
            }
            _shouldBeEnabled = true;
            _timerTemplateManagement.Start();
        }

        public static void LoadTemplates()
        {
            AvailableTemplates.Clear();
            List<PwGroup> _allGroups = new List<PwGroup>();
            _allGroups.Add(KeePassPasswordChangerExt.GroupTemplates);
            _allGroups.Add(KeePassPasswordChangerExt.GroupPrivateTemplates);
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
                       AvailableTemplates.Add(entryTemplate.UTID, entryTemplate);
                    }
                }
            }
        }

        public static void ClearLists()
        {
            while (true)
            {
                try
                {
                    LockTemplates.AcquireWriterLock(Options.LockTimeOut);

                    TemplatesReady.Clear();
                    TemplatesRemoved.Clear();
                    TemplatesInTransit.Clear();
                    TemplatesCompleted.Clear();
                    break;
                }
                catch (ApplicationException ex)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
                }
                finally
                {
                    if (LockTemplates.IsWriterLockHeld)
                        LockTemplates.ReleaseWriterLock();
                }
                Thread.Sleep(100);
            }
        }

        public static void StopTemplateManagement()
        {
            _shouldBeEnabled = false;
            _timerTemplateManagement.Stop();
        }

        private static void _timerTemplateManagement_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timerTemplateManagement.Stop();

            try
            {
                LockTemplates.AcquireWriterLock(Options.LockTimeOut);

                List<string> forRemoving = new List<string>();
                foreach (var template in TemplatesReady)
                {
                    if (TemplatesInTransit.Count < CefBrowserControl.Options.MaxBrowserInstances)
                    {
                        TemplatesInTransit.Add(template.Key, TemplatesReady[template.Key]);
                        template.Value.StartTemplateRun();
                        forRemoving.Add(template.Key);
                        //Thread.Sleep(100);
                    }
                }
                foreach (string utid in forRemoving)
                {
                    TemplatesReady.Remove(utid);
                }
                forRemoving.Clear();
            }
            catch (ApplicationException ex)
            {
                ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
            }
            finally
            {
                if (LockTemplates.IsWriterLockHeld)
                    LockTemplates.ReleaseWriterLock();
            }

            if(_shouldBeEnabled)
                _timerTemplateManagement.Start();
        }

        public static void AddTemplate(Template template)
        {
            while (true)
            {
                try
                {
                    LockTemplates.AcquireWriterLock(Options.LockTimeOut);
                    try
                    {
                        AvailableTemplates.Add(template.UTID, template);
                    }
                    catch (Exception ex)
                    {
                        throw Handling.GetException("Unexpected", ex);
                    }
                    return;
                }
                catch (ApplicationException ex)
                {
                    Thread.Sleep(100);
                    throw Handling.GetException("ReaderWriterLock", ex);
                }
                finally
                {
                    if (LockTemplates.IsWriterLockHeld)
                        LockTemplates.ReleaseWriterLock();
                }
            }
        }

        public static void RemoveTemplate(string utid)
        {
            while (true)
            {
                try
                {
                    LockTemplates.AcquireWriterLock(Options.LockTimeOut);
                    try
                    {
                        AvailableTemplates.Remove(utid);
                    }
                    catch (Exception ex)
                    {
                        throw Handling.GetException("Unexpected", ex);
                    }
                    return;
                }
                catch (ApplicationException ex)
                {
                    Thread.Sleep(100);
                    throw Handling.GetException("ReaderWriterLock", ex);
                }
                finally
                {
                    if (LockTemplates.IsWriterLockHeld)
                        LockTemplates.ReleaseWriterLock();
                }
            }
        }

        public static void AddKeePassDemoTemplate()
        {
            if (testTemplatesGenerated)
                return;
            testTemplatesGenerated = true;


            
            //AddTemplate(template);
        }

        public static void GenerateTestTemplate()
        {
            AddKeePassDemoTemplate();


         

            
        }
    }
}
