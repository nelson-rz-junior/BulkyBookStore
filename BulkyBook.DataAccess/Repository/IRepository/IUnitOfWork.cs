namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }

        ICoverTypeRepository CoverTypeRepository { get; }

        IProductRepository ProductRepository { get; }

        Task SaveChangesAsync();
    }
}
