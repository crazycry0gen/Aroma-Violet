using System.ServiceProcess;
using System.Timers;

namespace StratCorp.CorpSMS.Service
{
    partial class DeliveryReportService : ServiceBase
    {
        private static readonly string SOURCE = "CorpSMS Delivery Report Service";
        private static readonly int INTERVAL = 5;

        private Timer _timer = new Timer();

        public DeliveryReportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int timerInterval = INTERVAL * 60000;
            _timer.Enabled = true;
            _timer.Interval = timerInterval;
            _timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            _timer.Start();
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            ServiceHelpers.SafeExecutePublicMethod(() =>
                {
                    ServiceHelpers.ProxyService().RefreshDeliveryReport();
                    ServiceHelpers.ProxyService().Close();
                }, SOURCE, _timer);
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;
        }
    }
}
