using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionService.Dto;
using TransactionService.Models;


namespace TransactionService.Interfaces
{
    public interface ITransactioRepository
    {
        Task<IEnumerable<TransactionDto>> GetAllTransactions( int page, int limit );
        Task<TransactionDto?>  GetTransactionById( int id );
        Task<TransactionModel> CreateTransaction( TransactionModel modelTransaction );
        Task<TransactionModel> UpdateTransaction( TransactionModel model );
        Task<bool> DeleteTransaction( int id );
        
    }
}