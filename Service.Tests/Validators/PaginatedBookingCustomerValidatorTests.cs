using System;
using Service.Models.Booking.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class PaginatedBookingCustomerValidatorTests
{
    private readonly PaginatedBookingCustomerValidator _validator;

    public PaginatedBookingCustomerValidatorTests()
    {
        _validator = new PaginatedBookingCustomerValidator();
    }

    [Fact]
    public void CustomerId_Empty_ShouldFail()
    {
        // Arrange
        var request = new BookingFilterDto
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Customer Id", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void PaginatedBookingCustomer_Valid()
    {
        // Arrange
        var request = new BookingFilterDto
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerId = Guid.Parse("00000000-0000-0000-0000-000000000006")
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
