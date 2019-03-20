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
    public class Form : Element, IRelationship
    {
        [JsonProperty("name")]
        [XmlAttribute("name")]
        public  string Name
        {
            get;
            set;
        }

        [JsonIgnore]
        public string RelationshipID
        {
            get;
            set;
        }

     
        [XmlText(typeof(string))]
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        internal override void Persistent()
        {
            string sql = "INSERT INTO T_FORM(NID,RelationshipID,Name,InstanceID,Text) VALUES(@NID,@RelationshipID,@Name,@InstanceID,@Text)";
            Connection.Execute(sql, new
            {
                NID = Guid.NewGuid().ToString(),
                RelationshipID = RelationshipID,
                Name = Name,
                Text=Text,
                InstanceID = InstanceID
            });
        }
    }
}
