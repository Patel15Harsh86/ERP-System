namespace ERP.Core.DTOs
{
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfJoining { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
    }
}