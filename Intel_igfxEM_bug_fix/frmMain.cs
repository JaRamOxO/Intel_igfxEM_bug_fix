using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.ServiceProcess;
using Intel_igfxEM_bug_fix.Properties;


namespace Intel_igfxEM_bug_fix
{
    public partial class frmMain : Form
    {
        private readonly string progPic = Application.StartupPath + "\\Intel_Bug_FIX512x512.png";

        private bool IsHidden = true;

        private int FixRuns = 0;
        enum ProcStat
        {
            Unknown,
            Starting,
            Running,
            Stopping,
            Stopped
        }

        private ProcStat _serviceStatus;
        private ProcStat _processStatus;
        private ProcStat ServiceStatus
        {
            get => _serviceStatus;
            set
            {
                switch (value)
                {
                    case ProcStat.Unknown:
                        lblStatusService.ForeColor = Color.DarkRed;
                        break;
                    case ProcStat.Running:
                        lblStatusService.ForeColor = Color.Red;
                        break;
                    case ProcStat.Starting:
                    case ProcStat.Stopping:
                        lblStatusService.ForeColor = Color.OrangeRed;
                        break;
                    case ProcStat.Stopped:
                        //run check to see if process also stopped...
                        lblStatusService.ForeColor = Color.Green;
                        break;
                }
                if (value != _serviceStatus)
                {
                    _serviceStatus = value;
                    lblStatusService.Text = _serviceStatus.ToString();
                }
            }
        }
        private ProcStat ProcessStatus
        {
            get => _processStatus;
            set
            {
                switch (value)
                {
                    case ProcStat.Unknown:
                        lblStatusProcess.ForeColor = Color.DarkRed;
                        break;
                    case ProcStat.Running:
                        lblStatusProcess.ForeColor = Color.Red;
                        break;
                    case ProcStat.Starting:
                    case ProcStat.Stopping:
                        lblStatusProcess.ForeColor = Color.OrangeRed;
                        break;
                    case ProcStat.Stopped:
                        //run check to see if service also stopped...
                        lblStatusProcess.ForeColor = Color.Green;
                        break;
                }
                if (value != _processStatus)
                {
                    _processStatus = value;
                    lblStatusProcess.Text = _processStatus.ToString();
                }
            }
        }
        
        public frmMain()
        {
            InitializeComponent();
            ServiceStatus = ProcStat.Unknown;
            ProcessStatus = ProcStat.Unknown;
            CheckStatus();
            if (Settings.Default.ApplyFixOnStart)
            {
                Run();
            }
        }

        private void CheckStatus()
        {
            ServiceController sc = new ServiceController();
            //make variable so servicename can be changed in config
            sc.ServiceName = Settings.Default.ServiceName;
            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    ServiceStatus = ProcStat.Running;
                    break;
                case ServiceControllerStatus.Stopped:
                    ServiceStatus = ProcStat.Stopped;
                    break;
            }
            //make variable so processname can be changed in config
            if (Process.GetProcessesByName(Settings.Default.ProcessName).Length >= 1)
            {
                ProcessStatus = ProcStat.Running;
            }
            else
            {
                ProcessStatus = ProcStat.Stopped;
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private bool StopService(string serviceName)
        {
            bool svar = true;
            ServiceController sc = new ServiceController();
            //sc.ServiceName = "igfxCUIservice2.0.0.0";
            sc.ServiceName = serviceName;
            if (sc.Status == ServiceControllerStatus.Running)
            {
                try
                {
                    // Stop the service, and wait until its status is "Stopped".
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    
                    ServiceStatus = ProcStat.Stopped;
                }
                catch (InvalidOperationException)
                {
                    svar = false;
                }
            }
            return svar;
        }

        private bool StopProcess(string procName)
        {
            bool svar = true;
            try
            {
                //make sure service is stopped...
                if (StopService("igfxCUIservice2.0.0.0"))
                {
                    foreach (var process in Process.GetProcessesByName(procName))
                    {
                        process.Kill();
                    }
                    ProcessStatus = ProcStat.Stopped;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                svar = false;
            }

            return svar;
        }

        private void SetNotText(string text)
        {
            notIcon.ShowBalloonTip(5, "Intel bug fix", text, ToolTipIcon.Info);
        }

        private void btnRunSteps_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void Run()
        {
            if (FixRuns < 3)
            {
                try
                {
                    if (!StopService(Settings.Default.ServiceName))
                    {
                        FixRuns++;
                        Run();
                    }
                    if (!StopProcess(Settings.Default.ProcessName))
                    {
                        FixRuns++;
                        Run();
                    }

                    if (ServiceStatus == ProcStat.Stopped && ProcessStatus == ProcStat.Stopped)
                    {
                        SetNotText("Fix applyed...");
                    }
                    else
                    {
                        FixRuns++;
                        Run();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Fix could not be applyed\nContact JaRam!");
            }
            
        }

        private void showToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            IsHidden = false;
            this.Show();
            if (!this.Focused)
                this.Focus();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            IsHidden = true;
            this.Hide();
        }

        private void HideMe(object sender, EventArgs e)
        {
            if (IsHidden)
            this.Hide();
        }
    }
}
