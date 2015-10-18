namespace uomap_client
{
    partial class MainForm
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
            this.characterListBox = new uomap_client.RefreshingListBox();
            this.openMapButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // characterListBox
            // 
            this.characterListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.characterListBox.FormattingEnabled = true;
            this.characterListBox.Location = new System.Drawing.Point(12, 12);
            this.characterListBox.Name = "characterListBox";
            this.characterListBox.Size = new System.Drawing.Size(118, 212);
            this.characterListBox.TabIndex = 0;
            // 
            // openMapButton
            // 
            this.openMapButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openMapButton.Location = new System.Drawing.Point(253, 210);
            this.openMapButton.Name = "openMapButton";
            this.openMapButton.Size = new System.Drawing.Size(75, 23);
            this.openMapButton.TabIndex = 1;
            this.openMapButton.Text = "Open Map";
            this.openMapButton.UseVisualStyleBackColor = true;
            this.openMapButton.Click += new System.EventHandler(this.openMapButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 245);
            this.Controls.Add(this.openMapButton);
            this.Controls.Add(this.characterListBox);
            this.Name = "MainForm";
            this.Text = "uomap";
            this.ResumeLayout(false);

        }

        #endregion

        private RefreshingListBox characterListBox;
        private System.Windows.Forms.Button openMapButton;

    }
}

