namespace ERP.Core.Models
{
    public class SalaryStructure : BaseEntity
    {
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal Allowances { get; set; }
        public decimal PFDeduction { get; set; }
        public decimal TaxDeduction { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal GrossSalary => BasicSalary + HRA + Allowances;
        public decimal NetSalary => GrossSalary - PFDeduction - TaxDeduction - OtherDeductions;
    }
}