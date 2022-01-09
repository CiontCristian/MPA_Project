using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobModel.Data;
using JobModel.Models;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;


namespace JobRecruitment.Controllers
{
    [Authorize(Roles = "Employee")]
    public class JobsController : Controller
    {
        private readonly JobContext _context;
        private string _baseUrl = "http://localhost:56757/api/Jobs";

        public JobsController(JobContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(_baseUrl);

            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PositionsSortParm"] = sortOrder == "Positions" ? "positions_desc" : "Positions";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            if (response.IsSuccessStatusCode)
            {
                //var jobs = JsonConvert.DeserializeObject<List<Job>>(await response.Content.ReadAsStringAsync()).AsQueryable();
                var jobs = from job in _context.Jobs select job;

                if (!String.IsNullOrEmpty(searchString))
                {
                    jobs = jobs.Where(s => s.Title.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "title_desc":
                        jobs = jobs.OrderByDescending(job => job.Title);
                        break;
                    case "Positions":
                        jobs = jobs.OrderBy(job => job.Positions);
                        break;
                    case "positions_desc":
                        jobs = jobs.OrderByDescending(job => job.Positions);
                        break;
                    default:
                        jobs = jobs.OrderBy(job => job.Title);
                        break;
                }
                int pageSize = 2;
                return View(await PaginatedList<Job>.CreateAsync(jobs.AsNoTracking(), pageNumber ?? 1, pageSize));
            }
            return NotFound();

        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");

            if (response.IsSuccessStatusCode)
            {
                var job = JsonConvert.DeserializeObject<Job>(await response.Content.ReadAsStringAsync());
                var applicationsForJob = await _context.Applications
                    .Where(app => app.JobID == id)
                    .Include(app => app.Client)
                    .AsNoTracking()
                    .ToListAsync();
                job.Applications = applicationsForJob;

                return View(job);
            }
            return NotFound();
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jobs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Technologies,Positions")] Job job)
        {
            if (!ModelState.IsValid) return View(job);
            try
            {
                var client = new HttpClient();
                string jobJson = JsonConvert.SerializeObject(job);
                var response = await client.PostAsync(_baseUrl, new StringContent(jobJson, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to create record: {ex.Message}");
            }
            return View(job);
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");

            if (response.IsSuccessStatusCode)
            {
                var job = JsonConvert.DeserializeObject<Job>(await response.Content.ReadAsStringAsync());
                return View(job);
            }
            return new NotFoundResult();
        }

        // POST: Jobs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Technologies,Positions")] Job job)
        {
            if (!ModelState.IsValid) return View(job);

            var client = new HttpClient();
            string jobJson = JsonConvert.SerializeObject(job);
            var response = await client.PutAsync($"{_baseUrl}/{job.ID}", new StringContent(jobJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");

            if (response.IsSuccessStatusCode)
            {
                var job = JsonConvert.DeserializeObject<Job>(await response.Content.ReadAsStringAsync());
                return View(job);
            }
            return new NotFoundResult();
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("ID")] Job job)
        {
            try
            {
                var client = new HttpClient();
                HttpRequestMessage request =
                        new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}/{job.ID}")
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(job), Encoding.UTF8, "application/json")
                        };
                var response = await client.SendAsync(request);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete record: {ex.Message}");
            }
            return View(job);
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.ID == id);
        }
    }
}
