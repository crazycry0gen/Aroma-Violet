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
    public class SavedHtmlsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SavedHtmls
        public async Task<ActionResult> Index()
        {
            var savedHtmls = db.SavedHtmls.Include(s => s.SavedHtmlType);
            return View(await savedHtmls.ToListAsync());
        }

        // GET: SavedHtmls/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavedHtml savedHtml = await db.SavedHtmls.FindAsync(id);
            if (savedHtml == null)
            {
                return HttpNotFound();
            }
            return View(savedHtml);
        }

        // GET: SavedHtmls/Create
        public ActionResult Create()
        {
            ViewBag.SavedHtmlTypeId = new SelectList(db.SavedHtmlTypes, "SavedHtmlTypeId", "SavedHtmlTypeName");
            return View();
        }

        // POST: SavedHtmls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SavedHtmlId,SavedHtmlData,ClientId,SavedHtmlTypeId,Created")] SavedHtml savedHtml)
        {
            if (ModelState.IsValid)
            {
                savedHtml.SavedHtmlId = Guid.NewGuid();
                db.SavedHtmls.Add(savedHtml);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.SavedHtmlTypeId = new SelectList(db.SavedHtmlTypes, "SavedHtmlTypeId", "SavedHtmlTypeName", savedHtml.SavedHtmlTypeId);
            return View(savedHtml);
        }

        // GET: SavedHtmls/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavedHtml savedHtml = await db.SavedHtmls.FindAsync(id);
            if (savedHtml == null)
            {
                return HttpNotFound();
            }
            ViewBag.SavedHtmlTypeId = new SelectList(db.SavedHtmlTypes, "SavedHtmlTypeId", "SavedHtmlTypeName", savedHtml.SavedHtmlTypeId);
            return View(savedHtml);
        }

        // POST: SavedHtmls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SavedHtmlId,SavedHtmlData,ClientId,SavedHtmlTypeId,Created")] SavedHtml savedHtml)
        {
            if (ModelState.IsValid)
            {
                db.Entry(savedHtml).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SavedHtmlTypeId = new SelectList(db.SavedHtmlTypes, "SavedHtmlTypeId", "SavedHtmlTypeName", savedHtml.SavedHtmlTypeId);
            return View(savedHtml);
        }

        // GET: SavedHtmls/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavedHtml savedHtml = await db.SavedHtmls.FindAsync(id);
            if (savedHtml == null)
            {
                return HttpNotFound();
            }
            return View(savedHtml);
        }

        // POST: SavedHtmls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            SavedHtml savedHtml = await db.SavedHtmls.FindAsync(id);
            db.SavedHtmls.Remove(savedHtml);
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
