using FlightAPIs.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FlightAPIs.Internal.Token
{
    internal class EmployeeTokenProvider (IConfiguration configuration)
    {
        public string createEmployeeToken(Admin user)
        {
            //take secretkey 
            string keys = configuration["JWT:SecretKey"]!;
            //symmetric
            var encodeKeys = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keys!));
            //sign credential 
            var signCredential = new SigningCredentials(encodeKeys,SecurityAlgorithms.HmacSha256);
            //token description
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.Name,user.Name!),
                    new Claim(ClaimTypes.Email,user.Email!),
                    new Claim(ClaimTypes.Role,user.UserType==2?"Admin":"Employee")
                    ]),
                Expires = DateTime.Now.AddHours(1),//for demo only , 
                IssuedAt = DateTime.Now,
                Issuer = configuration["JWT:Issuer"],
                Audience = configuration["JWT:Audience"],
                SigningCredentials = signCredential

            };

            var jwtProvider = new JsonWebTokenHandler();
            var token = jwtProvider.CreateToken(tokenDescription);
            return token;
        }
    }
}
