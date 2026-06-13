using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Sales Orders";
            var orders = await _context.SalesOrders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "New Sales Order";
            ViewBag.Customers = await _context.Customers.ToListAsync();
            ViewBag.Products = await _context.Products.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string customerId,
            List<int> productIds, List<int> quantities, List<decimal> prices)
        {
            var count = await _context.SalesOrders.CountAsync();
            var order = new SalesOrder
            {
                OrderNumber = $"SO-{DateTime.Now.Year}-{(count + 1):D3}",
                CustomerId = int.Parse(customerId),
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                CreatedBy = User.Identity?.Name ?? "System"
            };

            decimal subTotal = 0;
            for (int i = 0; i < productIds.Count; i++)
            {
                var amount = quantities[i] * prices[i];
                subTotal += amount;
                order.Items.Add(new SalesOrderItem
                {
                    ProductId = productIds[i],
                    Quantity = quantities[i],
                    UnitPrice = prices[i],
                    Amount = amount,
                    CreatedBy = User.Identity?.Name ?? "System"
                });
            }

            order.SubTotal = subTotal;
            order.TaxAmount = subTotal * 0.18m;
            order.TotalAmount = subTotal + order.TaxAmount;

            _context.SalesOrders.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "Order Details";
            var order = await _context.SalesOrders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateInvoice(int orderId)
        {
            var order = await _context.SalesOrders.FindAsync(orderId);
            if (order == null) return NotFound();

            var count = await _context.Invoices.CountAsync();
            var invoice = new Invoice
            {
                InvoiceNumber = $"INV-{DateTime.Now.Year}-{(count + 1):D3}",
                SalesOrderId = orderId,
                InvoiceDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Amount = order.TotalAmount,
                Status = "Unpaid",
                CreatedBy = User.Identity?.Name ?? "System"
            };

            order.Status = "Invoiced";
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction("Invoices");
        }

        public async Task<IActionResult> Invoices()
        {
            ViewData["Title"] = "Invoices";
            var invoices = await _context.Invoices
                .Include(i => i.SalesOrder)
                .ThenInclude(o => o.Customer)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
            return View(invoices);
        }

        public async Task<IActionResult> InvoiceDetails(int id)
        {
            ViewData["Title"] = "Invoice Details";
            var invoice = await _context.Invoices
                .Include(i => i.SalesOrder)
                .ThenInclude(o => o.Customer)
                .Include(i => i.SalesOrder)
                .ThenInclude(o => o.Items)
                .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }
    }
}