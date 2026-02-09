using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Dto;
using TransactionService.Interfaces;
using TransactionService.Models;

namespace TransactionService.Repositories
{
    public class TransactionRepository : ITransactioRepository
    {
        private readonly ApplicationDbContext contextTransaction;

        public TransactionRepository( ApplicationDbContext context )
        {
            contextTransaction = context;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactions( int page, int limit )
        {
            var query = contextTransaction.Transactions.AsQueryable();

            var transactions = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(t => new TransactionDto
                {
                    IdTransaction = t.IdTransaction,
                    IdProduct = t.IdProduct,
                    DetailTransaction = t.DetailTransaction,
                    QuantityTransaction = t.QuantityTransaction,
                    UnitPriceTransaction = t.UnitPriceTransaction,
                    TotalPriceTransaction = t.TotalPriceTransaction,
                    TransactionType = t.TransactionType
                   
     
                })
                .ToListAsync();

            return transactions;

        }

        public async Task<TransactionDto?> GetTransactionById(int id)
        {
            var transaction = await contextTransaction.Transactions
                            .Where(t => t.IdTransaction == id)
                            .Select(t => new TransactionDto
                            {
                                IdTransaction = t.IdTransaction,
                                IdProduct = t.IdProduct,
                                DetailTransaction = t.DetailTransaction,
                                QuantityTransaction = t.QuantityTransaction,
                                UnitPriceTransaction = t.UnitPriceTransaction,
                                TotalPriceTransaction = t.TotalPriceTransaction,
                                TransactionType = t.TransactionType
                            })
                            .FirstOrDefaultAsync(); 

            return transaction;

        }

        public async Task<TransactionModel> CreateTransaction(TransactionModel model)
        { 
            
            contextTransaction.Transactions.Add(model);
            await contextTransaction.SaveChangesAsync();
            return model;
        }

        public async Task<TransactionModel> UpdateTransaction(TransactionModel model)
        {
            contextTransaction.Transactions.Update(model);
            await contextTransaction.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteTransaction(int id)
        {
            var model = await contextTransaction.Transactions
                .FirstOrDefaultAsync(
                                        x => x.IdTransaction == id
                                        
                                        );

            if (model == null)
            {  
                return false; 
            }
            contextTransaction.Transactions.Remove(model);
            await contextTransaction.SaveChangesAsync();
            return true; 

        }
        
    }
}