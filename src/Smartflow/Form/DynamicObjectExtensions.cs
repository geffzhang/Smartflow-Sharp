using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Smartflow.Form
{
    public static class DynamicObjectExtensions
    {
        public static bool Constant(this Object data, string propetyName)
        {
            return data.GetType().GetProperty(propetyName) != null;
        }

        public static object Value(this Object data, string propetyName)
        {
            if (data.Constant(propetyName))
            {
                return data.GetType().GetProperty(propetyName).GetValue(data, null);
            }
            return null;
        }
        
        public static void SetValue(this Object data, string propetyName, object value)
        {
            if (data.Constant(propetyName))
            {
               data.GetType().GetProperty(propetyName).SetValue(data, value, null);
            }
        }
    }
}
