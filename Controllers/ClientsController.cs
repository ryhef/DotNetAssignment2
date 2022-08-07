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
    public class ClientsController : Controller
    {
        private readonly MarketDbContext _context;

        public ClientsController(MarketDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(int? id)
        {

            var viewModel = new BrokerageViewModel
            {
                Clients = await _context.Clients
              .Include(i => i.Subscriptions)
              .AsNoTracking()
              .OrderBy(i => i)
              .ToListAsync()
            };
            if (id != null)
            {
                ViewData["ClientID"] = id;

                var x = _context.Subscriptions.Where(x => x.ClientId == id).ToList();
                viewModel.Brokerages = Enumerable.Empty<Brokerage>();
                foreach (Subscription y in x)
                {
                    if (viewModel.Brokerages == null)
                    {

                        viewModel.Brokerages = _context.Brokerages.Where(x => x.Id == y.BrokerageId);
                    }
                    else
                    {
                        viewModel.Brokerages = viewModel.Brokerages.Append(_context.Brokerages.Where(x => x.Id == y.BrokerageId).Single());
                    }
                }

            }

            return View(viewModel);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.Clients == null)
            {
                return Problem("Entity set 'MarketDbContext.Clients'  is null.");
            }
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        //POST: Clients/EditSubscriptions/5
        public async Task<IActionResult> EditSubscriptions(int id)
        {
            var viewModel = new ClientSubscriptionsViewModel
            {
                Client = await _context.Clients.FindAsync(id),
              
            };
            var x = await _context.Brokerages.Include(i => i.Subscriptions).ToListAsync();
            viewModel.Subscriptions = Enumerable.Empty<BrokerageSubscriptionsViewModel>();
            foreach (var brokerage in x) {
                viewModel.Subscriptions = viewModel.Subscriptions
                    .Append(new BrokerageSubscriptionsViewModel 
                    { 
                        BrokerageId = brokerage.Id, 
                        Title = brokerage.Title, 
                        IsMember = brokerage.Subscriptions.Any(x => x.ClientId == id) 
                    });
            }
            
            return View(viewModel);
        }
        
        //POST: Clients/AddSubscriptions?clientId=5&brokerageId=A1
        public IActionResult AddSubscriptions(int clientId, string brokerageId)
        {
            Subscription addSub = new Subscription() {
                ClientId = clientId,
                BrokerageId = brokerageId
            };
            _context.Subscriptions.Add(addSub);
            _context.SaveChanges();
            string returnString = "EditSubScriptions/" + clientId;
            var returnClient = _context.Clients.Where(x => x.Id == clientId).Single();
            return RedirectToAction("EditSubScriptions",returnClient);
        }

        //POST: Clients/RemoveSubscriptions?clientId=5&brokerageId=A1
        public IActionResult RemoveSubscriptions(int clientId, string brokerageId)
        {
            Subscription removeSub = new Subscription()
            {
                ClientId = clientId,
                BrokerageId = brokerageId
            };
            var findSub = _context.Subscriptions.Where(x => x.ClientId == clientId&&x.BrokerageId == brokerageId).Single();
            _context.Subscriptions.Remove(findSub);
            _context.SaveChanges();
            var returnClient = _context.Clients.Where(x => x.Id == clientId).Single();

            return RedirectToAction("EditSubScriptions", returnClient);

        }
        
    }
}
