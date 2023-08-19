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
        [HttpPost]
        [Route("register")]

        public async Task<ActionResult> RegisterAdminAsync(CreateAdminDto createAdminDto)
        {
            try
            {
                if (createAdminDto is null)
                {
                    throw new ArgumentNullException(nameof(createAdminDto));
                }

                ValidationResult validationResult = await this.createValidator.ValidateAsync(createAdminDto);
                var invalidException = new InvalidException("Validation error occured");

                foreach (var error in validationResult.Errors)
                {
                    invalidException.UpsertDataList(error.PropertyName, error.ErrorMessage);
                }

                invalidException.ThrowIfContainsErrors();

                var admin = new Admin
                {
                    Id = Guid.NewGuid(),
                    Firstname = createAdminDto.Firstname,
                    Lastname = createAdminDto.Lastname,
                    Email = createAdminDto.Email,
                    Password = this.passwordSecurity.Encrypt(createAdminDto.Password),
                    Role = Roles.Admin
                };

                var result = await this.adminRepository.InsertAsync(admin);

                return Created(result);
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
        [HttpDelete]

        public async Task<ActionResult> DeleteAdminAsync(Guid id)
        {
            try
            {
                var entity = await this.adminRepository.GetByIdAsync(id);
                await this.adminRepository.DeleteAsync(entity);

                return NoContent();
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
