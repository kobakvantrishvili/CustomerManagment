using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerManagment.Services.Models
{
    public class CustomerServiceModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Guid { get; set; }
        public bool IsVerified { get; set; }
    }
}
