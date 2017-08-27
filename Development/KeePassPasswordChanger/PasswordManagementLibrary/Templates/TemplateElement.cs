using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CefBrowserControl;
using CefBrowserControl.BrowserActions.Elements;
using CefBrowserControl.BrowserActions.Elements.EventTypes;
using CefBrowserControl.BrowserActions.Helper;
using CefBrowserControl.BrowserCommands;
using CefBrowserControl.Resources;

namespace KeePassPasswordChanger.Templates
{
    [XmlInclude(typeof(ElementToClickOn))]
    [XmlInclude(typeof(ElementToLoad))]
    [XmlInclude(typeof(EventToTrigger))]
    [XmlInclude(typeof(FrameLoaded))]
    [XmlInclude(typeof(GetAttribute))]
    [XmlInclude(typeof(GetFrameNames))]
    [XmlInclude(typeof(GetStyle))]
    [XmlInclude(typeof(HasAttributeSetTo))]
    [XmlInclude(typeof(HasStyleSetTo))]
    [XmlInclude(typeof(JavascriptToExecute))]
    [XmlInclude(typeof(ResourceToLoad))]
    [XmlInclude(typeof(ReturnNode))]
    [XmlInclude(typeof(SecondsToWait))]
    [XmlInclude(typeof(SetAttribute))]
    [XmlInclude(typeof(SetStyle))]
    [XmlInclude(typeof(SiteLoaded))]
    [XmlInclude(typeof(TextToTypeIn))]
    [XmlInclude(typeof(LoadUrl))]
    [XmlInclude(typeof(Open))]
    [XmlInclude(typeof(Quit))]
    [XmlInclude(typeof(SwitchUserInputEnabling))]
    [XmlInclude(typeof(SwitchWindowVisibility))]
    [XmlInclude(typeof(PasswordCreationPolicy))]
    [XmlInclude(typeof(BrowserAction))]
    [XmlInclude(typeof(BrowserCommand))]
    public class TemplateElement : ICloneable, IInstanciateInputParameters
    {
        //Always Run CheckRequiredParameters after adding elements to this list!
        public List<string> RequiredParameters = new List<string>();

        public string Name;

        public string UEID;

        public List<KeyValuePairEx<List<Condition>, TemplateElement>> ConditionBasedAppendedTemplateElements = new List<KeyValuePairEx<List<Condition>, TemplateElement>>();

        public List<Condition> SuccessConditions = new List<Condition>();

        public TemplateElement AppendedTemplateElement = null;

        public object BrowserActionOrCommand;

        public TemplateElement()
        {
            CheckRequiredParameters();
        }

        public void NewInstance()
        {
            SuccessConditions.Add(new Condition());
        }

        public void GenerateNewUeid()
        {
            GenerateNewUeid("");
        }

        public void GenerateNewUeid(string additional)
        {
            UEID = HashingEx.Hashing.GetSha1Hash(DateTime.Now.ToString() + " " + additional + KeePassPasswordChangerExt.GeneratePassword(10, true, true, true, true, true, true, false, false).ReadString());
        }

        private void AddRequiredPlacholder(string placeholder)
        {
            if(!RequiredParameters.Contains(placeholder))
                RequiredParameters.Add(placeholder);
        }

