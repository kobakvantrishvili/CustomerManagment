using System;
using Mapster;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CustomerManagment.Services.Abstraction;
using CustomerManagment.Services.Models;
using Microsoft.AspNetCore.Mvc;
using CustomerManagment.Data.Abstraction;
using CustomerManagment.Domain.POCO;
using CustomerManagment.Services.Models.Requests;
using CustomerManagment.Services.Models.Eums;
using System.Net.Mail;
using System.Net;

namespace CustomerManagment.Services.Implementatin
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IJWTService _JWTService;

        private async Task SendMailAync(string guid, string email)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("koba.kvantro@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Test";
                mail.Body = $"Confirmation link: https://localhost:44338/Customer/Activate?guid={guid}";
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("kvantroxbox@gmail.com", "Kvantro2001"); // doesn't work since google don't allow less secure apps access to the gmail anymore
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }

        }

        public CustomerService(ICustomerRepository repository, IJWTService jWTService)
        {
            _repository = repository;
            _JWTService = jWTService;
        }


    public async Task<(Status, string)> CreateCustomerAsync(CustomerServiceModel customer)
        {
            var exists = await _repository.Exists(customer.Email);

            if (exists)
                return (Status.Conflict, null);

            customer.Guid = Guid.NewGuid().ToString();
            customer.IsVerified = false;
            var customerAddedId = await _repository.CreateAsync(customer.Adapt<Customer>()).ConfigureAwait(false);

            //await SendMailAync(customer.Guid, customer.Email);

            return (Status.Success, customerAddedId.ToString());
        }

        public async Task<Status> VerifyCustomerAsync(string customerGuid)
        {
            var verified = await _repository.VerifyAsync(customerGuid).ConfigureAwait(false);

            if (!verified)
                return (Status.NotFound);

            return (Status.Success);
        }

        public async Task<Status> UpdateCustomerPasswordAsync(UpdatePasswordRequest request)
        {
            if(request.NewPassword == request.Password)
            {
                return Status.Conflict;
            }

            var updated = await _repository.UpdatePasswordAsync(request.Email, request.NewPassword, request.Password).ConfigureAwait(false);

            if (!updated)
                return (Status.NotFound);

            return (Status.Success);
        }

        public async Task<Status> DeleteCustomerAsync(string email)
        {
            var deleted = await _repository.DeleteAsync(email).ConfigureAwait(false);

            if (!deleted)
                return (Status.NotFound);

            return (Status.Success);
        }

        public async Task<(Status, string)> AuthenticateCustomerAsync(string customerEmail, string customerPassword)
        {
            string token = string.Empty;
            var customerEntity = await _repository.GetAsync(customerEmail, customerPassword).ConfigureAwait(false);

            if (customerEntity != null)
            {
                //generate token
                token = _JWTService.GenerateSecurityToken(customerEntity.Email); // returns serialized JWT token
                return (Status.Success, token);
            }

            return (Status.Unauthorized, token);
        }
    }
}
