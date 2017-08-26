namespace KeePassPasswordChanger
{
    partial class Working
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Working));
            this.labelDescription = new System.Windows.Forms.Label();
            this.listBoxRemainingTemplates = new System.Windows.Forms.ListBox();
            this.groupBoxQueuedTemplates = new System.Windows.Forms.GroupBox();
            this.buttonEnqueueEntry = new System.Windows.Forms.Button();
            this.buttonRemovedEntry = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxRemovedTemplates = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonStartChange = new System.Windows.Forms.Button();
            this.groupBoxActiveTemplates = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.listBoxActiveTemplates = new System.Windows.Forms.ListBox();
            this.groupBoxCompleted = new System.Windows.Forms.GroupBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.listBoxTemplatesCompleted = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.listBoxFailedTemplates = new System.Windows.Forms.ListBox();
            this.groupBoxQueuedTemplates.SuspendLayout();
            this.groupBoxActiveTemplates.SuspendLayout();
            this.groupBoxCompleted.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.Location = new System.Drawing.Point(12, 9);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(359, 29);
            this.labelDescription.TabIndex = 0;
            this.labelDescription.Text = "Press start when you are ready...";
            // 
            // listBoxRemainingTemplates
            // 
            this.listBoxRemainingTemplates.FormattingEnabled = true;
            this.listBoxRemainingTemplates.Location = new System.Drawing.Point(6, 84);
            this.listBoxRemainingTemplates.Name = "listBoxRemainingTemplates";
            this.listBoxRemainingTemplates.ScrollAlwaysVisible = true;
            this.listBoxRemainingTemplates.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxRemainingTemplates.Size = new System.Drawing.Size(344, 238);
            this.listBoxRemainingTemplates.TabIndex = 3;
            this.listBoxRemainingTemplates.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxRemainingTemplates_MouseDoubleClick);
            // 
            // groupBoxQueuedTemplates
            // 
            this.groupBoxQueuedTemplates.Controls.Add(this.buttonEnqueueEntry);
            this.groupBoxQueuedTemplates.Controls.Add(this.buttonRemovedEntry);
            this.groupBoxQueuedTemplates.Controls.Add(this.label3);
            this.groupBoxQueuedTemplates.Controls.Add(this.label2);
            this.groupBoxQueuedTemplates.Controls.Add(this.listBoxRemovedTemplates);
            this.groupBoxQueuedTemplates.Controls.Add(this.textBox1);
            this.groupBoxQueuedTemplates.Controls.Add(this.buttonStartChange);
            this.groupBoxQueuedTemplates.Controls.Add(this.listBoxRemainingTemplates);
            this.groupBoxQueuedTemplates.Location = new System.Drawing.Point(17, 56);
            this.groupBoxQueuedTemplates.Name = "groupBoxQueuedTemplates";
            this.groupBoxQueuedTemplates.Size = new System.Drawing.Size(358, 536);
            this.groupBoxQueuedTemplates.TabIndex = 5;
            this.groupBoxQueuedTemplates.TabStop = false;
            this.groupBoxQueuedTemplates.Text = "Waiting queue: remaining password changes";
            // 
            // buttonEnqueueEntry
            // 
            this.buttonEnqueueEntry.Location = new System.Drawing.Point(6, 476);
            this.buttonEnqueueEntry.Name = "buttonEnqueueEntry";
            this.buttonEnqueueEntry.Size = new System.Drawing.Size(344, 23);
            this.buttonEnqueueEntry.TabIndex = 14;
            this.buttonEnqueueEntry.Text = "Enqueue Entry again";
            this.buttonEnqueueEntry.UseVisualStyleBackColor = true;
            this.buttonEnqueueEntry.Click += new System.EventHandler(this.buttonEnqueueEntry_Click);
            // 
            // buttonRemovedEntry
            // 
            this.buttonRemovedEntry.Location = new System.Drawing.Point(6, 328);
            this.buttonRemovedEntry.Name = "buttonRemovedEntry";
            this.buttonRemovedEntry.Size = new System.Drawing.Size(344, 23);
            this.buttonRemovedEntry.TabIndex = 13;
            this.buttonRemovedEntry.Text = "Dequeue Entry from Queue";
            this.buttonRemovedEntry.UseVisualStyleBackColor = true;
            this.buttonRemovedEntry.Click += new System.EventHandler(this.buttonRemovedEntry_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Enqueued password entries:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 359);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Dequeued password entries:";
            // 
            // listBoxRemovedTemplates
            // 
            this.listBoxRemovedTemplates.FormattingEnabled = true;
            this.listBoxRemovedTemplates.Location = new System.Drawing.Point(6, 375);
            this.listBoxRemovedTemplates.Name = "listBoxRemovedTemplates";
            this.listBoxRemovedTemplates.ScrollAlwaysVisible = true;
            this.listBoxRemovedTemplates.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxRemovedTemplates.Size = new System.Drawing.Size(344, 95);
            this.listBoxRemovedTemplates.TabIndex = 10;
            this.listBoxRemovedTemplates.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxRemovedTemplates_MouseDoubleClick);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(6, 19);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(344, 46);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // buttonStartChange
            // 
            this.buttonStartChange.Enabled = false;
            this.buttonStartChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartChange.Location = new System.Drawing.Point(6, 507);
            this.buttonStartChange.Name = "buttonStartChange";
            this.buttonStartChange.Size = new System.Drawing.Size(344, 23);
            this.buttonStartChange.TabIndex = 8;
            this.buttonStartChange.Text = "Start";
            this.buttonStartChange.UseVisualStyleBackColor = true;
            this.buttonStartChange.Click += new System.EventHandler(this.buttonStartChange_Click);
            // 
            // groupBoxActiveTemplates
            // 
            this.groupBoxActiveTemplates.Controls.Add(this.textBox3);
            this.groupBoxActiveTemplates.Controls.Add(this.listBoxActiveTemplates);
            this.groupBoxActiveTemplates.Location = new System.Drawing.Point(381, 56);
            this.groupBoxActiveTemplates.Name = "groupBoxActiveTemplates";
            this.groupBoxActiveTemplates.Size = new System.Drawing.Size(726, 213);
            this.groupBoxActiveTemplates.TabIndex = 7;
            this.groupBoxActiveTemplates.TabStop = false;
            this.groupBoxActiveTemplates.Text = "Active password changes";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.Control;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Location = new System.Drawing.Point(6, 19);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(710, 20);
            this.textBox3.TabIndex = 16;
            this.textBox3.Text = "Info: You can double click on an active element to copy it to your clipboard.";
            // 
            // listBoxActiveTemplates
            // 
            this.listBoxActiveTemplates.FormattingEnabled = true;
            this.listBoxActiveTemplates.Location = new System.Drawing.Point(6, 45);
            this.listBoxActiveTemplates.Name = "listBoxActiveTemplates";
            this.listBoxActiveTemplates.ScrollAlwaysVisible = true;
            this.listBoxActiveTemplates.Size = new System.Drawing.Size(714, 160);
            this.listBoxActiveTemplates.TabIndex = 3;
            this.listBoxActiveTemplates.SelectedIndexChanged += new System.EventHandler(this.listBoxActiveTemplates_SelectedIndexChanged);
            // 
            // groupBoxCompleted
            // 
            this.groupBoxCompleted.Controls.Add(this.textBox4);
            this.groupBoxCompleted.Controls.Add(this.listBoxTemplatesCompleted);
            this.groupBoxCompleted.Location = new System.Drawing.Point(381, 275);
            this.groupBoxCompleted.Name = "groupBoxCompleted";
            this.groupBoxCompleted.Size = new System.Drawing.Size(360, 317);
            this.groupBoxCompleted.TabIndex = 7;
            this.groupBoxCompleted.TabStop = false;
            this.groupBoxCompleted.Text = "Completed password changes";
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.Control;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Location = new System.Drawing.Point(6, 19);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(344, 30);
            this.textBox4.TabIndex = 16;
            this.textBox4.Text = "Info: Once a password change is complete, the whole database will automatically g" +
    "et saved.";
            // 
            // listBoxTemplatesCompleted
            // 
            this.listBoxTemplatesCompleted.FormattingEnabled = true;
            this.listBoxTemplatesCompleted.Location = new System.Drawing.Point(6, 58);
            this.listBoxTemplatesCompleted.Name = "listBoxTemplatesCompleted";
            this.listBoxTemplatesCompleted.ScrollAlwaysVisible = true;
            this.listBoxTemplatesCompleted.Size = new System.Drawing.Size(344, 251);
            this.listBoxTemplatesCompleted.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.listBoxFailedTemplates);
            this.groupBox1.Location = new System.Drawing.Point(747, 275);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(360, 317);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Failed password changes";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.Control;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(6, 19);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(344, 30);
            this.textBox2.TabIndex = 15;
            this.textBox2.Text = "Info: You can double click on a failed element to copy it to your clipboard.";
            // 
            // listBoxFailedTemplates
            // 
            this.listBoxFailedTemplates.FormattingEnabled = true;
            this.listBoxFailedTemplates.Location = new System.Drawing.Point(6, 58);
            this.listBoxFailedTemplates.Name = "listBoxFailedTemplates";
            this.listBoxFailedTemplates.ScrollAlwaysVisible = true;
            this.listBoxFailedTemplates.Size = new System.Drawing.Size(344, 251);
            this.listBoxFailedTemplates.TabIndex = 3;
            this.listBoxFailedTemplates.SelectedIndexChanged += new System.EventHandler(this.listBoxFailedTemplates_SelectedIndexChanged);
            // 
            // Working
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1131, 608);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxCompleted);
            this.Controls.Add(this.groupBoxActiveTemplates);
            this.Controls.Add(this.groupBoxQueuedTemplates);
            this.Controls.Add(this.labelDescription);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Working";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change all password database passwords";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Working_FormClosing);
            this.Load += new System.EventHandler(this.Working_Load);
            this.groupBoxQueuedTemplates.ResumeLayout(false);
            this.groupBoxQueuedTemplates.PerformLayout();
            this.groupBoxActiveTemplates.ResumeLayout(false);
            this.groupBoxActiveTemplates.PerformLayout();
            this.groupBoxCompleted.ResumeLayout(false);
            this.groupBoxCompleted.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.ListBox listBoxRemainingTemplates;
        private System.Windows.Forms.GroupBox groupBoxQueuedTemplates;
        private System.Windows.Forms.GroupBox groupBoxActiveTemplates;
        private System.Windows.Forms.ListBox listBoxActiveTemplates;
        private System.Windows.Forms.GroupBox groupBoxCompleted;
        private System.Windows.Forms.ListBox listBoxTemplatesCompleted;
        private System.Windows.Forms.Button buttonStartChange;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxRemovedTemplates;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBoxFailedTemplates;
        private System.Windows.Forms.Button buttonEnqueueEntry;
        private System.Windows.Forms.Button buttonRemovedEntry;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
    }
}