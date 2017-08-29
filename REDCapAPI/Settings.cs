using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace REDCapAPI
{
    public class Settings
    {
        private const string FileName = @"{0}\Config.xml";
        private static string _configPath = @"{0}\Config.xml";
        private readonly XmlDocument _xmlDoc = new XmlDocument();

        public Settings(string path)
        {
            _configPath = string.Format(FileName, path);
            _xmlDoc.Load(_configPath);
        }

        public struct Credentials
        {
            public string ConfigId { get; set; }
            public string ApiUrl { get; set; }
            public string Token { get; set; }
            public string FormName { get; set; }
            public string StateId { get; set; }
            public string Exclude { get; set; }
            public string AuthorId { get; set; }
            public string CustodianId { get; set; }
            public string DataSource { get; set; }
            public string ConnectionString { get; set; }
            public string RecordStatus { get; set; }
            public string MsgContainerStartId { get; set; }
            public string PatLocalId { get; set; }
            public string InvLocalId { get; set; }
        }

        public struct Mappings
        {
            public string ConfigId { get; set; }
            public string DataSource { get; set; }
            public string ApiFieldName { get; set; }
            public string NbsFieldName { get; set; }
            public string ColumnName { get; set; }
            public string ApiValue { get; set; }
            public string TableName { get; set; }

            public string DocumentId { get; set; }
            public string RecordId { get; set; }
            public string DocTypeCd { get; set; }
            public string EffectiveTime { get; set; }
            public string RecordStatusCd { get; set; }
            public string RecordStatusTime { get; set; }
            public string MsgContainerStartId { get; set; }
            public string PatLocalId { get; set; }
            public string InvLocalId { get; set; }
            public List<Mappings> LineItems  { get; set; }
        }

        public Credentials ReadApiSettings()
        {
            var credentials = new Credentials();
            try
            {
                if (!File.Exists(_configPath))
                {
                    return credentials;
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(_configPath);

                XmlNode selectedNode = xmlDoc.SelectSingleNode("/Settings/DataSource");
                if (selectedNode != null)
                    credentials.DataSource = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/ConnectionString");
                if (selectedNode != null)
                    credentials.ConnectionString = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/RecordStatus");
                if (selectedNode != null)
                    credentials.RecordStatus = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/MsgContainerStartId");
                if (selectedNode != null)
                    credentials.MsgContainerStartId = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/PAT_LOCAL_ID");
                if (selectedNode != null)
                    credentials.PatLocalId = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/INV_LOCAL_ID");
                if (selectedNode != null)
                    credentials.InvLocalId = selectedNode.InnerText;
            }
            catch (Exception exception)
            {
                LogManager.Log.WriteToErrorLog(exception.Message);
            }
            return credentials;
        }
    }
}

