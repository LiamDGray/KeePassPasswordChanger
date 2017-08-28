using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Serialization;
using CefBrowserControl;
using CefBrowserControl.BrowserActions.Helper;
using CefBrowserControl.BrowserCommands;
using CefBrowserControl.Resources;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;
using KeePassPasswordChanger.Resources;
using Timer = System.Timers.Timer;

namespace KeePassPasswordChanger.Templates
{
    [XmlInclude(typeof(TemplateElement))]
    [XmlInclude(typeof(StringOrRegex))]
    [XmlInclude(typeof(PasswordCreationPolicy))]
    [XmlInclude(typeof(StringOrRegex))]
    [XmlInclude(typeof(BrowserAction))]
    [XmlInclude(typeof(BrowserCommand))]
    [Serializable]
    public class Template : InputParameters,  ICloneable
   { 
        public bool AutomaticallyCloseWindowWhenOutOfCommands = true;
        public const int TemplateVersionCurrentAvailable = 1;
        public static int counter = 0;

        //uniqueTemplateID
        public string UTID { get; set; }

        //for template run and browser identification
        public string UID { get; set; }

        public int MaxTries;

        public int UsedTries;

        //For updates?
       public int TemplateVersion = 1;

        //Template should only work with given URI
        public StringOrRegex BoundUrl = new StringOrRegex();

       public string Name = "";

        public List<KeyValuePairEx<string, object>> AvailableResources;

        public List<TemplateElement> TemplateElements = new List<TemplateElement>();

       public PasswordCreationPolicy PasswordCreationPolicy = new PasswordCreationPolicy();

        private System.Timers.Timer _timer;

        private CefControl _cefControl;

       public bool Completed;

       public bool Successful;

       public string UsedEntryName;

       public string PwUuid;
       public byte[] PwUidBytes;

       public string LastTemplateElement = "";

       public string LastTemplateElementFailureReason = "";

       public bool SetNewPasswordWhenSuccess = true;

        public Template()
        {
            GenerateNewUtid();
            TemplateVersion = 1;

            if (!HaveRequirementsBeenSet)
                HaveRequirementsBeenSet = true;
            else
                return;

            InputParameterAvailable = new List<KeyValuePairEx<string, object>>()
            {
                new KeyValuePairEx<string, object>("UTID", UTID),
                new KeyValuePairEx<string, object>("TemplateVersion", TemplateVersion),
                new KeyValuePairEx<string, object>("BoundUrl", BoundUrl),
                new KeyValuePairEx<string, object>("Name", Name),
                new KeyValuePairEx<string, object>("PasswordCreationPolicy", PasswordCreationPolicy),
                new KeyValuePairEx<string, object>("SetNewPasswordWhenSuccess", SetNewPasswordWhenSuccess),
            };
            InputParameterRequired = new List<string>()
            {
                "UTID",
                "TemplateVersion",
                "BoundUrl",
                "Name",
                "PasswordCreationPolicy",
                "SetNewPasswordWhenSuccess"
            };
        }

        private void GenerateNewUtid()
        {
            UTID = HashingEx.Hashing.GetSha512Hash(DateTime.Now.ToString() + " " + counter++ + KeePassPasswordChangerExt.GeneratePassword(10, true, true, true, true, true, true,false, false).ReadString());
        }

        public void GenerateNewUtid(string additional)
        {
            UTID = HashingEx.Hashing.GetSha512Hash(DateTime.Now.ToString() + " " + counter++ + additional + KeePassPasswordChangerExt.GeneratePassword(10, true, true, true, true, true, true, false, false).ReadString());
        }

        public Template(int templateVersion, StringOrRegex boundUrl, string name,PasswordCreationPolicy passwordCreationPolicy ,List<TemplateElement> templateElements, int maxTries = 3) : base()
        {
            GenerateNewUtid();
            TemplateVersion = templateVersion;
            BoundUrl = boundUrl;
            Name = name;
            MaxTries = maxTries;
            TemplateElements = templateElements;
            PasswordCreationPolicy = passwordCreationPolicy;
        }


