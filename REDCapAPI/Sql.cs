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
        private readonly string _connectionString;// = "server={0};database={1};User Id={2};Password={3};";

        //private const string ConnectionString1 = "Server={0};Database={1};Trusted_Connection=True;";

        private static string _configPath = @"{0}\Config.xml";
        private static SqlCredentials _credentials;

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
                        "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='NBS_MSGOUTE'";
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
                        string.Format("select question_identifier,question_display_name from MSG_QUESTION_LOOKUP");
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
                            qry = string.Format(
                                 "insert into FLD_MAPPING(Config_id,DataSource, Source_Fld_NM, NBS_Fld_NM, Column_NM, Table_NM) " +
                                 "values('{0}','{1}','{2}','{3}','{4}','{5}')",
                                 map.ConfigId, map.DataSource, map.ApiFieldName, map.NbsFieldName,
                                 map.ColumnName,
                                 map.TableName);
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
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            return true;
        }

        bool CloseConnection()
        {
            _connection.Close();
            return true;
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
                    string qry = string.Format("SELECT * from [FLD_MAPPING] where Config_Id = '{0}'", configId);
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
                    string qry = string.Format("select fm.Source_Fld_NM,fm.Table_NM,fm.Column_NM,fm.NBS_Fld_NM from FLD_MAPPING fm inner join Source_Config rc on rc.Config_id = fm.Config_id where rc.Form_NM = '{0}'", formName);
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
                        "insert into [MSG_CONTAINER](MSG_CONTAINER_UID,DOCUMENT_ID,DOC_TYPE_CD,EFFECTIVE_TIME,RECORD_STATUS_CD,RECORD_STATUS_TIME) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                        msgId, map.DocumentId, map.DocTypeCd, map.EffectiveTime, map.RecordStatusCd,
                        map.RecordStatusTime);
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
                        if (InsertMsgContainerId(mapList.First(), out msgId))
                        {
                            List<string> tableNames = mapList.Select(m => m.TableName).Distinct().ToList();

                            foreach (string tableName in tableNames)
                            {
                                var maps = mapList.Where(m => m.TableName == tableName);
                                var clmBuilder = new StringBuilder();
                                var valBuilder = new StringBuilder();

                                foreach (var m in maps)
                                {
                                    clmBuilder.Append(m.ColumnName).Append(",");
                                    valBuilder.Append("'").Append(m.ApiValue).Append("'").Append(",");
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
                                            qry = Get_MSG_CASE_INVESTIGATION_Qry(recId, tableName, clmNames, msgId, clmValues);
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
                                            qry = Get_MSG_PATIENT_Qry(tableName, clmNames, msgId, recId, clmValues);
                                        }
                                        break;
                                    case "MSG_ORGANIZATION":
                                        {
                                            qry = Get_MSG_ORGANIZATION_Qry(tableName, clmNames, msgId, recId, "", clmValues);
                                        }
                                        break;
                                    case "MSG_PLACE":
                                        {
                                            qry = Get_MSG_PLACE_Qry(tableName, clmNames, msgId, recId, "", clmValues);
                                        }
                                        break;
                                    case "MSG_PROVIDER":
                                        {
                                            qry = Get_MSG_PROVIDER_Qry(tableName, clmNames, msgId, recId, "", clmValues);
                                        }
                                        break;
                                    case "MSG_TREATMENT":
                                        {
                                            qry = Get_MSG_TREATMENT_Qry(tableName, clmNames, msgId, recId, "", clmValues);
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
                                                    qry = Get_MSG_ANSWER_Qry(questionIdentifier, msgId, recId, map.ApiValue);
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

        string Get_MSG_CASE_INVESTIGATION_Qry(string recId, string tableName, string clmNames, string msgId, string clmValues)
        {
            //string patId = GetMaxPatientId(false);
            string patId = "REDCap_PAT_" + recId;
            string locId = "REDCap_INV_" + recId;
            string qry =
                  string.Format(
                      "insert into {0}(MSG_CONTAINER_UID,INV_LOCAL_ID,PAT_LOCAL_ID,{1}) values('{2}','{3}','{4}',{5})",
                      tableName, clmNames, msgId, locId, patId, clmValues);
            return qry;
        }

        string Get_MSG_PATIENT_Qry(string tableName, string clmNames, string msgId, string recId, string clmValues)
        {
            string locId = "REDCap_PAT_" + recId;
            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PAT_LOCAL_ID,{1}) values('{2}','{3}',{4})",
                tableName, clmNames, msgId, locId, clmValues);
            return qry;
        }

        string Get_MSG_ORGANIZATION_Qry(string tableName, string clmNames, string msgId, string recId, string authorId, string clmValues)
        {
            string locId = "REDCap_ORG_" + recId;

            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,ORG_LOCAL_ID,ORG_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, msgId, locId, authorId, clmValues);
            return qry;
        }

        string Get_MSG_PLACE_Qry(string tableName, string clmNames, string msgId, string recId, string authorId, string clmValues)
        {
            string locId = "REDCap_PLA_" + recId;

            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PLA_LOCAL_ID,PLA_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, msgId, locId, authorId, clmValues);
            return qry;
        }

        string Get_MSG_PROVIDER_Qry(string tableName, string clmNames, string msgId, string recId, string authorId, string clmValues)
        {
            string locId = "REDCap_PRO_" + recId;

            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,PRV_LOCAL_ID,PRV_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, msgId, locId, authorId, clmValues);
            return qry;
        }

        string Get_MSG_TREATMENT_Qry(string tableName, string clmNames, string msgId, string recId, string authorId, string clmValues)
        {
            string locId = "REDCap_TRE_" + recId;

            string qry = string.Format(
                "insert into {0}(MSG_CONTAINER_UID,TRT_LOCAL_ID,TRT_AUTHOR_ID,{1}) values('{2}','{3}','{4}',{5})",
                tableName, clmNames, msgId, locId, authorId, clmValues);
            return qry;
        }

        string Get_MSG_ANSWER_Qry(string quesIden, string msgId, string recId, string ansTxt)
        {
            string locId = "REDCap_MSG_" + recId;

            string qry = string.Format(
                "insert into MSG_ANSWER (MSG_CONTAINER_UID,MSG_EVENT_ID,MSG_EVENT_TYPE,QUESTION_IDENTIFIER,ANSWER_TXT) values({0},'{1}','Case','{2}','{3}')",
                msgId, locId, quesIden, ansTxt);
            return qry;
        }
    }
}
