using Valeting.Repository.Entities;
using Valeting.Repository.Repositories;

namespace Integration.Tests.Repository;

public class BookingRepositoryTests : BaseRepositoryTest
{

    private readonly BookingRepository _bookingRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000011");

    public BookingRepositoryTests()
    {

        _bookingRepository = new BookingRepository(Context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddBookingToDatabase()
    {
        // Act
        await _bookingRepository.CreateAsync(
            new()
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
                Name = "name",
                BookingDate = DateTime.UtcNow,
                ContactNumber = 123,
                Email = "email",
                Approved = false,
                Flexibility = new()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000021")
                },
                VehicleSize = new()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000031")
                }
            });

        var result = await Context.Bookings.FindAsync(_mockId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBookingInDatabase()
    {
        // Act
        await _bookingRepository.UpdateAsync(
            new()
            {
                Id = _mockId,
                Name = "name1",
                BookingDate = DateTime.UtcNow,
                ContactNumber = 123,
                Email = "email",
                Approved = false,
                Flexibility = new()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
                    Description = "1 Day"
                },
                VehicleSize = new()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000031"),
                    Description = "Van"
                }
            });

        var result = await Context.Bookings.FindAsync(_mockId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBookingFromDatabase()
    {
        // Act
        await _bookingRepository.DeleteAsync(_mockId);
        var result = await Context.Bookings.FindAsync(_mockId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookingDoesNotExists()
    {
        // Act
        var result = await _bookingRepository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000013"));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBookingDtoWhenBookingExists()
    {
        // Act
        var result = await _bookingRepository.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredList()
    {
        // Arrange
        var existingFlex = Context.RdFlexibilities.First();
        var existingVehicle = Context.RdVehicleSizes.First();

        var booking = new Booking
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000014"),
            Name = "name",
            BookingDate = DateTime.UtcNow,
            ContactNumber = 123,
            Email = "email",
            Approved = false,
            Flexibility = existingFlex,
            VehicleSize = existingVehicle
        };

        Context.Bookings.Add(booking);
        await Context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetFilteredAsync(new());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}
