using ERP.Core.DTOs;
using ERP.Core.Interfaces;
using ERP.Core.Models;

namespace ERP.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _repo;
        private readonly IRepository<Department> _deptRepo;
        private readonly IRepository<Designation> _desigRepo;

        public EmployeeService(
            IRepository<Employee> repo,
            IRepository<Department> deptRepo,
            IRepository<Designation> desigRepo)
        {
            _repo = repo;
            _deptRepo = deptRepo;
            _desigRepo = desigRepo;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _repo.GetAllAsync();
            var result = new List<EmployeeDto>();
            foreach (var e in employees)
            {
                var dept = await _deptRepo.GetByIdAsync(e.DepartmentId);
                var desig = await _desigRepo.GetByIdAsync(e.DesignationId);
                result.Add(new EmployeeDto
                {
                    Id = e.Id,
                    EmployeeCode = e.EmployeeCode,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Phone = e.Phone,
                    Status = e.Status,
                    DateOfJoining = e.DateOfJoining,
                    Department = dept?.DepartmentName,
                    Designation = desig?.DesignationName
                });
            }
            return result;
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            var dept = await _deptRepo.GetByIdAsync(e.DepartmentId);
            var desig = await _desigRepo.GetByIdAsync(e.DesignationId);
            return new EmployeeDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                Status = e.Status,
                DateOfJoining = e.DateOfJoining,
                Department = dept?.DepartmentName,
                Designation = desig?.DesignationName
            };
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
        {
            var employees = await _repo.GetAllAsync();
            var count = employees.Count();
            var employee = new Employee
            {
                EmployeeCode = $"EMP{(count + 1):D3}",
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfJoining = dto.DateOfJoining,
                DepartmentId = dto.DepartmentId,
                DesignationId = dto.DesignationId,
                Status = "Active"
            };
            await _repo.AddAsync(employee);
            return await GetByIdAsync(employee.Id) ?? new EmployeeDto();
        }

        public async Task<EmployeeDto> UpdateAsync(int id, CreateEmployeeDto dto)
        {
            var employee = await _repo.GetByIdAsync(id);
            if (employee == null) throw new Exception("Employee not found");
            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.Phone = dto.Phone;
            employee.DateOfJoining = dto.DateOfJoining;
            employee.DepartmentId = dto.DepartmentId;
            employee.DesignationId = dto.DesignationId;
            await _repo.UpdateAsync(employee);
            return await GetByIdAsync(id) ?? new EmployeeDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _repo.GetByIdAsync(id);
            if (employee == null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}