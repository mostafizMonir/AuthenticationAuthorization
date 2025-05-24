using Microsoft.EntityFrameworkCore;
using MilanAuth.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using MilanAuth;

var builder = WebApplication.CreateBuilder(args);

// Register all services using extension method
builder.Services.AddApplicationServices(builder.Configuration);

// Read JWT config for token generation
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super_secret_key_123!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MilanAuthIssuer";

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/users", async (AppDbContext dbContext) =>
{
    var users = await dbContext.Users.ToListAsync();
    return Results.Ok(users);
}).RequireAuthorization();

app.MapPost("/user", async (User user, AppDbContext dbContext) =>
{
    // Hash the password before saving
    if (!string.IsNullOrEmpty(user.PasswordHash))
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
    }
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/user/{user.Id}", user);
});

app.MapPost("/login", async (AppDbContext dbContext, User loginUser) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == loginUser.UserName);
    if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.PasswordHash, user.PasswordHash))
    {
        return Results.Unauthorized();
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? "")
    };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
        issuer: jwtIssuer,
        audience: null,
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds
    );
    var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString });
});

app.Run();
