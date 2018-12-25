using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QubitControl
{
    public partial class frmSettings : Form
    {

        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (!ValidateURL()) return;


            Settings.btPassword= this.txtPassword.Text;
            Settings.btPort = int.Parse(this.txtPort.Text);
            Settings.btURL= this.txtQBURL.Text;
            Settings.btUserName= this.txtUserName.Text;
            Settings.btRequireAuth= this.chkAuth.Checked;
            this.Close();
        }

        private bool ValidateURL()
        {
            bool valid = true;
            int Port = 0;
            Uri uri = null;
            try
            {
                uri = new Uri(this.txtQBURL.Text);
                Port = uri.Port;
            }
            catch (ArgumentNullException ex)
            {
                this.errorProvider1.SetError(this.txtQBURL, "URL cannot be empty");
                valid = false;
            }
            catch (UriFormatException ex)
            {
                this.errorProvider1.SetError(this.txtQBURL, ex.Message);
                valid = false;
            }


            if (this.txtPort.Text == "")
            {
                if (Port != 0)
                {
                    this.txtPort.Text = Port.ToString();
                    this.errorProvider1.SetError(this.txtPort, "Port is mandatory. Assuming " + Port.ToString());
                    valid = false;
                }
            }
            else
            {
                if (!int.TryParse(this.txtPort.Text, out Port))
                {
                    this.errorProvider1.SetError(this.txtPort, "Port should be numeric");
                    valid = false;
                }
            }

            if (!valid) return false;

            string FullURL = uri.Scheme + "://";
            FullURL = FullURL + uri.Host;
            FullURL = FullURL + ":" + Port.ToString();
            FullURL = FullURL + uri.AbsolutePath;

            try
            {
                uri = new Uri(FullURL);
            }
            catch (Exception ex)
            {
                this.errorProvider1.SetError(this.txtQBURL, ex.Message);
                valid = false;
            }

            return valid;
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            this.txtPassword.Text = Settings.btPassword;
            this.txtPort.Text = Settings.btPort.ToString();
            this.txtQBURL.Text = Settings.btURL;
            this.txtUserName.Text = Settings.btUserName;
            this.chkAuth.Checked = Settings.btRequireAuth;
        }
    }
}
