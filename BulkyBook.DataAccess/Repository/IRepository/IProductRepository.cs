using BulkyBook.Models;
using BulkyBook.Models.ViewModels;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<ProductDT>> GetProductsDT();

        void Update(Product product);
    }
}
