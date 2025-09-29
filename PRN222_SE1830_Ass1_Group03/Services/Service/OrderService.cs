using BusinessObjects.DTO;
using BusinessObjects.Models;
using DataAccessLayer.Repositories;

namespace Services.Service
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly VehicleRepository _vehicleRepository;

        public OrderService(OrderRepository orderRepository, VehicleRepository vehicleRepository)
        {
            _orderRepository = orderRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<List<Orderdto>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAll();

            return orders.Select(o => new Orderdto
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


        public async Task<List<Orderdto>> GetByStatus(string status)
        {
            var orders = await _orderRepository.GetByStatus(status);

            return orders.Select(o => new Orderdto
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


        public async Task<List<Orderdto>> GetCustomerOrders(Guid customerId)
        {
            var orders = await _orderRepository.GetByCustomerId(customerId);

            return orders.Select(o => new Orderdto
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

        public async Task<Orderdto> GetOrderById(Guid id)
        {
            var order = await _orderRepository.GetById(id);

            if (order == null) return null;

            return new Orderdto
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


        public async Task<bool> CreateOrder(CreateOrderDto dto)
        {
            // Get vehicle details to calculate total amount
            var vehicle = await _vehicleRepository.GetById(dto.VehicleId);
            if (vehicle == null || vehicle.StockQuantity <= 0)
            {
                return false; // Vehicle not available
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = GenerateOrderNumber(),
                CustomerId = dto.CustomerId,
                DealerId = dto.DealerId,
                VehicleId = dto.VehicleId,
                TotalAmount = vehicle.Price,
                Status = "Pending",
                PaymentStatus = "Unpaid",
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await _orderRepository.Add(order);
            if (success)
            {
                // Add order history
                await _orderRepository.AddOrderHistory(order.Id, "Pending", "Đơn hàng được tạo", dto.CustomerId);
            }

            return success;
        }

        public async Task<bool> ConfirmOrder(Guid orderId, Guid staffId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null || order.Status != "Pending")
            {
                return false;
            }

            order.Status = "Confirmed";
            order.UpdatedAt = DateTime.UtcNow;

            var success = await _orderRepository.Update(order);
            if (success)
            {
                await _orderRepository.AddOrderHistory(orderId, "Confirmed", "Đơn hàng được xác nhận bởi nhân viên", staffId);
            }

            return success;
        }

        public async Task<bool> CompletePayment(Guid orderId, Guid customerId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null || order.Status != "Confirmed")
            {
                return false;
            }

            order.Status = "Completed";
            order.PaymentStatus = "Paid";
            order.UpdatedAt = DateTime.UtcNow;

            var success = await _orderRepository.Update(order);
            if (success)
            {
                // Reduce vehicle stock
                var vehicle = await _vehicleRepository.GetById(order.VehicleId);
                if (vehicle != null && vehicle.StockQuantity.HasValue)
                {
                    await _vehicleRepository.UpdateStock(order.VehicleId, vehicle.StockQuantity.Value - 1);
                }

                await _orderRepository.AddOrderHistory(orderId, "Completed", "Thanh toán hoàn tất", customerId);
            }

            return success;
        }

        public async Task<bool> UpdateOrder(Orderdto dto)
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
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow.Ticks.ToString().Substring(10)}";
        }
    }
}
