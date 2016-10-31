using System.ServiceProcess;
using System.Timers;

namespace StratCorp.CorpSMS.Service
{
    partial class ShortCodeService : ServiceBase
    {
        private static readonly string SOURCE = "CorpSMS ShortCode Message Service";
        private static readonly int INTERVAL = 5;

        private Timer _timer = new Timer();

        public ShortCodeService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int timerInterval = INTERVAL * 60000;
            _timer.Enabled = true;
            _timer.Interval = timerInterval;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();

        }

        protected void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ServiceHelpers.SafeExecutePublicMethod(() =>
                {
                    ServiceHelpers.ProxyService().RefreshShortCodes();
                    ServiceHelpers.ProxyService().Close();
                }, SOURCE, _timer);
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;
        }
    }
}
