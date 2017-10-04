using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace EpiInfoAPI
{
   public class AccessDBFactory
    {
        #region members
        private string _fileName;
        private string _connectionString = null;

        public string FileName 
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        public IDbConnection DbConnection
        {
            get; set;
        }

        public string ConnectionString 
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        protected OleDbConnection Oledbconnection
        {
            get; set;
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        public AccessDBFactory(string fileName)
        {
            _fileName = fileName;
            OleDbConnectionStringBuilder cnnStringBuilder = new OleDbConnectionStringBuilder();
            cnnStringBuilder.DataSource = fileName;                      
            ConnectionString = fileName; 
            Oledbconnection = new OleDbConnection(ConnectionString);
            DbConnection = GetConnection();
        }
      
        protected virtual OleDbConnection GetConnection()
        {
            if (Oledbconnection == null)
                return new OleDbConnection(ConnectionString);
            else
                return Oledbconnection;
        }

        public bool CanClaimConnectionString(string connectionString)
        {
            if (connectionString.ToLowerInvariant().Contains(".mdb") || (connectionString.ToLowerInvariant().Contains("provider=microsoft.jet.oledb.4.0") && !connectionString.ToLowerInvariant().Contains("excel") && !connectionString.ToLowerInvariant().Contains("fmt=delimited")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void OpenConnection(IDbConnection conn)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException("SQL exception:", ex);
            }
        }

        /// <summary>
        /// Close a specific connection if state is not already closed
        /// </summary>
        /// <param name="conn"></param>
        protected void CloseConnection(IDbConnection conn)
        {
            try
            {
                if (conn != null)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                conn = null;
                throw new System.ApplicationException("Error closing connection.", ex);
            }
        }

        public DataTable GetViewQueryString()
        {
            DataTable dataTable = new DataTable();
            string queryString =
                "select [ViewId], [Name], [CheckCode], [CheckCodeBefore], [CheckCodeAfter], [RecordCheckCodeBefore], " +
                "[RecordCheckCodeAfter], [CheckCodeVariableDefinitions], [IsRelatedView], [Width], [Height], [Orientation], [LabelAlign] " +
                "from metaViews";
            IDbCommand command = DbConnection.CreateCommand();
            command.CommandText = queryString;
            try
            {
                OpenConnection(DbConnection);
                dataTable.Load(command.ExecuteReader(), LoadOption.OverwriteChanges);
                return dataTable;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                CloseConnection(DbConnection);
            }
            return dataTable;
        }

        public string GetViewName()
        {
            string querystring = "SELECT top 1 [Name] from [metaViews] where [IsRelatedView] = 0";
            IDbCommand command = DbConnection.CreateCommand();
            command.CommandText = querystring;
            try
            {
                OpenConnection(DbConnection);
                var obj = command.ExecuteScalar();
                if (obj != null)
                    return obj.ToString();
                else
                    return "";
            }
            catch (System.Data.OleDb.OleDbException ex)
            {
                throw new System.ApplicationException("Error executing select query against the database.", ex);
            }
            finally
            {
                CloseConnection(DbConnection);
            }
        }

        public List<int> GetPages(string viewName)
        {
            DataTable dataTable = new DataTable();
            List<int> pageIds = new List<int>();
            string querystring = " select pageid from metaPages inner join metaViews on metaPages.ViewId=metaViews.ViewId where metaViews.Name=@viewName";
            IDbCommand command = DbConnection.CreateCommand();
            command.CommandText = querystring;
            OleDbParameter param = new OleDbParameter();
            param.ParameterName = "@viewName";
            param.DbType = DbType.String;
            param.Value = viewName;
            param.Direction = ParameterDirection.Input;
            param.SourceVersion = DataRowVersion.Default;
            command.Parameters.Add(param);
            try
            {
                OpenConnection(DbConnection);
                dataTable.Load(command.ExecuteReader(), LoadOption.OverwriteChanges);
                foreach (DataRow dr in dataTable.Rows)
                {
                    pageIds.Add(Convert.ToInt32(dr["PageId"].ToString()));
                }
                return pageIds;
            }
            catch (System.Data.OleDb.OleDbException ex)
            {
                throw new System.ApplicationException("Error executing select query against the database.", ex);
            }
            finally
            {
                CloseConnection(DbConnection);
            }
        }

        public DataTable GetData()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentOutOfRangeException("Connection string is empty");
            }
            DataTable dataTable = new DataTable();
            string viewname = GetViewName();
            List<int> pageIds = GetPages(viewname);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("SELECT * ");
            sb.Append("FROM");
            sb.Append(" ");
            sb.Append("");
            System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
            sb2.Append("[" + viewname + "] ");
            sb2.Append("t inner join ");
            foreach (int i in pageIds)
            {
                sb.Append("(");
                sb2.Append("[" + viewname + i.ToString() + "] ");
                sb2.Append("on t.GlobalRecordId = ");
                sb2.Append("[" + viewname + i.ToString() + "]");
                sb2.Append(".GlobalRecordId) inner join ");
            }
            sb2.Length = sb2.Length - 12;
            sb.Append(sb2);
            sb.Append(" order by [LastSaveTime] DESC ");
            IDbCommand command = DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            try
            {
                OpenConnection(DbConnection);                
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                adapter.SelectCommand = (OleDbCommand)command;
                adapter.Fill(dataTable);
                try
                {
                    adapter.FillSchema(dataTable, SchemaType.Source);
                }
                catch { }
                if (dataTable.Columns["RecStatus"] == null && dataTable.Columns["t.RecStatus"] != null)
                {
                    dataTable.Columns["t.RecStatus"].ColumnName = "RecStatus";
                }
                if (dataTable.Columns.Contains("t.GlobalRecordId"))
                {
                    dataTable.Columns["t.GlobalRecordId"].ColumnName = "GlobalRecordId";
                }
                if (dataTable.Columns.Contains("t.UniqueKey"))
                {
                    dataTable.Columns["t.UniqueKey"].ColumnName = "record_id";
                }
                if ( dataTable.Columns.Contains("UniqueKey"))
                {
                    dataTable.Columns["UniqueKey"].ColumnName = "record_id";
                }
                foreach (int i in pageIds)
                {                   
                    string pageGUIDName = viewname+ i.ToString()+ "."+"GlobalRecordId"  ;
                    if (dataTable.Columns.Contains(pageGUIDName))
                    {
                        dataTable.Columns.Remove(pageGUIDName);
                    }
                }
                if (dataTable.Columns.Contains("GlobalRecordId"))
                {
                    dataTable.Columns.Remove("GlobalRecordId");
                }
                if (dataTable.Columns.Contains("UniqueKey"))
                {
                    dataTable.Columns.Remove("UniqueKey");
                }
                if (dataTable.Columns.Contains("RecStatus"))
                {
                    dataTable.Columns.Remove("RecStatus");
                }
                if (dataTable.Columns.Contains("FKey"))
                {
                    dataTable.Columns.Remove("FKey");
                }
                if (dataTable.Columns.Contains("FirstSaveLogonName"))
                {
                    dataTable.Columns.Remove("FirstSaveLogonName");
                }
                if (dataTable.Columns.Contains("FirstSaveTime"))
                {
                    dataTable.Columns.Remove("FirstSaveTime");
                }
                if (dataTable.Columns.Contains("LastSaveLogonName"))
                {
                    dataTable.Columns.Remove("LastSaveLogonName");
                }
                //if (dataTable.Columns.Contains("LastSaveTime"))
                //{
                //    dataTable.Columns.Remove("LastSaveTime");
                //}              
                return dataTable;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                CloseConnection(DbConnection);
            }
            return dataTable;
        }

        public List<string> GetFields(int viewid)
        {
            try
            {
                List<string> fields = new List<string>();
                DataTable dataTable = new DataTable();
                string queryString =
                    "select F.[FieldId], F.[Name] As Name, F.[PageId], F.[FieldTypeId],  " +
                    " F.[RelateCondition], F.[RelatedViewId], " +
                    "F.[SourceTableName], F.[CodeColumnName], F.[TextColumnName], " +
                    "F.[SourceFieldId] " +
                    "from metaFields F where F.[ViewId] = @viewId " +
                    " and FieldTypeId not in(2,21,13)" +
                    "order by F.[FieldId]";

                IDbCommand command = DbConnection.CreateCommand();
                command.CommandText = queryString;
                OleDbParameter param = new OleDbParameter();
                param.ParameterName = "@viewId";
                param.DbType = DbType.Int32;
                param.Value = viewid;
                param.Direction = ParameterDirection.Input;
                param.SourceVersion = DataRowVersion.Default;
                command.Parameters.Add(param);
                try
                {
                    OpenConnection(DbConnection);
                    dataTable.Load(command.ExecuteReader(), LoadOption.OverwriteChanges);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        fields.Add(row["Name"].ToString());
                    }
                    fields.Remove("UniqueKey");
                    fields.Remove("RECSTATUS");
                    fields.Remove("GlobalRecordId");
                    return fields;
                }
                catch (Exception ex)
                {
                }
                return fields;
            }
            finally
            {
                CloseConnection(DbConnection);
            }
        }
    }
}
