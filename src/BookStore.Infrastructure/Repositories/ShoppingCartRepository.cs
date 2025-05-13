using BookStore.Application.Interfaces.Repositories;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddCart(int accountId, int bookId, int quantity)
        {
            var existingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(x => x.AccountID == accountId && x.BookID == bookId);

            if (existingCart != null)
            {
                existingCart.Quantity += quantity;
                _context.ShoppingCarts.Update(existingCart);
            }
            else
            {
                var cartItem = new ShoppingCart
                {
                    AccountID = accountId,
                    BookID = bookId,
                    Quantity = quantity,
                    CreatedAt = DateTime.Now
                };
                await _context.ShoppingCarts.AddAsync(cartItem);
            }
        }

        public async Task<List<ShoppingCart>> GetAllCart(int accountId)
        {
            return await _context.ShoppingCarts
                .Include(x => x.Book)
                .Where(x => x.AccountID == accountId)
                .ToListAsync();
        }

        public async Task<ShoppingCart> GetDetailCart(int accountId, ShoppingCart cart)
        {
            try
            {
                var item = await _context.ShoppingCarts
                    .Include(x => x.Book)
                    .FirstOrDefaultAsync(x => x.CartID == cart.CartID && x.AccountID == accountId);

                if (item == null)
                {
                    throw new Exception("Cart not found");
                }

                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveCart(int accountId, ShoppingCart cart)
        {
            try
            {
                var cartItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(x => x.AccountID == accountId && x.CartID == cart.CartID);

                if (cartItem == null)
                {
                    throw new Exception("Cart not found");
                }

                _context.ShoppingCarts.Remove(cartItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateQuantity(int accountId, int bookId, int newQuantity)
        {
            try
            {
                var cartItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(x => x.AccountID == accountId && x.BookID == bookId);

                if (cartItem == null)
                {
                    throw new Exception("Cart item not found or access denied");
                }

                cartItem.Quantity = newQuantity;
                _context.ShoppingCarts.Update(cartItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ClearCart(int accountId)
        {
            var cartItems = await _context.ShoppingCarts
                .Where(x => x.AccountID == accountId)
                .ToListAsync();

            _context.ShoppingCarts.RemoveRange(cartItems);
        }
    }
}