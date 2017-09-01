namespace KeePassPasswordChanger.Templates
{
    partial class Overview
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelViewMode = new System.Windows.Forms.Label();
            this.listBoxSavedTemplates = new System.Windows.Forms.ListBox();
            this.buttonCreateTemplate = new System.Windows.Forms.Button();
            this.buttonDeleteSelectedTemplates = new System.Windows.Forms.Button();
            this.buttonImportTemplates = new System.Windows.Forms.Button();
            this.buttonExportAllTemplates = new System.Windows.Forms.Button();
            this.buttonEditSelectedTemplate = new System.Windows.Forms.Button();
            this.buttonCloneSelectedTemplate = new System.Windows.Forms.Button();
            this.buttonCreateDemoTemplate = new System.Windows.Forms.Button();
            this.buttonExportSelectedTemplate = new System.Windows.Forms.Button();
            this.buttonVisitTemplatesPage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "You are viewing the ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(146, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Templates";
            // 
            // labelViewMode
            // 
            this.labelViewMode.AutoSize = true;
            this.labelViewMode.Location = new System.Drawing.Point(111, 9);
            this.labelViewMode.Name = "labelViewMode";
            this.labelViewMode.Size = new System.Drawing.Size(39, 13);
            this.labelViewMode.TabIndex = 2;
            this.labelViewMode.Text = "private";
            // 
            // listBoxSavedTemplates
            // 
            this.listBoxSavedTemplates.FormattingEnabled = true;
            this.listBoxSavedTemplates.Location = new System.Drawing.Point(12, 25);
            this.listBoxSavedTemplates.Name = "listBoxSavedTemplates";
            this.listBoxSavedTemplates.Size = new System.Drawing.Size(296, 277);
            this.listBoxSavedTemplates.Sorted = true;
            this.listBoxSavedTemplates.TabIndex = 3;
            // 
            // buttonCreateTemplate
            // 
            this.buttonCreateTemplate.Location = new System.Drawing.Point(314, 25);
            this.buttonCreateTemplate.Name = "buttonCreateTemplate";
            this.buttonCreateTemplate.Size = new System.Drawing.Size(214, 23);
            this.buttonCreateTemplate.TabIndex = 4;
            this.buttonCreateTemplate.Text = "Create new Template";
            this.buttonCreateTemplate.UseVisualStyleBackColor = true;
            this.buttonCreateTemplate.Click += new System.EventHandler(this.buttonCreateTemplate_Click);
            // 
            // buttonDeleteSelectedTemplates
            // 
            this.buttonDeleteSelectedTemplates.Location = new System.Drawing.Point(314, 112);
            this.buttonDeleteSelectedTemplates.Name = "buttonDeleteSelectedTemplates";
            this.buttonDeleteSelectedTemplates.Size = new System.Drawing.Size(214, 23);
            this.buttonDeleteSelectedTemplates.TabIndex = 4;
            this.buttonDeleteSelectedTemplates.Text = "Delete selected Templates";
            this.buttonDeleteSelectedTemplates.UseVisualStyleBackColor = true;
            this.buttonDeleteSelectedTemplates.Click += new System.EventHandler(this.buttonDeleteSelectedTemplates_Click);
            // 
            // buttonImportTemplates
            // 
            this.buttonImportTemplates.Location = new System.Drawing.Point(314, 141);
            this.buttonImportTemplates.Name = "buttonImportTemplates";
            this.buttonImportTemplates.Size = new System.Drawing.Size(214, 23);
            this.buttonImportTemplates.TabIndex = 4;
            this.buttonImportTemplates.Text = "Import Templates from Folder";
            this.buttonImportTemplates.UseVisualStyleBackColor = true;
            this.buttonImportTemplates.Click += new System.EventHandler(this.buttonImportTemplates_Click);
            // 
            // buttonExportAllTemplates
            // 
            this.buttonExportAllTemplates.Location = new System.Drawing.Point(314, 170);
            this.buttonExportAllTemplates.Name = "buttonExportAllTemplates";
            this.buttonExportAllTemplates.Size = new System.Drawing.Size(214, 23);
            this.buttonExportAllTemplates.TabIndex = 5;
            this.buttonExportAllTemplates.Text = "Export all Templates to Folder";
            this.buttonExportAllTemplates.UseVisualStyleBackColor = true;
            this.buttonExportAllTemplates.Click += new System.EventHandler(this.buttonExportSelectedTemplates_Click);
            // 
            // buttonEditSelectedTemplate
            // 
            this.buttonEditSelectedTemplate.Location = new System.Drawing.Point(314, 54);
            this.buttonEditSelectedTemplate.Name = "buttonEditSelectedTemplate";
            this.buttonEditSelectedTemplate.Size = new System.Drawing.Size(214, 23);
            this.buttonEditSelectedTemplate.TabIndex = 4;
            this.buttonEditSelectedTemplate.Text = "Edit selected Template";
            this.buttonEditSelectedTemplate.UseVisualStyleBackColor = true;
            this.buttonEditSelectedTemplate.Click += new System.EventHandler(this.buttonEditSelectedTemplate_Click);
            // 
            // buttonCloneSelectedTemplate
            // 
            this.buttonCloneSelectedTemplate.Location = new System.Drawing.Point(314, 83);
            this.buttonCloneSelectedTemplate.Name = "buttonCloneSelectedTemplate";
            this.buttonCloneSelectedTemplate.Size = new System.Drawing.Size(214, 23);
            this.buttonCloneSelectedTemplate.TabIndex = 4;
            this.buttonCloneSelectedTemplate.Text = "Clone selected Templates";
            this.buttonCloneSelectedTemplate.UseVisualStyleBackColor = true;
            this.buttonCloneSelectedTemplate.Click += new System.EventHandler(this.buttonCloneSelectedTemplate_Click);
            // 
            // buttonCreateDemoTemplate
            // 
            this.buttonCreateDemoTemplate.Location = new System.Drawing.Point(314, 279);
            this.buttonCreateDemoTemplate.Name = "buttonCreateDemoTemplate";
            this.buttonCreateDemoTemplate.Size = new System.Drawing.Size(214, 23);
            this.buttonCreateDemoTemplate.TabIndex = 4;
            this.buttonCreateDemoTemplate.Text = "Create Demo Template";
            this.buttonCreateDemoTemplate.UseVisualStyleBackColor = true;
            this.buttonCreateDemoTemplate.Click += new System.EventHandler(this.buttonCreateDemoTemplate_Click);
            // 
            // buttonExportSelectedTemplate
            // 
            this.buttonExportSelectedTemplate.Location = new System.Drawing.Point(314, 199);
            this.buttonExportSelectedTemplate.Name = "buttonExportSelectedTemplate";
            this.buttonExportSelectedTemplate.Size = new System.Drawing.Size(214, 23);
            this.buttonExportSelectedTemplate.TabIndex = 6;
            this.buttonExportSelectedTemplate.Text = "Export selected Template to Folder";
            this.buttonExportSelectedTemplate.UseVisualStyleBackColor = true;
            this.buttonExportSelectedTemplate.Click += new System.EventHandler(this.buttonExportSelectedTemplate_Click);
            // 
            // buttonVisitTemplatesPage
            // 
            this.buttonVisitTemplatesPage.Location = new System.Drawing.Point(314, 250);
            this.buttonVisitTemplatesPage.Name = "buttonVisitTemplatesPage";
            this.buttonVisitTemplatesPage.Size = new System.Drawing.Size(214, 23);
            this.buttonVisitTemplatesPage.TabIndex = 7;
            this.buttonVisitTemplatesPage.Text = "Visit the Public Templates Site";
            this.buttonVisitTemplatesPage.UseVisualStyleBackColor = true;
            this.buttonVisitTemplatesPage.Click += new System.EventHandler(this.buttonVisitTemplatesPage_Click);
            // 
            // Overview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 322);
            this.Controls.Add(this.buttonVisitTemplatesPage);
            this.Controls.Add(this.buttonExportSelectedTemplate);
            this.Controls.Add(this.buttonExportAllTemplates);
            this.Controls.Add(this.buttonImportTemplates);
            this.Controls.Add(this.buttonDeleteSelectedTemplates);
            this.Controls.Add(this.buttonCloneSelectedTemplate);
            this.Controls.Add(this.buttonEditSelectedTemplate);
            this.Controls.Add(this.buttonCreateDemoTemplate);
            this.Controls.Add(this.buttonCreateTemplate);
            this.Controls.Add(this.listBoxSavedTemplates);
            this.Controls.Add(this.labelViewMode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Overview";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Template Overview";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Overview_FormClosing);
            this.Load += new System.EventHandler(this.Overview_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelViewMode;
        private System.Windows.Forms.ListBox listBoxSavedTemplates;
        private System.Windows.Forms.Button buttonCreateTemplate;
        private System.Windows.Forms.Button buttonDeleteSelectedTemplates;
        private System.Windows.Forms.Button buttonImportTemplates;
        private System.Windows.Forms.Button buttonExportAllTemplates;
        private System.Windows.Forms.Button buttonEditSelectedTemplate;
        private System.Windows.Forms.Button buttonCloneSelectedTemplate;
        private System.Windows.Forms.Button buttonCreateDemoTemplate;
        private System.Windows.Forms.Button buttonExportSelectedTemplate;
        private System.Windows.Forms.Button buttonVisitTemplatesPage;
    }
}