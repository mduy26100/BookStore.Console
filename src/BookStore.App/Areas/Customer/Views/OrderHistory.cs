using BookStore.App.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Services;

namespace BookStore.App.Areas.Customer.Views
{
    public class OrderHistory
    {
        private readonly IOrderService _orderService;
        private readonly int _accountId;

        public OrderHistory(IOrderService orderService, int accountId)
        {
            _orderService = orderService;
            _accountId = accountId;
        }

        private async Task<bool> ValidateOrderOwnership(int orderId)
        {
            try
            {
                var orderDto = new OrderDto { OrderID = orderId };
                var orderDetails = await _orderService.GetDetailOrder(_accountId, orderDto);

                if (orderDetails == null || orderDetails.AccountID != _accountId)
                {
                    Console.WriteLine("You do not have permission to access this order.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


        public async Task ManageOrder()
        {
            int currentPage = 1;
            int pageSize = 5;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== My Orders ===");

                try
                {
                    var orders = await _orderService.GetAllOrder(_accountId);

                    if (orders == null || !orders.Any())
                    {
                        Console.WriteLine("Your order is empty.");
                        Console.WriteLine("\n1. Continue Shopping");
                        Console.WriteLine("0. Back to Main Menu");

                        string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1" });

                        if (choice == "0")
                            return;
                        else
                            continue;
                    }

                    int totalOrders = orders.Count;
                    int totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

                    var paginatedOrders = orders
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    DisplayAllOrder(paginatedOrders);

                    Console.WriteLine($"\nPage {currentPage}/{totalPages}");
                    Console.WriteLine("\nP. Previous Page");
                    Console.WriteLine("N. Next Page");
                    Console.WriteLine("G. Go to Page");
                    Console.WriteLine("1. Detail Order");
                    Console.WriteLine("2. Reject Pending Order");
                    Console.WriteLine("3. Mark Approved Order as Success");
                    Console.WriteLine("4. Update User Information in Order");
                    Console.WriteLine("0. Back to Main Menu");

                    string option = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2", "3", "4", "P", "N", "G" });

                    switch (option.ToUpper())
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
                        case "1":
                            await ShowOrderDetails();
                            break;
                        case "2":
                            await RejectPendingOrder();
                            break;
                        case "3":
                            await MarkOrderAsSuccess();
                            break;
                        case "4":
                            await UpdateUserInformationInOrder();
                            break;
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

        private void DisplayAllOrder(List<OrderDto> orderDto)
        {
            Console.WriteLine("\n=== Your Order Items ===");
            Console.WriteLine("ID\tName\tOrder Date\tPrice\tStatus");
            Console.WriteLine("------------------------------------------------------------------");

            foreach (var item in orderDto)
            {
                Console.WriteLine($"{item.OrderID}\t{item.CustomerName}\t{item.OrderDate}\t{item.TotalPrice}\t{item.Status}");
            }

            Console.WriteLine("------------------------------------------------------------------");
        }

        private async Task ShowOrderDetails()
        {
            Console.Clear();
            Console.WriteLine("=== Order Details ===");

            int? orderId = InputValidator.GetValidInteger("Enter Order ID: ", id => id > 0, "Invalid Order ID.");
            if (orderId == null) return;

            if (!await ValidateOrderOwnership(orderId.Value)) return;

            try
            {
                var orderDto = new OrderDto { OrderID = orderId.Value };
                var orderDetails = await _orderService.GetDetailOrder(_accountId, orderDto);

                if (orderDetails == null)
                {
                    Console.WriteLine("Order not found.");
                    return;
                }

                DisplayOrderItem(orderDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task RejectPendingOrder()
        {
            Console.Clear();
            Console.WriteLine("=== Reject Pending Order ===");

            int? orderId = InputValidator.GetValidInteger("Enter Order ID: ", id => id > 0, "Invalid Order ID.");
            if (orderId == null) return;

            if (!await ValidateOrderOwnership(orderId.Value)) return;

            try
            {
                await _orderService.RejectOrder(_accountId, orderId.Value);
                Console.WriteLine("Order status updated to Rejected successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task MarkOrderAsSuccess()
        {
            Console.Clear();
            Console.WriteLine("=== Mark Approved Order as Success ===");

            int? orderId = InputValidator.GetValidInteger("Enter Order ID: ", id => id > 0, "Invalid Order ID.");
            if (orderId == null) return;

            if (!await ValidateOrderOwnership(orderId.Value)) return;

            Console.WriteLine("Enter your review (optional):");
            string review = Console.ReadLine() ?? string.Empty;

            try
            {
                var orderDto = new OrderDto
                {
                    OrderID = orderId.Value,
                    CustomerReviews = review
                };

                bool success = await _orderService.SuccessOrder(_accountId, orderDto);
                if (success)
                {
                    Console.WriteLine("Order status updated to Success successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task UpdateUserInformationInOrder()
        {
            Console.Clear();
            Console.WriteLine("=== Update User Information in Order ===");

            int? orderId = InputValidator.GetValidInteger("Enter Order ID: ", id => id > 0, "Invalid Order ID.");
            if (orderId == null) return;

            if (!await ValidateOrderOwnership(orderId.Value)) return;

            var orderDto = new OrderDto { OrderID = orderId.Value };
            var orderDetails = await _orderService.GetDetailOrder(_accountId, orderDto);

            if (orderDetails == null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            DisplayOrderItem(orderDetails);

            Console.WriteLine("\nChoose an option to update:");
            Console.WriteLine("1. Update Name");
            Console.WriteLine("2. Update Phone");
            Console.WriteLine("3. Update Email");
            Console.WriteLine("4. Update Address");
            Console.WriteLine("5. Update All Information");
            Console.WriteLine("0. Back to Main Menu");

            string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2", "3", "4", "5" });

            try
            {
                switch (choice)
                {
                    case "1":
                        await UpdateName(orderId.Value);
                        break;
                    case "2":
                        await UpdatePhone(orderId.Value);
                        break;
                    case "3":
                        await UpdateEmail(orderId.Value);
                        break;
                    case "4":
                        await UpdateAddress(orderId.Value);
                        break;
                    case "5":
                        await UpdateAllInformation(orderId.Value);
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

        private async Task UpdateName(int orderId)
        {
            Console.WriteLine("Enter new name:");
            string name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return;
            }

            await _orderService.UpdateNameOrder(_accountId, orderId, name);
            Console.WriteLine("Name updated successfully.");
        }

        private async Task UpdatePhone(int orderId)
        {
            Console.WriteLine("Enter new phone:");
            string phone = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(phone))
            {
                Console.WriteLine("Phone cannot be empty.");
                return;
            }

            await _orderService.UpdatePhoneOrder(_accountId, orderId, phone);
            Console.WriteLine("Phone updated successfully.");
        }

        private async Task UpdateEmail(int orderId)
        {
            Console.WriteLine("Enter new email:");
            string email = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email cannot be empty.");
                return;
            }

            await _orderService.UpdateEmailOrder(_accountId, orderId, email);
            Console.WriteLine("Email updated successfully.");
        }

        private async Task UpdateAddress(int orderId)
        {
            Console.WriteLine("Enter new address:");
            string address = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(address))
            {
                Console.WriteLine("Address cannot be empty.");
                return;
            }

            await _orderService.UpdateAddressOrder(_accountId, orderId, address);
            Console.WriteLine("Address updated successfully.");
        }

        private async Task UpdateAllInformation(int orderId)
        {
            Console.WriteLine("Enter new name:");
            string name = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter new phone:");
            string phone = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter new email:");
            string email = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter new address:");
            string address = Console.ReadLine() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(name))
            {
                await _orderService.UpdateNameOrder(_accountId, orderId, name);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                await _orderService.UpdatePhoneOrder(_accountId, orderId, phone);
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                await _orderService.UpdateEmailOrder(_accountId, orderId, email);
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                await _orderService.UpdateAddressOrder(_accountId, orderId, address);
            }

            Console.WriteLine("All information updated successfully.");
        }

        private void DisplayOrderItem(OrderDto orderDto)
        {
            Console.WriteLine($"Order ID: {orderDto.OrderID}");
            Console.WriteLine($"Customer Name: {orderDto.CustomerName}");
            Console.WriteLine($"Customer Email: {orderDto.CustomerEmail}");
            Console.WriteLine($"Customer Phone: {orderDto.CustomerPhone}");
            Console.WriteLine($"Customer Address: {orderDto.CustomerAddress}");
            Console.WriteLine($"Order Date: {orderDto.OrderDate}");
            Console.WriteLine($"Total Price: {orderDto.TotalPrice}");
            Console.WriteLine($"Status: {orderDto.Status}");
            Console.WriteLine("\nOrder Details:");

            foreach (var detail in orderDto.OrderDetails)
            {
                Console.WriteLine($"- Book ID: {detail.BookID}, Quantity: {detail.Quantity}, Price: {detail.Price}");
            }
        }
    }
}
