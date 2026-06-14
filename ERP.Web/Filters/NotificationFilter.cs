using ERP.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Filters
{
    public class NotificationFilter : IAsyncActionFilter
    {
        private readonly AppDbContext _context;

        public NotificationFilter(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context.Controller is Controller controller)
            {
                var lowStockCount = await _context.Products
                    .Where(p => !p.IsDeleted && p.CurrentStock <= p.ReorderLevel)
                    .CountAsync();

                controller.ViewBag.LowStockCount = lowStockCount;
            }
            await next();
        }
    }
}