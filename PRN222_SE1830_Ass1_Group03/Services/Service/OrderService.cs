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
            var orders = await _orderRepository.GetAll();

            return orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.FullName,
                DealerId = o.DealerId,
                DealerName = o.Dealer?.Name,
                VehicleId = o.VehicleId,
                VehicleName = o.Vehicle?.Name,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            }).ToList();
        }


        public async Task<List<OrderDTO>> GetByStatus(string status)
        {
            var orders = await _orderRepository.GetByStatus(status);

            return orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.FullName,
                DealerId = o.DealerId,
                DealerName = o.Dealer?.Name,
                VehicleId = o.VehicleId,
                VehicleName = o.Vehicle?.Name,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt ?? DateTime.MinValue,
                UpdatedAt = o.UpdatedAt ?? DateTime.MinValue
            }).ToList();
        }


        public async Task<List<OrderDTO>> GetCustomerOrders(Guid customerId)
        {
            var orders = await _orderRepository.GetByCustomerId(customerId);

            return orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.FullName,
                DealerId = o.DealerId,
                DealerName = o.Dealer?.Name,
                VehicleId = o.VehicleId,
                VehicleName = o.Vehicle?.Name,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            }).ToList();
        }

        public async Task<OrderDTO?> GetOrderById(Guid id)
        {
            var order = await _orderRepository.GetById(id);

            if (order == null) return null;

            return new OrderDTO
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.FullName,
                DealerId = order.DealerId,
                DealerName = order.Dealer?.Name,
                VehicleId = order.VehicleId,
                VehicleName = order.Vehicle?.Name,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }


        public async Task<bool> CreateOrder(CreateOrderDTO dto)
        {

            var vehicle = await _vehicleRepository.GetById(dto.VehicleId);
            if (vehicle == null || vehicle.StockQuantity == null || vehicle.StockQuantity <= 0)
                return false;


            var dealers = await _dealerRepository.GetAll();
            if (dealers == null || !dealers.Any())
                return false;

            var dealerId = dealers.First().Id;

            // Tạo đơn hàng mới
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = GenerateOrderNumber(),
                CustomerId = dto.CustomerId,
                DealerId = dealerId,
                VehicleId = dto.VehicleId,
                TotalAmount = vehicle.Price,
                Status = "Processing",
                PaymentStatus = "Unpaid",
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var success = await _orderRepository.Add(order);
            if (success)
            {
                await _orderRepository.AddOrderHistory(order.Id, "Processing", "Đơn hàng được tạo", dto.CustomerId);
                vehicle.StockQuantity -= 1;
                await _vehicleRepository.UpdateAsync(vehicle);
            }

            return success;
        }

        public async Task<bool> ConfirmOrder(Guid orderId, Guid staffId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null || order.Status != "Processing")
                return false;

            order.Status = "Completed";
            order.UpdatedAt = DateTime.UtcNow;

            var success = await _orderRepository.Update(order);
            if (success)
                await _orderRepository.AddOrderHistory(orderId, "Completed", "Đơn hàng được xác nhận và hoàn tất bởi nhân viên", staffId);

            return success;
        }

        public async Task<bool> RejectOrder(Guid orderId, Guid customerId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null) return false;

            if (!order.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase))
                return false;

            order.Status = "Rejected";
            order.UpdatedAt = DateTime.UtcNow;

            var success = await _orderRepository.Update(order);
            if (success)
                await _orderRepository.AddOrderHistory(order.Id, "Rejected", "Khách hàng từ chối đơn hàng", customerId);

            return success;
        }

        public async Task<bool> CompletePayment(Guid orderId, Guid customerId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null) return false;

            if (!order.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase))
                return false;

            order.Status = "Completed";
            order.PaymentStatus = "Paid";
            order.UpdatedAt = DateTime.UtcNow;

            var success = await _orderRepository.Update(order);
            if (success)
            {
                var vehicle = await _vehicleRepository.GetById(order.VehicleId);
                if (vehicle != null && vehicle.StockQuantity.HasValue)
                {
                    vehicle.StockQuantity -= 1;
                    await _vehicleRepository.UpdateAsync(vehicle);
                }

                await _orderRepository.AddOrderHistory(orderId, "Completed", "Khách hàng đã thanh toán", customerId);
            }

            return success;
        }

        public async Task<bool> UpdateOrder(OrderDTO dto)
        {
            var order = await _orderRepository.GetById(dto.Id);
            if (order == null) return false;

            order.Status = dto.Status;
            order.TotalAmount = dto.TotalAmount;
            order.UpdatedAt = DateTime.UtcNow;

            return await _orderRepository.Update(order);
        }

        public async Task<bool> DeleteOrder(Guid id)
        {
            return await _orderRepository.Delete(id);
        }

        private string GenerateOrderNumber()

        public Task<List<Order>> GetOrderByUserId(Guid id)
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow.Ticks.ToString().Substring(10)}";
        }
    }

}
