using FluentAssertions.Equivalency.Tracing;
using StaffManagementSystem.Api.Domain.Entities;

namespace StaffManagementSystem.Api.Services.Security
{
    public interface IAuthManager
    {
        string GenerateToken(Admin admin);
        string GenerateToken(User user);
        string GenerateToken(Director director);
        string GenerateToken(Kadr kadr);
    }
}
