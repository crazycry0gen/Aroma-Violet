namespace StratCorp.CorpSMS.Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.replyServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.shortCodeServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.deliveryReportServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.maintenanceServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.sendingServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // replyServiceInstaller
            // 
            this.replyServiceInstaller.ServiceName = "CorpSMS Reply Service";
            this.replyServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // shortCodeServiceInstaller
            // 
            this.shortCodeServiceInstaller.ServiceName = "CorpSMS ShortCode Message Service";
            this.shortCodeServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.shortCodeServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.shortCodeServiceInstaller_AfterInstall);
            // 
            // deliveryReportServiceInstaller
            // 
            this.deliveryReportServiceInstaller.ServiceName = "CorpSMS Delivery Report Service";
            this.deliveryReportServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // maintenanceServiceInstaller
            // 
            this.maintenanceServiceInstaller.DelayedAutoStart = true;
            this.maintenanceServiceInstaller.ServiceName = "CorpSMS Maintenance Service";
            this.maintenanceServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // sendingServiceInstaller
            // 
            this.sendingServiceInstaller.ServiceName = "CorpSMS Sending Service";
            this.sendingServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.replyServiceInstaller,
            this.shortCodeServiceInstaller,
            this.deliveryReportServiceInstaller,
            this.sendingServiceInstaller,
            this.maintenanceServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller replyServiceInstaller;
        private System.ServiceProcess.ServiceInstaller shortCodeServiceInstaller;
        private System.ServiceProcess.ServiceInstaller deliveryReportServiceInstaller;
        private System.ServiceProcess.ServiceInstaller maintenanceServiceInstaller;
        private System.ServiceProcess.ServiceInstaller sendingServiceInstaller;
    }
}