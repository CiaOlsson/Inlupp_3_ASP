using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Inlämning_Bank.Api.Extensions
{
    public static class JwtExtension
    {

        public static IServiceCollection AddJwtExtended(this IServiceCollection services)
        {
            string secretKey = Environment.GetEnvironmentVariable("SecretKey");

            //Authentication sätts upp
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt => {
                //Konfiguration av JWT
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://localhost:5249/",
                    ValidAudience = "http://localhost:5249/",
                    IssuerSigningKey =
                     new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

                };
            });

            return services;
        }
    }   
}
