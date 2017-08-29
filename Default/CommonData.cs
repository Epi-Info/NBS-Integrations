using System;

namespace Default
{
    public static class CommonData
    {
        #region "Member variables"

        public const string AppName = "Red CAP";
        public static REDCapAPI.Settings.Credentials Credentials;
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        public const string NoErrorLogFound = "No error log found.";
        public static bool TemplateSaved;
        public const string FileFilter = "Excel File(2003-2010)(*.xls;*.xlsx;*.csv)|*.xls;*.xlsx;*.csv";
        public const string Quote = "\"";
        public const string Comma = ",";

        #endregion

        public enum TransactionType
        {
            CreditMemo,
            SalesReceipt
        }

        #region "MessageBox"
        /// <summary>
        /// To show message box
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgType"></param>
        /// <returns></returns>
        public static System.Windows.Forms.DialogResult ShowMessage(string msg, MsgBoxType msgType)
        {
            System.Windows.Forms.DialogResult dlgResult = System.Windows.Forms.DialogResult.No;

            switch (msgType)
            {
                case MsgBoxType.Error:
                    System.Windows.Forms.MessageBox.Show(msg, AppName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                    break;
                case MsgBoxType.Info:
                    System.Windows.Forms.MessageBox.Show(msg, AppName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                    break;
                case MsgBoxType.Question:
                    dlgResult = System.Windows.Forms.MessageBox.Show(msg, AppName, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                    break;
            }
            return dlgResult;
        }

        /// <summary>
        /// To handle message box type
        /// </summary>
        public enum MsgBoxType
        {
            Info,
            Question,
            Error
        }
        #endregion
    }
}
