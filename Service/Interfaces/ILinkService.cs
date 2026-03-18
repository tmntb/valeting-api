using Service.Models.Link.Payload;

namespace Service.Interfaces;

public interface ILinkService
{
    /// <summary>
    /// Generates the self-referential link (URL) for a given resource.
    /// </summary>
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
    /// <param name="generatePaginatedLinksDtoRequest">
    /// The request object containing the current HTTP context, pagination filter, and total number of pages.
    /// </param>
    /// <returns>A <see cref="GeneratePaginatedLinksDtoResponse"/> object containing URLs for the current (self), previous, and next pages.</returns>
    /// <exception cref="ValidationException">Thrown when the <paramref name="generatePaginatedLinksDtoRequest"/> fails validation.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during link generation.</exception>
    GeneratePaginatedLinksDtoResponse GeneratePaginatedLinks(GeneratePaginatedLinksDtoRequest generatePaginatedLinksDtoRequest);
}