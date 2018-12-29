using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Smartflow.BussinessService.Models;
using Smartflow.BussinessService.Services;
using Smartflow.BussinessService.WorkflowService;

namespace Smartflow.Web.Mvc.Code
{
    public class CommonMethods
    {
        public static bool CheckAuth(string nodeID, string instanceID, User userInfo)
        {
            return (new PendingService().Query(pending => pending.ACTORID == userInfo.IDENTIFICATION.ToString()
                && pending.NODEID == nodeID
                && pending.INSTANCEID == instanceID).FirstOrDefault() != null);

        }

        public static bool CheckUndoAuth(string instanceID, User userInfo)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            return instance.Current.GetFromNode().GetActors().Count(e => e.IDENTIFICATION == userInfo.IDENTIFICATION.ToString()) > 0;
        }

        public static bool CheckUndoButton(string instanceID)
        {
            string currentNodeName = BaseWorkflowService.Instance.GetCurrent(instanceID).APPELLATION;
            var executeNode = BaseWorkflowService.Instance.GetCurrentPrevNode(instanceID);
            return (currentNodeName == "开始" || currentNodeName == "结束" || executeNode.APPELLATION == "开始");
        }
    }
}