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
using System.IO;

namespace Aroma_Violet.Controllers
{
    public class PostalCodesController : Controller
    {
        private AromaContext db = new AromaContext();

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase FileUpload)
        {
            if (FileUpload != null && FileUpload.ContentLength > 0)
            {

                string fileName = Path.GetFileName(FileUpload.FileName);
                string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), Guid.NewGuid().ToString() + "-" + fileName);


                try
                {
                    var testFile = new FileInfo(path);
                    if (!testFile.Directory.Exists) testFile.Directory.Create();
                    FileUpload.SaveAs(path);
                    int importCount = ProcessCSV(path);

                    ViewData["Feedback"] = "Success";
                }
                catch (Exception ex)
                {

                    ViewData["Feedback"] = ex.Message;
                }
            }

            return RedirectToAction("Index");
        }

        private int ProcessCSV(string path)
        {
            char[] rowsplit = "\r\n".ToCharArray();
            char[] colsplit = ",".ToCharArray();
            int importCount = 0;
            var file = new FileInfo(path);
            using (var reader = file.OpenText())
            {
                var result = reader.ReadToEnd();
                var rows = result.Split(rowsplit, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (rows[0].StartsWith("Country")) rows.RemoveAt(0);
                var matrix = (from item in rows
                              select item.Split(colsplit, StringSplitOptions.None)).ToArray();
                foreach (var row in matrix)
                {
                     var postalCodeName = row[2].Trim();
                    var checkpostalCode = postalCodeName.ToLower();
                    var postalCode = db.PostalCodes.FirstOrDefault(m => m.PostalCodeName.ToLower() == checkpostalCode);
                    if (postalCode == null)
                    {
                        var countryName = row[0].Trim();
                        var checkCountry = countryName.ToLower();
                        var country = db.Countries.OrderBy(m=>m.CountryName).FirstOrDefault(m => m.CountryName.ToLower() == checkCountry);
                        if (country == null)
                        {
                            country = new Country() { Active = true, CountryName = countryName };
                            db.Countries.Add(country);
                        }

                        var provinceName = row[1].Trim();
                        var checkProvince = provinceName.ToLower();
                        var province = db.Provinces.FirstOrDefault(m => m.ProvinceName.ToLower() == checkProvince);
                        if (province == null)
                        {
                            province = new Province() { Active = true, ProvinceName = provinceName };
                            db.Provinces.Add(province);
                        }

                        var postalAreaName = row[3].Trim();
                        var checkpostalArea = postalAreaName.ToLower();
                        var postalArea = db.PostalAreas.FirstOrDefault(m => m.PostalAreaName.ToLower() == checkpostalArea);
                        if (postalArea == null)
                        {
                            postalArea = new PostalArea() { Active = true, PostalAreaName = postalAreaName };
                            db.PostalAreas.Add(postalArea);
                        }

                        db.SaveChanges();
                        ////////////////////////////////////////////////////////////////////////////

                        postalCode = new PostalCode()
                        {
                            Active = true,
                            Country = country,
                            PostalArea = postalArea,
                            Province = province,
                            PostalCodeName = postalCodeName
                        };
                        db.PostalCodes.Add(postalCode);
                    }
                }
            }
                return importCount;
        }

     
       
        public async Task<ActionResult> Index(string filterPostalCodes)
        {
            ViewBag.Filter = filterPostalCodes;
            if (filterPostalCodes != null && filterPostalCodes.Length > 0)
            {
                return View(await db.PostalCodes.OrderBy(m=>m.PostalCodeName).Where(m => m.PostalCodeName.Contains(filterPostalCodes)).Take(20).ToListAsync());
            }
            else
            {
                return View(await db.PostalCodes.OrderBy(m => m.PostalCodeName).Take(20).ToListAsync());
            }
        }
        // GET: PostalCodes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostalCode postalCode = await db.PostalCodes.FindAsync(id);
            if (postalCode == null)
            {
                return HttpNotFound();
            }
            return View(postalCode);
        }

        // GET: PostalCodes/Create
        public ActionResult Create()
        {
            ViewBag.CountryID = new SelectList(db.Countries.OrderBy(m=>m.CountryName), "CountryId", "CountryName");
            ViewBag.ProvinceId = new SelectList(db.Provinces.OrderBy(m=>m.ProvinceName), "ProvinceId", "ProvinceName");
            ViewBag.PostalAreaId = new SelectList(db.PostalAreas.OrderBy(m=>m.PostalAreaName), "PostalAreaId", "PostalAreaName");

            return View();
        }

        // POST: PostalCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PostalCodeId,PostalCodeName,Active,CountryId,ProvinceId,PostalAreaId")] PostalCode postalCode)
        {
            
            if (ModelState.IsValid)
            {
                db.PostalCodes.Add(postalCode);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CountryID = new SelectList(db.Countries.OrderBy(m=>m.CountryName).OrderBy(m=>m.CountryName), "CountryId", "CountryName");
                        ViewBag.ProvinceId = new SelectList(db.Provinces.OrderBy(m=>m.ProvinceName), "ProvinceId", "ProvinceName");
            ViewBag.PostalAreaId = new SelectList(db.PostalAreas.OrderBy(m=>m.PostalAreaName), "PostalAreaId", "PostalAreaName");

            return View(postalCode);
        }

        // GET: PostalCodes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostalCode postalCode = await db.PostalCodes.FindAsync(id);
            if (postalCode == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryID = new SelectList(db.Countries.OrderBy(m=>m.CountryName), "CountryId", "CountryName",postalCode.CountryId);
            ViewBag.ProvinceId = new SelectList(db.Provinces, "ProvinceId", "ProvinceName",postalCode.ProvinceId);
            ViewBag.PostalAreaId = new SelectList(db.PostalAreas, "PostalAreaId", "PostalAreaName",postalCode.PostalAreaId);
            return View(postalCode);
        }

        // POST: PostalCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PostalCodeId,PostalCodeName,Active,CountryId,ProvinceId,PostalAreaId")] PostalCode postalCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(postalCode).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CountryID = new SelectList(db.Countries.OrderBy(m=>m.CountryName), "CountryId", "CountryName", postalCode.CountryId);
            ViewBag.ProvinceId = new SelectList(db.Provinces, "ProvinceId", "ProvinceName", postalCode.ProvinceId);
            ViewBag.PostalAreaId = new SelectList(db.PostalAreas, "PostalAreaId", "PostalAreaName", postalCode.PostalAreaId);
            return View(postalCode);
        }

        // GET: PostalCodes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostalCode postalCode = await db.PostalCodes.FindAsync(id);
            if (postalCode == null)
            {
                return HttpNotFound();
            }
            return View(postalCode);
        }

        // POST: PostalCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PostalCode postalCode = await db.PostalCodes.FindAsync(id);
            db.PostalCodes.Remove(postalCode);
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
