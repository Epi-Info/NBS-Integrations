using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LogManager;

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
                CloseConnection();
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
                        "insert into [MSG_CONTAINER](MSG_CONTAINER_UID,DOCUMENT_ID,DOC_TYPE_CD,EFFECTIVE_TIME,RECORD_STATUS_CD,RECORD_STATUS_TIME,RECEIVING_SYSTEM ,DATA_MIGRATION_STATUS,ONGOING_CASE ) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                        msgId, map.DocumentId, map.DocTypeCd, map.EffectiveTime, map.RecordStatusCd,
                        map.RecordStatusTime,map.DataSource,-1,map.Ongoing_case);
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
                        "update [MSG_CONTAINER]  set ONGOING_CASE='{0}',EFFECTIVE_TIME='{2}',RECORD_STATUS_TIME='{3}' where MSG_CONTAINER_UID='{1}'",
                       map.Ongoing_case, msgId,map.EffectiveTime,map.RecordStatusTime);
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


        public bool InsertApiValues(IEnumerable<Settings.Mappings> mapList)
        {
            bool errorsOccured = false;
            try
            {
                if (OpenConnection())
                {
                    string qry = "";
                    try
                    {
                        string msgId;
                        List<string> tableNames = mapList.Select(m => m.TableName).Distinct().ToList();                      

                            if (InsertMsgContainerId(mapList.First(), out msgId))
                        {
                            //List<string> tableNames = mapList.Select(m => m.TableName).Distinct().ToList();

                            foreach (string tableName in tableNames)
                            {
                                var maps = mapList.Where(m => m.TableName == tableName);
                                var clmBuilder = new StringBuilder();
                                var valBuilder = new StringBuilder();
                                string datasource = "";
                                foreach (var m in maps)
                                {
                                    clmBuilder.Append(m.ColumnName).Append(",");
                                    valBuilder.Append("'").Append(m.ApiValue).Append("'").Append(",");
                                    datasource = m.DataSource;
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
                                switch (tableName)
                                {
                                    case "MSG_CASE_INVESTIGATION":
                                        {
                                            qry = Get_MSG_CASE_INVESTIGATION_Qry(recId, tableName, clmNames, msgId, clmValues, datasource);
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
                                            qry = Get_MSG_PATIENT_Qry(tableName, clmNames, msgId, recId, clmValues, datasource);
                                        }
                                        break;
                                    case "MSG_ORGANIZATION":
                                        {
                                            qry = Get_MSG_ORGANIZATION_Qry(tableName, clmNames, msgId, recId, "", clmValues, datasource);
                                        }
                                        break;
                                    case "MSG_PLACE":
                                        {
                                            qry = Get_MSG_PLACE_Qry(tableName, clmNames, msgId, recId, "", clmValues, datasource);
                                        }
                                        break;
                                    case "MSG_PROVIDER":
                                        {
                                            qry = Get_MSG_PROVIDER_Qry(tableName, clmNames, msgId, recId, "", clmValues, datasource);
                                        }
                                        break;
                                    case "MSG_TREATMENT":
                                        {
                                            qry = Get_MSG_TREATMENT_Qry(tableName, clmNames, msgId, recId, "", clmValues, datasource);
                                        }
                                        break;
                                    case "MSG_ANSWER":
                                        {
                                            try
                                            {
                                                foreach (Settings.Mappings map in mapList)
                                                {
                                                    string questionIdentifier = map.NbsFieldName;
                                                    if (map.NbsFieldName.Contains("-"))
                                                    {
                                                        questionIdentifier = map.NbsFieldName.Split('-')[0].Trim();
                                                    }
                                                    qry = Get_MSG_ANSWER_Qry(questionIdentifier, msgId, recId, map.ApiValue,map.DataSource);
                                                    com.CommandText = qry;
                                                    com.ExecuteNonQuery();
                                                }
                                                return true;
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.WriteToErrorLog("Error in qry: " + qry);
                                                Log.WriteToErrorLog(ex);
                                                return false;
                                            }
                                        }
                                }

                                com.CommandText = qry;
                                com.ExecuteNonQuery();
                            }
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




        public bool InsertApiValues1(IEnumerable<Settings.Mappings> mapList)
        {
            bool errorsOccured = false;
            try
            {
                MsgID = null;
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
                            switch (tableName)
                            {
                                case "MSG_CASE_INVESTIGATION":
                                    {
                                        qry = Get_MSG_CASE_INVESTIGATION_Qry1(recId, tableName, clmNames, clmValues, datasource, configid);
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
                                    }
                                    break;
                                case "MSG_ORGANIZATION":
                                    {
                                        qry = Get_MSG_ORGANIZATION_Qry1(tableName, clmNames, recId, "", clmValues, datasource,configid);
                                    }
                                    break;
                                case "MSG_PLACE":
                                    {
                                        qry = Get_MSG_PLACE_Qry1(tableName, clmNames, recId, "", clmValues, datasource, configid);
                                    }
                                    break;
                                case "MSG_PROVIDER":
                                    {
                                        qry = Get_MSG_PROVIDER_Qry1(tableName, clmNames, recId, "", clmValues, datasource, configid);
                                    }
                                    break;
                                case "MSG_TREATMENT":
                                    {
                                        qry = Get_MSG_TREATMENT_Qry1(tableName, clmNames, recId, "", clmValues, datasource, configid);
                                    }
                                    break;
                                case "MSG_ANSWER":
                                    {
                                        try
                                        {
                                            foreach (Settings.Mappings map in mapList)
                                            {
                                                string questionIdentifier = map.NbsFieldName;
                                                if (map.NbsFieldName.Contains("-"))
                                                {
                                                    questionIdentifier = map.NbsFieldName.Split('-')[0].Trim();
                                                }
                                                qry = Get_MSG_ANSWER_Qry1(questionIdentifier, recId, map.ApiValue, map.DataSource,configid);
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
                                                // }
                                            }
                                            return true;
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteToErrorLog("Error in qry: " + qry);
                                            Log.WriteToErrorLog(ex);
                                            return false;
                                        }
                                    }
                            }
                            query = qry;
                            if (qry.StartsWith("insert") && string.IsNullOrEmpty(MsgID))
                            {                                
                                InsertMsgContainerId(mapList.First(), out msgId);
                                MsgID = msgId;
                                query= qry.Replace("values('", "values('" + MsgID);
                            }
                            else
                            {
                                UpdateMsgContainerId(mapList.First(), MsgID);
                            }
                            com.CommandText = query;
                            com.ExecuteNonQuery();
                            // }
                        }
                        //}
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLog("Error in qry: " + qry);
                        Log.WriteToErrorLog(ex);
                        errorsOccured = true;
                    }
                    // }

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

        string Get_MSG_CASE_INVESTIGATION_Qry(string recId, string tableName, string clmNames, string msgId, string clmValues, string datasource)
        {
            //string patId = GetMaxPatientId(false);           
            string locId, patId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_INV_" + recId;
                patId = "EpiInfo_PAT_" + recId;
            }
            else
            {
                locId = "REDCap_INV_" + recId;
                patId = "REDCap_PAT_" + recId;
            }
            string qry =
                  string.Format(
                      "insert into {0}(MSG_CONTAINER_UID,INV_LOCAL_ID,PAT_LOCAL_ID,{1}) values('{2}','{3}','{4}',{5})",
                      tableName, clmNames, msgId, locId, patId, clmValues);
            return qry;
        }

        string Get_MSG_PATIENT_Qry(string tableName, string clmNames, string msgId, string recId, string clmValues, string datasource)
        {           
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_PAT_" + recId;
            }
            else
            {
                locId = "REDCap_PAT_" + recId;
            }
            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PAT_LOCAL_ID,{1}) values('{2}','{3}',{4})",
                tableName, clmNames, msgId, locId, clmValues);
            return qry;
        }

        string Get_MSG_ORGANIZATION_Qry(string tableName, string clmNames, string msgId, string recId, string authorId, string clmValues, string datasource)
        {            
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_ORG_" + recId;
            }
            else
            {
                locId = "REDCap_ORG_" + recId;
            }

            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,ORG_LOCAL_ID,ORG_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, msgId, locId, authorId, clmValues);
            return qry;
        }

        string Get_MSG_PLACE_Qry(string tableName, string clmNames, string msgId, string recId, string authorId, string clmValues, string datasource)
        {           
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_PLA_" + recId;
            }
            else
            {
                locId = "REDCap_PLA_" + recId;
            }

            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PLA_LOCAL_ID,PLA_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, msgId, locId, authorId, clmValues);
            return qry;
        }

        string Get_MSG_PROVIDER_Qry(string tableName, string clmNames, string msgId, string recId, string authorId, string clmValues, string datasource)
        {           
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_PRO_" + recId;
            }
            else
            {
                locId = "REDCap_PRO_" + recId;
            }

            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PRV_LOCAL_ID,PRV_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, msgId, locId, authorId, clmValues);
            return qry;
        }

        string Get_MSG_TREATMENT_Qry(string tableName, string clmNames, string msgId, string recId, string authorId, string clmValues, string datasource)
        {           
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_TRE_" + recId;
            }
            else
            {
                locId = "REDCap_TRE_" + recId;
            }

            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,TRT_LOCAL_ID,TRT_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, msgId, locId, authorId, clmValues);
            return qry;
        }

        string Get_MSG_ANSWER_Qry(string quesIden, string msgId, string recId, string ansTxt,string datasource)
        {
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_MSG_" + recId;
            }
            else
            {
                locId = "REDCap_MSG_" + recId;
            }

            string qry = string.Format(
                "insert into MSG_ANSWER (MSG_CONTAINER_UID,MSG_EVENT_ID,MSG_EVENT_TYPE,QUESTION_IDENTIFIER,ANSWER_TXT) values({0},'{1}','Case','{2}','{3}')",
                msgId, locId, quesIden, ansTxt);
            return qry;
        }




        string Get_MSG_CASE_INVESTIGATION_Qry1(string recId, string tableName, string clmNames,  string clmValues, string datasource,string configid)
        {
            //string patId = GetMaxPatientId(false);           
            string locId, patId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_INV_" + configid + "_" + recId;
                patId = "EpiInfo_PAT_" + configid + "_" + recId;
            }
            else
            {
                locId = "REDCap_INV_" + configid + "_" + recId;
                patId = "REDCap_PAT_" + configid + "_" + recId;
            }
            string qry = "";
            qry = string.Format(
              "select top 1 * from MSG_CASE_INVESTIGATION where inv_local_id='{0}'",
              locId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where inv_local_id = '{0}'", locId);
                qry = GetUpdateQuery(tableName, clmNames, locId, clmValues) + wherecond; ;

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

        string Get_MSG_PATIENT_Qry1(string tableName, string clmNames, string recId, string clmValues, string datasource,string configid)
        {
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_PAT_" + configid + "_" + recId;
            }
            else
            {
                locId = "REDCap_PAT_" + configid + "_" + recId;
            }
            string qry = "";
            qry = string.Format(
              "select top 1 * from MSG_PATIENT where pat_local_id ='{0}'",
              locId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where pat_local_id = '{0}'", locId);
                qry = GetUpdateQuery(tableName, clmNames, locId, clmValues) + wherecond; ;

            }
            else
            {
                 qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PAT_LOCAL_ID,{1}) values('{2}','{3}',{4})",
                tableName, clmNames, MsgID, locId, clmValues);
            }
            return qry;
        }

        string Get_MSG_ORGANIZATION_Qry1(string tableName, string clmNames,  string recId, string authorId, string clmValues, string datasource,string configid)
        {
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_ORG_" + configid + "_" + recId;
            }
            else
            {
                locId = "REDCap_ORG_" + configid + "_" + recId;
            }
            string qry = "";
            qry = string.Format(
              "select top 1 * from  MSG_ORGANIZATION  where org_local_id ='{0}'",
              locId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where org_local_id = '{0}'", locId);
                qry = GetUpdateQuery(tableName, clmNames, locId, clmValues) + wherecond; 

            }
            else
            {
                qry = string.Format(
               "insert into {0}(MSG_CONTAINER_UID,ORG_LOCAL_ID,ORG_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
               tableName, clmNames, MsgID, locId, authorId, clmValues);
            }
            return qry;
        }

        string Get_MSG_PLACE_Qry1(string tableName, string clmNames,  string recId, string authorId, string clmValues, string datasource,string configid)
        {
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_PLA_" + configid + "_" + recId;
            }
            else
            {
                locId = "REDCap_PLA_" + configid + "_" + recId;
            }
            string qry = "";
            qry = string.Format(
              "select top 1 * from MSG_PLACE where pla_local_id ='{0}'",
              locId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where pla_local_id = '{0}'", locId);
                qry = GetUpdateQuery(tableName, clmNames, locId, clmValues) + wherecond; 

            }
            else
            {
                qry = string.Format(
               "insert into {0}(MSG_CONTAINER_UID,PLA_LOCAL_ID,PLA_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
               tableName, clmNames, MsgID, locId, authorId, clmValues);
            }
            return qry;
        }

        string Get_MSG_PROVIDER_Qry1(string tableName, string clmNames,  string recId, string authorId, string clmValues, string datasource,string configid)
        {
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_PRO_"+configid+"_" + recId;
            }
            else
            {
                locId = "REDCap_PRO_" + configid + "_" + recId;
            }
            string qry = "";
            qry = string.Format(
              "select top 1 * from MSG_PROVIDER where prv_local_id='{0}'",
              locId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where prv_local_id = '{0}'", locId);
                qry = GetUpdateQuery(tableName, clmNames, locId, clmValues) + wherecond; 

            }
            else
            {
                qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PRV_LOCAL_ID,PRV_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, MsgID, locId, authorId, clmValues);
            }
            return qry;
        }

        string Get_MSG_TREATMENT_Qry1(string tableName, string clmNames, string recId, string authorId, string clmValues, string datasource,string configid)
        {
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_TRE_"+ configid+"_" + recId;
            }
            else
            {
                locId = "REDCap_TRE_" + configid + "_" + recId;
            }
            string qry = "";
            qry = string.Format(
              "select top 1 * from MSG_TREATMENT where trt_local_id ='{0}'",
             locId);
            if (CheckforUpdateQuery(qry))
            {
                string wherecond = string.Format(" where trt_local_id = '{0}'", locId);
                qry = GetUpdateQuery(tableName, clmNames, locId, clmValues) + wherecond;

            }
            else
            {
                qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,TRT_LOCAL_ID,TRT_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, MsgID, locId, authorId, clmValues);
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

        string Get_MSG_ANSWER_Qry1(string quesIden, string recId, string ansTxt, string datasource,string configid)
        {
            string locId = "";
            if (datasource == "Epi Info")
            {
                locId = "EpiInfo_MSG_"+configid+"_" + recId;
            }
            else
            {
                locId = "REDCap_MSG_" + configid + "_" + recId;
            }
            string qry = "";

            qry = string.Format(
              "select top 1 * from MSG_ANSWER where msg_event_id='{0}'",
             locId);
            if (CheckforUpdateQuery(qry))
            {               
                qry = string.Format(
                  "update  MSG_ANSWER set  QUESTION_IDENTIFIER='{0}', ANSWER_TXT='{1}' where MSG_EVENT_ID='{2}' ",
                   quesIden, ansTxt, locId);
                
            }
            else
            {
                qry = string.Format(
                   "insert into MSG_ANSWER (MSG_CONTAINER_UID,MSG_EVENT_ID,MSG_EVENT_TYPE,QUESTION_IDENTIFIER,ANSWER_TXT) values({0},'{1}','Case','{2}','{3}')",
                   MsgID, locId, quesIden, ansTxt);
            }
            return qry;
        }

        bool CheckforUpdateQuery(string query)
        {
            try
            {                
               // if (OpenConnection())
                //{                    
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
               // }
               // return false;
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
    }
}
