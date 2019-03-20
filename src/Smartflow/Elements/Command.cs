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
using System.Xml.Serialization;

using Smartflow.Dapper;
using Smartflow.Enums;
using Newtonsoft.Json;

namespace Smartflow.Elements
{
    public class Command : Element, IRelationship
    {

        /// <summary>
        /// 数据源ID
        /// </summary>
        [JsonProperty("id")]
        [XmlElement("id")]
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        [JsonProperty("text")]
        [XmlElement("text")]
        public string Text
        {
            get;
            set;
        }


        [JsonIgnore]
        [XmlIgnore]
        public string RelationshipID
        {
            get;
            set;
        }

        internal override void Persistent()
        {
            string sql = "INSERT INTO T_Command(NID,ID,RelationshipID,Text,InstanceID) VALUES(@NID,@ID,@RelationshipID,@Text,@InstanceID)";
            Connection.Execute(sql, new
            {
                NID = Guid.NewGuid().ToString(),
                ID=ID,
                RelationshipID = RelationshipID,
                Text = Text,
                InstanceID = InstanceID
            });
        }
    }
}
