#nullable disable
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The street address is required")]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "The city is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "The state is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "The postal code is required")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "The phone number is required")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
