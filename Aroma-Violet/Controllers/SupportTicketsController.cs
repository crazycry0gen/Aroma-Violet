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
using Microsoft.AspNet.Identity;

namespace Aroma_Violet.Controllers
{
    public class SupportTicketsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SupportTickets
        [Authorize]
        public async Task<ActionResult> Index(Guid? currentUserId, int statusMap = 0)
        {
            string description = "View All Tickets";

            if (statusMap == 0)
            {
                statusMap = currentUserId.HasValue ? 4 : 14;
            }

            var supportTickets = db.SupportTickets.Where(m=>((int)Math.Pow(2, m.SupportTicketStatusID) & statusMap) == (int)Math.Pow(2,m.SupportTicketStatusID) );
            
            if (currentUserId.HasValue)
            {
                supportTickets = supportTickets.Where(m => m.UserID == currentUserId.Value
                                                            ).Include(s => s.Client).Include(s => s.SupportTicketStatus);
                description = "View My Tickets";
            }

            ViewBag.Title = description;
            var context = new ApplicationDbContext();
            var data = (from item in context.Users.ToArray()
                        select new KeyValuePair<string,string>(item.Id, item.UserName )).ToArray();

            ViewBag.UserID = data;
            ViewBag.currentUserId = currentUserId;
            ViewBag.statusMap = statusMap;
            return View(await supportTickets.OrderByDescending(m=>m.iDate).ToListAsync());
        }

        [Authorize]
        public ActionResult UserIndex(int statusMap=0 )
        {
            var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
            return RedirectToAction("Index", new { currentUserId = currentUserId,statusMap=statusMap  });
        }

        // GET: SupportTickets/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportTicket supportTicket = await db.SupportTickets.FindAsync(id);
            if (supportTicket == null)
            {
                return HttpNotFound();
            }
            return View(supportTicket);
        }

        [Authorize]
        // GET: SupportTickets/Create
        public ActionResult Create(int clientId =0)
        {
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientClientId");
            ViewBag.SupportTicketStatusID = new SelectList(db.SupportTicketStatuses, "SupportTicketStatusId", "SupportTicketStatusName");
            ViewBag.SupportTicketTypeId = new SelectList(db.SupportTicketTypes, "SupportTicketTypeId", "SupportTicketTypeName");
            var newTicket = new SupportTicket() {iDate = DateTime.Now, SupportTicketStatus = db.SupportTicketStatuses.Where(m=>m.SupportTicketStatusName=="New").First() };
            newTicket.SupportTicketStatusID = newTicket.SupportTicketStatus.SupportTicketStatusId;
            if (clientId > 0)
                newTicket.ClientID = clientId;
            return View(newTicket);
        }

        // POST: SupportTickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SupportTicketId,Description,ClientID,UserID,SupportTicketStatusID,SupportTicketTypeId,iDate")] SupportTicket supportTicket)
        {
            var client = db.Clients.FirstOrDefault(m=>m.ClientId == supportTicket.ClientID);
            if (client == null)
            {
                ModelState.AddModelError("ClientID", "Client does not exists.");
            }
            else if (ModelState.IsValid)
            {
                supportTicket.SupportTicketId = Guid.NewGuid();
                db.SupportTickets.Add(supportTicket);
                await db.SaveChangesAsync();
                var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
                return RedirectToAction("Edit", new {id=supportTicket.SupportTicketId,statusMap=0, currentUserId=currentUserId });
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", supportTicket.ClientID);
            ViewBag.SupportTicketStatusID = new SelectList(db.SupportTicketStatuses, "SupportTicketStatusId", "SupportTicketStatusName", supportTicket.SupportTicketStatusID);
            ViewBag.SupportTicketTypeId = new SelectList(db.SupportTicketTypes, "SupportTicketTypeId", "SupportTicketTypeName");
            return View(supportTicket);
        }

        public async Task<ActionResult> Complete(Guid? id, Guid? currentUserId, int statusMap)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportTicket supportTicket = await db.SupportTickets.FindAsync(id);
            if (supportTicket == null)
            {
                return HttpNotFound();
            }
            supportTicket.SupportTicketStatusID = 3;
            db.Entry(supportTicket).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { currentUserId = currentUserId, statusMap = statusMap });
        }

        // GET: SupportTickets/Edit/5
        public async Task<ActionResult> Edit(Guid? id, Guid? currentUserId, int statusMap)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportTicket supportTicket = await db.SupportTickets.FindAsync(id);
            if (supportTicket == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", supportTicket.ClientID);
            ViewBag.SupportTicketStatusID = new SelectList(db.SupportTicketStatuses, "SupportTicketStatusId", "SupportTicketStatusName", supportTicket.SupportTicketStatusID);
            ViewBag.SupportTicketTypeId = new SelectList(db.SupportTicketTypes, "SupportTicketTypeId", "SupportTicketTypeName", supportTicket.SupportTicketTypeId);

            var context = new ApplicationDbContext();
            var data = (from item in context.Users
                             select new { UserID = item.Id, Name = item.UserName }).ToArray();

            ViewBag.UserID = new SelectList(data, "UserID", "Name", data.Where(m => m.UserID == supportTicket.UserID?.ToString()));
            ViewBag.currentUserId = currentUserId;
            ViewBag.statusMap = statusMap;

            return View(supportTicket);
        }

        // POST: SupportTickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SupportTicketId,Description,ClientID,UserID,SupportTicketStatusID,SupportTicketTypeId,iDate")] SupportTicket supportTicket, Guid? currentUserId, int statusMap)
        {
            if (ModelState.IsValid)
            {
                if (supportTicket.SupportTicketStatusID == 1 && supportTicket.UserID.HasValue)
                {
                    supportTicket.SupportTicketStatusID = 2;
                }
                db.Entry(supportTicket).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { currentUserId=currentUserId, statusMap=statusMap });
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", supportTicket.ClientID);
            ViewBag.SupportTicketStatusID = new SelectList(db.SupportTicketStatuses, "SupportTicketStatusId", "SupportTicketStatusName", supportTicket.SupportTicketStatusID);
            ViewBag.SupportTicketTypeId = new SelectList(db.SupportTicketTypes, "SupportTicketTypeId", "SupportTicketTypeName", supportTicket.SupportTicketTypeId);
            ViewBag.currentUserId = currentUserId;
            ViewBag.statusMap = statusMap;

            return View(supportTicket);
        }

        // GET: SupportTickets/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportTicket supportTicket = await db.SupportTickets.FindAsync(id);
            if (supportTicket == null)
            {
                return HttpNotFound();
            }
            return View(supportTicket);
        }

        // POST: SupportTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            SupportTicket supportTicket = await db.SupportTickets.FindAsync(id);
            db.SupportTickets.Remove(supportTicket);
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
