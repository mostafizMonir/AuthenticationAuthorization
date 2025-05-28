using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventDispatcher.Core.Abstractions;
using EventDispatcher.Core.Dispatchers;

namespace MilanAuth
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            
            services.AddDbContext<Data.AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // var jwtKey = configuration["Jwt:Key"] ?? "super_secret_key_123!";
            var jwtKey = "a6vQ~9#kP2$tLp5*WnZ8&bY4^cX7!mD3";
            var jwtIssuer = configuration["Jwt:Issuer"] ?? "MilanAuthIssuer";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });
            

            services.AddAuthorization();

            services.AddControllers();

            services.AddScoped<IEventDispatcher<IDomainEvent>, DomainEventDispatcher>();

            return services;
        }
    }
} 
