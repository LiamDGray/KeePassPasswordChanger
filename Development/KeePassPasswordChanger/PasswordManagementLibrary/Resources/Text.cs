using System;
using CefBrowserControl.Resources;
using KeePassLib.Security;

namespace KeePassPasswordChanger.Resources
{
    [Serializable]
    public class Text : Resource
    {
        public ProtectedString Value;

        public Text() : base()
        {

        }

        public Text(ProtectedString value)
        {
            Value = value;
        }
    }
}
