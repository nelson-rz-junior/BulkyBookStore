using BulkyBook.DataAccess.Context;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public bool Exists(Expression<Func<CoverType, bool>> filter)
        {
            return _context.CoverTypes.Any(filter);
        }

        public void Update(CoverType coverType)
        {
            _context.CoverTypes.Update(coverType);
        }
    }
}
