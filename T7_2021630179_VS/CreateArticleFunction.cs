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

public static class CreateArticleFunction
{
    [FunctionName("CreateArticle")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Processing a request to create an article.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

        string name = data?.name;
        string description = data?.description;
        decimal price = data?.price;
        int quantity = data?.quantity;
        string photo = data?.photo;

        string connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");

        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    var cmd = new MySqlCommand("INSERT INTO articulos (nombre, descripcion, precio, cantidad) VALUES (@name, @description, @price, @quantity)", connection);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    await cmd.ExecuteNonQueryAsync();

                    long articleId = cmd.LastInsertedId;

                    var cmdPhoto = new MySqlCommand("INSERT INTO fotos_articulos (foto, id_articulo) VALUES (@photo, @articleId)", connection);
                    cmdPhoto.Parameters.AddWithValue("@photo", Convert.FromBase64String(photo));
                    cmdPhoto.Parameters.AddWithValue("@articleId", articleId);
                    await cmdPhoto.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();

                    return new OkObjectResult("Article created successfully.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    log.LogError(ex, "Error creating article.");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
}
