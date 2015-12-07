using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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

            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var role = RoleManager.FindByName("Administrator");
            var roleId = Guid.Parse(role.Id);
            using (var db = new AromaContext())
            {
                var menuItems = db.SystemMenuList.Where(m => m.Active && m.RoleId.Equals(roleId)).Select(m=>m.SystemMenuListItem).OrderBy(m=>m.Text).ToArray();
                foreach (var mnuItem in menuItems)
                {
                    this.Add(mnuItem.Text, mnuItem.ActionName,mnuItem.ControllerName);
                }
            }
            //fixed items for now, could be fetched from DB at a later stage
            /*
            this.Add("System Accounts", "Index", "Account");
            this.Add("Client Type", "Index", "ClientTypes");
            this.Add("Ethnic Group", "Index", "EthnicGroups");
            this.Add("Title", "Index", "Titles");
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
            this.Add("Financial Account","Index","finAccounts");
            this.Add("Client Account", "Index", "finClientAccounts");
            this.Add("Client Subscription", "Index", "ClientSubscriptions");
            this.Add("Client", "Index", "Clients");
            this.Add("Product", "Index", "Products");
            this.Add("Subscription", "Index", "Subscriptions");
            this.Add("Debit Order", "Index", "DebitOrders");
            this.Add("Tickets", "Index", "SupportTickets");
            this.Add("Ticket Templates", "Index", "SystemTicketTemplates");
            this.Add("SMS", "Manage", "SystemSMS");
            this.Add("SMS Templates", "Index", "SystemSMSTemplates");
            this.Add("Postal Codes", "Index", "PostalCodes");*/
            ////////////////////////////////////////////////////////
            //must always be under admin
            if (this.Where(m => m.Text == "Menu Control").Count() == 0)
            {
                this.Add("Menu Control", "Index", "SystemMenuLists");
            }
            /////////////////////////////////////////////////////////
        }

 
    }
}