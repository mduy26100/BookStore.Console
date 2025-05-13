using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Application.Interfaces.Services;
using BookStore.Domain.Consts;
using BookStore.Domain.Entities;

namespace BookStore.Application.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShoppingCartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddShoppingCart(int accountId, int bookId, int quantity)
        {
            try
            {
                var book = await _unitOfWork.BookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    throw new Exception("Book not found");
                }

                if (book.Stock < quantity)
                {
                    throw new Exception("Not enough stock available");
                }

                await _unitOfWork.ShoppingCartRepository.AddCart(accountId, bookId, quantity);
                await _unitOfWork.SaveChange();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ShoppingCartDto> GetAllShoppingCart(int accountId)
        {
            try
            {
                var cartItems = await _unitOfWork.ShoppingCartRepository.GetAllCart(accountId);

                if (cartItems == null || !cartItems.Any())
                {
                    return new ShoppingCartDto { AccountID = accountId };
                }

                var result = new ShoppingCartDto
                {
                    AccountID = accountId,
                    Items = _mapper.Map<List<CartItemDto>>(cartItems)
                };

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ShoppingCartDto> GetDetailShoppingCart(int accountId, ShoppingCartDto shoppingCartDto)
        {
            try
            {
                var cart = await _unitOfWork.ShoppingCartRepository.GetDetailCart(accountId, _mapper.Map<ShoppingCart>(shoppingCartDto));
                return _mapper.Map<ShoppingCartDto>(cart);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveShoppingCart(int accountId, ShoppingCartDto cartDto)
        {
            try
            {
                await _unitOfWork.ShoppingCartRepository.RemoveCart(accountId, _mapper.Map<ShoppingCart>(cartDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateQuantityShoppingCart(int accountId, int bookId, int newQuantity)
        {
            try
            {
                var book = await _unitOfWork.BookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    throw new Exception("Book not found");
                }

                if (book.Stock < newQuantity)
                {
                    throw new Exception("Not enough stock available");
                }

                await _unitOfWork.ShoppingCartRepository.UpdateQuantity(accountId, bookId, newQuantity);
                await _unitOfWork.SaveChange();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckOutShoppingCart(int accountId, OrderDto orderDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cartItems = await _unitOfWork.ShoppingCartRepository.GetAllCart(accountId);

                if (cartItems == null || !cartItems.Any())
                {
                    throw new Exception("Shopping cart is empty");
                }

                foreach (var item in cartItems)
                {
                    var book = await _unitOfWork.BookRepository.GetByIdForUpdateAsync(item.BookID);
                    if (book == null)
                    {
                        throw new Exception($"Book not found: {item.BookID}");
                    }

                    if (book.Stock < item.Quantity)
                    {
                        throw new Exception($"Not enough stock for book: {book.Title}. Available: {book.Stock}, Requested: {item.Quantity}");
                    }

                    book.Stock -= item.Quantity;
                    await _unitOfWork.BookRepository.UpdateStock(book);
                }

                var order = new Order
                {
                    AccountID = accountId,
                    OrderDate = DateTime.Now,
                    Status = SD.Pending,
                    TotalPrice = cartItems.Sum(item => item.Book.Price * item.Quantity)
                };

                if (orderDto.UseAccountInfo)
                {
                    var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
                    if (account == null)
                    {
                        throw new Exception("Account not found");
                    }

                    order.CustomerName = account.Name;
                    order.CustomerEmail = account.Email;
                    order.CustomerPhone = account.PhoneNumber;
                    order.CustomerAddress = account.Address;
                }
                else
                {
                    order.CustomerName = orderDto.CustomerName;
                    order.CustomerEmail = orderDto.CustomerEmail;
                    order.CustomerPhone = orderDto.CustomerPhone;
                    order.CustomerAddress = orderDto.CustomerAddress;
                }

                await CreateOrder(order, cartItems);

                await _unitOfWork.ShoppingCartRepository.ClearCart(accountId);
                await _unitOfWork.SaveChange();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(); 
                throw;
            }
        }

        private async Task CreateOrder(Order order, List<ShoppingCart> cartItems)
        {
            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveChange();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    BookID = item.BookID,
                    Quantity = item.Quantity,
                    Price = item.Book.Price
                };

                await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);

                var book = await _unitOfWork.BookRepository.GetByIdAsync(item.BookID);
                book.Stock -= item.Quantity;
                await _unitOfWork.BookRepository.UpdateStock(book);
            }

            await _unitOfWork.SaveChange();
        }
    }
}