using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace BusinessObjects.DTO
    {
        public class OrderDTO
        {
            public Guid Id { get; set; }
            public Guid CustomerId { get; set; }
            public Guid DealerId { get; set; }
            public Guid VehicleId { get; set; }
            public decimal TotalPrice { get; set; }
            public string? Status { get; set; } = "pending";
            public string? PaymentStatus { get; set; } = "unpaid";
            public string? Notes { get; set; }
            public DateTime? CreatedAt { get; set; }
        }
    }
