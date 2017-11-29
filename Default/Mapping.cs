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
        private string _datasource;
        readonly List<Settings.Credentials> _configList = new List<Settings.Credentials>();
        List<string> redCapElemList = new List<string>();
        List<string> nbsElemList = new List<string>();
        private const string FileName = @"{0}\Config.xml";
        private static string _configPath = @"{0}\Config.xml";
        List<string> unMappedList = new List<string>();
        public Mapping()
        {
            InitializeComponent();
            var objSettings = new Settings(Application.StartupPath);
            var cred = objSettings.ReadApiSettings();
            CommonData.Credentials = cred;
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
            nbsElemList.Clear();
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
                _datasource = Convert.ToString(dt.Rows[0]["datasource"]);
                if (_datasource == "Epi Info")
                {
                   // var objClient = new Project(url,false);
                    if(unMappedList.Count>0)
                    {
                        redCapElemList = unMappedList;
                    }
                    else
                    {
                        redCapElemList.Clear(); ;
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
            cmbNbsTn.Items.Clear();
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
                    CommonData.ShowMessage("Please select Data Element.", CommonData.MsgBoxType.Error);
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
                if (lbRcDe.Items.Contains(rcde))
                {
                    lbRcDe.Items.Remove(rcde);
                }
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
                    var rows =
                        dgvValues.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
                    if (rows.Any())
                    {
                        foreach (DataGridViewRow row in rows)
                        {
                            dgvValues.Rows.Remove(row);
                        if (!lbRcDe.Items.Contains(row.Cells[2].Value))
                        {
                            lbRcDe.Items.Add(row.Cells[2].Value);
                        }
                    }
                    }              
                     else
                    {
                        CommonData.ShowMessage("Please select atleast one row from the grid.", CommonData.MsgBoxType.Error);
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
                                DataSource = _datasource,
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
                            _datasource = c.DataSource;
                            break;
                        }
                    }
                }
                Reset();
                redCapElemList.Clear(); 
                lbNbsDe.Items.Clear();
                lbRcDe.Items.Clear();              
                FillGrid(formName);
               


                try
                {
                    if (cmbConfigList.SelectedIndex == -1)
                    {
                        CommonData.ShowMessage("Please select configuration first.", CommonData.MsgBoxType.Error);
                        return;
                    }
                    //dgvValues.Rows.Clear();
                    EnableControls(true);                   
                    FillValues(formName);
                    FillMsgQuestions();
                    FillTableNames();                    
                }
                catch (Exception ex)
                {
                    Log.WriteToErrorLog(ex);
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        private void FillGrid(string formName)
        {
            List<string> FldMappingList = new List<string>();
            DataTable tbl = _objSql.LoadFieldMappings(_configId);
            DataTable dt = _objSql.ReadSettings(formName);
            if (dt != null && dt.Rows.Count > 0)
            {
                _datasource = Convert.ToString(dt.Rows[0]["datasource"]);
                string url = Convert.ToString(dt.Rows[0]["redcapurl"]);               
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    if (_datasource == "Epi Info")
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
                        if (_datasource == "Epi Info")
                        {
                            FldMappingList.Add(sourceFieldName);
                        }
                    }
                    if (_datasource == "Epi Info")
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
                else if (tbl != null && tbl.Rows.Count == 0 && _datasource == "Epi Info")
                {
                    FillEpimappings(tbl, url);
                }
            }
        }

        private void FillEpimappings(DataTable tbl,string url)
        {
            List<string> FldMappingList = new List<string>();
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
            foreach (string fieldname in redCapElemList)
            {
                foreach (KeyValuePair<string, string> mapping in settingconfig)
                {
                    if (mapping.Key.ToLower() == fieldname.ToLower().Substring(0, mapping.Key.ToLower().Length))
                    {
                        var map = new Settings.Mappings
                        {
                            ConfigId = _configId,
                            DataSource = _datasource,
                            ApiFieldName = fieldname,
                            NbsFieldName = " ",
                            TableName = mapping.Value,
                            ColumnName = fieldname
                        };
                        mapList.Add(map);
                        break;
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
                var rows = dgvValues.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));                
                       
                    if (rows.Any())
                    {
                    if (
                  CommonData.ShowMessage(
                      "This action will delete the configuration which you have already created. Do you want to continue?",
                      CommonData.MsgBoxType.Question) == DialogResult.Yes)
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

                        try
                        {                                                        
                                    foreach (DataGridViewRow row in rows)
                                    {
                                        dgvValues.Rows.Remove(row);
                                    }                                                         
                        }
                        catch (Exception ex)
                        {
                            Log.WriteToErrorLog(ex);
                        }
                    }
                }
                else
                {
                    CommonData.ShowMessage("Please select atleast one row from the grid.", CommonData.MsgBoxType.Error);
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
                lbNbsDe.Items.Add("Create NBS Data Element..");
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

        void cbHeader_OnCheckBoxClicked(bool state)
        {
            SelectAll(state);
        }

        void SelectAll(bool isSelected)
        {
            for (int i = 0; i < dgvValues.Rows.Count; i++)
            {
                if (!dgvValues[0, i].ReadOnly)
                    dgvValues[0, i].Value = isSelected;
            }
            GetSelectedRowsCount();
        }

        void GetSelectedRowsCount()
        {
            var rows = dgvValues.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
            if (rows.Any())
            {               
                return;
            }
          //  UpdateStatus("No records are selected.");
        }

        private void lbNbsDe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lbNbsDe.SelectedItem.ToString()== "Create NBS Data Element..")
            {
                var objSettings = new CreateLookup();
                objSettings.Datasource = _datasource;
                objSettings.ShowDialog(this);
                FillMsgQuestions();
            }
        }
    }
}
