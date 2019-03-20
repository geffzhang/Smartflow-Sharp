using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smartflow.Elements;
using Smartflow.BussinessService.Models;
using Smartflow.BussinessService.Services;
using System.Reflection;

namespace Smartflow.BussinessService.WorkflowService
{
    public partial class BaseWorkflowService
    {
        public List<Group> GetCurrentActorGroup(string instanceID)
        {
            return WorkflowInstance.GetInstance(instanceID).Current.Groups;
        }

        protected List<User> GetUsersByGroup(List<Group> items)
        {
            List<string> gList = new List<string>();
            foreach (Group g in items)
            {
                gList.Add(g.ID.ToString());
            }

            if (gList.Count == 0)
            {
                return new List<User>();
            }

            return new UserService().GetUserList(string.Join(",", gList));
        }

        public void OnCompleted(ExecutingContext executeContext)
        {
            //以下代码仅用于演示
            //流程结束（在完成事件中可以做业务操作）
            string bllServiceClass=executeContext.Data.bllService;
            object service = Activator.CreateInstance(Type.GetType(bllServiceClass));
            MethodInfo[] methods = service.GetType().GetMethods();
            MethodInfo methodGet=methods.FirstOrDefault(m => m.Name == "Get");
            IMdl mdl = methodGet.Invoke(service, new object[] { Convert.ToInt64(executeContext.Data.bussinessID) }) as IMdl;
            mdl.STATUS = 8;
            MethodInfo methodUpdate = methods.FirstOrDefault(m => m.Name == "Update");
            methodUpdate.Invoke(service, new object[] { mdl });
            pendingService.Delete(p => p.INSTANCEID == executeContext.Instance.InstanceID);
        }

        public void OnProcess(ExecutingContext executeContext)
        {
            if (executeContext.Instance.Current.NodeType == Enums.WorkflowNodeCategory.Decision)
            {
                DecisionJump(executeContext);
            }
            else
            {
                //写入审批记录
                WriteRecord(executeContext);
                string instanceID = executeContext.Instance.InstanceID;
                var current = GetCurrentNode(instanceID);
                if (current.Name == "结束")
                {
                    pendingService.Delete(p => p.INSTANCEID == instanceID);
                }
                else
                {
                    if (executeContext.Operation == Enums.WorkflowAction.Rollback)
                    {
                        //流程回退(谁审就退给谁) 仅限演示
                        var item = executeContext.Instance.Current.GetFromNode().GetActors().FirstOrDefault();
                        WritePending(item.ID, executeContext);
                    }
                    else
                    {
                        //流程跳转|流程撤销(重新指派人审批) 仅限演示
                        List<Group> items = (executeContext.Operation == Enums.WorkflowAction.Jump) ? current.Groups :
                            executeContext.Instance.Current.GetFromNode().Groups;
                        List<User> userList = GetUsersByGroup(items);
                        foreach (User user in userList)
                        {
                            WritePending(user.IDENTIFICATION.ToString(), executeContext);
                        }
                    }

                    string NID = executeContext.Instance.Current.NID;
                    pendingService.Delete(pending =>pending.NODEID == NID &&pending.INSTANCEID == instanceID);
                }
            }
        }

        /// <summary>
        /// 多条件跳转
        /// </summary>
        /// <param name="executeContext">执行上下文</param>
        private void DecisionJump(ExecutingContext executeContext)
        {
            string instanceID = executeContext.Instance.InstanceID;
            string NID = executeContext.Instance.Current.NID;
            pendingService.Delete(pending =>
                pending.NODEID == NID &&
                pending.INSTANCEID == instanceID);

            var current = GetCurrentNode(executeContext.Instance.InstanceID);
            if (executeContext.Operation == Enums.WorkflowAction.Jump && current.NodeType != Enums.WorkflowNodeCategory.Decision)
            {
                List<User> userList = GetUsersByGroup(current.Groups);
                foreach (var user in userList)
                {
                    WritePending(user.IDENTIFICATION.ToString(), executeContext);
                }

                pendingService.Delete(pending =>
                    pending.NODEID == NID &&
                    pending.INSTANCEID == instanceID);
            }
        }

        /// <summary>
        /// 写入审批记录
        /// </summary>
        /// <param name="executeContext"></param>
        public void WriteRecord(ExecutingContext executeContext)
        {
            //写入审批记录
            recordService.Insert(new Record()
            {
                INSTANCEID = executeContext.Instance.InstanceID,
                NODENAME = executeContext.From.Name,
                MESSAGE = executeContext.Data.Message
            });
        }

        /// <summary>
        /// 写待办信息
        /// </summary>
        /// <param name="actorID">参与者</param>
        /// <param name="executeContext"></param>
        public void WritePending(string actorID, ExecutingContext executeContext)
        {
            pendingService.Insert(new Pending()
            {
                ACTORID = actorID,
                ACTION = executeContext.Operation.ToString(),
                INSTANCEID = executeContext.Instance.InstanceID,
                NODEID = GetCurrentNode(executeContext.Instance.InstanceID).NID,
                APPELLATION = string.Format("<a href=\"javascript:;\" onclick=\"parent.window.document.getElementById('frmContent').src='../FileApply/FileApply/{0}'\">你有待办任务。</a>", executeContext.Data.bussinessID)
            });
        }
    }
}
