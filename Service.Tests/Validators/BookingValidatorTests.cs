using Common.Models.Booking;
using Service.Validators;

namespace Service.Tests.Validators;

#region CreateBookingValidatorTests
public class CreateBookingValidatorTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly CreateBookingValidator _validator;

    public CreateBookingValidatorTests()
    {
        _validator = new CreateBookingValidator();
    }

    [Fact]
    public void Name_Null_ShouldFail()
    {
        // Arrange
        var request = new CreateBookingDtoRequest
        {
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
        var request = new CreateBookingDtoRequest
        {
            Name = string.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Name", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Email_Null_ShouldFail()
    {
        // Arrange
        var request = new CreateBookingDtoRequest
        {
            Name = "name",
            Email = null
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Email_Empty_ShouldFail()
    {
        // Arrange
        var request = new CreateBookingDtoRequest
        {
            Name = "name",
            Email = string.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void ContactNumber_Null_ShouldFail()
    {
        // Arrange
        var request = new CreateBookingDtoRequest
        {
            Name = "name",
            Email = "email",
            ContactNumber = null
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Contact Number", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void BookingDate_MinValue_ShouldFail()
    {
        // Arrange
        var request = new CreateBookingDtoRequest
        {
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
        var request = new CreateBookingDtoRequest
        {
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
        var request = new CreateBookingDtoRequest
        {
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
        var request = new CreateBookingDtoRequest
        {
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
    public void CreateBookingRequest_Valid()
    {
        // Arrange
        var request = new CreateBookingDtoRequest
        {
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
#endregion

#region UpdateBookingValidatorTests
public class UpdateBookingValidatorTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly UpdateBookinValidator _validator;

    public UpdateBookingValidatorTests()
    {
        _validator = new UpdateBookinValidator();
    }

    [Fact]
    public void Id_Empty_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingDtoRequest
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
        var request = new UpdateBookingDtoRequest
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
        var request = new UpdateBookingDtoRequest
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
    public void Email_Null_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingDtoRequest
        {
            Id = _mockId,
            Name = "name",
            Email = null
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Email_Empty_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingDtoRequest
        {
            Id = _mockId,
            Name = "name",
            Email = string.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void ContactNumber_Null_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingDtoRequest
        {
            Id = _mockId,
            Name = "name",
            Email = "email",
            ContactNumber = null
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Contact Number", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void BookingDate_MinValue_ShouldFail()
    {
        // Arrange
        var request = new UpdateBookingDtoRequest
        {
            Id = _mockId,
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
        var request = new UpdateBookingDtoRequest
        {
            Id = _mockId,
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
        var request = new UpdateBookingDtoRequest
        {
            Id = _mockId,
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
        var request = new UpdateBookingDtoRequest
        {
            Id = _mockId,
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
    public void UpdateBookingDtoRequest_Valid()
    {
        // Arrange
        var request = new UpdateBookingDtoRequest
        {
            Id = _mockId,
            Name = "name",
            Email = "email",
            ContactNumber = 123,
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
#endregion

#region DeleteBookingValidatorTests
public class DeleteBookingValidatorTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly DeleteBookingValidator _validator;

    public DeleteBookingValidatorTests()
    {
        _validator = new DeleteBookingValidator();
    }

    [Fact]
    public void Id_Empty_ShouldFail()
    {
        // Arrange
        var request = new DeleteBookingDtoRequest
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
    public void DeleteBookingDtoRequest_Valid()
    {
        // Arrange
        var request = new DeleteBookingDtoRequest
        {
            Id = _mockId
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
#endregion

#region GetBookingValidatorTests
public class GetBookingValidatorTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly GetBookingValidator _validator;

    public GetBookingValidatorTests()
    {
        _validator = new GetBookingValidator();
    }

    [Fact]
    public void Id_Empty_ShouldFail()
    {
        // Arrange
        var request = new GetBookingDtoRequest
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
    public void GetBookingDtoRequest_Valid()
    {
        // Arrange
        var request = new GetBookingDtoRequest
        {
            Id = _mockId
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
#endregion

#region PaginatedBookingValidatorTests
public class PaginatedBookingValidatorTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly PaginatedBookingValidator _validator;

    public PaginatedBookingValidatorTests()
    {
        _validator = new PaginatedBookingValidator();
    }

    [Fact]
    public void Filter_Empty_ShouldFail()
    {
        // Arrange
        var request = new PaginatedBookingDtoRequest
        {
            Filter = null
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Filter", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void PageNumber_EqualToZero_ShouldFail()
    {
        // Arrange
        var request = new PaginatedBookingDtoRequest
        {
            Filter = new()
            {
                PageNumber = 0
            }
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
        var request = new PaginatedBookingDtoRequest
        {
            Filter = new()
            {
                PageNumber = -1
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Page Number", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void PaginatedBookingDtoRequest_Valid()
    {
        // Arrange
        var request = new PaginatedBookingDtoRequest
        {
            Filter = new()
            {
                PageNumber = 1,
                PageSize = 1
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
#endregion