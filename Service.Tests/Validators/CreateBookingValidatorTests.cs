using Service.Models.Booking;
using Service.Validators;

namespace Service.Tests.Validators;

public class CreateBookingValidatorTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly CreateBookingValidator _validator;

    public CreateBookingValidatorTests()
    {
        _validator = new CreateBookingValidator();
    }

    [Fact]
    public void ScheduledAt_MinValue_ShouldFail()
    {
        // Arrange
        var request = new BookingDto
        {
            ScheduledAt = DateTime.MinValue
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Booking Date", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void ScheduledAt_LessThanNow_ShouldFail()
    {
        // Arrange
        var request = new BookingDto
        {
            ScheduledAt = DateTime.Now.AddDays(-1)
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Booking Date", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void FlexibilityId_Empty_ShouldFail()
    {
        // Arrange
        var request = new BookingDto
        {
            ScheduledAt = DateTime.Now,
            Flexibility = new()
            {
                Id = Guid.Empty
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Flexibility", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void VehicleSizeId_Empty_ShouldFail()
    {
        // Arrange
        var request = new BookingDto
        {
            ScheduledAt = DateTime.Now,
            Flexibility = new()
            {
                Id = _mockId
            },
            VehicleSize = new()
            {
                Id = Guid.Empty
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Vehicle Size", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void CreateBookingRequest_Valid()
    {
        // Arrange
        var request = new BookingDto
        {
            ScheduledAt = DateTime.Now,
            Flexibility = new()
            {
                Id = _mockId
            },
            VehicleSize = new()
            {
                Id = _mockId
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
