using Aroma_Violet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aroma_Violet.Controllers
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
