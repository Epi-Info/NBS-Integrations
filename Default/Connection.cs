using LogManager;
using REDCapAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Default
{
    public partial class Connection : Form
    {
        public Connection()
        {
            InitializeComponent();
            LoadConnection();
        }

        private void LoadConnection()
        {
            var objSettings = new Settings(Application.StartupPath);
            var cred = objSettings.ReadApiSettings();
            CommonData.Credentials = cred;
            Sql objSql = new Sql(Application.StartupPath, CommonData.Credentials.ConnectionString);
            string error;
            // txtPassword.PasswordChar = '*';
            if (!string.IsNullOrEmpty(objSql._connectionString))
            {
                if (objSql.TestConnection(out error))
                {
                    var csb = new SqlConnectionStringBuilder(objSql._connectionString);
                    txtServerName.Text = csb.DataSource;
                    txtDBName.Text = csb.InitialCatalog;
                    txtPassword.Text = csb.Password;
                    txtUserName.Text = csb.UserID;
                    DisableFields();                                    
                }
                else
                {
                    txtPassword.PasswordChar = '\0';
                    // CommonData.ShowMessage("Please update the connectionstring", CommonData.MsgBoxType.Info);
                    // UpdateStatus("Failed");
                }
            }
            else
            {
                btnDelete.Enabled = false;
                btnEdit.Enabled = false;
            }
        }
          

        private void btnTest_Click(object sender, EventArgs e)
        {           

            if (!string.IsNullOrEmpty(txtServerName.Text) && !string.IsNullOrEmpty(txtDBName.Text)
                && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtUserName.Text))
            {
                var dbConnectionStringBuilder = new SqlConnectionStringBuilder();
                dbConnectionStringBuilder.DataSource = txtServerName.Text;
                dbConnectionStringBuilder.InitialCatalog = txtDBName.Text;
                dbConnectionStringBuilder.UserID = txtUserName.Text;
                dbConnectionStringBuilder.Password = txtPassword.Text;
                try
                {
                    UpdateStatus("Please wait....");
                    var _connection = new SqlConnection(dbConnectionStringBuilder.ToString());
                    _connection.Open();
                    System.Windows.Forms.MessageBox.Show("Success", "NBS", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                    UpdateStatus("Success");
                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Failed", "NBS", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                    UpdateStatus("Failed");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please enter valid connection", "NBS", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
               // UpdateStatus("Failed");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtServerName.Text) && !string.IsNullOrEmpty(txtDBName.Text)
                && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtUserName.Text))
            {
               var dbConnectionStringBuilder = new SqlConnectionStringBuilder();
                dbConnectionStringBuilder.DataSource = txtServerName.Text;
                dbConnectionStringBuilder.InitialCatalog = txtDBName.Text;
                dbConnectionStringBuilder.UserID = txtUserName.Text;
                dbConnectionStringBuilder.Password = txtPassword.Text;
                Settings s = new Settings(Application.StartupPath);
                s.UpdateApiSettings(dbConnectionStringBuilder.ToString());               
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;              
                DisableFields();
                System.Windows.Forms.MessageBox.Show("Saved sucessfully","NBS", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Failed","NBS", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                UpdateStatus("Failed");

            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = '*';
            EnableFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtServerName.Text) && !string.IsNullOrEmpty(txtDBName.Text)
               && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtUserName.Text))
            {
                if (
                       CommonData.ShowMessage(
                           "This action will delete the configuration which you have already created. Do you want to continue?",
                          CommonData.MsgBoxType.Question) == DialogResult.Yes)
                {
                    Settings s = new Settings(Application.StartupPath);
                    s.UpdateApiSettings("");                   
                    txtServerName.Text = "";
                    txtDBName.Text = "";
                    txtPassword.Text = "";
                    txtUserName.Text = "";
                    txtPassword.PasswordChar = '\0';
                    EnableFields();
                }                         
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please enter valid connection", "NBS", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
            }
        }

        void UpdateStatus(string msg)
        {
            Log.WriteToApplicationLog(msg);
        }

        void EnableFields()
        {
            txtServerName.Enabled = true; 
            txtDBName.Enabled = true;
            txtPassword.Enabled = true;
            txtUserName.Enabled = true;
        }
        void DisableFields()
        {
            txtServerName.Enabled = false;
            txtDBName.Enabled = false;
            txtPassword.Enabled = false;
            txtUserName.Enabled = false;
            txtPassword.PasswordChar = '*';           

        }
    }
}
