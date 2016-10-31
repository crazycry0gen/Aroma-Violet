using System.ServiceProcess;
using StratCorp.CorpSMS.Service;

namespace CorpSMS.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new DeliveryReportService(),
				new ReplyService(),
				new ShortCodeService(),
				new MaintenanceService(),
                new SendingService()
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
