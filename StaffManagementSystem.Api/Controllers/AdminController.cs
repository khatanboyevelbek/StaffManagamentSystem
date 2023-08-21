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
using StaffManagementSystem.Api.Services.EmailService;

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
        private readonly IEmailSender emailSender;

        public AdminController(IAdminRepository adminRepository,
            IValidator<CreateAdminDto> createValidator,
            IPasswordSecurity passwordSecurity,
            IValidator<UpdateAdminDto> updateValidator,
            IEmailSender emailSender)
        {
            this.adminRepository = adminRepository;
            this.createValidator = createValidator;
            this.passwordSecurity = passwordSecurity;
            this.updateValidator = updateValidator;
            this.emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> InsertAdminAsync(CreateAdminDto createAdminDto)
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
                    Password = createAdminDto.Password,
                    Role = Roles.Admin
                };

                var entity = await this.adminRepository.InsertAsync(admin);

                string actionMessage = $@"
                       <div>
                           <p>Iltmos bu ma'lumotlarni hech kimga bermang!</p>
                           <p><b>Email: {entity.Email}</b></p>
                           <p><b>Parol: {createAdminDto.Password}</b></p>
                           <P>---Tizimga admin sifatida kirishingiz mumkin---</p>
                       </div>";

                this.emailSender.SendEmail(createAdminDto.Email, string.Empty, actionMessage);

                return Created(entity);
            }
            catch (Exception)
            {

                throw;
            }
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

                await this.adminRepository.UpdateAsync(existingAdmin);

                string actionMessage = $@"
                       <div>
                           <p>Iltmos bu ma'lumotlarni hech kimga bermang!</p>
                           <p><b>Email: {updateAdminDto.Email}</b></p>
                           <p><b>Yangilangan parol: {updateAdminDto.Password}</b></p>
                           <P>---Tizimga admin sifatida kirishingiz mumkin---</p>
                       </div>";

                this.emailSender.SendEmail(updateAdminDto.Email, string.Empty, actionMessage);

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
