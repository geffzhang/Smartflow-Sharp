using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Smartflow.Web.Mvc.Code;
using Smartflow.BussinessService.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

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

        public JsonResult JsonWrapper(Object data)
        {
            return new JsonResultWrapper()
            {
                Data = data,
                ContentType = "application/json"
            };
        }

        public JsonResult JsonToLowerWrapper(Object data)
        {
            return new JsonResultWrapper()
            {
                ContractResolver = new LowerCaseContractResolver(),
                Data = data,
                ContentType = "application/json"
            };
        }
    }

    public class JsonResultWrapper : JsonResult
    {
        public IContractResolver ContractResolver
        {
            get;
            set;
        }

        public JsonResultWrapper()
            : base()
        {
            ContractResolver = new UpperCaseContractResolver();
        }

        public JsonResultWrapper(IContractResolver ContractResolver)
            : base()
        {
            this.ContractResolver = ContractResolver;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(this.Data,
                new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = ContractResolver
                });
            response.Write(data);
        }
    }

    public class UpperCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToUpper();
        }
    }

    public class LowerCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}