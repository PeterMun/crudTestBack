using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionService.Dto
{
    public class TransactionDto
    {
        public int IdTransaction { get; set; }
        public int? IdProduct { get; set; } 
        public string? DetailTransaction { get; set; } 
        public int? QuantityTransaction { get; set; } 
        public decimal? UnitPriceTransaction { get; set; }
        public decimal? TotalPriceTransaction { get; set; }
        public string? TransactionType { get; set; }
        public DateTime? CreatedAt { get; set; }
        
        
    }
}