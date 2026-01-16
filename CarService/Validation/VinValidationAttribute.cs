using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CarService.Validation
{
    public class VinValidationAttribute : ValidationAttribute
    {
        private static readonly Regex VinRegex = new Regex(
            @"^[A-HJ-NPR-Z0-9]{17}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public VinValidationAttribute()
        {
            ErrorMessage = "VIN must be exactly 17 characters and cannot contain letters I, O, or Q";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var vin = value.ToString();

            if (string.IsNullOrWhiteSpace(vin))
                return new ValidationResult(ErrorMessage);

            if (vin.Length != 17)
                return new ValidationResult("VIN must be exactly 17 characters");

            if (!VinRegex.IsMatch(vin))
                return new ValidationResult("VIN cannot contain letters I, O, or Q and must only contain alphanumeric characters");

            return ValidationResult.Success;
        }
    }
}
