using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NovusStoredProcedureExec
{
    public partial class NovusStoredProcedureExec : ServiceBase
    {

        private const string source = "NovusSPExec";
        private const string eventLogName = "Novus";
        public NovusStoredProcedureExec()
        {
            InitializeComponent();
            try
            {
                eventNovus = new EventLog();
                eventNovus.Source = source;
                eventNovus.Log = eventLogName;
            }
            catch (Exception ex)
            {
                //if (!System.Diagnostics.EventLog.SourceExists(source))
                //{
                EventLog.CreateEventSource(source, eventLogName);
                eventNovus = new EventLog();
                eventNovus.Source = source;
                eventNovus.Log = eventLogName;
                //}
            };

        }

        private System.Timers.Timer timer;
        protected override void OnStart(string[] args)
        {
            if (true) System.Diagnostics.Debugger.Launch();

            // Set up a timer to trigger every minute.
            timer = new System.Timers.Timer();
            timer.Interval = 5000; // 60 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            using (var db = new AromaVioletDataContext())
            {
                var executeAt = DateTime.Now;
                var procedures = (from item in db.SystemProcedures
                                  where item.Active && item.LastRun.AddMilliseconds((double)(item.Interval * item.SystemIntervalSpecifier.MilisecondConverter)) < executeAt
                                  select item).ToArray();
                foreach (var procedure in procedures)
                {
                    try
                    {
                        procedure.LastRun = DateTime.Now;
                        db.ExecuteCommand(procedure.ProcedureName);
                    }
                    catch(Exception ex)
                    {
                        var msg = new SystemProcedureMessage() {Message = ex.Message, MessageDate=DateTime.Now, SystemProcedureId=procedure.SystemProcedureId,SystemProcedureMessageId=Guid.NewGuid() };
                        db.SystemProcedureMessages.InsertOnSubmit(msg);
                    }
                        db.SubmitChanges();
                }
                var nextRun = (from item in db.SystemProcedures
                               where item.Active
                               select item.LastRun.AddMilliseconds((double)(item.Interval * item.SystemIntervalSpecifier.MilisecondConverter))).Max();
                var msec = nextRun.Subtract(DateTime.Now).TotalMilliseconds;
                timer.Interval = msec;
                timer.Start();
            }
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer = null;
        }
    }
}
