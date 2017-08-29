using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Data.OleDb;

namespace REDCapAPI
{
    public class RedCapClient
    {
        private string _strUri;
        private string _strPostToken;
        static readonly DataTable ODt = new DataTable();

        DataTable GetTableFromAnyRc(string strRecordsSelect, string strFields,
              string strEvents, string strForms, bool boolLabels, bool boolAccessGroups)
        {
            Debug.WriteLine("GetTableFromAnyRC()");
            string strRecord;
            CSVDoc csvDoc;

            DataTable dtDataTable;

            string strPostParameters = "&content=record&format=xml&type=flat&eventName=unique";

            if (strRecordsSelect != "")
            {
                strPostParameters += "&records=" + strRecordsSelect;
            }

            if (strFields != "")
            {
                strPostParameters += "&fields=" + strFields;
            }

            if (strEvents != "")
            {
                strPostParameters += "&events=" + strEvents;
            }

            if (strForms != "")
            {
                strPostParameters += "&forms=" + strForms;
            }

            if (boolLabels)
                strPostParameters += "&rawOrLabel=label";
            else
                strPostParameters += "&rawOrLabel=raw";

            // probably should take out if you are going to import this exported data
            if (boolAccessGroups)
            {
                strPostParameters += "&exportDataAccessGroups=true";
            }

            byte[] bytePostData = Encoding.UTF8.GetBytes("token=" + _strPostToken + strPostParameters);

            DataTable dt = ResponseHttp(bytePostData);
            return dt;
            const string strResponse = "";
            // if no records found, there are no fields.  new in RC 6 and greater
            // have to deal with null DataTable in your call to this function
            // if Rc < 6, then it will return field names without any rows of data
            if (strResponse == "\n")
            {
                return (dtDataTable);
            }

            // should return the first field you expect. otherwise it is error
            //if (strResponse.Substring(0, intReturnLength) != strReturnCheck)
            //{
            //    throw new Exception("RC Error: " + strResponse);
            //}

            csvDoc = new CSVDoc(strResponse);

            // first line of .csv is column names
            strRecord = csvDoc.ReadLine();

            // get column headers
            string[] strColumnHeaders = strRecord.Split(',');

            // set up table
            for (int i = 0; i < strColumnHeaders.Length; i++)
            {
                dtDataTable.Columns.Add(strColumnHeaders[i], typeof(string));
            }

            // now read all data and assign to data table
            while ((strRecord = csvDoc.ReadLine()) != null)
            {
                CSVLine csvLine = new CSVLine(strRecord);

                DataRow drRecord = dtDataTable.NewRow();

                // now get fields
                for (int i = 0; i < strColumnHeaders.Length; i++)
                {
                    drRecord[i] = csvLine.ReadField();
                }

                dtDataTable.Rows.Add(drRecord);
            }

            return (dtDataTable);
        }

