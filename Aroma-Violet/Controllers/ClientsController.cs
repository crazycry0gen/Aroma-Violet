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
    public class ClientsController : Controller
    {
        private AromaContext db = new AromaContext();
        private const int maxResult = 20;

        public async Task<ActionResult> Index(string criteria)
        {
            if (criteria == null) criteria = string.Empty;
            var lowSearch = criteria.ToLower();
            var clients = db.Clients.Where(m => m.ClientId.ToString() == lowSearch
                        || m.ClientName.ToLower().Contains(lowSearch)
                        || m.ClientSurname.Contains(lowSearch)
                        || m.IDNumber.Contains(lowSearch)
                        ).Take(maxResult)
                        .Include(c => c.ClientType)
                        .Include(c => c.Country)
                        .Include(c => c.EthnicGroup)
                        .Include(c => c.IncomeGroup)
                        .Include(c => c.Province)
                        .Include(c => c.Titel);
            ViewBag.Criteria = criteria;

            return View(await clients.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await db.Clients.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName");
            ViewBag.CountryID = new SelectList(db.Countries, "CountryId", "CountryName");
            ViewBag.EthnicGroupID = new SelectList(db.EthnicGroups, "EthnicGroupId", "EthnicGroupName");
            ViewBag.IncomeGroupID = new SelectList(db.IncomeGroups, "IncomeGroupId", "IncomeGroupName");
            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceId", "ProvinceName");
            ViewBag.TitelID = new SelectList(db.Titels, "TitelId", "TitelName");
            ViewBag.HomeLanguageID = new SelectList(db.Languages, "LanguageID", "LanguageName");
            ViewBag.AddressTypeID = new SelectList(db.AddressTypes, "AddressTypeID", "AddressTypeName");
            ViewBag.PreviousYearCount = 80;
            ViewBag.NextYearCount = 0;


            var newClient = new ClientViewModel();
            newClient.PostalAddress = GetNewAddress("Postal");
            newClient.DeliveryAddress = GetNewAddress("Physical");
            newClient.DateOfBirth = DateTime.Now;

            return View(newClient);
        }

        private Address GetNewAddress(string addressType)
        {
            string errorMessage = string.Format("Address type \"{0}\" not defined in lookup", addressType);
            var type = db.AddressTypes.FirstOrDefault(m => m.AddressTypeName == addressType);
            if (type == null)
                throw new Exception(errorMessage);
            var address = new Address();
            address.AddressType = type;
            address.AddressTypeID = type.AddressTypeId;
            address.Lines = new List<AddressLine>();
            address.Lines.Add(new AddressLine());
            address.Lines.Add(new AddressLine());
            address.Lines.Add(new AddressLine());
            return address;
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ClientId,ClientName,NickName,FullNames,HomeLanguageID,Employer,DateOfBirth,ClientSurname,SAResident,IDNumber,ClientTypeID,TitelID,EthnicGroupID,IncomeGroupID,PostalAddress,DeliveryAddress,DeliveryAddressLines,ProvinceID,CountryID,Lines,AddressLine,PostalAddressLines,AddressTypeID,DeliveryAddress,PostalAddress")] ClientViewModel clientView)
        {
            var client = clientView.GetBaseClient();
            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", client.ClientTypeID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryId", "CountryName", client.CountryID);
            ViewBag.EthnicGroupID = new SelectList(db.EthnicGroups, "EthnicGroupId", "EthnicGroupName", client.EthnicGroupID);
            ViewBag.IncomeGroupID = new SelectList(db.IncomeGroups, "IncomeGroupId", "IncomeGroupName", client.IncomeGroupID);
            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceId", "ProvinceName", client.ProvinceID);
            ViewBag.TitelID = new SelectList(db.Titels, "TitelId", "TitelName", client.TitelID);
            ViewBag.HomeLanguageID = new SelectList(db.Languages, "LanguageID", "LanguageName");
            ViewBag.AddressTypeID = new SelectList(db.AddressTypes, "AddressTypeID", "AddressTypeName");
            ViewBag.PreviousYearCount = 80;
            ViewBag.NextYearCount = 0;
            return View(clientView);
        }

        // GET: Clients/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await db.Clients.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", client.ClientTypeID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryId", "CountryName", client.CountryID);
            ViewBag.EthnicGroupID = new SelectList(db.EthnicGroups, "EthnicGroupId", "EthnicGroupName", client.EthnicGroupID);
            ViewBag.IncomeGroupID = new SelectList(db.IncomeGroups, "IncomeGroupId", "IncomeGroupName", client.IncomeGroupID);
            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceId", "ProvinceName", client.ProvinceID);
            ViewBag.TitelID = new SelectList(db.Titels, "TitelId", "TitelName", client.TitelID);
            ViewBag.HomeLanguageID = new SelectList(db.Languages, "LanguageID", "LanguageName");
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ClientId,ClientName,NickName,FullNames,HomeLanguageID,Employer,DateOfBirth,ClientSurname,SAResident,IDNumber,ClientTypeID,TitelID,EthnicGroupID,IncomeGroupID,ProvinceID,CountryID")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", client.ClientTypeID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryId", "CountryName", client.CountryID);
            ViewBag.EthnicGroupID = new SelectList(db.EthnicGroups, "EthnicGroupId", "EthnicGroupName", client.EthnicGroupID);
            ViewBag.IncomeGroupID = new SelectList(db.IncomeGroups, "IncomeGroupId", "IncomeGroupName", client.IncomeGroupID);
            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceId", "ProvinceName", client.ProvinceID);
            ViewBag.TitelID = new SelectList(db.Titels, "TitelId", "TitelName", client.TitelID);
            ViewBag.HomeLanguageID = new SelectList(db.Languages, "LanguageID", "LanguageName");

            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await db.Clients.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Client client = await db.Clients.FindAsync(id);
            db.Clients.Remove(client);
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
