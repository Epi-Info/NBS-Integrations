using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using LogManager;
using REDCapAPI;
using EpiInfoAPI;
using System.Xml;

namespace Default
{
    public partial class Mapping : Form
    {
        private readonly Sql _objSql;
        private string _configId;
        readonly List<Settings.Credentials> _configList = new List<Settings.Credentials>();
        List<string> redCapElemList = new List<string>();
        List<string> nbsElemList = new List<string>();
        private const string FileName = @"{0}\Config.xml";
        private static string _configPath = @"{0}\Config.xml";
        List<string> unMappedList = new List<string>();
        public Mapping()
        {
            InitializeComponent();
            _objSql = new Sql(Application.StartupPath, CommonData.Credentials.ConnectionString);
            FillConfigList();
            EnableControls(false);
            //FillValues();
            //FillMsgQuestions();
            //FillTableNames();
        }

        private void FillMsgQuestions()
        {
            DataTable dt = _objSql.GetMsgQsLookUps();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string questionIdentifier = Convert.ToString(row["question_identifier"]);
                    string questionDisplayName = Convert.ToString(row["question_display_name"]);
                    string fvalue = questionIdentifier + " - " + questionDisplayName;
                    nbsElemList.Add(fvalue);
                }
            }

            FillNbsDataElemListBox();
        }

        private void FillConfigList()
        {
            DataTable dt = _objSql.ReadSettings();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string formName = Convert.ToString(row["Form_NM"]);
                    string configId = Convert.ToString(row["Config_id"]);
                    var settings = new Settings.Credentials { ConfigId = configId, FormName = formName };
                    _configList.Add(settings);
                    cmbConfigList.Items.Add(formName);
                }
            }
        }

        private void FillValues(string formName)
        {
            DataTable dt = _objSql.ReadSettings(formName);
            if (dt != null && dt.Rows.Count > 0)
            {
                string url = Convert.ToString(dt.Rows[0]["redcapurl"]);
                string token = Convert.ToString(dt.Rows[0]["token"]);
                string formname = Convert.ToString(dt.Rows[0]["Form_NM"]);
                _configId = Convert.ToString(dt.Rows[0]["Config_id"]);
                string datasource = Convert.ToString(dt.Rows[0]["datasource"]);
                if (datasource == "Epi Info")
                {
                    var objClient = new Project(url,false);
                    if(unMappedList.Count>0)
                    {
                        redCapElemList = unMappedList;
                    }
                   // redCapElemList = objClient.GetFields(1);                   
                }
                else
                { 
                RedCapClient api = new RedCapClient();
                DataTable dataTbl = api.GetData(url, token, formname);
                if (dataTbl != null && dataTbl.Rows.Count > 0)
                {
                    foreach (DataColumn clmn in dataTbl.Columns)
                    {
                        string clmName = clmn.ColumnName;
                        redCapElemList.Add(clmName);
                    }
                }
            }
            }

            FillRedCapListBox();
        }

        private void FillTableNames()
        {
            DataTable tblName = _objSql.GetTableNames();
            if (tblName != null && tblName.Rows.Count > 0)
            {
                foreach (DataRow row in tblName.Rows)
                {
                    string tblname = Convert.ToString(row["TABLE_NAME"]);
                    cmbNbsTn.Items.Add(tblname);
                }
            }
        }

        private void cmbNbsTn_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbNbsCn.Items.Clear();
                string tblName = Convert.ToString(cmbNbsTn.SelectedItem);
                DataTable clmNames = _objSql.GetColumnNames(tblName);
                if (clmNames != null && clmNames.Rows.Count > 0)
                {
                    foreach (DataRow row in clmNames.Rows)
                    {
                        string clmname = Convert.ToString(row["COLUMN_NAME"]);
                        cmbNbsCn.Items.Add(clmname);
                    }
                }
            }
            catch (Exception ex)
            {

                Log.WriteToErrorLog(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string rcde = Convert.ToString(lbRcDe.SelectedItem);
                if (string.IsNullOrEmpty(rcde))
                {
                    CommonData.ShowMessage("Please select REDCap Data Element.", CommonData.MsgBoxType.Error);
                    return;
                }

                string nbsde = Convert.ToString(lbNbsDe.SelectedItem);
                if (string.IsNullOrEmpty(nbsde))
                {
                    CommonData.ShowMessage("Please select NBS Data Element.", CommonData.MsgBoxType.Error);
                    return;
                }

                string nbstn = Convert.ToString(cmbNbsTn.SelectedItem);
                if (string.IsNullOrEmpty(nbstn))
                {
                    CommonData.ShowMessage("Please select NBS Table Name.", CommonData.MsgBoxType.Error);
                    return;
                }

                string nbscn = Convert.ToString(cmbNbsCn.SelectedItem);
                if (string.IsNullOrEmpty(nbscn))
                {
                    CommonData.ShowMessage("Please select NBS Column Name.", CommonData.MsgBoxType.Error);
                    return;
                }

                dgvValues.Rows.Add(new object[] { false, "", rcde, nbsde, nbstn, nbscn });
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    CommonData.ShowMessage(
                        "This action will remove all the data you have added. Do you want to continue?",
                        CommonData.MsgBoxType.Question) == DialogResult.Yes)
                {
                    Reset();
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        void Reset()
        {
            lbNbsDe.SelectedIndex = -1;
            lbRcDe.SelectedIndex = -1;
            cmbNbsCn.SelectedIndex = -1;
            cmbNbsTn.SelectedIndex = -1;
            dgvValues.Rows.Clear();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                while (true)
                {
                    var rows =
                        dgvValues.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
                    if (rows.Any())
                    {
                        foreach (DataGridViewRow row in rows)
                        {
                            dgvValues.Rows.Remove(row);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvValues.Rows.Count > 0)
                {
                    var mapList = new List<Settings.Mappings>();
                    foreach (DataGridViewRow row in dgvValues.Rows)
                    {
                        var map = new Settings.Mappings
                            {
                                ConfigId = _configId,
                                DataSource = CommonData.Credentials.DataSource,
                                ApiFieldName = Convert.ToString(row.Cells["clmRcde"].Value),
                                NbsFieldName = Convert.ToString(row.Cells["clmNbsDe"].Value),
                                TableName = Convert.ToString(row.Cells["clmNbsTn"].Value),
                                ColumnName = Convert.ToString(row.Cells["clmNbsCn"].Value)
                            };
                        mapList.Add(map);
                    }
                    if (_objSql.SaveMappings(mapList))
                    {
                        CommonData.ShowMessage("Successfully saved.", CommonData.MsgBoxType.Info);
                    }
                    else
                    {
                        CommonData.ShowMessage("Not successfully saved.", CommonData.MsgBoxType.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        private void cmbConfigList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                dgvValues.Rows.Clear();
                string formName = Convert.ToString(cmbConfigList.SelectedItem);
                if (_configList.Count > 0)
                {
                    foreach (Settings.Credentials c in _configList)
                    {
                        if (c.FormName == formName)
                        {
                            _configId = c.ConfigId;
                            break;
                        }
                    }
                }
                FillGrid(formName);
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        private void FillGrid(string formName)
        {
            string datasource=""; List<string> FldMappingList = new List<string>();
            DataTable tbl = _objSql.LoadFieldMappings(_configId);
            DataTable dt = _objSql.ReadSettings(formName);
            if (dt != null && dt.Rows.Count > 0)
            {
                datasource = Convert.ToString(dt.Rows[0]["datasource"]);
                string url = Convert.ToString(dt.Rows[0]["redcapurl"]);               
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    if (datasource == "Epi Info")
                    {
                        var objClient = new Project(url, false);
                        redCapElemList = objClient.GetFields(1);                      
                    }
                        foreach (DataRow row in tbl.Rows)
                    {
                        string fldMapId = Convert.ToString(row["Fld_mapping_id"]);

                        string sourceFieldName = Convert.ToString(row["Source_Fld_NM "]);
                        string nbsFieldName = Convert.ToString(row["NBS_Fld_NM "]);
                        string tableNm = Convert.ToString(row["Table_NM"]);
                        string columnNm = Convert.ToString(row["Column_NM"]);
                        dgvValues.Rows.Add(new object[] { false, fldMapId, sourceFieldName, nbsFieldName, tableNm, columnNm });
                        if (datasource == "Epi Info")
                        {
                            FldMappingList.Add(sourceFieldName);
                        }
                    }
                    if (datasource == "Epi Info")
                    {
                        foreach (string fieldnm in redCapElemList)
                        {
                            if (!(FldMappingList.Contains(fieldnm)))
                            {
                                unMappedList.Add(fieldnm);
                            }
                        }
                    }
                }
                else if (tbl != null && tbl.Rows.Count == 0 && datasource == "Epi Info")
                {
                    var objClient = new Project(url, false);
                    redCapElemList = objClient.GetFields(1);
                    var xmlDoc = new XmlDocument();
                    _configPath = string.Format(FileName, Application.StartupPath);
                    xmlDoc.Load(_configPath);
                    XmlNode selectedNode = xmlDoc.SelectSingleNode("/Settings/TableNames");
                    Dictionary<string, string> settingconfig = new Dictionary<string, string>();// <prefix,tablename>
                    var mapList = new List<Settings.Mappings>();
                    foreach (XmlNode childnode in selectedNode.ChildNodes)
                    {
                        settingconfig.Add(childnode.Attributes[0].Value, childnode.InnerText);
                    }
                          foreach(string fieldname in redCapElemList)
                    {
                        foreach(KeyValuePair<string,string> mapping in settingconfig)
                        {
                            if(mapping.Key.ToLower()== fieldname.ToLower().Substring(0,mapping.Key.ToLower().Length))
                            {                                
                                var map = new Settings.Mappings
                                {
                                    ConfigId = _configId,
                                    DataSource = "Epi Info",
                                    ApiFieldName = fieldname,
                                    NbsFieldName = " ",
                                    TableName = mapping.Value,
                                    ColumnName = fieldname
                                };
                                mapList.Add(map);
                            }
                        }
                    }
                    if (mapList.Count > 0)
                    {
                        _objSql.SaveMappings(mapList);
                        tbl = _objSql.LoadFieldMappings(_configId);
                        if (tbl != null && tbl.Rows.Count > 0)
                        {
                            foreach (DataRow row in tbl.Rows)
                            {
                                string fldMapId = Convert.ToString(row["Fld_mapping_id"]);
                                string sourceFieldName = Convert.ToString(row["Source_Fld_NM "]);
                                string nbsFieldName = Convert.ToString(row["NBS_Fld_NM "]);
                                string tableNm = Convert.ToString(row["Table_NM"]);
                                string columnNm = Convert.ToString(row["Column_NM"]);
                                dgvValues.Rows.Add(new object[] { false, fldMapId, sourceFieldName, nbsFieldName, tableNm, columnNm });
                                FldMappingList.Add(sourceFieldName);
                            }
                        }
                        foreach (string fieldnm in redCapElemList)
                        {
                            if (!(FldMappingList.Contains(fieldnm)))
                            {
                                unMappedList.Add(fieldnm);
                            }
                        }
                    }
                }
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbConfigList.SelectedIndex == -1)
                {
                    CommonData.ShowMessage("Please select configuration first.", CommonData.MsgBoxType.Error);
                    return;
                }
                //dgvValues.Rows.Clear();
                EnableControls(true);

                string formName = Convert.ToString(cmbConfigList.SelectedItem);
                FillValues(formName);
                FillMsgQuestions();
                FillTableNames();
                btnAddNew.Enabled = false;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        void EnableControls(bool flag)
        {
            lbRcDe.Enabled = flag;
            lbNbsDe.Enabled = flag;
            cmbNbsTn.Enabled = flag;
            cmbNbsCn.Enabled = flag;
        }

        private void btnDelFromDb_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    CommonData.ShowMessage(
                        "This action will delete the configuration which you have already created. Do you want to continue?",
                        CommonData.MsgBoxType.Question) == DialogResult.Yes)
                {

                    var rows = dgvValues.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
                    if (rows.Any())
                    {
                        var fldMapIds = new List<string>();
                        foreach (DataGridViewRow row in rows)
                        {
                            string fldId = Convert.ToString(row.Cells["clmFldMapId"].Value);
                            fldMapIds.Add(fldId);
                        }

                        if (fldMapIds.Count > 0)
                        {
                            if (_objSql.DeleteMappings(fldMapIds))
                            {

                            }
                        }
                    }

                    try
                    {
                        while (true)
                        {

                            if (rows.Any())
                            {
                                foreach (DataGridViewRow row in rows)
                                {
                                    dgvValues.Rows.Remove(row);
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

        private void txtRedSearch_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtRedSearch.Text == "Type here to search...")
            {
                txtRedSearch.Text = "";
            }
        }

        private void txtRedSearch_MouseLeave(object sender, EventArgs e)
        {
            if (txtRedSearch.Text == "")
            {
                txtRedSearch.Text = "Type here to search...";
            }
        }

        void FillRedCapListBox()
        {
            lbRcDe.Items.Clear();

            if (redCapElemList.Count > 0)
            {
                foreach (var redElem in redCapElemList)
                {
                    lbRcDe.Items.Add(redElem);
                }
            }
        }

        private void txtRedSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtRedSearch.Text == "Type here to search...")
            {
                FillRedCapListBox();
                return;
            }
            if (txtRedSearch.Text == "")
            {
                FillRedCapListBox();
                return;
            }
            if (redCapElemList.Count > 0)
            {
                lbRcDe.Items.Clear();
                foreach (string redCapElem in redCapElemList)
                {
                    if (redCapElem.ToUpper().Contains(txtRedSearch.Text.ToUpper()))
                    {
                        lbRcDe.Items.Add(redCapElem);
                    }
                }
            }
        }

        private void txtDataElemSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtDataElemSearch.Text == "Type here to search...")
            {
                FillNbsDataElemListBox();
                return;
            }
            if (txtDataElemSearch.Text == "")
            {
                FillNbsDataElemListBox();
                return;
            }
            if (nbsElemList.Count > 0)
            {
                lbNbsDe.Items.Clear();
                foreach (string nbsElem in nbsElemList)
                {
                    if (nbsElem.ToUpper().Contains(txtDataElemSearch.Text.ToUpper()))
                    {
                        lbNbsDe.Items.Add(nbsElem);
                    }
                }
            }
        }

        void FillNbsDataElemListBox()
        {
            lbNbsDe.Items.Clear();

            if (nbsElemList.Count > 0)
            {
                foreach (var nbsElem in nbsElemList)
                {
                    lbNbsDe.Items.Add(nbsElem);
                }
            }
        }

        private void txtDataElemSearch_MouseLeave(object sender, EventArgs e)
        {
            if (txtDataElemSearch.Text == "")
            {
                txtDataElemSearch.Text = "Type here to search...";
            }
        }

        private void txtDataElemSearch_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtDataElemSearch.Text == "Type here to search...")
            {
                txtDataElemSearch.Text = "";
            }
        }
    }
}
