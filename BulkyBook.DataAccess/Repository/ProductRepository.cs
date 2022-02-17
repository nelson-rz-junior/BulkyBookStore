using BulkyBook.DataAccess.Context;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var currentProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (currentProduct != null)
            {
                currentProduct.Title = product.Title;
                currentProduct.Author = product.Author;
                currentProduct.ISBN = product.ISBN;
                currentProduct.Description = product.Description;
                currentProduct.Price = product.Price;
                currentProduct.Price50 = product.Price50;
                currentProduct.Price100 = product.Price100;
                currentProduct.ListPrice = product.ListPrice;
                currentProduct.CategoryId = product.CategoryId;
                currentProduct.CoverTypeId = product.CoverTypeId;
                currentProduct.Image = product.Image;
            }
        }
    }
}
