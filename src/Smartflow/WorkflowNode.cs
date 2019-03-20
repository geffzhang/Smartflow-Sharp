/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Smartflow.Dapper;
using Smartflow.Elements;
using Smartflow.Enums;

namespace Smartflow
{
    public class WorkflowNode : Node
    {
        protected WorkflowNode()
        {

        }

        public List<Transition> GetTransitions()
        {
            foreach (Smartflow.Elements.Transition transition in this.Transitions)
            {
                ASTNode an = this.GetNode(transition.Destination);
                Transition decisionTransition = transition;
                while (an.NodeType == Enums.WorkflowNodeCategory.Decision)
                {
                    WorkflowDecision decision = WorkflowDecision.ConvertToReallyType(an);
                    decisionTransition = decision.GetTransition();
                    an = this.GetNode(decisionTransition.Destination);
                }
                transition.Name = decisionTransition.Name;
            }
            return this.Transitions;
        }

        /// <summary>
        /// 上一个执行跳转路线
        /// </summary>
        public Transition FromTransition
        {
            get;
            set;
        }

        #region 节点方法

        public static WorkflowNode ConvertToReallyType(ASTNode node)
        {
            WorkflowNode wfNode = new WorkflowNode();
            wfNode.NID = node.NID;
            wfNode.ID = node.ID;
            wfNode.Name = node.Name;
            wfNode.NodeType = node.NodeType;
            wfNode.InstanceID = node.InstanceID;
            wfNode.Transitions = wfNode.QueryWorkflowNode(node.NID);
            wfNode.FromTransition = wfNode.GetHistoryTransition();
            wfNode.Groups = wfNode.GetGroup();
            wfNode.WebView = wfNode.GetWebView();

            return wfNode;
        }

        /// <summary>
        /// 上一个执行跳转节点
        /// </summary>
        /// <returns></returns>
        public WorkflowNode GetFromNode()
        {
            if (FromTransition == null) return null;
            ASTNode node = GetNode(FromTransition.Origin);
            return WorkflowNode.ConvertToReallyType(node);
        }

        public List<Actor> GetActors()
        {
            string query = " SELECT * FROM T_ACTOR WHERE RelationshipID=@RelationshipID AND InstanceID=@InstanceID AND Operation=@Operation ";
            return Connection.Query<Actor>(query, new
            {
                RelationshipID = NID,
                InstanceID = InstanceID,
                Operation = WorkflowAction.Jump

            }).ToList();
        }

        /// <summary>
        /// 获取回退线路
        /// </summary>
        /// <returns>路线</returns>
        protected Transition GetHistoryTransition()
        {
            Transition transition = null;
            try
            {
                WorkflowProcess process = WorkflowProcess.GetWorkflowProcessInstance(InstanceID, NID);
                if (process != null && NodeType != WorkflowNodeCategory.Start)
                {
                    ASTNode n = GetNode(process.Origin);
                    while (n.NodeType == WorkflowNodeCategory.Decision)
                    {
                        process = WorkflowProcess.GetWorkflowProcessInstance(InstanceID, n.NID);
                        n = GetNode(process.Origin);

                        if (n.NodeType == WorkflowNodeCategory.Start)
                            break;
                    }
                    transition = GetTransition(process.TransitionID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return transition;
        }

        /// <summary>
        /// 依据主键获取路线
        /// </summary>
        /// <param name="TRANSITIONID">路线主键</param>
        /// <returns>路线</returns>
        protected Transition GetTransition(string transitionID)
        {
            string query = "SELECT * FROM T_TRANSITION WHERE NID=@TransitionID AND InstanceID=@InstanceID";
            Transition transition = Connection.Query<Transition>(query, new
            {
                TransitionID = transitionID,
                InstanceID = InstanceID

            }).FirstOrDefault();

            return transition;
        }

        protected List<Group> GetGroup()
        {
            string query = "SELECT * FROM T_GROUP WHERE RelationshipID=@RelationshipID AND InstanceID=@InstanceID";
            return Connection.Query<Group>(query, new
            {
                RelationshipID = NID,
                InstanceID = InstanceID
            }).ToList();
        }

        protected Smartflow.Elements.Form GetWebView()
        {
            string query = "SELECT * FROM T_FORM WHERE RelationshipID=@RelationshipID AND InstanceID=@InstanceID";
            return Connection.Query<Smartflow.Elements.Form>(query, new
            {
                RelationshipID = NID,
                InstanceID = InstanceID

            }).FirstOrDefault();
        }


        /// <summary>
        /// 获取当前执行的跳转路线
        /// </summary>
        /// <param name="transitionID">跳转ID</param>
        /// <returns>跳转对象</returns>
        protected Transition GetExecuteTransition(string transitionID)
        {
            Transition executeTransition = Transitions
                .FirstOrDefault(t => t.NID == transitionID);

            ASTNode an = this.GetNode(executeTransition.Destination);
            Transition returnTransition = executeTransition;
            while (an.NodeType == Enums.WorkflowNodeCategory.Decision)
            {
                WorkflowDecision decision = WorkflowDecision.ConvertToReallyType(an);
                returnTransition = decision.GetTransition();
                an = this.GetNode(returnTransition.Destination);
            }
            return returnTransition;
        }

        /// <summary>
        /// 获取下一组
        /// </summary>
        /// <param name="transitionID">跳转ID</param>
        /// <returns></returns>
        public List<Group> GetNextGroup(string transitionID)
        {
            Transition executeTransition = this.GetExecuteTransition(transitionID);
            ASTNode selectNode = this.GetNode(executeTransition.Destination);

            string query = "SELECT * FROM T_GROUP WHERE RelationshipID=@RelationshipID AND InstanceID=@InstanceID";
            return Connection.Query<Group>(query, new
            {
                RelationshipID = selectNode.NID,
                InstanceID = InstanceID

            }).ToList();
        }
        #endregion

        /// <summary>
        /// 获取节点的审批记录
        /// </summary>
        /// <param name="instanceID">流程实例ID</param>
        /// <returns></returns>
        public static DataTable GetRecord(string instanceID)
        {
            string sql = ResourceManage.GetString(ResourceManage.SQL_ACTOR_RECORD);
            using (IDataReader dr = DapperFactory.CreateWorkflowConnection().ExecuteReader(sql,
                new { InstanceID = instanceID }))
            {
                DataTable dt = new DataTable(Guid.NewGuid().ToString());
                dt.Load(dr);
                return dt;
            }
        }
    }
}
