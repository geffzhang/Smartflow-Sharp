/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Smartflow.Dapper;
using Smartflow.Elements;
using Smartflow.Enums;

namespace Smartflow
{
    public partial class WorkflowService :WorkflowInfrastructure, IWorkflow
    {

        public Smartflow.Elements.Form Ready(string resourceXml)
        {
            Workflow workflow = XmlConfiguration.ParseflowXml<Workflow>(resourceXml);
            return workflow.StartNode.WebView;
        }


        public string Start(string resourceXml)
        {
            Workflow workflow = XmlConfiguration.ParseflowXml<Workflow>(resourceXml);
            List<Element> elements = new List<Element>();
            elements.Add(workflow.StartNode);
            elements.AddRange(workflow.ChildNode);
            elements.AddRange(workflow.ChildDecisionNode);
            elements.Add(workflow.EndNode);

            string instaceID = CreateWorkflowInstance(workflow.StartNode.ID,resourceXml);
            foreach (Element element in elements)
            {
                element.InstanceID = instaceID;
                element.Persistent();
            }
            return instaceID;
        }




        public void Kill(WorkflowInstance instance)
        {
            if (instance.State == WorkflowInstanceState.Running)
            {
                instance.State = WorkflowInstanceState.Kill;
                instance.Transfer();
            }
        }

        public void Terminate(WorkflowInstance instance)
        {
            if (instance.State == WorkflowInstanceState.Running)
            {
                instance.State = WorkflowInstanceState.Termination;
                instance.Transfer();
            }
        }

        public void Revert(WorkflowInstance instance)
        {
            if (instance.State == WorkflowInstanceState.Termination)
            {
                instance.State = WorkflowInstanceState.Running;
                instance.Transfer();
            }
        }

        protected string CreateWorkflowInstance(string NID, string resource)
        {
            return WorkflowInstance.CreateWorkflowInstance(NID, resource);
        }
    }
}
