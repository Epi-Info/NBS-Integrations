using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace EpiInfoAPI
{
   public class AccessDBFactory
    {
        private string _fileName;
        private string _connectionString = null;

        public string fileName // Implements IDbDriver.DbName
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

        public string ConnectionString // Implements IDbDriver.DbName
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

        public AccessDBFactory(string fileName)
        {
            _fileName = fileName;
            OleDbConnectionStringBuilder cnnStringBuilder = new OleDbConnectionStringBuilder();
            cnnStringBuilder.DataSource = fileName;
            cnnStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
        }

        protected virtual string GetConnection()
        {
            string MSAccess = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}";
            string result = string.Empty;
            string filestring = fileName.Trim(new char[] { '\'' });
            ConnectionString= result = string.Format(MSAccess, filestring);
            return result;
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

    }
}
