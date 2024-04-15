
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

            // Automapper sätts upp som en service, sättet Fredrik visade. 
            //builder.Services.AddAutoMapper(typeof(Program).Assembly);
            
            //Sättet man gör det på om man har problem med att filerna ligger i olika projekt. 
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; 
                    // undviker loopar, men kan ändå hänvisa till det tidigare objektet på något vis. blir större json än ignore cycles

                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // undviker också loopar

                    //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; 
                    // Jag vill nog att null värden ska visas också. för nu har jag ju en DTO klass för när man ska skriva in data.
                    
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

            //testar lokal miljövariabel
            string connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

            //// Om anslutningssträngen inte hittas i miljövariabler, använd appsettings.json
            //if (string.IsNullOrEmpty(connectionString))
            //{
            //    builder.Configuration.AddJsonFile("appsettings.json");
            //    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            //}

            //Databasen
            builder.Services.AddDbContext<BankAppDataContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString(connectionString))
                );

            //Här sätts Identity upp som en tjänst. Bygger på att EF finns. 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<BankAppDataContext>()
            .AddDefaultTokenProviders();


            //Dependency injection container
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<IDispositionService, DispositionService>(); 
            builder.Services.AddScoped<IDispositionRepo, DispositionRepo>(); 
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<ILoanRepo, LoanRepo>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();



            string secretKey = Environment.GetEnvironmentVariable("SecretKey");

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
                    ValidIssuer = "http://localhost:5249/",
                    ValidAudience = "http://localhost:5249/",
                    IssuerSigningKey =
                     new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                                        
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
