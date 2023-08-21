using StaffManagementSystem.Api.Domain.Enums;

namespace StaffManagementSystem.Api.Domain.DTOs
{
    public class CreateVacationDto
    {
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public VacationStatus Status { get; set; }
    }
}
