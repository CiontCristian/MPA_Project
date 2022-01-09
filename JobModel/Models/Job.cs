using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModel.Models
{
    public class Job
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Technologies { get; set; }
        public int Positions { get; set; }
        public ICollection<Application> Applications { get; set; }

        public ICollection<CompanyOffer> CompanyOffers { get; set; }
    }
}
