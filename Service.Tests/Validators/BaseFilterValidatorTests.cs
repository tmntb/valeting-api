using System;
using Service.Models.Booking.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class BaseFilterValidatorTests
{
    private readonly BaseFilterValidator<BookingFilterDto> _validator;

    public BaseFilterValidatorTests()
    {
        _validator = new BaseFilterValidator<BookingFilterDto>();
    }

    [Fact]
    public void PageNumber_EqualToZero_ShouldFail()
    {
        // Arrange
        var request = new BookingFilterDto
        {
            PageNumber = 0
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Page Number", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void PageNumber_LessThanZero_ShouldFail()
    {
        // Arrange
        var request = new BookingFilterDto
        {
            PageNumber = -1
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Page Number", result.Errors.FirstOrDefault().ErrorMessage);
    }
}
