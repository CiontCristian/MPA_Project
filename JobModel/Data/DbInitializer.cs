using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobModel.Models;

namespace JobModel.Data
{
    public class DbInitializer
    {
        public static void Initialize(JobContext context)
        {
            context.Database.EnsureCreated();
            if (context.Jobs.Any())
            {
                return;
            }

            var jobs = new Job[]
            {
            new Job{Title="Java Full-Stack Dev",Technologies="Java|Angular",Positions=3},
            };
            foreach (Job job in jobs)
            {
                context.Jobs.Add(job);
            }
            context.SaveChanges();
        }
    }
}
