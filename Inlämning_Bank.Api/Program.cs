
using Inlämning_Bank.Api.Extensions;
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


namespace Inlämning_Bank.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Sättet man kan sätta upp Automapper på om man har problem med att filerna ligger i olika projekt. 
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options => {   // undviker loopar
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

            //Swagger, med inställningar i en extension method. 
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

            //Jwt, inställningarna har jag lagt i en extension method.
            builder.Services.AddJwtExtended();


            var app = builder.Build();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerExtended();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            // Här skapas rollerna redan vid start om de inte redan finns i databasen. 
            // Såg det i videon av Hans Mattin-Lassei.
            // Det var bra att göra så för man ska helst inte skriva in data manuellt i databasen så detta var ett smidigt sätt att göra det på, så att det man behöver finns där från början. 
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
