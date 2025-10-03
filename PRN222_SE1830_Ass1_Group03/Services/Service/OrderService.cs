using BusinessObjects.DTO;
using BusinessObjects.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetAllOrders();
        Task<List<OrderDTO>> GetByStatus(string status);
        Task<List<OrderDTO>> GetCustomerOrders(Guid customerId);
        Task<OrderDTO?> GetOrderById(Guid id);
        Task<bool> CreateOrder(CreateOrderDTO dto);
        Task<bool> ConfirmOrder(Guid orderId, Guid staffId);
        Task<bool> CompletePayment(Guid orderId, Guid customerId);
        Task<bool> UpdateOrder(OrderDTO dto);
        Task<bool> DeleteOrder(Guid id);
        Task<bool> RejectOrder(Guid orderId, Guid customerId);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDealerRepository _dealerRepository;

        public OrderService(IOrderRepository orderRepository, IVehicleRepository vehicleRepository, IDealerRepository dealerRepository)
        {
            _orderRepository = orderRepository;
            _vehicleRepository = vehicleRepository;
            _dealerRepository = dealerRepository;
        }

        public async Task<List<OrderDTO>> GetAllOrders()
        {
            // Lấy tất cả đơn hàng dạng DTO
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<List<OrderDTO>> GetByStatus(string status)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<List<OrderDTO>> GetCustomerOrders(Guid customerId)
        {
            // Lấy tất cả đơn hàng rồi lọc theo customerId
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Where(o => o.CustomerId == customerId).ToList();
        }

        public async Task<OrderDTO?> GetOrderById(Guid id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<bool> CreateOrder(CreateOrderDTO dto)
        {
            // Validate vehicle
            var vehicle = await _vehicleRepository.GetById(dto.VehicleId);
            if (vehicle == null || vehicle.StockQuantity == null || vehicle.StockQuantity <= 0)
                return false;

            // Validate dealer
            var dealer = await _dealerRepository.GetById(dto.DealerId);
            if (dealer == null)
                return false;

            // Tạo request
            var req = new CreateOrderRequest
            {
                DealerId = dto.DealerId,
                CustomerId = dto.CustomerId,
                VehicleId = dto.VehicleId,
                TotalAmount = vehicle.Price,
                Notes = dto.Notes
            };

            // Tạo order (giả sử userId là customerId, bạn có thể sửa lại nếu cần)
            var orderId = await _orderRepository.CreateOrderAsync(req, dto.DealerId, dto.CustomerId);

            if (orderId != Guid.Empty)
            {
                vehicle.StockQuantity -= 1;
                await _vehicleRepository.UpdateAsync(vehicle);
                return true;
            }
            return false;
        }

        public async Task<bool> ConfirmOrder(Guid orderId, Guid staffId)
        {
            // Lấy order
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null || !order.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase))
                return false;

            // Update order
            var req = new UpdateOrderRequest
            {
                Id = orderId,
                VehicleId = order.VehicleId,
                TotalAmount = order.TotalAmount,
                Status = "Completed",
                PaymentStatus = order.PaymentStatus,
                Notes = order.Notes
            };
            return await _orderRepository.UpdateOrderAsync(req, staffId);
        }

        public async Task<bool> RejectOrder(Guid orderId, Guid customerId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null || !order.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase))
                return false;

            var req = new UpdateOrderRequest
            {
                Id = orderId,
                VehicleId = order.VehicleId,
                TotalAmount = order.TotalAmount,
                Status = "Rejected",
                PaymentStatus = order.PaymentStatus,
                Notes = "Khách hàng từ chối đơn hàng"
            };
            return await _orderRepository.UpdateOrderAsync(req, customerId);
        }

        public async Task<bool> CompletePayment(Guid orderId, Guid customerId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null || !order.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase))
                return false;

            var req = new UpdateOrderRequest
            {
                Id = orderId,
                VehicleId = order.VehicleId,
                TotalAmount = order.TotalAmount,
                Status = "Completed",
                PaymentStatus = "Paid",
                Notes = order.Notes
            };
            var result = await _orderRepository.UpdateOrderAsync(req, customerId);

            if (result)
            {
                var vehicle = await _vehicleRepository.GetById(order.VehicleId);
                if (vehicle != null && vehicle.StockQuantity.HasValue)
                {
                    vehicle.StockQuantity -= 1;
                    await _vehicleRepository.UpdateAsync(vehicle);
                }
            }
            return result;
        }

        public async Task<bool> UpdateOrder(OrderDTO dto)
        {
            var req = new UpdateOrderRequest
            {
                Id = dto.Id,
                VehicleId = dto.VehicleId,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status,
                PaymentStatus = dto.PaymentStatus,
                Notes = dto.Notes
            };
            // Giả sử updatedByUserId là null hoặc lấy từ context
            return await _orderRepository.UpdateOrderAsync(req, Guid.Empty);
        }

        public async Task<bool> DeleteOrder(Guid id)
        {
            // Soft delete
            return await _orderRepository.SoftDeleteOrderAsync(id, Guid.Empty);
        }
    }

}
