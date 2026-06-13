using ERP.Core.DTOs;
using ERP.Core.Interfaces;
using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _service;
        private readonly AppDbContext _context;

        public EmployeesController(IEmployeeService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Employees";
            var employees = await _service.GetAllAsync();
            return View(employees);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Add Employee";
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Designations = await _context.Designations.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto dto)
        {
            await _service.CreateAsync(dto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Employee";
            var emp = await _service.GetByIdAsync(id);
            if (emp == null) return NotFound();
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Designations = await _context.Designations.ToListAsync();
            return View(emp);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateEmployeeDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "Employee Details";
            var emp = await _service.GetByIdAsync(id);
            if (emp == null) return NotFound();
            return View(emp);
        }
    }
}