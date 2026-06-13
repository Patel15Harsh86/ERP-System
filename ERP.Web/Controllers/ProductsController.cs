using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Products";
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Add Product";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model)
        {
            var count = await _context.Products.CountAsync();
            model.ProductCode = $"PRD{(count + 1):D3}";
            model.CurrentStock = 0;
            _context.Products.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> StockIn(int id)
        {
            ViewData["Title"] = "Stock In";
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Product = product;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StockIn(int productId, int quantity, string reference, string? notes)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            product.CurrentStock += quantity;
            await _context.SaveChangesAsync();

            _context.StockMovements.Add(new StockMovement
            {
                ProductId = productId,
                MovementType = "IN",
                Quantity = quantity,
                Reference = reference,
                Notes = notes,
                MovementDate = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "System"
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}