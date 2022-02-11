using BulkyBook.Models;
using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        bool Exists(Expression<Func<Category, bool>> filter);

        void Update(Category category);
    }
}
