using ClosedXML.Excel;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Reports";
            return View();
        }

        public async Task<IActionResult> ExportEmployees()
        {
            var employees = await _context.Employees
                .Where(e => !e.IsDeleted)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Employees");

            // Header
            ws.Cell(1, 1).Value = "Code";
            ws.Cell(1, 2).Value = "First Name";
            ws.Cell(1, 3).Value = "Last Name";
            ws.Cell(1, 4).Value = "Email";
            ws.Cell(1, 5).Value = "Phone";
            ws.Cell(1, 6).Value = "Department";
            ws.Cell(1, 7).Value = "Designation";
            ws.Cell(1, 8).Value = "Joining Date";
            ws.Cell(1, 9).Value = "Status";

            var headerRow = ws.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F5C99");
            headerRow.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var emp in employees)
            {
                ws.Cell(row, 1).Value = emp.EmployeeCode;
                ws.Cell(row, 2).Value = emp.FirstName;
                ws.Cell(row, 3).Value = emp.LastName;
                ws.Cell(row, 4).Value = emp.Email;
                ws.Cell(row, 5).Value = emp.Phone;
                ws.Cell(row, 6).Value = emp.Department?.DepartmentName;
                ws.Cell(row, 7).Value = emp.Designation?.DesignationName;
                ws.Cell(row, 8).Value = emp.DateOfJoining.ToString("dd MMM yyyy");
                ws.Cell(row, 9).Value = emp.Status;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Employees.xlsx");
        }

        public async Task<IActionResult> ExportProducts()
        {
            var products = await _context.Products
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Products");

            ws.Cell(1, 1).Value = "Code";
            ws.Cell(1, 2).Value = "Product Name";
            ws.Cell(1, 3).Value = "Category";
            ws.Cell(1, 4).Value = "Unit";
            ws.Cell(1, 5).Value = "Cost Price";
            ws.Cell(1, 6).Value = "Sale Price";
            ws.Cell(1, 7).Value = "Current Stock";
            ws.Cell(1, 8).Value = "Stock Value";
            ws.Cell(1, 9).Value = "Reorder Level";

            var headerRow = ws.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F5C99");
            headerRow.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var p in products)
            {
                ws.Cell(row, 1).Value = p.ProductCode;
                ws.Cell(row, 2).Value = p.ProductName;
                ws.Cell(row, 3).Value = p.Category;
                ws.Cell(row, 4).Value = p.Unit;
                ws.Cell(row, 5).Value = p.CostPrice;
                ws.Cell(row, 6).Value = p.SalePrice;
                ws.Cell(row, 7).Value = p.CurrentStock;
                ws.Cell(row, 8).Value = p.CurrentStock * p.CostPrice;
                ws.Cell(row, 9).Value = p.ReorderLevel;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Products.xlsx");
        }

        public async Task<IActionResult> ExportSales()
        {
            var orders = await _context.SalesOrders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sales Orders");

            ws.Cell(1, 1).Value = "Order No";
            ws.Cell(1, 2).Value = "Customer";
            ws.Cell(1, 3).Value = "Order Date";
            ws.Cell(1, 4).Value = "Items";
            ws.Cell(1, 5).Value = "Sub Total";
            ws.Cell(1, 6).Value = "Tax";
            ws.Cell(1, 7).Value = "Total";
            ws.Cell(1, 8).Value = "Status";

            var headerRow = ws.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F5C99");
            headerRow.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var o in orders)
            {
                ws.Cell(row, 1).Value = o.OrderNumber;
                ws.Cell(row, 2).Value = o.Customer?.CustomerName;
                ws.Cell(row, 3).Value = o.OrderDate.ToString("dd MMM yyyy");
                ws.Cell(row, 4).Value = o.Items.Count;
                ws.Cell(row, 5).Value = o.SubTotal;
                ws.Cell(row, 6).Value = o.TaxAmount;
                ws.Cell(row, 7).Value = o.TotalAmount;
                ws.Cell(row, 8).Value = o.Status;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "SalesOrders.xlsx");
        }

        public async Task<IActionResult> ExportPayroll()
        {
            var payrolls = await _context.PayrollRuns
                .Include(p => p.Employee)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Payroll");

            ws.Cell(1, 1).Value = "Employee";
            ws.Cell(1, 2).Value = "Month";
            ws.Cell(1, 3).Value = "Basic";
            ws.Cell(1, 4).Value = "HRA";
            ws.Cell(1, 5).Value = "Allowances";
            ws.Cell(1, 6).Value = "Gross Salary";
            ws.Cell(1, 7).Value = "PF Deduction";
            ws.Cell(1, 8).Value = "Tax Deduction";
            ws.Cell(1, 9).Value = "Net Salary";
            ws.Cell(1, 10).Value = "Status";

            var headerRow = ws.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F5C99");
            headerRow.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var p in payrolls)
            {
                ws.Cell(row, 1).Value = $"{p.Employee?.FirstName} {p.Employee?.LastName}";
                ws.Cell(row, 2).Value = p.MonthName;
                ws.Cell(row, 3).Value = p.BasicSalary;
                ws.Cell(row, 4).Value = p.HRA;
                ws.Cell(row, 5).Value = p.Allowances;
                ws.Cell(row, 6).Value = p.GrossSalary;
                ws.Cell(row, 7).Value = p.PFDeduction;
                ws.Cell(row, 8).Value = p.TaxDeduction;
                ws.Cell(row, 9).Value = p.NetSalary;
                ws.Cell(row, 10).Value = p.Status;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Payroll.xlsx");
        }
    }
}