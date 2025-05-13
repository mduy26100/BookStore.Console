using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Application.Interfaces.Services;
using BookStore.Domain.Entities;

namespace BookStore.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddToCart(int accountId, int bookId, int quantity)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID");

            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            try
            {
                bool inStock = await _unitOfWork.BookRepository.CheckBookInStockAsync(bookId, quantity);
                if (!inStock)
                {
                    throw new Exception("Requested quantity is not available in stock");
                }

                await _unitOfWork.BookRepository.AddToCartAsync(accountId, bookId, quantity);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add book to cart: {ex.Message}", ex);
            }
        }

        public async Task<bool> BookExists(string title, string author)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Book title cannot be empty");

            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Book author cannot be empty");

            try
            {
                return await _unitOfWork.BookRepository.BookExistsAsync(title, author);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check if book exists: {ex.Message}", ex);
            }
        }

        public async Task<bool> CheckBookInStock(int bookId, int quantity)
        {
            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            try
            {
                return await _unitOfWork.BookRepository.CheckBookInStockAsync(bookId, quantity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check book stock: {ex.Message}", ex);
            }
        }

        public async Task CreateBook(BookDto bookDto, List<int> categoryIdBook)
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            if (string.IsNullOrWhiteSpace(bookDto.Title))
                throw new ArgumentException("Book title cannot be empty");

            if (string.IsNullOrWhiteSpace(bookDto.Author))
                throw new ArgumentException("Book author cannot be empty");

            if (bookDto.Price <= 0)
                throw new ArgumentException("Book price must be greater than 0");

            if (bookDto.Stock < 0)
                throw new ArgumentException("Book stock cannot be negative");

            try
            {
                bool exists = await _unitOfWork.BookRepository.BookExistsAsync(bookDto.Title, bookDto.Author);
                if (exists)
                {
                    throw new Exception($"A book with title '{bookDto.Title}' by author '{bookDto.Author}' already exists");
                }

                await _unitOfWork.BookRepository.AddBookWithCategoriesAsync(_mapper.Map<Book>(bookDto), categoryIdBook);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create book: {ex.Message}", ex);
            }
        }

        public async Task DeleteBook(int bookId)
        {
            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID");

            try
            {
                await _unitOfWork.BookRepository.RemoveAsync(bookId);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete book: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BookDto>> GetAllBook()
        {
            try
            {
                var list = await _unitOfWork.BookRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<BookDto>>(list);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve books: {ex.Message}", ex);
            }
        }

        public async Task<List<ReportDto>> GetBookReports(int bookId)
        {
            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID");

            try
            {
                var book = await _unitOfWork.BookRepository.GetBookWithReports(bookId);
                if (book == null)
                {
                    throw new Exception($"Book with ID {bookId} not found");
                }

                var reportDtos = _mapper.Map<List<ReportDto>>(book.Reports);

                foreach (var report in reportDtos)
                {
                    var reportEntity = book.Reports.FirstOrDefault(r => r.ReportID == report.ReportID);
                    if (reportEntity?.Order?.Account != null)
                    {
                        report.CustomerName = reportEntity.Order.Account.Name;
                    }
                    else
                    {
                        report.CustomerName = "Anonymous";
                    }
                }

                return reportDtos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve book reports: {ex.Message}", ex);
            }
        }

        public async Task<BookDto> GetDetailsBook(int bookId)
        {
            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID");

            try
            {
                var book = await _unitOfWork.BookRepository.GetBookWithCategories(bookId);
                if (book == null)
                {
                    throw new Exception($"Book with ID {bookId} not found");
                }

                var bookDto = _mapper.Map<BookDto>(book);

                var bookWithReports = await _unitOfWork.BookRepository.GetBookWithReports(bookId);
                if (bookWithReports != null && bookWithReports.Reports != null)
                {
                    bookDto.Reports = _mapper.Map<List<ReportDto>>(bookWithReports.Reports);

                    foreach (var report in bookDto.Reports)
                    {
                        var reportEntity = bookWithReports.Reports.FirstOrDefault(r => r.ReportID == report.ReportID);
                        if (reportEntity?.Order?.Account != null)
                        {
                            report.CustomerName = reportEntity.Order.Account.Name;
                        }
                        else
                        {
                            report.CustomerName = "Anonymous";
                        }
                    }
                }

                return bookDto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve book details: {ex.Message}", ex);
            }
        }

        public async Task<BookDto> GetDetailsBook(BookDto bookDto)
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            if (bookDto.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            try
            {
                var book = await _unitOfWork.BookRepository.GetDetailBook(_mapper.Map<Book>(bookDto));
                return _mapper.Map<BookDto>(book);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve book details: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BookDto>> GetBooksPaged(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0");

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than 0");

            try
            {
                var books = await _unitOfWork.BookRepository.GetPagedBooksAsync(pageNumber, pageSize);
                return _mapper.Map<IEnumerable<BookDto>>(books);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve paged books: {ex.Message}", ex);
            }
        }

        public async Task<int> GetTotalBooksCount()
        {
            try
            {
                return await _unitOfWork.BookRepository.GetTotalBooksCountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get total books count: {ex.Message}", ex);
            }
        }

        public async Task UpdateAuthorBook(BookDto bookDto)
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            if (bookDto.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            if (string.IsNullOrWhiteSpace(bookDto.Author))
                throw new ArgumentException("Book author cannot be empty");

            try
            {
                await _unitOfWork.BookRepository.UpdateAuthor(_mapper.Map<Book>(bookDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book author: {ex.Message}", ex);
            }
        }

        public async Task UpdateDescriptionBook(BookDto bookDto)
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            if (bookDto.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            try
            {
                await _unitOfWork.BookRepository.UpdateDescription(_mapper.Map<Book>(bookDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book description: {ex.Message}", ex);
            }
        }

        public async Task UpdatePriceBook(BookDto bookDto)
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            if (bookDto.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            if (bookDto.Price <= 0)
                throw new ArgumentException("Book price must be greater than 0");

            try
            {
                await _unitOfWork.BookRepository.UpdatePrice(_mapper.Map<Book>(bookDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book price: {ex.Message}", ex);
            }
        }

        public async Task UpdateStockBook(BookDto bookDto)
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            if (bookDto.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            if (bookDto.Stock < 0)
                throw new ArgumentException("Book stock cannot be negative");

            try
            {
                await _unitOfWork.BookRepository.UpdateStock(_mapper.Map<Book>(bookDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book stock: {ex.Message}", ex);
            }
        }

        public async Task UpdateTitleBook(BookDto bookDto)
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            if (bookDto.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            if (string.IsNullOrWhiteSpace(bookDto.Title))
                throw new ArgumentException("Book title cannot be empty");

            try
            {
                await _unitOfWork.BookRepository.UpdateTitle(_mapper.Map<Book>(bookDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book title: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BookDto>> SearchBooksByName(string searchTerm)
        {
            var books = await _unitOfWork.BookRepository.SearchBooksByNameAsync(searchTerm);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksSortedByPrice(bool ascending)
        {
            var books = await _unitOfWork.BookRepository.GetBooksSortedByPriceAsync(ascending);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByCategory(int categoryId)
        {
            var books = await _unitOfWork.BookRepository.GetBooksByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
    }
}
