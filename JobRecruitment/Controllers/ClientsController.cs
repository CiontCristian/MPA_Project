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
using Microsoft.AspNetCore.Authorization;

namespace JobRecruitment.Controllers
{
    [Authorize(Roles = "HumanResources")]
    public class ClientsController : Controller
    {
        private readonly JobContext _context;

        public ClientsController(JobContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            return View(await _context.Clients.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(client => client.Applications)
                .ThenInclude(app => app.Job)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Experience,Birthday")] Client client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(client);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex*/)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                "Try again!");
            }
           
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = new ClientApplication();

            viewModel.Client = await _context.Clients
                .Include(client => client.Applications)
                .AsNoTracking()
                .FirstOrDefaultAsync(client => client.ID == id);

            if (viewModel.Client == null)
            {
                return NotFound();
            }

            var appliedJobs = from job in _context.Jobs
                       join application in _context.Applications on job.ID equals application.JobID
                       join client in _context.Clients on application.ClientID equals id.Value                 
                       select job;

            var availableJobs = _context.Jobs
                .Where(job => !appliedJobs.Contains(job))
                .AsNoTracking()
                .ToListAsync().Result;

            viewModel.Jobs = availableJobs;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Apply(int? clientID, int? jobID)
        {
            if (clientID == null || jobID == null)
            {
                return NotFound();
            }
 
            try
            {
                Application application = new Application { ClientID = clientID.Value, JobID = jobID.Value, ApplyDate = DateTime.Now };
                _context.Add(application);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }


        // POST: Clients/Edit/5  
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClient(int? id, [Bind("ID,Name,Experience,Birthday")] Client client)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                _context.Update(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                "Try again!");
            }
            
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ID == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ID == id);
        }
    }
}
