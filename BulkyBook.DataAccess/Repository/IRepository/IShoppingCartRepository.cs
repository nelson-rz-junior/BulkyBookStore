using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncrementQuantity(ShoppingCart shoppingCart, int quantity);

        int DecrementQuantity(ShoppingCart shoppingCart, int quantity);

        decimal SetUnitPrice(ShoppingCart shoppingCart);

        decimal SetFinalPrice(ShoppingCart shoppingCart);
    }
}
