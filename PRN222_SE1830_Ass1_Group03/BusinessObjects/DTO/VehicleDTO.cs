using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO
{
    public class VehicleDTO
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
    }
}
