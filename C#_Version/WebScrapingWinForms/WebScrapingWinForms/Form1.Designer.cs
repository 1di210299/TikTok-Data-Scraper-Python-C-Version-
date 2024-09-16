namespace WebScrapingWinForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Button processAllButton;
        private System.Windows.Forms.Button loadUrlsButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.processAllButton = new System.Windows.Forms.Button();
            this.loadUrlsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(12, 12);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(776, 397);
            this.logTextBox.TabIndex = 0;
            // 
            // processAllButton
            // 
            this.processAllButton.Location = new System.Drawing.Point(12, 415);
            this.processAllButton.Name = "processAllButton";
            this.processAllButton.Size = new System.Drawing.Size(380, 23);
            this.processAllButton.TabIndex = 1;
            this.processAllButton.Text = "Procesar Todos los Videos";
            this.processAllButton.UseVisualStyleBackColor = true;
            this.processAllButton.Click += new System.EventHandler(this.ProcessAllButton_Click);
            // 
            // loadUrlsButton
            // 
            this.loadUrlsButton.Location = new System.Drawing.Point(408, 415);
            this.loadUrlsButton.Name = "loadUrlsButton";
            this.loadUrlsButton.Size = new System.Drawing.Size(380, 23);
            this.loadUrlsButton.TabIndex = 2;
            this.loadUrlsButton.Text = "Cargar URLs desde archivo";
            this.loadUrlsButton.UseVisualStyleBackColor = true;
            this.loadUrlsButton.Click += new System.EventHandler(this.LoadUrlsButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.loadUrlsButton);
            this.Controls.Add(this.processAllButton);
            this.Controls.Add(this.logTextBox);
            this.Name = "Form1";
            this.Text = "TikTok Video Scraper";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
