using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using StaffManagementSystem.Api.Domain.DTOs;
using StaffManagementSystem.Api.Domain.Entities;
using StaffManagementSystem.Api.Domain.Enums;
using StaffManagementSystem.Api.Infrastructure.IRepositories;
using StaffManagementSystem.Api.Services.EmailService;

namespace StaffManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacationController : RESTFulController
    {
        private readonly IVacationRepository vacationRepository;
        private readonly IUserRepository userRepository;
        private readonly IEmailSender emailSender;


        public VacationController(IVacationRepository vacationRepository,
            IUserRepository userRepository, IEmailSender emailSender)
        {
            this.vacationRepository = vacationRepository;
            this.userRepository = userRepository;
            this.emailSender = emailSender;
        }

        [Authorize(Roles = nameof(Roles.Kadr))]
        [HttpPost]
        public async Task<IActionResult> AddVacationAsync(CreateVacationDto createVacationDto)
        {
            try
            {
                if (createVacationDto is null)
                {
                    throw new ArgumentNullException(nameof(createVacationDto));
                }

                var vacation = new Vacation
                {
                    Id = Guid.NewGuid(),
                    UserId = createVacationDto.UserId,
                    StartDate = createVacationDto.StartDate,
                    EndDate = createVacationDto.StartDate,
                    Status = VacationStatus.Coming
                };

                var entity = await this.vacationRepository.InsertAsync(vacation);

                var currentUser = await this.userRepository.GetByIdAsync(createVacationDto.UserId);

                string actionMessage = $@"
                       <div>
                           <p><b>Xurmatli {currentUser.Firstname} {currentUser.Lastname}</b></p>
                           <p>Sizga {entity.StartDate.ToString("dd.MM.yyyy")} dan {entity.EndDate.ToString("dd.MM.yyyy")} gacha otpuska belgilandi</p>
                           <p>Batafsil ma'lumotlar bilan tanishish uchun tizimga kiring</p>
                       </div>";

                this.emailSender.SendEmail(currentUser.Email, string.Empty, actionMessage);

                return Created(entity);
            }
            catch (ArgumentNullException exception)
            {
                return BadRequest(exception);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize(Roles = nameof(Roles.Kadr))]
        [HttpPut]
        public async Task<IActionResult> UpdateVacationAsync(UpdateVacationDto updateVacationDto)
        {
            try
            {
                if (updateVacationDto is null) 
                {
                    throw new ArgumentNullException(nameof(updateVacationDto));
                }

                var existingVacation = await this.vacationRepository.GetByIdAsync(updateVacationDto.Id);

                existingVacation.StartDate = updateVacationDto.StartDate;
                existingVacation.EndDate = updateVacationDto.EndDate;
                existingVacation.Status = updateVacationDto.Status;

                var updatedEntity = await this.vacationRepository.UpdateAsync(existingVacation);
                var currentUser = await this.userRepository.GetByIdAsync(updateVacationDto.UserId);

                string actionMessage = $@"
                       <div>
                           <p><b>Xurmatli {currentUser.Firstname} {currentUser.Lastname}</b></p>
                           <p>Sizni otpuskangiz {updatedEntity.StartDate.ToString("dd.MM.yyyy")} dan {updatedEntity.EndDate.ToString("dd.MM.yyyy")} gacha o'zgartirildi</p>
                           <p>Batafsil ma'lumotlar bilan tanishish uchun tizimga kiring</p>
                       </div>";

                this.emailSender.SendEmail(currentUser.Email, string.Empty, actionMessage);

                return NoContent();
            }
            catch (ArgumentNullException exception)
            {
                return BadRequest(exception);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
