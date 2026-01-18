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
            ErrorMessage = "VIN musi mieć dokładnie 17 znaków i nie może zawierać liter I, O ani Q";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var vin = value.ToString();

            if (string.IsNullOrWhiteSpace(vin))
                return new ValidationResult(ErrorMessage);

            if (vin.Length != 17)
                return new ValidationResult("VIN musi mieć dokładnie 17 znaków");

            if (!VinRegex.IsMatch(vin))
                return new ValidationResult("VIN nie może zawierać liter I, O ani Q i może składać się tylko ze znaków alfanumerycznych");

            return ValidationResult.Success;
        }
    }
}
