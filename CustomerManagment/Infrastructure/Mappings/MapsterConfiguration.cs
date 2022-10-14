using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using CustomerManagment.Domain.POCO;
using CustomerManagment.Services.Models;
using CustomerManagment.Models.DTOs;
using CustomerManagment.Models.Requests;
using CustomerManagment.Services.Models.Requests;

namespace CustomerManagment.infrastructure.Mappings
{
    public static class MapsterConfiguration
    {
        // we will add this service in Startup.cs ConfigureServices method
        public static void RegisterMappings(this IServiceCollection services) // IServiceCollection extension method
        {
            TypeAdapterConfig<CustomerServiceModel, Customer>
                .NewConfig()
                .TwoWays();

            TypeAdapterConfig<CustomerRegistrationRequest, CustomerServiceModel>
                .NewConfig()
                .TwoWays();

            TypeAdapterConfig<CustomerDTO, CustomerServiceModel>
                .NewConfig()
                .TwoWays();

            TypeAdapterConfig<CustomerUpdatePasswordRequest, UpdatePasswordRequest>
                .NewConfig()
                .TwoWays();
        }
    }
}
