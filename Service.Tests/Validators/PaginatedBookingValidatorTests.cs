using Common.Enums;
using Service.Models.Booking.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class PaginatedBookingValidatorTests
{
    private readonly PaginatedBookingValidator _validator;

    public PaginatedBookingValidatorTests()
    {
        _validator = new PaginatedBookingValidator();
    }

    [Fact]
    public void Status_InvalidEnumValue_ShouldFail()
    {
        // Arrange
        var request = new BookingFilterDto
        {
            PageNumber = 1,
            PageSize = 10,
            Status = (StatusEnum)999 // Invalid enum value
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Status", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void PaginatedBookingDtoRequest_Valid()
    {
        // Arrange
        var request = new BookingFilterDto
        {
            PageNumber = 1,
            PageSize = 1,
            Status = StatusEnum.APPROVED
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
