using ETicaretAPI.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CreateOrderDto createOrder);
        Task<ListOrderDto> GetAllOrdersAsync(int page, int size);
        Task<SingleOrderDto> GetOrderByIdAsync(string id);
        Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id);
    }
}
