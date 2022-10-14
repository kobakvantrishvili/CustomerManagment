using CustomerManagment.Data.Extension;
using CustomerManagment.Services.Abstraction;
using CustomerManagment.Services.Implementatin;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerManagment.Services.Extensions
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IJWTService, JWTService>();

            services.AddRepositories();
        }
    }
}
