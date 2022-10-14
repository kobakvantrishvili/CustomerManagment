using CustomerManagment.Services.Models.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace CustomerManagment.Infrastructure.Extensions
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddTokenAuthentiction(this IServiceCollection service, IConfiguration options)
        {
            var secretKey = Encoding.ASCII.GetBytes(options.GetSection("JWTConfig").GetSection("Secret").Value); // grabbing the key from appsettings.json

            service.AddAuthentication(jwtOptions =>
            {
                jwtOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                jwtOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwtOptions =>
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey), // decrypts the token
                    ValidateLifetime = true, // validates the lifetime of token
                    ValidateIssuer = false,   // validates who's accessing
                    ValidateAudience = false, // validates what being accessed
                    // Issuer and Audience are being tested against
                    ValidIssuer = "localhost",   // who created a token
                    ValidAudience = "localhost", // taget of the token
                    //token validation uses 5 minute windows to validate token, that would mae token valid until 5 minutes after expiration
                    ClockSkew = TimeSpan.Zero // this sets immediate expiration
                });

            return service;
        }
    }
}
