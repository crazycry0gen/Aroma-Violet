using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace StratCorp.CorpSMS.Service
{
    partial class MaintenanceService : ServiceBase
    {
        private static readonly string SOURCE = "CorpSMS Maintenance Service";
        
        private string _path = @"C:\CorpSMS";
        private Timer _timer = new Timer(3600000); // 1 Hour


        public MaintenanceService()
        {
            InitializeComponent();
            this._timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            ServiceHelpers.SafeExecutePublicMethod(() =>
                {
                    if (Directory.Exists(_path))
                    {
                        foreach (var file in Directory.GetFiles(_path))
                        {
                            // Delete all files older than 7 days
                            if (File.GetCreationTime(file).CompareTo(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0))) == -1)
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }, SOURCE, _timer);
        }

        protected override void OnStart(string[] args)
        {
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;
        }
    }
}
