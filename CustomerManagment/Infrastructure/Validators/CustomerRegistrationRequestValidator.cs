using CustomerManagment.Infrastructure.Extensions;
using CustomerManagment.Models.Requests;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Windows.Markup;

namespace CustomerManagment.Infrastructure.Validators
{
    public class CustomerRegistrationRequestValidator : AbstractValidator<CustomerRegistrationRequest>
    {
        public CustomerRegistrationRequestValidator() // constructor
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(nameof(CustomerRegistrationRequest.FirstName) + " must not be empty");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(nameof(CustomerRegistrationRequest.LastName) + " must not be empty");

            RuleFor(x => x.UserName)
                .NotEmpty()
                .Must(value => value.Length >= 5)
                .WithMessage(nameof(CustomerRegistrationRequest.UserName) + " must be at least 5 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .Password(); // from CustomerManagment.Infrastructure.Extensions -> ValidationRuleExtension
        }


    }
}
