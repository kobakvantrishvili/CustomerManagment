using CustomerManagment.Models.DTOs;
using CustomerManagment.Models.Requests;
using CustomerManagment.Services.Abstraction;
using CustomerManagment.Services.Models;
using CustomerManagment.Services.Models.Requests;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CustomerManagment.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service) // dependancy injection
        {
            _service = service;
        }


        /// <summary>
        /// Customer Registration Action Method
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Register
        ///     {
        ///         "firstName": "Koba",
        ///         "lastName": "Kvantrishvili",
        ///         "userName": "Kvantro",
        ///         "password": "Kvantro123",
        ///         "email": "koba.kvantro@gmail.com"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Created a new customer data entry</response>
        /// <response code="409">Customer with same gmail already exists</response>
        [Route("Register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(CustomerRegistrationRequest customer)
        {
            var result = await _service.CreateCustomerAsync(customer.Adapt<CustomerServiceModel>());

            return StatusCode((int)result.Item1, result.Item2);
        }

        [Authorize]
        [Route("Activate")]
        [HttpPost]
        public async Task<IActionResult> Activate(string guid)
        {
            var result = await _service.VerifyCustomerAsync(guid);

            return StatusCode((int)result);
        }

        /// <summary>
        /// Customer Log In Action Method
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /LogIn
        ///     {
        ///         "email": "koba.kvantro@gmail.com",
        ///         "password": "Kvantro123"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Succesfully logged in</response>
        /// <response code="401">Customer authentication failed</response>
        [Route("LogIn")]
        [HttpPost]
        public async Task<IActionResult> LogIn(CustomerLogInRequest request)
        {
            var result = await _service.AuthenticateCustomerAsync(request.Email, request.Password);

            return StatusCode((int)result.Item1, result.Item2);
        }

        [Route("UpdatePassword")]
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(CustomerUpdatePasswordRequest request)
        {
            var result = await _service.UpdateCustomerPasswordAsync(request.Adapt<UpdatePasswordRequest>());

            return StatusCode((int)result);
        }

        [Route("DeleteAccount")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount(string email)
        {
            var result = await _service.DeleteCustomerAsync(email);

            return StatusCode((int)result);
        }




    }
}
