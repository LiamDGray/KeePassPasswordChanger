using System;
using System.Windows.Forms;
//https://msdn.microsoft.com/en-us/library/system.windows.forms.datetimepicker.customformat(v=vs.110).aspx

namespace KeePassPasswordChanger
{
    public partial class DateTimePicker : Form
    {
        public bool Success;

        public DateTimePicker()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Success = true;
            Close();
        }
    }
}
