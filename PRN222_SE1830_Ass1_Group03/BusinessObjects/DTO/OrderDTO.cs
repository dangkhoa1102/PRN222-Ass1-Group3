using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }

        // Thông tin khách hàng
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }

        // Thông tin đại lý
        public Guid DealerId { get; set; }
        public string DealerName { get; set; }
        public string DealerCode { get; set; }

        // Thông tin nhân viên (staff)
        public Guid? StaffId { get; set; }
        public string StaffName { get; set; }

        // Thông tin xe
        public Guid VehicleId { get; set; }
        public string VehicleName { get; set; }
        public string VehicleBrand { get; set; }
        public string VehicleModel { get; set; }
        public decimal VehiclePrice { get; set; }

        // Thông tin đơn hàng
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Notes { get; set; }

        // Thời gian
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
