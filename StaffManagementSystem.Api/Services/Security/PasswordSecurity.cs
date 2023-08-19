using System.Diagnostics.Eventing.Reader;
using BCrypt.Net;

namespace StaffManagementSystem.Api.Services.Security
{
    public class PasswordSecurity : IPasswordSecurity
    {
        public string Encrypt(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor: 13, HashType.SHA512);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash, HashType.SHA512);
        }
    }
}
