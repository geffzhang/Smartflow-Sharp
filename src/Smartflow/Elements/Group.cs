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
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Smartflow.Elements
{
    public class Group : ElementAttribute, IRelationship
    {
        [JsonIgnore]
        public string RelationshipID
        {
            get;
            set;
        }

        internal override void Persistent()
        {
            string sql = "INSERT INTO T_GROUP(NID,ID,RelationshipID,Name,InstanceID) VALUES(@NID,@ID,@RelationshipID,@Name,@InstanceID)";
            Connection.Execute(sql, new
            {
                NID = Guid.NewGuid().ToString(),
                ID = ID,
                RelationshipID = RelationshipID,
                Name = Name,
                InstanceID = InstanceID
            });
        }
    }
}
