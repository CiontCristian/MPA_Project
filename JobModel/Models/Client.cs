using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModel.Models
{
    public class Client
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Experience { get; set; }
        public DateTime Birthday { get; set; }
        public ICollection<Application> Applications { get; set; }
    }
}
