using CustomerManagment.Services.Abstraction;
using CustomerManagment.Services.Models.JWT;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Xml.Linq;
using System.Security.Claims;
using Microsoft.SqlServer.Server;

namespace CustomerManagment.Services.Implementatin
{
    public class JWTService : IJWTService
    {
        private readonly string _secret;
        private readonly int _expirationInMinutes;

        public JWTService(IOptions<JWTConfig> options)
        {
            _secret = options.Value.Secret;
            _expirationInMinutes = options.Value.ExpirationInMinutes;
        }

        public string GenerateSecurityToken(string Email)
        {
            var tokenHandler = new JwtSecurityTokenHandler(); // designed for creating and validating Json Web Tokens
            var secretKey = Encoding.ASCII.GetBytes(_secret); // store it as bytes
            
            var tokenDescriptor = new SecurityTokenDescriptor // Contains some information which used to create a security token.
            {
                //Payload : Data
                Subject = new ClaimsIdentity(new[] // who the token belongs to
                {
                    new Claim(ClaimTypes.Name, Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_expirationInMinutes),
                // this contains both verify signature and header part (since we are telling what algorithm to use)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor); // creates JWT token

            return tokenHandler.WriteToken(token); //Serializes a token into a JWT (string) in Compact Serialization Format
        }
    }
}
