using BookStore.App.Areas.Admin;
using BookStore.App.Areas.Customer;
using BookStore.App.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Services;
using BookStore.Domain.Consts;

namespace BookStore.App.Areas
{
    public class App
    {
        private readonly IAccountService _accountService;
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        private readonly IShoppingCartService _shoppingCartService;

        public App(IAccountService accountService
            , ICategoryService categoryService
            , IBookService bookService
            , IShoppingCartService shoppingCartService)
        {
            _accountService = accountService;
            _categoryService = categoryService;
            _bookService = bookService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== BookStore App ====");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("0. Exit");

                string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2" });

                if (choice == null)
                    continue;

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await Login();
                            break;
                        case "2":
                            await Register();
                            break;
                        case "0":
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private async Task Login()
        {
            Console.Clear();
            Console.WriteLine("=== Login ===");

            string username = InputValidator.GetNonEmptyString("Username: ");
            if (username == null) return;

            string password = InputValidator.GetNonEmptyString("Password: ");
            if (password == null) return;

            var loginDto = new AccountDto
            {
                Username = username,
                Password = password
            };

            try
            {
                var success = await _accountService.Login(loginDto);
                if (!success)
                {
                    Console.WriteLine("Login failed. Invalid username or password.");
                    return;
                }

                var user = await _accountService.GetDetailAccount(loginDto);
                if (user == null)
                {
                    Console.WriteLine("Error retrieving user details.");
                    return;
                }

                Console.WriteLine($"Login successful. Welcome {user.Name}!");

                if (user.Role == SD.Role_Admin)
                {
                    var adminDashboard = new AdminArea(_accountService, _categoryService, _bookService, user);
                    await adminDashboard.AdminDashboard(user);
                }
                else
                {
                    var customerDashboard = new CustomerArea(_accountService, _categoryService, _bookService, _shoppingCartService, user);
                    await customerDashboard.CustomerDashboard(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }
        }

        private async Task Register()
        {
            Console.Clear();
            Console.WriteLine("=== Register ===");

            // Check username uniqueness
            string username = await InputValidator.GetUniqueUsername("Username: ",
                async (usernameToCheck) => {
                    try
                    {
                        var existingAccounts = await _accountService.CheckUsernameExists(usernameToCheck);
                        return !existingAccounts; // Return true if username is unique (doesn't exist)
                    }
                    catch
                    {
                        return false; // On error, assume username is not unique to be safe
                    }
                });

            if (username == null) return;

            // Get password with confirmation
            string password = InputValidator.GetPasswordWithConfirmation(
                "Password: ",
                "Confirm password: ",
                pwd => !string.IsNullOrWhiteSpace(pwd) && pwd.Length >= 6,
                "Password must be at least 6 characters long."
            );

            if (password == null) return;

            // Get name with validation
            string name = InputValidator.GetNonEmptyString("Name: ");
            if (name == null) return;

            // Get email with validation
            string email = InputValidator.GetValidEmail("Email: ");
            if (email == null) return;

            // Get phone with validation
            string phone = InputValidator.GetValidPhoneNumber("Phone: ");
            if (phone == null) return;

            // Get address (optional)
            string address = InputValidator.GetValidString("Address: ",
                input => true, // Address can be any string, including empty
                "Invalid address format.");
            if (address == null) return;

            var registerDto = new AccountCreateDto
            {
                Username = username,
                Password = password,
                Name = name,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                Role = SD.Role_Customer
            };

            try
            {
                await _accountService.RegisterAccount(registerDto);
                Console.WriteLine("Registration successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
            }
        }
    }
}
