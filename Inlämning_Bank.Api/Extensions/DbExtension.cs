using Inlämning_Bank.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Inlämning_Bank.Api.Extensions
{
    public static class DbExtension
    {
        public static IServiceCollection AddDbExtended(this IServiceCollection services)
        {
            //testar använda en lokal miljövariabel till anslutningssträngen.
            string connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

            //Databasen
            services.AddDbContext<BankAppDataContext>(
                options => options.UseSqlServer(connectionString)
                );


            //// Om anslutningssträngen inte hittas i miljövariabler, använd appsettings.json
            //if (string.IsNullOrEmpty(connectionString))
            //{
            //    builder.Configuration.AddJsonFile("appsettings.json");
            //    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            //}

            return services;
        }
    }
}
