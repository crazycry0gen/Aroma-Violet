using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Indigo.Models
{
    public class ApplicationMenuList : List<ApplicationMenuItem>
    {
        public string Text { get; set; }
        public ApplicationMenuItem Add(string text, string actionName, string controllerName)
        {
            var newItem = new ApplicationMenuItem(text, actionName, controllerName);
            this.Add(newItem);
            return newItem;
        }
    }

    public class ApplicationMenuItem
    {
        public ApplicationMenuItem()
        { }
        public ApplicationMenuItem(string text, string actionName, string controllerName)
        {
            this.Text = text;
            this.ActionName = ActionName;
            this.ControllerName = controllerName;
        }
        public string Text { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
    }
}