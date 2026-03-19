using Service.Models.Flexibility.Payload;

namespace Service.Validators;

/// <summary>
/// Validator for paginated flexibility requests. This class defines the validation rules for the properties of the <see cref="FlexibilityFilterDto"/> when retrieving paginated flexibility data. It inherits from the <see cref="BaseFilterValidator{T}"/> class, which provides common validation rules for pagination parameters such as page number and page size. This validator can be extended in the future to include additional validation rules specific to flexibility filtering criteria as needed.
/// </summary>
public class PaginatedFlexibilityValidator : BaseFilterValidator<FlexibilityFilterDto>
{
    public PaginatedFlexibilityValidator(){}
}