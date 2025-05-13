using BookStore.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;


namespace BookStore.Application.Interfaces.SeedWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        IBookRepository BookRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IReportRepository ReportRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        Task<int> SaveChange();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
