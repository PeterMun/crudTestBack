using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Dto;
using ProductService.Interfaces;
using ProductService.Models;

namespace ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext contextProduct;

        public ProductRepository( ApplicationDbContext context )
        {
            contextProduct = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProducts( int page, int limit )
        {
            var query = contextProduct.Products.AsQueryable();

            var products = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(p => new ProductDto
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.NameProduct,
                    CategoryProduct = p.CategoryProduct,
                    ImageProduct = p.ImageProduct,
                    PriceProduct = p.PriceProduct,
                    StockProduct = p.StockProduct
                })
                .ToListAsync();

            return products;

        }

        public async Task<ProductDto?> GetProductById(int id)
        {
            var product = await contextProduct.Products
                            .Where(p => p.IdProduct == id)
                            .Select(p => new ProductDto
                            {
                                IdProduct = p.IdProduct,
                                NameProduct = p.NameProduct,
                                CategoryProduct = p.CategoryProduct,
                                ImageProduct = p.ImageProduct,
                                PriceProduct = p.PriceProduct,
                                StockProduct = p.StockProduct
                            })
                            .FirstOrDefaultAsync(); 

            return product;

        }

        public async Task<ProductModel> CreateProduct(ProductModel model)
        { 
            if (model.StockProduct < 0)
            {
                throw new Exception("El stock no puede ser negativo");
            }
            
            contextProduct.Products.Add(model);
            await contextProduct.SaveChangesAsync();
            return model;
        }

        public async Task<ProductModel> UpdateProduct(ProductModel model)
        {
            contextProduct.Products.Update(model);
            await contextProduct.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var model = await contextProduct.Products
                .FirstOrDefaultAsync(
                                        x => x.IdProduct == id
                                        
                                        );

            if (model == null)
            {  
                return false; 
            }
            contextProduct.Products.Remove(model);
            await contextProduct.SaveChangesAsync();
            return true; 

        }
        
    }
}