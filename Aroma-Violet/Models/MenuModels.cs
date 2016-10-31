using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public class ApplicationMenuList : List<ApplicationMenuItem>
    {
        public string Text { get; set; }
        public ApplicationMenuItem Add(string text, string actionName, string controllerName, string parameters)
        {
            var newItem = new ApplicationMenuItem(text, actionName, controllerName, parameters);
            this.Add(newItem);
            return newItem;
        }
        public void Save(AromaContext context)
        {
            foreach (var listItem in this)
            {
                var dbItem = context.SystemMenuListItems.FirstOrDefault(m => m.Text == listItem.Text);
                if (dbItem == null)
                {
                    dbItem = new SystemMenuListItem() { Text = listItem.Text, SystemMenuListItemId = Guid.NewGuid() };
                    context.SystemMenuListItems.Add(dbItem);
                }
                dbItem.ActionName = listItem.ActionName;
                dbItem.ControllerName = listItem.ControllerName;
                context.SaveChanges();
            }
        }

    }



    public class ApplicationMenuItem
    {
        public ApplicationMenuItem()
        { }
        public ApplicationMenuItem(string text, string actionName, string controllerName, string parameters)
        {
            this.Text = text;
            this.ActionName = actionName;
            this.ControllerName = controllerName;
            this.Parameters = parameters;
        }
        public string Text { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Parameters { get; private set; }
    }

    public static class ExtendContextWithMenuItems
    {
        public static ApplicationMenuList GetApplicationMenuList(this AromaContext context)
        {
            var items = context.SystemMenuListItems.Where(m=>m.Active).OrderByDescending(m => m.Order).ThenBy(m => m.Text).ToArray();
            var convertedItems = items.Select(m => new ApplicationMenuItem(m.Text, m.ActionName, m.ControllerName, m.Parameters)).ToArray();
            var ret = new ApplicationMenuList();
            ret.AddRange(convertedItems);
            return ret;
        }
    }

}
