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

        public class ItemDisplay<TValue>
        {
            private readonly string m_displayText;

            public ItemDisplay(TValue value, String displayText)
            {
                this.Value = value;
                m_displayText = displayText;
            }

            public TValue Value { get; set; }

            public override string ToString()
            {
                return m_displayText;
            }
        }

      /*  private void btnTest_Click(object sender, EventArgs e)
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
        }*/

        private void btnTest_Click(object sender, EventArgs e)
        {
            Connection objConnection = new Connection();
            objConnection.ShowDialog(this);
        }

        void UpdateStatus(string msg)
        {
           Log.WriteToApplicationLog(msg);
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

        private void redCapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objSettings = new Configuration();
            objSettings.ShowDialog(this);

        }

        private void epiInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objSettings = new EpiConfiguration();
            objSettings.ShowDialog(this);           
                
        }
    }
}
