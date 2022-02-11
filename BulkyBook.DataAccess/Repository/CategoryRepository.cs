using BulkyBook.DataAccess.Context;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public bool Exists(Expression<Func<Category, bool>> filter)
        {
            return _context.Categories.Any(filter);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }
    }
}
