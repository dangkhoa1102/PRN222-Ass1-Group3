using BusinessObjects.DTO;
using BusinessObjects.Models;
using DataAccessLayer.Repositories;

namespace Services.Service
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;

        public OrderService(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _orderRepository.GetAll();
        }

        public async Task<Order?> GetOrderById(Guid id)
        {
            return await _orderRepository.GetById(id);
        }

        public async Task<bool> AddOrder(OrderDTO dto)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}", 
                CustomerId = dto.CustomerId,
                DealerId = dto.DealerId, 
                VehicleId = dto.VehicleId,
                TotalAmount = dto.TotalPrice,
                Status = dto.Status?.ToLower() ?? "pending", 
                PaymentStatus = dto.PaymentStatus?.ToLower() ?? "unpaid",
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _orderRepository.Add(order);
        }

        public async Task<bool> UpdateOrder(OrderDTO dto)
        {
            var order = await _orderRepository.GetById(dto.Id);
            if (order == null) return false;

            order.Status = dto.Status;
            order.TotalAmount = dto.TotalPrice;
            order.CreatedAt = dto.CreatedAt;

            return await _orderRepository.Update(order);
        }

        public async Task<bool> DeleteOrder(Guid id)
        {
            return await _orderRepository.Delete(id);
        }
    }
}
