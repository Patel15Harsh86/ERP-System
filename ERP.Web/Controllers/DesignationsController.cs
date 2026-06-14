using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class DesignationsController : Controller
    {
        private readonly AppDbContext _context;

        public DesignationsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Designations";
            var designations = await _context.Designations
                .Where(d => !d.IsDeleted)
                .ToListAsync();
            return View(designations);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Add Designation";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Designation model)
        {
            model.CreatedBy = User.Identity?.Name ?? "System";
            _context.Designations.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Designation";
            var desig = await _context.Designations.FindAsync(id);
            if (desig == null) return NotFound();
            return View(desig);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Designation model)
        {
            var desig = await _context.Designations.FindAsync(id);
            if (desig == null) return NotFound();
            desig.DesignationName = model.DesignationName;
            desig.Description = model.Description;
            desig.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var desig = await _context.Designations.FindAsync(id);
            if (desig == null) return NotFound();
            desig.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}