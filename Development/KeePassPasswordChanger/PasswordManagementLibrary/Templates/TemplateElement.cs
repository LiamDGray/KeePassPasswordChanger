using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CefBrowserControl;
using CefBrowserControl.BrowserActions.Elements;
using CefBrowserControl.BrowserActions.Elements.EventTypes;
using CefBrowserControl.BrowserActions.Elements.ExecJavascriptHelper;
using CefBrowserControl.BrowserActions.Helper;
using CefBrowserControl.BrowserCommands;
using CefBrowserControl.Resources;
using KeePassLib.Collections;

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
    [XmlInclude(typeof(Selector))]
    [XmlInclude(typeof(InvokeMouseClick))]
    [XmlInclude(typeof(InvokeSubmit))]
    [XmlInclude(typeof(GetInnerHtml))]
    [XmlInclude(typeof(GetInnerText))]
    [XmlInclude(typeof(SetValue))]
    [XmlInclude(typeof(SecondsToWait))]
    [XmlInclude(typeof(InvokeFullKeyboardEvent))]


    public class TemplateElement : ICloneable
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
            SetAvailableInputParameters();
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

            SetAvailableInputParameters();

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

            //BaseObject baseObjectOriginal;
            //if (BrowserActionOrCommand is BrowserCommand)
            //    baseObjectOriginal = (BaseObject)BrowserActionOrCommand;
            //else
            //{
            //    baseObjectOriginal = (BaseObject)((BrowserAction)BrowserActionOrCommand).ActionObject;
            //}
            //List<KeyValuePairEx<string, object>> inputParametersAvailable = baseObjectOriginal.InputParameterAvailable;
            //string serializedInputParametersAvailable = SerializationDotNet2.Xml.Serializer.SerializeObjectToString(inputParametersAvailable, typeof(List<KeyValuePairEx<string, object>>));
            
                
                
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

            //BaseObject baseObjectCloned;
            //if (newTemplateElement.BrowserActionOrCommand is BrowserCommand)
            //    baseObjectCloned = (BaseObject)newTemplateElement.BrowserActionOrCommand;
            //else
            //{
            //    baseObjectCloned = (BaseObject)((BrowserAction)newTemplateElement.BrowserActionOrCommand).ActionObject;
            //}
            //List<KeyValuePairEx<string, object>> deSerializedInputParametersAvailable = (List < KeyValuePairEx < string, object>> )SerializationDotNet2.Xml.Deserializer.DeserializeObjectFromString(serializedInputParametersAvailable, typeof(List<KeyValuePairEx<string, object>>));
            //baseObjectCloned.InputParameterAvailable = deSerializedInputParametersAvailable;


            return newTemplateElement;
        }

        public void SetAvailableInputParameters()
        {
            if (BrowserActionOrCommand == null)
                return;

            if (BrowserActionOrCommand is BrowserCommand)
            {
                if (BrowserActionOrCommand is GetInputFromUser)
                    ((GetInputFromUser)BrowserActionOrCommand).SetAvailableInputParameters();
                else if (BrowserActionOrCommand is LoadUrl)
                    ((LoadUrl)BrowserActionOrCommand).SetAvailableInputParameters();
                else if (BrowserActionOrCommand is SwitchUserInputEnabling)
                    ((SwitchUserInputEnabling)BrowserActionOrCommand).SetAvailableInputParameters();
                else if (BrowserActionOrCommand is SwitchWindowVisibility)
                    ((SwitchWindowVisibility)BrowserActionOrCommand).SetAvailableInputParameters();
                else
                    ExceptionHandling.Handling.GetException("Unexpected",
                        new Exception("Browser Command should be found!"));
            }
            else
            {
                BrowserAction browserAction = (BrowserAction)BrowserActionOrCommand;
                //TODO read browseraction specific inputs when later wanted
                if (browserAction.ActionObject is InvokeMouseClick && browserAction.ActionObject.GetType().Name == typeof(InvokeMouseClick).Name)
                    ((InvokeMouseClick)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is ElementToClickOn && browserAction.ActionObject.GetType().Name == typeof(ElementToClickOn).Name)
                    ((ElementToClickOn)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is ElementToLoad && browserAction.ActionObject.GetType().Name == typeof(ElementToLoad).Name)
                    ((ElementToLoad)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is EventToTrigger && browserAction.ActionObject.GetType().Name == typeof(EventToTrigger).Name)
                    ((EventToTrigger)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is FrameLoaded && browserAction.ActionObject.GetType().Name == typeof(FrameLoaded).Name)
                    ((FrameLoaded)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is GetAttribute && browserAction.ActionObject.GetType().Name == typeof(GetAttribute).Name)
                    ((GetAttribute)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is GetFrameNames && browserAction.ActionObject.GetType().Name == typeof(GetFrameNames).Name)
                    ((GetFrameNames)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is GetHttpAuth && browserAction.ActionObject.GetType().Name == typeof(GetHttpAuth).Name)
                    ((GetHttpAuth)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is GetImage && browserAction.ActionObject.GetType().Name == typeof(GetImage).Name)
                    ((GetImage)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is GetJsPrompt && browserAction.ActionObject.GetType().Name == typeof(GetJsPrompt).Name)
                    ((GetJsPrompt)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is GetStyle && browserAction.ActionObject.GetType().Name == typeof(GetStyle).Name)
                    ((GetStyle)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is HasAttributeSetTo && browserAction.ActionObject.GetType().Name == typeof(HasAttributeSetTo).Name)
                    ((HasAttributeSetTo)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is HasStyleSetTo && browserAction.ActionObject.GetType().Name == typeof(HasStyleSetTo).Name)
                    ((HasStyleSetTo)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is InvokeSubmit && browserAction.ActionObject.GetType().Name == typeof(InvokeSubmit).Name)
                    ((InvokeSubmit)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is JavascriptToExecute && browserAction.ActionObject.GetType().Name == typeof(JavascriptToExecute).Name)
                    ((JavascriptToExecute)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is ResourceToLoad && browserAction.ActionObject.GetType().Name == typeof(ResourceToLoad).Name)
                    ((ResourceToLoad)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is ReturnNode && browserAction.ActionObject.GetType().Name == typeof(ReturnNode).Name)
                    ((ReturnNode)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is SecondsToWait && browserAction.ActionObject.GetType().Name == typeof(SecondsToWait).Name)
                    ((SecondsToWait)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is SetAttribute && browserAction.ActionObject.GetType().Name == typeof(SetAttribute).Name)
                    ((SetAttribute)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is SetHttpAuth && browserAction.ActionObject.GetType().Name == typeof(SetHttpAuth).Name)
                    ((SetHttpAuth)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is SetJsPrompt && browserAction.ActionObject.GetType().Name == typeof(SetJsPrompt).Name)
                    ((SetJsPrompt)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is SetStyle && browserAction.ActionObject.GetType().Name == typeof(SetStyle).Name)
                    ((SetStyle)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is SiteLoaded && browserAction.ActionObject.GetType().Name == typeof(SiteLoaded).Name)
                    ((SiteLoaded)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is TextToTypeIn && browserAction.ActionObject.GetType().Name == typeof(TextToTypeIn).Name)
                    ((TextToTypeIn)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is GetInnerText && browserAction.ActionObject.GetType().Name == typeof(GetInnerText).Name)
                    ((GetInnerText)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is GetInnerHtml && browserAction.ActionObject.GetType().Name == typeof(GetInnerHtml).Name)
                    ((GetInnerHtml)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is SetValue && browserAction.ActionObject.GetType().Name == typeof(SetValue).Name)
                    ((SetValue)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is SecondsToWait && browserAction.ActionObject.GetType().Name == typeof(SecondsToWait).Name)
                    ((SecondsToWait)browserAction.ActionObject).SetAvailableInputParameters();
                else if (browserAction.ActionObject is InvokeFullKeyboardEvent && browserAction.ActionObject.GetType().Name == typeof(InvokeFullKeyboardEvent).Name)
                    ((InvokeFullKeyboardEvent)browserAction.ActionObject).SetAvailableInputParameters();
                else
                    ExceptionHandling.Handling.GetException("Unexpected",
                        new Exception("Browser Action Object should be found!"));
            }
            //if (AppendedTemplateElement != null)
            //    AppendedTemplateElement.SetAvailableInputParameters();
            //foreach (var conditionsToTemplateElement in ConditionBasedAppendedTemplateElements)
            //{
            //    if (conditionsToTemplateElement.Value != null)
            //        conditionsToTemplateElement.Value.SetAvailableInputParameters();
            //}
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
                if (browserAction.ActionObject is InvokeMouseClick && browserAction.ActionObject.GetType().Name == typeof(InvokeMouseClick).Name)
                    ((InvokeMouseClick)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is ElementToClickOn && browserAction.ActionObject.GetType().Name == typeof(ElementToClickOn).Name)
                    ((ElementToClickOn)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is ElementToLoad && browserAction.ActionObject.GetType().Name == typeof(ElementToLoad).Name)
                    ((ElementToLoad)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is EventToTrigger && browserAction.ActionObject.GetType().Name == typeof(EventToTrigger).Name)
                    ((EventToTrigger)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is FrameLoaded && browserAction.ActionObject.GetType().Name == typeof(FrameLoaded).Name)
                    ((FrameLoaded)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetAttribute && browserAction.ActionObject.GetType().Name == typeof(GetAttribute).Name)
                    ((GetAttribute)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetFrameNames && browserAction.ActionObject.GetType().Name == typeof(GetFrameNames).Name)
                    ((GetFrameNames)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetHttpAuth && browserAction.ActionObject.GetType().Name == typeof(GetHttpAuth).Name)
                    ((GetHttpAuth)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetImage && browserAction.ActionObject.GetType().Name == typeof(GetImage).Name)
                    ((GetImage)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetJsPrompt && browserAction.ActionObject.GetType().Name == typeof(GetJsPrompt).Name)
                    ((GetJsPrompt)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetStyle && browserAction.ActionObject.GetType().Name == typeof(GetStyle).Name)
                    ((GetStyle)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is HasAttributeSetTo && browserAction.ActionObject.GetType().Name == typeof(HasAttributeSetTo).Name)
                    ((HasAttributeSetTo)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is HasStyleSetTo && browserAction.ActionObject.GetType().Name == typeof(HasStyleSetTo).Name)
                    ((HasStyleSetTo)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is InvokeSubmit && browserAction.ActionObject.GetType().Name == typeof(InvokeSubmit).Name)
                    ((InvokeSubmit)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is JavascriptToExecute && browserAction.ActionObject.GetType().Name == typeof(JavascriptToExecute).Name)
                    ((JavascriptToExecute)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is ResourceToLoad && browserAction.ActionObject.GetType().Name == typeof(ResourceToLoad).Name)
                    ((ResourceToLoad)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is ReturnNode && browserAction.ActionObject.GetType().Name == typeof(ReturnNode).Name)
                    ((ReturnNode)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SecondsToWait && browserAction.ActionObject.GetType().Name == typeof(SecondsToWait).Name)
                    ((SecondsToWait)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SetAttribute && browserAction.ActionObject.GetType().Name == typeof(SetAttribute).Name)
                    ((SetAttribute)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SetHttpAuth && browserAction.ActionObject.GetType().Name == typeof(SetHttpAuth).Name)
                    ((SetHttpAuth)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SetJsPrompt && browserAction.ActionObject.GetType().Name == typeof(SetJsPrompt).Name)
                    ((SetJsPrompt)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SetStyle && browserAction.ActionObject.GetType().Name == typeof(SetStyle).Name)
                    ((SetStyle)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SiteLoaded && browserAction.ActionObject.GetType().Name == typeof(SiteLoaded).Name)
                    ((SiteLoaded)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is TextToTypeIn && browserAction.ActionObject.GetType().Name == typeof(TextToTypeIn).Name)
                    ((TextToTypeIn)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetInnerText && browserAction.ActionObject.GetType().Name == typeof(GetInnerText).Name)
                    ((GetInnerText)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is GetInnerHtml && browserAction.ActionObject.GetType().Name == typeof(GetInnerHtml).Name)
                    ((GetInnerHtml)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SetValue && browserAction.ActionObject.GetType().Name == typeof(SetValue).Name)
                    ((SetValue)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is SecondsToWait && browserAction.ActionObject.GetType().Name == typeof(SecondsToWait).Name)
                    ((SecondsToWait)browserAction.ActionObject).ReadAvailableInputParameters();
                else if (browserAction.ActionObject is InvokeFullKeyboardEvent && browserAction.ActionObject.GetType().Name == typeof(InvokeFullKeyboardEvent).Name)
                    ((InvokeFullKeyboardEvent)browserAction.ActionObject).ReadAvailableInputParameters();
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
