namespace ERP.Core.Models
{
    public class Customer : BaseEntity
    {
        public string CustomerName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? GSTNumber { get; set; }
        public decimal CreditLimit { get; set; } = 0;
    }
}