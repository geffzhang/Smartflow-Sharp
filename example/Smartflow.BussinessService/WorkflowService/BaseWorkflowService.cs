/*
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://github.com/chengderen/Smartflow
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Smartflow.BussinessService.Models;
using Smartflow;
using Smartflow.Elements;
using System.Dynamic;
using Smartflow.BussinessService.Services;
using System.Threading.Tasks;

namespace Smartflow.BussinessService.WorkflowService
{
    public partial class BaseWorkflowService
    {
        private static WorkflowEngine context = BaseWorkflowEngine.CreateWorkflowEngine();
        private readonly static BaseWorkflowService singleton = new BaseWorkflowService();
        private RecordService recordService = new RecordService();
        private PendingService pendingService = new PendingService();

        private BaseWorkflowService()
        {
            WorkflowEngine.OnProcess += new DelegatingProcessHandle(OnProcess);
            WorkflowEngine.OnCompleted += new DelegatingCompletedHandle(OnCompleted);
        }

        public static BaseWorkflowService Instance
        {
            get { return singleton; }
        }

        public ASTNode GetCurrent(string instanceID)
        {
            return GetCurrentNode(instanceID);
        }

        public ASTNode GetCurrentPrevNode(string instanceID)
        {
            var current = GetCurrentNode(instanceID);
            return current.GetFromNode();
        }

        public WorkflowNode GetCurrentNode(string instanceID)
        {
            return WorkflowInstance.GetInstance(instanceID).Current;
        }

        public void UndoSubmit(string instanceID, string actorID, string actorName, string bussinessID)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            dynamic dynData = new ExpandoObject();
            dynData.bussinessID = bussinessID;
            dynData.Message = "撤销此节点";
            context.Cancel(new WorkflowContext()
            {
                Instance = instance,
                Data = dynData,
                ActorID = actorID,
                ActorName = actorName
            });
        }

        public void Rollback(string instanceID, string actorID, string actorName, dynamic data)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            context.Rollback(new WorkflowContext()
            {
                Instance = instance,
                Data = data,
                ActorID = actorID,
                ActorName = actorName
            });
        }

        public string Start(string identification)
        {
            WorkflowDesignService workflowDesignService = new WorkflowDesignService();
            WorkflowStructure structure = workflowDesignService.GetWorkflowStructure(identification);
            return context.Start(structure.STRUCTUREXML);
        }

        public void Jump(string instanceID, string transitionID, string actorID, string actorName, dynamic data)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            context.Jump(new WorkflowContext()
            {
                Instance = instance,
                TransitionID = transitionID,
                Data = data,
                ActorID = actorID,
                ActorName = actorName
            });
        }
    }
}