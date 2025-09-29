using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Specifications { get; set; }
        public string Images { get; set; }
        public int? StockQuantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
    }

    public class VehicleDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Specifications { get; set; }
        public string Images { get; set; }
        public int? StockQuantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
        public bool IsAvailable => StockQuantity > 0 && IsActive == true;
    }

    public class VehicleDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vehicle name is required")]
        [StringLength(100, ErrorMessage = "Vehicle name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(50, ErrorMessage = "Brand cannot exceed 50 characters")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters")]
        public string Model { get; set; }

        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
        public int? Year { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [StringLength(1000, ErrorMessage = "Specifications cannot exceed 1000 characters")]
        public string Specifications { get; set; }

        // Bỏ hết validation cho Images
        public string Images { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int? StockQuantity { get; set; }
    }
}
