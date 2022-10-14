using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CustomerManagment.Services.Models;
using CustomerManagment.Services.Models.Eums;
using CustomerManagment.Services.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagment.Services.Abstraction
{
    public interface ICustomerService
    {
        Task<(Status, string)> CreateCustomerAsync(CustomerServiceModel customer);
        Task<Status> VerifyCustomerAsync(string customerGuid);
        Task<Status> UpdateCustomerPasswordAsync(UpdatePasswordRequest request);
        Task<Status> DeleteCustomerAsync(string email);
        Task<(Status, string)> AuthenticateCustomerAsync(string customerEmail, string customerPassword);
    }
}
