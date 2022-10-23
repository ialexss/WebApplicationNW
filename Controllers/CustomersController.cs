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

namespace WebApplicationNW.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

      

        // GET: Customers
        public async Task<IActionResult> Index(string searchString,string nameColum,string order,string currentFilter, int? pageNumber)
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


            var customer = from m in _context.Customers
                            select m;

            if (!String.IsNullOrEmpty(nameColum) && !String.IsNullOrEmpty(searchString))
            {

                if (nameColum == "CompanyName")
                {
                    customer = customer.Where(s => s.CompanyName!.Contains(searchString));
                }
                else if (nameColum == "ContactName")
                {
                    customer = customer.Where(s => s.ContactName!.Contains(searchString));
                }
                else if (nameColum == "ContactTitle")
                {
                    customer = customer.Where(s => s.ContactTitle!.Contains(searchString));
                }
                else if (nameColum == "Address")
                {
                    customer = customer.Where(s => s.Address!.Contains(searchString));
                }
                else if (nameColum == "City")
                {
                    customer = customer.Where(s => s.City!.Contains(searchString));
                }
                else if (nameColum == "Region")
                {
                    customer = customer.Where(s => s.Region!.Contains(searchString));
                }
                else if (nameColum == "PostalCode")
                {
                    customer = customer.Where(s => s.PostalCode!.Contains(searchString));
                }
                else if (nameColum == "Country")
                {
                    customer = customer.Where(s => s.Country!.Contains(searchString));
                }
                else if (nameColum == "Phone")
                {
                    customer = customer.Where(s => s.Phone!.Contains(searchString));
                }
                else if (nameColum == "Fax")
                {
                    customer = customer.Where(s => s.Fax!.Contains(searchString));
                }
            }


            ViewData["FiltroCompany"] = String.IsNullOrEmpty(order) ? "CompanyDescendente" : "";
            ViewData["FiltroName"] = order == "NameAscendente" ? "NameDescendente" : "NameAscendente";
            ViewData["FiltroContactTitle"] = order == "ContactTitleAscendente" ? "ContactTitleDescendente" : "ContactTitleAscendente";
            ViewData["FiltroAddress"] = order == "AddressAscendente" ? "AddressDescendente" : "AddressAscendente";
            ViewData["FiltroCity"] = order == "CityAscendente" ? "CityDescendente" : "CityAscendente";
            ViewData["FiltroPostalCode"] = order == "PostalCodeAscendente" ? "PostalCodeDescendente" : "PostalCodeAscendente";
            ViewData["FiltroCountry"] = order == "CountryAscendente" ? "CountryDescendente" : "CountryAscendente";
            ViewData["FiltroPhone"] = order == "PhoneAscendente" ? "PhoneDescendente" : "PhoneAscendente";
            ViewData["FiltroFax"] = order == "FaxAscendente" ? "FaxDescendente" : "FaxAscendente";

            switch (order)
            {
                case "CompanyDescendente":
                    customer = customer.OrderByDescending(usuario => usuario.CompanyName);
                    break;
                case "NameAscendente":
                    customer = customer.OrderBy(usuarios => usuarios.ContactName);
                    break;
                case "NameDescendente":
                    customer = customer.OrderByDescending(usuarios => usuarios.ContactName);
                    break;
                case "ContactTitleAscendente":
                    customer = customer.OrderBy(usuarios => usuarios.ContactTitle);
                    break;
                case "ContactTitleDescendente":
                    customer = customer.OrderByDescending(usuarios => usuarios.ContactTitle);
                    break;
                case "AddressAscendente":
                    customer = customer.OrderBy(usuarios => usuarios.Address);
                    break;
                case "AddressDescendente":
                    customer = customer.OrderByDescending(usuarios => usuarios.Address);
                    break;
                case "CityAscendente":
                    customer = customer.OrderBy(usuarios => usuarios.City);
                    break;
                case "CityDescendente":
                    customer = customer.OrderByDescending(usuarios => usuarios.City);
                    break;
                case "PostalCodeAscendente":
                    customer = customer.OrderBy(usuarios => usuarios.PostalCode);
                    break;
                case "PostalCodeDescendente":
                    customer = customer.OrderByDescending(usuarios => usuarios.PostalCode);
                    break;
                case "CountryAscendente":
                    customer = customer.OrderBy(usuarios => usuarios.Country);
                    break;
                case "CountryDescendente":
                    customer = customer.OrderByDescending(usuarios => usuarios.Country);
                    break;
                case "PhoneAscendente":
                    customer = customer.OrderBy(usuarios => usuarios.Phone);
                    break;
                case "PhoneDescendente":
                    customer = customer.OrderByDescending(usuarios => usuarios.Phone);
                    break;
                case "FaxAscendente":
                    customer = customer.OrderBy(usuarios => usuarios.Fax);
                    break;
                case "FaxDescendente":
                    customer = customer.OrderByDescending(usuarios => usuarios.Fax);
                    break;
                default:
                    customer = customer.OrderBy(usuario => usuario.ContactName);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Customer>.CreateAsync(customer.AsNoTracking(), pageNumber ?? 1, pageSize));

            //return View(await customer.ToListAsync());
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
