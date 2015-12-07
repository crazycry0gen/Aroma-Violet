using Aroma_Violet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aroma_Violet.Controllers
{
    public class AccountMovementController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: AccountMovement
        [Authorize]
        public ActionResult Index(int? clientId)
        {
            var days = Generic.GetSetting<int>(db, "Default Statement Days");
            if (days == 0) {
                days = 30;
            }
            var fromDate = DateTime.Today.AddDays((Math.Abs( days) *-1));
            var model = AccountMovementViewModel.LoadJournals(db,fromDate, DateTime.Now,clientId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Index([Bind(Include = "ClientId,FromDate,toDate")] AccountMovementViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = AccountMovementViewModel.LoadJournals(db,model.FromDate, model.ToDate, model.ClientId);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult GetJournalExtraInfo(Guid journalId)
        {
            string extraInfo = string.Empty;
            const string template = "Effective Date:{0}<br/>Comment:{1}";

            var journal = db.Journals.First(m=>m.JournalId.Equals(journalId));
            extraInfo = string.Format(template,journal.EffectiveDate.ToString("dd MMM yy HH:mm"), journal.Comment);

            return Json(extraInfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}