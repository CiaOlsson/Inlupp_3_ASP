
using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Core.Services;
using Inlämning_Bank.Data.Contexts;
using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Data.Repos;
using Inlämning_Bank.Domain.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;


//Testa göra extension-methods till services som har mycket konfiguration. Tex swashbuckle. 

namespace Inlämning_Bank.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json");

            // Automapper sätts upp som en service
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Marcus skrev detta i discord. Får se om jag upplever problem med detta.
            // Märkte att Automapper började krångla lite om man har profilerna i ett annat projekt än där program.cs ligger. En lösning är följande
            // builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            //Swagger
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "BankApp API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }

                });
            });
            

            //Databasen
            builder.Services.AddDbContext<BankAppDataContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                );

            //Här sätts Identity upp som en tjänst. Bygger på att EF finns. 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<BankAppDataContext>()
            .AddDefaultTokenProviders();


            //Dependency injection container
            builder.Services.AddTransient<ICustomerService, CustomerService>();
            builder.Services.AddTransient<ICustomerRepo, CustomerRepo>();
            builder.Services.AddTransient<IAccountRepo, AccountRepo>();



            //Authentication sätts upp
            builder.Services.AddAuthentication(opt => {
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
                    ValidIssuer = "http://localhost:5166/",
                    ValidAudience = "http://localhost:5166/",
                    IssuerSigningKey =
                     new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretKey12345!#123456789101112"))
                    // Nyckeln kan man hantera med en tjänst så nyckeln inte ligger här, tex azure key vault.                    
                };
            });



            var app = builder.Build();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            // Här skapas rollerna redan vid start om de inte redan finns i databasen. 
            // SÅg det i video utav Hans Mattin-Lassei. 
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = ["Admin", "Customer"];
                foreach (var role in roles)
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
            }

            app.Run();
        }
    }
}
