using StaffManagementSystem.Api.Domain.Enums;

namespace StaffManagementSystem.Api.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }

        public ICollection<Vacation> Vacations { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
