using System.Text.Json.Serialization;
using StaffManagementSystem.Api.Domain.Enums;

namespace StaffManagementSystem.Api.Domain.Entities
{
    public class Vacation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public VacationStatus Status { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
