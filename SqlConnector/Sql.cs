using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace SqlConnector
{
    public class Sql
    {
        private const string FileName = @"{0}\Config.xml";
        private SqlConnection _connection;
        private const string ConnectionString = "server={0};database={1};User Id={2};Password={3};";

        private const string ConnectionString1 = "Server={0};Database={1};Trusted_Connection=True;";

        private static string _configPath = @"{0}\Config.xml";
        private static SqlCredentials _credentials;

        public Sql(string appPath)
        {
            _configPath = string.Format(FileName, appPath);
            _credentials = ReadSqlSettings();
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
                LogManager.Log.WriteToErrorLog(e);
                LogManager.Log.WriteToErrorLog(orderno);
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

        bool OpenConnection()
        {
            if (string.IsNullOrEmpty(_credentials.UserName) && string.IsNullOrEmpty(_credentials.Password))
            {
                _connection = new SqlConnection(string.Format(ConnectionString1, _credentials.Server, _credentials.DataBase));
            }
            else
            {
                _connection = new SqlConnection(string.Format(ConnectionString, _credentials.Server, _credentials.DataBase,
                                                    _credentials.UserName, _credentials.Password));
            }
            _connection.Open();
            return true;
        }

        bool CloseConnection()
        {
            _connection.Close();
            return true;
        }
    }
}
