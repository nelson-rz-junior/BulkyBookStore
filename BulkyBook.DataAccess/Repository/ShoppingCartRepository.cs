using BulkyBook.DataAccess.Context;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public int IncrementQuantity(ShoppingCart shoppingCart, int quantity)
        {
            shoppingCart.Quantity += quantity;
            return shoppingCart.Quantity;
        }

        public int DecrementQuantity(ShoppingCart shoppingCart, int quantity)
        {
            shoppingCart.Quantity -= quantity;
            return shoppingCart.Quantity;
        }

        public decimal SetUnitPrice(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Quantity > 0 && shoppingCart.Quantity <= 49)
            {
                shoppingCart.UnitPrice = shoppingCart.Product.Price;
            }
            else if (shoppingCart.Quantity > 49 && shoppingCart.Quantity <= 99)
            {
                shoppingCart.UnitPrice = shoppingCart.Product.Price50;
            }
            else if (shoppingCart.Quantity > 99)
            {
                shoppingCart.UnitPrice = shoppingCart.Product.Price100;
            }

            return shoppingCart.UnitPrice;
        }

        public decimal SetFinalPrice(ShoppingCart shoppingCart)
        {
            shoppingCart.FinalPrice = shoppingCart.UnitPrice * shoppingCart.Quantity;
            return shoppingCart.FinalPrice;
        }
    }
}
