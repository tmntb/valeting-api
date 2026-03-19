using Service.Models.VehicleSize.Payload;

namespace Service.Validators;

/// <summary>
/// Validator for paginated vehicle size requests. This class defines the validation rules for the properties of the <see cref="VehicleSizeFilterDto"/> when retrieving paginated vehicle size data. It inherits from the <see cref="BaseFilterValidator{T}"/> class, which provides common validation rules for pagination parameters such as page number and page size. This validator can be extended in the future to include additional validation rules specific to vehicle size filtering criteria as needed.
/// </summary>
public class PaginatedVehicleSizeValidator : BaseFilterValidator<VehicleSizeFilterDto>
{
    public PaginatedVehicleSizeValidator() { }
}