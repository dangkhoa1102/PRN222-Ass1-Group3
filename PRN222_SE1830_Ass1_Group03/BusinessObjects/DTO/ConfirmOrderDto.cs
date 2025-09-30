using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO
{
    public class ConfirmOrderDto
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        public string Notes { get; set; }
    }
}
