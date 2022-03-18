#nullable disable
namespace BulkyBook.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public OrderHeader OrderHeader { get; set; }

        public IEnumerable<ShoppingCart> Items { get; set; }
    }
}
