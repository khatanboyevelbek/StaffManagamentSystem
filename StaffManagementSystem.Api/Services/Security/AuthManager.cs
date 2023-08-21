using FluentAssertions.Equivalency.Tracing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StaffManagementSystem.Api.Domain.Entities;
using System.IO;

namespace StaffManagementSystem.Api.Services.Security
{
    public class AuthManager : IAuthManager
    {
        private readonly IConfiguration configuration;

        public AuthManager(IConfiguration configuration) =>
            this.configuration = configuration;

        public string GenerateToken(Admin admin)
        {
            var securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var cridentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Role, admin.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:ValidIssuer"],
                audience: configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: cridentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(User user)
        {
            var securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var cridentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:ValidIssuer"],
                audience: configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: cridentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(Director director)
        {
            var securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var cridentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, director.Id.ToString()),
                new Claim(ClaimTypes.Role, director.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:ValidIssuer"],
                audience: configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: cridentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(Kadr kadr)
        {
            var securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var cridentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, kadr.Id.ToString()),
                new Claim(ClaimTypes.Role, kadr.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:ValidIssuer"],
                audience: configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: cridentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
