using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow
{
    public class DefaultWordFilter : IFilter
    {
        public string Filter(string cmdText)
        {
            return cmdText;
        }
    }
}
