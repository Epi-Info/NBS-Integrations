namespace Default
{
    partial class CreateLookup
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
            this.txtQuesDsc = new System.Windows.Forms.TextBox();
            this.drp_Datatype = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtQuesIdentifier = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtQuesDisName = new System.Windows.Forms.TextBox();
            this.grp_Question = new System.Windows.Forms.GroupBox();
            this.txtQuesCode = new System.Windows.Forms.TextBox();
            this.grp_Answer = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.drp_CdTranslation = new System.Windows.Forms.ComboBox();
            this.txtInvsCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtAnsDisName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtAnsCodeDis = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAnsSysCode = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtAnsCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.grp_Question.SuspendLayout();
            this.grp_Answer.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(19, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Question Code *";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(19, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(186, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Question Code Description *";
            // 
            // txtQuesDsc
            // 
            this.txtQuesDsc.Location = new System.Drawing.Point(220, 75);
            this.txtQuesDsc.Name = "txtQuesDsc";
            this.txtQuesDsc.Size = new System.Drawing.Size(175, 22);
            this.txtQuesDsc.TabIndex = 2;
            this.txtQuesDsc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuesCode_KeyPress);
            // 
            // drp_Datatype
            // 
            this.drp_Datatype.FormattingEnabled = true;
            this.drp_Datatype.Items.AddRange(new object[] {
            "TEXT",
            "NUMERIC",
            "DATE",
            "DATE/TIME",
            "CODED"});
            this.drp_Datatype.Location = new System.Drawing.Point(220, 115);
            this.drp_Datatype.Name = "drp_Datatype";
            this.drp_Datatype.Size = new System.Drawing.Size(175, 24);
            this.drp_Datatype.TabIndex = 3;
            this.drp_Datatype.SelectedIndexChanged += new System.EventHandler(this.drp_Datatype_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Data Type";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Question Identifier *";
            // 
            // txtQuesIdentifier
            // 
            this.txtQuesIdentifier.Location = new System.Drawing.Point(220, 155);
            this.txtQuesIdentifier.Name = "txtQuesIdentifier";
            this.txtQuesIdentifier.Size = new System.Drawing.Size(175, 22);
            this.txtQuesIdentifier.TabIndex = 4;
            this.txtQuesIdentifier.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuesCode_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 195);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(115, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "Question Name *";
            // 
            // txtQuesDisName
            // 
            this.txtQuesDisName.Location = new System.Drawing.Point(220, 195);
            this.txtQuesDisName.Name = "txtQuesDisName";
            this.txtQuesDisName.Size = new System.Drawing.Size(175, 22);
            this.txtQuesDisName.TabIndex = 5;
            this.txtQuesDisName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuesCode_KeyPress);
            // 
            // grp_Question
            // 
            this.grp_Question.Controls.Add(this.txtQuesCode);
            this.grp_Question.Controls.Add(this.label1);
            this.grp_Question.Controls.Add(this.label2);
            this.grp_Question.Controls.Add(this.txtQuesDsc);
            this.grp_Question.Controls.Add(this.label3);
            this.grp_Question.Controls.Add(this.txtQuesIdentifier);
            this.grp_Question.Controls.Add(this.drp_Datatype);
            this.grp_Question.Controls.Add(this.txtQuesDisName);
            this.grp_Question.Controls.Add(this.label4);
            this.grp_Question.Controls.Add(this.label5);
            this.grp_Question.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.grp_Question.Location = new System.Drawing.Point(22, 21);
            this.grp_Question.Name = "grp_Question";
            this.grp_Question.Size = new System.Drawing.Size(420, 277);
            this.grp_Question.TabIndex = 10;
            this.grp_Question.TabStop = false;
            this.grp_Question.Text = "Question Info";
            // 
            // txtQuesCode
            // 
            this.txtQuesCode.Location = new System.Drawing.Point(220, 35);
            this.txtQuesCode.Name = "txtQuesCode";
            this.txtQuesCode.Size = new System.Drawing.Size(175, 22);
            this.txtQuesCode.TabIndex = 1;
            this.txtQuesCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuesCode_KeyPress);
            // 
            // grp_Answer
            // 
            this.grp_Answer.Controls.Add(this.label11);
            this.grp_Answer.Controls.Add(this.drp_CdTranslation);
            this.grp_Answer.Controls.Add(this.txtInvsCode);
            this.grp_Answer.Controls.Add(this.label10);
            this.grp_Answer.Controls.Add(this.txtAnsDisName);
            this.grp_Answer.Controls.Add(this.label9);
            this.grp_Answer.Controls.Add(this.txtAnsCodeDis);
            this.grp_Answer.Controls.Add(this.label8);
            this.grp_Answer.Controls.Add(this.txtAnsSysCode);
            this.grp_Answer.Controls.Add(this.label7);
            this.grp_Answer.Controls.Add(this.txtAnsCode);
            this.grp_Answer.Controls.Add(this.label6);
            this.grp_Answer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.grp_Answer.Location = new System.Drawing.Point(467, 22);
            this.grp_Answer.Name = "grp_Answer";
            this.grp_Answer.Size = new System.Drawing.Size(396, 276);
            this.grp_Answer.TabIndex = 11;
            this.grp_Answer.TabStop = false;
            this.grp_Answer.Text = "Answer Info";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 235);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(178, 17);
            this.label11.TabIndex = 11;
            this.label11.Text = "Code Translation Required";
            this.label11.Visible = false;
            // 
            // drp_CdTranslation
            // 
            this.drp_CdTranslation.FormattingEnabled = true;
            this.drp_CdTranslation.Items.AddRange(new object[] {
            "YES",
            "NO"});
            this.drp_CdTranslation.Location = new System.Drawing.Point(200, 235);
            this.drp_CdTranslation.Name = "drp_CdTranslation";
            this.drp_CdTranslation.Size = new System.Drawing.Size(175, 24);
            this.drp_CdTranslation.TabIndex = 11;
            this.drp_CdTranslation.Visible = false;
            // 
            // txtInvsCode
            // 
            this.txtInvsCode.Location = new System.Drawing.Point(200, 195);
            this.txtInvsCode.Name = "txtInvsCode";
            this.txtInvsCode.Size = new System.Drawing.Size(175, 22);
            this.txtInvsCode.TabIndex = 10;
            this.txtInvsCode.Visible = false;
            this.txtInvsCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuesCode_KeyPress);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 195);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(124, 17);
            this.label10.TabIndex = 8;
            this.label10.Text = "Investigation Code";
            this.label10.Visible = false;
            // 
            // txtAnsDisName
            // 
            this.txtAnsDisName.Location = new System.Drawing.Point(200, 155);
            this.txtAnsDisName.Name = "txtAnsDisName";
            this.txtAnsDisName.Size = new System.Drawing.Size(175, 22);
            this.txtAnsDisName.TabIndex = 9;
            this.txtAnsDisName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuesCode_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 155);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 17);
            this.label9.TabIndex = 6;
            this.label9.Text = "Answer Name";
            // 
            // txtAnsCodeDis
            // 
            this.txtAnsCodeDis.Location = new System.Drawing.Point(200, 115);
            this.txtAnsCodeDis.Name = "txtAnsCodeDis";
            this.txtAnsCodeDis.Size = new System.Drawing.Size(175, 22);
            this.txtAnsCodeDis.TabIndex = 8;
            this.txtAnsCodeDis.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuesCode_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 115);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(166, 17);
            this.label8.TabIndex = 4;
            this.label8.Text = "Answer Code Description";
            // 
            // txtAnsSysCode
            // 
            this.txtAnsSysCode.Location = new System.Drawing.Point(200, 75);
            this.txtAnsSysCode.Name = "txtAnsSysCode";
            this.txtAnsSysCode.Size = new System.Drawing.Size(175, 22);
            this.txtAnsSysCode.TabIndex = 7;
            this.txtAnsSysCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuesCode_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(141, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Answer System Code";
            // 
            // txtAnsCode
            // 
            this.txtAnsCode.Location = new System.Drawing.Point(200, 35);
            this.txtAnsCode.Name = "txtAnsCode";
            this.txtAnsCode.Size = new System.Drawing.Size(175, 22);
            this.txtAnsCode.TabIndex = 6;
            this.txtAnsCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAnsCode_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Answer Code";
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.Location = new System.Drawing.Point(494, 325);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(173, 45);
            this.btn_Save.TabIndex = 12;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Cancel.ForeColor = System.Drawing.Color.White;
            this.btn_Cancel.Location = new System.Drawing.Point(692, 325);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(173, 45);
            this.btn_Cancel.TabIndex = 13;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // CreateLookup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 382);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.grp_Answer);
            this.Controls.Add(this.grp_Question);
            this.Name = "CreateLookup";
            this.Text = "CreateLookup";
            this.grp_Question.ResumeLayout(false);
            this.grp_Question.PerformLayout();
            this.grp_Answer.ResumeLayout(false);
            this.grp_Answer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtQuesDsc;
        private System.Windows.Forms.ComboBox drp_Datatype;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtQuesIdentifier;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtQuesDisName;
        private System.Windows.Forms.GroupBox grp_Question;
        private System.Windows.Forms.TextBox txtQuesCode;
        private System.Windows.Forms.GroupBox grp_Answer;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox drp_CdTranslation;
        private System.Windows.Forms.TextBox txtInvsCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtAnsDisName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtAnsCodeDis;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAnsSysCode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAnsCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Cancel;
    }
}