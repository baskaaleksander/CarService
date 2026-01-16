using System.ComponentModel.DataAnnotations;

namespace CarService.Validation
{
    public class PositiveDecimalAttribute : ValidationAttribute
    {
        public double Minimum { get; set; } = 0.01;

        public PositiveDecimalAttribute()
        {
            ErrorMessage = "Value must be a positive number";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            decimal decimalValue;

            if (value is decimal d)
                decimalValue = d;
            else if (value is double dbl)
                decimalValue = (decimal)dbl;
            else if (value is float f)
                decimalValue = (decimal)f;
            else if (value is int i)
                decimalValue = i;
            else if (!decimal.TryParse(value.ToString(), out decimalValue))
                return new ValidationResult("Value must be a valid number");

            if (decimalValue < (decimal)Minimum)
            {
                var fieldName = validationContext.DisplayName ?? validationContext.MemberName ?? "Value";
                return new ValidationResult(ErrorMessage ?? $"{fieldName} must be at least {Minimum}");
            }

            return ValidationResult.Success;
        }
    }
}
