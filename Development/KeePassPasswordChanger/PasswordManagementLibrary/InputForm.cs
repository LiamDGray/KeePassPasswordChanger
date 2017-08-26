using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using CefBrowserControl;
using CefBrowserControl.BrowserCommands;
using CefBrowserControl.Resources;
using Timer = System.Timers.Timer;

namespace KeePassPasswordChanger
{
    public partial class InputForm : Form
    {
        private GetInputFromUser CommandObject;
        private const int VerticalSpace = 19;
        private const int HorizontalSpace = 6;
        private int _currentY = 19;
        Timer timer = new Timer();

        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(int hWnd);

        public InputForm()
        {
            InitializeComponent();
        }

        public InputForm(GetInputFromUser getInputFromUser) : this()
        {
            Timer timeOutTimer = new Timer();
            timeOutTimer.Interval = 100;
            timeOutTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                timeOutTimer.Stop();
                bool startAgain = true;
                if (CefBrowserControl.Timeout.ShouldBreakDueTimeout(((BaseObject)getInputFromUser)))
                {
                    try
                    {
                        this.BeginInvoke((MethodInvoker) delegate()
                        {
                            timer.Stop();
                            Close();
                        });
                        startAgain = false;
                    }
                    catch (Exception)
                    {
                        startAgain = false;
                    }
                }
                if(startAgain)
                timeOutTimer.Start();
            };
            timeOutTimer.Start();
        }

        public void PrepareForm(GetInputFromUser commandObject)
        {
            CommandObject = commandObject;

            foreach (var displayObject in CommandObject.InsecureDisplayObjects)
            {
                GroupBox groupBox = new GroupBox();
                groupBox.Text = "";
                groupBox.Location = new Point(groupBoxInput.Location.X, _currentY);
                groupBox.Width = groupBoxInput.Width;
                groupBox.TabIndex = 999;

                int neededHeight = VerticalSpace;

                if (displayObject is CefBrowserControl.Resources.InsecureText)
                {
                    CefBrowserControl.Resources.InsecureText text = (InsecureText) displayObject;

                    TextBox textBox = new TextBox();
                    textBox.Multiline = true;
                    textBox.BackColor = SystemColors.Control;
                    textBox.Text = CommandObject.TranslatePlaceholderStringToSingleString(text.Value);
                    textBox.Width = groupBox.Width - 2 * HorizontalSpace;
                    textBox.Location = new Point(HorizontalSpace, VerticalSpace);
                    textBox.AutoSize = true;
                    textBox.TabIndex = 999;
                    textBox.ReadOnly = true;
                    textBox.Height = 75;
                    textBox.ScrollBars = ScrollBars.Vertical;
                    textBox.TextAlign = HorizontalAlignment.Center;
                    neededHeight += textBox.Size.Height;

                    groupBox.Controls.Add(textBox);
                }
                else if (displayObject is CefBrowserControl.Resources.InsecureImage)
                {
                    CefBrowserControl.Resources.InsecureImage encodedImage = (InsecureImage) displayObject;

                    string base64String =
                        CommandObject.TranslatePlaceholderStringToSingleString(encodedImage.Base64EncodedImage);
                    System.Drawing.Image image = EncodingEx.Base64.Decoder.DecodeImage(base64String);
                    PictureBox pb = new PictureBox();
                    pb.Image = image;
                    pb.SizeMode = PictureBoxSizeMode.AutoSize;
                    pb.Width = groupBox.Width - 2 * HorizontalSpace;
                    pb.Location = new Point(HorizontalSpace, VerticalSpace);
                    pb.AutoSize = true;
                    pb.TabIndex = 999;
                    neededHeight += pb.Size.Height;
                    groupBox.Controls.Add(pb);

                }
                else
                {
                    ExceptionHandling.Handling.GetException("Unexpected", new Exception("Element does not exist!"));
                }

                groupBox.Height = neededHeight + VerticalSpace;
                _currentY += groupBox.Height + VerticalSpace;
                Controls.Add(groupBox);
            }
            groupBoxImage.Visible = false;
            groupBoxText.Visible = false;

            if (commandObject.InputNeeded.Value)
            {
                groupBoxInput.Location = new Point(groupBoxInput.Location.X, _currentY);
                //groupBoxInput.Width = Width - 2 * HorizontalSpace;
                _currentY += groupBoxInput.Height + VerticalSpace;
            }
            else
            {
                groupBoxInput.Visible = false;
            }
            groupBoxButtons.Location = new Point(groupBoxButtons.Location.X, _currentY);
            //groupBoxButtons.Width = Width - 2 * HorizontalSpace;
            _currentY = groupBoxButtons.Location.Y +  groupBoxButtons.Height + VerticalSpace*3;

            Height = _currentY;
            Width = Width + HorizontalSpace*3;

            timer.Interval = 100;
            timer.AutoReset = false;
            timer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                timer.Stop();
                try
                {
                    this.BeginInvoke((MethodInvoker) delegate()
                    {
                        this.TopMost = true;
                        BringToFront();
                        SetForegroundWindow(Handle.ToInt32());
                    });
                }
                catch (Exception)
                {
                    
                }
                timer.Start();
            };
            timer.Start();

            ShowDialog();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            timer.Stop();
            FinishSuccessfully();
        }

        private void FinishSuccessfully()
        {
            CommandObject.Successful = true;
            if (CommandObject.InputNeeded.Value)
            {
                CommandObject.UserInputResult = textBoxUserInput.Text;
                CommandObject.ReturnedOutput.Add(
                    new KeyValuePairEx<string, string>(GetInputFromUser.KeyList.UserInputResult.ToString(),
                        textBoxUserInput.Text));
            }
            CommandObject.Completed = true;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            timer.Stop();
            CommandObject.Completed = true;
            Close();
        }

        private void textBoxUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            timer.Stop();

            if (e.KeyCode == Keys.Enter)
                FinishSuccessfully();
        }

        private void InputForm_Load(object sender, EventArgs e)
        {
            BringToFront();
        }
    }
}
