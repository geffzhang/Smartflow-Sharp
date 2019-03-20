/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Smartflow.Dapper;
using Smartflow.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Smartflow.Elements
{
    [XmlInclude(typeof(List<Transition>))]
    [XmlInclude(typeof(List<Group>))]
    [XmlInclude(typeof(Form))]
    public class Node : ASTNode
    {
        private WorkflowNodeCategory _nodeType = WorkflowNodeCategory.Normal;

        [JsonProperty("category", ItemConverterType = typeof(EnumConverter))]
        public override WorkflowNodeCategory NodeType
        {
            get { return _nodeType; }
            set { _nodeType = value; }
        }

        [JsonProperty("layout")]
        [XmlAttribute("layout")]
        public virtual string Layout
        {
            get;
            set;
        }

        [JsonProperty("group")]
        [XmlElement(ElementName = "group")]
        public virtual List<Group> Groups
        {
            get;
            set;
        }


        [JsonProperty("form")]
        [XmlElement(ElementName = "form")]
        public virtual Form WebView
        {
            get;
            set;
        }

        internal override void Persistent()
        {
            base.Persistent();

            if (Transitions != null)
            {
                foreach (Transition transition in Transitions)
                {
                    transition.RelationshipID = this.NID;
                    transition.Origin = this.ID;
                    transition.InstanceID = InstanceID;
                    transition.Persistent();
                }
            }

            if (Groups != null)
            {
                foreach (Group r in Groups)
                {
                    r.RelationshipID = this.NID;
                    r.InstanceID = InstanceID;
                    r.Persistent();
                }
            }

            if (WebView != null)
            {
                WebView.RelationshipID = this.NID;
                WebView.InstanceID = InstanceID;
                WebView.Persistent();
            }
        }

        public ASTNode GetNode(string ID)
        {
            string query = "SELECT * FROM T_NODE WHERE ID=@ID AND InstanceID=@InstanceID";
            return Connection.Query<ASTNode>(query, new
            {
                ID = ID,
                InstanceID = InstanceID
            }).FirstOrDefault();
        }
    }
}
