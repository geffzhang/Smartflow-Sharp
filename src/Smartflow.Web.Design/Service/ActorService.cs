/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Smartflow.Dapper;
using Smartflow.Enums;
using System.Configuration;

namespace Smartflow.Web.Design
{
    public class ActorService
    {
        private static readonly ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["busConnection"];

        private IDbConnection Connection = DapperFactory.CreateConnection(connectionStringSettings.ProviderName, connectionStringSettings.ConnectionString);

        public DataTable GetRole(string roleIds, string searchKey)
        {
            string query = " SELECT * FROM T_ROLE WHERE 1=1 ";
            if (!String.IsNullOrEmpty(roleIds))
            {
                query = string.Format("{0} AND IDENTIFICATION NOT IN ({1})", query, BindQueryConditionQuot(roleIds));
            }
            if (!String.IsNullOrEmpty(searchKey))
            {
                query = string.Format("{0} AND APPELLATION LIKE '%{1}%'", query, searchKey);
            }
            DataTable roleData = new DataTable(Guid.NewGuid().ToString());
            using (IDataReader dr = Connection.ExecuteReader(query))
            {
                roleData.Load(dr);
            }
            return roleData;
        }

        /// <summary>
        /// 处理依据roleID查询少引号的情况
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public string BindQueryConditionQuot(string roleIds)
        {
            string[] RArry = roleIds.Split(',');
            string[] NRArray = new string[RArry.Length];
            for (int i = 0; i < RArry.Length; i++)
            {
                NRArray[i] = string.Format("'{0}'", RArry[i]);
            }
            return string.Join(",", NRArray);
        }
    }
}
