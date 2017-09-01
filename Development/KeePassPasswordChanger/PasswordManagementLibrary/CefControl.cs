//https://stackoverflow.com/questions/983030/type-checking-typeof-gettype-or-is#983061

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Timers;
using CefBrowserControl;
using CefBrowserControl.BrowserCommands;
using CefBrowserControl.Conversion;
using Timer = System.Timers.Timer;

namespace KeePassPasswordChanger
{
    public class CefControl
    {

        private Timer processingTimer;
        private int intervalProcessingTimer = 100;

        public static ReaderWriterLock BrowserListsLock = new ReaderWriterLock(),
            UcidToUidRedirectLock = new ReaderWriterLock();

        public static Dictionary<string, object> BrowserCommands = new Dictionary<string, object>();
        public static Dictionary<string, object> BrowserActions = new Dictionary<string, object>();
        public static Dictionary<string, object> BrowserCommandsInTransit = new Dictionary<string, object>();
        public static Dictionary<string, object> BrowserActionsInTransit = new Dictionary<string, object>();
        public static Dictionary<string, object> BrowserCommandsCompleted = new Dictionary<string, object>();
        public static Dictionary<string, object> BrowserActionsCompleted = new Dictionary<string, object>();

        public static ReaderWriterLock MessagesLock = new ReaderWriterLock();
        public static List<KeyValuePairEx<string, string>> PendingMessagesList = new List<KeyValuePairEx<string, string>>();
        public static List<string> ReceivedMessagesList = new List<string>();


        private  RpcReaderWriter _rpcReaderWriter;
        private  Thread rpcReaderThread;
        public static string RcpServerName = "CefBAAS-1";


        public CefControl()
        {
            if (_rpcReaderWriter == null)
            {
                _rpcReaderWriter = new RpcReaderWriter(PendingMessagesList, ReceivedMessagesList, MessagesLock, RcpServerName);
                rpcReaderThread = new Thread(new ThreadStart(_rpcReaderWriter.Listen));
                rpcReaderThread.Start();
            }

            //StartRpc();

            if (processingTimer == null)
            {
                processingTimer = new Timer();
                processingTimer.Interval = intervalProcessingTimer;
                processingTimer.Elapsed += ProcessingTimer_Elapsed;
                processingTimer.Start();
            }
        }

        public void KillRpc(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            foreach (var process in processes)
            {
                process.Kill();
            }
            rpcReaderThread.Abort();
        }

        public static Dictionary<string, Process> CefBrowserSessions = new Dictionary<string, Process>();

        public static void ClearLists()
        {
            while (true)
            {
                try
                {
                    BrowserListsLock.AcquireWriterLock(Options.LockTimeOut);

                    BrowserCommands.Clear();
                    BrowserActions.Clear();
                    BrowserCommandsInTransit.Clear();
                    BrowserActionsInTransit.Clear();
                    BrowserCommandsCompleted.Clear();
                    BrowserActionsCompleted.Clear();
                    break;
                }
                catch (ApplicationException ex)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
                }
                finally
                {
                    if (BrowserListsLock.IsWriterLockHeld)
                        BrowserListsLock.ReleaseWriterLock();
                }
                Thread.Sleep(100);
            }
        }

