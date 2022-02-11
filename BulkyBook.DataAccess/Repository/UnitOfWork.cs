using BulkyBook.DataAccess.Context;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository CategoryRepository { get; private set; }

        public ICoverTypeRepository CoverTypeRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            CategoryRepository = new CategoryRepository(_context);
            CoverTypeRepository = new CoverTypeRepository(_context);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
