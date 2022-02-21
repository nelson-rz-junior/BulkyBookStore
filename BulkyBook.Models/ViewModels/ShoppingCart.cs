#nullable disable
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models.ViewModels
{
    public class ShoppingCart
    {
        public Product Product { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a value bewteen 1 and 1000")]
        public int Quantity { get; set; }
    }
}
