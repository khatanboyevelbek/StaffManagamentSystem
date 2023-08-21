using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using StaffManagementSystem.Api.Domain.DTOs;
using StaffManagementSystem.Api.Domain.Entities;
using StaffManagementSystem.Api.Domain.Enums;
using StaffManagementSystem.Api.Infrastructure.IRepositories;
using StaffManagementSystem.Api.Services.Security;

namespace StaffManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : RESTFulController
    {
        private readonly IAuthManager authManager;
        private readonly IPasswordSecurity passwordSecurity;
        private readonly IAdminRepository adminRepository;
        private readonly IUserRepository userRepository;
        private readonly IKadrRepository kadrRepository;
        private readonly IDirectorRepository directorRepository;

        public AuthController(IAuthManager authManager, IPasswordSecurity passwordSecurity,
            IAdminRepository adminRepository, IUserRepository userRepository,
            IKadrRepository kadrRepository, IDirectorRepository directorRepository)
        {
            this.authManager = authManager;
            this.passwordSecurity = passwordSecurity;
            this.adminRepository = adminRepository;
            this.userRepository = userRepository;
            this.kadrRepository = kadrRepository;
            this.directorRepository = directorRepository;
        }

        [AllowAnonymous]
        [HttpPost("admin/login")]
        public async Task<IActionResult> LoginAdmin(LoginModel loginModel)
        {
            try
            {
                if (loginModel is null)
                {
                    throw new ArgumentNullException(nameof(loginModel));
                }

                Admin existingAdmin = await this.adminRepository.GetAdminByEmailAsync(loginModel.Email);
                bool verifyPassword = string.Equals(loginModel.Password, existingAdmin.Password);

                if ((existingAdmin is not null) && verifyPassword)
                {
                    string token = this.authManager.GenerateToken(existingAdmin);

                    return Ok(new { token, existingAdmin.Email });
                }

                return Unauthorized();
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

        [AllowAnonymous]
        [HttpPost("user/login")]
        public async Task<IActionResult> LoginUser(LoginModel loginModel)
        {
            try
            {
                if (loginModel is null)
                {
                    throw new ArgumentNullException(nameof(loginModel));
                }

                User existingUser = await this.userRepository.GetUserByEmailAsync(loginModel.Email);
                bool verifyPassword = this.passwordSecurity.Verify(loginModel.Password, existingUser.Password);

                if ((existingUser is not null) && verifyPassword)
                {
                    string token = this.authManager.GenerateToken(existingUser);

                    return Ok(new { token, existingUser.Email });
                }

                return Unauthorized();
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

        [AllowAnonymous]
        [HttpPost("kadr/login")]
        public async Task<IActionResult> LoginKadr(LoginModel loginModel)
        {
            try
            {
                if (loginModel is null)
                {
                    throw new ArgumentNullException(nameof(loginModel));
                }

                Kadr existingKadr = await this.kadrRepository.GetKadrByEmailAsync(loginModel.Email);
                bool verifyPassword = this.passwordSecurity.Verify(loginModel.Password, existingKadr.Password);

                if ((existingKadr is not null) && verifyPassword)
                {
                    string token = this.authManager.GenerateToken(existingKadr);

                    return Ok(new { token, existingKadr.Email });
                }

                return Unauthorized();
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

        [AllowAnonymous]
        [HttpPost("director/login")]
        public async Task<IActionResult> LoginDirector(LoginModel loginModel)
        {
            try
            {
                if (loginModel is null)
                {
                    throw new ArgumentNullException(nameof(loginModel));
                }

                Director existingDirector = await this.directorRepository.GetDirectorByEmailAsync(loginModel.Email);
                bool verifyPassword = this.passwordSecurity.Verify(loginModel.Password, existingDirector.Password);

                if ((existingDirector is not null) && verifyPassword)
                {
                    string token = this.authManager.GenerateToken(existingDirector);

                    return Ok(new { token, existingDirector.Email });
                }

                return Unauthorized();
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
