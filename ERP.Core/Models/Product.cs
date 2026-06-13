namespace ERP.Core.Models
{
    public class Product : BaseEntity
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int CurrentStock { get; set; } = 0;
        public int ReorderLevel { get; set; } = 10;
        public string? Category { get; set; }
    }
}