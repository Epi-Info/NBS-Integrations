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
            var objSettings = new Settings(Application.StartupPath);
            var cred = objSettings.ReadApiSettings();
            CommonData.Credentials = cred;
        }            

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Epi Info 7 Project Files (*.prj)|*.prj";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName.Trim();                                            
               // txtApiUrl.Text = filePath;
                Project project = new Project(filePath,true);
                txtFormName.Text = project.FormName;
                _connectionstring = project.DbConnection;
                txtApiUrl.Text = _connectionstring;
            }

        }

        private void dgvConfig_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1) return;
                string frmName = Convert.ToString(dgvConfig.Rows[e.RowIndex].Cells["clmFormName"].Value);
                _configId = Convert.ToString(dgvConfig.Rows[e.RowIndex].Cells["clmConfigId"].Value);

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
           // txtToken.Text = Convert.ToString(row["token"]);
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
                if (!string.IsNullOrEmpty(_configId))
                {
                    Save();
                    return;
                }
                if (_objSql.IsFormAlreadyExist(txtFormName.Text))
                {
                    CommonData.ShowMessage(
                        "There is already a configuration exist in the database for form " + txtFormName.Text +
                        ". This cant be saved.", CommonData.MsgBoxType.Info);
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
                if (
                    CommonData.ShowMessage(
                        "This action will delete the configuration which you have already created. Do you want to continue?",
                        CommonData.MsgBoxType.Question) == DialogResult.Yes)
                {

                    //var rows = dgvConfig.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
                    // if (rows.Any())
                    var rows = dgvConfig.SelectedRows;                  
                    if (rows.Count > 0)
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
                        while (true)
                        {

                            // if (rows.Any())
                            if (rows.Count > 0)
                            {
                                foreach (DataGridViewRow row in rows)
                                {
                                    dgvConfig.Rows.Remove(row);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLog(ex);
                    }
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
                CommonData.ShowMessage("Success", CommonData.MsgBoxType.Info);
                Close();
            }
            else
            {
                CommonData.ShowMessage("Failed", CommonData.MsgBoxType.Info);
            }
        }
    }
}
