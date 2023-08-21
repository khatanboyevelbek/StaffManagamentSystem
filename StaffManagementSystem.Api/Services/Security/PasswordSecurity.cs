using BCrypt.Net;

namespace StaffManagementSystem.Api.Services.Security
{
    public class PasswordSecurity : IPasswordSecurity
    {
        public string Encrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
