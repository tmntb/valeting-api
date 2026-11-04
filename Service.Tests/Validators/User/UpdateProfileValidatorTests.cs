using Service.Models.User;
using Service.Models.User.Payload;
using Service.Validators.User;

namespace Service.Tests.Validators.User;

public class UpdateProfileValidatorTests
{
    private readonly UserDto _userDto;
    private readonly UpdateProfileValidator _validator;

    public UpdateProfileValidatorTests()
    {
        _userDto = DataFactory.CreateUserDto();
        _validator = new UpdateProfileValidator();
    }

    [Fact]
    public void Id_ShouldFail()
    {
        // Arrange
        var request = new UpdateProfileDtoRequest
        {
            Id = Guid.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Id", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ThisFirstNameIsTooWayLongAndMustShouldFailValidation")]
    public void FirstName_ShouldFail(string firstName)
    {
        // Arrange
        var request = new UpdateProfileDtoRequest
        {
            Id = _userDto.Id,
            FirstName = firstName
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("First Name", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ThisLastNameIsTooWayLongAndMustShouldFailValidation")]
    public void LastName_ShouldFail(string lastName)
    {
        // Arrange
        var request = new UpdateProfileDtoRequest
        {
            Id = _userDto.Id,
            FirstName = "firstName",
            LastName = lastName
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Last Name", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void DateOfBirth_ShouldFail()
    {
        // Arrange
        var request = new UpdateProfileDtoRequest
        {
            Id = _userDto.Id,
            FirstName = "firstName",
            LastName = "lastName",
            DateOfBirth = DateOnly.MinValue
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Date Of Birth", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void ContactNumber_LengthNot9_ShouldFail()
    {
        // Arrange
        var request = new UpdateProfileDtoRequest
        {
            Id = _userDto.Id,
            FirstName = "firstName",
            LastName = "lastName",
            DateOfBirth = new DateOnly(1956, 10, 25),
            ContactNumber = 12345678
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Contact Number", result.Errors.FirstOrDefault().ErrorMessage);
    }
}
