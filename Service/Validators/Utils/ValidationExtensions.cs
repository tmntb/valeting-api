using FluentValidation;

namespace Service.Validators.Utils;

public static class ValidationExtensions
{
    /// <summary>
    /// Validates the specified request using the provided validator.
    /// Throws a <see cref="ValidationException"/> if validation fails.
    /// </summary>
    /// <typeparam name="T">The type of the request object.</typeparam>
    /// <param name="request">The request object to validate.</param>
    /// <param name="validator">The validator to use for validation.</param>
    public static void ValidateRequest<T>(this T request, IValidator<T> validator)
    {
        var result = validator.Validate(request);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}