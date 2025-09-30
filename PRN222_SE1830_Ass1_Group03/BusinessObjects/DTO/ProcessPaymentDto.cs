using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO
{
    public class ProcessPaymentDto
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } // Cash, Card, Transfer

        public string Notes { get; set; }
    }
}
