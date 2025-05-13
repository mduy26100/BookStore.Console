using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetAllOrder(int accountId);
        Task<Order> GetOrderDetail(int accountId, Order order);
        Task<Order> GetOrderDetailByAdmin(Order order);
        Task UpdateNameOrder(int accountId, int orderId, string name);
        Task UpdatePhoneOrder(int accountId, int orderId, string phone);
        Task UpdateAddressOrder(int accountId, int orderId, string address);
        Task UpdateEmailOrder(int accountId, int orderId, string email);
        Task UpdateAllOrder(int accountId, Order order);
        Task ApproveOrderByAdmin(int orderId);
        Task RejectOrderByAdmin(int orderId);
        Task RejectOrder(int accountId, int orderId);
        Task SuccessOrder(int accountId, int orderId);
        Task<List<ReportDto>> GetRevenueReportByDate(DateTime startDate, DateTime endDate);
    }
}
