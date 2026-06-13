namespace ERP.Core.Models
{
    public class StockMovement : BaseEntity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string MovementType { get; set; } = string.Empty; // IN or OUT
        public int Quantity { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    }
}