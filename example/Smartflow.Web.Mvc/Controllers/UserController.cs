using Smartflow.BussinessService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartflow.Web.Mvc.Controllers
{
    public class UserController : Controller
    {
        private UserService userService = new UserService();

        public JsonResult GetUser(string userName)
        {
            //演示使用
            Smartflow.BussinessService.Models.User userInfo = new UserService()
                .Get(u=>u.USERNAME==userName);

            if (userInfo == null)
            {
                return Json(false);
            }
            else
            {
                System.Web.HttpContext.Current.Session["user"] = userInfo;
                return Json(true);
            }
        }
    }
}
