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
    public class Command : Element, IRelationShip
    {
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        [JsonProperty("script")]
        [XmlElement("script")]
        public string SCRIPT
        {
            get;
            set;
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        [JsonProperty("connecte")]
        [XmlElement("connecte")]
        public string CONNECTE
        {
            get;
            set;
        }

        [JsonProperty("providername")]
        [XmlElement("providername")]
        public string PROVIDERNAME
        {
            get;
            set;
        }

        [JsonProperty("commandtype")]
        [XmlElement("commandtype")]
        public string COMMANDTYPE
        {
            get;
            set;
        }

        [JsonIgnore]
        [XmlIgnore]
        public string RNID
        {
            get;
            set;
        }

        internal override void Persistent()
        {
            string sql = "INSERT INTO T_COMMAND(NID,RNID,APPELLATION,SCRIPT,CONNECTE,INSTANCEID,PROVIDERNAME,COMMANDTYPE) VALUES(@NID,@RNID,@APPELLATION,@SCRIPT,@CONNECTE,@INSTANCEID,@PROVIDERNAME,@COMMANDTYPE)";
            Connection.Execute(sql, new
            {
                NID = Guid.NewGuid().ToString(),
                RNID = RNID,
                APPELLATION = APPELLATION,
                SCRIPT = SCRIPT,
                CONNECTE = CONNECTE,
                INSTANCEID = INSTANCEID,
                PROVIDERNAME = PROVIDERNAME,
                COMMANDTYPE = COMMANDTYPE
            });
        }
    }
}
