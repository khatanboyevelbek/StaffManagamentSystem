using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace StaffManagementSystem.Api.Domain.Entities
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
