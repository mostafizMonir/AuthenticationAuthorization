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

var app = builder.Build();
 
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

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("users/me", (ClaimsPrincipal claims) =>
{
    return claims.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.MapControllers();

app.Run();


