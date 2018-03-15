using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LogManager;
using System.Data.SqlTypes;
using System.Resources;

namespace REDCapAPI
{
    public class Sql
    {
        private const string FileName = @"{0}\Config.xml";
        private SqlConnection _connection;
        public readonly string _connectionString;// = "server={0};database={1};User Id={2};Password={3};";

        //private const string ConnectionString1 = "Server={0};Database={1};Trusted_Connection=True;";

        private static string _configPath = @"{0}\Config.xml";
        private static SqlCredentials _credentials;
        private string MsgID = "";
        private string locId = "";
        private string prid = "";
        private static string migrationstatus = "";
        private static string partcipation_cd_def = "";
        private static string msg_iden_def = "";

        public Sql(string appPath, string connString)
        {
            _configPath = string.Format(FileName, appPath);
            _credentials = ReadSqlSettings();
            _connectionString = connString;
            //throw new NotImplementedException();
        }

        private struct SqlCredentials
        {
            public string Server { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string DataBase { get; set; }
        }

        private static SqlCredentials ReadSqlSettings()
        {
            var credentials = new SqlCredentials();
            try
            {
                if (!File.Exists(_configPath))
                {
                    return credentials;
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(_configPath);

                XmlNode selectedNode = xmlDoc.SelectSingleNode("/Settings/Sql/Database");
                if (selectedNode != null)
                    credentials.DataBase = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/Sql/Username");
                if (selectedNode != null)
                    credentials.UserName = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/Sql/Password");
                if (selectedNode != null)
                    credentials.Password = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/Sql/Server");
                if (selectedNode != null)
                    credentials.Server = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/DATA_MIGRATION_STATUS");
                if (selectedNode != null)
                    migrationstatus = selectedNode.InnerText;
                else
                    migrationstatus = "-1";

                selectedNode = xmlDoc.SelectSingleNode("/Settings/PARTICIPATION_CD");
                if (selectedNode != null)
                    partcipation_cd_def = selectedNode.InnerText;
                else
                    partcipation_cd_def = "SubjOfPHC";

                selectedNode = xmlDoc.SelectSingleNode("/Settings/QUESTION_IDENTIFIER");
                if (selectedNode != null)
                    msg_iden_def = selectedNode.InnerText;
                else
                    msg_iden_def = "INV180";
            }
            catch (Exception exception)
            {
                throw;
            }
            return credentials;
        }
        
        public bool IsOrderNumberExist(string orderno)
        {
            try
            {
                if (OpenConnection())
                {
                    var myCommand = new SqlCommand(string.Format("select OrderID from [Order] where OrderNumber = {0}", orderno), _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt.Rows.Count > 0;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
                Log.WriteToErrorLog(orderno);
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public DataTable GetTableNames()
        {
            try
            {
                if (OpenConnection())
                {
                    string qry =
                        "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='NBS_MSGOUTE' "+
                      " And TABLE_NAME IN ('MSG_ANSWER','MSG_CASE_INVESTIGATION','MSG_INTERVIEW','MSG_ORGANIZATION','MSG_PATIENT','MSG_PLACE','MSG_PROVIDER','MSG_TREATMENT' )";
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }

        public DataTable GetColumnNames(string tblName)
        {
            try
            {
                if (OpenConnection())
                {
                    string qry =
                        string.Format("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='{0}'",
                                      tblName);
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }

        public DataTable GetMsgQsLookUps()
        {
            try
            {
                if (OpenConnection())
                {
                    string qry =
                        string.Format("select distinct question_identifier,question_display_name from MSG_QUESTION_LOOKUP where question_display_name is not null ");
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
              //  CloseConnection();
            }
            return null;
        }

        public bool SaveConfig(Settings.Credentials settings)
        {
            try
            {
                if (OpenConnection())
                {
                    SqlCommand com = new SqlCommand();
                    com.Connection = _connection;
                    com.CommandType = CommandType.Text;
                    string qry = string.Format(
                        "insert into Source_Config(DataSource, REDCapUrl, Token, Form_NM, Author_ID, Custodian_ID, SITE_OID,Exclude_Conditions)" +
                        "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                        settings.DataSource, settings.ApiUrl, settings.Token, settings.FormName, settings.AuthorId,
                        settings.CustodianId,
                        settings.StateId, settings.Exclude);

                    if (!string.IsNullOrEmpty(settings.ConfigId))
                    {
                        qry =
                            string.Format(
                                "update Source_Config set DataSource='{0}', REDCapUrl='{1}', Token='{2}', Form_NM='{3}', Author_ID='{4}', Custodian_ID='{5}', SITE_OID='{6}',Exclude_Conditions='{7}' where Config_id ='{8}'",
                                settings.DataSource, settings.ApiUrl, settings.Token, settings.FormName,
                                settings.AuthorId,
                                settings.CustodianId,
                                settings.StateId, settings.Exclude, settings.ConfigId);
                    }
                    com.CommandText = qry;
                    com.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public bool UpdateConfig(string configid)
        {
            try
            {
                if (OpenConnection())
                {
                    SqlCommand com = new SqlCommand();
                    com.Connection = _connection;
                    com.CommandType = CommandType.Text;
                    string qry = "";
                    if (!string.IsNullOrEmpty(configid))
                    {
                         qry =
                            string.Format(
                                "update Source_Config set LastImported ='"+System.DateTime.Now+"' where Config_id ='{0}'",
                                configid);
                    }
                    com.CommandText = qry;
                    com.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public bool SaveMappings(IEnumerable<Settings.Mappings> mappings)
        {
            bool errorsOccured = false;
            try
            {
                if (OpenConnection())
                {
                    string qry = "";
                    try
                    {
                        foreach (Settings.Mappings map in mappings)
                        {
                            var com = new SqlCommand();
                            com.Connection = _connection;
                            com.CommandType = CommandType.Text;
                            qry = string.Format("select * from FLD_MAPPING where Config_id='{0}' and Source_Fld_NM='{1}'", map.ConfigId, map.ApiFieldName);
                            com.CommandText = qry;
                            var adapter = new SqlDataAdapter(com);
                            var dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count == 0)
                            {
                                qry = string.Format(
                                     "insert into FLD_MAPPING(Config_id,DataSource, Source_Fld_NM, NBS_Fld_NM, Column_NM, Table_NM) " +
                                     "values('{0}','{1}','{2}','{3}','{4}','{5}')",
                                     map.ConfigId, map.DataSource, map.ApiFieldName, map.NbsFieldName,
                                     map.ColumnName,
                                     map.TableName);
                            }
                            else
                            {
                                qry = string.Format(
                                     "update FLD_MAPPING set  NBS_Fld_NM = '{1}', Column_NM='{2}', Table_NM='{3}' where Config_id='{4}' and Source_Fld_NM='{0}' ", map.ApiFieldName, map.NbsFieldName,
                                     map.ColumnName,
                                     map.TableName, map.ConfigId);
                            }
                            com.CommandText = qry;
                            com.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLog("Error in qry: " + qry);
                        Log.WriteToErrorLog(ex);
                        errorsOccured = true;
                    }
                }

                if (!errorsOccured)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public DataTable GetShipments(string fromDate, string toDate)
        {
            try
            {
                //string qry =string.Format("SELECT s.ShipmentID, o.OrderNumber,s.ShipmentType,s.ContentWeight,s.TotalWeight,s.Processed,s.ProcessedDate,s.ShipDate,s.ShipmentCost,s.TrackingNumber FROM Shipment s inner join [Order] o on o.OrderID = s.OrderID where ShipDate >= '{0}' and ShipDate <= '{1}'",fromDate, toDate);
                if (OpenConnection())
                {
                    string qry = string.Format("SELECT s.ShipmentID,o.StoreID, o.OrderNumber,s.ShipmentType,s.ContentWeight,s.TotalWeight,s.Processed,s.ProcessedDate,s.ShipDate,s.ShipmentCost,s.TrackingNumber,sp.Name as ShipmentName FROM Shipment s inner join [Order] o on o.OrderID = s.OrderID inner join ShippingProfile sp on sp.ShipmentType = s.ShipmentType where ShipDate >= '{0}' and ShipDate <= '{1}' and o.LocalStatus = 'Shipped'", fromDate, toDate);
                    LogManager.Log.WriteToApplicationLog(qry);
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }

        public DataTable ReadSettings()
        {
            try
            {
                if (OpenConnection())
                {
                    string qry = string.Format("SELECT * from [Source_Config]");
                    Log.WriteToApplicationLog(qry);
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }

        public DataTable ReadRedCapSettings()
        {
            try
            {
                if (OpenConnection())
                {
                    string qry = string.Format("SELECT * from [Source_Config] where DataSource <> 'Epi Info' ");
                    Log.WriteToApplicationLog(qry);
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }


        public DataTable ReadEpiSettings()
        {
            try
            {
                if (OpenConnection())
                {
                    string qry = string.Format("SELECT * from [Source_Config] where DataSource ='Epi Info'");
                    Log.WriteToApplicationLog(qry);
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }

        public DataTable ReadSettings(string formName)
        {
            try
            {
                if (OpenConnection())
                {
                    string qry = string.Format("SELECT * from [Source_Config] where Form_NM = '{0}'", formName);
                    Log.WriteToApplicationLog(qry);
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }


        bool OpenConnection()
        {
            //if (string.IsNullOrEmpty(_credentials.UserName) && string.IsNullOrEmpty(_credentials.Password))
            //{
            //    _connection = new SqlConnection(string.Format(ConnectionString1, _credentials.Server, _credentials.DataBase));
            //}
            //else
            //{
            //    _connection = new SqlConnection(string.Format(_connectionString, _credentials.Server, _credentials.DataBase,
            //                                        _credentials.UserName, _credentials.Password));
            //}
            if (!string.IsNullOrEmpty(_connectionString))
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
                return true;
            }
            else
            {
               string  error = "Please update connection";
                Log.WriteToErrorLog(error);
                return false;
            }
        }

        bool CloseConnection()
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                _connection.Close();
                return true;
            }
            else{
                string error = "Please update connection";
                Log.WriteToErrorLog(error);
                return false;
            }
        }

        public bool TestConnection(out string error)
        {
            error = "";
            try
            {
                if (OpenConnection())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Log.WriteToErrorLog(ex);
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public bool DeleteMappings(IEnumerable<String> mappingIds)
        {
            bool errorsOccured = false;
            try
            {
                if (OpenConnection())
                {
                    string qry = "";
                    foreach (string mapId in mappingIds)
                    {
                        try
                        {
                            var com = new SqlCommand();
                            com.Connection = _connection;
                            com.CommandType = CommandType.Text;
                            qry = string.Format("Delete from FLD_MAPPING where Fld_mapping_id = '{0}'", mapId);
                            com.CommandText = qry;
                            com.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Log.WriteToErrorLog("Error in qry: " + qry);
                            Log.WriteToErrorLog(ex);
                            errorsOccured = true;
                        }
                    }
                }

                if (!errorsOccured)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public bool DeleteConfiguration(IEnumerable<String> configIds)
        {
            bool errorsOccured = false;
            try
            {
                if (OpenConnection())
                {
                    string qry = "";
                    try
                    {
                        foreach (string configid in configIds)
                        {
                            var com = new SqlCommand();
                            com.Connection = _connection;
                            com.CommandType = CommandType.Text;
                            qry = string.Format("Delete from Source_Config where config_id = '{0}'", configid);
                            com.CommandText = qry;
                            com.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLog("Error in qry: " + qry);
                        Log.WriteToErrorLog(ex);
                        errorsOccured = true;
                    }
                }

                if (!errorsOccured)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public bool IsFormAlreadyExist(string formName)
        {
            try
            {
                if (OpenConnection())
                {
                    string qry = string.Format("SELECT * from [Source_Config] where Form_NM = '{0}'", formName);
                    Log.WriteToApplicationLog(qry);
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public DataTable LoadFieldMappings(string configId)
        {
            try
            {
                if (OpenConnection())
                {
                    string qry = string.Format("SELECT * from [FLD_MAPPING] where Config_Id = '{0}'  order by [Source_Fld_NM ]", configId);
                    Log.WriteToApplicationLog(qry);
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }

        public DataTable LoadFieldMappings1(string formName)
        {
            try
            {
                if (OpenConnection())
                {
                    string qry = string.Format("select fm.Config_id,fm.Source_Fld_NM,fm.DataSource,fm.Table_NM,fm.Column_NM,fm.NBS_Fld_NM from FLD_MAPPING fm inner join Source_Config rc on rc.Config_id = fm.Config_id where rc.Form_NM = '{0}'", formName);
                    Log.WriteToApplicationLog(qry);
                    var myCommand = new SqlCommand(qry, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }

        string GetMaxMsgId()
        {
            try
            {
                string qry = string.Format("select max(msg_container_uid) from [MSG_CONTAINER]");
                Log.WriteToApplicationLog(qry);
                var myCommand = new SqlCommand(qry, _connection);
                var adapter = new SqlDataAdapter(myCommand);
                var dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string msgId = Convert.ToString(dt.Rows[0][0]);
                    if (!string.IsNullOrEmpty(msgId))
                    {
                        msgId = (Convert.ToInt32(msgId) + 1).ToString();
                        return msgId;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return null;
        }

        string GetMaxInvestigationLocId(bool increment)
        {
            try
            {
                string qry = string.Format("select max(INV_LOCAL_ID) from [MSG_CASE_INVESTIGATION]");
                Log.WriteToApplicationLog(qry);
                var myCommand = new SqlCommand(qry, _connection);
                var adapter = new SqlDataAdapter(myCommand);
                var dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string patLocalId = Convert.ToString(dt.Rows[0][0]);
                    if (!string.IsNullOrEmpty(patLocalId))
                    {
                        if (increment)
                        {
                            patLocalId = (Convert.ToInt32(patLocalId) + 1).ToString();
                        }
                        return patLocalId;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return null;
        }


        string GetMaxPatientId(bool increment)
        {
            try
            {
                string qry = string.Format("select max(PAT_LOCAL_ID) from [MSG_PATIENT]");
                Log.WriteToApplicationLog(qry);
                var myCommand = new SqlCommand(qry, _connection);
                var adapter = new SqlDataAdapter(myCommand);
                var dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string patLocalId = Convert.ToString(dt.Rows[0][0]);
                    if (!string.IsNullOrEmpty(patLocalId))
                    {
                        if (increment)
                        {
                            patLocalId = (Convert.ToInt32(patLocalId) + 1).ToString();
                        }
                        return patLocalId;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return null;
        }


        bool InsertMsgContainerId(Settings.Mappings map, out string msgId)
        {
            msgId = GetMaxMsgId();
            if (string.IsNullOrEmpty(msgId))
            {
                msgId = map.MsgContainerStartId;
            }
            bool errorsOccured = false;
            string qry = "";
            try
            {
                var com = new SqlCommand();
                com.Connection = _connection;
                com.CommandType = CommandType.Text;
                qry =
                    string.Format(
                        "insert into [MSG_CONTAINER](MSG_CONTAINER_UID,DOCUMENT_ID,DOC_TYPE_CD,EFFECTIVE_TIME,RECORD_STATUS_CD,RECORD_STATUS_TIME,RECEIVING_SYSTEM ,DATA_MIGRATION_STATUS,ONGOING_CASE ,MSG_LOCAL_ID) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                        msgId, map.DocumentId, map.DocTypeCd, map.EffectiveTime, map.RecordStatusCd,
                        map.RecordStatusTime,map.DataSource, migrationstatus, map.Ongoing_case,locId);
                com.CommandText = qry;
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Error in qry: " + qry);
                Log.WriteToErrorLog(ex);
                errorsOccured = true;
            }
            if (!errorsOccured)
            {
                return true;
            }
            return false;
        }

        bool UpdateMsgContainerId(Settings.Mappings map,  string msgId)
        {          
            bool errorsOccured = false;
            string qry = "";
            try
            {
                var com = new SqlCommand();
                com.Connection = _connection;
                com.CommandType = CommandType.Text;
                qry =
                    string.Format(
                        "update [MSG_CONTAINER]  set ONGOING_CASE='{0}',EFFECTIVE_TIME='{2}',RECORD_STATUS_TIME='{3}',DATA_MIGRATION_STATUS='{4}'  where MSG_CONTAINER_UID='{1}'",
                       map.Ongoing_case, msgId,map.EffectiveTime,map.RecordStatusTime, migrationstatus);
                com.CommandText = qry;
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Error in qry: " + qry);
                Log.WriteToErrorLog(ex);
                errorsOccured = true;
            }
            if (!errorsOccured)
            {
                return true;
            }
            return false;
        }

        public bool InsertApiValues1(IEnumerable<Settings.Mappings> mapList)
        {
            bool errorsOccured = false;
            try
            {
                MsgID = null;locId = null;prid = null;
                if (OpenConnection())
                {
                    string qry = "";
                    try
                    {
                        string msgId;
                        List<string> tableNames = mapList.Select(m => m.TableName).Distinct().ToList();
                        //  if (InsertMsgContainerId(mapList.First(), out msgId))
                        // {
                        //List<string> tableNames = mapList.Select(m => m.TableName).Distinct().ToList();

                        foreach (string tableName in tableNames)
                        {
                            var maps = mapList.Where(m => m.TableName == tableName);
                            var clmBuilder = new StringBuilder();
                            var valBuilder = new StringBuilder();
                            string datasource = "";string configid = "";
                            foreach (var m in maps)
                            {
                                clmBuilder.Append(m.ColumnName).Append(",");
                                valBuilder.Append("'").Append(m.ApiValue).Append("'").Append(",");
                                datasource = m.DataSource;
                                configid = m.ConfigId;                                
                            }
                            string clmNames = clmBuilder.ToString();
                            if (clmNames.EndsWith(","))
                            {
                                clmNames = clmNames.Remove(clmNames.Length - 1, 1);
                            }

                            string clmValues = valBuilder.ToString();
                            if (clmValues.EndsWith(","))
                            {
                                clmValues = clmValues.Remove(clmValues.Length - 1, 1);
                            }
                            string recId = mapList.First().RecordId;

                            var com = new SqlCommand();
                            com.Connection = _connection;
                            com.CommandType = CommandType.Text;
                            string query ;
                            if (datasource == "Epi Info")
                            {
                                locId = "EpiInfo_" + configid + "_" + recId;
                            }
                            else
                            {
                                locId = "REDCap_" + configid + "_" + recId;
                            }
                            switch (tableName)
                            {
                                case "MSG_CASE_INVESTIGATION":
                                    {
                                        try
                                        {
                                            qry = Get_MSG_CASE_INVESTIGATION_Qry1(recId, tableName, clmNames, clmValues, datasource, configid);
                                            query = qry;
                                            if (qry.StartsWith("insert") && string.IsNullOrEmpty(MsgID))
                                            {
                                                InsertMsgContainerId(mapList.First(), out msgId);
                                                MsgID = msgId;
                                                query = qry.Replace("values('", "values('" + MsgID);
                                            }
                                            else
                                            {
                                                UpdateMsgContainerId(mapList.First(), MsgID);
                                            }
                                            com.CommandText = query;
                                            com.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteToErrorLog("Error in qry: " + qry);
                                            Log.WriteToErrorLog(ex);
                                        }
                                    }
                                    break;
                                case "MSG_PATIENT":
                                    {
                                        //string patId = GetMaxPatientId(true);
                                        //if (string.IsNullOrEmpty(patId))
                                        //{
                                        //    patId = mapList.First().PatLocalId;
                                        //}
                                        //qry = Get_MSG_PATIENT_Qry(tableName, clmNames, msgId, patId, clmValues);                                      
                                        qry = Get_MSG_PATIENT_Qry1(tableName, clmNames, recId, clmValues, datasource, configid);
                                        query = qry;
                                        if (qry.StartsWith("insert") && string.IsNullOrEmpty(MsgID))
                                        {
                                            InsertMsgContainerId(mapList.First(), out msgId);
                                            MsgID = msgId;
                                            query = qry.Replace("values('", "values('" + MsgID);
                                        }
                                        else
                                        {
                                            UpdateMsgContainerId(mapList.First(), MsgID);
                                        }
                                        try
                                        {
                                            com.CommandText = query;
                                            com.ExecuteNonQuery();
                                        }
                                        catch(Exception ex)
                                        {
                                            Log.WriteToErrorLog("Error in qry: " + qry);
                                            Log.WriteToErrorLog(ex);
                                        }
                                    }
                                    break;
                                case "MSG_ORGANIZATION":
                                    {
                                        try
                                        {
                                            qry = Get_MSG_ORGANIZATION_Qry1(tableName, clmNames, recId, "", clmValues, datasource, configid);
                                            query = qry;
                                            if (qry.StartsWith("insert") && string.IsNullOrEmpty(MsgID))
                                            {
                                                InsertMsgContainerId(mapList.First(), out msgId);
                                                MsgID = msgId;
                                                query = qry.Replace("values('", "values('" + MsgID);
                                            }
                                            else
                                            {
                                                UpdateMsgContainerId(mapList.First(), MsgID);
                                            }
                                            com.CommandText = query;
                                            com.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteToErrorLog("Error in qry: " + qry);
                                            Log.WriteToErrorLog(ex);
                                        }
                                    }
                                    break;
                                case "MSG_PLACE":
                                    {
                                        try
                                        {
                                            qry = Get_MSG_PLACE_Qry1(tableName, clmNames, recId, "", clmValues, datasource, configid);
                                            query = qry;
                                            if (qry.StartsWith("insert") && string.IsNullOrEmpty(MsgID))
                                            {
                                                InsertMsgContainerId(mapList.First(), out msgId);
                                                MsgID = msgId;
                                                query = qry.Replace("values('", "values('" + MsgID);
                                            }
                                            else
                                            {
                                                UpdateMsgContainerId(mapList.First(), MsgID);
                                            }
                                            com.CommandText = query;
                                            com.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteToErrorLog("Error in qry: " + qry);
                                            Log.WriteToErrorLog(ex);
                                        }
                                    }
                                    break;
                                case "MSG_PROVIDER":
                                    {
                                        try
                                        {
                                            qry = Get_MSG_PROVIDER_Qry1(tableName, clmNames, recId, "", clmValues, datasource, configid);
                                            query = qry;
                                            if (qry.StartsWith("insert") && string.IsNullOrEmpty(MsgID))
                                            {
                                                InsertMsgContainerId(mapList.First(), out msgId);
                                                MsgID = msgId;
                                                query = qry.Replace("values('", "values('" + MsgID);
                                            }
                                            else
                                            {
                                                UpdateMsgContainerId(mapList.First(), MsgID);
                                            }
                                            com.CommandText = query;
                                            com.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteToErrorLog("Error in qry: " + qry);
                                            Log.WriteToErrorLog(ex);
                                        }
                                    }
                                    break;
                                case "MSG_TREATMENT":
                                    {
                                        try
                                        {
                                            qry = Get_MSG_TREATMENT_Qry1(tableName, clmNames, recId, "", clmValues, datasource, configid);
                                            query = qry;
                                            if (qry.StartsWith("insert") && string.IsNullOrEmpty(MsgID))
                                            {
                                                InsertMsgContainerId(mapList.First(), out msgId);
                                                MsgID = msgId;
                                                query = qry.Replace("values('", "values('" + MsgID);
                                            }
                                            else
                                            {
                                                UpdateMsgContainerId(mapList.First(), MsgID);
                                            }
                                            com.CommandText = query;
                                            com.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteToErrorLog("Error in qry: " + qry);
                                            Log.WriteToErrorLog(ex);
                                        }
                                    }
                                    break;
                                case "MSG_INTERVIEW":
                                    {
                                        try
                                        {
                                            qry = Get_MSG_INTERVIEW_Qry1(recId, tableName, clmNames, clmValues, datasource, configid);
                                            query = qry;
                                            if (qry.StartsWith("insert") && string.IsNullOrEmpty(MsgID))
                                            {
                                                InsertMsgContainerId(mapList.First(), out msgId);
                                                MsgID = msgId;
                                                query = qry.Replace("values('", "values('" + MsgID);
                                            }
                                            else
                                            {
                                                UpdateMsgContainerId(mapList.First(), MsgID);
                                            }
                                            com.CommandText = query;
                                            com.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteToErrorLog("Error in qry: " + qry);
                                            Log.WriteToErrorLog(ex);
                                        }
                                    }
                                    break;
                                case "MSG_ANSWER":
                                    {
                                        try
                                        {
                                            foreach (Settings.Mappings map in maps)
                                            {
                                                string questionIdentifier = map.NbsFieldName;
                                                string questiondisplayname="";
                                                if (map.NbsFieldName.Contains("-"))
                                                {
                                                    questionIdentifier = map.NbsFieldName.Split('-')[0].Trim();
                                                    questiondisplayname= map.NbsFieldName.Split('-')[1].Trim();
                                                }

                                                DataTable dattable = Get_MSG_Lookups(questionIdentifier, questiondisplayname);
                                                string quescode=null, quescodedesc=null, anscode=null, anscodedesc=null, ansdisptxt = null;
                                                if (dattable.Rows.Count > 0)
                                                {                                                    

                                                    quescode = dattable.Rows[0]["QUES_CODE_SYSTEM_CD"].ToString();
                                                    quescodedesc = dattable.Rows[0]["QUES_CODE_SYSTEM_DESC_TXT"].ToString();
                                                    if (!string.IsNullOrEmpty(dattable.Rows[0]["ANS_FROM_CODE_SYSTEM_CD"].ToString()))
                                                        anscode = dattable.Rows[0]["ANS_FROM_CODE_SYSTEM_CD"].ToString();
                                                    if (!string.IsNullOrEmpty(dattable.Rows[0]["ANS_FROM_CODE_SYSTEM_DESC_TXT"].ToString()))
                                                        anscodedesc = dattable.Rows[0]["ANS_FROM_CODE_SYSTEM_DESC_TXT"].ToString();
                                                    if(!string.IsNullOrEmpty(dattable.Rows[0]["ANS_FROM_DISPLAY_NM"].ToString()))
                                                    ansdisptxt = dattable.Rows[0]["ANS_FROM_DISPLAY_NM"].ToString();

                                                    if (dattable.Rows[0]["DATA_TYPE"].ToString() == "CODED")
                                                    {
                                                        DataTable dt = Get_MSG_Ans_Lookup(questionIdentifier, map.ApiValue);
                                                        if (dt.Rows.Count == 0)
                                                        {
                                                            var lookup = new Settings.QuestioLookup
                                                            {

                                                                DocTypeId = "PHDC",
                                                                DocTypeVersionTxt = "1.3",
                                                                QuesCodeSysCD = quescode,
                                                                QuesCodeSysDescTxt = quescodedesc,
                                                                Data_Type = "CODED",
                                                                QuesIdentifier = questionIdentifier,
                                                                QuesDisplayName = questiondisplayname,
                                                                SendingSysCD = map.DataSource,

                                                                AnsFromCode = map.ApiValue.Trim(),
                                                                AnsFromCodeSysCD = anscode,
                                                                AnsFromCodeSysDecsTxt = anscodedesc,
                                                                AnsFromDisNM = ansdisptxt,
                                                                CodeTransReq = "NO",
                                                                InvestigationFormCd = "INV_FORM_GEN"
                                                            };

                                                            SaveAnslookup(lookup);
                                                        }
                                                    }
                                                }

                                                qry = Get_MSG_ANSWER_Qry1(questionIdentifier, recId, map.ApiValue, map.DataSource,configid,quescode,quescodedesc,anscode,anscodedesc,ansdisptxt, questiondisplayname);
                                                query = qry;
                                                if (qry.StartsWith("insert") && string.IsNullOrEmpty(MsgID))
                                                {
                                                    InsertMsgContainerId(mapList.First(), out msgId);
                                                    MsgID = msgId;
                                                    query = qry.Replace("values(", "values(" + MsgID);
                                                }
                                                else
                                                {
                                                    UpdateMsgContainerId(mapList.First(), MsgID);
                                                }
                                                com.CommandText = query;
                                                com.ExecuteNonQuery();                                               
                                            }                                          
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteToErrorLog("Error in qry: " + qry);
                                            Log.WriteToErrorLog(ex);                                            
                                        }
                                    }
                                    break;
                            }
                                                 
                        }                        
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLog("Error in qry: " + qry);
                        Log.WriteToErrorLog(ex);
                        errorsOccured = true;
                    }                    

                    if (!errorsOccured)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }      

        string Get_MSG_CASE_INVESTIGATION_Qry1(string recId, string tableName, string clmNames,  string clmValues, string datasource,string configid)
        {
            //string patId = GetMaxPatientId(false);           
            string  patId = "";
            if (datasource == "Epi Info")
            {            
                patId = "EpiInfo_PAT_" + configid + "_" + recId;
            }
            else
            {                
                patId = "REDCap_PAT_" + configid + "_" + recId;
            }
            string qry = "";
            qry = string.Format(
              "select top 1 * from MSG_CASE_INVESTIGATION where inv_local_id='{0}'",
              locId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where inv_local_id = '{0}'", locId);
                qry = GetUpdateQuery(tableName, clmNames, locId, clmValues) + wherecond;

            }
            else
            {
                 qry =
                  string.Format(
                      "insert into {0}(MSG_CONTAINER_UID,INV_LOCAL_ID,PAT_LOCAL_ID,{1}) values('{2}','{3}','{4}',{5})",
                      tableName, clmNames, MsgID, locId, patId, clmValues);
            }
            return qry;
        }


        string Get_MSG_INTERVIEW_Qry1(string recId, string tableName, string clmNames, string clmValues, string datasource, string configid)
        {
            //string patId = GetMaxPatientId(false);           
            string patId = "";
            if (datasource == "Epi Info")
            {
                patId = "EpiInfo_PAT_" + configid + "_" + recId;
            }
            else
            {
                patId = "REDCap_PAT_" + configid + "_" + recId;
            }
            string qry = "";
            qry = string.Format(
              "select top 1 * from MSG_INTERVIEW where IXS_LOCAL_ID='{0}'",
              locId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where inv_local_id = '{0}'", locId);
                qry = GetUpdateQuery(tableName, clmNames, locId, clmValues) + wherecond;

            }
            else
            {
                qry =
                 string.Format(
                     "insert into {0}(MSG_CONTAINER_UID,IXS_LOCAL_ID,IXS_INTERVIEWEE_ID,{1}) values('{2}','{3}','{4}',{5})",
                     tableName, clmNames, MsgID, locId, patId, clmValues);
            }
            return qry;
        }


        string Get_MSG_PATIENT_Qry1(string tableName, string clmNames, string recId, string clmValues, string datasource,string configid)
        {

            string qry = "" , patId = ""; ;
            if (datasource == "Epi Info")
            {
                patId = "EpiInfo_PAT_" + configid + "_" + recId;
            }
            else
            {
                patId = "REDCap_PAT_" + configid + "_" + recId;
            }
            qry = string.Format(
              "select top 1 * from MSG_PATIENT where pat_local_id ='{0}'",
              patId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where pat_local_id = '{0}'", patId);
                qry = GetUpdateQuery(tableName, clmNames, patId, clmValues) + wherecond; 
            }
            else
            {
                 qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PAT_LOCAL_ID,{1}) values('{2}','{3}',{4})",
                tableName, clmNames, MsgID, patId, clmValues);
            }
            return qry;
        }

        string Get_MSG_ORGANIZATION_Qry1(string tableName, string clmNames,  string recId, string authorId, string clmValues, string datasource,string configid)
        {
            string qry = "", orgId = "";
            if (datasource == "Epi Info")
            {
                orgId = "EpiInfo_ORG_" + configid + "_" + recId;
            }
            else
            {
                orgId = "REDCap_ORG_" + configid + "_" + recId;
            }
            qry = string.Format(
              "select top 1 * from  MSG_ORGANIZATION  where org_local_id ='{0}'",
              orgId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where org_local_id = '{0}'", orgId);
                qry = GetUpdateQuery(tableName, clmNames, orgId, clmValues) + wherecond; 

            }
            else
            {
                qry = string.Format(
               "insert into {0}(MSG_CONTAINER_UID,ORG_LOCAL_ID,ORG_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
               tableName, clmNames, MsgID, orgId, authorId, clmValues);
            }
            return qry;
        }

        string Get_MSG_PLACE_Qry1(string tableName, string clmNames,  string recId, string authorId, string clmValues, string datasource,string configid)
        {          
            string qry = "", plaId = "";
            if (datasource == "Epi Info")
            {
                plaId = "EpiInfo_PLA_" + configid + "_" + recId;
            }
            else
            {
                plaId = "REDCap_PLA_" + configid + "_" + recId;
            }
            qry = string.Format(
              "select top 1 * from MSG_PLACE where pla_local_id ='{0}'",
              plaId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where pla_local_id = '{0}'", plaId);
                qry = GetUpdateQuery(tableName, clmNames, plaId, clmValues) + wherecond; 

            }
            else
            {
                qry = string.Format(
               "insert into {0}(MSG_CONTAINER_UID,PLA_LOCAL_ID,PLA_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
               tableName, clmNames, MsgID, plaId, authorId, clmValues);
            }
            return qry;
        }

        string Get_MSG_PROVIDER_Qry1(string tableName, string clmNames,  string recId, string authorId, string clmValues, string datasource,string configid)
        {          
            string qry = "", prvId = "";
            if (datasource == "Epi Info")
            {
                prvId = "EpiInfo_PRV_" + configid + "_" + recId;
            }
            else
            {
                prvId = "REDCap_PRV_" + configid + "_" + recId;
            }
            prid = prvId;

            qry = string.Format(
              "select top 1 * from MSG_PROVIDER where prv_local_id='{0}'",
              prvId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where prv_local_id = '{0}'", prvId);
                qry = GetUpdateQuery(tableName, clmNames, prvId, clmValues) + wherecond; 

            }
            else
            {
                qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PRV_LOCAL_ID,PRV_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, MsgID, prvId, authorId, clmValues);
            }
            UpdateMSGAnswerTextforProv();
            return qry;
        }


       public bool Get_MSG_CONTAINER_Qry1( string locId)
        {

            string qry = "" ;          
            qry = string.Format(
              "select top 1 * from MSG_CONTAINER where MSG_LOCAL_ID ='{0}'",
              locId);
            if (CheckforUpdateQuery(qry))
            {
                return true;
            }
            else
            {
                return false;
            }           
        }

        bool UpdateMSGAnswerTextforProv()
        {
            string qry = ""; bool errorsOccured = false;
            string quesiden = "";

            qry = string.Format(
              "select  * from MSG_ANSWER where MSG_Container_UID='{0}' ",
             MsgID);
            DataTable dt = new DataTable();
            if (GetQuesAnsforProvider(qry, out dt))
            {              
                    foreach (DataRow dr in dt.Rows)
                    {
                        quesiden = dr["QUESTION_IDENTIFIER"].ToString();
                        string part_id = "";
                        part_id = GetParticipationType(quesiden);
                        string query = string.Format(
                        "update  MSG_ANSWER set   ANSWER_TXT='{0}',PART_TYPE_CD = '{1}' ",
                            prid, part_id);
                        string wherequery = string.Format(" where MSG_Container_UID='{0}' and QUESTION_IDENTIFIER= '{1}'  ", MsgID, quesiden);

                         qry = query + wherequery;
                        if (OpenConnection())
                        {
                            try
                            {
                                var com = new SqlCommand();
                                com.Connection = _connection;
                                com.CommandType = CommandType.Text;
                                com.CommandText = qry;
                                com.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Log.WriteToErrorLog("Error in qry: " + qry);
                                Log.WriteToErrorLog(ex);
                                errorsOccured = true;
                            }                          
                        }
                    }                
            }
            else
            {
                string quescode = "2.16.840.1.114222.4.5.232";
                string quescodedesc = "NEDSS Base System";
                string quesdispname = "Investigator System UID";
                string qry1 =
                    "insert into MSG_ANSWER (MSG_CONTAINER_UID,MSG_EVENT_ID,MSG_EVENT_TYPE,QUESTION_IDENTIFIER,ANSWER_TXT,QUES_CODE_SYSTEM_CD," +
                    "QUES_CODE_SYSTEM_DESC_TXT,QUES_DISPLAY_TXT,PART_TYPE_CD ";
                string qry2 = string.Format( " ) values({0},'{1}','Case','{2}','{3}','{4}','{5}','{6}','{7}')",
                        MsgID, locId, msg_iden_def, prid, quescode, quescodedesc, quesdispname, partcipation_cd_def);
                qry = qry1 + qry2;
                if (OpenConnection())
                {
                    try
                    {
                        var com = new SqlCommand();
                        com.Connection = _connection;
                        com.CommandType = CommandType.Text;
                        com.CommandText = qry;
                        com.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLog("Error in qry: " + qry);
                        Log.WriteToErrorLog(ex);
                        errorsOccured = true;
                    }
                }
            }
            if (!errorsOccured)
            {
                return true;
            }
            return false;
        }

        string Get_MSG_TREATMENT_Qry1(string tableName, string clmNames, string recId, string authorId, string clmValues, string datasource,string configid)
        {           
            string qry = "", trtId = "";
            if (datasource == "Epi Info")
            {
                trtId = "EpiInfo_TRT_" + configid + "_" + recId;
            }
            else
            {
                trtId = "REDCap_TRT_" + configid + "_" + recId;
            }
            qry = string.Format(
              "select top 1 * from MSG_TREATMENT where trt_local_id ='{0}'",
             trtId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where trt_local_id = '{0}'", trtId);
                qry = GetUpdateQuery(tableName, clmNames, trtId, clmValues) + wherecond;

            }
            else
            {
                qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,TRT_LOCAL_ID,TRT_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, MsgID, trtId, authorId, clmValues);
            }
            return qry;
        }

        string GetUpdateQuery(string tableName, string clmNames, string locId, string clmValues)
        {           
            StringBuilder sb = new StringBuilder();
            sb.Append("update ");
            sb.Append(tableName );
            sb.Append("  set  ");                      
           string[] columnames= clmNames.Split(',');//msgid aythid idg
           string[] colvalues= clmValues.Split(',');//'1' '1234' 'rtyu
            for(int i=0;i<columnames.Length; i++)
            {                
                    sb.Append(columnames[i]);
                    sb.Append(" =");
                    sb.Append(colvalues[i]);
                    sb.Append(" , ");                
            }
            string querystring = sb.ToString();
            if (querystring.EndsWith(", "))
            {
                querystring = querystring.Remove(querystring.Length - 2, 2);
            }                        
            return querystring;
        }

        DataTable Get_MSG_Lookups(string quesIden, string quesdispnm)
        {            
            string qry = "";
            qry = string.Format(
              "  select top 1 MQ.QUES_CODE_SYSTEM_CD,QUES_CODE_SYSTEM_DESC_TXT,"+
      " [ANS_FROM_CODE_SYSTEM_CD],[ANS_FROM_CODE_SYSTEM_DESC_TXT] ,[ANS_FROM_DISPLAY_NM], [DATA_TYPE] from MSG_QUESTION_LOOKUP MQ " +      
        "left outer join MSG_ANSWER_LOOKUP MA on MQ.QUESTION_IDENTIFIER = MA.QUESTION_IDENTIFIER "+
        "and MQ.QUES_CODE_SYSTEM_CD = MA.QUES_CODE_SYSTEM_CD where MQ.QUESTION_IDENTIFIER='{0}' and MQ.QUESTION_DISPLAY_NAME='{1}'",quesIden, quesdispnm);

            var myCommand = new SqlCommand(qry, _connection);
            var adapter = new SqlDataAdapter(myCommand);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;                        
        }


        DataTable Get_MSG_Ans_Lookup(string quesIden,string anscode)
        {
            string qry = "";
            qry = string.Format(
              "  select * from  msg_answer_lookup where QUESTION_IDENTIFIER='{0}' and ANS_FROM_CODE= '{1}'", quesIden,anscode);

            var myCommand = new SqlCommand(qry, _connection);
            var adapter = new SqlDataAdapter(myCommand);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }



        string Get_MSG_ANSWER_Qry1(string quesIden, string recId, string ansTxt, string datasource,string configid,string quescode,string quescodedesc,string anscode,string anscodedesc,string anscodedisptxt,string quesdispname)
        {          
            string qry = "";

            qry = string.Format(
              "select top 1 * from MSG_ANSWER where msg_event_id='{0}' and question_identifier ='{1}' ",
             locId, quesIden);         
            var clmBuilder = new StringBuilder();
            var valBuilder = new StringBuilder();
            if(!string.IsNullOrEmpty(anscode))
            {
                clmBuilder.Append("ANS_CODE_SYSTEM_CD ").Append(",");
                valBuilder.Append("'").Append(anscode).Append("'").Append(","); 
            }
            if (!string.IsNullOrEmpty(anscodedesc))
            {
                clmBuilder.Append("ANS_CODE_SYSTEM_DESC_TXT ").Append(",");
                valBuilder.Append("'").Append(anscodedesc).Append("'").Append(",");
            }
            if (!string.IsNullOrEmpty(anscodedisptxt))
            {
                clmBuilder.Append("ANS_DISPLAY_TXT ").Append(",");
                valBuilder.Append("'").Append(anscodedisptxt).Append("'");
            }

            string clmNames = clmBuilder.ToString();
            if (clmNames.EndsWith(","))
            {
                clmNames = clmNames.Remove(clmNames.Length - 1, 1);
            }

            string clmValues = valBuilder.ToString();
            if (clmValues.EndsWith(","))
           {
                clmValues = clmValues.Remove(clmValues.Length - 1, 1);
           }
            if(!string.IsNullOrEmpty(prid))
            {
                ansTxt = prid;
            }
            if (CheckforUpdateQuery(qry))
            {
                string query = string.Format(
                 "update  MSG_ANSWER set  QUESTION_IDENTIFIER='{0}', ANSWER_TXT='{1}' , QUES_CODE_SYSTEM_CD='{3}' , QUES_CODE_SYSTEM_DESC_TXT='{4}', " +
                 "QUES_DISPLAY_TXT='{5}' ,PART_TYPE_CD='{6}'",
                  quesIden, ansTxt, locId, quescode, quescodedesc, quesdispname, partcipation_cd_def);
                string wherequery = string.Format(" where MSG_EVENT_ID='{0}' and question_identifier ='{1}' ", locId, quesIden);
                StringBuilder sb = new StringBuilder(); string querystring = "";
                if (!string.IsNullOrEmpty(clmNames))
                {
                    string[] columnames = clmNames.Split(',');//msgid aythid idg
                    string[] colvalues = clmValues.Split(',');//'1' '1234' 'rtyu
                    for (int i = 0; i < columnames.Length; i++)
                    {
                        sb.Append(columnames[i]);
                        sb.Append(" =");
                        sb.Append(colvalues[i]);
                        sb.Append(" , ");
                    }
                    querystring = sb.ToString();
                    if (!string.IsNullOrEmpty(querystring) && querystring.EndsWith(", "))
                    {
                        querystring = querystring.Remove(querystring.Length - 2, 2);
                        querystring = querystring.Insert(0, ",");
                    }
                }
                qry = query + querystring + wherequery;

            }
            else
            {
                string qry1 = 
                      "insert into MSG_ANSWER (MSG_CONTAINER_UID,MSG_EVENT_ID,MSG_EVENT_TYPE,QUESTION_IDENTIFIER,ANSWER_TXT,QUES_CODE_SYSTEM_CD," +
                      "QUES_CODE_SYSTEM_DESC_TXT,QUES_DISPLAY_TXT,PART_TYPE_CD ";
                if (!string.IsNullOrEmpty(clmNames))
                {
                    string qry2 = string.Format("," + clmNames + " ) values({0},'{1}','Case','{2}','{3}','{4}','{5}','{6}','{7}',{8})",
                         MsgID, locId, quesIden, ansTxt, quescode, quescodedesc, quesdispname, partcipation_cd_def, clmValues);
                    qry = qry1 + qry2;
                }
                else
                {
                    string qry2 = string.Format( clmNames + " ) values({0},'{1}','Case','{2}','{3}','{4}','{5}','{6}','{7}')",
                        MsgID, locId, quesIden, ansTxt, quescode, quescodedesc, quesdispname, partcipation_cd_def);
                    qry = qry1 + qry2;

                }
            }
            return qry;
        }

        bool CheckforUpdateQuery(string query)
        {
            try
            {                                                
                    Log.WriteToApplicationLog(query);
                    var myCommand = new SqlCommand(query, _connection);
                    var adapter = new SqlDataAdapter(myCommand);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        MsgID = dt.Rows[0]["msg_container_uid"].ToString();
                        return true;
                    }
                    else
                        return false;               
            }
            catch (Exception e)
            {
                throw ;
            }
            finally
            {
               // CloseConnection();
            }
        }


        public bool SaveLookup(Settings.QuestioLookup lookup)
        {
            bool errorsOccured = false;
            try
            {
                if (OpenConnection())
                {
                    string qry = "";
                    try
                    {                        
                            var com = new SqlCommand();
                            com.Connection = _connection;
                            com.CommandType = CommandType.Text;                          
                                qry = string.Format(
                                     "insert into MSG_QUESTION_LOOKUP(DOC_TYPE_CD,DOC_TYPE_VERSION_TXT, QUES_CODE_SYSTEM_CD, QUES_CODE_SYSTEM_DESC_TXT, DATA_TYPE, QUESTION_IDENTIFIER,QUESTION_DISPLAY_NAME,SENDING_SYSTEM_CD) " +
                                     "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                                     lookup.DocTypeId, lookup.DocTypeVersionTxt, lookup.QuesCodeSysCD, lookup.QuesCodeSysDescTxt,
                                     lookup.Data_Type,
                                     lookup.QuesIdentifier, lookup.QuesDisplayName, lookup.SendingSysCD);                         
                            com.CommandText = qry;
                            com.ExecuteNonQuery();     
                        if(lookup.Data_Type=="CODED")
                        {
                            qry = string.Format(
                                    "insert into MSG_ANSWER_LOOKUP(ANS_FROM_CODE,ANS_FROM_CODE_SYSTEM_CD, ANS_FROM_CODE_SYSTEM_DESC_TXT, ANS_FROM_DISPLAY_NM,"+
                                    " ANS_TO_CODE, ANS_TO_CODE_SYSTEM_CD,ANS_TO_CODE_SYSTEM_DESC_TXT,ANS_TO_DISPLAY_NM," +
                                    " CODE_TRANSLATION_REQUIRED,DOC_TYPE_CD,DOC_TYPE_VERSION_TXT,QUES_CODE_SYSTEM_CD,QUESTION_IDENTIFIER,SENDING_SYSTEM_CD,INVESTIGATION_FORM_CD )" +
                                    "values('{0}','{1}','{2}','{3}','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                                    lookup.AnsFromCode, lookup.AnsFromCodeSysCD, lookup.AnsFromCodeSysDecsTxt, lookup.AnsFromDisNM,
                                    lookup.CodeTransReq,lookup.DocTypeId, lookup.DocTypeVersionTxt, lookup.QuesCodeSysCD,lookup.QuesIdentifier,lookup.SendingSysCD,lookup.InvestigationFormCd);
                            com.CommandText = qry;
                            com.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLog("Error in qry: " + qry);
                        Log.WriteToErrorLog(ex);
                        errorsOccured = true;
                    }
                }

                if (!errorsOccured)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
                //CloseConnection();
            }
            return false;
        }


        public bool SaveAnslookup(Settings.QuestioLookup lookup)
        {
            bool errorsOccured = false;
            try
            {
                if (OpenConnection())
                {
                    string qry = "";
                    try
                    {
                        var com = new SqlCommand();
                        com.Connection = _connection;
                        com.CommandType = CommandType.Text;
                       
                            qry = string.Format(
                                    "insert into MSG_ANSWER_LOOKUP(ANS_FROM_CODE,ANS_FROM_CODE_SYSTEM_CD, ANS_FROM_CODE_SYSTEM_DESC_TXT, ANS_FROM_DISPLAY_NM," +
                                    " ANS_TO_CODE, ANS_TO_CODE_SYSTEM_CD,ANS_TO_CODE_SYSTEM_DESC_TXT,ANS_TO_DISPLAY_NM," +
                                    " CODE_TRANSLATION_REQUIRED,DOC_TYPE_CD,DOC_TYPE_VERSION_TXT,QUES_CODE_SYSTEM_CD,QUESTION_IDENTIFIER,SENDING_SYSTEM_CD,INVESTIGATION_FORM_CD )" +
                                    "values('{0}','{1}','{2}','{3}','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                                    lookup.AnsFromCode, lookup.AnsFromCodeSysCD, lookup.AnsFromCodeSysDecsTxt, lookup.AnsFromDisNM,
                                    lookup.CodeTransReq, lookup.DocTypeId, lookup.DocTypeVersionTxt, lookup.QuesCodeSysCD, lookup.QuesIdentifier, lookup.SendingSysCD, lookup.InvestigationFormCd);
                            com.CommandText = qry;
                            com.ExecuteNonQuery();                      
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLog("Error in qry: " + qry);
                        Log.WriteToErrorLog(ex);
                        errorsOccured = true;
                    }
                }

                if (!errorsOccured)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.WriteToErrorLog(e);
            }
            finally
            {
              //  CloseConnection();
            }
            return false;
        }

        public string GetParticipationType(string quesiden)
        {
            string participationtype = "";
            participationtype = REDCapAPI.Partcipation.ResourceManager.GetString(quesiden);
            if (string.IsNullOrEmpty(participationtype))
                participationtype = partcipation_cd_def;
            return participationtype;
        }

        bool GetQuesAnsforProvider(string query,out DataTable datatable)
        {
            try
            {
                Log.WriteToApplicationLog(query);
                var myCommand = new SqlCommand(query, _connection);
                var adapter = new SqlDataAdapter(myCommand);
                datatable = new DataTable(); ;
                adapter.Fill(datatable);
                if (datatable.Rows.Count > 0)
                {
                    MsgID = datatable.Rows[0]["msg_container_uid"].ToString();                  
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                // CloseConnection();
            }
        }

    }
    
}
