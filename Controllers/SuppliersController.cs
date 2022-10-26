using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationNW.Data;
using WebApplicationNW.Models;

namespace WebApplicationNW.Controllers
{
    //[Authorize]
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index(string searchString, string nameColumn, string currentFilter, int? pageNumber, string order = "Asc_CompanyName")
        {

            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;
            ViewData["NameColumn"] = nameColumn;
            ViewData["CurrentOrder"] = order;


            var suppliers = from m in _context.Suppliers
                            select m;

            if (!String.IsNullOrEmpty(nameColumn) && !String.IsNullOrEmpty(searchString))
            {
                var paramenter_filter = Expression.Parameter(typeof(Supplier), "parameter");
                var lambda_filter = Expression.Lambda<Func<Supplier, bool>>(
                                  Expression.Call(
                                      instance: Expression.Property(paramenter_filter, nameColumn),
                                      method: typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                                      arguments: Expression.Constant(searchString)
                                  ), paramenter_filter);
                suppliers = suppliers.Where(lambda_filter);
            }

            foreach (var column in Supplier.Columns)
            {
                ViewData["Order" + column.Name] = ((order == "Asc_" + column.Name) ? "Des_" : "Asc_") + column.Name;
            }


            string columnaAordenar = order.Substring(4, order.Length - 4);
            string modo = order.Substring(0, 3);
            var parameter_order = Expression.Parameter(typeof(Supplier), "parameter");
            var lambda_order = Expression.Lambda<Func<Supplier, Object>>(Expression.Property(parameter_order, columnaAordenar), parameter_order);
            suppliers = modo == "Asc" ? suppliers.OrderBy(lambda_order) : suppliers.OrderByDescending(lambda_order);

            int pageSize = 10;
            return View(await PaginatedList<Supplier>.CreateAsync(suppliers.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax,HomePage")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierId,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax,HomePage")] Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierId))
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
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Suppliers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Suppliers'  is null.");
            }
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
          return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
