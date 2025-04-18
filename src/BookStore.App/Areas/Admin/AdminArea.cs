using BookStore.App.Areas.Admin.Views;
using BookStore.App.Areas.Identity;
using BookStore.App.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Services;

namespace BookStore.App.Areas.Admin
{
    public class AdminArea
    {
        private readonly IAccountService _accountService;
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        public int _accountId;

        public AdminArea(IAccountService accountService
            , ICategoryService categoryService
            , IBookService bookService
            , AccountDto accountDto)
        {
            _accountService = accountService;
            _categoryService = categoryService;
            _bookService = bookService;
            _accountId = accountDto.AccountID;
        }

        public async Task AdminDashboard(AccountDto admin)
        {
            var identity = new IdentityArea(_accountService);
            var categoryManagement = new CategoryManagement(_categoryService);
            var bookManagement = new BookManagement(_bookService, _categoryService, _accountId);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Admin Dashboard ===");
                Console.WriteLine($"Admin: {admin.Name}");
                Console.WriteLine();
                Console.WriteLine("1. View Profile");
                Console.WriteLine("2. View All Accounts");
                Console.WriteLine("3. Manage Categories");
                Console.WriteLine("4. Manage Books");
                Console.WriteLine("5. Manage Orders (Not implemented)");
                Console.WriteLine("6. Manage Reports (Not implemented)");
                Console.WriteLine("0. Logout");

                string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2", "3", "4", "5", "6" });

                if (choice == null)
                    continue;

                try
                {
                    switch (choice)
                    {
                        case "1":
                            identity.ViewProfile(admin);
                            break;
                        case "2":
                            await identity.ViewAllAccounts();
                            break;
                        case "3":
                            await categoryManagement.ManageCategories();
                            break;
                        case "4":
                            await bookManagement.ManageBooks();
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
    }
}