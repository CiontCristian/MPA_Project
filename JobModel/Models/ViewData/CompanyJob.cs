using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModel.Models.ViewData
{
    public class CompanyJob
    {
        public int JobID { get; set; }
        public string Title { get; set; }
        public bool IsPartOfCompany { get; set; }
    }
}
