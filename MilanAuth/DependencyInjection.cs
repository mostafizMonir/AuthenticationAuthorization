using System.Text;
using DotNetChannel.Models;
using DotNetChannel.Services;
using EventDispatcher.Core.Abstractions;
using EventDispatcher.Core.Dispatchers;
using EventDispatcher.Core.Events;
using EventDispatcher.Core.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MilanAuth.Abstractions;
using MilanAuth.Data;
using MilanAuth.Services;
using Scrutor;
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

        services.AddScoped(typeof(Repository<>));
        services.AddScoped(typeof(RabbitMQService));
        services.AddScoped<IImageProcessingService, ImageProcessingService>();

        services.AddSingleton<IMessageService, MessageService>();
        services.AddHostedService<MessageReceiverService>();

        services.AddTransient <IdGenerator>();
        services.AddTransient(typeof(IdPrinter));

        // var jwtKey = configuration["Jwt:Key"] ?? "super_secret_key_123!";
        // var jwtKey = "a6vQ~9#kP2$tLp5*WnZ8&bY4^cX7!mD3";
        // var jwtIssuer = configuration["Jwt:Issuer"] ?? "MilanAuthIssuer";
        //
        // services.AddAuthentication(options =>
        //     {
        //         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //     })
        //     .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
        //     {
        //         ValidateIssuer = true,
        //         ValidateAudience = false,
        //         ValidateLifetime = true,
        //         ValidateIssuerSigningKey = true,
        //         ValidIssuer = jwtIssuer,
        //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        //     });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.Authority = configuration["Authentication:Issuer"];
            o.Audience = configuration["Authentication:Audience"];
            o.RequireHttpsMetadata = false; // Only for development

            // Add these configurations:
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Authentication:Issuer"],
                ValidAudience = configuration["Authentication:Audience"],
                // Important for Keycloak:
                ClockSkew = TimeSpan.FromSeconds(30) // Adjust tolerance as needed
            };

            o.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Authentication failed: {Exception}", context.Exception);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Token validated successfully");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("Challenge issued: {Error}", context.Error);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        services.AddControllers();

        // Register all event dispatchers and handlers using Scrutor
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(IEventDispatcher<>), typeof(IDomainEventHandler<>))
                .AddClasses(classes => classes.AssignableTo(typeof(IEventDispatcher<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
        );

        

        return services;
    }
}
