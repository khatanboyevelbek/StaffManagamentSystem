using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Api.Domain.DTOs;
using StaffManagementSystem.Api.Infrastructure.IRepositories;
using RESTFulSense.Controllers;
using FluentValidation;
using FluentValidation.Results;
using StaffManagementSystem.Api.Domain.Exceptions;
using StaffManagementSystem.Api.Domain.Entities;
using StaffManagementSystem.Api.Services.Security;
using StaffManagementSystem.Api.Domain.Enums;

namespace StaffManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : RESTFulController
    {
        private readonly IAdminRepository adminRepository;
        private readonly IValidator<CreateAdminDto> createValidator;
        private readonly IValidator<UpdateAdminDto> updateValidator;
        private readonly IPasswordSecurity passwordSecurity;

        public AdminController(IAdminRepository adminRepository,
            IValidator<CreateAdminDto> createValidator,
            IPasswordSecurity passwordSecurity,
            IValidator<UpdateAdminDto> updateValidator)
        {
            this.adminRepository = adminRepository;
            this.createValidator = createValidator;
            this.passwordSecurity = passwordSecurity;
            this.updateValidator = updateValidator;
        }

        [AllowAnonymous]
        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPut]
        public async Task<ActionResult> UpdateAdminAsync(UpdateAdminDto updateAdminDto)
        {
            try
            {
                if (updateAdminDto is null)
                {
                    throw new ArgumentNullException(nameof(updateAdminDto));
                }

                ValidationResult validationResult = await this.updateValidator.ValidateAsync(updateAdminDto);
                var invalidException = new InvalidException("Validation error occured");

                foreach (var error in validationResult.Errors)
                {
                    invalidException.UpsertDataList(error.PropertyName, error.ErrorMessage);
                }

                invalidException.ThrowIfContainsErrors();

                var existingAdmin = await this.adminRepository.GetByIdAsync(updateAdminDto.Id);

                existingAdmin.Firstname = updateAdminDto.Firstname;
                existingAdmin.Lastname = updateAdminDto.Lastname;
                existingAdmin.Email = updateAdminDto.Email;
                existingAdmin.Password = this.passwordSecurity.Encrypt(updateAdminDto.Password);

                return NoContent();
            }
            catch (ArgumentNullException exception)
            {
                return BadRequest(exception);
            }
            catch (InvalidException exception)
            {
                return BadRequest(exception);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [AllowAnonymous]
        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpGet]

        public async Task<ActionResult> GetAdminById(Guid id)
        {
            try
            {
                var entity = await this.adminRepository.GetByIdAsync(id);

                if (entity is null)
                {
                    return NotFound();
                }

                return Ok(entity);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
