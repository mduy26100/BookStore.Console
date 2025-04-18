using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetAllOrder(int accountId);
        Task<Order> GetOrderDetail(int accountId, Order order);
        Task ApproveOrder(int accountId, int orderId);
        Task RejectOrder(int accountId, int orderId);
        Task SuccessOrder(int accountId, int orderId);
    }
}
