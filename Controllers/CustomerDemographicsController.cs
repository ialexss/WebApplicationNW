using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationNW.Data;
using WebApplicationNW.Models;

namespace WebApplicationNW.Controllers
{
    public class CustomerDemographicsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerDemographicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CustomerDemographics
        public async Task<IActionResult> Index()
        {
              return View(await _context.CustomerDemographics.ToListAsync());
        }

        // GET: CustomerDemographics/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.CustomerDemographics == null)
            {
                return NotFound();
            }

            var customerDemographic = await _context.CustomerDemographics
                .FirstOrDefaultAsync(m => m.CustomerTypeId == id);
            if (customerDemographic == null)
            {
                return NotFound();
            }

            return View(customerDemographic);
        }

        // GET: CustomerDemographics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomerDemographics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerTypeId,CustomerDesc")] CustomerDemographic customerDemographic)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerDemographic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customerDemographic);
        }

        // GET: CustomerDemographics/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.CustomerDemographics == null)
            {
                return NotFound();
            }

            var customerDemographic = await _context.CustomerDemographics.FindAsync(id);
            if (customerDemographic == null)
            {
                return NotFound();
            }
            return View(customerDemographic);
        }

        // POST: CustomerDemographics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CustomerTypeId,CustomerDesc")] CustomerDemographic customerDemographic)
        {
            if (id != customerDemographic.CustomerTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerDemographic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerDemographicExists(customerDemographic.CustomerTypeId))
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
            return View(customerDemographic);
        }

        // GET: CustomerDemographics/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.CustomerDemographics == null)
            {
                return NotFound();
            }

            var customerDemographic = await _context.CustomerDemographics
                .FirstOrDefaultAsync(m => m.CustomerTypeId == id);
            if (customerDemographic == null)
            {
                return NotFound();
            }

            return View(customerDemographic);
        }

        // POST: CustomerDemographics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.CustomerDemographics == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CustomerDemographics'  is null.");
            }
            var customerDemographic = await _context.CustomerDemographics.FindAsync(id);
            if (customerDemographic != null)
            {
                _context.CustomerDemographics.Remove(customerDemographic);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerDemographicExists(string id)
        {
          return _context.CustomerDemographics.Any(e => e.CustomerTypeId == id);
        }
    }
}
