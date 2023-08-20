using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Api.Domain.DTOs;
using StaffManagementSystem.Api.Infrastructure.IRepositories;
using StaffManagementSystem.Api.Infrastructure.Repositories;

namespace StaffManagementSystem.Api.Services.Vaidations
{
    public class UserCreateDtoValidation : AbstractValidator<CreateUserDto>
    {
        public UserCreateDtoValidation(IUserRepository userRepository) 
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
