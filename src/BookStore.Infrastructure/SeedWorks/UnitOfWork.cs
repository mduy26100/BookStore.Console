using BookStore.Application.Interfaces.Repositories;
using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookStore.Infrastructure.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        public IAccountRepository AccountRepository { get; set; }

        public IBookRepository BookRepository { get; set; }

        public ICategoryRepository CategoryRepository { get; set; }

        public IOrderRepository OrderRepository { get; set; }
        public IOrderDetailRepository OrderDetailRepository { get; set; }

        public IReportRepository ReportRepository { get; set; }

        public IShoppingCartRepository ShoppingCartRepository {  get; set; }

        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            AccountRepository = new AccountRepository(context);
            BookRepository = new BookRepository(context);
            CategoryRepository = new CategoryRepository(context);
            OrderRepository = new OrderRepository(context);
            OrderDetailRepository = new OrderDetailRepository(context);
            ReportRepository = new ReportRepository(context);
            ShoppingCartRepository = new ShoppingCartRepository(context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChange()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
