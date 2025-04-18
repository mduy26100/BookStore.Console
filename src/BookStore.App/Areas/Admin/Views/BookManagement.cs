using BookStore.App.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Services;

namespace BookStore.App.Areas.Admin.Views
{
    public class BookManagement
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly int _accountId;

        public BookManagement(IBookService bookService, ICategoryService categoryService, int accountId)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _accountId = accountId;
        }

        public async Task ManageBooks()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Book Management ===");
                Console.WriteLine("1. View All Books (Paged)");
                Console.WriteLine("2. Add New Book");
                Console.WriteLine("3. Edit Book");
                Console.WriteLine("4. Delete Book");
                Console.WriteLine("5. View Book Details");
                Console.WriteLine("6. Search Books by Name");
                Console.WriteLine("7. Sort Books by Price");
                Console.WriteLine("8. Browse Books by Category");
                Console.WriteLine("0. Back to Admin Dashboard");

                string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" });

                if (choice == null)
                    continue;

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ViewAllBooksPaged();
                            break;
                        case "2":
                            await AddBook();
                            break;
                        case "3":
                            await EditBook();
                            break;
                        case "4":
                            await DeleteBook();
                            break;
                        case "5":
                            await ViewBookDetails();
                            break;
                        case "6":
                            await SearchBooksByName();
                            break;
                        case "7":
                            await SortBooksByPrice();
                            break;
                        case "8":
                            await BrowseBooksByCategory();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Feature not implemented yet.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private async Task ViewAllBooksPaged()
        {
            int currentPage = 1;
            int pageSize = 10;
            int totalBooks = await _bookService.GetTotalBooksCount();
            int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Books (Page {currentPage}/{totalPages}) ===");

                var books = await _bookService.GetBooksPaged(currentPage, pageSize);

                if (books == null || !books.Any())
                {
                    Console.WriteLine("No books found.");
                    break;
                }

                Console.WriteLine("ID\tTitle\t\t\tAuthor\t\t\tPrice\tStock");
                Console.WriteLine("------------------------------------------------------------------");

                foreach (var book in books)
                {
                    string title = book.Title.Length > 20 ? book.Title.Substring(0, 17) + "..." : book.Title.PadRight(20);
                    string author = book.Author.Length > 20 ? book.Author.Substring(0, 17) + "..." : book.Author.PadRight(20);

                    Console.WriteLine($"{book.BookID}\t{title}\t{author}\t${book.Price}\t{book.Stock}");
                }

                Console.WriteLine("\nNavigation:");
                Console.WriteLine("P - Previous Page");
                Console.WriteLine("N - Next Page");
                Console.WriteLine("G - Go to Page");
                Console.WriteLine("X - Exit to Menu");

                string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "P", "N", "G", "X", "p", "n", "g", "x" });
                if (choice == null) break;

                switch (choice.ToUpper())
                {
                    case "P":
                        if (currentPage > 1)
                            currentPage--;
                        break;
                    case "N":
                        if (currentPage < totalPages)
                            currentPage++;
                        break;
                    case "G":
                        int? pageNumber = InputValidator.GetValidInteger($"Enter page number (1-{totalPages}): ",
                            page => page >= 1 && page <= totalPages,
                            $"Please enter a valid page number between 1 and {totalPages}.");

                        if (pageNumber != null)
                            currentPage = pageNumber.Value;
                        break;
                    case "X":
                        return;
                }
            }
        }

        private async Task AddBook()
        {
            Console.Clear();
            Console.WriteLine("=== Add New Book ===");

            string title = await GetUniqueBookTitle("Enter book title: ");
            if (title == null) return;

            string author = InputValidator.GetNonEmptyString("Enter author: ");
            if (author == null) return;

            decimal? price = InputValidator.GetValidDecimal("Enter price: $",
                price => price > 0,
                "Price must be greater than 0.");
            if (price == null) return;

            int? stock = InputValidator.GetValidInteger("Enter stock quantity: ",
                stock => stock >= 0,
                "Stock quantity must be 0 or greater.");
            if (stock == null) return;

            string description = InputValidator.GetValidString("Enter description: ",
                input => !string.IsNullOrWhiteSpace(input),
                "Description cannot be empty.");
            if (description == null) return;

            var bookDto = new BookDto
            {
                Title = title,
                Author = author,
                Price = price.Value,
                Stock = stock.Value,
                Description = description
            };

            List<int> selectedCategoryIds = await SelectCategories();
            if (selectedCategoryIds == null) return;

            try
            {
                await _bookService.CreateBook(bookDto, selectedCategoryIds);
                Console.WriteLine("Book added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding book: {ex.Message}");
            }
        }

        private async Task EditBook()
        {
            Console.Clear();
            Console.WriteLine("=== Edit Book ===");

            await ViewAllBooksPaged();

            int? bookId = InputValidator.GetValidInteger("Enter book ID to edit: ",
                id => id > 0,
                "Invalid book ID. Please enter a positive number.");
            if (bookId == null) return;

            try
            {
                var book = await _bookService.GetDetailsBook(bookId.Value);
                if (book == null)
                {
                    Console.WriteLine($"Book with ID {bookId} not found.");
                    return;
                }

                Console.WriteLine($"Editing Book: {book.Title}");
                Console.WriteLine("1. Edit Title");
                Console.WriteLine("2. Edit Author");
                Console.WriteLine("3. Edit Price");
                Console.WriteLine("4. Edit Stock");
                Console.WriteLine("5. Edit Description");
                Console.WriteLine("0. Cancel");

                string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2", "3", "4", "5" });
                if (choice == null) return;

                switch (choice)
                {
                    case "1":
                        await EditBookTitle(book);
                        break;
                    case "2":
                        await EditBookAuthor(book);
                        break;
                    case "3":
                        await EditBookPrice(book);
                        break;
                    case "4":
                        await EditBookStock(book);
                        break;
                    case "5":
                        await EditBookDescription(book);
                        break;
                    case "0":
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task EditBookTitle(BookDto book)
        {
            string newTitle = await GetUniqueBookTitleForEdit("Enter new title: ", book.Title, book.Author);
            if (newTitle == null) return;

            book.Title = newTitle;

            try
            {
                await _bookService.UpdateTitleBook(book);
                Console.WriteLine("Book title updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book title: {ex.Message}");
            }
        }

        private async Task EditBookAuthor(BookDto book)
        {
            string newAuthor = InputValidator.GetNonEmptyString("Enter new author: ");
            if (newAuthor == null) return;

            if (newAuthor != book.Author)
            {
                try
                {
                    bool exists = await _bookService.BookExists(book.Title, newAuthor);
                    if (exists)
                    {
                        Console.WriteLine($"A book with title '{book.Title}' and author '{newAuthor}' already exists.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking book existence: {ex.Message}");
                    return;
                }
            }

            book.Author = newAuthor;

            try
            {
                await _bookService.UpdateAuthorBook(book);
                Console.WriteLine("Book author updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book author: {ex.Message}");
            }
        }

        private async Task EditBookPrice(BookDto book)
        {
            decimal? newPrice = InputValidator.GetValidDecimal("Enter new price: $",
                price => price > 0,
                "Price must be greater than 0.");
            if (newPrice == null) return;

            book.Price = newPrice.Value;

            try
            {
                await _bookService.UpdatePriceBook(book);
                Console.WriteLine("Book price updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book price: {ex.Message}");
            }
        }

        private async Task EditBookStock(BookDto book)
        {
            int? newStock = InputValidator.GetValidInteger("Enter new stock quantity: ",
                stock => stock >= 0,
                "Stock quantity must be 0 or greater.");
            if (newStock == null) return;

            book.Stock = newStock.Value;

            try
            {
                await _bookService.UpdateStockBook(book);
                Console.WriteLine("Book stock updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book stock: {ex.Message}");
            }
        }

        private async Task EditBookDescription(BookDto book)
        {
            string newDescription = InputValidator.GetValidString("Enter new description: ",
                input => !string.IsNullOrWhiteSpace(input),
                "Description cannot be empty.");
            if (newDescription == null) return;

            book.Description = newDescription;

            try
            {
                await _bookService.UpdateDescriptionBook(book);
                Console.WriteLine("Book description updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book description: {ex.Message}");
            }
        }

        private async Task DeleteBook()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Book ===");

            await ViewAllBooksPaged();

            int? bookId = InputValidator.GetValidInteger("Enter book ID to delete: ",
                id => id > 0,
                "Invalid book ID. Please enter a positive number.");
            if (bookId == null) return;

            try
            {
                var book = await _bookService.GetDetailsBook(bookId.Value);
                if (book == null)
                {
                    Console.WriteLine($"Book with ID {bookId} not found.");
                    return;
                }

                Console.WriteLine($"Are you sure you want to delete the book '{book.Title}' by {book.Author}?");
                string confirmation = InputValidator.GetValidMenuChoice("Enter 'Y' to confirm or 'N' to cancel: ", new[] { "Y", "N", "y", "n" });
                if (confirmation == null) return;

                if (confirmation.ToUpper() == "Y")
                {
                    await _bookService.DeleteBook(bookId.Value);
                    Console.WriteLine("Book deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Deletion cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ViewBookDetails()
        {
            Console.Clear();
            Console.WriteLine("=== View Book Details ===");

            await ViewAllBooksPaged();

            int? bookId = InputValidator.GetValidInteger("Enter book ID to view details: ",
                id => id > 0,
                "Invalid book ID. Please enter a positive number.");
            if (bookId == null) return;

            try
            {
                var book = await _bookService.GetDetailsBook(bookId.Value);
                if (book == null)
                {
                    Console.WriteLine($"Book with ID {bookId} not found.");
                    return;
                }

                Console.Clear();
                Console.WriteLine($"=== Book Details: {book.Title} ===");
                Console.WriteLine($"ID: {book.BookID}");
                Console.WriteLine($"Title: {book.Title}");
                Console.WriteLine($"Author: {book.Author}");
                Console.WriteLine($"Price: ${book.Price}");
                Console.WriteLine($"Stock: {book.Stock} copies");
                Console.WriteLine($"Description: {book.Description}");

                Console.WriteLine("\nCategories:");
                if (book.Categories != null && book.Categories.Any())
                {
                    foreach (var category in book.Categories)
                    {
                        Console.WriteLine($"- {category.Name}");
                    }
                }
                else
                {
                    Console.WriteLine("No categories assigned.");
                }

                Console.WriteLine("\nCustomer Reviews:");
                if (book.Reports != null && book.Reports.Any())
                {
                    foreach (var report in book.Reports.Where(r => !string.IsNullOrWhiteSpace(r.CustomerReviews)))
                    {
                        Console.WriteLine($"- {report.CustomerName}: {report.CustomerReviews}");
                    }
                }
                else
                {
                    Console.WriteLine("No reviews yet.");
                }

                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Add to Cart");
                Console.WriteLine("0. Back to Book Management");

                string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1" });
                if (choice == null) return;

                switch (choice)
                {
                    case "1":
                        await AddToCart(book);
                        break;
                    case "0":
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task AddToCart(BookDto book)
        {
            int? quantity = InputValidator.GetValidInteger("Enter quantity to add to cart: ",
                qty => qty > 0,
                "Quantity must be greater than 0.");
            if (quantity == null) return;

            try
            {
                bool inStock = await _bookService.CheckBookInStock(book.BookID, quantity.Value);
                if (!inStock)
                {
                    Console.WriteLine($"Sorry, only {book.Stock} copies are available in stock.");
                    return;
                }

                await _bookService.AddToCart(_accountId, book.BookID, quantity.Value);
                Console.WriteLine($"{quantity} copies of '{book.Title}' added to your cart successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to cart: {ex.Message}");
            }
        }

        private async Task<List<int>> SelectCategories()
        {
            var selectedCategoryIds = new List<int>();
            var categories = await _categoryService.GetAllCategory();

            if (categories == null || !categories.Any())
            {
                Console.WriteLine("No categories found. Please add categories first.");
                return null;
            }

            Console.WriteLine("\nAvailable Categories:");
            foreach (var category in categories)
            {
                Console.WriteLine($"{category.CategoryID}. {category.Name}");
            }

            string input = InputValidator.GetNonEmptyString("\nSelect categories for this book (enter category IDs separated by commas, e.g., '1,3,5'): ");

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("No categories selected.");
                return selectedCategoryIds;
            }

            string[] categoryIdStrings = input.Split(',');
            foreach (var idString in categoryIdStrings)
            {
                if (int.TryParse(idString.Trim(), out int categoryId))
                {
                    if (categories.Any(c => c.CategoryID == categoryId))
                    {
                        selectedCategoryIds.Add(categoryId);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Category ID {categoryId} not found and will be skipped.");
                    }
                }
            }

            if (selectedCategoryIds.Count == 0)
            {
                Console.WriteLine("No valid categories selected.");
            }
            else
            {
                Console.WriteLine($"Selected {selectedCategoryIds.Count} categories.");
            }

            return selectedCategoryIds;
        }

        private async Task<string> GetUniqueBookTitle(string prompt)
        {
            int attempts = 0;
            const int maxRetries = 3;

            while (attempts < maxRetries)
            {
                string title = InputValidator.GetNonEmptyString(prompt);
                if (title == null) return null;

                string author = InputValidator.GetNonEmptyString("Enter author: ");
                if (author == null) return null;

                try
                {
                    bool exists = await _bookService.BookExists(title, author);
                    if (!exists)
                    {
                        return title;
                    }

                    attempts++;
                    Console.WriteLine($"A book with title '{title}' by author '{author}' already exists. Please choose a different title or author.");
                    Console.WriteLine($"Attempts remaining: {maxRetries - attempts}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking book title: {ex.Message}");
                    return null;
                }
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }

        private async Task<string> GetUniqueBookTitleForEdit(string prompt, string currentTitle, string currentAuthor)
        {
            int attempts = 0;
            const int maxRetries = 3;

            while (attempts < maxRetries)
            {
                string title = InputValidator.GetNonEmptyString(prompt);
                if (title == null) return null;

                if (title.Equals(currentTitle, StringComparison.OrdinalIgnoreCase))
                {
                    return title;
                }

                try
                {
                    bool exists = await _bookService.BookExists(title, currentAuthor);
                    if (!exists)
                    {
                        return title;
                    }

                    attempts++;
                    Console.WriteLine($"A book with title '{title}' by author '{currentAuthor}' already exists. Please choose a different title.");
                    Console.WriteLine($"Attempts remaining: {maxRetries - attempts}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking book title: {ex.Message}");
                    return null;
                }
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }

        private async Task SearchBooksByName()
        {
            Console.Clear();
            Console.WriteLine("=== Search Books by Name ===");
            string searchTerm = InputValidator.GetNonEmptyString("Enter search term: ");

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Search term cannot be empty.");
                return;
            }

            var books = await _bookService.SearchBooksByName(searchTerm);

            if (!books.Any())
            {
                Console.WriteLine("No books found matching your search term.");
                return;
            }

            DisplayBooks(books);
        }

        private async Task SortBooksByPrice()
        {
            Console.Clear();
            Console.WriteLine("=== Sort Books by Price ===");
            Console.WriteLine("1. Sort by Price (Low to High)");
            Console.WriteLine("2. Sort by Price (High to Low)");
            Console.WriteLine("0. Back");

            string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2" });

            if (choice == "0")
                return;

            bool ascending = choice == "1";
            var books = await _bookService.GetBooksSortedByPrice(ascending);

            if (!books.Any())
            {
                Console.WriteLine("No books found.");
                return;
            }

            string sortOrder = ascending ? "Low to High" : "High to Low";
            Console.WriteLine($"=== Books Sorted by Price ({sortOrder}) ===");
            DisplayBooks(books);
        }

        private async Task BrowseBooksByCategory()
        {
            Console.Clear();
            Console.WriteLine("=== Browse Books by Category ===");

            var categories = await _categoryService.GetAllCategory();

            if (!categories.Any())
            {
                Console.WriteLine("No categories found.");
                return;
            }

            Console.WriteLine("Available Categories:");
            int index = 1;
            foreach (var category in categories)
            {
                Console.WriteLine($"{index}. {category.Name} - {category.Description}");
                index++;
            }

            Console.WriteLine("0. Back");

            int maxChoice = categories.Count();
            string[] validChoices = Enumerable.Range(0, maxChoice + 1).Select(i => i.ToString()).ToArray();

            string choice = InputValidator.GetValidMenuChoice("Select a category: ", validChoices);

            if (choice == "0")
                return;

            int categoryIndex = int.Parse(choice) - 1;
            var selectedCategory = categories.ElementAt(categoryIndex);

            var books = await _bookService.GetBooksByCategory(selectedCategory.CategoryID);

            if (!books.Any())
            {
                Console.WriteLine($"No books found in category '{selectedCategory.Name}'.");
                return;
            }

            Console.WriteLine($"=== Books in Category: {selectedCategory.Name} ===");
            DisplayBooks(books);
        }

        private void DisplayBooks(IEnumerable<BookDto> books)
        {
            Console.WriteLine("\nID\tTitle\tAuthor\tPrice\tStock");
            Console.WriteLine("--------------------------------------------------");

            foreach (var book in books)
            {
                Console.WriteLine($"{book.BookID}\t{book.Title}\t{book.Author}\t${book.Price}\t{book.Stock}");
            }

            Console.WriteLine($"\nTotal books: {books.Count()}");
        }
    }
}