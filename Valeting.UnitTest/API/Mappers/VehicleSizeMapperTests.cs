using AutoMapper;
using Valeting.API.Mappers;
using Valeting.API.Models.VehicleSize;
using Valeting.Common.Models.VehicleSize;
using Valeting.Repository.Entities;

namespace Valeting.Tests.API.Mappers;

public class VehicleSizeMapperTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000000");

    private readonly IMapper _mapper;

    public VehicleSizeMapperTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<VehicleSizeMapper>());
        _mapper = config.CreateMapper();
    }

    #region Api -> Dto
    [Fact]
    public void Should_Map_VehicleSizeApiParameters_To_PaginatedVehicleSizeDtoRequest()
    {
        // Arrange
        var source = new VehicleSizeApiParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Active = false
        };

        // Act
        var result = _mapper.Map<PaginatedVehicleSizeDtoRequest>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.PageNumber, result.Filter.PageNumber);
        Assert.Equal(source.PageSize, result.Filter.PageSize);
        Assert.Equal(source.Active, result.Filter.Active);
    }

    [Fact]
    public void Should_Map_VehicleSizeApiParameters_To_VehicleSizeFilterDto()
    {
        // Arrange
        var source = new VehicleSizeApiParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Active = false
        };

        // Act
        var result = _mapper.Map<VehicleSizeFilterDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.PageNumber, result.PageNumber);
        Assert.Equal(source.PageSize, result.PageSize);
        Assert.Equal(source.Active, result.Active);
    }
    #endregion

    #region Entity -> Dto
    [Fact]
    public void Should_Map_RdVehicleSize_To_VehicleSizeDto()
    {
        // Arrange
        var source = new RdVehicleSize
        {
            Id = _mockId,
            Description = "description",
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
    #endregion

    #region Dto -> Api
    [Fact]
    public void Should_Map_VehicleSizeDto_To_VehicleSizeApi()
    {
        // Arrange
        var source = new VehicleSizeDto
        {
            Id = _mockId,
            Description = "description",
            Active = true
        };

        // Act
        var result = _mapper.Map<VehicleSizeApi>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Description, result.Description);
        Assert.Equal(source.Active, result.Active);
    }
    #endregion
}
