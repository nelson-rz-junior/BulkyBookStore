using BulkyBook.Models;
using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface ICoverTypeRepository : IRepository<CoverType>
    {
        bool Exists(Expression<Func<CoverType, bool>> filter);

        void Update(CoverType coverType);
    }
}
