using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly AppDbContext _context;

        public PurchaseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Purchase Orders";
            var orders = await _context.PurchaseOrders
                .Include(o => o.Vendor)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "New Purchase Order";
            ViewBag.Vendors = await _context.Vendors.ToListAsync();
            ViewBag.Products = await _context.Products.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string vendorId,
            List<int> productIds, List<int> quantities,
            List<decimal> prices, string expectedDelivery)
        {
            var count = await _context.PurchaseOrders.CountAsync();
            var order = new PurchaseOrder
            {
                PONumber = $"PO-{DateTime.Now.Year}-{(count + 1):D3}",
                VendorId = int.Parse(vendorId),
                PODate = DateTime.UtcNow,
                ExpectedDelivery = DateTime.Parse(expectedDelivery),
                Status = "Draft",
                CreatedBy = User.Identity?.Name ?? "System"
            };

            decimal total = 0;
            for (int i = 0; i < productIds.Count; i++)
            {
                var amount = quantities[i] * prices[i];
                total += amount;
                order.Items.Add(new PurchaseOrderItem
                {
                    ProductId = productIds[i],
                    Quantity = quantities[i],
                    UnitPrice = prices[i],
                    Amount = amount,
                    CreatedBy = User.Identity?.Name ?? "System"
                });
            }

            order.TotalAmount = total;
            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "PO Details";
            var order = await _context.PurchaseOrders
                .Include(o => o.Vendor)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var order = await _context.PurchaseOrders.FindAsync(id);
            if (order == null) return NotFound();
            order.Status = "Approved";
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Receive(int id)
        {
            var order = await _context.PurchaseOrders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            foreach (var item in order.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.CurrentStock += item.Quantity;
                    _context.StockMovements.Add(new StockMovement
                    {
                        ProductId = item.ProductId,
                        MovementType = "IN",
                        Quantity = item.Quantity,
                        Reference = order.PONumber,
                        Notes = "Received from PO",
                        MovementDate = DateTime.UtcNow,
                        CreatedBy = User.Identity?.Name ?? "System"
                    });
                }
            }

            order.Status = "Received";
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}