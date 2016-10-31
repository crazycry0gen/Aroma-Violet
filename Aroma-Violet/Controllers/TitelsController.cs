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
    public class TitelsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: Titels
        public async Task<ActionResult> Index()
        {
            return View(await db.Titels.ToListAsync());
        }

        // GET: Titels/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Titel titel = await db.Titels.FindAsync(id);
            if (titel == null)
            {
                return HttpNotFound();
            }
            return View(titel);
        }

        // GET: Titels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Titels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TitelId,TitelName,Active")] Titel titel)
        {
            if (ModelState.IsValid)
            {
                db.Titels.Add(titel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(titel);
        }

        // GET: Titels/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Titel titel = await db.Titels.FindAsync(id);
            if (titel == null)
            {
                return HttpNotFound();
            }
            return View(titel);
        }

        // POST: Titels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TitelId,TitelName,Active")] Titel titel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(titel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(titel);
        }

        // GET: Titels/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Titel titel = await db.Titels.FindAsync(id);
            if (titel == null)
            {
                return HttpNotFound();
            }
            return View(titel);
        }

        // POST: Titels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Titel titel = await db.Titels.FindAsync(id);
            db.Titels.Remove(titel);
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
