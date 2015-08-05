using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Indigo.Models
{
    public class AdministratorViewModel
    {
        public AdministratorMenu Menu { get; set; }
    }

    public class AdministratorMenu:ApplicationMenuList
    {
        public AdministratorMenu()
        {
            this.Text = "Administrator Menu";
            //fixed items for now, could be fetched from DB at a later stage
            this.Add("Client Type", "Index", "ClientType");
            this.Add("Client", "Index", "Client");
            this.Add("Product", "Index", "Product");
            this.Add("Subscription", "Index", "Product");
        }
    }
}