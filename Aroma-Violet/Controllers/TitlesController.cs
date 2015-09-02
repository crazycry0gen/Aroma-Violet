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
    public class TitlesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: Titles
        public async Task<ActionResult> Index()
        {
            return View(await db.Titles.ToListAsync());
        }

        // GET: Titles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Title Title = await db.Titles.FindAsync(id);
            if (Title == null)
            {
                return HttpNotFound();
            }
            return View(Title);
        }

        // GET: Titles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Titles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TitleId,TitleName,Active")] Title Title)
        {
            if (ModelState.IsValid)
            {
                db.Titles.Add(Title);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(Title);
        }

        // GET: Titles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Title Title = await db.Titles.FindAsync(id);
            if (Title == null)
            {
                return HttpNotFound();
            }
            return View(Title);
        }

        // POST: Titles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TitleId,TitleName,Active")] Title Title)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Title).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(Title);
        }

        // GET: Titles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Title Title = await db.Titles.FindAsync(id);
            if (Title == null)
            {
                return HttpNotFound();
            }
            return View(Title);
        }

        // POST: Titles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Title Title = await db.Titles.FindAsync(id);
            db.Titles.Remove(Title);
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
