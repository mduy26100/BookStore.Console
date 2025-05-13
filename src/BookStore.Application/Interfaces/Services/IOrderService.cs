using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrderByAdmin();
        Task<List<OrderDto>> GetAllOrder(int accountId);
        Task<OrderDto> GetDetailOrder(int accountId, OrderDto orderDto);
        Task<OrderDto> GetOrderDetailByAdmin(OrderDto orderDto);
        Task UpdateNameOrder(int accountId, int orderId, string name);
        Task UpdatePhoneOrder(int accountId, int orderId, string phone);
        Task UpdateAddressOrder(int accountId, int orderId, string address);
        Task UpdateEmailOrder(int accountId, int orderId, string email);
        Task UpdateAllOrder(int accountId, OrderDto orderDto);
        Task ApproveOrderByAdmin(int orderId);
        Task RejectOrderByAdmin(int orderId);
        Task RejectOrder(int accountId, int orderId);
        Task<bool> SuccessOrder(int accountId, OrderDto orderDto);
        Task<List<ReportDto>> GetRevenueReport(DateTime startDate, DateTime endDate);
    }
}
