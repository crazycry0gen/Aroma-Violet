using Aroma_Indigo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aroma_Indigo.Controllers
{
    public class AdministratorController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var adminView = new AdministratorViewModel();
            adminView.Menu = new AdministratorMenu();
            return View(adminView);
        }

    }
}
