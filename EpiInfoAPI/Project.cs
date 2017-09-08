using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EpiInfoAPI
{
    public class Project
    {

        private XmlDocument xmlDoc = null;
        private string collectedDataConnectionString;
        public static readonly string passPhrase = "80787d6053694493be171dd712e51c61";
        public static readonly string saltValue = "476ba16073764022bc7f262c6d67ebef";
        public static readonly string initVector = "0f8f*d5bd&cb4~9f";

        #region members
        public string FilePath
        {
            get; set;
        }
        public SqlDBFactory sqldbfactory
        {
            get;set;
        }
        public string DbConnection
        {
            get;set;
        }
        public bool IsProjectFile
        {
            get;set;
        }
        /// <summary>
        /// Gets/sets the path name of project file.
        /// </summary>
        public string Location
        {
            get
            {
                return xmlDoc.DocumentElement.Attributes["location"].Value;
            }
            set
            {
                xmlDoc.DocumentElement.Attributes["location"].Value = value;
            }
        }

        public string FormName
        {
            get
            {
                return xmlDoc.DocumentElement.Attributes["name"].Value;
            }
            set
            {
                xmlDoc.DocumentElement.Attributes["name"].Value = value;
            }
        }

        public string CollectedDataDriver
        {
            get
            {
                return GetCollectedDataDbNode().Attributes["dataDriver"].Value;
            }
            set
            {
                GetCollectedDataDbNode().Attributes["dataDriver"].Value = value.ToString();
            }
        }

        protected string CollectedDataConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(collectedDataConnectionString))
                {
                    DbConnection = GetCollectedDataDbNode().Attributes["connectionString"].Value;
                    collectedDataConnectionString = Decrypt(GetCollectedDataDbNode().Attributes["connectionString"].Value);
                }
                if (this.CollectedDataDriver == "Epi.Data.Office.AccessDBFactory, Epi.Data.Office")
                    return this.SetOleDbDatabaseFilePath(collectedDataConnectionString);
                else
                    return collectedDataConnectionString;
            }
        }
        #endregion
        private String SetOleDbDatabaseFilePath(string pConnectionString)
        {
            System.Data.OleDb.OleDbConnectionStringBuilder connectionBuilder = new System.Data.OleDb.OleDbConnectionStringBuilder(pConnectionString);
            connectionBuilder.DataSource = this.FilePath.Replace(".prj", ".mdb");
            collectedDataConnectionString = connectionBuilder.ToString();
            return connectionBuilder.ToString();
        }      

        private XmlNode GetCollectedDataDbNode()
        {
            return xmlDoc.DocumentElement.SelectSingleNode("/Project/CollectedData/Database");
        }
             
        public Project(string filePath, bool Isprojectfile)
        {
            IsProjectFile = Isprojectfile;
            Construct(filePath);
        }

        private void Construct(string filePath)
        {           
            if (IsProjectFile)
            { 
                filePath = Environment.ExpandEnvironmentVariables(filePath);
                xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);
                FileInfo fileInfo = new FileInfo(filePath);
                if (string.IsNullOrEmpty(Location))
                {
                    Location = fileInfo.DirectoryName;
                }
                DbConnection = GetCollectedDataDbNode().Attributes["connectionString"].Value;
                collectedDataConnectionString = Decrypt(GetCollectedDataDbNode().Attributes["connectionString"].Value);
                    string[] Driver = this.CollectedDataDriver.Split(',');
                switch (Driver[0].Trim())
                    {
                        case "Epi.Data.Office.AccessDBFactory":
                              this.CollectedDataDriver = "Epi.Data.Office.AccessDBFactory, Epi.Data.Office";
                              AccessDBFactory accessdbfactory = new AccessDBFactory(filePath);
                              break;
                        case "Epi.Data.SqlServer.SqlDBFactory":
                        default:
                              this.CollectedDataDriver = "Epi.Data.SqlServer.SqlDBFactory, Epi.Data.SqlServer";
                              sqldbfactory = new SqlDBFactory(collectedDataConnectionString);
                              break;
                    }
            }
            else
            {
               collectedDataConnectionString = Decrypt(filePath);
               if(collectedDataConnectionString.Contains("Epi.Data.Office.AccessDBFactory"))
                {                   
                    AccessDBFactory accessdbfactory = new AccessDBFactory(filePath);
                }
               else
                {                    
                    sqldbfactory = new SqlDBFactory(collectedDataConnectionString);
                }
            }                                    
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

        public DataTable GetData()
        {
            return sqldbfactory.GetData();
        }

        public List<string> GetFields(int viewid)
        {
            return sqldbfactory.GetFields(viewid);
        }

        }
}