        public void CheckRequiredParameters()
        {
            RequiredParameters.Clear();
            if (BrowserActionOrCommand == null)
                return;
            BaseObject baseObject;
            if (BrowserActionOrCommand is BrowserCommand)
                baseObject = (BaseObject)BrowserActionOrCommand;
            else
            {
                baseObject = (BaseObject)((BrowserAction)BrowserActionOrCommand).ActionObject;
            }
            foreach (var identifierToObjectKeyValuePair in baseObject.InputParameterAvailable)
            {
                if (identifierToObjectKeyValuePair.Value is InsecureText)
                {
                    foreach (var placeholder in BaseObject.ExtractAllPlaceholdersFromString(((InsecureText)identifierToObjectKeyValuePair.Value).Value))
                        AddRequiredPlacholder(placeholder);
                }
                else if (identifierToObjectKeyValuePair.Value is Selector)
                {
                    foreach (var placeholder in BaseObject.ExtractAllPlaceholdersFromString(((Selector)identifierToObjectKeyValuePair.Value).SelectorString))
                        AddRequiredPlacholder(placeholder);
                }
                else if (identifierToObjectKeyValuePair.Value is StringOrRegex)
                {
                    foreach (var placeholder in BaseObject.ExtractAllPlaceholdersFromString(((StringOrRegex)identifierToObjectKeyValuePair.Value).Value.Value))
                        AddRequiredPlacholder(placeholder);
                }
                else if (identifierToObjectKeyValuePair.Value is InsecureInt)
                {
                    
                }
                else if (identifierToObjectKeyValuePair.Value is InsecureHttpAuthSchemaType)
                {
                   
                }
                else if (identifierToObjectKeyValuePair.Value is InsecureDialogType)
                {
                    
                }
                else if (identifierToObjectKeyValuePair.Value is InsecureBool)
                {
                    
                }
                else if (identifierToObjectKeyValuePair.Value is List<object>)
                {
                    foreach (var heldObject in ((List<object>) identifierToObjectKeyValuePair.Value))
                    {
                        if (heldObject is InsecureText)
                        {
                            InsecureText insecureText = (InsecureText) heldObject;
                            foreach (var placeholder in BaseObject.ExtractAllPlaceholdersFromString(insecureText.Value))
                                AddRequiredPlacholder(placeholder);
                        }
                        if (heldObject is InsecureImage)
                        {
                            InsecureImage insecureImage = (InsecureImage)heldObject;
                            foreach (var placeholder in BaseObject.ExtractAllPlaceholdersFromString(insecureImage.Base64EncodedImage))
                                AddRequiredPlacholder(placeholder);
                        }
                    }
                }
            }
        }

        public TemplateElement(string name, object browserActionOrCommand) : this()
        {
            NewInstance();
            Name = name;

            BrowserActionOrCommand = browserActionOrCommand;
            CheckRequiredParameters();
            GenerateNewUeid(name);
        }

        public object Clone()
        {
            Type type = BrowserActionOrCommand.GetType();
            string browserOrActionCommand =
                SerializationDotNet2.Xml.Serializer.SerializeObjectToString(BrowserActionOrCommand,type);
            string appendedTemplate = "";
            if (AppendedTemplateElement != null)
                appendedTemplate = SerializationDotNet2.Xml.Serializer.SerializeObjectToString(AppendedTemplateElement, AppendedTemplateElement.GetType());
            string conditions = SerializationDotNet2.Xml.Serializer.SerializeObjectToString(SuccessConditions, SuccessConditions.GetType());
            string conditionBasedTemplateElements = SerializationDotNet2.Xml.Serializer.SerializeObjectToString(ConditionBasedAppendedTemplateElements, ConditionBasedAppendedTemplateElements.GetType());
            //return (TemplateElement) this.MemberwiseClone();
            TemplateElement newTemplateElement = (TemplateElement) this.MemberwiseClone();

            newTemplateElement.BrowserActionOrCommand = SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(browserOrActionCommand, type);

            if (appendedTemplate != "") 
                newTemplateElement.AppendedTemplateElement = (TemplateElement) SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(appendedTemplate, AppendedTemplateElement.GetType());
            newTemplateElement.SuccessConditions = (List<Condition>) SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(conditions, SuccessConditions.GetType());
            newTemplateElement.ConditionBasedAppendedTemplateElements = (List<KeyValuePairEx<List<Condition>, TemplateElement>>) SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(conditionBasedTemplateElements, ConditionBasedAppendedTemplateElements.GetType());

            //run for every templateelment in there:
            if (newTemplateElement.AppendedTemplateElement != null)
                newTemplateElement.AppendedTemplateElement = (TemplateElement) newTemplateElement.AppendedTemplateElement.Clone();

