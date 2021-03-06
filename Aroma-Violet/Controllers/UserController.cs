﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aroma_Violet.Models;

namespace Aroma_Violet.Controllers
{
    public class UserController : Controller
    {
        [Authorize]
        public ActionResult Index(string spec = "User")
        {
            var adminView = new UserViewModel();
            adminView.Menu = new UserMenu(spec);
            return View(adminView);
        }
    }
}