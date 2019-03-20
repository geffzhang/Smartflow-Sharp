/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Script.Serialization;

using Smartflow;
using Smartflow.Elements;
using Newtonsoft.Json;
using Smartflow.BussinessService.WorkflowService;

namespace Smartflow.Web.Mvc.Controllers
{
    public class WorkflowDesignController : BaseController
    {
        private WorkflowDesignService designService = new WorkflowDesignService();
        private ActorService roleService = new ActorService();

        public ActionResult Design(string id)
        {
            ViewBag.WFID = id;
            return View();
        }

        public JsonResult GetWorkflowStructure(string WFID)
        {
            WorkflowStructure workflowStructure = designService.GetWorkflowStructure(WFID);
            return JsonToLowerWrapper(new
            {
                appellation = workflowStructure.APPELLATION,
                structure = GetNodeList(workflowStructure.STRUCTUREXML)
            });
        }

        public JsonResult Save(WorkflowStructure model)
        {
            model.STRUCTUREXML = System.Web.HttpContext.Current.Server.UrlDecode(model.STRUCTUREXML);
            if (String.IsNullOrEmpty(model.IDENTIFICATION))
            {
                model.IDENTIFICATION = Guid.NewGuid().ToString();
                designService.Persistent(model);
            }
            else
            {
                designService.Update(model);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WorkflowImage(string instanceID)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                structure = GetImageList(instance.Resource),
                id = instance.Current.ID
            }, new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new LowerCaseContractResolver()
            });

            ViewBag.Result = data;
            DataTable dt = WorkflowNode.GetRecord(instanceID);
            ViewBag.Record = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return View();
        }

        public ActionResult WorkflowDesignSettings()
        {
            return View();
        }

        public JsonResult GetRole(string roleIds, string searchKey)
        {
            return JsonWrapper(roleService.GetRole(roleIds, searchKey));
        }

        public JsonResult GetConfigs()
        {
            return JsonWrapper(WorkflowConfig.GetSettings());
        }

        private List<Node> GetNodeList(string resource)
        {
            Workflow workflow = XmlConfiguration.ParseflowXml<Workflow>(resource);
            List<Node> elements = new List<Node>();
            elements.Add(workflow.StartNode);
            elements.AddRange(workflow.ChildNode);
            elements.AddRange(workflow.ChildDecisionNode);
            elements.Add(workflow.EndNode);
            return elements;
        }

        private List<Node> GetImageList(string resource)
        {
            List<Node> elements = this.GetNodeList(resource);
            elements.ForEach(n =>
            {
                n.WebView = null;
            });
            return elements;
        }
    }
}
