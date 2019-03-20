using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow.BussinessService
{
    [Serializable]
    public class FieldInfo
    {
        public string COLUMN_NAME
        {
            get;
            set;
        }

        public string DATA_TYPE
        {
            get;
            set;
        }
    }
}
