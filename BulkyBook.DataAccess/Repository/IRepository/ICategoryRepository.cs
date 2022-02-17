using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        bool Exists(Expression<Func<Category, bool>> filter);

        void Update(Category category);

        IEnumerable<SelectListItem> GetCategoryListForDropDown();
    }
}
