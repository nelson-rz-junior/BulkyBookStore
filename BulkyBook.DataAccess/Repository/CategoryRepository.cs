using BulkyBook.DataAccess.Context;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IEnumerable<SelectListItem> GetCategoryListForDropDown()
        {
            return _context.Categories.Select(ct => new SelectListItem
            {
                Text = ct.Name,
                Value = ct.Id.ToString()
            });
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }
    }
}
