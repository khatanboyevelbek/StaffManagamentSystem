﻿using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Api.Domain.DTOs;
using StaffManagementSystem.Api.Infrastructure.IRepositories;

namespace StaffManagementSystem.Api.Services.Vaidations
{
    public class AdminCreateDtoValidation : AbstractValidator<CreateAdminDto>
    {
        public AdminCreateDtoValidation(IAdminRepository adminRepository) 
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
                .WithMessage("Password must conatin at least 8 characters");
        }
    }
}
