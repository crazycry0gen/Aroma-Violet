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
using Microsoft.AspNet.Identity;

namespace Aroma_Violet.Controllers
{
    public class SystemSMSController : Controller
    {
        private AromaContext db = new AromaContext();
        [Authorize]
        [HttpGetAttribute]
        public async Task<ActionResult> Manage(int? clientId, int smsCount=0)
        {
            if ((SMSDistributionViewModel.Items== null || SMSDistributionViewModel.Items.Count==0 ) && !clientId.HasValue)
            {
                SMSDistributionViewModel.Items = new List<SMSDistributionItemModel>();
                SMSDistributionViewModel.Items.AddRange(await db.Countries.Where(m => m.Active).Select(m=>new SMSDistributionItemModel() {Id= m.CountryId, Description=m.CountryName, ItemType=SMSDistributionViewModel.enumSMSDistributionItemType.Country}).ToListAsync());
                SMSDistributionViewModel.Items.AddRange(await db.Provinces.Where(m => m.Active).Select(m=>new SMSDistributionItemModel() {Id= m.ProvinceId, Description=m.ProvinceName, ItemType=SMSDistributionViewModel.enumSMSDistributionItemType.Province}).ToListAsync());
                SMSDistributionViewModel.Items.AddRange(await db.PostalAreas.Where(m => m.Active).Select(m=>new SMSDistributionItemModel() {Id= m.PostalAreaId, Description=m.PostalAreaName, ItemType=SMSDistributionViewModel.enumSMSDistributionItemType.PostalArea}).ToListAsync());
                SMSDistributionViewModel.Items.AddRange(await db.PostalCodes.Where(m => m.Active).Select(m=>new SMSDistributionItemModel() {Id= m.PostalCodeId, Description=m.PostalCodeName, ItemType=SMSDistributionViewModel.enumSMSDistributionItemType.PostalCode}).ToListAsync());
                SMSDistributionViewModel.Items.AddRange(await db.ClientTypes.Where(m => m.Active).Select(m => new SMSDistributionItemModel() { Id = m.ClientTypeId, Description = m.ClientTypeName, ItemType = SMSDistributionViewModel.enumSMSDistributionItemType.ClientType }).ToListAsync());

                SMSDistributionViewModel.RelationShips = (from item in db.PostalCodes.Where(m=>m.Active).ToArray()
                                                  select new int[] { item.Country.CountryId, item.Province.ProvinceId, item.PostalArea.PostalAreaId, item.PostalCodeId }).ToArray();
            }

            var manager = new SMSManagerViewModel();
            manager.UnsentSMSCount = await db.SystemSMSes.Where(m => m.SystemSMSStatusId == 1).CountAsync();
            manager.LastSendAttempt = await db.SystemSMSes.Where(m => m.SystemSMSStatusId==1).Select(m=> (DateTime?)m.LastSendAttempt).MaxAsync();
            manager.LastSMSAdded = await db.SystemSMSes.Select(m => (DateTime?)m.iDate).MaxAsync();
            manager.LastSuccessfulSend = await db.SystemSMSes.Where(m => m.SystemSMSStatusId == 2).Select(m => (DateTime?)m.LastSendAttempt).MaxAsync();
            manager.ClientSMSCount = smsCount;
            manager.ClientSMSMaxCount = await db.Clients.Where(m => m.Active).CountAsync();
            manager.SystemSMSTemplateID = 0;
            var itemList = await db.SystemSMSTemplates.ToListAsync();
            itemList.Insert(0,new SystemSMSTemplate() { SystemSMSTemplateId = 0, Description = "Custom",Text=string.Empty });
            var nlist = new SelectList(itemList, "SystemSMSTemplateId","Description", null);
            ViewBag.Templates = itemList.Select(m=>m.Text).ToArray();
            ViewBag.SystemSMSTemplateID = nlist;

            if (clientId.HasValue)
            {
                manager.Countries = new List<SMSDistributionItemModel>();
                ViewBag.Client = db.Clients.FirstOrDefault(m=>m.ClientId==clientId.Value);
            }

            if (ViewBag.Client != null)
            {
                manager.ClientID = clientId.Value;
                manager.ClientSMSCount = 1;
                manager.ClientSMSMaxCount = 1;
            }
            else
            {
                manager.Countries = SMSDistributionViewModel.Items.Where(m => m.ItemType == SMSDistributionViewModel.enumSMSDistributionItemType.Country).ToList();
                manager.ClientTypes = SMSDistributionViewModel.Items.Where(m => m.ItemType == SMSDistributionViewModel.enumSMSDistributionItemType.ClientType).ToList();

            }
            manager.Variables = SystemSMSTemplateModel.GetVariableList();

            return View(manager);
        }

