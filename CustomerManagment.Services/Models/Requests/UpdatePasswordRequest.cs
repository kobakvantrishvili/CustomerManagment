using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerManagment.Services.Models.Requests
{
    public class UpdatePasswordRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
