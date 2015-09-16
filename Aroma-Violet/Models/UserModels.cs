using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public class UserViewModel
    {
        public UserMenu Menu { get; set; }
    }

    public class UserMenu : ApplicationMenuList
    {
        public UserMenu()
        {
            this.Text = "Menu";
            //fixed items for now, could be fetched from DB at a later stage
            this.Add("Capture Application", "Create", "Clients");
            this.Add("Client", "Index", "Clients");
        }
    }

}