using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NovusBackProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            var urlList = new string[] {
                "http://192.168.0.2/Applications/NovusTest/RebateClientTypes/CalculateSubscriptionRebates",
                "http://192.168.0.2/Applications/NovusTest/RebateClientTypes/CalculateGroupSalesRebates"
            };
            foreach (var url in urlList)
            {
                CallUrl(url);
            }
        }

        static void CallUrl(string url)
        {

            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(url);
            }

        }
    }
}
