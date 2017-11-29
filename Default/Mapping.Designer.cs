namespace Default
{
    partial class Mapping
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSave = new System.Windows.Forms.Button();
            this.lbRcDe = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbNbsDe = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbNbsTn = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbNbsCn = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvValues = new System.Windows.Forms.DataGridView();
            this.clmSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmFldMapId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmRcde = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmNbsDe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmNbsTn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmNbsCn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbConfigList = new System.Windows.Forms.ComboBox();
            this.btnDelFromDb = new System.Windows.Forms.Button();
            this.txtRedSearch = new System.Windows.Forms.TextBox();
            this.txtDataElemSearch = new System.Windows.Forms.TextBox();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvValues)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.SteelBlue;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(899, 700);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(173, 48);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save to the Database";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lbRcDe
            // 
            this.lbRcDe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbRcDe.FormattingEnabled = true;
            this.lbRcDe.ItemHeight = 16;
            this.lbRcDe.Location = new System.Drawing.Point(10, 148);
            this.lbRcDe.Name = "lbRcDe";
            this.lbRcDe.Size = new System.Drawing.Size(237, 212);
            this.lbRcDe.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(12, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Data Element";
            // 
            // lbNbsDe
            // 
            this.lbNbsDe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbNbsDe.FormattingEnabled = true;
            this.lbNbsDe.ItemHeight = 16;
            this.lbNbsDe.Location = new System.Drawing.Point(294, 148);
            this.lbNbsDe.Name = "lbNbsDe";
            this.lbNbsDe.Size = new System.Drawing.Size(383, 212);
            this.lbNbsDe.TabIndex = 7;
            this.lbNbsDe.SelectedIndexChanged += new System.EventHandler(this.lbNbsDe_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(291, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "NBS Data Element";
            // 
            // cmbNbsTn
            // 
            this.cmbNbsTn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cmbNbsTn.FormattingEnabled = true;
            this.cmbNbsTn.Location = new System.Drawing.Point(718, 116);
            this.cmbNbsTn.Name = "cmbNbsTn";
            this.cmbNbsTn.Size = new System.Drawing.Size(357, 24);
            this.cmbNbsTn.TabIndex = 9;
            this.cmbNbsTn.SelectedIndexChanged += new System.EventHandler(this.cmbNbsTn_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(715, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "NBS Table Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(715, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "NBS Column Name";
            // 
            // cmbNbsCn
            // 
            this.cmbNbsCn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cmbNbsCn.FormattingEnabled = true;
            this.cmbNbsCn.Location = new System.Drawing.Point(715, 215);
            this.cmbNbsCn.Name = "cmbNbsCn";
            this.cmbNbsCn.Size = new System.Drawing.Size(357, 24);
            this.cmbNbsCn.TabIndex = 11;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.SteelBlue;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(10, 700);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(173, 48);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.Text = "Add Mapping to the GRID";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dgvValues
            // 
            this.dgvValues.AllowUserToAddRows = false;
            this.dgvValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvValues.BackgroundColor = System.Drawing.Color.LightGray;
            this.dgvValues.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmSelect,
            this.clmFldMapId,
            this.clmRcde,
            this.clmNbsDe,
            this.clmNbsTn,
            this.clmNbsCn});
            this.dgvValues.EnableHeadersVisualStyles = false;
            this.dgvValues.Location = new System.Drawing.Point(10, 378);
            this.dgvValues.Name = "dgvValues";
            this.dgvValues.RowHeadersVisible = false;
            this.dgvValues.RowTemplate.Height = 24;
            this.dgvValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvValues.Size = new System.Drawing.Size(1062, 306);
            this.dgvValues.TabIndex = 14;
            // 
            // clmSelect
            // 
            this.clmSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.clmSelect.HeaderText = "";
            this.clmSelect.Name = "clmSelect";
            this.clmSelect.Width = 5;
            // 
            // clmFldMapId
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.clmFldMapId.DefaultCellStyle = dataGridViewCellStyle1;
            this.clmFldMapId.HeaderText = "Column1";
            this.clmFldMapId.Name = "clmFldMapId";
            this.clmFldMapId.Visible = false;
            // 
            // clmRcde
            // 
            this.clmRcde.HeaderText = "Source Data Element";
            this.clmRcde.Name = "clmRcde";
            // 
            // clmNbsDe
            // 
            this.clmNbsDe.HeaderText = "NBS Data Element";
            this.clmNbsDe.Name = "clmNbsDe";
            // 
            // clmNbsTn
            // 
            this.clmNbsTn.HeaderText = "NBS Table Name";
            this.clmNbsTn.Name = "clmNbsTn";
            // 
            // clmNbsCn
            // 
            this.clmNbsCn.HeaderText = "NBS Column Name";
            this.clmNbsCn.Name = "clmNbsCn";
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.SteelBlue;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.ForeColor = System.Drawing.Color.White;
            this.btnRemove.Location = new System.Drawing.Point(189, 700);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(173, 48);
            this.btnRemove.TabIndex = 15;
            this.btnRemove.Text = "Remove Selection from the GRID";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.SteelBlue;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(720, 700);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(173, 48);
            this.btnReset.TabIndex = 16;
            this.btnReset.Text = "Reset GRID";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(12, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(165, 17);
            this.label5.TabIndex = 18;
            this.label5.Text = "Select the configuration: ";
            // 
            // cmbConfigList
            // 
            this.cmbConfigList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cmbConfigList.FormattingEnabled = true;
            this.cmbConfigList.Location = new System.Drawing.Point(183, 30);
            this.cmbConfigList.Name = "cmbConfigList";
            this.cmbConfigList.Size = new System.Drawing.Size(357, 24);
            this.cmbConfigList.TabIndex = 17;
            this.cmbConfigList.SelectedIndexChanged += new System.EventHandler(this.cmbConfigList_SelectedIndexChanged);
            // 
            // btnDelFromDb
            // 
            this.btnDelFromDb.BackColor = System.Drawing.Color.SteelBlue;
            this.btnDelFromDb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelFromDb.ForeColor = System.Drawing.Color.White;
            this.btnDelFromDb.Location = new System.Drawing.Point(368, 700);
            this.btnDelFromDb.Name = "btnDelFromDb";
            this.btnDelFromDb.Size = new System.Drawing.Size(173, 48);
            this.btnDelFromDb.TabIndex = 20;
            this.btnDelFromDb.Text = "Remove Selection from the Database";
            this.btnDelFromDb.UseVisualStyleBackColor = true;
            this.btnDelFromDb.Click += new System.EventHandler(this.btnDelFromDb_Click);
            // 
            // txtRedSearch
            // 
            this.txtRedSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtRedSearch.Location = new System.Drawing.Point(10, 117);
            this.txtRedSearch.Name = "txtRedSearch";
            this.txtRedSearch.Size = new System.Drawing.Size(237, 22);
            this.txtRedSearch.TabIndex = 21;
            this.txtRedSearch.Text = "Type here to search...";
            this.txtRedSearch.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtRedSearch_MouseClick);
            this.txtRedSearch.TextChanged += new System.EventHandler(this.txtRedSearch_TextChanged);
            this.txtRedSearch.MouseLeave += new System.EventHandler(this.txtRedSearch_MouseLeave);
            // 
            // txtDataElemSearch
            // 
            this.txtDataElemSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtDataElemSearch.Location = new System.Drawing.Point(294, 116);
            this.txtDataElemSearch.Name = "txtDataElemSearch";
            this.txtDataElemSearch.Size = new System.Drawing.Size(383, 22);
            this.txtDataElemSearch.TabIndex = 22;
            this.txtDataElemSearch.Text = "Type here to search...";
            this.txtDataElemSearch.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtDataElemSearch_MouseClick);
            this.txtDataElemSearch.TextChanged += new System.EventHandler(this.txtDataElemSearch_TextChanged);
            this.txtDataElemSearch.MouseLeave += new System.EventHandler(this.txtDataElemSearch_MouseLeave);
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.HeaderText = "";
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.Width = 212;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "REDCap Data Element";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 212;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "NBS Data Element";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 211;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "NBS Table Name";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 212;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "NBS Column Name";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 212;
            // 
            // Mapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1084, 758);
            this.Controls.Add(this.txtDataElemSearch);
            this.Controls.Add(this.txtRedSearch);
            this.Controls.Add(this.btnDelFromDb);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbConfigList);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.dgvValues);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbNbsCn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbNbsTn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbNbsDe);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbRcDe);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Mapping";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mapping";
            ((System.ComponentModel.ISupportInitialize)(this.dgvValues)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lbRcDe;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbNbsDe;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbNbsTn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbNbsCn;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView dgvValues;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbConfigList;
        private System.Windows.Forms.Button btnDelFromDb;
        private System.Windows.Forms.TextBox txtRedSearch;
        private System.Windows.Forms.TextBox txtDataElemSearch;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmFldMapId;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmRcde;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmNbsDe;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmNbsTn;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmNbsCn;
    }
}