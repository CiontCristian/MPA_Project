using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModel.Models
{
    public class Company
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Employees { get; set; }

        public ICollection<CompanyOffer> CompanyOffers { get; set; }
    }
}
