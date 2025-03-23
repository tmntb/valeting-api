using AutoMapper;
using Valeting.API.Mappers;
using Valeting.API.Models.Booking;
using Valeting.API.Models.Flexibility;
using Valeting.API.Models.VehicleSize;
using Valeting.Common.Models.Booking;
using Valeting.Common.Models.Flexibility;
using Valeting.Common.Models.VehicleSize;
using Valeting.Repository.Entities;

namespace Valeting.Tests.API.Mappers;

public class BookingMapperTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000000");

    private readonly IMapper _mapper;

    public BookingMapperTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<BookingMapper>());
        _mapper = config.CreateMapper();
    }

    #region Api -> Dto
    [Fact]
    public void Should_Map_FlexibilityApi_To_FlexibilityDto()
    {
        // Arrange
        var source = new FlexibilityApi 
        {
            Id = _mockId,
            Description = "descrption",
            Active = true
        };

        // Act
        var result = _mapper.Map<FlexibilityDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Description, result.Description);
        Assert.Equal(source.Active, result.Active);
    }

    [Fact]
    public void Should_Map_VehicleSizeApi_To_VehicleSizeDto()
    {
        // Arrange
        var source = new VehicleSizeApi 
        {
            Id = _mockId,
            Description = "descrption",
            Active = true
        };

        // Act
        var result = _mapper.Map<VehicleSizeDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Description, result.Description);
        Assert.Equal(source.Active, result.Active);
    }

    [Fact]
    public void Should_Map_CreateBookingApiRequest_To_CreateBookingDtoRequest()
    {
        // Arrange
        var source = new CreateBookingApiRequest
        {
            Name = "name",
            BookingDate = DateTime.Now,
            Flexibility = new()
            {
                Id = _mockId,
                Description = "descrption",
                Active = true
            }, 
            VehicleSize = new()
            {
                Id = _mockId,
                Description = "descrption",
                Active = true
            },
            ContactNumber = 123,
            Email = "email@email.com"
        };

        // Act
        var result = _mapper.Map<CreateBookingDtoRequest>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.BookingDate, result.BookingDate);
        Assert.Equal(source.Flexibility.Id, result.Flexibility.Id);
        Assert.Equal(source.Flexibility.Description, result.Flexibility.Description);
        Assert.Equal(source.Flexibility.Active, result.Flexibility.Active);
        Assert.Equal(source.VehicleSize.Id, result.VehicleSize.Id);
        Assert.Equal(source.VehicleSize.Description, result.VehicleSize.Description);
        Assert.Equal(source.VehicleSize.Active, result.VehicleSize.Active);
        Assert.Equal(source.ContactNumber, result.ContactNumber);
        Assert.Equal(source.Email, result.Email);
    }

    [Fact]
    public void Should_Map_UpdateBookingApiRequest_To_UpdateBookingDtoRequest()
    {
        // Arrange
        var source = new UpdateBookingApiRequest
        {
            Name = "name",
            BookingDate = DateTime.Now,
            Flexibility = new()
            {
                Id = _mockId,
                Description = "descrption",
                Active = true
            },
            VehicleSize = new()
            {
                Id = _mockId,
                Description = "descrption",
                Active = true
            },
            ContactNumber = 123,
            Email = "email@email.com",
            Approved = false
        };

        // Act
        var result = _mapper.Map<UpdateBookingDtoRequest>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.BookingDate, result.BookingDate);
        Assert.Equal(source.Flexibility.Id, result.Flexibility.Id);
        Assert.Equal(source.Flexibility.Description, result.Flexibility.Description);
        Assert.Equal(source.Flexibility.Active, result.Flexibility.Active);
        Assert.Equal(source.VehicleSize.Id, result.VehicleSize.Id);
        Assert.Equal(source.VehicleSize.Description, result.VehicleSize.Description);
        Assert.Equal(source.VehicleSize.Active, result.VehicleSize.Active);
        Assert.Equal(source.ContactNumber, result.ContactNumber);
        Assert.Equal(source.Email, result.Email);
        Assert.Equal(source.Approved, result.Approved);
    }

    [Fact]
    public void Should_Map_BookingApiParameters_To_PaginatedBookingDtoRequest()
    {
        // Arrange
        var source = new BookingApiParameters 
        { 
            PageNumber = 1, 
            PageSize = 10 
        };

        // Act
        var result = _mapper.Map<PaginatedBookingDtoRequest>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.PageNumber, result.Filter.PageNumber);
        Assert.Equal(source.PageSize, result.Filter.PageSize);
    }

    [Fact]
    public void Should_Map_BookingApiParameters_To_BookingFilterDto()
    {
        // Arrange
        var source = new BookingApiParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = _mapper.Map<BookingFilterDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.PageNumber, result.PageNumber);
        Assert.Equal(source.PageSize, result.PageSize);
    }
    #endregion

    #region Dto -> Dto
    [Fact]
    public void Should_Map_CreateBookingDtoRequest_To_BookingDto()
    {
        // Arrange
        var source = new CreateBookingDtoRequest
        {
            Name = "name",
            BookingDate = DateTime.Now,
            Flexibility = new()
            {
                Id = _mockId
            },
            VehicleSize = new()
            {
                Id = _mockId
            },
            ContactNumber = 123,
            Email = "email@email.com"
        };

        // Act
        var result = _mapper.Map<BookingDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.BookingDate, result.BookingDate);
        Assert.Equal(source.Flexibility.Id, result.Flexibility.Id);
        Assert.Equal(source.VehicleSize.Id, result.VehicleSize.Id);
        Assert.Equal(source.ContactNumber, result.ContactNumber);
        Assert.Equal(source.Email, result.Email);
        Assert.False(result.Approved);
    }

    [Fact]
    public void Should_Map_UpdateBookingDtoRequest_To_BookingDto()
    {
        // Arrange
        var source = new UpdateBookingDtoRequest
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
            },
            ContactNumber = 123,
            Email = "email@email.com",
            Approved = true
        };

        // Act
        var result = _mapper.Map<BookingDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.BookingDate, result.BookingDate);
        Assert.Equal(source.Flexibility.Id, result.Flexibility.Id);
        Assert.Equal(source.VehicleSize.Id, result.VehicleSize.Id);
        Assert.Equal(source.ContactNumber, result.ContactNumber);
        Assert.Equal(source.Email, result.Email);
        Assert.True(result.Approved);
    }
    #endregion

    #region Dto -> Entity
    [Fact]
    public void Should_Map_FlexibilityDto_To_RdFlexibility()
    {
        // Arrange
        var source = new FlexibilityDto 
        {
            Id = _mockId,
            Description = "descrption",
            Active = true
        };

        // Act
        var result = _mapper.Map<RdFlexibility>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Description, result.Description);
        Assert.Equal(source.Active, result.Active);
    }

    [Fact]
    public void Should_Map_VehicleSizeDto_To_RdVehicleSize()
    {
        // Arrange
        var source = new VehicleSizeDto 
        {
            Id = _mockId,
            Description = "descrption",
            Active = true
        };

        // Act
        var result = _mapper.Map<RdVehicleSize>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Description, result.Description);
        Assert.Equal(source.Active, result.Active);
    }

    [Fact]
    public void Should_Map_BookingDto_To_Booking()
    {
        // Arrange
        var source = new BookingDto
        {
            Id = _mockId,
            Name = "name",
            BookingDate = DateTime.Now,
            ContactNumber = 123,
            Email = "email@email.com",
            Approved = true
        };

        // Act
        var result = _mapper.Map<Booking>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.BookingDate, result.BookingDate);
        Assert.Equal(source.ContactNumber, result.ContactNumber);
        Assert.Equal(source.Email, result.Email);
        Assert.Equal(source.Approved, result.Approved);
    }
    #endregion

    #region Entity -> Dto
    [Fact]
    public void Should_Map_Booking_To_BookingDto()
    {
        // Arrange
        var source = new Booking 
        {
            Id = _mockId,
            Name = "name",
            BookingDate = DateTime.Now,
            ContactNumber = 123,
            Email = "email@email.com",
            Approved = true
        };

        // Act
        var result = _mapper.Map<BookingDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.BookingDate, result.BookingDate);
        Assert.Equal(source.ContactNumber, result.ContactNumber);
        Assert.Equal(source.Email, result.Email);
        Assert.Equal(source.Approved, result.Approved);
    }
    #endregion

    #region Dto -> Api
    [Fact]
    public void Should_Map_BookingDto_To_BookingApi()
    {
        // Arrange
        var source = new BookingDto 
        { 
            Id = _mockId,
            Name = "name",
            BookingDate = DateTime.Now,
            ContactNumber = 123,
            Email = "email@email.com",
            Approved = false
        };

        // Act
        var result = _mapper.Map<BookingApi>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.BookingDate, result.BookingDate);
        Assert.Equal(source.Email, result.Email);
        Assert.Equal(source.Approved, result.Approved);
    }

    [Fact]
    public void Should_Map_CreateBookingDtoResponse_To_CreateBookingApiResponse()
    {
        // Arrange
        var source = new CreateBookingDtoResponse
        {
            Id = _mockId
        };

        // Act
        var result = _mapper.Map<CreateBookingApiResponse>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
    }
    #endregion
}
