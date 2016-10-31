using System.ServiceProcess;
using System.Timers;

namespace StratCorp.CorpSMS.Service
{
    partial class SendingService : ServiceBase
    {
        private static readonly string SOURCE = "CorpSMS Sending Service";
        private static readonly int INTERVAL = 300000; // 5 minutes

        private Timer _timer = new Timer();

        public SendingService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int timerInterval = INTERVAL;
            _timer.Enabled = true;
            _timer.Interval = timerInterval;
            _timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            _timer.Start();
        }

        protected void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            ServiceHelpers.SafeExecutePublicMethod(() =>
                {
                    ServiceHelpers.ProxyService().SendAllQueuedMessages();
                    ServiceHelpers.ProxyService().Close();
                }, SOURCE, _timer);
            _timer.Interval = INTERVAL;
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;
        }
    }
}
