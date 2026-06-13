using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Customers";
            var customers = await _context.Customers.ToListAsync();
            return View(customers);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Add Customer";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer model)
        {
            model.CreatedBy = User.Identity?.Name ?? "System";
            _context.Customers.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Customer";
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Customer model)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            customer.CustomerName = model.CustomerName;
            customer.ContactPerson = model.ContactPerson;
            customer.Email = model.Email;
            customer.Phone = model.Phone;
            customer.Address = model.Address;
            customer.GSTNumber = model.GSTNumber;
            customer.CreditLimit = model.CreditLimit;
            customer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "Customer Details";
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }
    }
}