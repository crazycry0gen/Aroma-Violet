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
using Microsoft.AspNet.Identity.EntityFramework;

namespace Aroma_Violet.Controllers
{
    public class SystemMenuListsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SystemMenuLists
        public async Task<ActionResult> Index()
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var l = await  RoleManager.Roles.Select(m=>new {Id=m.Id, Name = m.Name }).ToListAsync();
            ViewBag.Roles = l.Select(m => new KeyValuePair<string,string>(m.Id, m.Name));
            ViewBag.MenuItems = db.GetApplicationMenuList();
            return View(await db.SystemMenuList.Where(m=>m.Active).Include(mbox=>mbox.SystemMenuListItem).ToListAsync());
        }

        // GET: SystemMenuLists/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemMenuList systemMenuList = await db.SystemMenuList.FindAsync(id);
            if (systemMenuList == null)
            {
                return HttpNotFound();
            }
            return View(systemMenuList);
        }

        // GET: SystemMenuLists/Create
        public ActionResult Create()
        {
            return View();
        }

        public JsonResult UpdateMenu(bool status, Guid roleId, string menuText)
        {
            var response = string.Empty;
            try
            {
                var menuItem = db.SystemMenuListItems.Where(m => m.Text == menuText).First();
                var listItem = db.SystemMenuList.Where(m => m.SystemMenuListItemId.Equals(menuItem.SystemMenuListItemId) && m.RoleId.Equals(roleId)).FirstOrDefault();
                if(!status && listItem==null) return Json(response);
                if (status && listItem == null)
                {
                    listItem = new SystemMenuList() {SystemMenuListItemId=menuItem.SystemMenuListItemId, RoleId=roleId };
                    db.SystemMenuList.Add(listItem);
                }
                listItem.Active = status;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            return Json(response);
        }

        // POST: SystemMenuLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SystemMenuListId,RoleId,SystemMenuListItemId,Order")] SystemMenuList systemMenuList)
        {
            if (ModelState.IsValid)
            {
                db.SystemMenuList.Add(systemMenuList);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(systemMenuList);
        }

        // GET: SystemMenuLists/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemMenuList systemMenuList = await db.SystemMenuList.FindAsync(id);
            if (systemMenuList == null)
            {
                return HttpNotFound();
            }
            return View(systemMenuList);
        }

        // POST: SystemMenuLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SystemMenuListId,RoleId,SystemMenuListItemId,Order")] SystemMenuList systemMenuList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemMenuList).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(systemMenuList);
        }

        // GET: SystemMenuLists/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemMenuList systemMenuList = await db.SystemMenuList.FindAsync(id);
            if (systemMenuList == null)
            {
                return HttpNotFound();
            }
            return View(systemMenuList);
        }

        // POST: SystemMenuLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SystemMenuList systemMenuList = await db.SystemMenuList.FindAsync(id);
            db.SystemMenuList.Remove(systemMenuList);
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
