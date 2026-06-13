using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class VendorsController : Controller
    {
        private readonly AppDbContext _context;

        public VendorsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Vendors";
            var vendors = await _context.Vendors.ToListAsync();
            return View(vendors);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Add Vendor";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vendor model)
        {
            _context.Vendors.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "Vendor Details";
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();
            return View(vendor);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Vendor";
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();
            return View(vendor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Vendor model)
        {
            _context.Vendors.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}