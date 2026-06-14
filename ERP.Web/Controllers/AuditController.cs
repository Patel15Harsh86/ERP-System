using ERP.Core.Models;
using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AuditController : Controller
    {
        private readonly AppDbContext _context;

        public AuditController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Audit Log";
            var logs = await _context.AuditLogs
                .OrderByDescending(a => a.CreatedAt)
                .Take(200)
                .ToListAsync();
            return View(logs);
        }

        public static async Task Log(AppDbContext context,
            string userEmail, string action,
            string module, string description,
            string? ipAddress = null)
        {
            context.AuditLogs.Add(new AuditLog
            {
                UserEmail = userEmail,
                Action = action,
                Module = module,
                Description = description,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }
}