using KodiksCase.Application.Services.Interfaces;
using KodiksCase.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Application.Services.Implementations
{
    /// <summary>
    /// Service responsible for generating JWT tokens based on user identifiers.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Initializes TokenService with JWT configuration settings.
        /// </summary>
        /// <param name="jwtOptions">Configuration options for JWT.</param>
        public TokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// Generates a JWT token string containing the user ID as a claim.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A signed JWT token string.</returns>
        public string GenerateToken(string userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
