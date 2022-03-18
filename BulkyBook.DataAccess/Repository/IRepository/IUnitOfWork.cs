namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }

        ICoverTypeRepository CoverTypeRepository { get; }

        IProductRepository ProductRepository { get; }

        ICompanyRepository CompanyRepository { get; }

        IShoppingCartRepository ShoppingCartRepository { get; }

        IApplicationUserRepository ApplicationUserRepository { get; }

        IOrderHeaderRepository OrderHeaderRepository { get; }

        IOrderDetailRepository OrderDetailRepository { get; }

        Task SaveChangesAsync();
    }
}
