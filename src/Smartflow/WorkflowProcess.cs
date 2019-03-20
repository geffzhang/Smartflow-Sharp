/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smartflow.Dapper;
using Smartflow.Enums;
using System.Data;

namespace Smartflow
{
    public class WorkflowProcess :WorkflowInfrastructure,IPersistent, IRelationship
    {
        /// <summary>
        /// 外键
        /// </summary>
        public string RelationshipID
        {
            get;
            set;
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string NID
        {
            get;
            set;
        }

        /// <summary>
        /// 当前节点
        /// </summary>
        public string Origin
        {
            get;
            set;
        }

        /// <summary>
        /// 跳转到的节点
        /// </summary>
        public string Destination
        {
            get;
            set;
        }

        /// <summary>
        /// 路线ID
        /// </summary>
        public string TransitionID
        {
            get;
            set;
        }

        /// <summary>
        /// 实例ID
        /// </summary>
        public string InstanceID
        {
            get;
            set;
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        public WorkflowNodeCategory NodeType
        {
            get;
            set;
        }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 退回、撤销、跳转
        /// </summary>
        public WorkflowAction Operation
        {
            get;
            set;
        }

        /// <summary>
        /// 将数据持久到数据库
        /// </summary>
        public void Persistent()
        {
            string sql = "INSERT INTO T_PROCESS(NID,Origin,Destination,TransitionID,InstanceID,NodeType,RelationshipID,Operation) VALUES(@NID,@Origin,@Destination,@TransitionID,@InstanceID,@NodeType,@RelationshipID,@Operation)";
            Connection.Execute(sql, new
            {
                NID = Guid.NewGuid().ToString(),
                Origin = Origin,
                Destination = Destination,
                TransitionID = TransitionID,
                InstanceID = InstanceID,
                NodeType = NodeType.ToString(),
                RelationshipID = RelationshipID,
                Operation = Operation
            });
        }

        public static WorkflowProcess GetWorkflowProcessInstance(string instanceID, string NID)
        {
            WorkflowProcess instance = new WorkflowProcess();
            string query = ResourceManage.GetString(ResourceManage.SQL_WORKFLOW_PROCESS);
            instance = instance.Connection.Query<WorkflowProcess>(query, new
            {
                InstanceID = instanceID,
                NID = NID,
                Operation = WorkflowAction.Jump

            }).OrderByDescending(order => order.CreateDateTime).FirstOrDefault();

            return instance;
        }
    }
}
