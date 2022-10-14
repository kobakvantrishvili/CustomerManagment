namespace CustomerManagment.Models.Requests
{
    public class CustomerUpdatePasswordRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
