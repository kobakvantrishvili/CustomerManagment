using CustomerManagment.Data.Models;
using CustomerManagment.infrastructure.Mappings;
using CustomerManagment.Infrastructure.Extensions;
using CustomerManagment.Services.Extensions;
using CustomerManagment.Services.Models.JWT;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using FluentValidation.AspNetCore;
using Microsoft.CodeAnalysis.Options;

namespace CustomerManagment
{
    public class Startup
    {   // Goal is to craete application that will do CRUD operations on User

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //add fluent validations
            services.AddControllers().AddFluentValidation(configurationExpression => configurationExpression.RegisterValidatorsFromAssemblyContaining<Startup>());
            // Registers configuration instance IOptions will bind against
            services.Configure<ConnectionStrings>(Configuration.GetSection(nameof(ConnectionStrings)));
            services.Configure<JWTConfig>(Configuration.GetSection(nameof(JWTConfig)));
            // dependancy injection
            services.AddServices(); // in Services.Extensions
            // JWT token Authentication configuration
            services.AddTokenAuthentiction(Configuration);
            // Registers mappings for for translating data between layers
            services.RegisterMappings();
            // Swagger : for checking and testing Actions
            services.AddSwaggerGen(options =>
            {
                // defining docs to be created by swagger generator
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer API", Version = "v1" });

                // describes how API is protected
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer abcdef\"",
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
                        new string[] {}
                    }
                });

                // For XML comments
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
