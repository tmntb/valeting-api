using Service.Models.Link.Payload;

namespace Service.Interfaces;

public interface ILinkService
{
    /// <summary>
    /// Generates the self-referential link (URL) for a given resource.
    /// </summary>
    /// <remarks>
    /// Validates the provided request and constructs the self-link using the base URL and optional resource ID.  
    /// If the ID is not provided or is the default value, only the base URL is returned.
    /// </remarks>
    /// <param name="generateSelfLinkDtoRequest">
    /// The request object containing the current HTTP context, the resource path, and the optional resource identifier.
    /// </param>
    /// <returns> A string representing the complete self URL for the resource.</returns>
    /// <exception cref="ValidationException">Thrown when the <paramref name="generateSelfLinkDtoRequest"/> fails validation.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during URL generation.</exception>
    string GenerateSelf(GenerateSelfLinkDtoRequest generateSelfLinkDtoRequest);

    /// <summary>
    /// Generates pagination links (self, previous, and next) for a paginated API response.
    /// </summary>
    /// <remarks>
    /// Validates the provided request and constructs URLs for navigating through paginated data.
    /// The generated links include query parameters based on the provided filter and current pagination state.
    /// </remarks>
    /// <param name="generatePaginatedLinksDtoRequest">
    /// The request object containing the current HTTP context, pagination filter, and total number of pages.
    /// </param>
    /// <returns>A <see cref="GeneratePaginatedLinksDtoResponse"/> object containing URLs for the current (self), previous, and next pages.</returns>
    /// <exception cref="ValidationException">Thrown when the <paramref name="generatePaginatedLinksDtoRequest"/> fails validation.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during link generation.</exception>
    GeneratePaginatedLinksDtoResponse GeneratePaginatedLinks(GeneratePaginatedLinksDtoRequest generatePaginatedLinksDtoRequest);
}