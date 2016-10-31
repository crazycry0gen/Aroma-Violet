using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace StratcolService
{
    public partial class StratcolService : ServiceBase
    {
        StratcolInterface.StratcolMain scInterface = new StratcolInterface.StratcolMain();
        private static System.Timers.Timer timer = new Timer();
        int lastDayRun = 0;

        public StratcolService()
        {
            InitializeComponent();
            scInterface.StratcolMainInit();
            if (!System.Diagnostics.EventLog.SourceExists("StratcolIntegration"))
            {
                System.Diagnostics.EventLog.CreateEventSource("StratcolIntegration", "Application");
            }
            eventLog.Source = "StratcolIntegration";
            eventLog.Log = "Application";

            timer.Elapsed += new ElapsedEventHandler(timer_Tick);
            timer.Interval = 1000;            
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("In OnStart");
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("In onStop.");
            timer.Enabled = false;
        }

        protected override void OnContinue()
        {
            eventLog.WriteEntry("In OnContinue.");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // we will submite new values every 2 hours between 10 and 17
            // we will check for updates and results only once a day after 10h
            // we will not check anything over weekends
            if (DateTime.Now.Hour < 10 || DateTime.Now.Hour > 17 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return;            

            timer.Enabled = false;

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                // Do a full check of last 120 day
                if (DateTime.Now.Day != lastDayRun)
                {
                    scInterface.UpdateOutputs(true);
                    // Update the lastDayRun Value after we have check the updates at least once
                    lastDayRun = DateTime.Now.Day;
                }
                timer.Enabled = true;
                return;
            }


            eventLog.WriteEntry("UpdateInputs");
            try
            {
                scInterface.UpdateInputs();
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("UpdateInputs " +ex.InnerException.ToString());
            }

            // only check once a day
            if (DateTime.Now.Day != lastDayRun)
            {                
                eventLog.WriteEntry("UpdateRejected");
                try
                {
                    scInterface.UpdateRejected();
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry("UpdateRejected " + ex.InnerException.ToString());
                }
                eventLog.WriteEntry("UpdateOutputs");
                try
                {
                    scInterface.UpdateOutputs(false);
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry("UpdateOutputs " + ex.InnerException.ToString());
                }
                eventLog.WriteEntry("UpdateAhv");
                try
                {
                    scInterface.UpdateAhv();
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry("UpdateAhv " + ex.InnerException.ToString());
                }
                eventLog.WriteEntry("ProcessResults");
                try
                {
                    scInterface.SearchCancelled();
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry("SearchCancelled " + ex.InnerException.ToString());
                }
                eventLog.WriteEntry("SearchCancelled");
                try
                {
                    scInterface.ProcessResults();
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry("ProcessResults " + ex.InnerException.ToString());
                }

                // Update the lastDayRun Value after we have check the updates at least once
                lastDayRun = DateTime.Now.Day;
            }
            
            if (timer.Interval == 1000) // Initial Run, this will cause the service to run immediatly when started and thereafter every 4 hours
            {
                eventLog.WriteEntry("Timer Updated to every 2 hours");
                timer.Interval = 7200000;                
            }
            timer.Enabled = true;
        }  
    }
}
