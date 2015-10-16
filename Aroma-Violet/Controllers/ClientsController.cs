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
                        || m.ClientInitials.ToLower().Contains(lowSearch)
                        || m.FullNames.ToLower().Contains(lowSearch)
                        || m.ClientSurname.Contains(lowSearch)
                        || m.IDNumber.Contains(lowSearch)
                        ).Take(maxResult)
                        .Include(c => c.ClientType)
                        .Include(c => c.Country)
                        .Include(c => c.EthnicGroup)
                        .Include(c => c.IncomeGroup)
                        .Include(c => c.Province)
                        .Include(c=>c.BankingDetails)
                        .Include(c => c.Title);
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
            Client client = await db.Clients.Include(maxResult => maxResult.ClientSubscriptions).Include(m => m.BankingDetails).FirstAsync(m => m.ClientId == id);
            if (client == null)
            {
                return HttpNotFound();
            }
            ViewBag.ResellerID = (from item in db.ClientRelationShips
                                  where item.ChildID == client.ClientId
                                  && item.Active
                                  select item.ParentID).FirstOrDefault();

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
            ViewBag.CountryID = new SelectList(db.Countries, "CountryId", "CountryName");
            ViewBag.EthnicGroupID = new SelectList(db.EthnicGroups, "EthnicGroupId", "EthnicGroupName");
            ViewBag.IncomeGroupID = new SelectList(db.IncomeGroups, "IncomeGroupId", "IncomeGroupName");
            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceId", "ProvinceName");
            ViewBag.TitleID = new SelectList(db.Titles, "TitleId", "TitleName");
            ViewBag.LanguageID = new SelectList(db.Languages, "LanguageID", "LanguageName");
            ViewBag.AddressTypeID = new SelectList(db.AddressTypes, "AddressTypeID", "AddressTypeName");
            ViewBag.PreviousYearCount = 80;
            ViewBag.NextYearCount = 0;


            var newClient = new ClientViewModel();
            newClient.PostalAddress = GetNewAddress("Postal");
            newClient.DeliveryAddress = GetNewAddress("Physical");
            newClient.DateOfBirth = DateTime.Parse("1 Jan 1950");

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

        private void CreateClientBankingDetails(int clientId, string initials, string surname, int cellContactId, int homeContactId, int workContactId, int emailContactId)
        {
            const string accountHolderText = "Self";
            const string accountTypeText = "Cheque";
            const string BankText = "ABSA";

            var accountHolder = db.AccountHolders.FirstOrDefault(m => m.AccountHolderName == accountHolderText);
            if (accountHolder == null)
            {
                string errorMessage = string.Format("Account holder \"{0}\" not defined in lookup", accountHolderText);
                throw new Exception(errorMessage);
            }
            var accountType = db.AccountTypes.FirstOrDefault(m => m.AccountTypeName == accountTypeText);
            if (accountType == null)
            {
                string errorMessage = string.Format("Account type \"{0}\" not defined in lookup", accountTypeText);
                throw new Exception(errorMessage);
            }
            var bank = db.Banks.FirstOrDefault(m => m.BankName == BankText);
            if (bank == null)
            {
                string errorMessage = string.Format("Bank name \"{0}\" not defined in lookup", BankText);
                throw new Exception(errorMessage);
            }

            var branch = db.Branches.FirstOrDefault(m => m.BankId == bank.BankId);
            if (branch == null)
            {
                string errorMessage = string.Format("No branch defined for \"{0}\" not defined in lookup", BankText);
                throw new Exception(errorMessage);
            }

            if (db.BankingDetails.Where(m => m.ClientID == clientId).Count() == 0)
            {
                var bankingDetail = new BankingDetail()
                {
                    Initials = initials,
                    Surname = surname,
                    AccountHolderID = accountHolder.AccountHolderId,
                    AccountTypeID = accountType.AccountTypeId,
                    BankID = bank.BankId,
                    ClientID = clientId,
                    CommencementDate = DateTime.Now.AddMonths(1),
                    SalaryDate = DateTime.Now,
                    AccountNumber = "0",
                    BranchID = branch.BranchId,
                    CellContact = db.Contacts.First(m => m.ContactId == cellContactId).ContactName,
                    HomeContact = db.Contacts.First(m => m.ContactId == homeContactId).ContactName,
                    WorkContact = db.Contacts.First(m => m.ContactId == workContactId).ContactName,
                    EmailContact = db.Contacts.First(m => m.ContactId == emailContactId).ContactName,
                    Active = false
                };

                db.BankingDetails.Add(bankingDetail);
                db.SaveChanges();
            }
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ClientId,ClientInitials,NickName,FullNames,LanguageID,Employer,DateOfBirth,ClientSurname,SAResident,IDNumber,ClientTypeID,TitleID,EthnicGroupID,IncomeGroupID,PostalAddress,DeliveryAddress,DeliveryAddressLines,ProvinceID,CountryID,Lines,AddressLine,PostalAddressLines,AddressTypeID,DeliveryAddress,PostalAddress,TelWork,Cell,TelHome,EMail,ResellerID")] ClientViewModel clientView)
        {
            var client = clientView.GetBaseClient();
            if (ModelState.IsValid)
            {
                //check if client exists on IDNo and update else add
                var existingClient = (from item in db.Clients
                                      where item.IDNumber == client.IDNumber
                                      select item).FirstOrDefault();
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
                var workContactId = CreateContact(client.ClientId, "Tel (Work)", clientView.TelWork);
                var cellContactId = CreateContact(client.ClientId, "Cell", clientView.Cell);
                var homeContactId = CreateContact(client.ClientId, "Tel (Home)", clientView.TelHome);
                var emailContactId = CreateContact(client.ClientId, "EMail", clientView.Email);

                CreateClientBankingDetails(client.ClientId, client.ClientInitials, client.ClientSurname, cellContactId, homeContactId, workContactId, emailContactId);
                return RedirectToAction("Manage", "ClientSubscriptions", new { ClientID = client.ClientId });
            }
            
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", client.ClientTypeID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryId", "CountryName", client.CountryID);
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

            if (rel != null) clientView.ResellerID = rel.ParentID;

            clientView.Contacts = (from item in db.ContactTypes.Where(m => m.Active).ToArray()
                                   select new ClientContactView {ClientContactTypeName =  item.ContactTypeName,ClientContactTypeId = item.ContactTypeId, ClientContactValue=  db.Contacts.Where(m=>m.Active
                                                                                                && m.ContactTypeID == item.ContactTypeId
                                                                                                && m.ClientID==id).Select(m=>m.ContactName).FirstOrDefault()}).ToArray();



            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeId", "ClientTypeName", client.ClientTypeID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryId", "CountryName", client.CountryID);
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
            curContact.ContactName = value;
            db.SaveChanges();
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ClientId, ResellerID,ClientInitials,NickName,FullNames,LanguageID,Employer,DateOfBirth,ClientSurname,SAResident,IDNumber,ClientTypeID,TitleID,EthnicGroupID,IncomeGroupID,ProvinceID,CountryID,Active")] ClientViewModel clientView)
        {
            var client = clientView.GetBaseClient();
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
