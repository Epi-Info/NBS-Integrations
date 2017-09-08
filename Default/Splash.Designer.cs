namespace Default
{
    partial class Splash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnTest = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.redCapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.epiInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.openToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1147, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnTest});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // btnTest
            // 
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(220, 26);
            this.btnTest.Text = "Test SQL Connection";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainScreenToolStripMenuItem,
            this.mnuSettings,
            this.mnuMapping});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // mainScreenToolStripMenuItem
            // 
            this.mainScreenToolStripMenuItem.Name = "mainScreenToolStripMenuItem";
            this.mainScreenToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.mainScreenToolStripMenuItem.Text = "Main Screen";
            this.mainScreenToolStripMenuItem.Click += new System.EventHandler(this.mainScreenToolStripMenuItem_Click);
            // 
            // mnuSettings
            // 
            this.mnuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.redCapToolStripMenuItem,
            this.epiInfoToolStripMenuItem});
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(181, 26);
            this.mnuSettings.Text = "Settings";
            // 
            // redCapToolStripMenuItem
            // 
            this.redCapToolStripMenuItem.Name = "redCapToolStripMenuItem";
            this.redCapToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.redCapToolStripMenuItem.Text = "Red Cap";
            this.redCapToolStripMenuItem.Click += new System.EventHandler(this.redCapToolStripMenuItem_Click);
            // 
            // epiInfoToolStripMenuItem
            // 
            this.epiInfoToolStripMenuItem.Name = "epiInfoToolStripMenuItem";
            this.epiInfoToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.epiInfoToolStripMenuItem.Text = "Epi Info";
            this.epiInfoToolStripMenuItem.Click += new System.EventHandler(this.epiInfoToolStripMenuItem_Click);
            // 
            // mnuMapping
            // 
            this.mnuMapping.Name = "mnuMapping";
            this.mnuMapping.Size = new System.Drawing.Size(181, 26);
            this.mnuMapping.Text = "Mapping";
            this.mnuMapping.Click += new System.EventHandler(this.mnuMapping_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 28);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1147, 634);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // Splash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1147, 662);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Splash";
            this.Text = "NEDSS Base System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnTest;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuMapping;
        private System.Windows.Forms.ToolStripMenuItem mainScreenToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem redCapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem epiInfoToolStripMenuItem;
    }
}