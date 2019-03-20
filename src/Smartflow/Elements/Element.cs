﻿/********************************************************************
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

using Smartflow.Enums;
using Newtonsoft.Json;
namespace Smartflow.Elements
{
    /// <summary>
    /// 为兼容其他数据库，对ID、NAME 名称进行调整，与数据库保留关键字进行区分
    /// </summary>
    [Serializable]
    public abstract class Element : WorkflowInfrastructure
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public string NID
        {
            get;
            set;
        }


        [JsonIgnore]
        [XmlIgnore]
        public virtual string InstanceID
        {
            get;
            set;
        }

        internal abstract void Persistent();
    }
}
