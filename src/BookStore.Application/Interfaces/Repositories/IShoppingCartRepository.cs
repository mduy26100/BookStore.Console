using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Domain.Entities;
using System.Numerics;

namespace BookStore.Application.Interfaces.Repositories
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<List<ShoppingCart>> GetAllCart(int accountId);
        Task<ShoppingCart> GetDetailCart(int accountId, ShoppingCart cart);
        Task AddCart(int accountId, int bookId, int quantity);
        Task UpdateQuantity(int accountId, int bookId, int newQuantity);
        Task RemoveCart(int accountId, ShoppingCart cart);
        Task ClearCart(int accountId);
    }
}
