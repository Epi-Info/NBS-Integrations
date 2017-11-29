using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace REDCapAPI
{
    public class Settings
    {
        private const string FileName = @"{0}\Config.xml";
        private static string _configPath = @"{0}\Config.xml";
        private readonly XmlDocument _xmlDoc = new XmlDocument();
        private const string passPhrase = "jEz9wopRFHNx8R7OQSgmr0Ye6xBb9nPKKDZAydJ6fmp2/jFJPEYDnz33TQqXz+/qXjoYhWh5QD9MG/BBzDrjAskF2XaJX44LwceZC3yiuR5/CPI013gYuffEsCPeTuo0VHeqxQ==";
        private const string saltValue = "I3mi1ehgzE/9eGiWdTVrMxCQPWHgkOGVg9mZuIcF1XSnxG6dOOAtnbzeYsrnQHvSD1zh3V1eVBLuypTGP0vNw7lEo6FXCpnICGXy+yNH57i+JnT9MTBZuRc5BrBbQTPF64vANg==";
        private const string initVector = "G6up33hyUX5guTj+";
        private string _path;

        public Settings(string path)
        {
            _path = path;
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
            public string Ongoing_case { get; set; }

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

        public struct QuestioLookup
        {
            public string DocTypeId { get; set; }
            public string DocTypeVersionTxt { get; set; }
            public string QuesCodeSysCD { get; set; }
            public string QuesCodeSysDescTxt { get; set; }
            public string Data_Type { get; set; }
            public string QuesIdentifier { get; set; }
            public string QuesDisplayName { get; set; }
            public string SendingSysCD { get; set; }

            public string AnsFromCode { get; set; }
            public string AnsFromCodeSysCD { get; set; }
            public string AnsFromCodeSysDecsTxt { get; set; }
            public string AnsFromDisNM { get; set; }          
            public string CodeTransReq { get; set; }                    
            public string InvestigationFormCd { get; set; }           
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
                    credentials.ConnectionString = Decrypt(selectedNode.InnerText.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty).Trim());

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
      
        public static string Decrypt(string cipherText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, "MD5", 1);
            byte[] keyBytes = password.GetBytes(16);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            return plainText;
        }

        /// <summary>
        /// Encryption
        /// </summary>
        /// <param name="plainText">The plaintext to encrypt</param>
        /// <returns>The ciphertext</returns>
        public static string Encrypt(string plainText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, "MD5", 1);
            byte[] keyBytes = password.GetBytes(16);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherTextBytes);
            return cipherText;
        }


        public bool UpdateApiSettings(string conn)
        {
            var credentials = new Credentials();
            try
            {
                if (!File.Exists(_configPath))
                {
                    return false;           
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(_configPath);

                XmlNode selectedNode = xmlDoc.SelectSingleNode("/Settings/DataSource");
                if (selectedNode != null)
                    credentials.DataSource = selectedNode.InnerText;

                selectedNode = xmlDoc.SelectSingleNode("/Settings/ConnectionString");
                if (selectedNode != null)
                {
                    //credentials.ConnectionString = Decrypt(selectedNode.InnerText);
                    if (!string.IsNullOrEmpty(conn))
                    {
                        selectedNode.InnerText = Encrypt(conn);
                    }
                    else
                    {
                        selectedNode.InnerText = "";
                    }
                   
                   // xmlDoc.SelectSingleNode("/Settings/ConnectionString").InnerText = Encrypt(conn);
                    xmlDoc.Save(_configPath);
                }

                return true;
            }
            catch (Exception exception)
            {
                LogManager.Log.WriteToErrorLog(exception.Message);
                return false;
            }           
        }
    }
}

