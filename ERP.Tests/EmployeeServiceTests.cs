using ERP.Core.DTOs;
using ERP.Core.Interfaces;
using ERP.Core.Models;
using ERP.Core.Services;
using Moq;

namespace ERP.Tests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IRepository<Employee>> _mockEmpRepo;
        private readonly Mock<IRepository<Department>> _mockDeptRepo;
        private readonly Mock<IRepository<Designation>> _mockDesigRepo;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _mockEmpRepo = new Mock<IRepository<Employee>>();
            _mockDeptRepo = new Mock<IRepository<Department>>();
            _mockDesigRepo = new Mock<IRepository<Designation>>();
            _service = new EmployeeService(
                _mockEmpRepo.Object,
                _mockDeptRepo.Object,
                _mockDesigRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsAllEmployees()
        {
            // Arrange
            _mockEmpRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee>
                {
                    new Employee { Id=1, FirstName="John",
                        LastName="Doe", Status="Active",
                        DepartmentId=1, DesignationId=1 },
                    new Employee { Id=2, FirstName="Jane",
                        LastName="Smith", Status="Active",
                        DepartmentId=1, DesignationId=1 }
                });

            _mockDeptRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Department { DepartmentName = "IT" });

            _mockDesigRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Designation { DesignationName = "Developer" });

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetById_ReturnsCorrectEmployee()
        {
            // Arrange
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Employee
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Status = "Active",
                    DepartmentId = 1,
                    DesignationId = 1
                });

            _mockDeptRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Department { DepartmentName = "IT" });

            _mockDesigRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Designation { DesignationName = "Developer" });

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.FullName);
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockEmpRepo.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Create_GeneratesEmployeeCode()
        {
            // Arrange
            _mockEmpRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee>());
            _mockEmpRepo.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);
            _mockEmpRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Employee
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "User",
                    EmployeeCode = "EMP001",
                    DepartmentId = 1,
                    DesignationId = 1
                });
            _mockDeptRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Department { DepartmentName = "IT" });
            _mockDesigRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Designation { DesignationName = "Developer" });

            var dto = new CreateEmployeeDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                Phone = "1234567890",
                DepartmentId = 1,
                DesignationId = 1,
                DateOfJoining = DateTime.Today
            };

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            _mockEmpRepo.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            _mockEmpRepo.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _service.DeleteAsync(99);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Delete_ReturnsTrue_WhenFound()
        {
            // Arrange
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Employee { Id = 1 });
            _mockEmpRepo.Setup(r => r.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }
    }
}