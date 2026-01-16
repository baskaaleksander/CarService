using System.ComponentModel.DataAnnotations;

namespace CarService.Validation
{
    public class RatingRangeAttribute : ValidationAttribute
    {
        public int Minimum { get; set; } = 1;
        public int Maximum { get; set; } = 5;

        public RatingRangeAttribute()
        {
            ErrorMessage = "Rating must be between 1 and 5 stars";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is not int rating)
            {
                if (!int.TryParse(value.ToString(), out rating))
                    return new ValidationResult("Rating must be a valid integer");
            }

            if (rating < Minimum || rating > Maximum)
                return new ValidationResult(ErrorMessage ?? $"Rating must be between {Minimum} and {Maximum} stars");

            return ValidationResult.Success;
        }
    }
}
