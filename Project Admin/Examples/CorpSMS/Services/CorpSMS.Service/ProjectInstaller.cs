using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace StratCorp.CorpSMS.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private EventLogInstaller _customEventLogInstaller;
        private static readonly List<string> _sources = new List<string>()
            {
                "CorpSMS Delivery Report Service",
                "CorpSMS Message Reply Service",
                "CorpSMS ShortCode Message Service",
                "CorpSMS Maintenance Service",
                "CorpSMS Sending Service"
            };
        private string _log = "Application";

        public ProjectInstaller()
        {
            InitializeComponent();

            foreach (var source in _sources)
            {
                _customEventLogInstaller = new EventLogInstaller();
                _customEventLogInstaller.Source = source;
                _customEventLogInstaller.Log = _log;
                Installers.Add(_customEventLogInstaller);
            }    
        }

        protected override void OnAfterInstall(System.Collections.IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            foreach (var source in _sources)
            {
                if (!System.Diagnostics.EventLog.SourceExists(source))
                {
                    System.Diagnostics.EventLog.CreateEventSource(source, _log);
                }
            }
        }

        private void shortCodeServiceInstaller_AfterInstall(object sender, System.Configuration.Install.InstallEventArgs e)
        {

        }
    }
}
