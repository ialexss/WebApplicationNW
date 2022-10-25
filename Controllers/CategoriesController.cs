using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> Index(string searchString, string nameColum, string order, string currentFilter, int? pageNumber)
        {

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["NameColum"] = nameColum;
            ViewData["CurrentOrder"] = order;


            var categorie = from m in _context.Categories
                           select m;

            if (!String.IsNullOrEmpty(nameColum) && !String.IsNullOrEmpty(searchString))
            {

                if (nameColum == "CategoryName")
                {
                    categorie = categorie.Where(s => s.CategoryName!.Contains(searchString));
                }
                else if (nameColum == "Description")
                {
                    categorie = categorie.Where(s => s.Description!.Contains(searchString));
                }
                
            }


            ViewData["FiltroCategoryName"] = String.IsNullOrEmpty(order) ? "CategoryNameDescendente" : "";
            ViewData["FiltroDescription"] = order == "DescriptionAscendente" ? "DescriptionDescendente" : "DescriptionAscendente";


            switch (order)
            {
                case "CategoryNameDescendente":
                    categorie = categorie.OrderByDescending(usuario => usuario.CategoryName);
                    break;
                case "DescriptionAscendente":
                    categorie = categorie.OrderBy(usuarios => usuarios.Description);
                    break;
                case "DescriptionDescendente":
                    categorie = categorie.OrderByDescending(usuarios => usuarios.Description);
                    break;
                default:
                    categorie = categorie.OrderBy(usuario => usuario.CategoryName);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Category>.CreateAsync(categorie.AsNoTracking(), pageNumber ?? 1, pageSize));

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
