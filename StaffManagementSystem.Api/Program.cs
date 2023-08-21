
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Api.Domain.DTOs;
using StaffManagementSystem.Api.Infrastructure;
using StaffManagementSystem.Api.Infrastructure.IRepositories;
using StaffManagementSystem.Api.Infrastructure.Repositories;
using StaffManagementSystem.Api.Services.Security;
using StaffManagementSystem.Api.Services.Vaidations;
using StaffManagementSystem.Api.Services.EmailService;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            ConfigureCORS(builder.Services);
            ConfigureSwagger(builder.Services, builder.Configuration);
            RegisterAuthentication(builder.Services, builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthentication();
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
            services.AddTransient<IVacationRepository, VacationRepository>();
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
            services.AddTransient<IAuthManager, AuthManager>();
        }

        private static void ConfigureCORS(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        private static void ConfigureSwagger(IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    { Title = "StaffManagementSystem.Api", Version = "v1" }
                    );

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };

                c.AddSecurityRequirement(securityRequirement);

            });
        }

        private static void RegisterAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:ValidIssuer"],
                        ValidAudience = configuration["Jwt:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });
        }
    }
}