using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.DTO;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public interface IOderRepository    
    {
        // Basic CRUD operations
        Task<List<Order>> GetAll();
        Task<Order?> GetById(Guid id);
        Task<bool> Add(Order order);
        Task<bool> Update(Order order);
        Task<bool> Delete(Guid id);

        // Order filtering operations
        Task<List<Order>> GetByCustomerId(Guid customerId);
        Task<List<Order>> GetByStatus(string status);
        Task<List<Order>> GetOrderByUserId(Guid id);
        Task<List<OrderDTO>> GetOrdersByDealerAsync(Guid dealerId);
        Task<List<OrderDTO>> GetOrdersByCreatorAsync(Guid creatorUserId);
        Task<List<OrderDTO>> GetAllOrdersAsync();
        
        // Specific order operations
        Task<OrderDTO?> GetOrderByIdAsync(Guid orderId);
        Task<Guid> CreateOrderAsync(CreateOrderRequest req, Guid dealerId, Guid createdByUserId);
        Task<bool> UpdateOrderAsync(UpdateOrderRequest req, Guid updatedByUserId);
        Task<bool> SoftDeleteOrderAsync(Guid orderId, Guid deletedByUserId);
        Task<bool> AddOrderHistory(Guid orderId, string status, string notes, Guid createdBy);
        Task<bool> IsOrderCreatedByUserAsync(Guid orderId, Guid userId);
        
        // Vehicle-related operations
        Task<List<Vehicle>> GetAvailableVehiclesAsync();
        Task<List<VehicleSalesStats>> GetVehicleSalesStatsAsync();
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly Vehicle_Dealer_ManagementContext _context;

        public OrderRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAll()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vehicle)
                .Include(o => o.Dealer)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByStatus(string status)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vehicle)
                .Include(o => o.Dealer)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByCustomerId(Guid customerId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vehicle)
                .Include(o => o.Dealer)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetById(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vehicle)
                .Include(o => o.Dealer)
                .Include(o => o.OrderHistories)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> Add(Order order)
        {
            _context.Orders.Add(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(Order order)
        {
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<List<Order>> GetOrderByUserId(Guid id)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == id)
                .ToListAsync();
        }

        public async Task<List<OrderDTO>> GetOrdersByDealerAsync(Guid dealerId)
        {
            return await _context.Orders
                .Where(o => o.DealerId == dealerId)
                .Include(o => o.Customer)
                .Include(o => o.Dealer)
                .Include(o => o.Vehicle)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer != null ? o.Customer.FullName : "",
                    CustomerPhone = o.Customer != null ? o.Customer.Phone : "",
                    DealerId = o.DealerId,
                    DealerName = o.Dealer != null ? o.Dealer.Name : "",
                    VehicleId = o.VehicleId,
                    VehicleName = o.Vehicle != null ? o.Vehicle.Name : "",
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    Notes = o.Notes ?? "",
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<OrderDTO>> GetOrdersByCreatorAsync(Guid creatorUserId)
        {
            return await _context.Orders
                .Include(o => o.OrderHistories)
                .Include(o => o.Customer)
                .Include(o => o.Dealer)
                .Include(o => o.Vehicle)
                .Where(o => o.OrderHistories.Any(h => h.CreatedBy == creatorUserId && h.Notes == "Created by staff"))
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer != null ? o.Customer.FullName : "",
                    CustomerPhone = o.Customer != null ? o.Customer.Phone : "",
                    DealerId = o.DealerId,
                    DealerName = o.Dealer != null ? o.Dealer.Name : "",
                    VehicleId = o.VehicleId,
                    VehicleName = o.Vehicle != null ? o.Vehicle.Name : "",
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    Notes = o.Notes ?? "",
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<OrderDTO>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Dealer)
                .Include(o => o.Vehicle)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer != null ? o.Customer.FullName : "",
                    CustomerPhone = o.Customer != null ? o.Customer.Phone : "",
                    DealerId = o.DealerId,
                    DealerName = o.Dealer != null ? o.Dealer.Name : "",
                    VehicleId = o.VehicleId,
                    VehicleName = o.Vehicle != null ? o.Vehicle.Name : "",
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    Notes = o.Notes ?? "",
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(Guid orderId)
        {
            var o = await _context.Orders
                .Include(x => x.Customer)
                .Include(x => x.Dealer)
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (o == null) return null;

            return new OrderDTO
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.FullName ?? "",
                CustomerPhone = o.Customer?.Phone ?? "",
                DealerId = o.DealerId,
                DealerName = o.Dealer?.Name ?? "",
                VehicleId = o.VehicleId,
                VehicleName = o.Vehicle?.Name ?? "",
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                Notes = o.Notes ?? "",
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };
        }

        public async Task<Guid> CreateOrderAsync(CreateOrderRequest req, Guid dealerId, Guid createdByUserId)
        {
            User customer = null;
            if (req.CustomerId.HasValue)
            {
                customer = await _context.Users.FindAsync(req.CustomerId.Value);
            }

            if (customer == null)
            {
                customer = await _context.Users.FirstOrDefaultAsync(u => u.Phone == req.CustomerPhone && u.Role == "customer");
            }

            if (customer == null)
            {
                var generatedUserName = $"cust_{Guid.NewGuid().ToString().Substring(0, 8)}";
                var safeEmail = $"{generatedUserName}@autodeal.local";
                var safePassword = Guid.NewGuid().ToString("N");

                customer = new User
                {
                    Id = Guid.NewGuid(),
                    Username = generatedUserName,
                    Email = safeEmail,
                    Password = safePassword,
                    FullName = req.CustomerName,
                    Phone = req.CustomerPhone,
                    Role = "customer",
                    DealerId = null,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Users.Add(customer);
                await _context.SaveChangesAsync();
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}",
                CustomerId = customer.Id,
                DealerId = dealerId,
                VehicleId = req.VehicleId,
                TotalAmount = req.TotalAmount,
                Status = "processing",
                PaymentStatus = "unpaid",
                Notes = req.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var hist = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = order.Status,
                Notes = "Created by staff",
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };
            _context.OrderHistories.Add(hist);
            await _context.SaveChangesAsync();

            return order.Id;
        }

        public async Task<bool> UpdateOrderAsync(UpdateOrderRequest req, Guid updatedByUserId)
        {
            var order = await _context.Orders.FindAsync(req.Id);
            if (order == null) return false;

            order.VehicleId = req.VehicleId;
            order.TotalAmount = req.TotalAmount;
            order.Status = req.Status;
            order.PaymentStatus = req.PaymentStatus;
            order.Notes = req.Notes;
            order.UpdatedAt = DateTime.UtcNow;

            _context.Orders.Update(order);

            var hist = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = order.Status,
                Notes = "Updated by staff",
                CreatedBy = updatedByUserId,
                CreatedAt = DateTime.UtcNow
            };
            _context.OrderHistories.Add(hist);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteOrderAsync(Guid orderId, Guid deletedByUserId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = "cancelled";
            order.UpdatedAt = DateTime.UtcNow;
            _context.Orders.Update(order);

            var hist = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = "cancelled",
                Notes = "Cancelled by staff",
                CreatedBy = deletedByUserId,
                CreatedAt = DateTime.UtcNow
            };
            _context.OrderHistories.Add(hist);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _context.Vehicles
                .Where(v => v.IsActive == true)
                .ToListAsync();
        }

        public async Task<bool> IsOrderCreatedByUserAsync(Guid orderId, Guid userId)
        {
            return await _context.OrderHistories
                .AnyAsync(h => h.OrderId == orderId && h.CreatedBy == userId && h.Notes == "Created by staff");
        }

        public async Task<bool> AddOrderHistory(Guid orderId, string status, string notes, Guid createdBy)
        {
            var orderHistory = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Status = status,
                Notes = notes,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.OrderHistories.Add(orderHistory);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<VehicleSalesStats>> GetVehicleSalesStatsAsync()
        {
            return await _context.Orders
                .Include(o => o.Vehicle)
                .GroupBy(o => new { o.VehicleId, o.Vehicle.Name })
                .Select(g => new VehicleSalesStats
                {
                    VehicleId = g.Key.VehicleId,
                    VehicleName = g.Key.Name,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(s => s.TotalOrders)
                .Take(5)
                .ToListAsync();
        }
    }

    public class VehicleSalesStats
    {
        public Guid VehicleId { get; set; }
        public string VehicleName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
