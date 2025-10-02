using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO
{
    public class OrderHistoryDTO
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
