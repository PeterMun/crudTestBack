using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Dto;
using TransactionService.Interfaces;
using TransactionService.Models;
using System.Text.Json;

namespace TransactionService.Controllers
{
    public class TransactionController : ControllerBase
    {
        private readonly ITransactioRepository repoTransaction;
        private readonly ILogger<TransactionController> logger;
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        

        public TransactionController( 
            ITransactioRepository  repository,
            ILogger<TransactionController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration
        )
        { 
            repoTransaction = repository;
            this.logger = logger;
            httpClient = httpClientFactory.CreateClient();
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("/getAllTransactions")]
        public async Task<IActionResult> GetAllTransactions(
            [FromQuery] int page = 1, 
            [FromQuery] int limit = 5
        )
        {
            var transactions = await repoTransaction.GetAllTransactions(page, limit);

            return Ok(new
            {
                ok = true,
                msg = "Consulta realizada correctamente",
                data = transactions
            });
            
        }

        [HttpGet]
        [Route("/getTransactionById/{id}")]
        public async Task<IActionResult> GetTransactionById( int id )
        {
            var transaction = await repoTransaction.GetTransactionById(id);

            if (transaction == null)
            {
                return NotFound(new
                {
                    ok = false,
                    msg = "Transaccion no encontrada"
                });
            }

            return Ok(new
            {
                ok = true,
                msg = "Consulta realizada correctamente",
                data = transaction
            });

        }

        [HttpPost]
        [Route("/createTransaction")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto modelDto)
        {
            try
            {
                var productServiceUrl = configuration["Services:ProductService:BaseUrl"];
  
                var productResponse = await httpClient.GetAsync(
                    $"{productServiceUrl}/getProductById/{modelDto.IdProduct}"
                );

                if (!productResponse.IsSuccessStatusCode)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        msg = "Producto no encontrado"
                    });
                }

                var json = await productResponse.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(json);

                int currentStock = document
                    .RootElement
                    .GetProperty("data")
                    .GetProperty("stockProduct")
                    .GetInt32();


                if (modelDto.TransactionType == "VENTA")
                {
                    if (currentStock < modelDto.QuantityTransaction)
                    {
                        return BadRequest(new
                        {
                            ok = false,
                            msg = "Stock insuficiente para realizar la venta"
                        });
                    }

                    await httpClient.PutAsJsonAsync(
                        $"{productServiceUrl}/updateProductStock/{modelDto.IdProduct}",
                        new
                        {
                            quantity = modelDto.QuantityTransaction,
                            operation = "SUBTRACT"
                        }
                    );
                }

                if (modelDto.TransactionType == "COMPRA")
                {

                    var stockResponse =  await httpClient.PutAsJsonAsync(
                        $"{productServiceUrl}/updateProductStock/{modelDto.IdProduct}",
                        new
                        {
                            quantity = modelDto.QuantityTransaction,
                            operation = "ADD"
                        }
                    );

                    if (!stockResponse.IsSuccessStatusCode)
                    {
                        logger.LogError("No se pudo actualizar el stock del producto {Id}", modelDto.IdProduct);
                        return StatusCode(500, new
                        {
                            ok = false,
                            msg = "La transacción se creó, pero no se pudo actualizar el stock"
                        });
                    }
                }

                var model = new TransactionModel
                {
                    IdProduct = modelDto.IdProduct,
                    DetailTransaction = modelDto.DetailTransaction,
                    QuantityTransaction = modelDto.QuantityTransaction,
                    UnitPriceTransaction = modelDto.UnitPriceTransaction,
                    TotalPriceTransaction = modelDto.TotalPriceTransaction,
                    TransactionType = modelDto.TransactionType
                };

                var created = await repoTransaction.CreateTransaction(model);

                return Ok(new
                {
                    ok = true,
                    msg = "Transacción creada correctamente",
                    data = created
                });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error al crear la transacción");
                return StatusCode(500, new
                {
                    ok = false,
                    msg = "Error interno del servidor",
                    e
                });
            }
        }


        [HttpPut]
        [Route("/updateTransaction/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] TransactionDto modelDto)
        {
            var existing = await repoTransaction.GetTransactionById(id);

            if (existing == null)
            {
                return NotFound(new
                {
                    ok = false,
                    msg = "Transaccion no encontrada"
                });
            }

            var model = new TransactionModel
            {
                IdTransaction = id,
                IdProduct = modelDto.IdProduct,
                DetailTransaction = modelDto.DetailTransaction,
                QuantityTransaction = modelDto.QuantityTransaction,
                UnitPriceTransaction = modelDto.UnitPriceTransaction,
                TotalPriceTransaction = modelDto.TotalPriceTransaction,
                TransactionType = modelDto.TransactionType
            };

            await repoTransaction.UpdateTransaction(model);

            return Ok(new
            {
                ok = true,
                msg = "Transaccion actualizada correctamente"
            });

        }


        [HttpDelete]
        [Route("/deleteTransaction/")]
        public async Task<IActionResult> DeleteTransaction(int id)
         {

             try
             {
                 var deleted = await repoTransaction.DeleteTransaction( id );

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
                         msg = "Transaccion eliminada correctamente"
                     }
                 );


             }
             catch( DataException de )
             {
                 logger.LogError(de, "Error al borrar la transaccion con ID: {Id}", id);
                 return StatusCode(500, new
                 {
                     ok = false,
                     msg = "Error al acceder a los datos"
                 });
             }
             catch ( Exception e)
             {
                 logger.LogError(e, "Error inesperado al borrar la trnsaccion.");
                 return StatusCode(500, new
                 {
                     ok = false,
                     msg = "Ocurrió un error inesperado."
                 });
             }

         }




        
    }
}