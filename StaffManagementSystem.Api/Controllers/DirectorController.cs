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
    public class DirectorController : RESTFulController
    {
        private readonly IDirectorRepository directorRepository;
        private readonly IPasswordSecurity passwordSecurity;
        private readonly IEmailSender emailSender;
        private readonly IValidator<CreateDirectorDto> createValidator;
        private readonly IValidator<UpdateDirectorDto> updateValidator;

        public DirectorController(IDirectorRepository directorRepository,
            IPasswordSecurity passwordSecurity, IEmailSender emailSender,
            IValidator<CreateDirectorDto> createValidator,
            IValidator<UpdateDirectorDto> updateValidator)
        {
            this.directorRepository = directorRepository;
            this.passwordSecurity = passwordSecurity;
            this.emailSender = emailSender;
            this.createValidator = createValidator;
            this.updateValidator = updateValidator;
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost("register")]
        public async Task<IActionResult> InsertDirectorAsync(CreateDirectorDto createDirectorDto)
        {
            try
            {
                if (createDirectorDto is null)
                {
                    throw new ArgumentNullException(nameof(createDirectorDto));
                }

                ValidationResult validationResult = await this.createValidator.ValidateAsync(createDirectorDto);
                var invalidException = new InvalidException("Validation error occured");

                foreach (var error in validationResult.Errors)
                {
                    invalidException.UpsertDataList(error.PropertyName, error.ErrorMessage);
                }

                invalidException.ThrowIfContainsErrors();

                var existingDirector = await this.directorRepository.GetAll().AnyAsync(k => k.Email == createDirectorDto.Email);

                if (existingDirector)
                {
                    return Conflict();
                }

                var director = new Director
                {
                    Id = Guid.NewGuid(),
                    Firstname = createDirectorDto.Firstname,
                    Lastname = createDirectorDto.Lastname,
                    Email = createDirectorDto.Email,
                    Password = this.passwordSecurity.Encrypt(createDirectorDto.Password),
                    Role = Roles.Director
                };

                var addedEntity = await this.directorRepository.InsertAsync(director);

                string actionMessage = $@"
                       <div>
                           <p>Iltmos bu ma'lumotlarni hech kimga bermang!</p>
                           <p><b>Email: {addedEntity.Email}</b></p>
                           <p><b>Parol: {createDirectorDto.Password}</b></p>
                           <P>---Tizimga director sifatida kirishingiz mumkin---</p>
                       </div>";

                this.emailSender.SendEmail(createDirectorDto.Email, createDirectorDto.Password, actionMessage);

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
        public async Task<IActionResult> UpdateDirectorAsync(UpdateDirectorDto updateDirectorDto)
        {
            try
            {
                if (updateDirectorDto is null)
                {
                    throw new ArgumentNullException(nameof(updateDirectorDto));
                }

                ValidationResult validationResult = await this.updateValidator.ValidateAsync(updateDirectorDto);
                var invalidException = new InvalidException("Validation error occured");

                foreach (var error in validationResult.Errors)
                {
                    invalidException.UpsertDataList(error.PropertyName, error.ErrorMessage);
                }

                invalidException.ThrowIfContainsErrors();

                var existingDirector = await this.directorRepository.GetByIdAsync(updateDirectorDto.Id);

                existingDirector.Firstname = updateDirectorDto.Firstname;
                existingDirector.Lastname = updateDirectorDto.Lastname;
                existingDirector.Email = updateDirectorDto.Email;
                existingDirector.Password = this.passwordSecurity.Encrypt(updateDirectorDto.Password);

                var updatedEntity = await this.directorRepository.UpdateAsync(existingDirector);

                string actionMessage = $@"
                       <div>
                           <p>Iltmos bu ma'lumotlarni hech kimga bermang!</p>
                           <p><b>Email: {updateDirectorDto.Email}</b></p>
                           <p><b>Yangilangan parol: {updateDirectorDto.Password}</b></p>
                           <P>---Tizimga director sifatida kirishingiz mumkin---</p>
                       </div>";

                this.emailSender.SendEmail(updateDirectorDto.Email, updateDirectorDto.Password, actionMessage);

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
        public async Task<IActionResult> GetDirectorByIdAsync(Guid id)
        {
            try
            {
                var director = await this.directorRepository.GetByIdAsync(id);

                if (director is null)
                {
                    return NotFound();
                }

                return Ok(director);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpGet]
        public ActionResult<IQueryable<Director>> GetAllDirectors()
        {
            try
            {
                IQueryable<Director> directors = this.directorRepository.GetAll();

                return Ok(directors);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDirectorAsync(Guid id)
        {
            try
            {
                var director = await this.directorRepository.GetByIdAsync(id);

                if (director is null)
                {
                    return NotFound();
                }

                await this.directorRepository.DeleteAsync(director);

                return NoContent();
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
