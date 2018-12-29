using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartflow.Web.Mvc.Code
{
    public class SessionOnlineViewResult  : ContentResult
    {
        public SessionOnlineViewResult(ControllerBase controller)
            : base()
        {
            Content = ReaderPartialViewToStrings(controller, "SessionOnline", null);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Content != null)
            {
                response.Write(Content);
            }

            base.ExecuteResult(context);
        }

        public static string ReaderPartialViewToStrings(ControllerBase controller, string viewName, object model)
        {
            IView view = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName).View;
            controller.ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(controller.ControllerContext, view, controller.ViewData, controller.TempData, sw);

                viewContext.View.Render(viewContext, sw);

                return sw.ToString();
            }
        }
    }
}