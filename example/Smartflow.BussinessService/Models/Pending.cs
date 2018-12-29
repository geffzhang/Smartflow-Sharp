using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow.BussinessService.Models
{
    [Table("T_PENDING")]
    public class Pending
    {
        [Key]
        public long IDENTIFICATION
        {
            get;
            set;
        }

        public string ACTORID
        {
            get;
            set;
        }
        
        public string NODEID
        {
            get;
            set;
        }

        public string INSTANCEID
        {
            get;
            set;
        }

        public string APPELLATION
        {
            get;
            set;
        }

        public string ACTION
        {
            get;
            set;
        }
    }
}
