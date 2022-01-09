using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobModel.Data;
using JobModel.Models;
using JobModel.Models.ViewData;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace JobRecruitment.Controllers
{
    [Authorize(Policy = "Recruitment")]
    public class CompaniesController : Controller
    {
        private readonly JobContext _context;

        public CompaniesController(JobContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index(int? id, int? jobID)
        {
            var viewModel = new CompanyData();

            var companies = await _context.Companies
                .Include(comp => comp.CompanyOffers)
                .ThenInclude(offer => offer.Job)
                .ThenInclude(job => job.Applications)
                .ThenInclude(app => app.Client)
                .AsNoTracking()
                .ToListAsync();

            viewModel.Companies = companies;

            if (id != null)
            {
                ViewData["CompanyID"] = id.Value;
                Company company = companies.Where(comp => comp.ID == id).Single();
                viewModel.Jobs = company.CompanyOffers.Select(offer => offer.Job);
            }

            if (jobID != null)
            {
                ViewData["JobID"] = jobID.Value;
                Job job = viewModel.Jobs.Where(job => job.ID == jobID).Single();
                viewModel.Applications = job.Applications;
            }
            return View(viewModel);
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(comp => comp.CompanyOffers)
                .ThenInclude(offer => offer.Job)
                .FirstOrDefaultAsync(comp => comp.ID == id);

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Employees")] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(company);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex*/)
            {
                ModelState.AddModelError("", "Unable to save changes." +
                "Try again!");
            }
            
            return View(company);
        }

        private void PopulateCompanyJob(Company company)
        {
            var allJobs = _context.Jobs;
            var companyJobs = new HashSet<int>(company.CompanyOffers.Select(offer => offer.JobID));
            var viewModel = new List<CompanyJob>();

            foreach (var job in allJobs)
            {
                viewModel.Add(new CompanyJob
                {
                    JobID = job.ID,
                    Title = job.Title,
                    IsPartOfCompany = companyJobs.Contains(job.ID)
                });
            }
            ViewData["Jobs"] = viewModel;
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
               .Include(comp => comp.CompanyOffers)
               .ThenInclude(offer => offer.Job)
               .FirstOrDefaultAsync(comp => comp.ID == id);

            if (company == null)
            {
                return NotFound();
            }

            PopulateCompanyJob(company);

            return View(company);
        }

        private void UpdateCompanyJobs(string[] selectedJobs, Company company)
        {
            if (selectedJobs == null)
            {
                company.CompanyOffers = new List<CompanyOffer>();
                return;
            }
            var selectedJobssHS = new HashSet<string>(selectedJobs);
            var ownedJobs = new HashSet<int>(company.CompanyOffers.Select(offer => offer.JobID));

            foreach (var job in _context.Jobs)
            {
                if (selectedJobssHS.Contains(job.ID.ToString()))
                {
                    if (!ownedJobs.Contains(job.ID))
                    {
                        company.CompanyOffers.Add(new CompanyOffer { CompanyID = company.ID, JobID = job.ID });
                       
                    }
                }
                else
                {
                    if (ownedJobs.Contains(job.ID))
                    {
                        CompanyOffer offerToRemove = company.CompanyOffers.FirstOrDefault(offer => offer.JobID == job.ID);
                        _context.Remove(offerToRemove);

                    }
                }
            }
        }

        // POST: Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedJobs)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
               .Include(comp => comp.CompanyOffers)
               .ThenInclude(offer => offer.Job)
               .FirstOrDefaultAsync(comp => comp.ID == id);


            if (await TryUpdateModelAsync<Company>(company, "", i => i.Name, i => i.Employees))
            {

                UpdateCompanyJobs(selectedJobs, company);
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, ");
                }
                return RedirectToAction(nameof(Index));
            }
            UpdateCompanyJobs(selectedJobs, company);
            PopulateCompanyJob(company);
            return View(company);
        }

       

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.ID == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.ID == id);
        }
    }
}
