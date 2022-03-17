#nullable disable
namespace BulkyBook.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public decimal TotalPrice { get; set; }

        public IEnumerable<ShoppingCart> Items { get; set; }
    }
}
