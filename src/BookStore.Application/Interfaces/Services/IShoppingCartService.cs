using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces.Services
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto> GetAllShoppingCart(int accountId);
        Task<ShoppingCartDto> GetDetailShoppingCart(int accountId, ShoppingCartDto shoppingCartDto);
        Task AddShoppingCart(int accountId, int bookId, int quantity);
        Task UpdateQuantityShoppingCart(int accountId, int bookId, int newQuantity);
        Task RemoveShoppingCart(int accountId, ShoppingCartDto cartDto);
        Task<bool> CheckOutShoppingCart(int accountId, OrderDto orderDto);
    }
}
