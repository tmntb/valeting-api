using FluentValidation;
using Service.Models.User.Payload;

namespace Service.Validators.User;

/// <summary>
/// Validator for updating a user's administrative settings. 
/// This class defines the validation rules for the properties of the <see cref="UpdateAdminSettingsDtoRequest"/> when updating a user's administrative settings. 
/// It ensures that the UserId is valid and not the same as the AdminId, and that if a RoleId is provided, it is not an empty GUID.
/// </summary>
public class UpdateAdminSettingsValidator : AbstractValidator<UpdateAdminSettingsDtoRequest>
{
    public UpdateAdminSettingsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty)
            .NotEqual(x => x.AdminId);

        RuleFor(x => x.RoleId)
            .NotEqual(Guid.Empty)
            .When(x => x.RoleId.HasValue);
    }
}
