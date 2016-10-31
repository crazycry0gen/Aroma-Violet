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
    public class SystemSMSTemplatesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SystemSMSTemplates
        public async Task<ActionResult> Index(int? clientId)
        {
            if (clientId.HasValue)
            {
                    ViewBag.ClientID = clientId.Value;
            }

            return View(await db.SystemSMSTemplates.ToListAsync());
        }
        
        // GET: SystemSMSTemplates/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSMSTemplate systemSMSTemplate = await db.SystemSMSTemplates.FindAsync(id);
            if (systemSMSTemplate == null)
            {
                return HttpNotFound();
            }
            return View(systemSMSTemplate);
        }

        // GET: SystemSMSTemplates/Create
        public ActionResult Create()
        {
            ViewBag.Variables = SystemSMSTemplateModel.GetVariableList();
            return View();
        }

        // POST: SystemSMSTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SystemSMSTemplateId,Description,Text,Active")] SystemSMSTemplate systemSMSTemplate)
        {
            if (ModelState.IsValid)
            {
                db.SystemSMSTemplates.Add(systemSMSTemplate);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Variables = SystemSMSTemplateModel.GetVariableList();
            return View(systemSMSTemplate);
        }

        // GET: SystemSMSTemplates/Edit/5
        public async Task<ActionResult> Edit(int? id,int? clientId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSMSTemplate systemSMSTemplate = await db.SystemSMSTemplates.FindAsync(id);
            if (systemSMSTemplate == null)
            {
                return HttpNotFound();
            }

            ViewBag.TemplateVariables = SystemSMSTemplateModel.GetVariableList();
            if (clientId.HasValue)
            {
                ViewBag.ClientID = clientId.Value;
                var client = db.Clients.FirstOrDefault(m => m.ClientId == clientId.Value);
                if (client != null)
                {
                    ViewBag.ClientID = clientId.Value;
                    var template = new SystemSMSTemplateModel(systemSMSTemplate.Text);
                    template.Populate(client, db);
                    ViewBag.Preview = template.Generate();
                }
            }
            return View(systemSMSTemplate);
        }

        // POST: SystemSMSTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SystemSMSTemplateId,Description,Text,Active")] SystemSMSTemplate systemSMSTemplate, int? clientId, bool preview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemSMSTemplate).State = EntityState.Modified;
                await db.SaveChangesAsync();

                if (preview)
                {
                    if (!clientId.HasValue) clientId = db.Clients.FirstOrDefault(m=>m.Active)?.ClientId;
                    return RedirectToAction("Edit", new {id=systemSMSTemplate.SystemSMSTemplateId, clientId=clientId });
                }
                else
                {
                    return RedirectToAction("Index", new { clientId = clientId });
                }
            }

            ViewBag.TemplateVariables = SystemSMSTemplateModel.GetVariableList();
            if (clientId.HasValue)
            {
                ViewBag.ClientID = clientId.Value;
                var client = db.Clients.FirstOrDefault(m => m.ClientId == clientId.Value);
                if (client != null)
                {
                    ViewBag.ClientID = clientId.Value;
                    var template = new SystemSMSTemplateModel(systemSMSTemplate.Text);
                    template.Populate(client,db);
                    ViewBag.Preview = template.Generate();
                }
            }
            return View(systemSMSTemplate);
        }
 

        // GET: SystemSMSTemplates/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSMSTemplate systemSMSTemplate = await db.SystemSMSTemplates.FindAsync(id);
            if (systemSMSTemplate == null)
            {
                return HttpNotFound();
            }
            return View(systemSMSTemplate);
        }

        // POST: SystemSMSTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SystemSMSTemplate systemSMSTemplate = await db.SystemSMSTemplates.FindAsync(id);
            db.SystemSMSTemplates.Remove(systemSMSTemplate);
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
