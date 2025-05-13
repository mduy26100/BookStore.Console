using BookStore.Application.Interfaces.Repositories;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddBookWithCategoriesAsync(Book book, List<int> categoryIds)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            if (categoryIds == null)
                throw new ArgumentNullException(nameof(categoryIds));

            try
            {
                await _context.Books.AddAsync(book);

                book.BookCategories = categoryIds.Select(categoryId => new BookCategory
                {
                    Book = book,
                    CategoryID = categoryId
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add book with categories: {ex.Message}", ex);
            }
        }

        public async Task AddToCartAsync(int accountId, int bookId, int quantity)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID");

            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            try
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    throw new Exception($"Book with ID {bookId} not found");
                }

                var account = await _context.Accounts.FindAsync(accountId);
                if (account == null)
                {
                    throw new Exception($"Account with ID {accountId} not found");
                }

                var existingCartItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.AccountID == accountId && sc.BookID == bookId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                    _context.ShoppingCarts.Update(existingCartItem);
                }
                else
                {
                    var cartItem = new ShoppingCart
                    {
                        AccountID = accountId,
                        BookID = bookId,
                        Quantity = quantity,
                        CreatedAt = DateTime.Now
                    };

                    await _context.ShoppingCarts.AddAsync(cartItem);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add book to cart: {ex.Message}", ex);
            }
        }

        public async Task<bool> BookExistsAsync(string title, string author)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Book title cannot be empty");

            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Book author cannot be empty");

            try
            {
                return await _context.Books.AnyAsync(b =>
                    b.Title.ToLower() == title.ToLower() &&
                    b.Author.ToLower() == author.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check if book exists: {ex.Message}", ex);
            }
        }

        public async Task<bool> CheckBookInStockAsync(int bookId, int quantity)
        {
            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            try
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    throw new Exception($"Book with ID {bookId} not found");
                }

                return book.Stock >= quantity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check book stock: {ex.Message}", ex);
            }
        }

        public async Task<Book> GetBookWithCategories(int bookId)
        {
            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID");

            try
            {
                return await _context.Books
                    .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                    .FirstOrDefaultAsync(b => b.BookID == bookId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get book with categories: {ex.Message}", ex);
            }
        }

        public async Task<Book> GetBookWithReports(int bookId)
        {
            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID");

            try
            {
                return await _context.Books
                    .Include(b => b.Reports)
                    .ThenInclude(r => r.Order)
                    .ThenInclude(o => o.Account)
                    .FirstOrDefaultAsync(b => b.BookID == bookId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get book with reports: {ex.Message}", ex);
            }
        }

        public async Task<Book> GetDetailBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            if (book.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            try
            {
                var entity = await _context.Books.FindAsync(book.BookID);
                if (entity == null)
                {
                    throw new Exception($"Book with ID {book.BookID} not found");
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get book details: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Book>> GetPagedBooksAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0");

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than 0");

            try
            {
                return await _context.Books
                    .OrderBy(b => b.BookID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get paged books: {ex.Message}", ex);
            }
        }

        public async Task<int> GetTotalBooksCountAsync()
        {
            try
            {
                return await _context.Books.CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get total books count: {ex.Message}", ex);
            }
        }

        public async Task UpdateAuthor(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            if (book.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Book author cannot be empty");

            try
            {
                var entity = await _context.Books.FindAsync(book.BookID);
                if (entity == null)
                {
                    throw new Exception($"Book with ID {book.BookID} not found");
                }
                entity.Author = book.Author;
                _context.Books.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book author: {ex.Message}", ex);
            }
        }

        public async Task UpdateDescription(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            if (book.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            try
            {
                var entity = await _context.Books.FindAsync(book.BookID);
                if (entity == null)
                {
                    throw new Exception($"Book with ID {book.BookID} not found");
                }
                entity.Description = book.Description;
                _context.Books.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book description: {ex.Message}", ex);
            }
        }

        public async Task UpdatePrice(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            if (book.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            if (book.Price <= 0)
                throw new ArgumentException("Book price must be greater than 0");

            try
            {
                var entity = await _context.Books.FindAsync(book.BookID);
                if (entity == null)
                {
                    throw new Exception($"Book with ID {book.BookID} not found");
                }
                entity.Price = book.Price;
                _context.Books.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book price: {ex.Message}", ex);
            }
        }

        public async Task UpdateStock(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            if (book.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            if (book.Stock < 0)
                throw new ArgumentException("Book stock cannot be negative");

            try
            {
                var entity = await _context.Books.FindAsync(book.BookID);
                if (entity == null)
                {
                    throw new Exception($"Book with ID {book.BookID} not found");
                }
                entity.Stock = book.Stock;
                _context.Books.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book stock: {ex.Message}", ex);
            }
        }

        public async Task UpdateTitle(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            if (book.BookID <= 0)
                throw new ArgumentException("Invalid book ID");

            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Book title cannot be empty");

            try
            {
                var entity = await _context.Books.FindAsync(book.BookID);
                if (entity == null)
                {
                    throw new Exception($"Book with ID {book.BookID} not found");
                }
                entity.Title = book.Title;
                _context.Books.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update book title: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Book>> SearchBooksByNameAsync(string searchTerm)
        {
            return await _context.Books
                .Where(b => b.Title.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksSortedByPriceAsync(bool ascending)
        {
            if (ascending)
            {
                return await _context.Books
                    .OrderBy(b => b.Price)
                    .ToListAsync();
            }
            else
            {
                return await _context.Books
                    .OrderByDescending(b => b.Price)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _context.Books
                .Include(b => b.BookCategories)
                .Where(b => b.BookCategories.Any(bc => bc.CategoryID == categoryId))
                .ToListAsync();
        }

        public async Task<bool> CheckAndLockStockAsync(int bookId, int quantity)
        {
            var book = await _context.Books
                .FromSqlRaw("SELECT * FROM Books WITH (UPDLOCK, ROWLOCK) WHERE BookID = {0}", bookId)
                .FirstOrDefaultAsync();

            if (book == null || book.Stock < quantity)
            {
                return false;
            }

            return true;
        }

        public async Task UpdateStockAfterPurchaseAsync(int bookId, int quantity)
        {
            var book = await _context.Books.FindAsync(bookId);

            if (book == null)
            {
                throw new Exception("Book not found");
            }

            if (book.Stock < quantity)
            {
                throw new Exception("Not enough stock available");
            }

            book.Stock -= quantity;
            _context.Books.Update(book);
        }

        public async Task<Book> GetByIdForUpdateAsync(int bookId)
        {
            return await _context.Books
                .Where(b => b.BookID == bookId)
                .FirstOrDefaultAsync();
        }
    }
}
