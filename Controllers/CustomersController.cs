using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationNW.Data;
using WebApplicationNW.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq.Expressions;

namespace WebApplicationNW.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IConverter _converter;

        public CustomersController(ApplicationDbContext context,IConverter converter)
        {
            _context = context;
            _converter = converter;
        }



        // GET: Customers
        public async Task<IActionResult> Index(string searchString, string nameColumn, string currentFilter, int? pageNumber, string order = "Des_CompanyName")
        {

            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;
            ViewData["NameColumn"] = nameColumn;
            ViewData["CurrentOrder"] = order;


            var customers = from m in _context.Customers
                             select m;

            if (!String.IsNullOrEmpty(nameColumn) && !String.IsNullOrEmpty(searchString))
            {
                var paramenter_filter = Expression.Parameter(typeof(Customer), "parameter");
                var lambda_filter = Expression.Lambda<Func<Customer, bool>>(
                                  Expression.Call(
                                      instance: Expression.Property(paramenter_filter, nameColumn),
                                      method: typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                                      arguments: Expression.Constant(searchString)
                                  ), paramenter_filter);
                customers = customers.Where(lambda_filter);
            }

            foreach (var column in Customer.Columns)
            {
                ViewData["Order" + column.Name] = ((order == "Asc_" + column.Name) ? "Des_" : "Asc_") + column.Name;
            }


            string columnaAordenar = order.Substring(4, order.Length - 4);
            string modo = order.Substring(0, 3);
            var parameter_order = Expression.Parameter(typeof(Customer), "parameter");
            var lambda_order = Expression.Lambda<Func<Customer, Object>>(Expression.Property(parameter_order, columnaAordenar), parameter_order);
            customers = modo == "Asc" ? customers.OrderBy(lambda_order) : customers.OrderByDescending(lambda_order);

            int pageSize = 10;
            return View(await PaginatedList<Customer>.CreateAsync(customers.AsNoTracking(), pageNumber ?? 1, pageSize));

        }
        public async Task<IActionResult> VistaParaPDF(string searchString, string nameColum, string order, string currentFilter, int? pageNumber)
        {

            var customer = from m in _context.Customers
                           select m;

            return View(await customer.ToListAsync());
        }

        public IActionResult MostrarPDFenPagina()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Customers/VistaParaPDF";


            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings(){
                        Page = url_pagina
                    }
                }

            };

            var archivoPDF = _converter.Convert(pdf);


            return File(archivoPDF,"application/pdf");
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CustomerId,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(string id)
        {
          return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
