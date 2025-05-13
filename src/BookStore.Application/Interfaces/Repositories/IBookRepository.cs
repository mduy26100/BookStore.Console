using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
        Task AddBookWithCategoriesAsync(Book book, List<int> categoryIds);
        Task<Book> GetDetailBook(Book book);
        Task<Book> GetBookWithCategories(int bookId);
        Task<Book> GetBookWithReports(int bookId);
        Task<IEnumerable<Book>> GetPagedBooksAsync(int pageNumber, int pageSize);
        Task<int> GetTotalBooksCountAsync();
        Task UpdateTitle(Book book);
        Task UpdateAuthor(Book book);
        Task UpdatePrice(Book book);
        Task UpdateStock(Book book);
        Task UpdateDescription(Book book);
        Task<bool> BookExistsAsync(string title, string author);
        Task AddToCartAsync(int accountId, int bookId, int quantity);
        Task<bool> CheckBookInStockAsync(int bookId, int quantity);
        Task<IEnumerable<Book>> SearchBooksByNameAsync(string searchTerm);
        Task<IEnumerable<Book>> GetBooksSortedByPriceAsync(bool ascending);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
        Task<bool> CheckAndLockStockAsync(int bookId, int quantity);
        Task UpdateStockAfterPurchaseAsync(int bookId, int quantity);
        Task<Book> GetByIdForUpdateAsync(int bookId);
    }
}
