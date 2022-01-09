using JobRecruitment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JobModel.Data;
using JobModel.Models.ViewData;

namespace JobRecruitment.Controllers
{
    public class HomeController : Controller
    {
        private readonly JobContext _context;
        public HomeController(JobContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            IQueryable<SubscribedJob> data =
            from job in _context.Jobs
            join application in _context.Applications on job.ID equals application.JobID
            group job by job.Title into jobApplication
            select new SubscribedJob()
            {
                JobTitle = jobApplication.Key,
                NrOfApplications = jobApplication.Count()
            };

            return View(await data.AsNoTracking().ToListAsync());
        }

        public IActionResult Chat()
        {
            return View();
        }

        public async Task<ActionResult> Statistics()
        {
            IQueryable<SubscribedJob> data =
            from job in _context.Jobs
            join application in _context.Applications on job.ID equals application.JobID
            group job by job.Title into jobApplication
            select new SubscribedJob()
            {
                JobTitle = jobApplication.Key,
                NrOfApplications = jobApplication.Count()
            };

            return View(await data.AsNoTracking().ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
