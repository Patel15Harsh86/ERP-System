using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Accounts";
            var invoices = await _context.Invoices
                .Include(i => i.SalesOrder)
                .ThenInclude(o => o.Customer)
                .ToListAsync();
            var payrolls = await _context.PayrollRuns
                .Include(p => p.Employee)
                .ToListAsync();

            ViewBag.TotalSales = invoices.Sum(i => i.Amount);
            ViewBag.TotalPaid = invoices.Sum(i => i.PaidAmount);
            ViewBag.TotalOutstanding = invoices.Sum(i => i.Amount - i.PaidAmount);
            ViewBag.TotalPayroll = payrolls.Sum(p => p.NetSalary);
            ViewBag.Invoices = invoices;
            ViewBag.Payrolls = payrolls;
            return View();
        }
    }
}