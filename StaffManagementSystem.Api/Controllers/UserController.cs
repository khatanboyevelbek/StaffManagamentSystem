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
    public class UserController : RESTFulController
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordSecurity passwordSecurity;
        private readonly IValidator<CreateUserDto> createValidator;
        private readonly IValidator<UpdateUserDto> updateValidator;
        private readonly IEmailSender emailSender;

        public UserController(IUserRepository userRepository, 
            IPasswordSecurity passwordSecurity,
            IValidator<CreateUserDto> createValidator,
            IValidator<UpdateUserDto> updateValidator,
            IEmailSender emailSender)
        {
            this.userRepository = userRepository;
            this.passwordSecurity = passwordSecurity;
            this.createValidator = createValidator;
            this.updateValidator = updateValidator;
            this.emailSender = emailSender;
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost("register")]
        public async Task<IActionResult> AddUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                if (createUserDto is null)
                {
                    throw new ArgumentNullException(nameof(createUserDto));
                }

                ValidationResult validationResult = await this.createValidator.ValidateAsync(createUserDto);
                var invalidException = new InvalidException("Validation error occured");

                foreach (var error in validationResult.Errors)
                {
                    invalidException.UpsertDataList(error.PropertyName, error.ErrorMessage);
                }

                invalidException.ThrowIfContainsErrors();

                var existingUser = await this.userRepository.GetAll().AnyAsync(u => u.Email == createUserDto.Email);

                if (existingUser)
                {
                    return Conflict();
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Firstname = createUserDto.Firstname,
                    Lastname = createUserDto.Lastname,
                    Email = createUserDto.Email,
                    Password = this.passwordSecurity.Encrypt(createUserDto.Password),
                    Role = Roles.User,
                    Vacations = new List<Vacation>(),
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                var entity = await this.userRepository.InsertAsync(user);

                string actionMessage = $@"
                       <div>
                           <p>Iltmos bu ma'lumotlarni hech kimga bermang!</p>
                           <p><b>Email: {entity.Email}</b></p>
                           <p><b>Parol: {createUserDto.Password}</b></p>
                           <P>---Tizimga hodim sifatida kirishingiz mumkin---</p>
                       </div>";

                this.emailSender.SendEmail(createUserDto.Email, createUserDto.Password, actionMessage);

                return Created(entity);
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

        [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Kadr))]
        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            try
            {
                if (updateUserDto is null)
                {
                    throw new ArgumentNullException(nameof(updateUserDto));
                }

                ValidationResult validationResult = await this.updateValidator.ValidateAsync(updateUserDto);
                var invalidException = new InvalidException("Validation error occured");

                foreach (var error in validationResult.Errors)
                {
                    invalidException.UpsertDataList(error.PropertyName, error.ErrorMessage);
                }

                invalidException.ThrowIfContainsErrors();

                var existingUser = await this.userRepository.GetByIdAsync(updateUserDto.Id);

                existingUser.Firstname = updateUserDto.Firstname;
                existingUser.Lastname = updateUserDto.Lastname;
                existingUser.Email = updateUserDto.Email;
                existingUser.Password = this.passwordSecurity.Encrypt(updateUserDto.Password);

                var updatedEntity = await this.userRepository.UpdateAsync(existingUser);

                string actionMessage = $@"
                       <div>
                           <p>Iltmos bu ma'lumotlarni hech kimga bermang!</p>
                           <p><b>Email: {updateUserDto.Email}</b></p>
                           <p><b>Yangilangan parol: {updateUserDto.Password}</b></p>
                           <P>---Tizimga hodim sifatida kirishingiz mumkin---</p>
                       </div>";

                this.emailSender.SendEmail(updateUserDto.Email, updateUserDto.Password, actionMessage);

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

        [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Kadr) + "," + nameof(Roles.Director))]
        [HttpGet]
        public ActionResult<IQueryable<User>> GetAllUser()
        {
            try
            {
                IQueryable<User> users = this.userRepository.GetAll();

                return Ok(users);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await this.userRepository.GetByIdAsync(id);

                if(user is null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await this.userRepository.GetByIdAsync(id);

                if (user is null)
                {
                    return NotFound();
                }

                await this.userRepository.DeleteAsync(user);

                return NoContent();
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
