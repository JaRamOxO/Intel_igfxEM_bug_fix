using System.Windows.Forms;
using Intel_igfxEM_bug_fix.Properties;

namespace Intel_igfxEM_bug_fix
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();

            txtProcessName.Text = Settings.Default.ProcessName;
            txtServiceName.Text = Settings.Default.ServiceName;
            chkApplyFixOnStart.Checked = Settings.Default.ApplyFixOnStart;
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            Settings.Default.ProcessName = txtProcessName.Text;
            Settings.Default.ServiceName = txtServiceName.Text;
            Settings.Default.ApplyFixOnStart = chkApplyFixOnStart.Checked;
            Settings.Default.Save();
            this.Close();
        }
    }
}
