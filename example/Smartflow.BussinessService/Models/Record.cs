using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartflow.BussinessService.Models
{
    [Table("T_RECORD")]
    public class Record
    {
        [Key]
        public long IDENTIFICATION
        {
            get;
            set;
        }

        public string NODENAME
        {
            get;
            set;
        }

        public string MESSAGE
        {
            get;
            set;
        }

        public string INSTANCEID
        {
            get;
            set;
        }
    }
}