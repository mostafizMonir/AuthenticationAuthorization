using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MilanAuth.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MilanAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;

        public UserController(AppDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
          //  _jwtKey = config["Jwt:Key"] ?? "a6vQ~9#kP2$tLp5*WnZ8&bY4^cX7!mD3";
            _jwtKey =  "a6vQ~9#kP2$tLp5*WnZ8&bY4^cX7!mD3";
            _jwtIssuer = config["Jwt:Issuer"] ?? "MilanAuthIssuer";
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Created($"/api/user/{user.Id}", user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User loginUser)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == loginUser.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.PasswordHash, user.PasswordHash))
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { token = tokenString });
        }
    }
} 
