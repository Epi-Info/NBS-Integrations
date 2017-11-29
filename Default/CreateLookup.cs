using REDCapAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Default
{
    public partial class CreateLookup : Form
    {
        private readonly Sql _objSql;
        public string Datasource
        {
            get;set;
        }
        public CreateLookup()
        {
            InitializeComponent();
            grp_Answer.Enabled = false;
            _objSql = new Sql(Application.StartupPath, CommonData.Credentials.ConnectionString);
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQuesCode.Text) && !string.IsNullOrEmpty(txtQuesIdentifier.Text) && !string.IsNullOrEmpty(txtQuesDsc.Text) && !string.IsNullOrEmpty(txtQuesDisName.Text))
            {
                if (drp_Datatype.Text != "CODED" || drp_Datatype.Text == "CODED" & !string.IsNullOrEmpty(txtAnsSysCode.Text) & !string.IsNullOrEmpty(txtAnsCodeDis.Text) & !string.IsNullOrEmpty(txtAnsDisName.Text))
                {
                    var lookup = new Settings.QuestioLookup
                    {

                        DocTypeId = "PHDC",
                        DocTypeVersionTxt = "1.3",
                        QuesCodeSysCD = txtQuesCode.Text,
                        QuesCodeSysDescTxt = txtQuesDsc.Text,
                        Data_Type = drp_Datatype.Text,
                        QuesIdentifier = txtQuesIdentifier.Text,
                        QuesDisplayName = txtQuesDisName.Text,
                        SendingSysCD = Datasource,

                        AnsFromCode = txtAnsCode.Text,
                        AnsFromCodeSysCD = txtAnsSysCode.Text,
                        AnsFromCodeSysDecsTxt = txtAnsCodeDis.Text,
                        AnsFromDisNM = txtAnsDisName.Text,
                        CodeTransReq = "NO",
                        InvestigationFormCd = "INV_FORM_GEN"
                    };

                    if (_objSql.SaveLookup(lookup))
                    {
                        CommonData.ShowMessage("Look up is created successfully in NBS database.",
                                                             CommonData.MsgBoxType.Info);
                        this.Close();
                    }
                }
                else
                {
                    CommonData.ShowMessage("Please enter the required values.",
                                                     CommonData.MsgBoxType.Info);
                }
            }
            else
            {
                CommonData.ShowMessage("Please enter the required values.",
                                                        CommonData.MsgBoxType.Info);
            }

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void drp_Datatype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(drp_Datatype.SelectedItem.ToString()=="CODED")
            {
                grp_Answer.Enabled = true;
            }
            else
                grp_Answer.Enabled = false;
        }

        private void txtQuesCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^a-zA-Z0-9\s]");
            if (regex.IsMatch(e.KeyChar.ToString()) && e.KeyChar != (char)8 && e.KeyChar != '-' && e.KeyChar != '_' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtAnsCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^a-zA-Z0-9\s]");
            if (regex.IsMatch(e.KeyChar.ToString()) && e.KeyChar != (char)8 && e.KeyChar != '-' && e.KeyChar != '_' && e.KeyChar != '.' || (Keys)e.KeyChar == Keys.Space)
            {
                e.Handled = true;
            }

        }
    }
}
