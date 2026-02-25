using Microsoft.IdentityModel.Tokens;
using server.BLL;
using server.BLL.Intarfaces;
using server.Models;
using server.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.BLL
{
    public class TokenBLL : ITokenBLL
    {

        private readonly IConfiguration _configuration;
        public TokenBLL(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(AuthUserDTO user)
        {
            // 1. קריאת המפתח הסודי והגדרות נוספות
            var keyString = _configuration["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(keyString);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // 2. יצירת ה-Claims (המידע שבתוך הטוקן)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName)
            };
            // 3. הגדרת המאפיינים של הטוקן
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // תוקף לשבוע
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // 4. יצירת הטוקן
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
