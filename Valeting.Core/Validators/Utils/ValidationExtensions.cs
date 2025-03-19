using FluentValidation;

namespace Valeting.Core.Validators.Utils;

public static class ValidationExtensions
{
    public static void ValidateRequest<T>(this T request, IValidator<T> validator)
    {
        var result = validator.Validate(request);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}
