using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Dto
{
    public class ProductUpdateStock
    {
        public int Quantity { get; set; }
        public string Operation { get; set; }
    }
}