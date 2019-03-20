﻿/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Smartflow.Enums
{
    /// <summary>
    /// 工作流节点类型
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    public enum WorkflowNodeCategory
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        Start,

        /// <summary>
        /// 普通节点
        /// </summary>
        Normal,

        /// <summary>
        /// 决策节点
        /// </summary>
        Decision,

        /// <summary>
        /// 结束节点
        /// </summary>
        End,

        /// <summary>
        /// 跳转节点
        /// </summary>
        Transition
    }
}
