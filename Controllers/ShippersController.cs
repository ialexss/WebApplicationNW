using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationNW.Data;
using WebApplicationNW.Models;

namespace WebApplicationNW.Controllers
{
    public class ShippersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShippersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shippers
        public async Task<IActionResult> Index(string searchString, string nameColumn, string currentFilter, int? pageNumber, string order = "Asc_CompanyName")
        {

            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;
            ViewData["NameColumn"] = nameColumn;
            ViewData["CurrentOrder"] = order;


            var shippers = from m in _context.Shippers
                            select m;

            if (!String.IsNullOrEmpty(nameColumn) && !String.IsNullOrEmpty(searchString))
            {
                var paramenter_filter = Expression.Parameter(typeof(Shipper), "parameter");
                var lambda_filter = Expression.Lambda<Func<Shipper, bool>>(
                                  Expression.Call(
                                      instance: Expression.Property(paramenter_filter, nameColumn),
                                      method: typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                                      arguments: Expression.Constant(searchString)
                                  ), paramenter_filter);
                shippers = shippers.Where(lambda_filter);
            }

            foreach (var column in Shipper.Columns)
            {
                ViewData["Order" + column.Name] = ((order == "Asc_" + column.Name) ? "Des_" : "Asc_") + column.Name;
            }


            string columnaAordenar = order.Substring(4, order.Length - 4);
            string modo = order.Substring(0, 3);
            var parameter_order = Expression.Parameter(typeof(Shipper), "parameter");
            var lambda_order = Expression.Lambda<Func<Shipper, Object>>(Expression.Property(parameter_order, columnaAordenar), parameter_order);
            shippers = modo == "Asc" ? shippers.OrderBy(lambda_order) : shippers.OrderByDescending(lambda_order);

            int pageSize = 10;
            return View(await PaginatedList<Shipper>.CreateAsync(shippers.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        // GET: Shippers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Shippers == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.ShipperId == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // GET: Shippers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shippers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShipperId,CompanyName,Phone")] Shipper shipper)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shipper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shipper);
        }

        // GET: Shippers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shippers == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers.FindAsync(id);
            if (shipper == null)
            {
                return NotFound();
            }
            return View(shipper);
        }

        // POST: Shippers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShipperId,CompanyName,Phone")] Shipper shipper)
        {
            if (id != shipper.ShipperId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipperExists(shipper.ShipperId))
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
            return View(shipper);
        }

        // GET: Shippers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shippers == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.ShipperId == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // POST: Shippers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shippers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Shippers'  is null.");
            }
            var shipper = await _context.Shippers.FindAsync(id);
            if (shipper != null)
            {
                _context.Shippers.Remove(shipper);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipperExists(int id)
        {
          return _context.Shippers.Any(e => e.ShipperId == id);
        }
    }
}
