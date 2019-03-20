using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow
{
    public interface IFilter
    {
        string Filter(string cmdText);
    }
}
