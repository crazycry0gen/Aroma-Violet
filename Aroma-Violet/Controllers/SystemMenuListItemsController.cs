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
    public class SystemMenuListItemsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SystemMenuListItems
        public async Task<ActionResult> Index()
        {
            return View(await db.SystemMenuListItems.ToListAsync());
        }

        // GET: SystemMenuListItems/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemMenuListItem systemMenuListItem = await db.SystemMenuListItems.FindAsync(id);
            if (systemMenuListItem == null)
            {
                return HttpNotFound();
            }
            return View(systemMenuListItem);
        }

        // GET: SystemMenuListItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemMenuListItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SystemMenuListItemId,Text,ActionName,ControllerName,Order,Active")] SystemMenuListItem systemMenuListItem)
        {
            if (ModelState.IsValid)
            {
                systemMenuListItem.SystemMenuListItemId = Guid.NewGuid();
                db.SystemMenuListItems.Add(systemMenuListItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(systemMenuListItem);
        }

        // GET: SystemMenuListItems/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemMenuListItem systemMenuListItem = await db.SystemMenuListItems.FindAsync(id);
            if (systemMenuListItem == null)
            {
                return HttpNotFound();
            }
            return View(systemMenuListItem);
        }

        // POST: SystemMenuListItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SystemMenuListItemId,Text,ActionName,ControllerName,Order,Active")] SystemMenuListItem systemMenuListItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemMenuListItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(systemMenuListItem);
        }

        // GET: SystemMenuListItems/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemMenuListItem systemMenuListItem = await db.SystemMenuListItems.FindAsync(id);
            if (systemMenuListItem == null)
            {
                return HttpNotFound();
            }
            return View(systemMenuListItem);
        }

        // POST: SystemMenuListItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            SystemMenuListItem systemMenuListItem = await db.SystemMenuListItems.FindAsync(id);
            db.SystemMenuListItems.Remove(systemMenuListItem);
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
