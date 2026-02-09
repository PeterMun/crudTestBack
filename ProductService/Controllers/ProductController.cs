using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductService.Dto;
using ProductService.Interfaces;
using ProductService.Models;

namespace ProductService.Controllers
{
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository repoProduct;
        private readonly ILogger<ProductController> logger;

        public ProductController( 
            IProductRepository  repository,
            ILogger<ProductController> logger 
        )
        { 
            repoProduct = repository;
            this.logger = logger;
        }
        

        [HttpGet]
        [Route("/getAllProducts")]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] int page = 1, 
            [FromQuery] int limit = 5
        )
        {
            var products = await repoProduct.GetAllProducts(page, limit);

            return Ok(new
            {
                ok = true,
                msg = "Consulta realizada correctamente",
                data = products
            });
            
        }

        [HttpGet]
        [Route("/getProductById/{id}")]
        public async Task<IActionResult> GetProductById( int id )
        {
            var product = await repoProduct.GetProductById(id);

            if (product == null)
            {
                return NotFound(new
                {
                    ok = false,
                    msg = "Producto no encontrado"
                });
            }

            return Ok(new
            {
                ok = true,
                msg = "Consulta realizada correctamente",
                data = product
            });

        }

        [HttpPost]
        [Route("/createProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto modelDto)
        {
            var model = new ProductModel
            {
                NameProduct = modelDto.NameProduct,
                DescriptionProduct = modelDto.DescriptionProduct,
                CategoryProduct = modelDto.CategoryProduct,
                ImageProduct = modelDto.ImageProduct,
                PriceProduct = modelDto.PriceProduct,
                StockProduct = modelDto.StockProduct
            };

            var created = await repoProduct.CreateProduct(model);

            return Ok(new
            {
                ok = true,
                msg = "Producto creado correctamente",
                data = created
            });

        }

        [HttpPut]
        [Route("/updateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto modelDto)
        {
            var existing = await repoProduct.GetProductById(id);

            if (existing == null)
            {
                return NotFound(new
                {
                    ok = false,
                    msg = "Producto no encontrado"
                });
            }

            var model = new ProductModel
            {
                IdProduct = id,
                NameProduct = modelDto.NameProduct,
                CategoryProduct = modelDto.CategoryProduct,
                ImageProduct = modelDto.ImageProduct,
                PriceProduct = modelDto.PriceProduct,
                StockProduct = modelDto.StockProduct
            };

            await repoProduct.UpdateProduct(model);

            return Ok(new
            {
                ok = true,
                msg = "Producto actualizado correctamente"
            });

        }

        [HttpDelete]
        [Route("/deleteProduct/")]
        public async Task<IActionResult> DeleteProduct(int id)
         {

             try
             {
                 var deleted = await repoProduct.DeleteProduct( id );

                 if ( !deleted )
                 {
                     return NotFound(
                         new {
                             ok = false,
                             msg = $"No se encontró el registro con el id especificado"
                         }
                     );  
                 }

                 return Ok(
                     new {
                         ok = true,
                         msg = "Producto eliminado correctamente"
                     }
                 );


             }
             catch( DataException de )
             {
                 logger.LogError(de, "Error al borrar el producto con ID: {Id}", id);
                 return StatusCode(500, new
                 {
                     ok = false,
                     msg = "Error al acceder a los datos"
                 });
             }
             catch ( Exception e)
             {
                 logger.LogError(e, "Error inesperado al borrar el producto.");
                 return StatusCode(500, new
                 {
                     ok = false,
                     msg = "Ocurrió un error inesperado."
                 });
             }

        }

        [HttpPut]
        [Route("/updateProductStock/{id}")]
        public async Task<IActionResult> UpdateProductStock(int id, [FromBody] ProductUpdateStock body)
        {
            var productDto = await repoProduct.GetProductById(id);

            if (productDto == null)
            {
                return NotFound(new { ok = false, msg = "Producto no encontrado" });
            }

            int quantity = (int)body.Quantity;
            string operation = (string)body.Operation;

            int newStock = productDto.StockProduct ?? 0;

            if (operation == "ADD")
                newStock += quantity;

            if (operation == "SUBTRACT")
                newStock -= quantity;

            var productModel = new ProductModel
            {
                IdProduct = productDto.IdProduct,
                NameProduct = productDto.NameProduct,
                DescriptionProduct = productDto.DescriptionProduct,
                CategoryProduct = productDto.CategoryProduct,
                ImageProduct = productDto.ImageProduct,
                PriceProduct = productDto.PriceProduct,
                StockProduct = newStock
            };

            await repoProduct.UpdateProduct(productModel);

            return Ok(new
            {
                ok = true,
                msg = "Stock actualizado correctamente"
            });
        }








    }
}