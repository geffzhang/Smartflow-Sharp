using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow.BussinessService
{
    public interface IMdl
    {
        long IDENTIFICATION
        {
            get;
            set;
        }

        int STATUS
        {
            get;
            set;
        }


        string BLLSERVICE
        {
            get;
        }
    }
}
