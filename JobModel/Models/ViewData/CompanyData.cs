using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModel.Models.ViewData
{
    public class CompanyData
    {
        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
        public IEnumerable<Application> Applications { get; set; }
       
    }
}
