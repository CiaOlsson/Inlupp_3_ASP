using Microsoft.OpenApi.Models;

namespace Inlämning_Bank.Api.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerExtended(this IServiceCollection services)
        {

            services.AddSwaggerGen(options =>
            {
                //Generell information 
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Cissis BankApp API",
                    Version = "v1"
                });

                //Konfiguration för att hantera authentication med JWT i Swagger
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
                            Reference=new OpenApiReference{Type =ReferenceType.SecurityScheme, Id="Bearer"}

                        },
                        Array.Empty<string>()
                    }
                });

            });


            return services;

        }

        public static IApplicationBuilder UseSwaggerExtended(this IApplicationBuilder app)
        {

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cissis BankApp API");
                options.RoutePrefix = string.Empty;
                //Den här raden gör att man inte behöver skriva in swagger i sin url hela tiden, apiet öppnas automagiskt i swagger.  
            });



            return app;
        }

    }
}