        public void InitializeTemplate(List<KeyValuePairEx<string, ProtectedString>> runtimeParameters, CefControl cefControl, string usedEntryName, PwUuid pwUuid)
        {
            PwUuid = pwUuid.ToString();
            PwUidBytes = pwUuid.UuidBytes;
            UsedEntryName = usedEntryName;
            AvailableResources = new List<KeyValuePairEx<string, object>>();
            foreach (var runtimeParameterKeyValuePair in runtimeParameters)
            {
                string identifier = runtimeParameterKeyValuePair.Key;
                AvailableResources.Add(new KeyValuePairEx<string, object>(identifier, new Text(runtimeParameterKeyValuePair.Value)));
            }
            UsedTries = 0;
            _cefControl = cefControl;
        }

       public void StartTemplateRun()
       {
            _timer = new Timer();
            _timer.Interval = 100;
            _timer.Elapsed += TimerTemplateRunOnElapsed;
            _timer.AutoReset = false;
            _timer.Enabled = true;
        }

       public void TimerTemplateRunOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
       {
           _timer.Stop();
           Open cmd = new Open(HashingEx.Hashing.GetSha1Hash(DateTime.Now.ToString() + " " + counter++ + KeePassPasswordChangerExt.GeneratePassword(10, true, true, true, true, true, true, false, false).ReadString()));
           _cefControl.AddCefBrowserCommand(cmd);
           UID = cmd.UID;
           Thread.Sleep(1000);
           bool success = true;

           while (true)
           {
               Thread.Sleep(100);
               if (CefControl.BrowserCommandsCompleted.ContainsKey(cmd.UCID))
                   break;
           }

           if (TemplateElements.Count > 0)
           {
               foreach (var templateElement in TemplateElements)
               {
                   if (!RunTemplate(templateElement, _cefControl))
                   {
                       success = false;
                       break;
                   }
               }
           }

           if (AutomaticallyCloseWindowWhenOutOfCommands && CefControl.CefBrowserSessions.ContainsKey(UID) ||
               !success && CefControl.CefBrowserSessions.ContainsKey(UID))
           {
               Quit quit = new Quit(UID);
               _cefControl.AddCefBrowserCommand(quit);
               while (true)
               {
                   Thread.Sleep(100);
                   if (CefControl.BrowserCommandsCompleted.ContainsKey(cmd.UCID))
                       break;
               }
           }
           Completed = true;
           Successful = success;
           bool run = true;
           while (run)
           {
               try
               {
                   TemplateManagement.LockTemplates.AcquireWriterLock(Options.LockTimeOut);
                   if (TemplateManagement.TemplatesInTransit.ContainsKey(UTID))
                   {
                       TemplateManagement.TemplatesCompleted.Add(UTID,
                           TemplateManagement.TemplatesInTransit[UTID]);
                       if (Successful)
                       {
                           Template template = TemplateManagement.TemplatesInTransit[UTID];
                           ProtectedString oldPassword = null, newPassword = null;
                           foreach (var templateAvailableResourceKeyValuePair in template.AvailableResources)
                           {
                               if (
                                   BaseObject.ExtractSinglePlaceholderToString(templateAvailableResourceKeyValuePair.Key) ==
                                   "" && (templateAvailableResourceKeyValuePair.Value).GetType().Name == "Text")
                               {
                                   newPassword = ((Text) templateAvailableResourceKeyValuePair.Value).Value;
                               }
                               if (
                                   BaseObject.ExtractSinglePlaceholderToString(templateAvailableResourceKeyValuePair.Key) ==
                                   PwDefs.PasswordField &&
                                   (templateAvailableResourceKeyValuePair.Value).GetType().Name == "Text")
                               {
                                   oldPassword = ((Text) templateAvailableResourceKeyValuePair.Value).Value;
                               }
                           }
                           if (template.SetNewPasswordWhenSuccess)
                           {
                               PwEntry entry =
                                   KeePassPasswordChangerExt._mHost.MainWindow.ActiveDatabase.RootGroup.FindEntry(
                                       new PwUuid(template.PwUidBytes),
                                       true);
                               PwEntry newone = new PwEntry(true, true);
                               newone.Strings.Set(PwDefs.PasswordField,
                                   new ProtectedString(true, oldPassword.ReadString()));
                               newone.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "Old Password"));
                               entry.History.Add(newone);
                               if (newPassword != null)
                                   entry.Strings.Set(PwDefs.PasswordField, newPassword);
                               KeePassPasswordChangerExt._mHost.MainWindow.BeginInvoke((MethodInvoker) delegate()
                               {
                                   entry.LastModificationTime = TimeUtil.ToUtc(DateTime.Now, false);
                                   KeePassPasswordChangerExt.RefreshUiEntry(entry);
                               });
                               KeePassPasswordChangerExt.SaveCurrentDb();
                           }
                       }
                       TemplateManagement.TemplatesInTransit.Remove(UTID);
                   }
                   run = false;
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

           //_timer.Start();
       }

