using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplicationNW.Data;
using WebApplicationNW.Models;

namespace WebApplicationNW.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConverter _converter;
        public CategoriesController(ApplicationDbContext context, IConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string searchString, string nameColumn, string currentFilter, int? pageNumber, string order="Asc_CategoryName")
        {

            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;
            ViewData["NameColumn"] = nameColumn;
            ViewData["CurrentOrder"] = order;


            var categories = from m in _context.Categories
                           select m;

            if (!String.IsNullOrEmpty(nameColumn) && !String.IsNullOrEmpty(searchString))
            {
                var parameter_filter = Expression.Parameter(typeof(Category), "parameter");
                var lambda_filter = Expression.Lambda<Func<Category, bool>>(
                                  Expression.Call(
                                      instance: Expression.Property(parameter_filter, nameColumn),
                                      method: typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                                      arguments: Expression.Constant(searchString)
                                  ), parameter_filter);
                //categories = Queryable.Where(categories, lambda);
                categories = categories.Where(lambda_filter); 
            }

            foreach (var column in Category.Columns)
            {
                ViewData["Order"+column.Name] = ((order == "Asc_" + column.Name) ? "Des_" : "Asc_") + column.Name;
            }


            string columnaAordenar = order.Substring(4, order.Length-4);
            string modo= order.Substring(0, 3);
            var parameter_order = Expression.Parameter(typeof(Category), "parameter");
            var lambda_order = Expression.Lambda<Func<Category, Object>>(Expression.Property(parameter_order, columnaAordenar), parameter_order);
            categories = modo=="Asc"? categories.OrderBy(lambda_order): categories.OrderByDescending(lambda_order);

            int pageSize = 10;
            return View(await PaginatedList<Category>.CreateAsync(categories.AsNoTracking(), pageNumber ?? 1, pageSize));

            //return View(await _context.Categories.ToListAsync());
        }

        public async Task<IActionResult> VistaParaPdf()
        {

            var category = from m in _context.Categories
                           select m;

            return View(await category.ToListAsync());
        }

        public IActionResult MostrarPDFenPagina()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Categories/VistaParaPDF";


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

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,Description,Picture")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Description,Picture")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
