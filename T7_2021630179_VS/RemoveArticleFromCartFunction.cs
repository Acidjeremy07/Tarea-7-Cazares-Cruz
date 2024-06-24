using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

public static class RemoveArticleFromCartFunction
{
    [FunctionName("RemoveArticleFromCart")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Processing a request to remove an article from the cart.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

        int articleId = data?.id_articulo;

        string connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");

        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    var cmdSelect = new MySqlCommand("SELECT cantidad FROM carrito_compra WHERE id_articulo = @articleId", connection);
                    cmdSelect.Parameters.AddWithValue("@articleId", articleId);

                    int quantity = Convert.ToInt32(await cmdSelect.ExecuteScalarAsync());

                    var cmdDelete = new MySqlCommand("DELETE FROM carrito_compra WHERE id_articulo = @articleId", connection);
                    cmdDelete.Parameters.AddWithValue("@articleId", articleId);
                    await cmdDelete.ExecuteNonQueryAsync();

                    var cmdUpdate = new MySqlCommand("UPDATE articulos SET cantidad = cantidad + @quantity WHERE id_articulo = @articleId", connection);
                    cmdUpdate.Parameters.AddWithValue("@quantity", quantity);
                    cmdUpdate.Parameters.AddWithValue("@articleId", articleId);
                    await cmdUpdate.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();

                    return new OkObjectResult("Artículo eliminado del carrito exitosamente.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    log.LogError(ex, "Error removing article from cart.");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
}
