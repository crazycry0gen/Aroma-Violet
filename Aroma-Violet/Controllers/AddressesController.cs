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
    public class AddressesController : Controller
    {
        private AromaContext db = new AromaContext();

        public JsonResult CheckPostalCode(string code)
        {
            var codeEntry = db.PostalCodes.Where(m => m.PostalCodeName == code).FirstOrDefault();
            if (codeEntry == null)
            {
                return Json( "Code not found" );
            }
            else
            {
                return Json(codeEntry.PostalArea.PostalAreaName );

            }
        }

        // GET: Addresses
        public async Task<ActionResult> Index()
        {
            var addresses = db.Addresses.Include(a => a.AddressType);
            return View(await addresses.ToListAsync());
        }

        // GET: Addresses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = await db.Addresses.FindAsync(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // GET: Addresses/Create
        public ActionResult Create()
        {
            ViewBag.AddressTypeID = new SelectList(db.AddressTypes, "AddressTypeId", "AddressTypeName");
            return View();
        }

        // POST: Addresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AddressId,Code,ClientID,AddressTypeID,Active")] Address address)
        {
            if (ModelState.IsValid)
            {
                db.Addresses.Add(address);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AddressTypeID = new SelectList(db.AddressTypes, "AddressTypeId", "AddressTypeName", address.AddressTypeID);
            return View(address);
        }

        // GET: Addresses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = await db.Addresses.Where(m=>m.AddressId== id).Include(m=>m.Client).Include(m=>m.Lines).SingleOrDefaultAsync();
            if (address.ClientID == 0)
            {
                var client = (from item in db.Clients
                              where item.DeliveryAddress.AddressId == address.AddressId
                              || item.PostalAddress.AddressId == address.AddressId
                              select item).FirstOrDefault();
                if (client != null)
                {
                    address.ClientID = client.ClientId;
                    db.SaveChanges();
                }
            }
            if (address == null)
            {
                return HttpNotFound();
            }
            ViewBag.AddressTypeID = new SelectList(db.AddressTypes, "AddressTypeId", "AddressTypeName", address.AddressTypeID);
            return View(address);
        }

        // POST: Addresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AddressId,Code,ClientID,AddressTypeID,Active,Lines,ClientId,Line1,Line2,Line3,Line4 ")] Address address, int? ClientId, string Line1, string Line2, string Line3, string Line4, string Line5)
        {

            if (ModelState.IsValid)
            {
                var inputLines = new string[]
                    {
                        Line1,
                        Line2,
                        Line3,
                        Line4,
                        Line5
                    };
                var lines = db.AddressLines.Where(m => m.AddressID == address.AddressId).OrderBy(m => m.Order).ToArray();
                while (lines.Length < inputLines.Length)
                {
                    var order = (from item in lines
                                 select item.Order).Max() + 1;
                    db.AddressLines.Add(new AddressLine() {Active=true, AddressID = address.AddressId, AddressLineText=string.Empty, Order= order });
                    db.SaveChanges();
                    lines = db.AddressLines.Where(m => m.AddressID == address.AddressId).OrderBy(m => m.Order).ToArray();
                }
                for (int i = 0; i < inputLines.Length; i++)
                {
                    lines[i].AddressLineText = inputLines[i];
                }
                db.Entry(address).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Edit","Clients",routeValues:new{id=ClientId });
            }
            ViewBag.AddressTypeID = new SelectList(db.AddressTypes, "AddressTypeId", "AddressTypeName", address.AddressTypeID);
            return View(address);
        }

        // GET: Addresses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = await db.Addresses.FindAsync(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Address address = await db.Addresses.FindAsync(id);
            db.Addresses.Remove(address);
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
