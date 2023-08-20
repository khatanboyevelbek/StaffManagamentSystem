using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTFulSense.Controllers;
using StaffManagementSystem.Api.Domain.DTOs;
using StaffManagementSystem.Api.Domain.Entities;
using StaffManagementSystem.Api.Domain.Enums;
using StaffManagementSystem.Api.Domain.Exceptions;
using StaffManagementSystem.Api.Infrastructure.IRepositories;
using StaffManagementSystem.Api.Services.EmailService;
using StaffManagementSystem.Api.Services.Security;

namespace StaffManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KadrController : RESTFulController
    {
        private readonly IKadrRepository kadrRepository;
        private readonly IValidator<CreateKadrDto> createValidator;
        private readonly IValidator<UpdateKadrDto> updateValidator;
        private readonly IPasswordSecurity passwordSecurity;
        private readonly IEmailSender emailSender;

        public KadrController(IKadrRepository kadrRepository,
            IValidator<CreateKadrDto> createValidator,
            IPasswordSecurity passwordSecurity,
            IEmailSender emailSender,
            IValidator<UpdateKadrDto> updateValidator)
        {
            this.kadrRepository = kadrRepository;
            this.createValidator = createValidator;
            this.passwordSecurity = passwordSecurity;
            this.emailSender = emailSender;
            this.updateValidator = updateValidator;
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost("register")]
        public async Task<IActionResult> CreateKadrAsync(CreateKadrDto createKadrDto)
        {
            try
            {
                if (createKadrDto is null)
                {
                    throw new ArgumentNullException(nameof(createKadrDto));
                }

                ValidationResult validationResult = await this.createValidator.ValidateAsync(createKadrDto);
                var invalidException = new InvalidException("Validation error occured");

                foreach (var error in validationResult.Errors)
                {
                    invalidException.UpsertDataList(error.PropertyName, error.ErrorMessage);
                }

                invalidException.ThrowIfContainsErrors();

                var existingKadr = await this.kadrRepository.GetAll().AnyAsync(k => k.Email == createKadrDto.Email);

                if (existingKadr)
                {
                    return Conflict();
                }

                var kadr = new Kadr
                {
                    Id = Guid.NewGuid(),
                    Firstname = createKadrDto.Firstname,
                    Lastname = createKadrDto.Lastname,
                    Email = createKadrDto.Email,
                    Password = this.passwordSecurity.Encrypt(createKadrDto.Password),
                    Role = Roles.Kadr
                };

                var addedEntity = await this.kadrRepository.InsertAsync(kadr);

                string actionMessage = $@"
                       <div>
                           <p>Iltmos bu ma'lumotlarni hech kimga bermang!</p>
                           <p><b>Email: {addedEntity.Email}</b></p>
                           <p><b>Parol: {createKadrDto.Password}</b></p>
                           <P>---Tizimga kadr sifatida kirishingiz mumkin---</p>
                       </div>";

                this.emailSender.SendEmail(createKadrDto.Email, createKadrDto.Password, actionMessage);

                return Created(addedEntity);
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

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPut]
        public async Task<IActionResult> UpdateKadrAsync(UpdateKadrDto updateKadrDto)
        {
            try
            {
                if (updateKadrDto is null)
                {
                    throw new ArgumentNullException(nameof(updateKadrDto));
                }

                ValidationResult validationResult = await this.updateValidator.ValidateAsync(updateKadrDto);
                var invalidException = new InvalidException("Validation error occured");

                foreach (var error in validationResult.Errors)
                {
                    invalidException.UpsertDataList(error.PropertyName, error.ErrorMessage);
                }

                invalidException.ThrowIfContainsErrors();

                var existingKadr = await this.kadrRepository.GetByIdAsync(updateKadrDto.Id);

                existingKadr.Firstname = updateKadrDto.Firstname;
                existingKadr.Lastname = updateKadrDto.Lastname;
                existingKadr.Email = updateKadrDto.Email;
                existingKadr.Password = this.passwordSecurity.Encrypt(updateKadrDto.Password);

                var updatedEntity = await this.kadrRepository.UpdateAsync(existingKadr);

                string actionMessage = $@"
                       <div>
                           <p>Iltmos bu ma'lumotlarni hech kimga bermang!</p>
                           <p><b>Email: {updatedEntity.Email}</b></p>
                           <p><b>Yangilangan parol: {updateKadrDto.Password}</b></p>
                           <P>---Tizimga kadr sifatida kirishingiz mumkin---</p>
                       </div>";

                this.emailSender.SendEmail(updateKadrDto.Email, updateKadrDto.Password, actionMessage);

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

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKadrByIdAsync(Guid id)
        {
            try
            {
                var kadr = await this.kadrRepository.GetByIdAsync(id);

                if (kadr is null)
                {
                    return NotFound();
                }

                return Ok(kadr);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpGet]
        public ActionResult<IQueryable<Kadr>> GetAllKadr()
        {
            try
            {
                IQueryable<Kadr> kadrs = this.kadrRepository.GetAll();

                return Ok(kadrs);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKadrAsync(Guid id)
        {
            try
            {
                var kadr = await this.kadrRepository.GetByIdAsync(id);

                if (kadr is null)
                {
                    return NotFound();
                }

                await this.kadrRepository.DeleteAsync(kadr);

                return NoContent();
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
