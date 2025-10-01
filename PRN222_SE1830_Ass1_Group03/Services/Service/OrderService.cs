using BusinessObjects.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }
        public Task<List<Order>> GetOrderByUserId(Guid id)
        {
            return _orderRepo.GetOrderByUserId(id);
        }
    }
}
