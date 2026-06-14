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
        public async Task<IActionResult> StockIn(int productId, int quantity,
            string reference, string? notes)
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

        public async Task<IActionResult> StockOut(int id)
        {
            ViewData["Title"] = "Stock Out";
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Product = product;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StockOut(int productId, int quantity,
            string reference, string? notes)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            if (product.CurrentStock < quantity)
            {
                ViewBag.Product = product;
                ViewBag.Error = $"Insufficient stock! Available: {product.CurrentStock}";
                return View();
            }

            product.CurrentStock -= quantity;
            await _context.SaveChangesAsync();

            _context.StockMovements.Add(new StockMovement
            {
                ProductId = productId,
                MovementType = "OUT",
                Quantity = quantity,
                Reference = reference,
                Notes = notes,
                MovementDate = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "System"
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Product";
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            product.ProductName = model.ProductName;
            product.Category = model.Category;
            product.Unit = model.Unit;
            product.CostPrice = model.CostPrice;
            product.SalePrice = model.SalePrice;
            product.ReorderLevel = model.ReorderLevel;
            product.Description = model.Description;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}