             List<KeyValuePairEx<List<Condition>, TemplateElement>> newConditionBasedAppendedTemplateElements = new List<KeyValuePairEx<List<Condition>, TemplateElement>>();
            foreach (var conditionsToTemplateElement in newTemplateElement.ConditionBasedAppendedTemplateElements)
            {
                KeyValuePairEx < List<Condition>, TemplateElement > newKeyValuePairEx = new KeyValuePairEx<List<Condition>, TemplateElement>(conditionsToTemplateElement.Key, conditionsToTemplateElement.Value);
                if (newKeyValuePairEx.Value != null)
                    newKeyValuePairEx.Value = (TemplateElement) conditionsToTemplateElement.Value.Clone();
                newConditionBasedAppendedTemplateElements.Add(newKeyValuePairEx);
            }
            newTemplateElement.ConditionBasedAppendedTemplateElements = newConditionBasedAppendedTemplateElements;

            return newTemplateElement;
        }

        public  void ReadAvailableInputParameters()
        {
            CheckRequiredParameters();
            if (BrowserActionOrCommand is BrowserCommand)
            {
                if(BrowserActionOrCommand is GetInputFromUser)
                    ((GetInputFromUser)BrowserActionOrCommand).ReadAvailableInputParameters();
                else if (BrowserActionOrCommand is LoadUrl)
                    ((LoadUrl)BrowserActionOrCommand).ReadAvailableInputParameters();
                else if (BrowserActionOrCommand is SwitchUserInputEnabling)
                    ((SwitchUserInputEnabling)BrowserActionOrCommand).ReadAvailableInputParameters();
                else if (BrowserActionOrCommand is SwitchWindowVisibility)
                    ((SwitchWindowVisibility) BrowserActionOrCommand).ReadAvailableInputParameters();
                else
                    ExceptionHandling.Handling.GetException("Unexpected",
                        new Exception("Browser Command should be found!"));
            }
            else
            {
                BrowserAction browserAction = (BrowserAction) BrowserActionOrCommand;
                //TODO read browseraction specific inputs when later wanted
                if (browserAction.ActionObject is InvokeMouseClick)
                    ((InvokeMouseClick)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is ElementToClickOn)
                    ((ElementToClickOn)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is ElementToLoad)
                    ((ElementToLoad)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is EventToTrigger)
                    ((EventToTrigger)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is FrameLoaded)
                    ((FrameLoaded)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetAttribute)
                    ((GetAttribute)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetFrameNames)
                    ((GetFrameNames)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetHttpAuth)
                    ((GetHttpAuth)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetImage)
                    ((GetImage)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetJsPrompt)
                    ((GetJsPrompt)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetStyle)
                    ((GetStyle)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is HasAttributeSetTo)
                    ((HasAttributeSetTo)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is HasStyleSetTo)
                    ((HasStyleSetTo)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is InvokeSubmit)
                    ((InvokeSubmit)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is JavascriptToExecute)
                    ((JavascriptToExecute)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is ResourceToLoad)
                    ((ResourceToLoad)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is ReturnNode)
                    ((ReturnNode)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SecondsToWait)
                    ((SecondsToWait)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SetAttribute)
                    ((SetAttribute)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SetHttpAuth)
                    ((SetHttpAuth)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SetJsPrompt)
                    ((SetJsPrompt)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SetStyle)
                    ((SetStyle)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SiteLoaded)
                    ((SiteLoaded)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is TextToTypeIn)
                    ((TextToTypeIn)browserAction.ActionObject).ReadAvailableInputParameters();
                else
                    ExceptionHandling.Handling.GetException("Unexpected",
                        new Exception("Browser Action Object should be found!"));
            }
            if(AppendedTemplateElement != null)
                AppendedTemplateElement.ReadAvailableInputParameters();
            foreach (var conditionsToTemplateElement in ConditionBasedAppendedTemplateElements)
            {
                if(conditionsToTemplateElement.Value != null)
                    conditionsToTemplateElement.Value.ReadAvailableInputParameters();
            }
        }
    }
}
