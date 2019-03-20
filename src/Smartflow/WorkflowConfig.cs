using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Smartflow.Dapper;

namespace Smartflow
{
    public class WorkflowConfig
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// 提供访问者
        /// </summary>
        public string ProviderName
        {
            get;
            set;
        }

        public static WorkflowConfig GetInstance(string id)
        {
            string query = " SELECT * FROM T_CONFIG WHERE ID=@ID ";

            return DapperFactory
                .CreateWorkflowConnection()
                .Query<WorkflowConfig>(query, new { ID = id })
                .FirstOrDefault();
        }

        public static DataTable GetSettings()
        {
            string query = " SELECT * FROM T_CONFIG ";
            DataTable configData = new DataTable(Guid.NewGuid().ToString());
            using (IDataReader dr = DapperFactory.CreateWorkflowConnection().ExecuteReader(query))
            {
                configData.Load(dr);
            }
            return configData;
        }
    }
}
