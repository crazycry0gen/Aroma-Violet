using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aroma_Violet.Models;

namespace Aroma_Violet.Controllers
{
    public class SystemEventsController : Controller
    {
        private AromaContext db = new AromaContext();
        public JsonResult RunAutoEvents()
        {
            CheckPostalCodes();
            CheckPostalCodePostageCharges();
            var result = Json(string.Empty);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        private void CheckPostalCodePostageCharges()
        {
            var eventDetail = db.SystemEvents.Find(Generic.SystemEventCheckPostal);
            if (eventDetail != null && eventDetail.UserId.HasValue)
            {
                var codes = db.PostalCodes.Where(m=>m.Active).ToArray();
                foreach (var code in codes)
                {
                    var charge = (from item in db.PostalCodePostageCharges
                                  where item.PostalCodeId == code.PostalCodeId
                                  select item).FirstOrDefault();
                    if (charge == null)
                    {
                        var msg = string.Format("Postal code {0} does not have a postage charge set up", code.PostalCodeName);

                        //create a note
                        var note = (from item in db.SystemNotes
                                    where item.NoteText == msg
                                    select item).FirstOrDefault();

                        if (note == null)
                        {
                            int clientId = 0;
                            var clientIds = (from item in db.Addresses
                                            where item.Code == code.PostalCodeName
                                            select item.ClientID).ToArray();

                            if (clientIds.Length > 0) clientId = clientIds.Max();

                            if (clientId > 0)
                            {
                                note = new SystemNote()
                                {
                                    UserID = eventDetail.UserId.Value,
                                    Created = DateTime.Now,
                                    NoteId = Guid.NewGuid(),
                                    NoteText = msg
                                };
                                db.SystemNotes.Add(note);
                                db.SaveChanges();


                                SystemTicketTemplatesController.PersistTicket(db, Generic.SystemEventCheckPostal,
                                    note.NoteId,
                                    clientId,
                                    Generic.SupportTicketTypeIdSystem,
                                    msg,
                                    eventDetail.UserId.Value);
                            }
                        }
                    }
                }
            }
        }

        private void CheckPostalCodes()
        {

            var eventDetail = db.SystemEvents.Find(Generic.SystemEventCheckPostal);
            if (eventDetail != null && eventDetail.UserId.HasValue)
            {
                var clientPostalCodes = (from item in db.Addresses
                                         where item.AddressTypeID == 2
                                         && item.ClientID>0
                                         select item).ToArray();
                var clientPostalCodeGrouped = (from item in clientPostalCodes
                                               group item by item.Code into grp
                                               select grp).ToArray();


                foreach (var grouped in clientPostalCodeGrouped)
                {
                    var code = (from item in db.PostalCodes
                                where item.PostalCodeName == grouped.Key
                                select item).FirstOrDefault();
                    if (code == null)
                    {

                        var msg = string.Format("Postal code \"{0}\" does not exist but is used by {1} client(s) as a shipping address code. First client: {2}", grouped.Key, grouped.Count(), grouped.First().ClientID);

                        //create a note
                        var note = (from item in db.SystemNotes
                                    where item.NoteText == msg
                                    select item).FirstOrDefault();

                        if (note == null)
                        {
                            note = new SystemNote()
                            {
                                UserID = eventDetail.UserId.Value,
                                Created = DateTime.Now,
                                NoteId = Guid.NewGuid(),
                                NoteText = msg
                            };
                            db.SystemNotes.Add(note);
                            db.SaveChanges();

                            SystemTicketTemplatesController.PersistTicket(db, Generic.SystemEventCheckPostal,
                                note.NoteId,
                                grouped.First().ClientID,
                                Generic.SupportTicketTypeIdSystem,
                                msg,
                                eventDetail.UserId.Value);

                        }
                    }
                }
            }
        }
        // GET: SystemEvents
        public async Task<ActionResult> Index()
        {
            ViewBag.Users = GetUsers();
            return View(await db.SystemEvents.ToListAsync());
        }

        // GET: SystemEvents/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemEvent systemEvent = await db.SystemEvents.FindAsync(id);
            if (systemEvent == null)
            {
                return HttpNotFound();
            }

            return View(systemEvent);
        }

        private KeyValuePair<Guid,string>[] GetUsers()
        {
            var context = new ApplicationDbContext();
            var users = (from item in context.Users.ToArray()
                         orderby item.Email
                         select new KeyValuePair<Guid, string>(Guid.Parse(item.Id), item.Email)).ToArray();
            return users;
        }

        // GET: SystemEvents/Create
        public ActionResult Create()
        {
            var users = GetUsers();
            ViewBag.UserId = new SelectList(users, "Key", "Value");
            return View();
        }

        // POST: SystemEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SystemEventId,SystemEventName,UserId")] SystemEvent systemEvent)
        {
            if (ModelState.IsValid)
            {
                systemEvent.SystemEventId = Guid.NewGuid();
                db.SystemEvents.Add(systemEvent);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var users = GetUsers();
            ViewBag.UserId = new SelectList(users, "Key", "Value", systemEvent.UserId);
            return View(systemEvent);
        }

        // GET: SystemEvents/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemEvent systemEvent = await db.SystemEvents.FindAsync(id);
            if (systemEvent == null)
            {
                return HttpNotFound();
            }
            var users = GetUsers();
            ViewBag.UserId = new SelectList(users, "Key", "Value", systemEvent.UserId);
            return View(systemEvent);
        }

        // POST: SystemEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SystemEventId,SystemEventName,UserId")] SystemEvent systemEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemEvent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var users = GetUsers();
            ViewBag.UserId = new SelectList(users, "Key", "Value", systemEvent.UserId);
            return View(systemEvent);
        }

        // GET: SystemEvents/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemEvent systemEvent = await db.SystemEvents.FindAsync(id);
            if (systemEvent == null)
            {
                return HttpNotFound();
            }
            return View(systemEvent);
        }

        // POST: SystemEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            SystemEvent systemEvent = await db.SystemEvents.FindAsync(id);
            db.SystemEvents.Remove(systemEvent);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
