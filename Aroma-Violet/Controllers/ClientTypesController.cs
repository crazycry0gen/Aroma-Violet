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
    public class ClientTypesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: ClientTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.ClientTypes.OrderBy(m => m.ClientTypeName).ToListAsync());
        }

        // GET: ClientTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientType clientType = await db.ClientTypes.FindAsync(id);
            if (clientType == null)
            {
                return HttpNotFound();
            }
            return View(clientType);
        }

        // GET: ClientTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ClientTypeId,ClientTypeName,Active,AddShipping")] ClientType clientType)
        {
            var existing = db.ClientTypes.Where(m => m.ClientTypeName.Trim().ToLower() == clientType.ClientTypeName.Trim().ToLower()).FirstOrDefault();
            if (existing != null)
            {
                ModelState.AddModelError("", string.Format("A client type already exists with the name {0}", clientType.ClientTypeName));
            }
            if (ModelState.IsValid)
            {
                db.ClientTypes.Add(clientType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(clientType);
        }

        // GET: ClientTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientType clientType = await db.ClientTypes.FindAsync(id);
            if (clientType == null)
            {
                return HttpNotFound();
            }
            return View(clientType);
        }

        // POST: ClientTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ClientTypeId,ClientTypeName,Active,AddShipping")] ClientType clientType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(clientType);
        }

        // GET: ClientTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientType clientType = await db.ClientTypes.FindAsync(id);
            if (clientType == null)
            {
                return HttpNotFound();
            }
            return View(clientType);
        }

        // POST: ClientTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ClientType clientType = await db.ClientTypes.FindAsync(id);
            db.ClientTypes.Remove(clientType);
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
