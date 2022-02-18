#nullable disable
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The ISBN is required")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "The author is required")]
        public string Author { get; set; }

        [Display(Name = "List Price")]
        public decimal ListPrice { get; set; }

        [Display(Name = "Price for 1-50")]
        public decimal Price { get; set; }

        [Display(Name = "Price for 51-100")]
        public decimal Price50 { get; set; }

        [Display(Name = "Price for 100+")]
        public decimal Price100 { get; set; }

        public byte[] Image { get; set; }

        [Required(ErrorMessage = "The category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        [Required(ErrorMessage = "The cover type is required")]
        [Display(Name = "Cover Type")]
        public int CoverTypeId { get; set; }

        public CoverType CoverType { get; set; }
    }
}
