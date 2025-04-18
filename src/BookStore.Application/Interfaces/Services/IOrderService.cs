using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto> GetAllOrder(int accountId);
        Task<OrderDto> GetDetailOrder(int accountId, OrderDto orderDto);
        Task ApproveOrder(int accountId, int orderId);
        Task RejectOrder(int accountId, int orderId);
        Task SuccessOrder(int accountId, int orderId);
    }
}
