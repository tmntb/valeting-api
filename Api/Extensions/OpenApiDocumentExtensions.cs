using Microsoft.OpenApi;

namespace Api.Extensions;

public static class OpenApiDocumentExtensions
{
    /// <summary>
    /// Sets custom Swagger/OpenAPI document filter for the Auth endpoints.
    /// </summary>
    /// <param name="path">The endpoint path.</param>
    /// <param name="method">The HTTP method.</param>
    /// <param name="operationId">The operation identifier.</param>
    /// <param name="summary">The operation summary.</param>
    /// <param name="description">The operation description.</param>
    /// <param name="requiresBearer">Indicates whether the operation requires a bearer token.</
    /// <returns>The open api document filled.</returns>
    internal static void ConfigureOperation(this OpenApiDocument swaggerDoc, string path, HttpMethod method, string operationId, string summary, string description, bool requiresBearer = true)
    {
        var pathItem = swaggerDoc.Paths.FirstOrDefault(x => x.Key.Contains(path)).Value;
        if (pathItem == null || !pathItem.Operations.TryGetValue(method, out var operation))
        {
            return;
        }

        operation.OperationId = operationId;
        operation.Summary = summary;
        operation.Description = description;
        if (!requiresBearer)
        {
            operation.Security = [];
        }
    }
}
