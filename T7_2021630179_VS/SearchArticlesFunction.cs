using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;

public static class SearchArticlesFunction
{
    [FunctionName("SearchArticles")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Processing a request to search articles.");

        string keyword = req.Query["keyword"];
        if (string.IsNullOrEmpty(keyword))
        {
            return new BadRequestObjectResult("Please provide a search keyword.");
        }

        string connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new MySqlCommand(
                    @"SELECT a.id_articulo, a.nombre, a.descripcion, a.precio, f.foto 
                      FROM articulos a 
                      LEFT JOIN fotos_articulos f ON a.id_articulo = f.id_articulo 
                      WHERE a.nombre LIKE @keyword OR a.descripcion LIKE @keyword",
                    connection);

                cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                cmd.CommandTimeout = 120;

                var reader = await cmd.ExecuteReaderAsync();
                var articles = new List<dynamic>();

                while (await reader.ReadAsync())
                {
                    articles.Add(new
                    {
                        Id = reader["id_articulo"],
                        Name = reader["nombre"],
                        Description = reader["descripcion"],
                        Price = reader["precio"],
                        Photo = reader["foto"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["foto"]) : null
                    });
                }

                return new OkObjectResult(articles);
            }
        }
        catch (MySqlException ex)
        {
            log.LogError($"MySQL error: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch (Exception ex)
        {
            log.LogError($"General error: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
