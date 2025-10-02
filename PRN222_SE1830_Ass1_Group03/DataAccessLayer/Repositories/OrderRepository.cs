using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.DTO;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class OrderRepository
    {
        private readonly Vehicle_Dealer_ManagementContext _context;

        public OrderRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        // Lấy orders theo dealer (đây là cách bạn show orders cho staff: staff.dealer_id == orders.dealer_id)
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

        // Lấy orders do một nhân viên tạo (dựa vào lịch sử tạo đầu tiên)
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

        // Dành cho evm_staff: lấy tất cả orders
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

        // Tạo order: tạo customer nếu cần, gán dealerId = dealerId (lấy từ staff)
        public async Task<Guid> CreateOrderAsync(CreateOrderRequest req, Guid dealerId, Guid createdByUserId)
        {
            // 1) tìm hoặc tạo customer
            User customer = null;
            if (req.CustomerId.HasValue)
            {
                customer = await _context.Users.FindAsync(req.CustomerId.Value);
            }

            if (customer == null)
            {
                // try find by phone
                customer = await _context.Users.FirstOrDefaultAsync(u => u.Phone == req.CustomerPhone && u.Role == "customer");
            }

            if (customer == null)
            {
                var generatedUserName = $"cust_{Guid.NewGuid().ToString().Substring(0, 8)}";
                // Ensure required fields (Email, Password) are non-null to satisfy DB constraints
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

            // 2) tạo order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}",
                CustomerId = customer.Id,
                DealerId = dealerId,
                VehicleId = req.VehicleId,
                TotalAmount = req.TotalAmount,
                Status = "processing",      // bắt đầu là processing (bạn có thể đổi)
                PaymentStatus = "unpaid",
                Notes = req.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 3) ghi lịch sử vào order_history
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

        // Update đơn (status/payment/vehicle/total/notes)
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

        // Soft delete: set status = cancelled + history
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
                .Where(v => v.IsActive == true)   // EF property likely IsActive
                .ToListAsync();
        }

        // Kiểm tra đơn có được tạo bởi user này hay không
        public async Task<bool> IsOrderCreatedByUserAsync(Guid orderId, Guid userId)
        {
            return await _context.OrderHistories
                .AnyAsync(h => h.OrderId == orderId && h.CreatedBy == userId && h.Notes == "Created by staff");
        }

        // Thống kê xe bán chạy
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
