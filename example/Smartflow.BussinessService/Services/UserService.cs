using Smartflow.BussinessService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace Smartflow.BussinessService.Services
{
    public class UserService : RepositoryService<User>
    {
        public List<User> GetUserList(string roleIDs)
        {
            string executeSql = @"SELECT * FROM T_USER WHERE IDENTIFICATION IN (SELECT UUID FROM T_UMR  WHERE RID IN (" + roleIDs + "))";
            return Connection.Query<User>(executeSql).ToList();
        }

        public List<User> GetPendingUserList(string nodeID, string instanceID)
        {
            string executeSql = @" SELECT * FROM T_USER WHERE IDENTIFICATION IN (SELECT ACTORID FROM T_PENDING WHERE NODEID=@NODEID AND InstanceID=@InstanceID) ";
            return Connection.Query<User>(executeSql, new
            {
                NODEID = nodeID,
                INSTANCEID = instanceID

            }).ToList();
        }
    }
}
