using System;

namespace BusinessObjects.DTO
{
    public class UpdateOrderRequest
    {
        public Guid Id { get; set; }

        public Guid VehicleId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Notes { get; set; }

        // hiển thị lại trong View
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