        [HttpPost]
        public async Task<JsonResult> Preview(string SMSText, int SystemSMSTemplateID, int clientId)
        {
            var textResult = string.Empty;
            if (SystemSMSTemplateID > 0)
            {
                var result = await db.SystemSMSTemplates.FirstAsync(m => m.SystemSMSTemplateId == SystemSMSTemplateID);
                SMSText = result.Text;
            }
               var  client = await db.Clients.FirstOrDefaultAsync(m => m.ClientId == clientId) ;
            if(client != null)
            {
                var smsSub = new SystemSMSTemplateModel(SMSText, client);
                textResult = smsSub.Generate();
            }
            return Json(textResult);
        }

        [HttpPost]
        public async Task<ActionResult> Manage(int[] selectedCountries, int[] selectedProvinces, int[] selectedAreas, int[] selectedCodes,int[] selectedClientTypes, string SMSText, int SystemSMSTemplateID, int? clientId, bool GetSMSCount)
        {

            var userId = Guid.Parse( User.Identity.GetUserId());

            if (SystemSMSTemplateID > 0)
            {
                SMSText = db.SystemSMSTemplates.First(m => m.SystemSMSTemplateId == SystemSMSTemplateID).Text;
            }

            var selectedItems = new List<SMSDistributionItemModel>();
            selectedItems.AddRange(SMSDistributionViewModel.Find(selectedCountries, SMSDistributionViewModel.enumSMSDistributionItemType.Country));
            selectedItems.AddRange(SMSDistributionViewModel.Find(selectedProvinces, SMSDistributionViewModel.enumSMSDistributionItemType.Province));
            selectedItems.AddRange(SMSDistributionViewModel.Find(selectedAreas, SMSDistributionViewModel.enumSMSDistributionItemType.PostalArea));
            selectedItems.AddRange(SMSDistributionViewModel.Find(selectedCodes, SMSDistributionViewModel.enumSMSDistributionItemType.PostalCode));
            
            var parents = (from item in selectedItems.Where(m=>m.Parent!=null).ToArray()
                           select item.Parent.Key).ToArray();
            selectedItems.RemoveAll(m => parents.Contains(m.Key));
            var codes = (from item in await db.PostalCodes.Where(m=>m.Active).ToArrayAsync()
                         where (
                         selectedItems.Where(m => m.ItemType == SMSDistributionViewModel.enumSMSDistributionItemType.Country).Select(m => m.Id).Contains(item.Country.CountryId)
                         || selectedItems.Where(m => m.ItemType == SMSDistributionViewModel.enumSMSDistributionItemType.Province ).Select(m => m.Id).Contains(item.Province.ProvinceId)
                         || selectedItems.Where(m => m.ItemType == SMSDistributionViewModel.enumSMSDistributionItemType.PostalArea).Select(m => m.Id).Contains(item.PostalArea.PostalAreaId)
                         || selectedItems.Where(m => m.ItemType == SMSDistributionViewModel.enumSMSDistributionItemType.PostalCode).Select(m => m.Id).Contains(item.PostalCodeId)
                         )
                         select item.PostalCodeName).ToArray();

            Client[] clients = null;

            if (clientId.HasValue && clientId.Value > 0)
            {
                clients = new Client[] { db.Clients.FirstOrDefault(m=>m.ClientId==clientId.Value)};
            }
            else
            {
                clients = (from item in await db.Clients.Where(m => m.Active).ToArrayAsync()
                            where codes.Contains(item.DeliveryAddress.Code)
                            && selectedClientTypes.Contains(item.ClientTypeID)
                           select item).ToArray();

            }

            if (GetSMSCount)
            {
             return RedirectToAction("Manage", new { smsCount = clients.Length, clientid = clientId });
            }
            else
            {

                var smsesSubs = (from item in clients
                                 select new SystemSMSTemplateModel(SMSText, item));

                var entries = (from item in smsesSubs
                               select new SystemSMS()
                               {
                                   Active = true,
                                   ClientID = item.ClientID,
                                   iDate = DateTime.Now,
                                   Number = item.Cell,
                                   SMSDescription = item.Generate(),

                                   Source = userId,
                                   SystemSMSStatusId = 1
                               }).ToArray();

                var links = (from item in entries
                             select new SystemLink()
                             {
                                 UserID = userId,
                                 Created = DateTime.Now,
                                 LinkId = Guid.NewGuid(),
                                 Parent = ClientKey(item.ClientID)
                             }).ToArray();

                db.SystemSMSes.AddRange(entries);
                db.SystemLinks.AddRange(links);
                db.SaveChanges();

                return RedirectToAction("Index", new { clientId = clientId });
            }

        }

        public Guid ClientKey(int clientId)
        {
            var res = db.ClientGuids.FirstOrDefault(m => m.ClientId == clientId);
            if (res == null)
            {
                res = new ClientGuid()
                {
                    ClientId = clientId,
                    ClientKey = Guid.NewGuid()
                };
                db.ClientGuids.Add(res);
                db.SaveChanges();
            }
            return res.ClientKey;
        }

