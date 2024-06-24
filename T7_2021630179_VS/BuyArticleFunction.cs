using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;

public static class BuyArticleFunction
{
    [FunctionName("BuyArticle")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Processing a request to buy an article.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

        int articleId = data?.id_articulo;
        int quantity = data?.cantidad;

        if (articleId == 0 || quantity == 0)
        {
            return new BadRequestObjectResult("Please provide a valid article ID and quantity.");
        }

        string connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var cmdSelect = new MySqlCommand("SELECT cantidad FROM articulos WHERE id_articulo = @articleId", connection);
                        cmdSelect.Parameters.AddWithValue("@articleId", articleId);

                        int currentQuantity = Convert.ToInt32(await cmdSelect.ExecuteScalarAsync());

                        if (currentQuantity < quantity)
                        {
                            return new BadRequestObjectResult("No hay suficientes artículos.");
                        }

                        var cmdUpdate = new MySqlCommand("UPDATE articulos SET cantidad = cantidad - @quantity WHERE id_articulo = @articleId", connection);
                        cmdUpdate.Parameters.AddWithValue("@quantity", quantity);
                        cmdUpdate.Parameters.AddWithValue("@articleId", articleId);
                        await cmdUpdate.ExecuteNonQueryAsync();

                        var cmdInsert = new MySqlCommand("INSERT INTO carrito_compra (id_articulo, cantidad) VALUES (@articleId, @quantity)", connection);
                        cmdInsert.Parameters.AddWithValue("@articleId", articleId);
                        cmdInsert.Parameters.AddWithValue("@quantity", quantity);
                        await cmdInsert.ExecuteNonQueryAsync();

                        await transaction.CommitAsync();

                        return new OkObjectResult("Artículo comprado exitosamente.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        log.LogError(ex, "Error buying article.");
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error: {ex.Message}");
            return new StatusCodeResult(500);
        }
    }
}
