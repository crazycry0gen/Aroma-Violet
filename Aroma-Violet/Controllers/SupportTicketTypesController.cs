﻿using System;
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
    public class SupportTicketTypesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SupportTicketTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.SupportTicketTypes.ToListAsync());
        }

        // GET: SupportTicketTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportTicketType supportTicketType = await db.SupportTicketTypes.FindAsync(id);
            if (supportTicketType == null)
            {
                return HttpNotFound();
            }
            return View(supportTicketType);
        }

        // GET: SupportTicketTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SupportTicketTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SupportTickettypeId,SupportTicketTypeName")] SupportTicketType supportTicketType)
        {
            if (ModelState.IsValid)
            {
                db.SupportTicketTypes.Add(supportTicketType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(supportTicketType);
        }

        // GET: SupportTicketTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportTicketType supportTicketType = await db.SupportTicketTypes.FindAsync(id);
            if (supportTicketType == null)
            {
                return HttpNotFound();
            }
            return View(supportTicketType);
        }

        // POST: SupportTicketTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SupportTickettypeId,SupportTicketTypeName")] SupportTicketType supportTicketType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supportTicketType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(supportTicketType);
        }

        // GET: SupportTicketTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportTicketType supportTicketType = await db.SupportTicketTypes.FindAsync(id);
            if (supportTicketType == null)
            {
                return HttpNotFound();
            }
            return View(supportTicketType);
        }

        // POST: SupportTicketTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SupportTicketType supportTicketType = await db.SupportTicketTypes.FindAsync(id);
            db.SupportTicketTypes.Remove(supportTicketType);
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
