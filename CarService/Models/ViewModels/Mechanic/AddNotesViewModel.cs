using System.ComponentModel.DataAnnotations;

namespace CarService.Models.ViewModels.Mechanic
{
    public class AddNotesViewModel
    {
        public int OrderId { get; set; }
        public string? ExistingNotes { get; set; }

        [Required]
        [StringLength(2000)]
        public string Notes { get; set; } = string.Empty;
    }
}
