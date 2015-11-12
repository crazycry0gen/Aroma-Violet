using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var role = RoleManager.FindByName("User");
            var roleId = Guid.Parse(role.Id);
            using (var db = new AromaContext())
            {
                var menuItems = db.SystemMenuList.Where(m =>m.Active && m.RoleId.Equals(roleId)).Select(m => m.SystemMenuListItem).ToArray();
                foreach (var mnuItem in menuItems)
                {
                    this.Add(mnuItem.Text, mnuItem.ActionName, mnuItem.ControllerName);
                }
            }
            ////fixed items for now, could be fetched from DB at a later stage
            //this.Add("Capture Application", "Create", "Clients");
            //this.Add("Client", "Index", "Clients");
            //this.Add("Support tickets", "Index", "SupportTickets");

        }
    }

}