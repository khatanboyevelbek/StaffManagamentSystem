namespace StaffManagementSystem.Api.Services.Security
{
    public interface IPasswordSecurity
    {
        string Encrypt(string password);
        bool Verify(string password, string passwordHash);
    }
}
