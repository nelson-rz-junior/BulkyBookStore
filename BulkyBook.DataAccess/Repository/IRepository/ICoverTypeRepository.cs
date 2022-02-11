using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface ICoverTypeRepository : IRepository<CoverTypeRepository>
    {
        void Update(CoverType coverType);
    }
}
