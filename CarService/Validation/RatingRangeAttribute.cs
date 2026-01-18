using System.ComponentModel.DataAnnotations;

namespace CarService.Validation
{
    public class RatingRangeAttribute : ValidationAttribute
    {
        public int Minimum { get; set; } = 1;
        public int Maximum { get; set; } = 5;

        public RatingRangeAttribute()
        {
            ErrorMessage = "Ocena musi być w zakresie od 1 do 5 gwiazdek";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is not int rating)
            {
                if (!int.TryParse(value.ToString(), out rating))
                    return new ValidationResult("Ocena musi być poprawną liczbą całkowitą");
            }

            if (rating < Minimum || rating > Maximum)
                return new ValidationResult(ErrorMessage ?? $"Ocena musi być w zakresie od {Minimum} do {Maximum} gwiazdek");

            return ValidationResult.Success;
        }
    }
}
