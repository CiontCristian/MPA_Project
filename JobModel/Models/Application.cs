using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModel.Models
{
    public class Application
    {
        public int JobID { get; set; }
        public int ClientID { get; set; }
        public Job Job { get; set; }
        public Client Client { get; set; }

        public DateTime ApplyDate { get; set; }
    }
}
