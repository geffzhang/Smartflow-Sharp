using Smartflow.BussinessService.Models;
using Smartflow.Web.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartflow.Web.Mvc.Code
{
    public class AuthorizationFilter : ActionFilterAttribute
    {
        public AuthorizationFilter()
            : base()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var attrs = filterContext.ActionDescriptor.GetCustomAttributes(false);
            if (filterContext.Controller is BaseController && attrs.Count(f => (f is UnAuthorizationMethodFilterAttribute)) == 0)
            {
                if (filterContext.HttpContext.Session["user"] == null)
                {
                    filterContext.Result = new EmptyResult(); 
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    User userInfo = (filterContext.HttpContext.Session["user"] as User);
                    BaseController baseController = (filterContext.Controller as BaseController);
                    baseController.UserInfo = userInfo;
                    
                    base.OnActionExecuting(filterContext);
                }
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}