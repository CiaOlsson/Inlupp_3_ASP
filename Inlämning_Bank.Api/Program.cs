
using Inl�mning_Bank.Core.Interfaces;
using Inl�mning_Bank.Core.Services;
using Inl�mning_Bank.Data.Contexts;
using Inl�mning_Bank.Data.Interfaces;
using Inl�mning_Bank.Data.Repos;
using Inl�mning_Bank.Domain.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;


//Testa g�ra extension-methods till services som har mycket konfiguration. Tex swashbuckle. 

namespace Inl�mning_Bank.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json");

            // Automapper s�tts upp som en service, s�ttet Fredrik visade. 
            //builder.Services.AddAutoMapper(typeof(Program).Assembly);
            
            //S�ttet man g�r det p� om man har problem med att filerna ligger i olika projekt. 
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; 
                    // undviker loopar, men kan �nd� h�nvisa till det tidigare objektet p� n�got vis. blir st�rre json �n ignore cycles

                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // undviker ocks� loopar

                    //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; 
                    // Jag vill nog att null v�rden ska visas ocks�. f�r nu har jag ju en DTO klass f�r n�r man ska skriva in data.
                    
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

            //testar lokal milj�variabel
            string connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

            //// Om anslutningsstr�ngen inte hittas i milj�variabler, anv�nd appsettings.json
            //if (string.IsNullOrEmpty(connectionString))
            //{
            //    builder.Configuration.AddJsonFile("appsettings.json");
            //    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            //}

            //Databasen
            builder.Services.AddDbContext<BankAppDataContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString(connectionString))
                );

            //H�r s�tts Identity upp som en tj�nst. Bygger p� att EF finns. 
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

            //Authentication s�tts upp
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

            // H�r skapas rollerna redan vid start om de inte redan finns i databasen. 
            // S�g det i video utav Hans Mattin-Lassei. 
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
