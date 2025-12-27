using Service.Models.Booking;
using Service.Validators;

namespace Service.Tests.Validators;

public class UpdateBookingValidatorTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly UpdateBookingValidator _validator;

    public UpdateBookingValidatorTests()
    {
        _validator = new UpdateBookingValidator();
    }

    [Fact]
    public void Id_Empty_ShouldFail()
    {
        // Arrange
        var request = new BookingDto
        {
            Id = Guid.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Id", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Name_Null_ShouldFail()
    {
        // Arrange
        var request = new BookingDto
        {
            Id = _mockId,
            Name = null
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Name", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Name_Empty_ShouldFail()
    {
        // Arrange
        var request = new BookingDto
        {
            Id = _mockId,
            Name = string.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Name", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void BookingDate_MinValue_ShouldFail()
    {
        // Arrange
        var request = new BookingDto
        {
            Id = _mockId,
            Name = "name",
            BookingDate = DateTime.MinValue
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Booking Date", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void BookingDate_LessThanNow_ShouldFail()
    {
        // Arrange
        var request = new BookingDto
        {
            Id = _mockId,
            Name = "name",
            BookingDate = DateTime.Now.AddDays(-1)
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
            Id = _mockId,
            Name = "name",
            BookingDate = DateTime.Now,
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
            Id = _mockId,
            Name = "name",
            BookingDate = DateTime.Now,
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
    public void BookingDto_Valid()
    {
        // Arrange
        var request = new BookingDto
        {
            Id = _mockId,
            Name = "name",
            BookingDate = DateTime.Now,
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
