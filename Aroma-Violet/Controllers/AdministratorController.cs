﻿using Aroma_Violet.Models;
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
            var context = new AromaContext();
            OrderHeadersController.FixScewedTotals(context);
            adminView.Menu.Save(context);
            return View(adminView);
        }

    }
}
