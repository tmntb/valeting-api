using AutoMapper;
using Valeting.API.Mappers;
using Valeting.API.Models.Flexibility;
using Valeting.Common.Models.Flexibility;
using Valeting.Repository.Entities;

namespace Valeting.Tests.API.Mappers;

public class FlexibilityMapperTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000000");

    private readonly IMapper _mapper;

    public FlexibilityMapperTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FlexibilityMapper>());
        _mapper = config.CreateMapper();
    }

    #region Api -> Dto
    [Fact]
    public void Should_Map_FlexibilityApiParameters_To_PaginatedFlexibilityDtoRequest()
    {
        // Arrange
        var source = new FlexibilityApiParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Active = false
        };

        // Act
        var result = _mapper.Map<PaginatedFlexibilityDtoRequest>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.PageNumber, result.Filter.PageNumber);
        Assert.Equal(source.PageSize, result.Filter.PageSize);
        Assert.Equal(source.Active, result.Filter.Active);
    }

    [Fact]
    public void Should_Map_FlexibilityApiParameters_To_FlexibilityFilterDto()
    {
        // Arrange
        var source = new FlexibilityApiParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Active = false
        };

        // Act
        var result = _mapper.Map<FlexibilityFilterDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.PageNumber, result.PageNumber);
        Assert.Equal(source.PageSize, result.PageSize);
        Assert.Equal(source.Active, result.Active);
    }
    #endregion

    #region Entity -> Dto
    [Fact]
    public void Should_Map_RdFlexibility_To_FlexibilityDto()
    {
        // Arrange
        var source = new RdFlexibility
        {
            Id = _mockId,
            Description = "description",
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
    #endregion

    #region Dto -> Api
    [Fact]
    public void Should_Map_FlexibilityDto_To_FlexibilityApi()
    {
        // Arrange
        var source = new FlexibilityDto
        {
            Id = _mockId,
            Description = "description",
            Active = true
        };

        // Act
        var result = _mapper.Map<FlexibilityApi>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Description, result.Description);
        Assert.Equal(source.Active, result.Active);
    }
    #endregion
}
