using FluentValidation;
using StaffManagementSystem.Api.Domain.DTOs;

namespace StaffManagementSystem.Api.Services.Vaidations
{
    public class AdminCreateDtoValidation : AbstractValidator<CreateAdminDto>
    {
        public AdminCreateDtoValidation() 
        {
            RuleFor(a => a.Firstname)
                .NotNull()
                .NotEmpty()
                .WithMessage("Please provide valid firstname");

            RuleFor(a => a.Lastname)
                .NotNull()
                .NotEmpty()
                .WithMessage("Please provide valid lastname");

            RuleFor(a => a.Email)
                .NotEmpty()
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Please provide valid email");

            RuleFor(a => a.Password)
                .NotEmpty()
                .Must(p => p.Length >= 8)
                .WithMessage("Password must conatin at leastr 8 characters");
        }
    }
}
