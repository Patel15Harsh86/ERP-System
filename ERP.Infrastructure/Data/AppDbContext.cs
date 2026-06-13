using ERP.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<SalaryStructure> SalaryStructures { get; set; }
        public DbSet<PayrollRun> PayrollRuns { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Soft delete global filters
            builder.Entity<Employee>()
                .HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Department>()
                .HasQueryFilter(d => !d.IsDeleted);
            builder.Entity<Product>()
                .HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Vendor>()
                .HasQueryFilter(v => !v.IsDeleted);
            builder.Entity<Customer>()
                .HasQueryFilter(c => !c.IsDeleted);
        }
    }
}