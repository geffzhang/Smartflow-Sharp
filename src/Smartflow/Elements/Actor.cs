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
using Smartflow.Enums;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Smartflow.Elements
{
    public class Actor : ElementAttribute, IRelationship
    {
        public string RelationshipID
        {
            get;
            set;
        }

        public WorkflowAction Operation
        {
            get;
            set;
        }

        internal override void Persistent()
        {
            string sql = "INSERT INTO T_ACTOR(NID,ID,RelationshipID,Name,InstanceID,Operation) VALUES(@NID,@ID,@RelationshipID,@Name,@InstanceID,@Operation)";
            DapperFactory.CreateWorkflowConnection().Execute(sql, new
            {
                NID = Guid.NewGuid().ToString(),
                ID = ID,
                RelationshipID = RelationshipID,
                Name = Name,
                InstanceID = InstanceID,
                Operation = Operation
            });
        }
    }
}
