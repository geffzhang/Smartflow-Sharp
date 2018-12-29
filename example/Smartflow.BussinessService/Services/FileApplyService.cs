using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using Smartflow.BussinessService.Models;

namespace Smartflow.BussinessService.Services
{
    public class FileApplyService : RepositoryService<FileApply>
    {
        public void Persistent(FileApply model)
        {
            if (model.IDENTIFICATION == 0)
            {
                Insert(model);
            }
            else
            {
                Update(model);
            }
        }
    }
}