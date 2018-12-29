using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow
{
    internal class EnumConverter : StringEnumConverter
    {
        public EnumConverter()
        {
            base.CamelCaseText = true;
        }
    }
}
