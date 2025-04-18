using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Application.Interfaces.Services;
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

        public async Task ApproveOrder(int accountId, int orderId)
        {
            try
            {
                await _unitOfWork.OrderRepository.ApproveOrder(accountId, orderId);
                await _unitOfWork.SaveChange();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrderDto> GetAllOrder(int accountId)
        {
            try
            {
                var list = await _unitOfWork.OrderRepository.GetAllOrder(accountId);
                return _mapper.Map<OrderDto>(list);
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

        public async Task RejectOrder(int accountId, int orderId)
        {
            try
            {
                await _unitOfWork.OrderRepository.RejectOrder(accountId, orderId);
                await _unitOfWork.SaveChange();
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
                await _unitOfWork.OrderRepository.SuccessOrder(accountId, orderId);
                await _unitOfWork.SaveChange();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
