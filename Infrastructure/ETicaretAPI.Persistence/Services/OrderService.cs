using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderWriteRepository _orderWriteRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository)
        {
            _orderWriteRepository = orderWriteRepository;
        }

        public Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task CreateOrderAsync(CreateOrderDto createOrder)
        {
            await _orderWriteRepository.AddAsync(new()
            {
                Adress = createOrder.Address,
                Id = Guid.Parse(createOrder.BasketId),
                Description = createOrder.Description
            });
            await _orderWriteRepository.SaveAsync();
        }

        public Task<ListOrderDto> GetAllOrdersAsync(int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<SingleOrderDto> GetOrderByIdAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
