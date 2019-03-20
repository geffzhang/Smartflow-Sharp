/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smartflow.Elements;
using Smartflow.Dapper;
using System.Data;
using Smartflow.Enums;

namespace Smartflow
{
    public class WorkflowInstance : WorkflowInfrastructure
    {
        protected WorkflowInstance()
        {
        }

        public WorkflowInstanceState State
        {
            get;
            set;
        }

        public WorkflowNode Current
        {
            get;
            set;
        }

        public string InstanceID
        {
            get;
            set;
        }


        public string Resource
        {
            get;
            set;
        }

        /// <summary>
        /// 获取流程实例
        /// </summary>
        /// <param name="instanceID">实例ID</param>
        /// <returns>流程实例</returns>
        public static WorkflowInstance GetInstance(string instanceID)
        {
            WorkflowInstance workflowInstance = new WorkflowInstance();
            workflowInstance.InstanceID = instanceID;
            string sql = ResourceManage.GetString(ResourceManage.SQL_WORKFLOW_INSTANCE);
            try
            {
                workflowInstance = workflowInstance.Connection.Query<WorkflowInstance, ASTNode, WorkflowInstance>(sql, (instance, node) =>
                {
                    instance.Current = WorkflowNode.ConvertToReallyType(node);
                    return instance;

                }, param: new { INSTANCEID = instanceID }, splitOn: "Name").FirstOrDefault<WorkflowInstance>();

                return workflowInstance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 进行流程跳转
        /// </summary>
        /// <param name="transitionTo">选择跳转路线</param>
        internal void Jump(string transitionTo)
        {
            string update = " UPDATE T_INSTANCE SET RelationshipID=@RelationshipID WHERE InstanceID=@InstanceID ";
            Connection.Execute(update, new
            {
                RelationshipID = transitionTo,
                InstanceID = InstanceID
            });
        }

        /// <summary>
        /// 状态转换
        /// </summary>
        internal void Transfer()
        {
            string update = " UPDATE T_INSTANCE SET State=@State WHERE InstanceID=@InstanceID ";
            Connection.Execute(update, new
            {
                State = State.ToString(),
                InstanceID = InstanceID
            });
        }

        internal static string CreateWorkflowInstance(string nodeID, string resource)
        {
            WorkflowInstance instance = new WorkflowInstance();
            string instanceID = Guid.NewGuid().ToString();
            string sql = "INSERT INTO T_INSTANCE(InstanceID,RelationshipID,State,Resource) VALUES(@InstanceID,@RelationshipID,@State,@Resource)";

            instance.Connection.Execute(sql, new
            {
                InstanceID = instanceID,
                RelationshipID = nodeID,
                State = WorkflowInstanceState.Running.ToString(),
                Resource = resource
            });
            return instanceID;
        }
    }
}