        private void ProcessingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            processingTimer.Stop();
            try
            {
                MessagesLock.AcquireWriterLock(Options.LockTimeOut);
                try
                {
                    if (ReceivedMessagesList.Count > 0)
                    {
                        try
                        {
                            BrowserListsLock.AcquireWriterLock(Options.LockTimeOut);
                            try
                            {
                                foreach (string message in ReceivedMessagesList)
                                {
                                    //DEBUG
                                    //Console.WriteLine(message);
                                    string plain = EncodingEx.Base64.Decoder.DecodeString(Encoding.UTF8, message);
                                    try
                                    {
                                        CefDecodeResult cefDecodeResult = CefDecoding.Decode(plain);
                                        if (cefDecodeResult.DecodedObject is BrowserAction)
                                        {
                                            BrowserAction browserAction = (BrowserAction) cefDecodeResult.DecodedObject;
                                            browserAction.ExecuteEventHandler = true;
                                            browserAction.SetFinished(true);

                                            BrowserActionsInTransit.Remove(cefDecodeResult.UCID);
                                            BrowserActionsCompleted.Add(cefDecodeResult.UCID, cefDecodeResult.DecodedObject);
                                        }
                                        else if (cefDecodeResult.DecodedObject is BrowserCommand)
                                        {
                                            BrowserCommand browerCommand = (BrowserCommand)cefDecodeResult.DecodedObject;
                                            browerCommand.ExecuteEventHandler = true;
                                            browerCommand.SetCompleted(true);

                                            BrowserCommandsInTransit.Remove(cefDecodeResult.UCID);
                                            BrowserCommandsCompleted.Add(cefDecodeResult.UCID, cefDecodeResult.DecodedObject);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandling.Handling.GetException("Unexpected", ex);

                                    }
                                }
                                ReceivedMessagesList.Clear();
                            }
                            catch (Exception ex)
                            {
                                ExceptionHandling.Handling.GetException("Unexpected", ex);

                            }
                        }
                        catch (ApplicationException ex1)
                        {
                            ExceptionHandling.Handling.GetException("ReaderWriterLock", ex1);
                        }
                        finally
                        {
                            if (BrowserListsLock.IsWriterLockHeld)
                                BrowserListsLock.ReleaseWriterLock();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandling.Handling.GetException("Unexpected", ex);

                }
            }
            catch (ApplicationException ex1)
            {
                ExceptionHandling.Handling.GetException("ReaderWriterLock", ex1);
            }
            finally
            {
                if (MessagesLock.IsWriterLockHeld)
                    MessagesLock.ReleaseWriterLock();
            }
            try
            {
                BrowserListsLock.AcquireWriterLock(Options.LockTimeOut);
                if (BrowserActions.Count > 0 || BrowserCommands.Count > 0)
                {
                    try
                    {
                        MessagesLock.AcquireWriterLock(Options.LockTimeOut);
                        foreach (var browserCommandKeyValuePair in BrowserCommands)
                        {
                            try
                            {
                                string commandType = browserCommandKeyValuePair.Value.GetType().ToString();
                                //Console.WriteLine(commandType);
                                switch (commandType)
                                {
                                    //Pass to application, or ucid will be very wrong...
                                    case "CefBrowserControl.BrowserCommands.Open":
                                        Open open = (Open) browserCommandKeyValuePair.Value;
                                        if (!CefBrowserSessions.ContainsKey(open.UID))
                                        {
                                            ////Todo: Clean Shutdown
                                            Process p = Process.Start("CefBrowser.exe", open.UID + (CefBrowserControl.Options.IsDebug?" --debug":""));
                                            CefBrowserSessions.Add(open.UID, p);
                                            //TODO: Add code that checks thread
                                        }
                                                _rpcReaderWriter.AddClient(open.UID);
                                        //BrowserCommandsCompleted.Add(open.UCID, open);
                                        goto default;
                                    case "CefBrowserControl.BrowserCommands.Quit":
                                        Quit quit = (Quit) browserCommandKeyValuePair.Value;
                                        if (quit.All)
                                        {
                                            foreach (var cefBrowserSession in CefBrowserSessions)
                                            {
                                                Process process = cefBrowserSession.Value;
                                                if (process != null)
                                                    process.Kill();
                                                        _rpcReaderWriter.RemoveClient(quit.UID);
                                                
                                            }
                                            quit.Successful = true;
                                            CefBrowserSessions.Clear();
                                        }
                                        else
                                        {
                                            Process process = CefBrowserSessions[quit.UID];
                                            if (process != null)
                                                process.Kill();
                                            quit.Successful = true;
                                            CefBrowserSessions.Remove(quit.UID);
                                                    _rpcReaderWriter.RemoveClient(quit.UID);
                                        }
                                        quit.Completed = true;
                                        BrowserCommandsCompleted.Add(quit.UCID, quit);
                                        break;
                                    default:
                                        BrowserCommand cmd = (BrowserCommand)browserCommandKeyValuePair.Value;
                                        PendingMessagesList.Add(new KeyValuePairEx<string, string>(cmd.UID, CefEncoding.Encode(cmd.UCID, cmd)));
                                        BrowserCommandsInTransit.Add(cmd.UCID, cmd);
                                        break;
                                }

                                
                            }
                            catch (Exception ex)
                            {
                                ExceptionHandling.Handling.GetException("Unexpected", ex);

                            }
                        }
                        foreach (var browserActionKeyValuePair in BrowserActions)
                        {
                            try
                            {
                                BrowserAction browserAction = (BrowserAction)browserActionKeyValuePair.Value;
                                PendingMessagesList.Add(new KeyValuePairEx<string, string>(browserAction.UID, CefEncoding.Encode(browserAction.UCID, browserActionKeyValuePair.Value)));
                                BrowserCommandsInTransit.Add(browserAction.UCID, browserAction);
                                //PendingMessagesList.Add("CefBAAS-1", CefEncoding.EncodeString(browserActionKeyValuePair.Key, browserActionKeyValuePair.ExpectedValue));
                                //BrowserActionsInTransit.Add(browserActionKeyValuePair.Key, browserActionKeyValuePair.ExpectedValue);
                            }
                            catch (Exception ex)
                            {
                                ExceptionHandling.Handling.GetException("Unexpected", ex);

                            }
                        }
                        BrowserCommands.Clear();
                        BrowserActions.Clear();
                    }
                    catch (ApplicationException ex2)
                    {
                        ExceptionHandling.Handling.GetException("ReaderWriterLock", ex2);
                    }
                    finally
                    {
                        if (MessagesLock.IsWriterLockHeld)
                            MessagesLock.ReleaseWriterLock();
                    }
                }
            }
            catch (ApplicationException ex1)
            {
                ExceptionHandling.Handling.GetException("ReaderWriterLock", ex1);

            }
            finally
            {
                if (BrowserListsLock.IsWriterLockHeld)
                    BrowserListsLock.ReleaseWriterLock();
            }
            processingTimer.Start();
        }

        //public void AddUcidToUidEntry(string ucid, string uid)
        //{
        //    while(true)
        //    {
        //        try
        //        {
        //            UcidToUidRedirectLock.AcquireWriterLock(Options.LockTimeOut);
        //            UcidToUidSetDictionary.Add(ucid, uid);
        //            break;
        //        }
        //        catch (ApplicationException ex)
        //        {
        //            ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
        //        }
        //        finally
        //        {
        //            if(UcidToUidRedirectLock.IsWriterLockHeld)
        //                UcidToUidRedirectLock.ReleaseWriterLock();
        //        }
        //        Thread.Sleep(100);
        //    }
        //}

        //public void RemoveUcidToUidEntry(string ucid)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            UcidToUidRedirectLock.AcquireWriterLock(Options.LockTimeOut);
        //            UcidToUidSetDictionary.Remove(ucid);
        //            break;
        //        }
        //        catch (ApplicationException ex)
        //        {
        //            ExceptionHandling.Handling.GetException("ReaderWriterLock", ex);
        //        }
        //        finally
        //        {
        //            if (UcidToUidRedirectLock.IsWriterLockHeld)
        //                UcidToUidRedirectLock.ReleaseWriterLock();
        //        }
        //        Thread.Sleep(100);
        //    }
        //}

        public void AddCefBrowserCommand(object browserCommand)
        {
            if (browserCommand == null)
                return;
            while (true)
            {
                try
                {
                    BrowserListsLock.AcquireWriterLock(Options.LockTimeOut);
                    try
                    {
                        BrowserCommands.Add(((BrowserCommand) browserCommand).UCID, browserCommand);
                        //string serialized = SerializiationDotNet2.Xml.Serializer.SerializeObjectToString(
                        //    browserCommand, browserCommand.GetType());
                        //string encoded = EncodingEx.Base64.Encoder.EncodeString(CefEncoding.UTF8, serialized);
                        break;

                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.Handling.GetException("Unexpected", ex);

                    }
                }
                catch (ApplicationException ex1)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex1);

                }
                finally
                {
                    if (BrowserListsLock.IsWriterLockHeld)
                        BrowserListsLock.ReleaseWriterLock();
                }
            }

        }

        public void AddCefBrowserAction(object browseraction)
        {
            if (browseraction == null)
                return;
            while (true)
            {
                try
                {
                    BrowserListsLock.AcquireWriterLock(Options.LockTimeOut);
                    try
                    {
                        BrowserActions.Add(((BrowserAction) browseraction).UCID, browseraction);
                        break;

                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.Handling.GetException("Unexpected", ex);

                    }
                }
                catch (ApplicationException ex1)
                {
                    ExceptionHandling.Handling.GetException("ReaderWriterLock", ex1);
                }
                finally
                {
                    if (BrowserListsLock.IsWriterLockHeld)
                        BrowserListsLock.ReleaseWriterLock();
                }
            }

        }
    }
}
