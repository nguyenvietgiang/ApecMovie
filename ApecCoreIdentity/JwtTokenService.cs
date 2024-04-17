using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApecCoreIdentity
{
    public class JwtTokenService
    {
        public const double AccessTokenExpirationMinutes = 60; // Thời gian hết hạn của access token
        public const double RefreshTokenExpirationDays = 7; // Thời gian hết hạn của refresh token

        public (string accessToken, string refreshToken) GenerateTokens(ClaimsIdentity claimsIdentity)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var accessToken = GenerateAccessToken(claimsIdentity, credentials);
            var refreshToken = GenerateRefreshToken();

            return (accessToken, refreshToken);
        }

        private string GenerateAccessToken(ClaimsIdentity claimsIdentity, SigningCredentials credentials)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes),
                Issuer = JwtConfig.Issuer,
                Audience = JwtConfig.Audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}