        // will return a csv string to import, given a data table
        // if given any field string (like "studyid,redcap_event_name,field1,field2") will only put those fields in the csv string
        private string GetCSVFromTable(DataTable dtData, string strFields)
        {
            Debug.WriteLine("GetCSVFromTable()");

            string strCSVContents = "";

            int i = 0;
            int j = 0;

            try
            {
                string[] strFieldsArray = strFields.Split(',');

                //Write into csv format
                for (i = 0; i < dtData.Columns.Count; i++)
                {
                    if (strFields == "")
                    {
                        if (i > 0)
                            strCSVContents += ",";

                        strCSVContents += dtData.Columns[i].ColumnName;
                    }
                    else
                    {
                        if (InArray(strFieldsArray, dtData.Columns[i].ColumnName))
                        {
                            if (i > 0)
                                strCSVContents += ",";

                            strCSVContents += dtData.Columns[i].ColumnName;
                        }
                    }
                }

                strCSVContents += "\n";

                for (i = 0; i < dtData.Rows.Count; i++)
                {
                    for (j = 0; j < dtData.Columns.Count; j++)
                    {
                        if (strFields == "")
                        {
                            if (j > 0)
                                strCSVContents += ",";

                            // double quote all fields. replace any double quotes in field with escape clause.
                            // this allows things like inches (") to be put in fields, or any quote marks
                            strCSVContents += "\"" + dtData.Rows[i][j].ToString().Replace("\"", "\\\"") + "\"";
                        }
                        else
                        {
                            if (InArray(strFieldsArray, dtData.Columns[j].ColumnName))
                            {
                                if (j > 0)
                                    strCSVContents += ",";

                                // double quote all fields. replace any double quotes in field with escape clause.
                                // this allows things like inches (") to be put in fields, or any quote marks
                                strCSVContents += "\"" + dtData.Rows[i][j].ToString().Replace("\"", "\\\"") + "\"";
                            }
                        }
                    }

                    strCSVContents += "\n";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RC Error: " + ex.Message, ex);
            }

            return (strCSVContents);
        }

        // if value found in array, will return true
        private bool InArray(IEnumerable<string> strArray, string strText)
        {
            foreach (string strItem in strArray)
            {
                if (strItem == strText)
                {
                    return true;
                }
            }
            return false;
        }

        // will import a csv string into RC. 
        // remember that ONLY data exported in 'raw' mode can be imported (I think 'label' will not work)
        private DataTable RcImportCSVFlat(string strPostToken, string strCSVContents, bool boolOverwrite)
        {
            string strPostParameters = "&content=record&format=csv&type=flat&overwriteBehavior=overwrite&data=" + strCSVContents;
            if (boolOverwrite)
                strPostParameters += "&overwriteBehavior=overwrite";
            strPostParameters += "&data=" + strCSVContents;
            byte[] bytePostData = Encoding.ASCII.GetBytes("token=" + strPostToken + strPostParameters);
            return ResponseHttp(bytePostData);
        }

        // makes the API call and returns response from request
        DataTable ResponseHttp(byte[] bytePostData)
        {
            Debug.WriteLine("responseHTTP()");
            string strResponse;

            try
            {
                HttpWebRequest webreqRedCap = (HttpWebRequest)WebRequest.Create(_strUri);

                webreqRedCap.Method = "POST";
                webreqRedCap.ContentType = "application/x-www-form-urlencoded";
                webreqRedCap.ContentLength = bytePostData.Length;

                // Get the request stream and read it
                Stream streamData = webreqRedCap.GetRequestStream();
                streamData.Write(bytePostData, 0, bytePostData.Length);
                streamData.Close();

                HttpWebResponse webrespRedCap = (HttpWebResponse)webreqRedCap.GetResponse();

                //Now, read the response (the string), and output it.
                Stream streamResponse = webrespRedCap.GetResponseStream();
                StreamReader readerResponse = new StreamReader(streamResponse);

                strResponse = readerResponse.ReadToEnd();
                StringReader theReader = new StringReader(strResponse);
                DataSet theDataSet = new DataSet();
                theDataSet.ReadXml(theReader);
                return theDataSet.Tables[0];
            }
            //catch (WebException exWe)
            //{
            //    Stream streamWe = exWe.Response.GetResponseStream();
            //    var sbResponse = new StringBuilder("", 65536);

            //    try
            //    {
            //        var readBuffer = new byte[1000];

            //        for (; ; )
            //        {
            //            if (streamWe != null)
            //            {
            //                int intCnt = streamWe.Read(readBuffer, 0, readBuffer.Length);

            //                if (intCnt == 0)
            //                {
            //                    break;
            //                }
            //                sbResponse.Append(Encoding.UTF8.GetString(readBuffer, 0, intCnt));
            //            }
            //        }
            //    }
            //    finally
            //    {
            //        if (streamWe != null) streamWe.Close();
            //        strResponse = sbResponse.ToString();
            //    }
            //}
            catch (Exception ex)
            {
                strResponse = ex.Message;
                LogManager.Log.WriteToErrorLog(ex);
            }
            return null;
        }

        //----------------------------------------------
        /// Sample GetData code. gives you an idea of how to use above functions
        public DataTable GetData(string url, string token, string formname)
        {
            _strUri = url;
            _strPostToken = token;
            try
            {
                DataTable dtRcTable = GetTableFromAnyRc("", "", "", formname, false, false);
                //foreach (DataRow row in dtRcTable.Rows)
                //{
                //    foreach (DataColumn col in dtRcTable.Columns)
                //    {
                //        Console.WriteLine(row[col]);
                //        bFound = FindNbsField(col.ColumnName);
                //    }
                //}
                return dtRcTable;
            }
            catch (Exception ex)
            {
               LogManager.Log.WriteToErrorLog(ex);
            }
            return null;
        }

        static Boolean FindNbsField(string sRedCapField)
        {
            string sQuery = "REDCAPFieldName LIKE '%" + sRedCapField + "%'";
            DataRow[] result = ODt.Select(sQuery);
            if (result.Length > 0)
            {
                return true;
            }
            return false;
        }

        static DataTable ExcelToDataTable(string pathName, string sheetName)
        {
            DataTable tbContainer = new DataTable();
            string strConn = string.Empty;
            if (string.IsNullOrEmpty(sheetName)) { sheetName = "Sheet1"; }
            FileInfo file = new FileInfo(pathName);
            if (!file.Exists) { throw new Exception("Error, file doesn't exists!"); }
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                    break;
                default:
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
            }
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            OleDbDataAdapter oda = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheetName), cnnxls);
            DataSet ds = new DataSet();
            oda.Fill(tbContainer);
            return tbContainer;
        }

        // sample SetData code.  gives you an idea of how to use the above functions
        public void SetData(DataTable dtData)
        {
            string strCSVContents = "";

            try
            {
                // makes a csv string from all columns in data table
                strCSVContents = GetCSVFromTable(dtData, "");

                // imports this csv string to RC using API, with overwrite
                RcImportCSVFlat(_strPostToken, strCSVContents, true);

            }
            catch (Exception ex)
            {
                // whatever you want to do with exception
            }
            finally
            {
                // any actions needed here
            }
        }

    }
}
