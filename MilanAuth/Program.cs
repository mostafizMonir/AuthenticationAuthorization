using Microsoft.EntityFrameworkCore;
using MilanAuth.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using MilanAuth;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Register all services using extension method
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Prometheus metrics endpoint
app.UseMetricServer();
app.UseHttpMetrics();

// Add detailed logging for authentication
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request path: {Path}", context.Request.Path);
    logger.LogInformation("Authorization header: {AuthHeader}", context.Request.Headers.Authorization.ToString());
    
    await next();
    
    logger.LogInformation("Response status code: {StatusCode}", context.Response.StatusCode);
});

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("users/me", (ClaimsPrincipal claims) =>
{
    return claims.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.MapControllers();

app.Run();


