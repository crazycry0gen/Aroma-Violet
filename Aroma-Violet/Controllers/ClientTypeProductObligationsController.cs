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
    public class ClientTypeProductObligationsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: ClientTypeProductObligations
        public async Task<ActionResult> Index()
        {
            var clientTypeProductObligations = db.ClientTypeProductObligations.Include(c => c.ClientType).Include(c => c.Product).OrderBy(m=>m.ClientType.ClientTypeName);
            return View(await clientTypeProductObligations.ToListAsync());
        }

        // GET: ClientTypeProductObligations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTypeProductObligation clientTypeProductObligation = await db.ClientTypeProductObligations.FindAsync(id);
            if (clientTypeProductObligation == null)
            {
                return HttpNotFound();
            }
            return View(clientTypeProductObligation);
        }

        // GET: ClientTypeProductObligations/Create
        public ActionResult Create()
        {
            ViewBag.ClientTypeId = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName");
            ViewBag.ProductId = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: ClientTypeProductObligations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ClientTypeProductObligationId,ProductId,ClientTypeId,Active")] ClientTypeProductObligation clientTypeProductObligation)
        {
            if (ModelState.IsValid)
            {
                db.ClientTypeProductObligations.Add(clientTypeProductObligation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientTypeId = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", clientTypeProductObligation.ClientTypeId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductID", "ProductName", clientTypeProductObligation.ProductId);
            return View(clientTypeProductObligation);
        }

        // GET: ClientTypeProductObligations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTypeProductObligation clientTypeProductObligation = await db.ClientTypeProductObligations.FindAsync(id);
            if (clientTypeProductObligation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientTypeId = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", clientTypeProductObligation.ClientTypeId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductID", "ProductName", clientTypeProductObligation.ProductId);
            return View(clientTypeProductObligation);
        }

        // POST: ClientTypeProductObligations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ClientTypeProductObligationId,ProductId,ClientTypeId,Active")] ClientTypeProductObligation clientTypeProductObligation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientTypeProductObligation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientTypeId = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", clientTypeProductObligation.ClientTypeId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductID", "ProductName", clientTypeProductObligation.ProductId);
            return View(clientTypeProductObligation);
        }

        // GET: ClientTypeProductObligations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTypeProductObligation clientTypeProductObligation = await db.ClientTypeProductObligations.FindAsync(id);
            if (clientTypeProductObligation == null)
            {
                return HttpNotFound();
            }
            return View(clientTypeProductObligation);
        }

        // POST: ClientTypeProductObligations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ClientTypeProductObligation clientTypeProductObligation = await db.ClientTypeProductObligations.FindAsync(id);
            db.ClientTypeProductObligations.Remove(clientTypeProductObligation);
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
