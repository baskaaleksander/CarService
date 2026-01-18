using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CarService.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public ICollection<ServiceOrder> ClientOrders { get; set; } = new List<ServiceOrder>();

        public ICollection<ServiceOrder> MechanicOrders { get; set; } = new List<ServiceOrder>();
    }
}
