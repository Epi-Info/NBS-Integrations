using System.Drawing;
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnTest = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.redCapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.epiInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.btnDataviewer = new System.Windows.Forms.Button();
            this.btnMapping = new System.Windows.Forms.Button();
            this.btnRedCap = new System.Windows.Forms.Button();
            this.btnEpiInfo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.pictureBox1.Size = new System.Drawing.Size(798, 648);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestConnection.BackColor = System.Drawing.Color.SteelBlue;
            this.btnTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Image = ((System.Drawing.Image)(resources.GetObject("btnTestConnection.Image")));
            this.btnTestConnection.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnTestConnection.Location = new System.Drawing.Point(45, 147);
            this.btnTestConnection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Padding = new System.Windows.Forms.Padding(0, 25, 0, 25);
            this.btnTestConnection.Size = new System.Drawing.Size(204, 158);
            this.btnTestConnection.TabIndex = 4;
            this.btnTestConnection.Text = "NBS Data Connection";
            this.btnTestConnection.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.Color.White;
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
            this.mainScreenToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
            this.mainScreenToolStripMenuItem.Text = "Data Viewer";
            this.mainScreenToolStripMenuItem.Click += new System.EventHandler(this.mainScreenToolStripMenuItem_Click);
            // 
            // mnuSettings
            // 
            this.mnuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.redCapToolStripMenuItem,
            this.epiInfoToolStripMenuItem});
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(165, 26);
            this.mnuSettings.Text = "Settings";
            // 
            // redCapToolStripMenuItem
            // 
            this.redCapToolStripMenuItem.Name = "redCapToolStripMenuItem";
            this.redCapToolStripMenuItem.Size = new System.Drawing.Size(138, 26);
            this.redCapToolStripMenuItem.Text = "REDCap";
            this.redCapToolStripMenuItem.Click += new System.EventHandler(this.redCapToolStripMenuItem_Click);
            // 
            // epiInfoToolStripMenuItem
            // 
            this.epiInfoToolStripMenuItem.Name = "epiInfoToolStripMenuItem";
            this.epiInfoToolStripMenuItem.Size = new System.Drawing.Size(138, 26);
            this.epiInfoToolStripMenuItem.Text = "Epi Info";
            this.epiInfoToolStripMenuItem.Click += new System.EventHandler(this.epiInfoToolStripMenuItem_Click);
            // 
            // mnuMapping
            // 
            this.mnuMapping.Name = "mnuMapping";
            this.mnuMapping.Size = new System.Drawing.Size(165, 26);
            this.mnuMapping.Text = "Mapping";
            this.mnuMapping.Click += new System.EventHandler(this.mnuMapping_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.openToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(800, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // btnDataviewer
            // 
            this.btnDataviewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDataviewer.BackColor = System.Drawing.Color.SteelBlue;
            this.btnDataviewer.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnDataviewer.FlatAppearance.BorderSize = 2;
            this.btnDataviewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDataviewer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDataviewer.ForeColor = System.Drawing.Color.White;
            this.btnDataviewer.Image = ((System.Drawing.Image)(resources.GetObject("btnDataviewer.Image")));
            this.btnDataviewer.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDataviewer.Location = new System.Drawing.Point(299, 147);
            this.btnDataviewer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDataviewer.Name = "btnDataviewer";
            this.btnDataviewer.Padding = new System.Windows.Forms.Padding(0, 25, 0, 25);
            this.btnDataviewer.Size = new System.Drawing.Size(204, 158);
            this.btnDataviewer.TabIndex = 5;
            this.btnDataviewer.Text = "Data Viewer";
            this.btnDataviewer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDataviewer.UseVisualStyleBackColor = false;
            this.btnDataviewer.Click += new System.EventHandler(this.mainScreenToolStripMenuItem_Click);
            // 
            // btnMapping
            // 
            this.btnMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapping.BackColor = System.Drawing.Color.SteelBlue;
            this.btnMapping.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnMapping.FlatAppearance.BorderSize = 2;
            this.btnMapping.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMapping.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMapping.ForeColor = System.Drawing.Color.White;
            this.btnMapping.Image = ((System.Drawing.Image)(resources.GetObject("btnMapping.Image")));
            this.btnMapping.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnMapping.Location = new System.Drawing.Point(549, 147);
            this.btnMapping.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMapping.Name = "btnMapping";
            this.btnMapping.Padding = new System.Windows.Forms.Padding(0, 25, 0, 25);
            this.btnMapping.Size = new System.Drawing.Size(213, 158);
            this.btnMapping.TabIndex = 6;
            this.btnMapping.Text = "Mapping";
            this.btnMapping.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnMapping.UseVisualStyleBackColor = false;
            this.btnMapping.Click += new System.EventHandler(this.mnuMapping_Click);
            // 
            // btnRedCap
            // 
            this.btnRedCap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRedCap.BackColor = System.Drawing.Color.White;
            this.btnRedCap.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnRedCap.FlatAppearance.BorderSize = 2;
            this.btnRedCap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRedCap.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRedCap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnRedCap.Image = ((System.Drawing.Image)(resources.GetObject("btnRedCap.Image")));
            this.btnRedCap.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRedCap.Location = new System.Drawing.Point(217, 411);
            this.btnRedCap.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRedCap.Name = "btnRedCap";
            this.btnRedCap.Padding = new System.Windows.Forms.Padding(0, 50, 0, 10);
            this.btnRedCap.Size = new System.Drawing.Size(256, 182);
            this.btnRedCap.TabIndex = 7;
            this.btnRedCap.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRedCap.UseVisualStyleBackColor = false;
            this.btnRedCap.Click += new System.EventHandler(this.redCapToolStripMenuItem_Click);
            // 
            // btnEpiInfo
            // 
            this.btnEpiInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEpiInfo.BackColor = System.Drawing.Color.White;
            this.btnEpiInfo.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnEpiInfo.FlatAppearance.BorderSize = 2;
            this.btnEpiInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEpiInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEpiInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnEpiInfo.Image = ((System.Drawing.Image)(resources.GetObject("btnEpiInfo.Image")));
            this.btnEpiInfo.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEpiInfo.Location = new System.Drawing.Point(495, 411);
            this.btnEpiInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEpiInfo.Name = "btnEpiInfo";
            this.btnEpiInfo.Padding = new System.Windows.Forms.Padding(0, 60, 0, 10);
            this.btnEpiInfo.Size = new System.Drawing.Size(267, 182);
            this.btnEpiInfo.TabIndex = 8;
            this.btnEpiInfo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEpiInfo.UseVisualStyleBackColor = false;
            this.btnEpiInfo.Click += new System.EventHandler(this.epiInfoToolStripMenuItem_Click);
            // 
            // Splash
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(798, 648);
            this.Controls.Add(this.btnEpiInfo);
            this.Controls.Add(this.btnRedCap);
            this.Controls.Add(this.btnMapping);
            this.Controls.Add(this.btnDataviewer);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "Splash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NEDSS Base System";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnTest;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mainScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem redCapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem epiInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuMapping;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button btnDataviewer;
        private System.Windows.Forms.Button btnMapping;
        private System.Windows.Forms.Button btnRedCap;
        private System.Windows.Forms.Button btnEpiInfo;
    }
}