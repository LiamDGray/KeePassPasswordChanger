using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CefBrowserControl;
using CefBrowserControl.BrowserActions.Elements;
using CefBrowserControl.BrowserActions.Elements.EventTypes;
using CefBrowserControl.BrowserActions.Elements.ExecJavascriptHelper;
using CefBrowserControl.BrowserCommands;

namespace KeePassPasswordChanger.Templates
{
    public partial class NewTemplateElement : Form
    {
        public TemplateElement TemplateElement;

        private List<object> availableBaseObjects = new List<object>();

        public NewTemplateElement()
        {
            InitializeComponent();

            LoadUrl loadUrl = new LoadUrl();
            loadUrl.NewInstance();
            availableBaseObjects.Add(loadUrl);
            GetInputFromUser getInputFromUser = new GetInputFromUser();
            getInputFromUser.NewInstance();
            availableBaseObjects.Add(getInputFromUser);
            SwitchWindowVisibility switchWindowVisibility = new SwitchWindowVisibility();
            switchWindowVisibility.NewInstance();
            availableBaseObjects.Add(switchWindowVisibility);
            ElementToLoad elementToLoad = new ElementToLoad();
            elementToLoad.NewInstance();
            availableBaseObjects.Add(elementToLoad);
            EventToTrigger eventToTrigger = new EventToTrigger();
            eventToTrigger.NewInstance();
            availableBaseObjects.Add(eventToTrigger);
            GetAttribute getAttribute = new GetAttribute();
            getAttribute.NewInstance();
            availableBaseObjects.Add(getAttribute);
            GetHttpAuth getHttpAuth = new GetHttpAuth();
            getHttpAuth.NewInstance();
            availableBaseObjects.Add(getHttpAuth);
            GetImage getImage = new GetImage();
            getImage.NewInstance();
            availableBaseObjects.Add(getImage);
            GetJsPrompt getJsPrompt = new GetJsPrompt();
            getJsPrompt.NewInstance();
            availableBaseObjects.Add(getJsPrompt);
            GetStyle getStyle = new GetStyle();
            getStyle.NewInstance();
            availableBaseObjects.Add(getStyle);
            InvokeSubmit invokeSubmit= new InvokeSubmit();
            invokeSubmit.NewInstance();
            availableBaseObjects.Add(invokeSubmit);
            InvokeMouseClick invokeMouseClick = new InvokeMouseClick();
            invokeMouseClick.NewInstance();
            availableBaseObjects.Add(invokeMouseClick);
            JavascriptToExecute javascriptToExecute = new JavascriptToExecute();
            javascriptToExecute.NewInstance();
            availableBaseObjects.Add(javascriptToExecute);
            ResourceToLoad resourceToLoad = new ResourceToLoad();
            resourceToLoad.NewInstance();
            availableBaseObjects.Add(resourceToLoad);
            SetAttribute setAttribute = new SetAttribute();
            setAttribute.NewInstance();
            availableBaseObjects.Add(setAttribute);
            SetHttpAuth setHttpAuth = new SetHttpAuth();
            setHttpAuth.NewInstance();
            availableBaseObjects.Add(setHttpAuth);
            SetJsPrompt setJsPrompt =  new SetJsPrompt();
            setJsPrompt.NewInstance();
            availableBaseObjects.Add(setJsPrompt);
            SetStyle setStyle = new SetStyle();
            setStyle.NewInstance();
            availableBaseObjects.Add(setStyle);
            SiteLoaded siteLoaded = new SiteLoaded();
            siteLoaded.NewInstance();
            availableBaseObjects.Add(siteLoaded);
            GetInnerText getInnerText = new GetInnerText();
            getInnerText.NewInstance();
            availableBaseObjects.Add(getInnerText);
            GetInnerHtml getInnerHtml = new GetInnerHtml();
            getInnerHtml.NewInstance();
            availableBaseObjects.Add(getInnerHtml);
            SetValue setValue = new SetValue();
            setValue.NewInstance();
            availableBaseObjects.Add(setValue);
            SecondsToWait secondsToWait = new SecondsToWait();
            secondsToWait.NewInstance();
            availableBaseObjects.Add(secondsToWait);
            InvokeFullKeyboardEvent invokeFullKeyBoardEvent = new InvokeFullKeyboardEvent();
            invokeFullKeyBoardEvent.NewInstance();
            availableBaseObjects.Add(invokeFullKeyBoardEvent);
        }

        private void NewTemplateElement_Load(object sender, EventArgs e)
        {
            foreach (var baseObject in availableBaseObjects)
            {
                comboBox1.Items.Add(((BaseObject)baseObject).GetType().Name);
            }
            comboBox1.Sorted = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           UpdateElement();
        }

        private void UpdateElement()
        {
            string text = "";
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (comboBox1.SelectedIndex == i)
                {
                    text = comboBox1.Items[i].ToString();
                }
            }
            if (text == "")
                return;
            foreach (var baseObject in availableBaseObjects)
            {
                if (((BaseObject)baseObject).GetType().Name == text)
                {
                    if (baseObject is BrowserCommand)
                        TemplateElement = new TemplateElement(textBoxName.Text, baseObject);
                    else
                    {
                        BrowserAction browserAction = new BrowserAction();
                        browserAction.ActionObject = baseObject;
                        TemplateElement = new TemplateElement(textBoxName.Text, browserAction);
                    }
                    textBoxDescription.Text = ((BaseObject)baseObject).Description;
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            TemplateElement = null;
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            UpdateElement();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                TemplateElement = null;
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
