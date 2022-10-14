using CustomerManagment.Domain.POCO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagment.Data.Abstraction
{
    public interface ICustomerRepository
    {
        Task<int> CreateAsync (Customer customer);
        Task<bool> VerifyAsync(string guid);
        Task<bool> UpdatePasswordAsync (string email, string newPassword, string currentPassword);
        Task<Customer> GetAsync (string email, string password);
        Task<bool> DeleteAsync (string email);
        Task<bool> Exists(string email);
    }
}
