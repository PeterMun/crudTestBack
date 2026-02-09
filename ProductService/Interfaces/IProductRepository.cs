using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductService.Dto;
using ProductService.Models;

namespace ProductService.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllProducts( int page, int limit );
        Task<ProductDto?>  GetProductById( int id );
        Task<ProductModel> CreateProduct( ProductModel modelProduct );
        Task<ProductModel> UpdateProduct( ProductModel model );
        Task<bool> DeleteProduct( int id );
    }
}