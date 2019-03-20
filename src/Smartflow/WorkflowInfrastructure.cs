/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Data;
using System.Linq;

namespace Smartflow
{
    /// <summary>
    /// 定义工作流基础服务
    /// </summary>
    public class WorkflowInfrastructure
    {
        /// <summary>
        /// 访问数据库服务
        /// </summary>
        protected IDbConnection Connection
        {
            get { return DapperFactory.CreateWorkflowConnection(); }
        }
    }
}
