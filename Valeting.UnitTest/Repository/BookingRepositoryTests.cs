using Microsoft.EntityFrameworkCore;
using Moq;
using Valeting.Common.Models.Booking;
using Valeting.Repository.Entities;
using Valeting.Repository.Repositories;

namespace Valeting.Tests.Repository;

public class BookingRepositoryTests
{

    private readonly ValetingContext _valetingContext;
    private readonly BookingRepository _bookingRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public BookingRepositoryTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<ValetingContext>()
            .UseInMemoryDatabase(databaseName: "ValetingTestDb")
            .Options;

        _valetingContext = new ValetingContext(dbContextOptions);
        _bookingRepository = new BookingRepository(_valetingContext);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddBookingToDatabase()
    {
        // Act
        await _bookingRepository.CreateAsync(
            new()
            {
                Id = _mockId,
                Name = "name",
                Email = "email"
            });

        var result = await _valetingContext.Bookings.FindAsync(_mockId);

        // Assert
        Assert.NotNull(result);

        // Clear data
        _valetingContext.Bookings.Remove(result);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBookingInDatabase()
    {
        // Arrange
        _valetingContext.Bookings.Add(
            new Booking
            {
                Id = _mockId,
                Name = "name",
                Email = "email"
            });
        await _valetingContext.SaveChangesAsync();

        // Act
        await _bookingRepository.UpdateAsync(
            new()
            {
                Id = _mockId,
                Name = "name",
                Email = "email"
            });

        var result = await _valetingContext.Bookings.FindAsync(_mockId);

        // Assert
        Assert.NotNull(result);

        // Clear data
        _valetingContext.Bookings.Remove(result);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBookingFromDatabase()
    {
        // Arrange
        _valetingContext.Bookings.Add(
            new Booking
            {
                Id = _mockId,
                Name = "name",
                Email = "email"
            });
        await _valetingContext.SaveChangesAsync();

        // Act
        await _bookingRepository.DeleteAsync(_mockId);
        var result = await _valetingContext.Bookings.FindAsync(_mockId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookingDoesNotExists()
    {
        // Act
        var result = await _bookingRepository.GetByIdAsync(_mockId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBookingDtoWhenBookingExists()
    {
        // Arrange
        _valetingContext.Bookings.Add(
            new Booking
            {
                Id = _mockId,
                Name = "name",
                Email = "email"
            });
        await _valetingContext.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);

        // Clear data
        var clearData = _valetingContext.Bookings;
        _valetingContext.Bookings.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredList()
    {
        // Arrange
        _valetingContext.Bookings.AddRange(
            new List<Booking>
            {
                new() 
                { 
                    Id = _mockId,
                    Name = "name",
                    Email= "email"
                },
                new() 
                { 
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "name",
                    Email= "email"
                }
            });
        await _valetingContext.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetFilteredAsync(new());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Clear data
        var clearData = _valetingContext.Bookings;
        _valetingContext.Bookings.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }
}