       //Returns true on success or false on timeout/fail...
       public bool RunTemplate(TemplateElement element, CefControl cefControl)
       {
           LastTemplateElement = element.Name + " @ TemplateElement-ID " + element.UEID;
           BaseObject baseObject = (BaseObject) element.BrowserActionOrCommand;
           if (baseObject is BrowserCommand)
           {
               if (baseObject.TimeoutInSec != null)
                   baseObject.Timeout = new TimeSpan(0, 0, baseObject.TimeoutInSec.Value);
           }
           else if (baseObject is BrowserAction)
           {
                if (((BaseObject)((BrowserAction)baseObject).ActionObject).TimeoutInSec != null)
                    baseObject.Timeout = new TimeSpan(0, 0, ((BaseObject)((BrowserAction)baseObject).ActionObject).TimeoutInSec.Value);
            }
           //rework anyhow?
           //@ requirement calculation: is required a pwdef placeholder?
           foreach (var requiredParameterString in element.RequiredParameters)
           {
               foreach (var availableResource in AvailableResources)
               {
                   if (requiredParameterString == availableResource.Key && availableResource.Value is Text)
                   {
                       KeyValuePairEx<string, string> data =
                           new KeyValuePairEx<string, string>(requiredParameterString,
                               ((Text) availableResource.Value).Value.ReadString());
                        baseObject.OverloadData.Add(data);
                   }
               }
           }

            if (element.BrowserActionOrCommand is BrowserCommand)
            {

                BrowserCommand command = (BrowserCommand) element.BrowserActionOrCommand;
                command.UID = UID;
                command.GenerateNewUcid(UTID);

                _cefControl.AddCefBrowserCommand(element.BrowserActionOrCommand);

                while (true)
                {
                    Thread.Sleep(100);
                    if (CefControl.BrowserCommandsCompleted.ContainsKey(command.UCID))
                    {
                        BrowserCommand browserCommandCompleted = (BrowserCommand) CefControl.BrowserCommandsCompleted[command.UCID];

                        foreach (var outputPlacholderToValuePair in browserCommandCompleted.ReturnedOutput)
                        {
                            string identifier = EncodeTemplateElementIdWithOutputName(element.UEID,
                                outputPlacholderToValuePair.Key);
                            int? forRemoving = null;
                            for (var i = 0; i < AvailableResources.Count; i++)
                            {
                                
                                if (AvailableResources[i].Key ==
                                    identifier/* && AvailableResources[i].ExpectedValue == null*/)
                                {
                                    forRemoving = i;
                                }
                            }
                            if (forRemoving != null)
                            {
                                AvailableResources.RemoveAt((int)forRemoving);
                            }
                            AvailableResources.Add(
                                   new KeyValuePairEx<string, object>(
                                       identifier, new Text(new ProtectedString(true, outputPlacholderToValuePair.Value != null ? outputPlacholderToValuePair.Value : ""))));
                            //else
                            //{
                            //    //Console.WriteLine("w000T");
                            //}
                        }
                        foreach (var conditionsToTemplateElement in element.ConditionBasedAppendedTemplateElements)
                        {
                            bool success = true;
                            foreach (var condition in conditionsToTemplateElement.Key)
                            {
                                string firstOperand = condition.FirstOperand,
                               secondOperand = condition.SecondOperand;
                                if (firstOperand == BaseObject.ConvertStringToPlaceholderString("successfull"))
                                    firstOperand = browserCommandCompleted.Successful.ToString();
                                else if (firstOperand == BaseObject.ConvertStringToPlaceholderString("completed"))
                                    firstOperand = browserCommandCompleted.Completed.ToString();
                                else if (firstOperand.Contains(BaseObject.ConvertStringToPlaceholderString("output")))
                                {
                                    firstOperand = GetValueFromEncodedTemplateOperand(firstOperand, element);
                                }
                                else if (element.RequiredParameters.Contains(firstOperand))
                                {
                                    foreach (var availableResource in AvailableResources)
                                    {
                                        if (availableResource.Key == firstOperand)
                                        {
                                            firstOperand = availableResource.Value.ToString();
                                            break;
                                        }
                                    }
                                }
                                if (secondOperand == BaseObject.ConvertStringToPlaceholderString("successfull"))
                                    secondOperand = browserCommandCompleted.Successful.ToString();
                                else if (secondOperand == BaseObject.ConvertStringToPlaceholderString("completed"))
                                    secondOperand = browserCommandCompleted.Completed.ToString();
                                else if (secondOperand.Contains(BaseObject.ConvertStringToPlaceholderString("output")))
                                {
                                    secondOperand = GetValueFromEncodedTemplateOperand(firstOperand, element);
                                }
                                else if (element.RequiredParameters.Contains(secondOperand))
                                {
                                    foreach (var availableResource in AvailableResources)
                                    {
                                        if (availableResource.Key == secondOperand)
                                        {
                                            secondOperand = availableResource.Value.ToString();
                                            break;
                                        }
                                    }
                                }
                                if (!condition.Compare(firstOperand, secondOperand))
                                {
                                    success = false;
                                    break;
                                }
                            }
                            if (success)
                            {
                                if (!RunTemplate(conditionsToTemplateElement.Value, _cefControl))
                                    return false;
                            }
                        }
                        foreach (var elementSuccessCondition in element.SuccessConditions)
                        {
                            string firstOperand = elementSuccessCondition.FirstOperand,
                                secondOperand = elementSuccessCondition.SecondOperand;
                            if (firstOperand == BaseObject.ConvertStringToPlaceholderString("successfull"))
                                firstOperand = browserCommandCompleted.Successful.ToString();
                            else if (firstOperand == BaseObject.ConvertStringToPlaceholderString("completed"))
                                firstOperand = browserCommandCompleted.Completed.ToString();
                            else if (firstOperand.Contains(BaseObject.ConvertStringToPlaceholderString("output")))
                            {
                                firstOperand = GetValueFromEncodedTemplateOperand(firstOperand, element);
                            }
                            else if (element.RequiredParameters.Contains(firstOperand))
                            {
                                foreach (var availableResource in AvailableResources)
                                {
                                    if (availableResource.Key == firstOperand)
                                    {
                                        firstOperand = availableResource.Value.ToString();
                                        break;
                                    }
                                }
                            }
                            if (secondOperand == BaseObject.ConvertStringToPlaceholderString("successfull"))
                                secondOperand = browserCommandCompleted.Successful.ToString();
                            else if (secondOperand == BaseObject.ConvertStringToPlaceholderString("completed"))
                                secondOperand = browserCommandCompleted.Completed.ToString();
                            else if (secondOperand.Contains(BaseObject.ConvertStringToPlaceholderString("output")))
                            {
                                secondOperand = GetValueFromEncodedTemplateOperand(firstOperand, element);
                            }
                            else if (element.RequiredParameters.Contains(secondOperand))
                            {
                                foreach (var availableResource in AvailableResources)
                                {
                                    if (availableResource.Key == secondOperand)
                                    {
                                        secondOperand = availableResource.Value.ToString();
                                        break;
                                    }
                                }
                            }
                            if (!elementSuccessCondition.Compare(firstOperand, secondOperand))
                            {
                                LastTemplateElementFailureReason = "Failed @ following success condition: " +
                                                                   elementSuccessCondition.ConditionToString(
                                                                       firstOperand, secondOperand);
                                return false;
                            }
                        }
                        if (element.AppendedTemplateElement != null)
                        {
                            if (!RunTemplate(element.AppendedTemplateElement, _cefControl))
                                return false;
                        }
                        break;
                    }
                    if (CefBrowserControl.Timeout.ShouldBreakDueTimeout(baseObject))
                    {
                        LastTemplateElementFailureReason = "Command Timeout";
                        return false;
                    }
                }
                if (!((BrowserCommand) CefControl.BrowserCommandsCompleted[command.UCID]).Successful)
                {
                    LastTemplateElementFailureReason = "Failed Browser Command in CefBrowser";
                    return false;
                }
            }
            else if (element.BrowserActionOrCommand is BrowserAction)
            {
                BrowserAction action = (BrowserAction)element.BrowserActionOrCommand;
                action.UID = UID;
                action.GenerateNewUCID(UTID);
                ((BaseObject)action.ActionObject).OverloadData = baseObject.OverloadData;
                cefControl.AddCefBrowserAction(element.BrowserActionOrCommand);

                while (true)
                {
                    Thread.Sleep(100);
                    if (CefControl.BrowserActionsCompleted.ContainsKey(action.UCID))
                    {
                        BrowserAction browserActionCompleted = (BrowserAction)CefControl.BrowserActionsCompleted[action.UCID];
                        BaseObject subObject = (BaseObject) browserActionCompleted.ActionObject;

                        foreach (var outputPlacholderToValuePair in subObject.ReturnedOutput)
                        {
                            string identifier = EncodeTemplateElementIdWithOutputName(element.UEID,
                                outputPlacholderToValuePair.Key);
                            int? forRemoving = null;
                            for (var i = 0; i < AvailableResources.Count; i++)
                            {
                                if (AvailableResources[i].Key ==
                                    identifier/* && AvailableResources[i].ExpectedValue == null*/)
                                {
                                    forRemoving = i;
                                }
                            }
                            if (forRemoving != null)
                            {
                                AvailableResources.RemoveAt((int) forRemoving);
                            }
                            AvailableResources.Add(
                                    new KeyValuePairEx<string, object>(
                                        identifier, new Text(new ProtectedString(true, outputPlacholderToValuePair.Value != null ? outputPlacholderToValuePair.Value : ""))));
                            //else
                            //{
                            //    //Console.WriteLine("w000T");
                            //}
                        }

                        foreach (var templateElement in TemplateElements)
                        {
                            if (templateElement.UEID == element.UEID)
                                templateElement.BrowserActionOrCommand = CefControl.BrowserActionsCompleted[action.UCID];
                        }
                        foreach (var conditionsToTemplateElement in element.ConditionBasedAppendedTemplateElements)
                        {
                            bool success = true;
                            foreach (var condition in conditionsToTemplateElement.Key)
                            {
                                string firstOperand = condition.FirstOperand,
                               secondOperand = condition.SecondOperand;
                                if (firstOperand == BaseObject.ConvertStringToPlaceholderString("successfull"))
                                    firstOperand = subObject.Successful.ToString();
                                else if (firstOperand == BaseObject.ConvertStringToPlaceholderString("completed"))
                                    firstOperand = subObject.Completed.ToString();
                                else if (firstOperand.Contains(BaseObject.ConvertStringToPlaceholderString("output")))
                                {
                                    firstOperand = GetValueFromEncodedTemplateOperand(firstOperand, element);
                                }
                                else if (element.RequiredParameters.Contains(firstOperand))
                                {
                                    foreach (var availableResource in AvailableResources)
                                    {
                                        if (availableResource.Key == firstOperand)
                                        {
                                            firstOperand = availableResource.Value.ToString();
                                            break;
                                        }
                                    }
                                }
                                if (secondOperand == BaseObject.ConvertStringToPlaceholderString("successfull"))
                                    secondOperand = subObject.Successful.ToString();
                                else if (secondOperand == BaseObject.ConvertStringToPlaceholderString("completed"))
                                    secondOperand = subObject.Completed.ToString();
                                else if (secondOperand.Contains(BaseObject.ConvertStringToPlaceholderString("output")))
                                {
                                    secondOperand = GetValueFromEncodedTemplateOperand(firstOperand, element);
                                }
                                else if (element.RequiredParameters.Contains(secondOperand))
                                {
                                    foreach (var availableResource in AvailableResources)
                                    {
                                        if (availableResource.Key == secondOperand)
                                        {
                                            secondOperand = availableResource.Value.ToString();
                                            break;
                                        }
                                    }
                                }
                                if (!condition.Compare(firstOperand, secondOperand))
                                {
                                    success = false;
                                    break;
                                }
                            }
                            if (success)
                            {
                                if (!RunTemplate(conditionsToTemplateElement.Value, _cefControl))
                                    return false;
                            }
                            else
                                return false;
                        }
                        foreach (var elementSuccessCondition in element.SuccessConditions)
                        {
                            string firstOperand = elementSuccessCondition.FirstOperand,
                                secondOperand = elementSuccessCondition.SecondOperand;
                            if (firstOperand == BaseObject.ConvertStringToPlaceholderString("successfull"))
                                firstOperand = subObject.Successful.ToString();
                            else if (firstOperand == BaseObject.ConvertStringToPlaceholderString("completed"))
                                firstOperand = subObject.Completed.ToString();
                            else if (firstOperand.Contains(BaseObject.ConvertStringToPlaceholderString("output")))
                            {
                                firstOperand = GetValueFromEncodedTemplateOperand(firstOperand, element);
                            }
                            else if (element.RequiredParameters.Contains(firstOperand))
                            {
                                foreach (var availableResource in AvailableResources)
                                {
                                    if (availableResource.Key == firstOperand)
                                    {
                                        firstOperand = availableResource.Value.ToString();
                                        break;
                                    }
                                }
                            }
                            if (secondOperand == BaseObject.ConvertStringToPlaceholderString("successfull"))
                                secondOperand = subObject.Successful.ToString();
                            else if (secondOperand == BaseObject.ConvertStringToPlaceholderString("completed"))
                                secondOperand = subObject.Completed.ToString();
                            else if (secondOperand.Contains(BaseObject.ConvertStringToPlaceholderString("output")))
                            {
                                secondOperand = GetValueFromEncodedTemplateOperand(secondOperand, element);
                            }
                            else if (element.RequiredParameters.Contains(secondOperand))
                            {
                                foreach (var availableResource in AvailableResources)
                                {
                                    if (availableResource.Key == secondOperand)
                                    {
                                        secondOperand = availableResource.Value.ToString();
                                        break;
                                    }
                                }
                            }
                            if (!elementSuccessCondition.Compare(firstOperand, secondOperand))
                            {
                                LastTemplateElementFailureReason = "Failed @ following success condition: " +
                                                                   elementSuccessCondition.ConditionToString(
                                                                       firstOperand, secondOperand);
                                return false;
                            }
                        }
                        if (element.AppendedTemplateElement != null)
                        {
                            if (!RunTemplate(element.AppendedTemplateElement, _cefControl))
                                return false;
                        }
                        break;
                    }
                    if (CefBrowserControl.Timeout.ShouldBreakDueTimeout(baseObject))
                    {
                        LastTemplateElementFailureReason = "Action Timeout";
                        return false;
                    }
                }
                if (!((BrowserAction) CefControl.BrowserActionsCompleted[action.UCID]).Successful)
                {
                    string text = "";
                    foreach (var keyValuePair in action.ReturnedOutput)
                    {
                        text += "Entry: " + keyValuePair.Key + " --> " + keyValuePair.Value + ".";
                    }
                    LastTemplateElementFailureReason = "Failed Browser Action in CefBrowser: " + text;
                    return false;
                }
            }
           return true;
       }

