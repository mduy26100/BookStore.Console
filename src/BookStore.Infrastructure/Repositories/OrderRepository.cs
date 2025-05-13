using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Repositories;
using BookStore.Domain.Consts;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.SeedWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BookStore.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task ApproveOrderByAdmin(int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderID == orderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }
                order.Status = SD.Approved;
                _context.Orders.Update(order);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Order>> GetAllOrder(int accountId)
        {
            try
            {
                var list = await _context.Orders
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Account)
                    .Where(x => x.AccountID == accountId)
                    .ToListAsync();

                return list;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Order> GetOrderDetail(int accountId, Order order)
        {
            try
            {
                var orderDetail = await _context.Orders.FirstOrDefaultAsync(x => x.AccountID == accountId && x.OrderID == order.OrderID);
                if( orderDetail == null)
                {
                    throw new Exception("Order not found");
                }
                return orderDetail;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Order> GetOrderDetailByAdmin(Order order)
        {
            try
            {
                var orderDetail = await _context.Orders.FirstOrDefaultAsync(x => x.OrderID == order.OrderID);
                if (orderDetail == null)
                {
                    throw new Exception("Order not found");
                }
                return orderDetail;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RejectOrder(int accountId, int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.AccountID == accountId && x.OrderID == orderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }
                order.Status = SD.Canceled;
                _context.Orders.Update(order);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RejectOrderByAdmin(int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderID == orderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }
                order.Status = SD.Canceled;
                _context.Orders.Update(order);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SuccessOrder(int accountId, int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.AccountID == accountId && x.OrderID == orderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }
                order.Status = SD.Completed;
                _context.Orders.Update(order);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateAddressOrder(int accountId, int orderId, string address)
        {
            try
            {
                var orderItem = await _context.Orders.FirstOrDefaultAsync(x => x.AccountID == accountId && x.OrderID == orderId);

                if (orderItem == null)
                {
                    throw new Exception("Order not found");
                }

                orderItem.CustomerAddress = address;
                _context.Orders.Update(orderItem);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateAllOrder(int accountId, Order order)
        {
            try
            {
                var existingOrder = await _context.Orders.FirstOrDefaultAsync(x => x.AccountID == accountId && x.OrderID == order.OrderID);

                if (existingOrder == null)
                {
                    throw new Exception("Order not found");
                }

                existingOrder.CustomerName = order.CustomerName;
                existingOrder.CustomerPhone = order.CustomerPhone;
                existingOrder.CustomerEmail = order.CustomerEmail;
                existingOrder.CustomerAddress = order.CustomerAddress;
                existingOrder.Status = order.Status;

                _context.Orders.Update(existingOrder);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateEmailOrder(int accountId, int orderId, string email)
        {
            try
            {
                var orderItem = await _context.Orders.FirstOrDefaultAsync(x => x.AccountID == accountId && x.OrderID == orderId);

                if (orderItem == null)
                {
                    throw new Exception("Order not found");
                }

                orderItem.CustomerEmail = email;
                _context.Orders.Update(orderItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateNameOrder(int accountId, int orderId, string name)
        {
            try
            {
                var orderItem = await _context.Orders.FirstOrDefaultAsync(x => x.AccountID == accountId && x.OrderID == orderId);

                if (orderItem == null)
                {
                    throw new Exception("Order not found");
                }

                orderItem.CustomerName = name;
                _context.Orders.Update(orderItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdatePhoneOrder(int accountId, int orderId, string phone)
        {
            try
            {
                var orderItem = await _context.Orders.FirstOrDefaultAsync(x => x.AccountID == accountId && x.OrderID == orderId);

                if (orderItem == null)
                {
                    throw new Exception("Order not found");
                }

                orderItem.CustomerPhone = phone;
                _context.Orders.Update(orderItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ReportDto>> GetRevenueReportByDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                var report = await _context.Reports
                    .Where(r => r.OrderDate >= startDate && r.OrderDate <= endDate)
                    .GroupBy(r => r.OrderDate.Date)
                    .Select(g => new ReportDto
                    {
                        OrderDate = g.Key,
                        Price = g.Sum(r => r.Price * r.Quantity),
                        Quantity = g.Sum(r => r.Quantity), 
                        CustomerReviews = $"Total Orders: {g.Select(r => r.OrderID).Distinct().Count()}"
                    })
                    .OrderBy(r => r.OrderDate)
                    .ToListAsync();

                return report;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating revenue report: {ex.Message}", ex);
            }
        }
    }
}
