using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoApp.API.Core.DTOs.Auth;
using ToDoApp.API.Core.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.API.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Username already in use");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid username or password");

            return GenerateToken(user);
        }

        private AuthResponseDto GenerateToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey не настроен в appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var expiryMinutesStr = jwtSettings["ExpiryMinutes"];
            var expiryMinutes = string.IsNullOrEmpty(expiryMinutesStr) ? 60 : double.Parse(expiryMinutesStr);
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.Username,
                ExpiresAt = expiresAt
            };
        }
    }
}
