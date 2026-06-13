namespace ERP.Core.Models
{
    public class PurchaseOrder : BaseEntity
    {
        public string PONumber { get; set; } = string.Empty;
        public int VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        public DateTime PODate { get; set; } = DateTime.UtcNow;
        public DateTime ExpectedDelivery { get; set; }
        public string Status { get; set; } = "Draft";
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }

    public class PurchaseOrderItem : BaseEntity
    {
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}