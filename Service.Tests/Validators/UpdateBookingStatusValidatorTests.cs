using Common.Enums;
using Service.Models.Booking.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class UpdateBookingStatusValidatorTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly UpdateBookingStatusValidator _validator;

    public UpdateBookingStatusValidatorTests()
    {
        _validator = new UpdateBookingStatusValidator();
    }

    [Fact]
    public void Id_Empty_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingStatusDtoRequest
        {
            Id = Guid.Empty,
            UserDto = new()
            {
                Role = new()
                {
                    Code = RoleEnum.ADMIN
                }
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Id", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Status_Invalid_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingStatusDtoRequest
        {
            Id = _mockId,
            Status = (StatusEnum)999,
            UserDto = new()
            {
                Role = new()
                {
                    Code = RoleEnum.ADMIN
                }
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Status", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void UserRole_Invalid_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingStatusDtoRequest
        {
            Id = _mockId,
            Status = StatusEnum.APPROVED,
            UserDto = new()
            {
                Role = new()
                {
                    Code = (RoleEnum)999
                }
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Role", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void UserRole_NotAllowedToSetStatus_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingStatusDtoRequest
        {
            Id = _mockId,
            Status = StatusEnum.APPROVED,
            UserDto = new()
            {
                Role = new()
                {
                    Code = RoleEnum.USER
                }
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("User is not allowed to set this booking status.", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void NotAllowedToSetCompletedStatus_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingStatusDtoRequest
        {
            Id = _mockId,
            Status = StatusEnum.COMPLETED,
            UserDto = new()
            {
                Role = new()
                {
                    Code = RoleEnum.ADMIN
                }
            },
            CurrentStatus = StatusEnum.APPROVED
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("User is not allowed to set this booking status.", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void CurrentStatus_Invalid_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingStatusDtoRequest
        {
            Id = _mockId,
            Status = StatusEnum.APPROVED,
            UserDto = new()
            {
                Role = new()
                {
                    Code = RoleEnum.ADMIN
                }
            },
            CurrentStatus = (StatusEnum)999
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Current Status", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData(StatusEnum.APPROVED)]
    [InlineData(StatusEnum.REJECTED)]
    [InlineData(StatusEnum.CANCELLED)]
    [InlineData(StatusEnum.COMPLETED)]
    public void CurrentStatus_NotAllowed_ShouldFail(StatusEnum currentStatus)
    {
        // Arrange
        var request = new UpdateBookingStatusDtoRequest
        {
            Id = _mockId,
            Status = StatusEnum.APPROVED,
            UserDto = new()
            {
                Role = new()
                {
                    Code = RoleEnum.ADMIN
                }
            },
            CurrentStatus = currentStatus
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Current Status", result.Errors.FirstOrDefault().ErrorMessage);
    }
}
