using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModel.Models
{
    public class CompanyOffer
    {
        public int CompanyID { get; set; }
        public int JobID { get; set; }
        public Company Company { get; set; }
        public Job Job { get; set; }
    }
}
