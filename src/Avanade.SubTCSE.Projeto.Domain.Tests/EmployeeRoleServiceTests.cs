using Avanade.SubTCSE.Projeto.Domain.Aggregates.EmployeeRole.Entities;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.EmployeeRole.Interfaces.Repositories;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.EmployeeRole.Services;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.EmployeeRole.Validators;
using Avanade.SubTCSE.Projeto.Domain.Base.Repository.MongoDB;
using MongoDB.Driver;
using Moq;

namespace Avanade.SubTCSE.Projeto.Domain.Tests;

public class EmployeeRoleServiceTests
{
    private readonly EmployeeRoleValidator _validator;
    private readonly Mock<IEmployeeRoleRepository> _repositoryMock;
    private readonly EmployeeRoleService _employeeService;
    private readonly Mock<IMongoCollection<EmployeeRole>> _collectionMock;
    private readonly Mock<IMongoDBContext> _dbContextMock;

    public EmployeeRoleServiceTests()
    {
        _validator = new EmployeeRoleValidator();
        _repositoryMock = new Mock<IEmployeeRoleRepository>();
        _employeeService = new EmployeeRoleService(_validator, _repositoryMock.Object);

        _collectionMock = new Mock<IMongoCollection<EmployeeRole>>();

        _dbContextMock = new Mock<IMongoDBContext>();
        _dbContextMock.Setup(db => db.GetCollection<EmployeeRole>(It.IsAny<string>())).Returns(_collectionMock.Object);

        _repositoryMock = new Mock<IEmployeeRoleRepository>();

        _employeeService = new EmployeeRoleService(_validator, _repositoryMock.Object);
    }

    [Fact]
    public async Task AddEmployeeRoleAsync_ShouldReturnError_WhenValidationFails()
    {
        // Arrange
        var employeeRole = new EmployeeRole("");

        // Act
        var result = await _employeeService.AddEmployeeRoleAsync(employeeRole);

        // Assert
        Assert.NotEmpty(result.Erros);
    }

    [Fact]
    public async Task AddEmployeeRoleAsync_ShouldAddEmployeeRole_WhenValidationPasses()
    {
        // Arrange
        var employeeRole = new EmployeeRole("Teste");
        
        _repositoryMock.Setup(repo => repo.AddAsync(employeeRole)).ReturnsAsync(employeeRole);

        // Act
        var result = await _employeeService.AddEmployeeRoleAsync(employeeRole);

        // Assert
        _repositoryMock.Verify(repo => repo.AddAsync(employeeRole), Times.Once);
        Assert.Null(result.Erros);
    }

    [Fact]
    public async Task UpdateEmployeeRoleAsync_ShouldReturnError_WhenValidationFails()
    {
        // Arrange
        var employeeRole = new EmployeeRole("");
        
        employeeRole.Id = "1";
        _repositoryMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(employeeRole);

        // Act
        var result = await _employeeService.UpdateEmployeeRoleAsync("1", employeeRole);

        // Assert
        Assert.NotEmpty(result.Erros);
    }

    [Fact]
    public async Task UpdateEmployeeRoleAsync_ShouldUpdateEmployeeRole_WhenValidationPasses()
    {
        // Arrange
        var employeeRole = new EmployeeRole("Teste");
        employeeRole.Id = "1";
        _repositoryMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(employeeRole);

        // Act
        var result = await _employeeService.UpdateEmployeeRoleAsync("1", employeeRole);

        // Assert
        _repositoryMock.Verify(repo => repo.UpdateAsync(employeeRole), Times.Once);
        Assert.Null(result.Erros);
    }

    [Fact]
    public async Task DeleteEmployeeRoleAsync_ShouldDeleteEmployeeRole()
    {
        // Arrange
        _repositoryMock.Setup(repo => repo.DeleteAsync("1")).Returns(Task.CompletedTask);

        // Act
        await _employeeService.DeleteEmployeeRoleAsync("1");

        // Assert
        _repositoryMock.Verify(repo => repo.DeleteAsync("1"), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeRoleAsync_ShouldReturnEmployeeRole()
    {
        // Arrange
        var employeeRole = new EmployeeRole( "Teste");
        _repositoryMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(employeeRole);

        // Act
        var result = await _employeeService.GetEmployeeRoleAsync("1");

        // Assert
        Assert.Equal(employeeRole, result);
    }

    [Fact]
    public async Task ListEmployeeRolesAsync_ShouldReturnEmployeeRoles()
    {
        // Arrange
        var employees = new List<EmployeeRole>
        {
            new EmployeeRole("teste1"),
            new EmployeeRole("teste2")
        };

        _repositoryMock.Setup(x => x.FindAllAsync()).ReturnsAsync(employees);

        // Act
        var result = await _employeeService.ListEmployeeRoleAsync();

        // Assert
        Assert.Equal(employees, result);
    }

    [Fact]
    public async Task ListEmployeeRolesAsync_ShouldReturnEmptyList_WhenNoEmployeeRolesFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.FindAllAsync()).ReturnsAsync(new List<EmployeeRole>());

        // Act
        var result = await _employeeService.ListEmployeeRoleAsync();

        // Assert
        Assert.Empty(result);
    }
    
}