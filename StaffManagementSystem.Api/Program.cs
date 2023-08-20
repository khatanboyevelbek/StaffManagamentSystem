
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Api.Domain.DTOs;
using StaffManagementSystem.Api.Infrastructure;
using StaffManagementSystem.Api.Infrastructure.IRepositories;
using StaffManagementSystem.Api.Infrastructure.Repositories;
using StaffManagementSystem.Api.Services.Security;
using StaffManagementSystem.Api.Services.Vaidations;
using StaffManagementSystem.Api.Services.EmailService;

namespace StaffManagementSystem.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString);
            });
            RegisterRepositories(builder.Services);
            RegisterUtilities(builder.Services);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAdminRepository, AdminRepository>();
            services.AddTransient<IDirectorRepository, DirectorRepository>();
            services.AddTransient<IKadrRepository, KadrRepository>();
        }

        private static void RegisterUtilities(IServiceCollection services)
        {
            services.AddTransient<IValidator<CreateAdminDto>, AdminCreateDtoValidation>();
            services.AddTransient<IValidator<UpdateAdminDto>, AdminUpdateDtoValidation>();
            services.AddTransient<IValidator<CreateUserDto>, UserCreateDtoValidation>();
            services.AddTransient<IValidator<UpdateUserDto>, UserUpdateDtoValidation>();
            services.AddTransient<IValidator<CreateKadrDto>, KadrCreateDtoValidation>();
            services.AddTransient<IValidator<UpdateKadrDto>, KadrUpdateDtoValidation>();
            services.AddTransient<IValidator<CreateDirectorDto>, DirectorCreateDtoValidation>();
            services.AddTransient<IValidator<UpdateDirectorDto>, DirectorUpdateDtoValidation>();
            services.AddScoped<IPasswordSecurity, PasswordSecurity>();
            services.AddTransient<IEmailSender, EmailSender>();
        }
    }
}