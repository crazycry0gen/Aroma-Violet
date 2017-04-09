using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Aroma_Violet.Models
{
    public static class Extend
    {
        public static string RenderViewToString(this Controller parent, object model)
        {
            string viewName = GetCurrentMethod();
            parent.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(parent.ControllerContext, viewName);
                var viewContext = new ViewContext(parent.ControllerContext, viewResult.View, parent.ViewData, parent.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(parent.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);

            return sf.GetMethod().Name;
        }
    }
    
}