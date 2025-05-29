using System.Text;
using EventDispatcher.Core.Abstractions;
using EventDispatcher.Core.Dispatchers;
using EventDispatcher.Core.Events;
using EventDispatcher.Core.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace MilanAuth;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Seq("http://seq:80")
            .CreateLogger();
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

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
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            });
            

        services.AddAuthorization();

        services.AddControllers();

        services.AddScoped<IEventDispatcher<IDomainEvent>, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<OrderShippedEvent>, OrderShippedEmailHandler>();

        return services;
    }
}
