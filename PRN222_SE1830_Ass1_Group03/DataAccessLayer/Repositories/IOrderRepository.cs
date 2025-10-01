using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrderByUserId(Guid id);
    }
}
