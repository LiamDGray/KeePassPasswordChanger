namespace KeePassPasswordChanger.Templates
{
    partial class Manipulate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manipulate));
            this.checkBoxAutomaticallyGenerateAndSavePassword = new System.Windows.Forms.CheckBox();
            this.textBoxSelectorString = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxSelectorIsRegex = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxTemplateVersion = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxUTID = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.buttonEditTemplateElement = new System.Windows.Forms.Button();
            this.buttonCloneTemplateElement = new System.Windows.Forms.Button();
            this.buttonRemoveTemplateElement = new System.Windows.Forms.Button();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonAddTemplateElement = new System.Windows.Forms.Button();
            this.listBoxTemplateElements = new System.Windows.Forms.ListBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.checkBoxExcludeLookAlikes = new System.Windows.Forms.CheckBox();
            this.checkBoxNoRepeating = new System.Windows.Forms.CheckBox();
            this.checkBoxBrackets = new System.Windows.Forms.CheckBox();
            this.checkBoxPunctuation = new System.Windows.Forms.CheckBox();
            this.checkBoxAscii = new System.Windows.Forms.CheckBox();
            this.checkBoxDigits = new System.Windows.Forms.CheckBox();
            this.checkBoxUpperCase = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPasswordLength = new System.Windows.Forms.TextBox();
            this.checkBoxLowerCase = new System.Windows.Forms.CheckBox();
            this.textBoxPasswordCreationPolicy = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxAutomaticallyGenerateAndSavePassword
            // 
            this.checkBoxAutomaticallyGenerateAndSavePassword.AutoSize = true;
            this.checkBoxAutomaticallyGenerateAndSavePassword.Location = new System.Drawing.Point(6, 19);
            this.checkBoxAutomaticallyGenerateAndSavePassword.Name = "checkBoxAutomaticallyGenerateAndSavePassword";
            this.checkBoxAutomaticallyGenerateAndSavePassword.Size = new System.Drawing.Size(277, 17);
            this.checkBoxAutomaticallyGenerateAndSavePassword.TabIndex = 1;
            this.checkBoxAutomaticallyGenerateAndSavePassword.Text = "Automatically generate new password and save entry";
            this.checkBoxAutomaticallyGenerateAndSavePassword.UseVisualStyleBackColor = true;
            this.checkBoxAutomaticallyGenerateAndSavePassword.CheckedChanged += new System.EventHandler(this.checkBoxAutomaticallyGenerateAndSavePassword_CheckedChanged);
            // 
            // textBoxSelectorString
            // 
            this.textBoxSelectorString.Location = new System.Drawing.Point(79, 19);
            this.textBoxSelectorString.Name = "textBoxSelectorString";
            this.textBoxSelectorString.Size = new System.Drawing.Size(571, 20);
            this.textBoxSelectorString.TabIndex = 6;
            this.textBoxSelectorString.TextChanged += new System.EventHandler(this.textBoxSelectorString_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxSelectorIsRegex);
            this.groupBox1.Controls.Add(this.textBoxSelectorString);
            this.groupBox1.Location = new System.Drawing.Point(12, 172);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(656, 47);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Bound URI: String Or Regex";
            // 
            // checkBoxSelectorIsRegex
            // 
            this.checkBoxSelectorIsRegex.AutoSize = true;
            this.checkBoxSelectorIsRegex.Location = new System.Drawing.Point(6, 20);
            this.checkBoxSelectorIsRegex.Name = "checkBoxSelectorIsRegex";
            this.checkBoxSelectorIsRegex.Size = new System.Drawing.Size(67, 17);
            this.checkBoxSelectorIsRegex.TabIndex = 5;
            this.checkBoxSelectorIsRegex.Text = "is Regex";
            this.checkBoxSelectorIsRegex.UseVisualStyleBackColor = true;
            this.checkBoxSelectorIsRegex.CheckedChanged += new System.EventHandler(this.checkBoxSelectorIsRegex_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxTemplateVersion);
            this.groupBox2.Location = new System.Drawing.Point(12, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(656, 47);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Template Version: Int";
            // 
            // textBoxTemplateVersion
            // 
            this.textBoxTemplateVersion.Location = new System.Drawing.Point(6, 19);
            this.textBoxTemplateVersion.Name = "textBoxTemplateVersion";
            this.textBoxTemplateVersion.Size = new System.Drawing.Size(644, 20);
            this.textBoxTemplateVersion.TabIndex = 3;
            this.textBoxTemplateVersion.TextChanged += new System.EventHandler(this.textBoxTemplateVersion_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Location = new System.Drawing.Point(12, 66);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(656, 47);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Template Version: String";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(644, 20);
            this.textBox1.TabIndex = 6;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBoxUTID);
            this.groupBox4.Location = new System.Drawing.Point(12, 66);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(656, 47);
            this.groupBox4.TabIndex = 9999999;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Unique Template ID: String";
            // 
            // textBoxUTID
            // 
            this.textBoxUTID.Location = new System.Drawing.Point(6, 19);
            this.textBoxUTID.Name = "textBoxUTID";
            this.textBoxUTID.ReadOnly = true;
            this.textBoxUTID.Size = new System.Drawing.Size(644, 20);
            this.textBoxUTID.TabIndex = 999999;
            this.textBoxUTID.TextChanged += new System.EventHandler(this.textBoxUTID_TextChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxAutomaticallyGenerateAndSavePassword);
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(656, 47);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Generate new password and save when successfull : Bool";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBoxName);
            this.groupBox6.Location = new System.Drawing.Point(12, 228);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(656, 47);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Name: String";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(6, 19);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(644, 20);
            this.textBoxName.TabIndex = 8;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.buttonEditTemplateElement);
            this.groupBox7.Controls.Add(this.buttonCloneTemplateElement);
            this.groupBox7.Controls.Add(this.buttonRemoveTemplateElement);
            this.groupBox7.Controls.Add(this.buttonMoveDown);
            this.groupBox7.Controls.Add(this.buttonMoveUp);
            this.groupBox7.Controls.Add(this.buttonAddTemplateElement);
            this.groupBox7.Controls.Add(this.listBoxTemplateElements);
            this.groupBox7.Location = new System.Drawing.Point(12, 281);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(438, 348);
            this.groupBox7.TabIndex = 19;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Template Elements";
            // 
            // buttonEditTemplateElement
            // 
            this.buttonEditTemplateElement.Location = new System.Drawing.Point(325, 48);
            this.buttonEditTemplateElement.Name = "buttonEditTemplateElement";
            this.buttonEditTemplateElement.Size = new System.Drawing.Size(105, 23);
            this.buttonEditTemplateElement.TabIndex = 22;
            this.buttonEditTemplateElement.Text = "Edit element";
            this.buttonEditTemplateElement.UseVisualStyleBackColor = true;
            this.buttonEditTemplateElement.Click += new System.EventHandler(this.buttonEditTemplateElement_Click);
            // 
            // buttonCloneTemplateElement
            // 
            this.buttonCloneTemplateElement.Location = new System.Drawing.Point(325, 106);
            this.buttonCloneTemplateElement.Name = "buttonCloneTemplateElement";
            this.buttonCloneTemplateElement.Size = new System.Drawing.Size(105, 23);
            this.buttonCloneTemplateElement.TabIndex = 24;
            this.buttonCloneTemplateElement.Text = "Clone element";
            this.buttonCloneTemplateElement.UseVisualStyleBackColor = true;
            this.buttonCloneTemplateElement.Click += new System.EventHandler(this.buttonCloneTemplateElement_Click);
            // 
            // buttonRemoveTemplateElement
            // 
            this.buttonRemoveTemplateElement.Location = new System.Drawing.Point(325, 77);
            this.buttonRemoveTemplateElement.Name = "buttonRemoveTemplateElement";
            this.buttonRemoveTemplateElement.Size = new System.Drawing.Size(105, 23);
            this.buttonRemoveTemplateElement.TabIndex = 23;
            this.buttonRemoveTemplateElement.Text = "Remove element";
            this.buttonRemoveTemplateElement.UseVisualStyleBackColor = true;
            this.buttonRemoveTemplateElement.Click += new System.EventHandler(this.buttonRemoveTemplateElement_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Location = new System.Drawing.Point(325, 184);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(105, 23);
            this.buttonMoveDown.TabIndex = 26;
            this.buttonMoveDown.Text = "˅";
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Location = new System.Drawing.Point(325, 155);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(105, 23);
            this.buttonMoveUp.TabIndex = 25;
            this.buttonMoveUp.Text = "˄";
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonAddTemplateElement
            // 
            this.buttonAddTemplateElement.Location = new System.Drawing.Point(325, 19);
            this.buttonAddTemplateElement.Name = "buttonAddTemplateElement";
            this.buttonAddTemplateElement.Size = new System.Drawing.Size(105, 23);
            this.buttonAddTemplateElement.TabIndex = 21;
            this.buttonAddTemplateElement.Text = "Add element";
            this.buttonAddTemplateElement.UseVisualStyleBackColor = true;
            this.buttonAddTemplateElement.Click += new System.EventHandler(this.buttonAddTemplateElement_Click);
            // 
            // listBoxTemplateElements
            // 
            this.listBoxTemplateElements.FormattingEnabled = true;
            this.listBoxTemplateElements.Location = new System.Drawing.Point(6, 19);
            this.listBoxTemplateElements.Name = "listBoxTemplateElements";
            this.listBoxTemplateElements.Size = new System.Drawing.Size(313, 316);
            this.listBoxTemplateElements.TabIndex = 20;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.textBoxPasswordCreationPolicy);
            this.groupBox8.Controls.Add(this.checkBoxExcludeLookAlikes);
            this.groupBox8.Controls.Add(this.checkBoxNoRepeating);
            this.groupBox8.Controls.Add(this.checkBoxBrackets);
            this.groupBox8.Controls.Add(this.checkBoxPunctuation);
            this.groupBox8.Controls.Add(this.checkBoxAscii);
            this.groupBox8.Controls.Add(this.checkBoxDigits);
            this.groupBox8.Controls.Add(this.checkBoxUpperCase);
            this.groupBox8.Controls.Add(this.label1);
            this.groupBox8.Controls.Add(this.textBoxPasswordLength);
            this.groupBox8.Controls.Add(this.checkBoxLowerCase);
            this.groupBox8.Location = new System.Drawing.Point(456, 281);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(212, 348);
            this.groupBox8.TabIndex = 9;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Password Creation Policy";
            // 
            // checkBoxExcludeLookAlikes
            // 
            this.checkBoxExcludeLookAlikes.AutoSize = true;
            this.checkBoxExcludeLookAlikes.Location = new System.Drawing.Point(52, 206);
            this.checkBoxExcludeLookAlikes.Name = "checkBoxExcludeLookAlikes";
            this.checkBoxExcludeLookAlikes.Size = new System.Drawing.Size(122, 17);
            this.checkBoxExcludeLookAlikes.TabIndex = 18;
            this.checkBoxExcludeLookAlikes.Text = "Exclude Look Alikes";
            this.checkBoxExcludeLookAlikes.UseVisualStyleBackColor = true;
            this.checkBoxExcludeLookAlikes.CheckedChanged += new System.EventHandler(this.checkBoxExcludeLookAlikes_CheckedChanged);
            // 
            // checkBoxNoRepeating
            // 
            this.checkBoxNoRepeating.AutoSize = true;
            this.checkBoxNoRepeating.Location = new System.Drawing.Point(52, 183);
            this.checkBoxNoRepeating.Name = "checkBoxNoRepeating";
            this.checkBoxNoRepeating.Size = new System.Drawing.Size(146, 17);
            this.checkBoxNoRepeating.TabIndex = 17;
            this.checkBoxNoRepeating.Text = "No Repeating Characters";
            this.checkBoxNoRepeating.UseVisualStyleBackColor = true;
            this.checkBoxNoRepeating.CheckedChanged += new System.EventHandler(this.checkBoxNoRepeating_CheckedChanged);
            // 
            // checkBoxBrackets
            // 
            this.checkBoxBrackets.AutoSize = true;
            this.checkBoxBrackets.Location = new System.Drawing.Point(52, 160);
            this.checkBoxBrackets.Name = "checkBoxBrackets";
            this.checkBoxBrackets.Size = new System.Drawing.Size(106, 17);
            this.checkBoxBrackets.TabIndex = 16;
            this.checkBoxBrackets.Text = "Include Brackets";
            this.checkBoxBrackets.UseVisualStyleBackColor = true;
            this.checkBoxBrackets.CheckedChanged += new System.EventHandler(this.checkBoxBrackets_CheckedChanged);
            // 
            // checkBoxPunctuation
            // 
            this.checkBoxPunctuation.AutoSize = true;
            this.checkBoxPunctuation.Location = new System.Drawing.Point(52, 137);
            this.checkBoxPunctuation.Name = "checkBoxPunctuation";
            this.checkBoxPunctuation.Size = new System.Drawing.Size(121, 17);
            this.checkBoxPunctuation.TabIndex = 15;
            this.checkBoxPunctuation.Text = "Include Punctuation";
            this.checkBoxPunctuation.UseVisualStyleBackColor = true;
            this.checkBoxPunctuation.CheckedChanged += new System.EventHandler(this.checkBoxPunctuation_CheckedChanged);
            // 
            // checkBoxAscii
            // 
            this.checkBoxAscii.AutoSize = true;
            this.checkBoxAscii.Location = new System.Drawing.Point(52, 114);
            this.checkBoxAscii.Name = "checkBoxAscii";
            this.checkBoxAscii.Size = new System.Drawing.Size(129, 17);
            this.checkBoxAscii.TabIndex = 14;
            this.checkBoxAscii.Text = "Include Special ASCII";
            this.checkBoxAscii.UseVisualStyleBackColor = true;
            this.checkBoxAscii.CheckedChanged += new System.EventHandler(this.checkBoxAscii_CheckedChanged);
            // 
            // checkBoxDigits
            // 
            this.checkBoxDigits.AutoSize = true;
            this.checkBoxDigits.Location = new System.Drawing.Point(52, 91);
            this.checkBoxDigits.Name = "checkBoxDigits";
            this.checkBoxDigits.Size = new System.Drawing.Size(90, 17);
            this.checkBoxDigits.TabIndex = 13;
            this.checkBoxDigits.Text = "Include Digits";
            this.checkBoxDigits.UseVisualStyleBackColor = true;
            this.checkBoxDigits.CheckedChanged += new System.EventHandler(this.checkBoxDigits_CheckedChanged);
            // 
            // checkBoxUpperCase
            // 
            this.checkBoxUpperCase.AutoSize = true;
            this.checkBoxUpperCase.Location = new System.Drawing.Point(52, 68);
            this.checkBoxUpperCase.Name = "checkBoxUpperCase";
            this.checkBoxUpperCase.Size = new System.Drawing.Size(116, 17);
            this.checkBoxUpperCase.TabIndex = 12;
            this.checkBoxUpperCase.Text = "Include Uppercase";
            this.checkBoxUpperCase.UseVisualStyleBackColor = true;
            this.checkBoxUpperCase.CheckedChanged += new System.EventHandler(this.checkBoxUpperCase_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Length";
            // 
            // textBoxPasswordLength
            // 
            this.textBoxPasswordLength.Location = new System.Drawing.Point(52, 19);
            this.textBoxPasswordLength.Name = "textBoxPasswordLength";
            this.textBoxPasswordLength.Size = new System.Drawing.Size(154, 20);
            this.textBoxPasswordLength.TabIndex = 10;
            this.textBoxPasswordLength.TextChanged += new System.EventHandler(this.textBoxPasswordLength_TextChanged);
            // 
            // checkBoxLowerCase
            // 
            this.checkBoxLowerCase.AutoSize = true;
            this.checkBoxLowerCase.Location = new System.Drawing.Point(52, 45);
            this.checkBoxLowerCase.Name = "checkBoxLowerCase";
            this.checkBoxLowerCase.Size = new System.Drawing.Size(116, 17);
            this.checkBoxLowerCase.TabIndex = 11;
            this.checkBoxLowerCase.Text = "Include Lowercase";
            this.checkBoxLowerCase.UseVisualStyleBackColor = true;
            this.checkBoxLowerCase.CheckedChanged += new System.EventHandler(this.checkBoxLowerCase_CheckedChanged);
            // 
            // textBoxPasswordCreationPolicy
            // 
            this.textBoxPasswordCreationPolicy.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxPasswordCreationPolicy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPasswordCreationPolicy.Location = new System.Drawing.Point(6, 229);
            this.textBoxPasswordCreationPolicy.Multiline = true;
            this.textBoxPasswordCreationPolicy.Name = "textBoxPasswordCreationPolicy";
            this.textBoxPasswordCreationPolicy.ReadOnly = true;
            this.textBoxPasswordCreationPolicy.Size = new System.Drawing.Size(200, 113);
            this.textBoxPasswordCreationPolicy.TabIndex = 19;
            this.textBoxPasswordCreationPolicy.Text = resources.GetString("textBoxPasswordCreationPolicy.Text");
            // 
            // Manipulate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 641);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Manipulate";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Template: ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Manipulate_FormClosing);
            this.Load += new System.EventHandler(this.Manipulate_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxAutomaticallyGenerateAndSavePassword;
        private System.Windows.Forms.TextBox textBoxSelectorString;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxSelectorIsRegex;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxTemplateVersion;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBoxUTID;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button buttonCloneTemplateElement;
        private System.Windows.Forms.Button buttonRemoveTemplateElement;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonAddTemplateElement;
        private System.Windows.Forms.ListBox listBoxTemplateElements;
        private System.Windows.Forms.Button buttonEditTemplateElement;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPasswordLength;
        private System.Windows.Forms.CheckBox checkBoxLowerCase;
        private System.Windows.Forms.CheckBox checkBoxDigits;
        private System.Windows.Forms.CheckBox checkBoxUpperCase;
        private System.Windows.Forms.CheckBox checkBoxAscii;
        private System.Windows.Forms.CheckBox checkBoxExcludeLookAlikes;
        private System.Windows.Forms.CheckBox checkBoxNoRepeating;
        private System.Windows.Forms.CheckBox checkBoxBrackets;
        private System.Windows.Forms.CheckBox checkBoxPunctuation;
        private System.Windows.Forms.TextBox textBoxPasswordCreationPolicy;
    }
}