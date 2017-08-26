using System;

namespace KeePassPasswordChanger
{
    public class Options
    {
        public const int LockTimeOut = 100;
        public static TimeSpan DefaultCefBrowserActionOrCommandTimeoutMsec = new TimeSpan(0, 0, 0, 30);
        //Keepass DB X Password Changer Exported Template
        public const string ExportExtension = ".kdbxpcet";
        public const string PublicTemplatesUrl = "https://github.com/phi-el/keepass-password-changer/tree/master/Public%20Templates";
        public const string UpdateUrl = "https://raw.githubusercontent.com/phi-el/keepass-password-changer/master/Build/keepass-password-changer-version.txt";
    }
}
