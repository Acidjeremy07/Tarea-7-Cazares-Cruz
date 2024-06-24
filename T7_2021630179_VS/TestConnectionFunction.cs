using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System;

public static class TestConnectionFunction
{
    [FunctionName("TestConnectionFunction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return new OkObjectResult("Connection successful");
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error: {ex.Message}");
            return new StatusCodeResult(500);
        }
    }
}
