using System;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using EpiInfoAPI;
using System.Collections.Generic;
using LogManager;
using REDCapAPI;

namespace Default
{
    public partial class EpiConfiguration : Form
    {
        private readonly Sql _objSql;
        private string _configId;
        private string _connectionstring;

        public EpiConfiguration()
        {
            InitializeComponent();
            var objSettings = new Settings(Application.StartupPath);
            var cred = objSettings.ReadApiSettings();
            CommonData.Credentials = cred;
            _objSql = new Sql(Application.StartupPath, CommonData.Credentials.ConnectionString);
            DataTable tbl = _objSql.ReadEpiSettings();
            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    _configId = Convert.ToString(row["Config_id"]);
                    string formName = Convert.ToString(row["Form_NM"]);
                    dgvConfig.Rows.Add(new object[] { false, _configId, formName });
                }
            }           
            DisableFields();
        }            

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (dgvConfig.Rows.Count == 0) return;
            var dgvrows =
                   dgvConfig.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
            if (dgvrows.Any())
            {               
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Epi Info 7 Project Files (*.prj)|*.prj";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = false;
                DialogResult res = openFileDialog.ShowDialog();
                if (res == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName.Trim();                  
                    Project project = new Project(filePath, true);
                    txtFormName.Text = project.FormName;
                    _connectionstring = project.DbConnection;
                    txtApiUrl.Text = _connectionstring;
                    EnableFields();
                }
            }
            else
                {
                System.Windows.Forms.MessageBox.Show("Please Select atleast one row", "Epi Info", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
            }

        }

        private void dgvConfig_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1) return;               
                    string frmName = Convert.ToString(dgvConfig.Rows[e.RowIndex].Cells["clmFormName"].Value);
                _configId = Convert.ToString(dgvConfig.Rows[e.RowIndex].Cells["clmConfigId"].Value);
                DisableFields();
                DataTable tbl = _objSql.ReadSettings(frmName);
                if (tbl != null)
                {
                    if (tbl.Rows.Count > 0)
                    {
                        FillValues(tbl.Rows[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        void FillValues(DataRow row)
        {
            txtApiUrl.Text = Convert.ToString(row["redcapurl"]);          
            txtFormName.Text = Convert.ToString(row["Form_NM"]);
            txtAuthorId.Text = Convert.ToString(row["author_id"]);
            txtCustodianId.Text = Convert.ToString(row["custodian_id"]);
            txtStateId.Text = Convert.ToString(row["site_oid"]);
            txtExConditions.Text = Convert.ToString(row["Exclude_Conditions"]);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtApiUrl.Text) || string.IsNullOrEmpty(txtFormName.Text))
                {

                    System.Windows.Forms.MessageBox.Show("Please enter valid information", "Epi Info", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                    return;
                }
                if (!string.IsNullOrEmpty(_configId))
                {
                    Save();
                    return;
                }
                if (_objSql.IsFormAlreadyExist(txtFormName.Text))
                {                   
                    System.Windows.Forms.MessageBox.Show("There is already a configuration exist in the database for form." + txtFormName.Text +
                        "This cant be saved.", "Epi Info", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                    return;
                }
                Save();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvConfig.Rows.Count == 0) return;
                var dgvrows =
                       dgvConfig.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
                if (dgvrows.Any())
                {

                   if (
                        CommonData.ShowMessage(
                            "This action will delete the configuration which you have already created. Do you want to continue?",
                           CommonData.MsgBoxType.Question) == DialogResult.Yes)
                      //  if(System.Windows.Forms.MessageBox.Show("Success", "Epi Info", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Question, System.Windows.Forms.MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {                   
                        var rows = dgvConfig.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
                        if (rows.Any())
                        {
                            var configIds = new List<string>();
                            foreach (DataGridViewRow row in rows)
                            {
                                string configId = Convert.ToString(row.Cells["clmConfigId"].Value);
                                configIds.Add(configId);
                            }

                            if (configIds.Count > 0)
                            {
                                if (_objSql.DeleteConfiguration(configIds))
                                {

                                }
                            }
                        }

                        try
                        {                           
                                if (rows.Any())                               
                                {
                                    foreach (DataGridViewRow row in rows)
                                    {
                                        dgvConfig.Rows.Remove(row);
                                    }
                                }                               
                            ResetValues();
                        }
                        catch (Exception ex)
                        {
                            Log.WriteToErrorLog(ex);
                        }
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Please Select atleast one row", "Epi Info", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        void Save()
        {
            var c = new Settings.Credentials
            {
                DataSource = "Epi Info",
                ApiUrl = txtApiUrl.Text,
                Token = " ",
                FormName = txtFormName.Text,
                StateId = txtStateId.Text,
                AuthorId = txtAuthorId.Text,
                CustodianId = txtCustodianId.Text,
                ConfigId = _configId,
                Exclude = txtExConditions.Text
            };
            if (_objSql.SaveConfig(c))
            {
                System.Windows.Forms.MessageBox.Show("Success", "Epi Info", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);               
                Close();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Failed", "Epi Info", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);              
            }
        }

        private void btn_AddNew_Click(object sender, EventArgs e)
        {
            _configId = "";
            ResetValues();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Epi Info 7 Project Files (*.prj)|*.prj";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName.Trim();                
                Project project = new Project(filePath, true);
                txtFormName.Text = project.FormName;
                _connectionstring = project.DbConnection;
                txtApiUrl.Text = _connectionstring;
                EnableFields();
            }
        }

        void ResetValues()
        {
            txtApiUrl.Text = "";          
            txtFormName.Text = "";
            txtAuthorId.Text = "";
            txtCustodianId.Text = "";
            txtStateId.Text = "";
            txtExConditions.Text = "";            
        }

        void EnableFields()
        {
            //txtApiUrl.Enabled = true;           
           // txtFormName.Enabled = true;
            txtAuthorId.Enabled = true;
            txtCustodianId.Enabled = true;
            txtStateId.Enabled = true;
            txtExConditions.Enabled = true;
        }

        void DisableFields()
        {
            //txtApiUrl.Enabled=false;           
           // txtFormName.Enabled = false;
            txtAuthorId.Enabled = false;
            txtCustodianId.Enabled = false;
            txtStateId.Enabled = false;
            txtExConditions.Enabled = false;
        }
    }
}
