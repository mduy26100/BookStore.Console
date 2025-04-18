using BookStore.Application.Interfaces.Repositories;
using BookStore.Domain.Consts;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task ApproveOrder(int accountId, int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.AccountID == accountId && x.OrderID == orderId);
                if(order == null)
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

        public async Task<Order> GetAllOrder(int accountId)
        {
            try
            {
                var listOrder = await _context.Orders.FindAsync(accountId);
                if(listOrder == null)
                {
                    throw new Exception("Order not found");
                }
                return listOrder;
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
    }
}
