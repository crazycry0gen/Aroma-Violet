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
using System.Threading;
using Microsoft.AspNet.Identity;

namespace Aroma_Violet.Controllers
{
    public class ClientsController : Controller
    {
        private AromaContext db = new AromaContext();
        private const int maxResult = 20;

        public void EnforceClientNumberConcurrency()
        {
            const string sSqlCheckSeed = "select i.last_value from sys.sysobjects o left join 	 sys.identity_columns i on o.id = i.[object_id] where o.name = 'Client'";
            const string sSqlReseed = "DBCC CHECKIDENT ('dbo.Client',RESEED,{0});";

            var clientNos = (from item in db.Clients select item.ClientId).ToArray();
            var checkNo = 0;
            if (clientNos.Length > 0) checkNo = clientNos[0];
            for (int index = 0; index < clientNos.Length - 1; index++)
            {
                if (clientNos[index] != checkNo)
                {
                    break;
                }
                checkNo++;
            }
            var currentSeed = db.Database.SqlQuery<int>(sSqlCheckSeed).FirstOrDefault();
            for (int reTry = 0; reTry < 10 && currentSeed != checkNo; reTry++)
            {
                try
                {
                    db.Database.ExecuteSqlCommand(string.Format(sSqlReseed, checkNo));
                    currentSeed = db.Database.SqlQuery<int>(sSqlCheckSeed).FirstOrDefault();
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
        }
        public async Task<ActionResult> Index(string criteria)
        {
            if (criteria == null) criteria = string.Empty;
            var lowSearch = criteria.ToLower();
            var clients = db.Clients.Where(m => m.ClientId.ToString() == lowSearch
                        || m.ClientInitials.ToLower().Contains(lowSearch)
                        || m.FullNames.ToLower().Contains(lowSearch)
                        || m.ClientSurname.Contains(lowSearch)
                        || m.IDNumber.Contains(lowSearch)
                        || m.ClientType.ClientTypeName.Contains(lowSearch)
                        || m.CompanyName.ToLower().Contains(lowSearch)
                        || m.NickName.ToLower().Contains(lowSearch)
                        || m.Contact.Where(c=>c.Active).Select(c=>c.ContactName.ToLower()).Any(x => x.Contains(lowSearch))
                        ).Take(maxResult)
                        .Include(c => c.ClientType)
                        .Include(c => c.Country)
                        .Include(c => c.EthnicGroup)
                        .Include(c => c.IncomeGroup)
                        .Include(c => c.Province)
                        .Include(c=>c.BankingDetails)
                        .Include(c => c.Title);
            ViewBag.Criteria = criteria;
            var userId = Guid.Parse(User.Identity.GetUserId());
            var clientId = Generic.GetMyClientId(db, userId);
            var myAllowedClientTypes = GetMyAllowedClientTypes(clientId);
            ViewBag.MyClientId = clientId;
            return View(await clients.Where(m => myAllowedClientTypes.Contains(m.ClientTypeID) || m.ClientId == clientId).ToListAsync());
        }

        private int[] GetMyAllowedClientTypes(int? clientId)
        {
            var result = (from item in this.db.ClientTypes select item.ClientTypeId).ToArray();

            if (!clientId.HasValue)
            {
                return result;
            }

            // TODO:make configurable, get from table ClientTypeRelation
            var distributors = new int[] { 1, 6 };
            var clientTypeId = this.db.Clients.Find(clientId.Value).ClientTypeID;
            return distributors.Contains(clientTypeId) ? new int[] { 2 } : result;
        }

        // GET: Clients/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await db.Clients.Include(maxResult => maxResult.ClientSubscriptions).Include(m => m.BankingDetails).FirstAsync(m => m.ClientId == id);
            if (client == null)
            {
                return HttpNotFound();
            }
 
            ViewBag.ContactInfo = (from item in db.ContactTypes.Where(m => m.Active).ToArray()
                                   select new string[] { item.ContactTypeName, db.Contacts.Where(m=>m.Active
                                                                                                && m.ContactTypeID == item.ContactTypeId
                                                                                                && m.ClientID==id).Select(m=>m.ContactName).FirstOrDefault()
                                                        }).ToArray();
        
    return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName");
            ViewBag.CountryID = new SelectList(db.Countries.OrderBy(m=>m.CountryName), "CountryId", "CountryName");
            ViewBag.EthnicGroupID = new SelectList(db.EthnicGroups, "EthnicGroupId", "EthnicGroupName");
            ViewBag.IncomeGroupID = new SelectList(db.IncomeGroups, "IncomeGroupId", "IncomeGroupName");
                        ViewBag.ProvinceId = new SelectList(db.Provinces.OrderBy(m=>m.ProvinceName), "ProvinceId", "ProvinceName");
            ViewBag.TitleID = new SelectList(db.Titles, "TitleId", "TitleName");
            ViewBag.LanguageID = new SelectList(db.Languages, "LanguageID", "LanguageName");
            ViewBag.AddressTypeID = new SelectList(db.AddressTypes, "AddressTypeID", "AddressTypeName");
            ViewBag.PreviousYearCount = 80;
            ViewBag.NextYearCount = 0;

            EnforceClientNumberConcurrency();

            var newClient = new ClientViewModel();
            newClient.PostalAddress = GetNewAddress("Postal");
            newClient.DeliveryAddress = GetNewAddress("Physical");
            newClient.DateOfBirth = DateTime.Parse("1 Jan 1950");

            //newClient.ClientId = 10000;
            //try
            //{
            //    newClient.ClientId = db.Clients.Select(m => m.ClientId).Max()+1;
            //}
            //catch { }

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
            address.Lines.Add(new AddressLine());
            address.Lines.Add(new AddressLine());
            return address;
        }

        private int CreateContact(int clientId, string contactType, string contactValue)
        {
            var active = true;
            if (!(contactValue?.Length > 0))
            {
                contactValue = "None";
                active = false;
            }
            string errorMessage = string.Format("Contact type \"{0}\" not defined in lookup", contactType);
            var type = db.ContactTypes.FirstOrDefault(m => m.ContactTypeName == contactType);
            if (type == null)
                throw new Exception(errorMessage);
            int oldContactActivatedId = 0;
            var checkExisting = (from item in db.Contacts
                                 where item.ContactTypeID == type.ContactTypeId
                                 && item.ClientID == clientId
                                 select item).ToArray();
            for (int i=0;i< checkExisting.Length; i++)
            {
                if (checkExisting[i].ContactName.ToLower() == contactValue.ToLower())
                {
                    checkExisting[i].Active = true;
                    oldContactActivatedId = checkExisting[i].ContactId;
                }
                else
                {
                    checkExisting[i].Active = false;
                }
            }
            if(checkExisting.Length>0) db.SaveChanges();
            if (oldContactActivatedId > 0) return oldContactActivatedId;
            var newContact = new Contact() { Active = active, ClientID = clientId, ContactName = contactValue, ContactTypeID = type.ContactTypeId };
            db.Contacts.Add(newContact);
            db.SaveChanges();
            return newContact.ContactId;
        }

        public JsonResult CheckCode(string code)
        {
            var ret = string.Empty;
            var objCode = db.PostalCodes.FirstOrDefault(m => m.PostalCodeName == code);
            if (objCode == null)
            {
                ret = string.Format("The code \"{0}\" does not exist in postal codes or in shipping method postal code.", code);
            }
            else
            {
                var shipping = db.ShippingMethodPostalCodes.FirstOrDefault(m => m.PostalCodeId == objCode.PostalCodeId);
                if (shipping == null)
                {
                    ret = string.Format("The code \"{0}\" is not set up in shipping method postal code", code);
                }
            }
            return Json(ret);
        }

        public JsonResult CheckIDNumber(string id)
        {
            var ret = string.Empty;
            var client = db.Clients.FirstOrDefault(m => m.IDNumber == id);
            if (client != null)
            {
                ret = string.Format("ID exists already for {0} {1}", client.Title.TitleName, client.ClientSurname);
            }
            return Json(ret);
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ClientId,ClientInitials,NickName,FullNames,LanguageID,Employer,DateOfBirth,ClientSurname,SAResident,IDNumber,ClientTypeID,TitleID,EthnicGroupID,IncomeGroupID,PostalAddress,DeliveryAddress,DeliveryAddressLines,ProvinceID,CountryID,Lines,AddressLine,PostalAddressLines,AddressTypeID,DeliveryAddress,PostalAddress,TelWork,Cell,TelHome,EMail,ResellerID,RegistrationNumber,CompanyName,IgnoreRebate,Occupation")] ClientViewModel clientView)
        {
            
            var client = clientView.GetBaseClient();
            client.iDate = DateTime.Now;
            db.Log(Generic.enumLGActivity.CreateClient, client.ClientId, "Base client fetch");



            if (ModelState.IsValid)
            {

                ////check if client exists on IDNo and update else add
                //var existingClient = (from item in db.Clients
                //                      where item.IDNumber == client.IDNumber
                //                      select item).FirstOrDefault();

                Client existingClient = null;

                if (existingClient == null)
                {
                    
                    db.Clients.Add(client);
                    await db.SaveChangesAsync();
                    
                }
                else
                {
                    existingClient = Generic.CopyObject(client, existingClient,"ClientId");
                    db.Entry(existingClient).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    client = existingClient;
                }
                //Create relationship
                if (clientView.ResellerID.HasValue)
                {
                    CreateRelationship(clientView.ResellerID.Value, client.ClientId);
                }

                //Create contact
                if (clientView.TelWork == null)       clientView.TelWork = string.Empty;
                if (clientView.TelHome == null)       clientView.TelHome = string.Empty;
                if (clientView.Cell == null) clientView.Cell = string.Empty;
                var workContactId = CreateContact(client.ClientId, "Tel (Work)", clientView.TelWork.Replace(" ",string.Empty));
                var cellContactId = CreateContact(client.ClientId, "Cell", clientView.Cell.Replace(" ", string.Empty));
                var homeContactId = CreateContact(client.ClientId, "Tel (Home)", clientView.TelHome.Replace(" ", string.Empty));
                var emailContactId = CreateContact(client.ClientId, "EMail", clientView.Email);

                BankingDetailsController.CreateClientBankingDetails(db,client.ClientId, client.ClientInitials, client.ClientSurname, cellContactId, homeContactId, workContactId, emailContactId);

                var userId = Guid.Parse(User.Identity.GetUserId());
                if (client.ClientTypeID == 2)
                {
                    SystemSMSController.SendSMSEvent(db, 2, client.ClientId,userId, null);
                }
                if (client.ResellerID.HasValue && client.ResellerID.Value > 0)
                {
                    var cId = new KeyValuePair<string, string>("ChildClientId", client.ClientId.ToString());
                    var cInit = new KeyValuePair<string, string>("ChildInitials", client.ClientInitials);
                    var cSur = new KeyValuePair<string, string>("ChildSurname", client.ClientSurname);
                    SystemSMSController.SendSMSEvent(db, 3, client.ResellerID.Value, userId,null,cId,cInit,cSur);
                }

                return RedirectToAction("Manage", "ClientSubscriptions", new { ClientID = client.ClientId });
            }
            
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", client.ClientTypeID);
            ViewBag.CountryID = new SelectList(db.Countries.OrderBy(m=>m.CountryName), "CountryId", "CountryName", client.CountryID);
            ViewBag.EthnicGroupID = new SelectList(db.EthnicGroups, "EthnicGroupId", "EthnicGroupName", client.EthnicGroupID);
            ViewBag.IncomeGroupID = new SelectList(db.IncomeGroups, "IncomeGroupId", "IncomeGroupName", client.IncomeGroupID);
            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceId", "ProvinceName", client.ProvinceID);
            ViewBag.TitleID = new SelectList(db.Titles, "TitleId", "TitleName", client.TitleID);
            ViewBag.LanguageID = new SelectList(db.Languages, "LanguageID", "LanguageName",client.LanguageID);
            ViewBag.AddressTypeID = new SelectList(db.AddressTypes, "AddressTypeID", "AddressTypeName");
            ViewBag.PreviousYearCount = 80;
            ViewBag.NextYearCount = 0;
            return View(clientView);
            
            
        }

        private void CreateRelationship(int parentId, int childId)
        {
            var relationships = (from item in db.ClientRelationShips
                                 where item.ChildID == childId
                                 select item).ToArray();
            var curRel = (from item in relationships
                          where item.ParentID == parentId
                          select item).FirstOrDefault();
            if (curRel != null)
            {
                curRel.Active = true;
                relationships = relationships.Except(new ClientRelationship[] { curRel }).ToArray();
            }
            else
            {
                curRel = new ClientRelationship() { ParentID = parentId, ChildID = childId, Active = true, ClientRelationshipId = Guid.NewGuid() };
                db.ClientRelationShips.Add(curRel);
            }
            if (relationships != null)
            {
                foreach (var rel in relationships)
                {
                    rel.Active = false;
                }
            }
            db.SaveChanges();
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
            ClientViewModel clientView = client as ClientViewModel;

            var rel = (from item in db.ClientRelationShips
                       where item.ChildID == client.ClientId
                       && item.Active
                       select item).FirstOrDefault();

            if (rel != null && clientView.ResellerID == null) clientView.ResellerID = rel.ParentID;

            clientView.Contacts = (from item in db.ContactTypes.Where(m => m.Active).ToArray()
                                   select new ClientContactView {ClientContactTypeName =  item.ContactTypeName,ClientContactTypeId = item.ContactTypeId, ClientContactValue=  db.Contacts.Where(m=>m.Active
                                                                                                && m.ContactTypeID == item.ContactTypeId
                                                                                                && m.ClientID==id).Select(m=>m.ContactName).FirstOrDefault()}).ToArray();



            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", client.ClientTypeID);
            ViewBag.CountryID = new SelectList(db.Countries.OrderBy(m=>m.CountryName), "CountryId", "CountryName", client.CountryID);
            ViewBag.EthnicGroupID = new SelectList(db.EthnicGroups, "EthnicGroupId", "EthnicGroupName", client.EthnicGroupID);
            ViewBag.IncomeGroupID = new SelectList(db.IncomeGroups, "IncomeGroupId", "IncomeGroupName", client.IncomeGroupID);
            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceId", "ProvinceName", client.ProvinceID);
            ViewBag.TitleID = new SelectList(db.Titles, "TitleId", "TitleName", client.TitleID);
            ViewBag.LanguageID = new SelectList(db.Languages, "LanguageID", "LanguageName", client.LanguageID);
            return View(clientView);
        }

        [HttpPost]
        public void UpdateContact(int clientId, int contactTypeId, string value)
        {
            var curContact = (from Contact item in db.Contacts
                              where item.ClientID == clientId
                              && item.ContactTypeID == contactTypeId
                                && item.Active
                              select item).FirstOrDefault();
            if (curContact == null)
            {
                curContact = new Contact() { Active=true, ClientID=clientId, ContactTypeID = contactTypeId };
                db.Contacts.Add(curContact);
            }
            curContact.ContactName = value.Replace(" ", string.Empty);
            db.SaveChanges();
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ClientId, ResellerID,ClientInitials,NickName,FullNames,LanguageID,Employer,DateOfBirth,ClientSurname,SAResident,IDNumber,ClientTypeID,TitleID,EthnicGroupID,IncomeGroupID,ProvinceID,CountryID,Active,RegistrationNumber,CompanyName,IgnoreRebate,Occupation,iDate,DeliveryAddress_AddressId,PostalAddress_AddressId")] ClientViewModel clientView)
        {
            var client = clientView.GetBaseClient();
            if (ModelState.IsValid)
            {
              
                db.Entry(client).State = EntityState.Modified;
                if (client.PostalAddress_AddressId == 0 || client.DeliveryAddress_AddressId == 0)
                {
                    var addresses = db.Addresses.Where(m => m.ClientID == client.ClientId);
                    var delivery = addresses.FirstOrDefault(m => m.AddressTypeID == 2 && m.Active);
                    if (delivery == null)
                    {
                        delivery = addresses.FirstOrDefault(m => m.AddressTypeID == 2);
                    }
                    if (client.DeliveryAddress_AddressId == 0 && delivery != null)
                    {
                        client.DeliveryAddress_AddressId = delivery.AddressId;
                        delivery.Active = true;
                    }

                    var post = addresses.FirstOrDefault(m => m.AddressTypeID == 1 && m.Active);
                    if (post == null)
                    {
                        post = addresses.FirstOrDefault(m => m.AddressTypeID == 1);
                    }
                    if (client.PostalAddress_AddressId == 0 && post != null)
                    {
                        client.PostalAddress_AddressId = post.AddressId;
                        post.Active = true;
                    }

                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", client.ClientTypeID);
            ViewBag.CountryID = new SelectList(db.Countries.OrderBy(m=>m.CountryName), "CountryId", "CountryName", client.CountryID);
            ViewBag.EthnicGroupID = new SelectList(db.EthnicGroups, "EthnicGroupId", "EthnicGroupName", client.EthnicGroupID);
            ViewBag.IncomeGroupID = new SelectList(db.IncomeGroups, "IncomeGroupId", "IncomeGroupName", client.IncomeGroupID);
            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceId", "ProvinceName", client.ProvinceID);
            ViewBag.TitleID = new SelectList(db.Titles, "TitleId", "TitleName", client.TitleID);
            ViewBag.LanguageID = new SelectList(db.Languages, "LanguageID", "LanguageName", client.LanguageID);

            var curClient = db.Clients.First(m => m.ClientId == client.ClientId);
            client.PostalAddress = curClient.PostalAddress;
            client.DeliveryAddress = curClient.DeliveryAddress;

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
