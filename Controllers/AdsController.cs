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
using Azure.Storage.Blobs;
using Azure;

namespace Assignment2.Controllers
{
    public class AdsController : Controller
    {
        private readonly MarketDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string nameContainer = "advertisements";

        public AdsController(MarketDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Ads
        public async Task<IActionResult> Index(string id)
        {
            var viewModel = new AdsViewModel {
                Ads = await _context.Ads.Where(x => x.brokerageId == id).ToListAsync(),
                Brokerage = await _context.Brokerages.FindAsync(id)
            };

             return View(viewModel);
        }

        // GET: Ads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ad == null)
            {
                return NotFound();
            }

            var ad = await _context.Ad
                .FirstOrDefaultAsync(m => m.AdId == id);
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        // GET: Ads/Create
        public IActionResult Create(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        // POST: Ads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        /*
        public async Task<IActionResult> Create([Bind("AdId,FileName,Url")] Ad ad)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(ad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ad);
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file,string id)
        {
            BlobContainerClient containerClient;

            ViewData["id"] = id;

            

            var containerName = nameContainer;
            // Create the container and return a container client object
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (Azure.RequestFailedException e)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }


            try
            {
                string randomFileName = Path.GetRandomFileName();
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(randomFileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                // add the photo to the database if it uploaded successfully
                var addAd = new Ad
                {
                    Url = blockBlob.Uri.AbsoluteUri,
                    FileName = randomFileName,
                    brokerageId = id
                };
                
                _context.Ads.Add(addAd);
                _context.SaveChanges();
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("/Shared","Error");
            }

            var viewModel = new AdsViewModel
            {
                Ads = await _context.Ads.Where(x => x.brokerageId == id).ToListAsync(),
                Brokerage = await _context.Brokerages.FindAsync(id)
            };

            return View("Index",viewModel);
        }

        // GET: Ads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ad == null)
            {
                return NotFound();
            }

            var ad = await _context.Ad.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            return View(ad);
        }

        // POST: Ads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdId,FileName,Url")] Ad ad)
        {
            if (id != ad.AdId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdExists(ad.AdId))
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
            return View(ad);
        }

        // GET: Ads/Delete/5
        public async Task<IActionResult> Delete(int? id)
       {
            if (id == null)
            {
                return NotFound();
            }

            var AdDelete = await _context.Ads.FindAsync(id);

            string returnId = AdDelete.brokerageId;

            if (AdDelete != null)
            {
                BlobContainerClient containerClient;
                try
                {
                    containerClient = _blobServiceClient.GetBlobContainerClient(nameContainer);
                }
                catch (RequestFailedException)
                {
                    return RedirectToPage("Error");
                }

                try
                {
                    // Get the blob that holds the data
                    var blockBlob = containerClient.GetBlobClient(AdDelete.FileName);
                    if (await blockBlob.ExistsAsync())
                    {
                        await blockBlob.DeleteAsync();
                    }

                    _context.Ads.Remove(AdDelete);
                    await _context.SaveChangesAsync();

                }
                catch (RequestFailedException)
                {
                    return RedirectToPage("Error");
                }
            }

            var viewModel = new AdsViewModel
            {
                Ads = await _context.Ads.Where(x => x.brokerageId == returnId).ToListAsync(),
                Brokerage = await _context.Brokerages.FindAsync(returnId)
            };

            return View("Index", viewModel);
        }

        // POST: Ads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, BlobServiceClient blobServiceClient)
        {
            
            if (_context.Ad == null)
            {
                return Problem("Entity set 'MarketDbContext.Ad'  is null.");
            }
            var ad = await _context.Ad.FindAsync(id);
            if (ad != null)
            {
                _context.Ad.Remove(ad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdExists(int id)
        {
          return _context.Ad.Any(e => e.AdId == id);
        }
    }
}
