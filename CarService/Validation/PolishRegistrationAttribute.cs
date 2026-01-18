using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CarService.Validation
{
    public class PolishRegistrationAttribute : ValidationAttribute
    {
        private static readonly Regex RegistrationRegex = new Regex(
            @"^[A-Z]{2,3}\s?[A-Z0-9]{4,5}$",
            RegexOptions.Compiled);

        public PolishRegistrationAttribute()
        {
            ErrorMessage = "Nieprawidłowy format polskiego numeru rejestracyjnego. Oczekiwany format: 2-3 litery i 4-5 znaków alfanumerycznych (np. WA12345, KR ABC12)";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var registration = value.ToString();

            if (string.IsNullOrWhiteSpace(registration))
                return new ValidationResult(ErrorMessage);

            var normalized = registration.Trim().ToUpperInvariant();

            if (!RegistrationRegex.IsMatch(normalized))
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
