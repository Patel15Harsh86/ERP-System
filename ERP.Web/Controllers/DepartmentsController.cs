using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Departments";
            var departments = await _context.Departments
                .Where(d => !d.IsDeleted)
                .Include(d => d.Employees)
                .ToListAsync();
            return View(departments);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Add Department";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department model)
        {
            model.CreatedBy = User.Identity?.Name ?? "System";
            _context.Departments.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Department";
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Department model)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return NotFound();
            dept.DepartmentName = model.DepartmentName;
            dept.Description = model.Description;
            dept.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return NotFound();
            dept.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}