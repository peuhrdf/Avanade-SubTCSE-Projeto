using Avanade.SubTCSE.Projeto.Domain.Aggregates.Employee.Entities;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.Employee.Interfaces.Repositories;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.Employee.Services;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.Employee.Validators;
using Avanade.SubTCSE.Projeto.Domain.Aggregates.EmployeeRole.Entities;
using Moq;

namespace Avanade.SubTCSE.Projeto.Domain.Tests;

public class EmployeeServiceTests
{
    private readonly EmployeeValidator _validator;
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _validator =  new EmployeeValidator();
        _repositoryMock = new Mock<IEmployeeRepository>();
        _employeeService = new EmployeeService(_validator, _repositoryMock.Object);
    }

    [Fact]
    public async Task AddEmployeeAsync_ShouldReturnErros_WhenValidationFails()
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


}