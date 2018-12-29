using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartflow.BussinessService.Models
{
    [Table("T_APPLY")]
    public class FileApply : IMdl
    {
        [Key]
        public long IDENTIFICATION
        {
            get;
            set;
        }

        public int STATUS
        {
            get;
            set;
        }

        public string FNAME
        {
            get;
            set;
        }

        public string DESCRIPTION
        {
            get;
            set;
        }

        public string STRUCTUREID
        {
            get;
            set;
        }

        public string INSTANCEID
        {
            get;
            set;
        }

        public string SECRETGRADE
        {
            get;
            set;
        }

        [Computed]
        public string BLLSERVICE
        {
            get
            {
                return "Smartflow.BussinessService.Services.FileApplyService,Smartflow.BussinessService";
            }
        }
    }
}