
namespace Image_indexer
{
    partial class savingSettingsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(savingSettingsWindow));
            this.projectLabel = new System.Windows.Forms.Label();
            this.projectIDLabel = new System.Windows.Forms.Label();
            this.projectNameField = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.projectIDField = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // projectLabel
            // 
            this.projectLabel.AutoSize = true;
            this.projectLabel.Location = new System.Drawing.Point(13, 13);
            this.projectLabel.Name = "projectLabel";
            this.projectLabel.Size = new System.Drawing.Size(72, 13);
            this.projectLabel.TabIndex = 0;
            this.projectLabel.Text = "Project name:";
            // 
            // projectIDLabel
            // 
            this.projectIDLabel.AutoSize = true;
            this.projectIDLabel.Location = new System.Drawing.Point(13, 49);
            this.projectIDLabel.Name = "projectIDLabel";
            this.projectIDLabel.Size = new System.Drawing.Size(57, 13);
            this.projectIDLabel.TabIndex = 1;
            this.projectIDLabel.Text = "Project ID:";
            // 
            // projectNameField
            // 
            this.projectNameField.Location = new System.Drawing.Point(91, 10);
            this.projectNameField.Name = "projectNameField";
            this.projectNameField.Size = new System.Drawing.Size(180, 20);
            this.projectNameField.TabIndex = 1;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(47, 82);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(192, 23);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // projectIDField
            // 
            this.projectIDField.Location = new System.Drawing.Point(91, 46);
            this.projectIDField.Name = "projectIDField";
            this.projectIDField.Size = new System.Drawing.Size(180, 20);
            this.projectIDField.TabIndex = 2;
            // 
            // savingSettingsWindow
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 117);
            this.Controls.Add(this.projectIDField);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.projectNameField);
            this.Controls.Add(this.projectIDLabel);
            this.Controls.Add(this.projectLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(299, 156);
            this.MinimumSize = new System.Drawing.Size(299, 156);
            this.Name = "savingSettingsWindow";
            this.Text = "Config saving";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label projectLabel;
        private System.Windows.Forms.Label projectIDLabel;
        private System.Windows.Forms.TextBox projectNameField;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox projectIDField;
    }
}