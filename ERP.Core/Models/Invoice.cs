namespace ERP.Core.Models
{
    public class Invoice : BaseEntity
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public int SalesOrderId { get; set; }
        public SalesOrder? SalesOrder { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public string Status { get; set; } = "Unpaid";
        public string? Notes { get; set; }
    }
}