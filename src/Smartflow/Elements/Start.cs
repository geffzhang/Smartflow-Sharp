﻿/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Smartflow.Dapper;
using Smartflow.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Smartflow.Elements
{
    [XmlInclude(typeof(List<Transition>))]
    public class Start : Node
    {
        [JsonProperty("name")]
        [XmlIgnore]
        public override string Name
        {
            get { return "开始"; }
        }

        [JsonProperty("category", ItemConverterType = typeof(StringEnumConverter))]
        public override WorkflowNodeCategory NodeType
        {
            get
            {
                return WorkflowNodeCategory.Start;
            }
        }
    }
}
