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
    public class SubscriptionsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: Subscriptions
        public async Task<ActionResult> Index()
        {
            ViewBag.ClientTypes = db.ClientTypes.Where(m => m.Active).OrderBy(m => m.ClientTypeName).ToArray();
            var subscriptions = db.Subscriptions.Include(s => s.ClientType).Include(s => s.Product).OrderBy(m=>m.ClientType.ClientTypeName).ThenBy(m=>m.Product.ProductName);
            return View(await subscriptions.ToListAsync());
        }

        // GET: Subscriptions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // GET: Subscriptions/Create
        public ActionResult Create()
        {
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName");
            ViewBag.ProductID = new SelectList(db.Products.Where(m=>m.Active), "ProductID", "ProductName");
            ViewBag.InitialOnceOffFromAccountID = GetInitialOnceOffAccounts(Guid.Empty);
            ViewBag.SalesTypeID = new SelectList(db.SalesTypes.Where(m => m.Active).OrderBy(m => m.SalesTypeDescription).ToArray(), "SalesTypeID", "SalesTypeDescription", 0);
            var subscription = new Subscription() { ValidFromDate = DateTime.Now, Active = true};
            
            return View(subscription);
        }

        private SelectList GetInitialOnceOffAccounts(Guid selectedId)
        {
            var accounts = db.Accounts.Where(m =>m.Active && !m.IsSystemAccount).OrderBy(m=>m.AccountName).ToList();
            var defAccount = new finAccount() {AccountId=Guid.Empty, AccountName="None", Active=true, IsSystemAccount=false };
            accounts.Insert(0,defAccount);
            return new SelectList(accounts, "AccountId", "AccountName", selectedId);
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SubscriptionId,ClientTypeID,ProductID,MandatoryQuantity,ValidFromDate,Price,PriceExcl,Active,SalesTypeID")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                db.Subscriptions.Add(subscription);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", subscription.ClientTypeID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", subscription.ProductID);
            ViewBag.InitialOnceOffFromAccountID = GetInitialOnceOffAccounts(subscription.InitialOnceOffFromAccountID);
            ViewBag.SalesTypeID = new SelectList(db.SalesTypes.Where(m => m.Active).OrderBy(m => m.SalesTypeDescription).ToArray(), "SalesTypeID", "SalesTypeDescription", subscription.SalesTypeID);

            return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", subscription.ClientTypeID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", subscription.ProductID);
            ViewBag.InitialOnceOffFromAccountID = GetInitialOnceOffAccounts(subscription.InitialOnceOffFromAccountID);
            ViewBag.SalesTypeID = new SelectList(db.SalesTypes.Where(m => m.Active).OrderBy(m => m.SalesTypeDescription).ToArray(), "SalesTypeID", "SalesTypeDescription", subscription.SalesTypeID);


            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SubscriptionId,ClientTypeID,ProductID,ValidFromDate,MandatoryQuantity,Price,PriceExcl,Active,InitialOnceOffFromAccountID,SalesTypeID")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subscription).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes.OrderBy(m=>m.ClientTypeName), "ClientTypeId", "ClientTypeName", subscription.ClientTypeID);
            ViewBag.ProductID = new SelectList(db.Products.OrderBy(m=>m.ProductName), "ProductID", "ProductName", subscription.ProductID);
            ViewBag.InitialOnceOffFromAccountID = GetInitialOnceOffAccounts(subscription.InitialOnceOffFromAccountID);
            ViewBag.SalesTypeID = new SelectList( db.SalesTypes.Where(m => m.Active).OrderBy(m => m.SalesTypeDescription).ToArray(), "SalesTypeID", "SalesTypeDescription", subscription.SalesTypeID);


            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            db.Subscriptions.Remove(subscription);
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
