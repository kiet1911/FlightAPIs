using FlightAPIs.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Text;

namespace FlightAPIs.Internal.Token
{
    internal sealed class TokenProvider ( IConfiguration configuration) 
    {
        public string CreateTokenWithUser(User user)
        {
            //take secretkey and convert to hmacsha256
            String secretKey = configuration["JWT:SecretKey"]!;
            var securtiyKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            //hmacsha256 secettkey 
            var credentail = new SigningCredentials(securtiyKey,SecurityAlgorithms.HmacSha256);

            //token description : JWT payload
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                        new Claim("UserId",user.Id.ToString()),
                        new Claim(ClaimTypes.Role,user.UserType==1?"User":"Employee")
                    ]
                ),
                Expires = DateTime.UtcNow.AddHours(1),
                IssuedAt = DateTime.UtcNow,
                Issuer = configuration["JWT:Issuer"],
                Audience = configuration["JWT:Audience"],
                SigningCredentials = credentail,
            };

            var handler = new JsonWebTokenHandler();

            //create token 
            var token = handler.CreateToken(tokenDescription);

            return token;
        }
    }
}
