using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Application.Interfaces.Services;
using BookStore.Domain.Consts;
using BookStore.Domain.Entities;

namespace BookStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task ApproveOrderByAdmin(int orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }
                if (order.Status == SD.Completed || order.Status == SD.Approved)
                {
                    throw new Exception("Order cannot be approved");
                }

                await _unitOfWork.OrderRepository.ApproveOrderByAdmin(orderId);
                await _unitOfWork.SaveChange();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<OrderDto>> GetAllOrder(int accountId)
        {
            try
            {
                var list = await _unitOfWork.OrderRepository.GetAllOrder(accountId);

                if (list == null || !list.Any())
                {
                    throw new Exception("Order not found");
                }

                return _mapper.Map<List<OrderDto>>(list);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<OrderDto>> GetAllOrderByAdmin()
        {
            try
            {
                var list = await _unitOfWork.OrderRepository.GetAllAsync();
                if (list == null)
                {
                    throw new Exception("Order not found");
                }
                return _mapper.Map<List<OrderDto>>(list);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrderDto> GetDetailOrder(int accountId, OrderDto orderDto)
        {
            try
            {
                var list = await _unitOfWork.OrderRepository.GetOrderDetail(accountId, _mapper.Map<Order>(orderDto));
                return _mapper.Map<OrderDto>(list);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrderDto> GetOrderDetailByAdmin(OrderDto orderDto)
        {
            try
            {
                var list = await _unitOfWork.OrderRepository.GetOrderDetailByAdmin(_mapper.Map<Order>(orderDto));
                return _mapper.Map<OrderDto>(list);
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
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }
                if (order.AccountID != accountId)
                {
                    throw new Exception("You are not the owner of this order");
                }
                if (order.Status == SD.Completed || order.Status == SD.Approved || order.Status == SD.Canceled)
                {
                    throw new Exception("Order cannot be rejected");
                }

                await _unitOfWork.OrderRepository.RejectOrder(accountId, orderId);
                await _unitOfWork.SaveChange();
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
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }
                if (order.Status == SD.Completed || order.Status == SD.Approved || order.Status == SD.Canceled)
                {
                    throw new Exception("Order cannot be rejected");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SuccessOrder(int accountId, OrderDto orderDto)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderDto.OrderID);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                if (order.AccountID != accountId)
                {
                    throw new Exception("You are not the owner of this order");
                }

                if (order.Status == SD.Completed || order.Status == SD.Pending || order.Status == SD.Canceled)
                {
                    throw new Exception("Order cannot be success");
                }

                await _unitOfWork.OrderRepository.SuccessOrder(accountId, orderDto.OrderID);

                foreach (var detail in order.OrderDetails)
                {
                    var report = new Report
                    {
                        OrderID = order.OrderID,
                        BookID = detail.BookID,
                        Quantity = detail.Quantity,
                        Price = detail.Price,
                        OrderDate = DateTime.Now,
                        CustomerReviews = orderDto.CustomerReviews ?? string.Empty
                    };

                    await _unitOfWork.ReportRepository.AddAsync(report);
                }

                await _unitOfWork.SaveChange();
                return true;
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
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

                if (!(order.Status == SD.Pending))
                {
                    throw new Exception("Order cannot update.");
                }

                await _unitOfWork.OrderRepository.UpdateAddressOrder(accountId, orderId, address);
                await _unitOfWork.SaveChange();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateAllOrder(int accountId, OrderDto orderDto)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderDto.OrderID);

                if (!(order.Status == SD.Pending))
                {
                    throw new Exception("Order cannot update.");
                }

                await _unitOfWork.OrderRepository.UpdateAllOrder(accountId, _mapper.Map<Order>(orderDto));
                await _unitOfWork.SaveChange();
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
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

                if (!(order.Status == SD.Pending))
                {
                    throw new Exception("Order cannot update.");
                }

                await _unitOfWork.OrderRepository.UpdateEmailOrder(accountId, orderId, email);
                await _unitOfWork.SaveChange();
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
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

                if (!(order.Status == SD.Pending))
                {
                    throw new Exception("Order cannot update.");
                }

                await _unitOfWork.OrderRepository.UpdateNameOrder(accountId, orderId, name);
                await _unitOfWork.SaveChange();
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
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

                if (!(order.Status == SD.Pending))
                {
                    throw new Exception("Order cannot update.");
                }

                await _unitOfWork.OrderRepository.UpdatePhoneOrder(accountId, orderId, phone);
                await _unitOfWork.SaveChange();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<ReportDto>> GetRevenueReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    throw new Exception("Start date cannot be greater than end date.");
                }

                var report = await _unitOfWork.OrderRepository.GetRevenueReportByDate(startDate, endDate);
                return report;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating revenue report: {ex.Message}", ex);
            }
        }
    }
}
