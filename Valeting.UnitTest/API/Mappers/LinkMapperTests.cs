using AutoMapper;
using Valeting.API.Mappers;
using Valeting.API.Models.Core;
using Valeting.Common.Models.Link;

namespace Valeting.Tests.API.Mappers;

public class LinkMapperTests
{
    private readonly IMapper _mapper;

    public LinkMapperTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<LinkMapper>());
        _mapper = config.CreateMapper();
    }

    #region Dto -> Api
    [Fact]
    public void Should_Map_GeneratePaginatedLinksDtoResponse_To_PaginationLinksApi()
    {
        // Arrange
        var source = new GeneratePaginatedLinksDtoResponse
        {
            Next = "next",
            Prev = "prev",
            Self = "self"
        };

        // Act
        var result = _mapper.Map<PaginationLinksApi>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Next, result.Next.Href);
        Assert.Equal(source.Prev, result.Prev.Href);
        Assert.Equal(source.Self, result.Self.Href);
    }
    #endregion
}
