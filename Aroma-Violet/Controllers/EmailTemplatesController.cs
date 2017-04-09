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
    public class EmailTemplatesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: EmailTemplates
        public async Task<ActionResult> Index()
        {
            return View(await db.EmailTemplates.ToListAsync());
        }

        public void CreateSnapshotAndMail(int clientId, string subject, string html, int savedHtmlTypeId)
        {
            var address = db.Contacts.Where(m => m.Active && m.ClientID == clientId && m.ContactTypeID == 6).FirstOrDefault();
            if(address==null)
            {
                throw new Exception("No email address found");
            }
            var mailClient = new MassComm.MassEmail("c.hattingh@hotmail.com", "smtp.live.com", "N0k1a!@#$&", 587, "c.hattingh@hotmail.com");
            var msg = new MassComm.EmailLayout() {EmailTo=address.ContactName, Subject = subject };
            var savedHtml = new SavedHtml()
            {
                ClientId = clientId,
                Created = DateTime.Now,
                SavedHtmlData = html,
                SavedHtmlId = Guid.NewGuid(),
                SavedHtmlTypeId = savedHtmlTypeId
            };
            db.SavedHtmls.Add(savedHtml);
            db.SaveChanges();



            mailClient.Send(msg);
        }

        // GET: EmailTemplates/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplate emailTemplate = await db.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }
            return View(emailTemplate);
        }

        // GET: EmailTemplates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EmailTemplateId,SqlStatement,EmailAddress,Subject,EmailBody")] EmailTemplate emailTemplate)
        {
            if (ModelState.IsValid)
            {
                emailTemplate.EmailTemplateId = Guid.NewGuid();
                db.EmailTemplates.Add(emailTemplate);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(emailTemplate);
        }

        // GET: EmailTemplates/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplate emailTemplate = await db.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }
            return View(emailTemplate);
        }

        // POST: EmailTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EmailTemplateId,SqlStatement,EmailAddress,Subject,EmailBody")] EmailTemplate emailTemplate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emailTemplate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(emailTemplate);
        }

        // GET: EmailTemplates/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplate emailTemplate = await db.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }
            return View(emailTemplate);
        }

        // POST: EmailTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            EmailTemplate emailTemplate = await db.EmailTemplates.FindAsync(id);
            db.EmailTemplates.Remove(emailTemplate);
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
