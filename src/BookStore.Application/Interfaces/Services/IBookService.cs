using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBook();
        Task<IEnumerable<BookDto>> GetBooksPaged(int pageNumber, int pageSize);
        Task<int> GetTotalBooksCount();
        Task<BookDto> GetDetailsBook(int bookId);
        Task<BookDto> GetDetailsBook(BookDto bookDto);
        Task CreateBook(BookDto bookDto, List<int> categoryIdBook);
        Task UpdateAuthorBook(BookDto bookDto);
        Task UpdateDescriptionBook(BookDto bookDto);
        Task UpdatePriceBook(BookDto bookDto);
        Task UpdateStockBook(BookDto bookDto);
        Task UpdateTitleBook(BookDto bookDto);
        Task DeleteBook(int bookId);
        Task<bool> BookExists(string title, string author);
        Task<List<ReportDto>> GetBookReports(int bookId);
        Task AddToCart(int accountId, int bookId, int quantity);
        Task<bool> CheckBookInStock(int bookId, int quantity);
        Task<IEnumerable<BookDto>> SearchBooksByName(string searchTerm);
        Task<IEnumerable<BookDto>> GetBooksSortedByPrice(bool ascending);
        Task<IEnumerable<BookDto>> GetBooksByCategory(int categoryId);
    }
}
