using BookStore.App.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Services;

namespace BookStore.App.Areas.Admin.Views
{
    public class OrderManagement
    {
        private readonly IOrderService _orderService;

        public OrderManagement(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task ManageOrders()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Order Management ===");

                try
                {
                    Console.WriteLine("\n1. View All Orders");
                    Console.WriteLine("2. View Order Details");
                    Console.WriteLine("3. Approve Order");
                    Console.WriteLine("4. Reject Order");
                    Console.WriteLine("5. View Revenue Report");
                    Console.WriteLine("0. Back to Main Menu");

                    string choice = InputValidator.GetValidMenuChoice("Choose an option: ", new[] { "0", "1", "2", "3", "4", "5" });

                    switch (choice)
                    {
                        case "1":
                            await ViewAllOrders();
                            break;
                        case "2":
                            await ViewOrderDetails();
                            break;
                        case "3":
                            await ApproveOrder();
                            break;
                        case "4":
                            await RejectOrder();
                            break;
                        case "5":
                            await ViewRevenueReport();
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

        private async Task ViewAllOrders()
        {
            int currentPage = 1;
            int pageSize = 5;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== All Orders ===");

                try
                {
                    var orders = await _orderService.GetAllOrderByAdmin();

                    if (orders == null || !orders.Any())
                    {
                        Console.WriteLine("No orders found.");
                        return;
                    }

                    int totalOrders = orders.Count;
                    int totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

                    var paginatedOrders = orders
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    Console.WriteLine("ID\tCustomer Name\tOrder Date\tTotal Price\tStatus");
                    Console.WriteLine("------------------------------------------------------------------");

                    foreach (var order in paginatedOrders)
                    {
                        Console.WriteLine($"{order.OrderID}\t{order.CustomerName}\t{order.OrderDate}\t{order.TotalPrice}\t{order.Status}");
                    }

                    Console.WriteLine("------------------------------------------------------------------");
                    Console.WriteLine($"\nPage {currentPage}/{totalPages}");
                    Console.WriteLine("\nP. Previous Page");
                    Console.WriteLine("N. Next Page");
                    Console.WriteLine("G. Go to Page");
                    Console.WriteLine("0. Back to Main Menu");

                    string option = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "P", "N", "G" });

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

        private async Task ViewOrderDetails()
        {
            Console.Clear();
            Console.WriteLine("=== Order Details ===");

            int? orderId = InputValidator.GetValidInteger("Enter Order ID: ", id => id > 0, "Invalid Order ID.");
            if (orderId == null) return;

            try
            {
                var orderDto = new OrderDto { OrderID = orderId.Value };
                var orderDetails = await _orderService.GetOrderDetailByAdmin(orderDto);

                if (orderDetails == null)
                {
                    Console.WriteLine("Order not found.");
                    return;
                }

                DisplayOrderDetails(orderDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ApproveOrder()
        {
            Console.Clear();
            Console.WriteLine("=== Approve Order ===");

            int? orderId = InputValidator.GetValidInteger("Enter Order ID: ", id => id > 0, "Invalid Order ID.");
            if (orderId == null) return;

            try
            {
                await _orderService.ApproveOrderByAdmin(orderId.Value);
                Console.WriteLine("Order approved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task RejectOrder()
        {
            Console.Clear();
            Console.WriteLine("=== Reject Order ===");

            int? orderId = InputValidator.GetValidInteger("Enter Order ID: ", id => id > 0, "Invalid Order ID.");
            if (orderId == null) return;

            try
            {
                await _orderService.RejectOrderByAdmin(orderId.Value);
                Console.WriteLine("Order rejected successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ViewRevenueReport()
        {
            Console.Clear();
            Console.WriteLine("=== Revenue Report ===");

            try
            {
                DateTime? startDate = InputValidator.GetValidDate("Enter start date (yyyy-MM-dd): ", "Invalid date format.");
                if (startDate == null) return;

                DateTime? endDate = InputValidator.GetValidDate("Enter end date (yyyy-MM-dd): ", "Invalid date format.");
                if (endDate == null) return;

                var report = await _orderService.GetRevenueReport(startDate.Value, endDate.Value);

                if (report == null || !report.Any())
                {
                    Console.WriteLine("No revenue data found for the specified date range.");
                    return;
                }

                Console.WriteLine("Date\t\tTotal Revenue\tTotal Quantity\tTotal Orders");
                Console.WriteLine("------------------------------------------------------------------");

                foreach (var item in report)
                {
                    Console.WriteLine($"{item.OrderDate:yyyy-MM-dd}\t{item.Price:C}\t{item.Quantity}\t{item.CustomerReviews}");
                }

                Console.WriteLine("------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DisplayOrderDetails(OrderDto orderDto)
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
