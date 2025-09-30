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
        public List<Orderdto> MyOrders { get; set; } = new List<Orderdto>();
        public List<Orderdto> PendingOrders { get; set; } = new List<Orderdto>();
    }
}