        //Get through template elements requirements...
        public List<string> GetRequiredKeepassVariables()
        {
            List<string> variables = new List<string>();
            foreach (var templateElement in TemplateElements)
            {
                if (templateElement.RequiredParameters != null)
                {
                    foreach (var requiredParameter in templateElement.RequiredParameters)
                    {
                        if (!variables.Contains(requiredParameter))
                            variables.Add(requiredParameter);
                    }
                    if (templateElement.AppendedTemplateElement != null && templateElement.AppendedTemplateElement.RequiredParameters != null)
                    {
                        foreach (var requiredParameter in templateElement.AppendedTemplateElement.RequiredParameters)
                        {
                            if (!variables.Contains(requiredParameter))
                                variables.Add(requiredParameter);
                        }
                    }
                    foreach (var conditionsToTemplateElement in templateElement.ConditionBasedAppendedTemplateElements)
                    {
                        if (conditionsToTemplateElement.Value != null && conditionsToTemplateElement.Value.RequiredParameters != null)
                        {
                            foreach (var requiredParameter in conditionsToTemplateElement.Value.RequiredParameters)
                            {
                                if (!variables.Contains(requiredParameter))
                                    variables.Add(requiredParameter);
                            }
                        }
                    }
                }
            }
            return variables;
        }

