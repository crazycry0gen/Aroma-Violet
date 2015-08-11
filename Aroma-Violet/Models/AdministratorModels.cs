using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
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
            this.Add("Client Type", "Index", "ClientTypes");
            this.Add("Ethnic Group", "Index", "EthnicGroups");
            this.Add("Titel", "Index", "Titels");
            this.Add("Language", "Index", "Languages");
            this.Add("Income Group", "Index", "IncomeGroups");
            this.Add("Address Type", "Index", "AddressTypes");
            this.Add("Province", "Index", "Provinces");
            this.Add("Country", "Index", "Countries");
            this.Add("Contact Type", "Index", "ContactTypes");
            this.Add("Account Holder", "Index", "AccountHolders");
            this.Add("Account Type", "Index", "AccountTypes");
            this.Add("Bank", "Index", "Banks");
            this.Add("Branch", "Index", "Branches");
            this.Add("Contact Type", "Index", "ContactTypes");
            this.Add("Client", "Index", "Clients");
            this.Add("Product", "Index", "Products");
            this.Add("Subscription", "Index", "Product");
        }
    }
}