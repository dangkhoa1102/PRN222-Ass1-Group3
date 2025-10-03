using BusinessObjects.DTO;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrderByUserId(Guid id);
        Task<List<OrderDTO>> GetOrdersByDealerAsync(Guid dealerId);
        Task<List<OrderDTO>> GetOrdersByCreatorAsync(Guid creatorUserId);
        Task<List<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO?> GetOrderByIdAsync(Guid orderId);
        Task<Guid> CreateOrderAsync(CreateOrderRequest req, Guid dealerId, Guid createdByUserId);
        Task<bool> UpdateOrderAsync(UpdateOrderRequest req, Guid updatedByUserId);
        Task<bool> SoftDeleteOrderAsync(Guid orderId, Guid deletedByUserId);
        Task<List<Vehicle>> GetAvailableVehiclesAsync();
        Task<bool> IsOrderCreatedByUserAsync(Guid orderId, Guid userId);
        Task<List<VehicleSalesStats>> GetVehicleSalesStatsAsync();
    }
}
