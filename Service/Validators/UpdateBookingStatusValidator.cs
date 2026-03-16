using System.Data;
using Common.Enums;
using FluentValidation;
using Service.Models.Booking.Payload;

namespace Service.Validators;

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
