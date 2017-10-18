using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using LogManager;
using REDCapAPI;
using EpiInfoAPI;
using System.Drawing;

namespace Default
{
    public partial class MainScreen : Form
    {
        private readonly Sql _objSql;
        private string _siteOid;
        DatagridViewCheckBoxHeaderCell _cbHeader;      

        public MainScreen()
        {
            InitializeComponent();
            var objSettings = new Settings(Application.StartupPath);
            var cred = objSettings.ReadApiSettings();
            CommonData.Credentials = cred;
            _objSql = new Sql(Application.StartupPath, CommonData.Credentials.ConnectionString);
            FillConfigList();
        }

        private void FillConfigList()
        {
            DataTable dt = _objSql.ReadSettings();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string formName = Convert.ToString(row["form_nm"]);
                    cmbConfigList.Items.Add(formName);
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                dgvOrders.DataSource = null;
                dgvOrders.Columns.Clear();
                if (cmbConfigList.SelectedIndex == -1)
                {
                    CommonData.ShowMessage("Please select the configuration first.", CommonData.MsgBoxType.Error);
                    UpdateStatus("Please select the configuration first.");
                    return;
                }

                UpdateStatus("Please wait.. it is loading....");
                string formName = Convert.ToString(cmbConfigList.SelectedItem);
                DataTable tbl = _objSql.ReadSettings(formName);
               
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        string url = Convert.ToString(row["redcapurl"]);
                        string token = Convert.ToString(row["token"]);
                        string formname = Convert.ToString(row["form_nm"]);
                        string excludeConditions = Convert.ToString(row["exclude_conditions"]);
                        string datasource = Convert.ToString(row["datasource"]);
                        string lastimported= Convert.ToString(row["LastImported"]);

                        _siteOid = Convert.ToString(row["site_oid"]);
                        if (datasource == "Epi Info")
                        {
                          var objClient = new Project(url,false);
                          DataTable datatable=  objClient.GetData();

                            if (!string.IsNullOrEmpty(excludeConditions))
                            {
                                string qry = "condition not in (" + excludeConditions + ")";
                                var rows = datatable.Select(qry);
                                if (rows.Length > 0)
                                {
                                    DataTable newtbl = datatable.Clone();
                                    foreach (DataRow row1 in rows)
                                    {
                                        newtbl.ImportRow(row1);
                                    }                                 
                                    dgvOrders.DataSource = newtbl;                                    
                                     if (!string.IsNullOrEmpty(lastimported))
                                    {
                                        DateTime importdt = Convert.ToDateTime(lastimported);
                                        foreach (DataGridViewRow r in dgvOrders.Rows)
                                        {
                                            DateTime lastsave = Convert.ToDateTime(r.Cells["LastSaveTime"].Value.ToString());
                                            if(lastsave<= importdt)
                                            {
                                                r.DefaultCellStyle.ForeColor = Color.Gray;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {                               
                                dgvOrders.DataSource = datatable;                                
                                 if (!string.IsNullOrEmpty(lastimported))
                                {
                                    DateTime importdt = Convert.ToDateTime(lastimported);
                                    foreach (DataGridViewRow r in dgvOrders.Rows)
                                    {
                                        DateTime lastsave = Convert.ToDateTime(r.Cells["LastSaveTime"].Value.ToString());
                                        if (lastsave <= importdt)
                                        {
                                            r.DefaultCellStyle.ForeColor = Color.Gray;
                                        }
                                    }
                                }
                            }                            
                        }
                        else
                        {
                           var objClient = new RedCapClient();
                            DataTable apitbl = objClient.GetData(url, token, formname);
                            if (!string.IsNullOrEmpty(excludeConditions))
                            {
                                string qry = "condition not in (" + excludeConditions + ")";

                                var rows = apitbl.Select(qry);
                                if (rows.Length > 0)
                                {
                                    DataTable newtbl = apitbl.Clone();
                                    foreach (DataRow row1 in rows)
                                    {
                                        newtbl.ImportRow(row1);
                                    }
                                    newtbl.DefaultView.Sort = "record_id asc";
                                    dgvOrders.DataSource = newtbl;
                                }
                            }
                            else
                            {
                                dgvOrders.DataSource = apitbl;
                            }
                        }
                        InsertCheckBoxColumn();
                        UpdateStatus("Completed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        void SelectAll(bool isSelected)
        {
            for (int i = 0; i < dgvOrders.Rows.Count; i++)
            {
                if (!dgvOrders[0, i].ReadOnly)
                    dgvOrders[0, i].Value = isSelected;
            }
            GetSelectedRowsCount();
        }

        void GetSelectedRowsCount()
        {
            var rows = dgvOrders.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
            if (rows.Any())
            {
                UpdateStatus("Selected records count: " + rows.Count());
                return;
            }
            UpdateStatus("No records are selected.");
        }

        void InsertCheckBoxColumn()
        {
            if (dgvOrders.Columns.Count <= 0) return;
            var colCb = new DataGridViewCheckBoxColumn { Width = 30, Frozen = true };
            _cbHeader = new DatagridViewCheckBoxHeaderCell();
            _cbHeader.OnCheckBoxClicked += cbHeader_OnCheckBoxClicked;
            colCb.HeaderCell = _cbHeader;
            dgvOrders.Columns.Insert(0, colCb);

            var colCb1 = new DataGridViewCheckBoxColumn { Width = 120, Frozen = false };
            var _cbHeader1 = new DataGridViewColumnHeaderCell();
            _cbHeader1.Value = "Create Case";
           // _cbHeader.OnCheckBoxClicked += cbHeader_OnCheckBoxClicked;
            colCb1.HeaderCell = _cbHeader1;
            dgvOrders.Columns.Insert(2, colCb1);
        }

        void cbHeader_OnCheckBoxClicked(bool state)
        {
            SelectAll(state);
        }

        void UpdateStatus(string msg)
        {
            lblStatus.Text = msg;
            Application.DoEvents();
        }

        public static String toStringYesNo(bool bstr)
        {
            return bstr ? "YES" : "NULL";

        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool anyerrors = false;
                if (dgvOrders.Rows.Count > 0)
                {
                    DataTable tbl = _objSql.LoadFieldMappings1(Convert.ToString(cmbConfigList.SelectedItem));                   
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        var rows =
                           dgvOrders.Rows.OfType<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[0].Value));
                        if (rows.Any())
                        {
                            foreach (DataGridViewRow dgrow in rows)
                            {
                                var mapList = new List<Settings.Mappings>();
                                foreach (DataRow drow in tbl.Rows)
                                {
                                    string tblName = Convert.ToString(drow["Table_NM"]);
                                    string columnNm = Convert.ToString(drow["Column_NM"]);
                                    string apiFieldName = Convert.ToString(drow["Source_Fld_NM"]);
                                    string nbsFldNm = Convert.ToString(drow["NBS_Fld_NM"]);
                                    string datasource= Convert.ToString(drow["DataSource"]);
                                    string configid = Convert.ToString(drow["Config_id"]);                                  
                                    string value = Convert.ToString(dgrow.Cells[apiFieldName].Value);
                                    string modvalue = value;
                                    if (tblName == "MSG_PATIENT" && datasource == "Epi Info" && columnNm == "PAT_ETHNIC_GROUP_IND_CD" || columnNm == "PAT_RACE_CATEGORY_CD" && !string.IsNullOrEmpty(value))
                                    {
                                        modvalue = value.Replace(":", "-");
                                    }
                                        if (!string.IsNullOrEmpty(value))
                                    {                                     
                                            var mapping = new Settings.Mappings
                                            {
                                               
                                                TableName = tblName,
                                                ColumnName = columnNm,
                                                ApiValue = modvalue.Replace("_", "-")
                                            };
                                        mapping.NbsFieldName = nbsFldNm;                                       
                                        mapping.RecordId = Convert.ToString(dgrow.Cells["record_id"].Value);
                                        mapping.DocumentId = _siteOid + "^" +
                                                                 Convert.ToString(dgrow.Cells["record_id"].Value);                                        
                                        mapping.DocTypeCd = _siteOid + "^" + DateTime.Now.ToString("yyyy-MM-dd");
                                        mapping.EffectiveTime = DateTime.Now.ToString();
                                        mapping.RecordStatusCd = CommonData.Credentials.RecordStatus;
                                        mapping.RecordStatusTime = DateTime.Now.ToString();
                                        mapping.MsgContainerStartId = CommonData.Credentials.MsgContainerStartId;
                                        mapping.PatLocalId = CommonData.Credentials.PatLocalId;
                                        mapping.InvLocalId = CommonData.Credentials.InvLocalId;
                                        mapping.DataSource = datasource;
                                        mapping.ConfigId = configid;
                                        mapping.Ongoing_case= toStringYesNo(Convert.ToBoolean(dgrow.Cells[2].Value));
                                        mapList.Add(mapping);
                                    }
                                }

                                if (_objSql.InsertApiValues1(mapList))
                                {
                                    string formName = Convert.ToString(cmbConfigList.SelectedItem);
                                    DataTable dt = _objSql.ReadSettings(formName);
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        foreach (DataRow row in dt.Rows)
                                        {                                            
                                            string formname = Convert.ToString(row["form_nm"]);                                          
                                            string datasource = Convert.ToString(row["datasource"]);
                                            string id = Convert.ToString(row["Config_id"]);                                         
                                            if (datasource == "Epi Info")
                                            {
                                                _objSql.UpdateConfig(id);
                                                dgrow.DefaultCellStyle.ForeColor = Color.Gray;                                                                                                                                          
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    anyerrors = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        CommonData.ShowMessage("Not all the selected records were exported successfully. See error log.",
                                                                                             CommonData.MsgBoxType.Error);
                        UpdateStatus("No Mappings creatd. See error log.");
                    }
                }

                if (anyerrors)
                {
                    CommonData.ShowMessage("Not all the selected records were exported successfully. See error log.",
                                                                                              CommonData.MsgBoxType.Error);
                    UpdateStatus("Not all the selected records were exported successfully. See error log.");
                }
                else
                {
                    CommonData.ShowMessage("All the selected records were successfully exported to NBS database.",
                                                         CommonData.MsgBoxType.Info);
                    UpdateStatus("All the selected records were successfully exported to NBS database.");
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog(ex);
            }
        }

        private void cmbConfigList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
