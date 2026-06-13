namespace ERP.Core.Models
{
    public class Designation : BaseEntity
    {
        public string DesignationName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}