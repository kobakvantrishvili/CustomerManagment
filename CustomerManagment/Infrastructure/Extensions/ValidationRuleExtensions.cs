using CustomerManagment.Models.Requests;
using FluentValidation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;

namespace CustomerManagment.Infrastructure.Extensions
{
    public static class ValidationRuleExtensions
    {
        public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(value => value.Length >= 8).Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$").WithMessage(nameof(CustomerRegistrationRequest.Password) + " must contain at least 1 lowercase, 1 uppercase and 1 numeric character and has to be longer than 8 characters"); ;
        }
    }
}
