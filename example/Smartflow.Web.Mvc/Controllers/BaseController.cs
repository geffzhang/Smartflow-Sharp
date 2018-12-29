using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Smartflow.Web.Mvc.Code;
using Smartflow.BussinessService.Models;


namespace Smartflow.Web.Mvc.Controllers
{
    [AuthorizationFilter]
    public class BaseController : Controller
    {
        public User UserInfo
        {
            get;
            set;
        }
    }
}