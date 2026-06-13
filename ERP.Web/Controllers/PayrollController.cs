using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class PayrollController : Controller
    {
        private readonly AppDbContext _context;

        public PayrollController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Payroll";
            var payrolls = await _context.PayrollRuns
                .Include(p => p.Employee)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ToListAsync();
            return View(payrolls);
        }

        public async Task<IActionResult> SalaryStructure()
        {
            ViewData["Title"] = "Salary Structure";
            var structures = await _context.SalaryStructures
                .Include(s => s.Employee)
                .ToListAsync();
            ViewBag.Employees = await _context.Employees
                .Where(e => !e.IsDeleted).ToListAsync();
            return View(structures);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSalary(SalaryStructure model)
        {
            var existing = await _context.SalaryStructures
                .FirstOrDefaultAsync(s => s.EmployeeId == model.EmployeeId);
            if (existing != null)
            {
                existing.BasicSalary = model.BasicSalary;
                existing.HRA = model.HRA;
                existing.Allowances = model.Allowances;
                existing.PFDeduction = model.PFDeduction;
                existing.TaxDeduction = model.TaxDeduction;
                existing.OtherDeductions = model.OtherDeductions;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                model.CreatedBy = User.Identity?.Name ?? "System";
                _context.SalaryStructures.Add(model);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("SalaryStructure");
        }

        public async Task<IActionResult> ProcessPayroll()
        {
            ViewData["Title"] = "Process Payroll";
            ViewBag.Employees = await _context.Employees
                .Where(e => !e.IsDeleted).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayroll(int month, int year)
        {
            var employees = await _context.Employees
                .Where(e => !e.IsDeleted).ToListAsync();

            foreach (var emp in employees)
            {
                var salary = await _context.SalaryStructures
                    .FirstOrDefaultAsync(s => s.EmployeeId == emp.Id);
                if (salary == null) continue;

                var exists = await _context.PayrollRuns
                    .AnyAsync(p => p.EmployeeId == emp.Id
                        && p.Month == month && p.Year == year);
                if (exists) continue;

                _context.PayrollRuns.Add(new PayrollRun
                {
                    EmployeeId = emp.Id,
                    Month = month,
                    Year = year,
                    BasicSalary = salary.BasicSalary,
                    HRA = salary.HRA,
                    Allowances = salary.Allowances,
                    GrossSalary = salary.GrossSalary,
                    PFDeduction = salary.PFDeduction,
                    TaxDeduction = salary.TaxDeduction,
                    OtherDeductions = salary.OtherDeductions,
                    NetSalary = salary.NetSalary,
                    Status = "Processed",
                    CreatedBy = User.Identity?.Name ?? "System"
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Payslip(int id)
        {
            ViewData["Title"] = "Payslip";
            var payroll = await _context.PayrollRuns
                .Include(p => p.Employee)
                .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (payroll == null) return NotFound();
            return View(payroll);
        }
    }
}