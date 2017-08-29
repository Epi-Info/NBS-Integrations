using System;
using System.Windows.Forms;
using LogManager;
using REDCapAPI;

namespace Default
{
    public partial class Splash : Form
    {
        private readonly Sql _objSql;

        public Splash()
        {
            InitializeComponent();
            var objSettings = new Settings(Application.StartupPath);
            var cred = objSettings.ReadApiSettings();
            CommonData.Credentials = cred;
            _objSql = new Sql(Application.StartupPath, CommonData.Credentials.ConnectionString);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Sql objSql = new Sql(Application.StartupPath, CommonData.Credentials.ConnectionString);
            string error;
            if (objSql.TestConnection(out error))
            {
                CommonData.ShowMessage("Success", CommonData.MsgBoxType.Info);
                UpdateStatus("Success");
            }
            else
            {
                CommonData.ShowMessage("Failed", CommonData.MsgBoxType.Info);
                UpdateStatus("Failed");
            }
        }

        void UpdateStatus(string msg)
        {
           Log.WriteToApplicationLog(msg);
        }

        private void mnuSettings_Click(object sender, EventArgs e)
        {
            var objSettings = new Configuration();
            objSettings.ShowDialog(this);
        }

        private void mnuMapping_Click(object sender, EventArgs e)
        {
            UpdateStatus("Please wait.. It is opening...");
            Mapping objMaps = new Mapping();
            objMaps.ShowDialog(this);
        }

        private void mainScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainScreen mainObj = new MainScreen();
            mainObj.ShowDialog(this);
        }
    }
}
