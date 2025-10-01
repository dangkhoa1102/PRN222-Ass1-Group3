using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrderByUserId(Guid id);
    }
}
