using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using Microsoft.EntityFrameworkCore;
using WebApplicationNW.Data;
using WebApplicationNW.Models;

namespace WebApplicationNW.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IConverter _converter;
        public OrdersController(ApplicationDbContext context, IConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string searchString, string nameColumn, string currentFilter, int? pageNumber, string order = "")
        {

            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;
            ViewData["NameColumn"] = nameColumn;
            ViewData["CurrentOrder"] = order;


            var orders = from m in _context.Orders
                            select m;

            if (!String.IsNullOrEmpty(nameColumn) && !String.IsNullOrEmpty(searchString))
            {
                var paramenter_filter = Expression.Parameter(typeof(Order), "parameter");
                var lambda_filter = Expression.Lambda<Func<Order, bool>>(
                                  Expression.Call(
                                      instance: Expression.Property(paramenter_filter, nameColumn),
                                      method: typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                                      arguments: Expression.Constant(searchString)
                                  ), paramenter_filter);
                orders = orders.Where(lambda_filter);
            }

            foreach (var column in Order.Columns)
            {
                ViewData["Order" + column.Name] = ((order == "Asc_" + column.Name) ? "Des_" : "Asc_") + column.Name;
            }

            if (!String.IsNullOrEmpty(order))
            {
                string columnaAordenar = order.Substring(4, order.Length - 4);
                string modo = order.Substring(0, 3);
                var parameter_order = Expression.Parameter(typeof(Order), "parameter");
                var lambda_order = Expression.Lambda<Func<Order, Object>>(Expression.Property(parameter_order, columnaAordenar), parameter_order);
                orders = modo == "Asc" ? orders.OrderBy(lambda_order) : orders.OrderByDescending(lambda_order);
            }

            int pageSize = 10;
            return View(await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        public async Task<IActionResult> VistaParaPDF()
        {

            var order = from m in _context.Orders
                           select m;

            return View(await order.ToListAsync());
        }

        public IActionResult MostrarPDF()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Orders/VistaParaPDF";


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


            return File(archivoPDF, "application/pdf");
        }

        public IActionResult MostrarPDFdetail(int? id)
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Orders/VistaParaPDFdetail/{id}";


            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.B5,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings(){
                        Page = url_pagina
                    }
                }

            };

            var archivoPDF = _converter.Convert(pdf);


            return File(archivoPDF, "application/pdf");
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> VistaParaPDFdetail(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,EmployeeId,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipVia);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipVia);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,EmployeeId,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipVia);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
