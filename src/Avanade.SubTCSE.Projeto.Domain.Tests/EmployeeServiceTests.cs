using System.Globalization;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.Employee.Entities;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.Employee.Interfaces.Repositories;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.Employee.Services;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.Employee.Validators;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.EmployeeRole.Entities;
using Avanade.SubTCSE.Projeto.Domain.Base.Repository.MongoDB;
using MongoDB.Driver;
using Moq;

namespace Avanade.SubTCSE.Projeto.Domain.Tests;

public class EmployeeServiceTests
{
    private readonly EmployeeValidator _validator;
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly EmployeeService _employeeService;
    private readonly Mock<IMongoCollection<Employee>> _collectionMock;
    private readonly Mock<IMongoDBContext> _dbContextMock;

    public EmployeeServiceTests()
    {
        _validator = new EmployeeValidator();
        _repositoryMock = new Mock<IEmployeeRepository>();
        _employeeService = new EmployeeService(_validator, _repositoryMock.Object);

        _collectionMock = new Mock<IMongoCollection<Employee>>();

        _dbContextMock = new Mock<IMongoDBContext>();
        _dbContextMock.Setup(db => db.GetCollection<Employee>(It.IsAny<string>())).Returns(_collectionMock.Object);

        _repositoryMock = new Mock<IEmployeeRepository>();

        _employeeService = new EmployeeService(_validator, _repositoryMock.Object);
    }

    [Fact]
    public async Task AddEmployeeAsync_ShouldReturnError_WhenValidationFails()
    {
        // Arrange
        var employee = new Employee(
            firstName: "",
            surname: "teste",
            birthday: new DateTime(1990, 1, 1),
            active: true,
            salary: 50000m,
            employeeRole: new EmployeeRole("7")
        );

        // Act
        var result = await _employeeService.AddEmployeeAsync(employee);

        // Assert
        Assert.NotEmpty(result.Erros);
    }

    [Fact]
    public async Task AddEmployeeAsync_ShouldAddEmployee_WhenValidationPasses()
    {
        // Arrange
        var employee = new Employee(
            firstName: "Teste",
            surname: "Teste",
            birthday: new DateTime(1990, 1, 1),
            active: true,
            salary: 50000m,
            employeeRole: new EmployeeRole("7")
        );

        _repositoryMock.Setup(repo => repo.AddAsync(employee)).ReturnsAsync(employee);

        // Act
        var result = await _employeeService.AddEmployeeAsync(employee);

        // Assert
        _repositoryMock.Verify(repo => repo.AddAsync(employee), Times.Once);
        Assert.Null(result.Erros);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldReturnError_WhenValidationFails()
    {
        // Arrange
        var employee = new Employee(
            firstName: "",
            surname: "teste",
            birthday: new DateTime(1990, 1, 1),
            active: true,
            salary: 50000m,
            employeeRole: new EmployeeRole("7")
        );
        employee.Id = "1";
        _repositoryMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(employee);

        // Act
        var result = await _employeeService.UpdateEmployeeAsync("1", employee);

        // Assert
        Assert.NotEmpty(result.Erros);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdateEmployee_WhenValidationPasses()
    {
        // Arrange
        var employee = new Employee(
            firstName: "Teste",
            surname: "Teste",
            birthday: new DateTime(1990, 1, 1),
            active: true,
            salary: 50000m,
            employeeRole: new EmployeeRole("7")
        );
        employee.Id = "1";
        _repositoryMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(employee);

        // Act
        var result = await _employeeService.UpdateEmployeeAsync("1", employee);

        // Assert
        _repositoryMock.Verify(repo => repo.UpdateAsync(employee), Times.Once);
        Assert.Null(result.Erros);
    }

    [Fact]
    public async Task DeleteEmployeeAsync_ShouldDeleteEmployee()
    {
        // Arrange
        _repositoryMock.Setup(repo => repo.DeleteAsync("1")).Returns(Task.CompletedTask);

        // Act
        await _employeeService.DeleteEmployeeAsync("1");

        // Assert
        _repositoryMock.Verify(repo => repo.DeleteAsync("1"), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeAsync_ShouldReturnEmployee()
    {
        // Arrange
        var employee = new Employee(
            firstName: "Teste",
            surname: "Teste",
            birthday: new DateTime(1990, 1, 1),
            active: true,
            salary: 50000m,
            employeeRole: new EmployeeRole("7")
        );
        employee.Id = "1";
        _repositoryMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(employee);

        // Act
        var result = await _employeeService.GetEmployeeAsync("1");

        // Assert
        Assert.Equal(employee, result);
    }

    [Fact]
    public async Task ListEmployeesAsync_ShouldReturnEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee(
                firstName: "Teste",
                surname: "Teste",
                birthday: new DateTime(1990, 1, 1),
                active: true,
                salary: 50000m,
                employeeRole: new EmployeeRole("7")
            ),
            new Employee(
                firstName: "Teste2",
                surname: "Teste2",
                birthday: new DateTime(1990, 1, 1),
                active: true,
                salary: 60000m,
                employeeRole: new EmployeeRole("8")
            )
        };

        _repositoryMock.Setup(x => x.FindAllAsync()).ReturnsAsync(employees);

        // Act
        var result = await _employeeService.ListEmployeeAsync();

        // Assert
        Assert.Equal(employees, result);
    }

    [Fact]
    public async Task ListEmployeesAsync_ShouldReturnEmptyList_WhenNoEmployeesFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.FindAllAsync()).ReturnsAsync(new List<Employee>());

        // Act
        var result = await _employeeService.ListEmployeeAsync();

        // Assert
        Assert.Empty(result);
    }
}