using Repository.Entities;
using Repository.Repositories;
using Service.Models.Booking;

namespace Integration.Tests.Repository;

public class BookingRepositoryTests : BaseRepositoryTest
{
    private readonly BookingDto _bookingDto;
    private readonly BookingRepository _bookingRepository;

    public BookingRepositoryTests()
    {
        _bookingDto = DataFactory.CreateBookingDto();

        _bookingRepository = new BookingRepository(Context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddBookingToDatabase()
    {
        // Act
        await _bookingRepository.CreateAsync(_bookingDto);

        var result = await Context.Bookings.FindAsync(_bookingDto.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturn_WhenBookingDoesNotExist()
    {   
        // Act
        await _bookingRepository.UpdateAsync(_bookingDto);

        var result = await Context.Bookings.FindAsync(_bookingDto.Id);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task UpdateAsync_ShouldUpdateBookingInDatabase()
    {
        // Arrange
        var bookingDto = DataFactory.CreateBookingDto(bookingId: Guid.Parse("00000000-0000-0000-0000-000000000011"));
        
        // Act
        await _bookingRepository.UpdateAsync(bookingDto);

        var result = await Context.Bookings.FindAsync(bookingDto.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBookingFromDatabase()
    {
        // Act
        await _bookingRepository.DeleteAsync(Guid.Parse("00000000-0000-0000-0000-000000000011"));
        var result = await Context.Bookings.FindAsync(Guid.Parse("00000000-0000-0000-0000-000000000011"));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveBookingFromDatabase()
    {
        // Act
        await _bookingRepository.DeleteAsync(_bookingDto.Id);
        var result = await Context.Bookings.FindAsync(_bookingDto.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookingDoesNotExists()
    {
        // Act
        var result = await _bookingRepository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000099"));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBookingDtoWhenBookingExists()
    {
        // Act
        var result = await _bookingRepository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000011"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000011"), result.Id);
    }

    [Fact]
    public async Task GetCustomerFilteredAsync_ShouldReturnFilteredList()
    {
        // Arrange
        var existingFlex = Context.RdFlexibilities.First();
        var existingVehicle = Context.RdVehicleSizes.First();
        var existingCustomer = Context.ApplicationUsers.First();
        var existingStatus = Context.RdStatus.First();

        var booking = new Booking
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
            Reference = "name",
            Customer = existingCustomer,
            Flexibility = existingFlex,
            VehicleSize = existingVehicle,
            ScheduledAt = DateTime.UtcNow,
            Status = existingStatus,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.MinValue,
            DecisionAt = DateTime.MinValue,
            DecisionBy = null,
            RequiresApproval = true,
            Notes = "notes"
        };

        Context.Bookings.Add(booking);
        await Context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetFilteredAsync(new(){ CustomerId = existingCustomer.Id });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetStatusFilteredAsync_ShouldReturnFilteredList()
    {
        // Arrange
        var existingFlex = Context.RdFlexibilities.First();
        var existingVehicle = Context.RdVehicleSizes.First();
        var existingCustomer = Context.ApplicationUsers.First();
        var existingStatus = Context.RdStatus.First();

        var booking = new Booking
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
            Reference = "name",
            Customer = existingCustomer,
            Flexibility = existingFlex,
            VehicleSize = existingVehicle,
            ScheduledAt = DateTime.UtcNow,
            Status = existingStatus,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.MinValue,
            DecisionAt = DateTime.MinValue,
            DecisionBy = null,
            RequiresApproval = true,
            Notes = "notes"
        };

        Context.Bookings.Add(booking);
        await Context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetFilteredAsync(new(){ Status = existingStatus.Code });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}