        [HttpPost]
        public JsonResult FetchSMSDistributionItems(int parentId, int type)
        {
            var parentItem = SMSDistributionViewModel.Items.Where(m => m.Id == parentId && (int)m.ItemType == type).First();
            var children = (from SMSDistributionItemModel item in parentItem.Children.OrderBy(m=>m.Description)
                            select new {
                                id=item.Id,
                                description=item.Description,
                                type=(int)item.ItemType,
                                childCount=item.Children.Count()
                            }).ToArray();
            return  Json(children);
        }

        [HttpPost]
        public int GetClientCountPerCountries(List<int> CountryIDs)
        {
            var postalCodes = db.PostalCodes.Where(m => CountryIDs.Contains(m.Country.CountryId)).Select(m=>m.PostalCodeName).ToArray();
            var count = db.Clients.Where(m => m.Active && postalCodes.Contains(m.DeliveryAddress.Code)).Count();
            return count;
        }

        [HttpPost]
        public int GetClientCountPerProvinces(List<int> ProvinceIDs)
        {
            var postalCodes = db.PostalCodes.Where(m => ProvinceIDs.Contains(m.Province.ProvinceId)).Select(m => m.PostalCodeName).ToArray();
            var count = db.Clients.Where(m => m.Active && postalCodes.Contains(m.DeliveryAddress.Code)).Count();
            return count;
        }

        [HttpPost]
        public int GetClientCountPerPostalArea(List<int> PostalAreaIDs)
        {
            var postalCodes = db.PostalCodes.Where(m => PostalAreaIDs.Contains(m.PostalArea.PostalAreaId)).Select(m => m.PostalCodeName).ToArray();
            var count = db.Clients.Where(m => m.Active && postalCodes.Contains(m.DeliveryAddress.Code)).Count();
            return count;
        }

        [HttpPost]
        public int GetClientCountPerPostalCode(List<int> PostalCodeIDs)
        {
            var postalCodes = db.PostalCodes.Where(m => PostalCodeIDs.Contains(m.PostalCodeId)).Select(m => m.PostalCodeName).ToArray();
            var count = db.Clients.Where(m => m.Active && postalCodes.Contains(m.DeliveryAddress.Code)).Count();
            return count;
        }

        // GET: SystemSMS
        public async Task<ActionResult> Index(int? clientId, int? start)
        {
            const int smsPerScreen = 10;
            var smses = db.SystemSMSes.AsQueryable();

            if (clientId.HasValue && clientId.Value>0)
            {
                smses = smses.Where(m => m.ClientID == clientId.Value);
            }

            smses = smses.OrderByDescending(m => m.iDate);
            if (start.HasValue)
            {
                smses = smses.Skip(start.Value);
            }

            ViewBag.ClientID = clientId.HasValue ? clientId.Value : 0;
            ViewBag.Start = start.HasValue ? start.Value : 0;

            var res = await smses.Take(smsPerScreen).ToListAsync();
            return View(res);
        }

        // GET: SystemSMS/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSMS systemSMS = await db.SystemSMSes.FindAsync(id);
            if (systemSMS == null)
            {
                return HttpNotFound();
            }
            return View(systemSMS);
        }

        // GET: SystemSMS/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: SystemSMS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SystemSMSId,ClientID,Number,SMSDescription,iDate,Sent,LastSendAttempt,LastSendMessage,Active,Source,SystemSMSStatusId")] SystemSMS systemSMS)
        {
            if (ModelState.IsValid)
            {
                db.SystemSMSes.Add(systemSMS);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(systemSMS);
        }

        // GET: SystemSMS/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSMS systemSMS = await db.SystemSMSes.FindAsync(id);
            if (systemSMS == null)
            {
                return HttpNotFound();
            }
            var itemList = await db.SystemSMSStatuses.ToListAsync();
            var nlist = new SelectList(itemList, "SystemSMSTemplateId", "Description", null);
            ViewBag.SystemSMSTemplateID = nlist;
            return View(systemSMS);
        }

        // POST: SystemSMS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SystemSMSId,ClientID,Number,SMSDescription,iDate,Sent,LastSendAttempt,LastSendMessage,Active,Source,SystemSMSStatusId")] SystemSMS systemSMS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemSMS).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(systemSMS);
        }

        // GET: SystemSMS/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSMS systemSMS = await db.SystemSMSes.FindAsync(id);
            if (systemSMS == null)
            {
                return HttpNotFound();
            }
            return View(systemSMS);
        }

        // POST: SystemSMS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SystemSMS systemSMS = await db.SystemSMSes.FindAsync(id);
            db.SystemSMSes.Remove(systemSMS);
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
