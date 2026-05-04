using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartSchedule.Infrastructure.Integration
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _jwtOptions;

        public JwtService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateAccessToken(User user)
        {
            //if (_jwtOptions == null)
            //    throw new Exception("JwtOptions is null");

            //if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
            //    throw new Exception("JwtOptions.Key is null or empty");

            //if (string.IsNullOrWhiteSpace(_jwtOptions.Issuer))
            //    throw new Exception("JwtOptions.Issuer is null or empty");

            //if (string.IsNullOrWhiteSpace(_jwtOptions.Audience))
            //    throw new Exception("JwtOptions.Audience is null or empty");

            //if (_jwtOptions.ExpirationMinutes <= 0)
            //    throw new Exception($"JwtOptions.ExpirationMinutes is invalid: {_jwtOptions.ExpirationMinutes}");


            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}