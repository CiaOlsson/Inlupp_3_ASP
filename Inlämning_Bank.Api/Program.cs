
using Inl�mning_Bank.Api.Extensions;
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


namespace Inl�mning_Bank.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //S�ttet man kan s�tta upp Automapper p� om man har problem med att filerna ligger i olika projekt. 
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options => {   // undviker loopar
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

            //Swagger, med inst�llningar i en extension method. 
            builder.Services.AddSwaggerExtended();

            //Databasen
            builder.Services.AddDbExtended();
       

            //Identity
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

            //Jwt, inst�llningarna har jag lagt i en extension method.
            builder.Services.AddJwtExtended();


            var app = builder.Build();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerExtended();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            // H�r skapas rollerna redan vid start om de inte redan finns i databasen. 
            // S�g det i videon av Hans Mattin-Lassei.
            // Det var bra att g�ra s� f�r man ska helst inte skriva in data manuellt i databasen s� detta var ett smidigt s�tt att g�ra det p�, s� att det man beh�ver finns d�r fr�n b�rjan. 
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
