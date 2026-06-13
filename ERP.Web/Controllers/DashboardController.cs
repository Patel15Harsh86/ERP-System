using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";

            // Real data from database
            ViewBag.TotalEmployees = await _context.Employees
                .Where(e => !e.IsDeleted).CountAsync();
            ViewBag.TotalProducts = await _context.Products
                .Where(p => !p.IsDeleted).CountAsync();
            ViewBag.LowStockProducts = await _context.Products
                .Where(p => !p.IsDeleted && p.CurrentStock <= p.ReorderLevel).CountAsync();
            ViewBag.TotalVendors = await _context.Vendors
                .Where(v => !v.IsDeleted).CountAsync();
            ViewBag.TotalCustomers = await _context.Customers
                .Where(c => !c.IsDeleted).CountAsync();
            ViewBag.PendingOrders = await _context.SalesOrders
                .Where(o => o.Status == "Pending").CountAsync();
            ViewBag.TotalSales = await _context.Invoices
                .SumAsync(i => i.Amount);
            ViewBag.UnpaidInvoices = await _context.Invoices
                .Where(i => i.Status == "Unpaid").CountAsync();
            ViewBag.StockValue = await _context.Products
                .Where(p => !p.IsDeleted)
                .SumAsync(p => p.CurrentStock * p.CostPrice);
            ViewBag.TotalPayroll = await _context.PayrollRuns
                .SumAsync(p => p.NetSalary);

            // Monthly sales for chart
            var monthlySales = await _context.Invoices
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(i => i.Amount)
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            ViewBag.ChartLabels = string.Join(",",
                monthlySales.Select(m =>
                    $"'{new DateTime(m.Year, m.Month, 1).ToString("MMM yyyy")}'"));
            ViewBag.ChartData = string.Join(",",
                monthlySales.Select(m => m.Total));

            // Recent orders
            ViewBag.RecentOrders = await _context.SalesOrders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}