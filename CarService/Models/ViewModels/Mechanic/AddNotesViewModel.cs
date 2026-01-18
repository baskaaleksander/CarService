using System.ComponentModel.DataAnnotations;

namespace CarService.Models.ViewModels.Mechanic
{
    public class AddNotesViewModel
    {
        public int OrderId { get; set; }
        public string? ExistingNotes { get; set; }

        [Required(ErrorMessage = "Notatki są wymagane")]
        [StringLength(2000, ErrorMessage = "Notatki nie mogą przekraczać 2000 znaków")]
        [Display(Name = "Notatki")]
        public string Notes { get; set; } = string.Empty;
    }
}
