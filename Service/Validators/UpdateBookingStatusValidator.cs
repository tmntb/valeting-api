using Common.Enums;
using FluentValidation;
using Service.Models.Booking.Payload;

namespace Service.Validators;

/// <summary>
/// Validator for updating booking status requests. This class defines the validation rules for the properties of the <see cref="UpdateBookingStatusDtoRequest"/> when updating the status of a booking. It ensures that the Id is a valid GUID, the Status is a valid enumeration value, and that the user's role allows them to set the specified booking status. Additionally, it validates that the CurrentStatus is a valid enumeration value and is not set to certain statuses that are not allowed for updates. This validation is essential for maintaining data integrity and enforcing business rules related to booking status updates based on user roles.
/// </summary>
public class UpdateBookingStatusValidator : AbstractValidator<UpdateBookingStatusDtoRequest>
{
    public UpdateBookingStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.UserDto.Role.Code)
            .IsInEnum();

        RuleFor(x => x)
            .Must(BeValidStatusForRole)
            .WithMessage("User is not allowed to set this booking status.");

        RuleFor(x => x.CurrentStatus)
            .IsInEnum()
            .NotEqual(StatusEnum.APPROVED)
            .NotEqual(StatusEnum.REJECTED)
            .NotEqual(StatusEnum.CANCELLED)
            .NotEqual(StatusEnum.COMPLETED);
    }

    private bool BeValidStatusForRole(UpdateBookingStatusDtoRequest updateBookingStatusDtoRequest)
    {
        if (updateBookingStatusDtoRequest.Status == StatusEnum.COMPLETED)
            return false;

        return updateBookingStatusDtoRequest.UserDto.Role.Code switch
        {
            RoleEnum.ADMIN => updateBookingStatusDtoRequest.Status is StatusEnum.APPROVED or StatusEnum.REJECTED or StatusEnum.CANCELLED,
            RoleEnum.USER => updateBookingStatusDtoRequest.Status is StatusEnum.CANCELLED,
            _ => false
        };
    }
}
