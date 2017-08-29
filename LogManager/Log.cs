using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace LogManager
{
    public static class Log
    {
        #region "Member variables"
        //private static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string AppLogFilePath = AppPath;
        public static readonly string ErrorLogFilePath = AppPath;
        #endregion

        #region "Error Log"
        /// <summary>
        /// To write error message to error log
        /// </summary>
        /// <param name="pLogMessage"> </param>
        public static void WriteToErrorLog(string pLogMessage)
        {
            try
            {
                if (pLogMessage.Trim().Length <= 0) return;
                string errLogPath = GetErrorLogFilePath();
                string path = Path.GetDirectoryName(errLogPath);
                if (path != null && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var writer = new StreamWriter(errLogPath, true);
                writer.WriteLine("[" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + "] " + pLogMessage);
                writer.WriteLine("------------------------------------------------------------------");
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
            catch { }
        }

        public static void WriteToErrorLog1(string pLogMessage)
        {
            try
            {
                if (pLogMessage.Trim().Length <= 0) return;
                string errLogPath = GetErrorLogFilePath();
                string path = Path.GetDirectoryName(errLogPath);
                if (path != null && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var writer = new StreamWriter(errLogPath, true);
                writer.WriteLine(pLogMessage);
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
            catch { }
        }

        public static string GetAppLogFilePath()
        {
            return AppLogFilePath + string.Format(@"\Logs\App\{0}\AppLog.txt", DateTime.Now.ToShortDateString().Replace("/", "-"));
        }

        public static string GetErrorLogFilePath()
        {
            return ErrorLogFilePath + string.Format(@"\Logs\Error\{0}\ErrorLog.txt", DateTime.Now.ToShortDateString().Replace("/", "-"));
        }

        public static string GetUomLogFilePath()
        {
            return ErrorLogFilePath + string.Format(@"\Logs\Uom\{0}\Uom_Log.txt", DateTime.Now.ToShortDateString().Replace("/", "-"));
        }

        public static void WriteToErrorLog(Exception exception)
        {
            WriteToErrorLog(exception.Message + Environment.NewLine + exception.StackTrace);
        }
        /// <summary>
        /// To write error message to error log
        /// </summary>
        /// <param name="pLogMessage"> </param>
        public static void WriteToApplicationLog(string pLogMessage)
        {
            try
            {
                if (pLogMessage.Trim().Length > 0)
                {
                    string appLogPath = GetAppLogFilePath();
                    string path = Path.GetDirectoryName(appLogPath);
                    if (path != null && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var writer = new StreamWriter(appLogPath, true);
                    writer.WriteLine("[" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + "] " + pLogMessage);
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch { }
        }

        public static void WriteToApplicationLog1(string pLogMessage)
        {
            try
            {
                if (pLogMessage.Trim().Length > 0)
                {
                    string appLogPath = GetAppLogFilePath();
                    string path = Path.GetDirectoryName(appLogPath);
                    if (path != null && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var writer = new StreamWriter(appLogPath, true);
                    writer.WriteLine(pLogMessage);
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch { }
        }


        /// <summary>
        /// To write error message to error log
        /// </summary>
        /// <param name="pLogMessage"> </param>
        /// <param name="fileName"></param>
        public static void WriteToSearsXml(string pLogMessage, string fileName)
        {
            try
            {
                if (pLogMessage.Trim().Length > 0)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.InnerXml = pLogMessage;

                    string path = AppPath + "\\Sears";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string appLogPath = path + "\\" + fileName + ".xml";
                    xml.Save(appLogPath);
                }
            }
            catch { }
        }

        /// <summary>
        /// To get application log
        /// </summary>
        /// <param name="pLogFileType"> </param>
        public static string GetLog(LogFileType pLogFileType)
        {
            string retStr = "";
            try
            {
                string filePath;
                switch (pLogFileType)
                {
                    case LogFileType.Application:
                        filePath = AppLogFilePath;
                        break;
                    case LogFileType.Error:
                        filePath = ErrorLogFilePath;
                        break;
                }
                filePath = GetAppLogFilePath();
                if (File.Exists(filePath))
                {
                    var reader = new StreamReader(filePath);
                    retStr = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch { }
            return retStr;
        }

        public static string ReadAscFile(string path)
        {
            string retStr = "";
            try
            {
                if (File.Exists(path))
                {
                    var reader = new StreamReader(path);
                    retStr = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch { }
            return retStr;
        }

        /// <summary>
        /// Used to clear the application log
        /// </summary>
        /// <param name="pLogFileType"></param>
        /// <returns></returns>
        public static void ClearLog(LogFileType pLogFileType)
        {
            try
            {
                string filePath;
                switch (pLogFileType)
                {
                    case LogFileType.Application:
                        filePath = AppLogFilePath;
                        break;
                    case LogFileType.Error:
                        filePath = ErrorLogFilePath;
                        break;
                }
                try
                {
                    filePath = GetAppLogFilePath();
                    if (File.Exists(filePath))
                    {
                        var writer = new StreamWriter(filePath, false);
                        writer.WriteLine("-------------------------------------------------------------------------");
                        writer.WriteLine("Log cleared at " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                        writer.WriteLine("-------------------------------------------------------------------------");
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                    }
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// To handle Log file name
        /// </summary>
        public enum LogFileType
        {
            Application,
            Error
        }
        #endregion

        //public static void WriteToErrorLog1(string pLogMessage)
        //{
        //    try
        //    {
        //        if (pLogMessage.Trim().Length > 0)
        //        {
        //            string errLogPath = GetErrorLogFilePath();
        //            string path = Path.GetDirectoryName(errLogPath);
        //            if (path != null && !Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }
        //            var writer = new StreamWriter(errLogPath, true);
        //            writer.WriteLine(pLogMessage);
        //            writer.Flush();
        //            writer.Close();
        //            writer.Dispose();
        //        }
        //    }
        //    catch { }
        //}

        public static void WriteLinesInLog(string pLogMessage)
        {
            try
            {
                if (pLogMessage.Trim().Length > 0)
                {
                    string errLogPath = GetErrorLogFilePath();
                    string path = Path.GetDirectoryName(errLogPath);
                    if (path != null && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var writer = new StreamWriter(errLogPath, true);
                    writer.WriteLine(pLogMessage);
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch { }
        }

        public static void UpdateCSV(string[] lines, string csvpath)
        {
            try
            {
                if (lines.Length > 0)
                {
                    string csvFilePath = csvpath;
                    string path = Path.GetDirectoryName(csvFilePath);
                    if (path != null && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var writer = new StreamWriter(csvFilePath, false);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i == lines.Length - 1)
                        {

                        }
                        
                        var line = GetLine(lines, i);
                        if (string.IsNullOrEmpty(line)) continue;
                        writer.WriteLine(line);
                    }
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch { }
        }


        private static string GetLine(string[] lines, int currentindex)
        {
            int index = 0;
            string prevString = "";
            int iterationCount = 0;
            for (int i = currentindex; i < lines.Length; i++)
            {
                string tempLine = lines[i];
            
                prevString += (" " + tempLine).Trim();
                if (string.IsNullOrEmpty(tempLine))
                {
                    //if (index == lines.Length)
                    //{
                    //    continue;
                    //}
                    //prevString += tempLine;
                    if (iterationCount == 0)
                    {
                        return "";
                    }
                    continue;
                }
                if (tempLine.Length < 100)
                {
                    //prevString += tempLine;
                    if (iterationCount == 0)
                    {
                        return "";
                    }
                    continue;
                }
                if (tempLine.StartsWith("\""))
                {
                    //prevString += tempLine;
                    if (iterationCount == 0)
                    {
                        return "";
                    }
                    return prevString;
                }
                iterationCount++;
                if (iterationCount == 2)
                {
                    return lines[currentindex];
                }
            }

            return lines[currentindex];
        }

        public static void WriteMissingUoM(IEnumerable<string> uomErrors)
        {
            foreach (string uom in uomErrors)
            {
                string uomLogPath = GetUomLogFilePath();
                string path = Path.GetDirectoryName(uomLogPath);
                if (path != null && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var writer = new StreamWriter(uomLogPath, true);
                writer.WriteLine(uom);
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }
    }
}
