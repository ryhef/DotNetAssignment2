using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Models.ViewModels;

namespace Assignment2.Controllers
{
    public class BrokeragesController : Controller
    {
        private readonly MarketDbContext _context;

        public BrokeragesController(MarketDbContext context)
        {
            _context = context;
        }

        // GET: Brokerages
        public async Task<IActionResult> Index(string? id)
        {
            var viewModel = new BrokerageViewModel
            {
                Brokerages = await _context.Brokerages
                .Include(i => i.Subscriptions)
                .AsNoTracking()
                .OrderBy(i => i.Title)
                .ToListAsync()
            };

            if (id != null)
            {
                ViewData["BrokerageID"] = id;
                var x = _context.Subscriptions.Where(x => x.BrokerageId == id).ToList();
                viewModel.Clients = Enumerable.Empty<Client>();
                foreach (Subscription y in x) {
                    if (viewModel.Clients == null)
                    {
                        viewModel.Clients = _context.Clients.Where(x => x.Id == y.ClientId);
                    }
                    else {
                        viewModel.Clients = viewModel.Clients.Append(_context.Clients.Where(x => x.Id == y.ClientId).Single());
                    }
                }
            }

            return View(viewModel);
        }

        // GET: Brokerages/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Brokerages == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brokerage == null)
            {
                return NotFound();
            }

            return View(brokerage);
        }

        // GET: Brokerages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brokerages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] Brokerage brokerage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brokerage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brokerage);
        }

        // GET: Brokerages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Brokerages == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null)
            {
                return NotFound();
            }
            return View(brokerage);
        }

        // POST: Brokerages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] Brokerage brokerage)
        {
            if (id != brokerage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brokerage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrokerageExists(brokerage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brokerage);
        }

        // GET: Brokerages/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Brokerages == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (brokerage == null)
            {
                return NotFound();
            }

            
            return View(brokerage);
        }

        // POST: Brokerages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Brokerages == null)
            {
                return Problem("Entity set 'MarketDbContext.Brokerages'  is null.");
            }
           
            var brokerage = await _context.Brokerages.FindAsync(id);
            if (_context.Ads.Where(x=>x.brokerageId == id).Count() != 0)
            {
                return View("Error",brokerage);
            }
            if (brokerage != null)
            {
                _context.Brokerages.Remove(brokerage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error(string id) {
            var brokerage = _context.Brokerages.FindAsync(id);

            return View();
        }

        private bool BrokerageExists(string id)
        {
          return _context.Brokerages.Any(e => e.Id == id);
        }
    }
}
