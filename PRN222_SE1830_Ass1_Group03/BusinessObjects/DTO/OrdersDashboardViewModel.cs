using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace BusinessObjects.DTO
{
    public class OrdersDashboardViewModel
    {
        public List<OrderDTO> MyOrders { get; set; } = new List<OrderDTO>();
        public List<OrderDTO> PendingOrders { get; set; } = new List<OrderDTO>();
    }
}