        public static string EncodeTemplateElementIdWithOutputName(string ueid, string outputName)
        {
            return BaseObject.ConvertStringToPlaceholderString("output") + BaseObject.ConvertStringToPlaceholderString(ueid) + BaseObject.ConvertStringToPlaceholderString(outputName);
        }

        public string GetValueFromEncodedTemplateOperand(string operand, TemplateElement currentElement)
        {
            List<string> operandsList = BaseObject.ExtractPlaceholdersToListWithoutOutputConcat(operand);
            if (operandsList.Count < 3)
                ExceptionHandling.Handling.GetException("Unexpected",
                    new Exception("The overloaded list is not valid"));
            string ueid = operandsList[1], name = operandsList[2];
            if (ueid == "")
                ueid = currentElement.UEID;
            foreach (var templateElement in TemplateElements)
            {
                if (templateElement.UEID == ueid)
                {
                    foreach (var returnedOutput in ((BaseObject)templateElement.BrowserActionOrCommand).ReturnedOutput)
                    {
                        if (returnedOutput.Key == name)
                        {
                            return returnedOutput.Value;
                        }
                    }
                }
            }
            ExceptionHandling.Handling.GetException("Unexpected", new Exception(
                "Your template element could not get the output from the desired element... sorry get better templates"));
            return null;
        }

        public object Clone()
       {
           counter++;
           

           Template clone = (Template) this.MemberwiseClone();
            List<TemplateElement> newElements = new List<TemplateElement>();
            foreach (TemplateElement element in clone.TemplateElements)
            {
                newElements.Add((TemplateElement)element.Clone());
            }
            clone.TemplateElements = newElements;
            clone.GenerateNewUtid();
           return clone;
       }
   }
}
