using System;

namespace BusinessObjects.DTO
{
    public class CreateOrderRequest
    {
        public Guid DealerId { get; set; }       // lấy từ ViewBag
        public Guid StaffId { get; set; }        // lấy từ ViewBag
        public Guid? CustomerId { get; set; }    // có thể null (nếu khách mới)

        // Thông tin khách hàng mới
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        // Thông tin đơn hàng
        public Guid VehicleId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }

        // Optional
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
