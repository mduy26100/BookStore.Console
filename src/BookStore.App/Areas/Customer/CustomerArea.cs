using BookStore.App.Areas.Customer.Views;
using BookStore.App.Areas.Identity;
using BookStore.App.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Services;
using BookStore.Application.Services;
using Microsoft.Identity.Client;


namespace BookStore.App.Areas.Customer
{
    public class CustomerArea
    {
        private readonly IAccountService _accountService;
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        private readonly IShoppingCartService _shoppingCartService;
        public int _accountId;

        public CustomerArea(IAccountService accountService
            , ICategoryService categoryService
            , IBookService bookService
            , IShoppingCartService shoppingCartService
            , AccountDto accountDto)
        {
            _accountService = accountService;
            _categoryService = categoryService;
            _bookService = bookService;
            _shoppingCartService = shoppingCartService;
            _accountId = accountDto.AccountID;
        }

        public async Task CustomerDashboard(AccountDto customer)
        {
            var shoppingCart = new ShoppingCart(_shoppingCartService, _bookService, _accountId);
            var identity = new IdentityArea(_accountService);
            int currentPage = 1;
            int pageSize = 10;

            while (true)
            {
                Console.Clear();
                int totalBooks = await _bookService.GetTotalBooksCount();
                int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

                Console.WriteLine($"=== Customer Dashboard ===");
                Console.WriteLine($"Customer: {customer.Name}");
                Console.WriteLine($"=== Books (Page {currentPage}/{totalPages}) ===");

                var books = await _bookService.GetBooksPaged(currentPage, pageSize);

                if (books == null || !books.Any())
                {
                    Console.WriteLine("No books found.");
                }
                else
                {
                    Console.WriteLine("ID\tTitle\t\t\tAuthor\t\t\tPrice\tStock");
                    Console.WriteLine("------------------------------------------------------------------");

                    foreach (var book in books)
                    {
                        string title = book.Title.Length > 20 ? book.Title.Substring(0, 17) + "..." : book.Title.PadRight(20);
                        string author = book.Author.Length > 20 ? book.Author.Substring(0, 17) + "..." : book.Author.PadRight(20);

                        Console.WriteLine($"{book.BookID}\t{title}\t{author}\t${book.Price}\t{book.Stock}");
                    }
                }

                Console.WriteLine("\n=== Options ===");
                Console.WriteLine("P. Previous Page");
                Console.WriteLine("N. Next Page");
                Console.WriteLine("G. Go to Page");
                Console.WriteLine("1. Search Books by Name");
                Console.WriteLine("2. Sort Books by Price");
                Console.WriteLine("3. Browse Books by Category");
                Console.WriteLine("4. View Profile");
                Console.WriteLine("5. View Book Details");
                Console.WriteLine("6. View Shopping Carts");
                Console.WriteLine("7. View Orders");
                Console.WriteLine("0. Logout");

                string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2", "3", "4", "5", "6", "7", "P", "N", "G", "p", "n", "g" });

                if (choice == null)
                    continue;

                try
                {
                    switch (choice.ToUpper())
                    {
                        case "1":
                            await SearchBooksByName();
                            break;
                        case "2":
                            await SortBooksByPrice();
                            break;
                        case "3":
                            await BrowseBooksByCategory();
                            break;
                        case "4":
                            identity.ViewProfile(customer);
                            break;
                        case "5":
                            await ViewBookDetails();
                            break;
                        case "6":
                            await shoppingCart.ManageCart();
                            break;
                        case "7":
                            break;
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
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
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

        private async Task ViewBookDetails()
        {
            Console.WriteLine("=== View Book Details ===");

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