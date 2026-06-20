using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoApp.Core.DTOs.Auth;
using ToDoApp.Core.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.Core.Services
{
    public class AuthService(AppDbContext context, IConfiguration config) : IAuthService
    {
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Username already in use");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return GenerateToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid username or password");

            return GenerateToken(user);
        }

        private AuthResponseDto GenerateToken(User user)                                
        {
            var jwtSettings = config.GetSection("JwtSettings");

            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey не настроен в appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var expiryMinutesStr = jwtSettings["ExpiryMinutes"];
            var expiryMinutes = double.TryParse(expiryMinutesStr, out var minutes) ? minutes : 60;
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
