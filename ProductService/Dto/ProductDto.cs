using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Dto
{
    public class ProductDto
    {
        public int IdProduct { get; set; }
        public string? NameProduct { get; set; } 
        public string? DescriptionProduct { get; set; } 
        public string? CategoryProduct { get; set; } 
        public string? ImageProduct { get; set; }
        public decimal? PriceProduct { get; set; }
        public int? StockProduct { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
    }
}