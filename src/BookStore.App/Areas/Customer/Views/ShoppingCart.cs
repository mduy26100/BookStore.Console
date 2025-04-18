using BookStore.App.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Services;
using BookStore.Domain.Consts;

namespace BookStore.App.Areas.Customer.Views
{
    public class ShoppingCart
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IBookService _bookService;
        private readonly int _accountId;

        public ShoppingCart(IShoppingCartService shoppingCartService, IBookService bookService, int accountId)
        {
            _shoppingCartService = shoppingCartService;
            _bookService = bookService;
            _accountId = accountId;
        }

        public async Task ManageCart()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Shopping Cart ===");

                try
                {
                    var cart = await _shoppingCartService.GetAllShoppingCart(_accountId);

                    if (cart == null || cart.Items == null || !cart.Items.Any())
                    {
                        Console.WriteLine("Your shopping cart is empty.");
                        Console.WriteLine("\n1. Continue Shopping");
                        Console.WriteLine("0. Back to Main Menu");

                        string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1" });

                        if (choice == "0")
                            return;
                        else
                            continue;
                    }

                    // Display cart items
                    DisplayCartItems(cart);

                    Console.WriteLine("\n1. Update Item Quantity");
                    Console.WriteLine("2. Remove Item");
                    Console.WriteLine("3. Checkout");
                    Console.WriteLine("4. Continue Shopping");
                    Console.WriteLine("0. Back to Main Menu");

                    string option = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2", "3", "4" });

                    switch (option)
                    {
                        case "1":
                            await UpdateItemQuantity(cart);
                            break;
                        case "2":
                            await RemoveItem(cart);
                            break;
                        case "3":
                            await Checkout(cart);
                            break;
                        case "4":
                            continue;
                        case "0":
                            return;
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

        private void DisplayCartItems(ShoppingCartDto cart)
        {
            Console.WriteLine("\n=== Your Cart Items ===");
            Console.WriteLine("ID\tBook Title\tAuthor\tPrice\tQuantity\tSubtotal");
            Console.WriteLine("------------------------------------------------------------------");

            foreach (var item in cart.Items)
            {
                decimal subtotal = item.Book.Price * item.Quantity;
                Console.WriteLine($"{item.CartID}\t{item.Book.Title}\t{item.Book.Author}\t${item.Book.Price}\t{item.Quantity}\t${subtotal}");
            }

            decimal total = cart.Items.Sum(item => item.Book.Price * item.Quantity);
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine($"Total: ${total}");
        }

        private async Task UpdateItemQuantity(ShoppingCartDto cart)
        {
            Console.WriteLine("\n=== Update Item Quantity ===");

            // Display items with index for selection
            for (int i = 0; i < cart.Items.Count; i++)
            {
                var item = cart.Items[i];
                Console.WriteLine($"{i + 1}. {item.Book.Title} - Current Quantity: {item.Quantity}");
            }

            Console.WriteLine("0. Cancel");

            string[] validOptions = Enumerable.Range(0, cart.Items.Count + 1).Select(i => i.ToString()).ToArray();
            string choice = InputValidator.GetValidMenuChoice("Select item to update: ", validOptions);

            if (choice == "0")
                return;

            int index = int.Parse(choice) - 1;
            var selectedItem = cart.Items[index];

            int? newQuantity = InputValidator.GetValidInteger(
                $"Enter new quantity for {selectedItem.Book.Title} (1-99): ",
                qty => qty > 0 && qty <= 99,
                "Quantity must be between 1 and 99."
            );

            if (!newQuantity.HasValue)
                return;

            // Check if the book has enough stock
            bool inStock = await _bookService.CheckBookInStock(selectedItem.BookID, newQuantity.Value);

            if (!inStock)
            {
                Console.WriteLine("Sorry, there is not enough stock available for this quantity.");
                return;
            }

            await _shoppingCartService.UpdateQuantityShoppingCart(_accountId, selectedItem.BookID, newQuantity.Value);
            Console.WriteLine("Quantity updated successfully!");
        }

        private async Task RemoveItem(ShoppingCartDto cart)
        {
            Console.WriteLine("\n=== Remove Item from Cart ===");

            // Display items with index for selection
            for (int i = 0; i < cart.Items.Count; i++)
            {
                var item = cart.Items[i];
                Console.WriteLine($"{i + 1}. {item.Book.Title}");
            }

            Console.WriteLine("0. Cancel");

            string[] validOptions = Enumerable.Range(0, cart.Items.Count + 1).Select(i => i.ToString()).ToArray();
            string choice = InputValidator.GetValidMenuChoice("Select item to remove: ", validOptions);

            if (choice == "0")
                return;

            int index = int.Parse(choice) - 1;
            var selectedItem = cart.Items[index];

            await _shoppingCartService.RemoveShoppingCart(_accountId, new ShoppingCartDto { CartID = selectedItem.CartID });
            Console.WriteLine("Item removed successfully!");
        }

        private async Task Checkout(ShoppingCartDto cart)
        {
            Console.Clear();
            Console.WriteLine("=== Checkout ===");

            // Verify stock for all items before proceeding
            List<CartItemDto> outOfStockItems = new List<CartItemDto>();

            foreach (var item in cart.Items)
            {
                bool inStock = await _bookService.CheckBookInStock(item.BookID, item.Quantity);
                if (!inStock)
                {
                    outOfStockItems.Add(item);
                }
            }

            if (outOfStockItems.Any())
            {
                Console.WriteLine("The following items are out of stock or don't have enough quantity:");
                foreach (var item in outOfStockItems)
                {
                    Console.WriteLine($"- {item.Book.Title} (Requested: {item.Quantity})");
                    await _shoppingCartService.RemoveShoppingCart(_accountId, new ShoppingCartDto { CartID = item.CartID });
                }

                Console.WriteLine("These items have been removed from your cart.");
                Console.WriteLine("Please review your updated cart before checking out.");
                return;
            }

            // Choose shipping information
            Console.WriteLine("1. Use my account information");
            Console.WriteLine("2. Enter new shipping information");
            Console.WriteLine("0. Cancel checkout");

            string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2" });

            if (choice == "0")
                return;

            OrderDto order = new OrderDto
            {
                AccountID = _accountId,
                OrderDate = DateTime.Now,
                Status = SD.Pending,
                UseAccountInfo = choice == "1"
            };

            if (choice == "2")
            {
                // Enter new shipping information
                Console.WriteLine("\n=== Enter Shipping Information ===");

                string name = InputValidator.GetNonEmptyString("Enter your name: ");
                if (name == null) return;

                string email = InputValidator.GetValidEmail("Enter your email: ");
                if (email == null) return;

                string phone = InputValidator.GetValidPhoneNumber("Enter your phone number: ");
                if (phone == null) return;

                string address = InputValidator.GetNonEmptyString("Enter your address: ");
                if (address == null) return;

                order.CustomerName = name;
                order.CustomerEmail = email;
                order.CustomerPhone = phone;
                order.CustomerAddress = address;
            }

            // Confirm order
            Console.WriteLine("\n=== Order Summary ===");
            DisplayCartItems(cart);
            Console.WriteLine("\nAre you sure you want to place this order?");
            Console.WriteLine("1. Yes, place order");
            Console.WriteLine("0. No, cancel");

            string confirmChoice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1" });

            if (confirmChoice == "0")
                return;

            try
            {
                bool success = await _shoppingCartService.CheckOutShoppingCart(_accountId, order);

                if (success)
                {
                    Console.WriteLine("\nOrder placed successfully!");
                    Console.WriteLine("Thank you for your purchase.");
                }
                else
                {
                    Console.WriteLine("\nFailed to place order. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError during checkout: {ex.Message}");
            }
        }
    }
}