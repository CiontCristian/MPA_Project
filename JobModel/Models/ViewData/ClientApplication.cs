using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModel.Models.ViewData
{
    public class ClientApplication
    {
        public Client Client { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
       
    }
}
