// (c) Carlos Pineda Guerrero. 2024
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp3
{
    public static class alta_articulo
    {
        class Articulo
        {
            public string? nombre { get; set; }
            public string? descripcion { get; set; }
            public float? precio { get; set; }
            public int? cantidad { get; set; }
            public string? foto { get; set; }  // foto en base 64
        }

        class ParamAltaArticulo
        {
            public Articulo? articulo { get; set; }
        }

        class Error
        {
            public string mensaje { get; set; }
            public Error(string mensaje)
            {
                this.mensaje = mensaje;
            }
        }

        [FunctionName("alta_articulo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string body = await new StreamReader(req.Body).ReadToEndAsync();
                ParamAltaArticulo? data = JsonConvert.DeserializeObject<ParamAltaArticulo>(body);
                Articulo? articulo = data?.articulo;

                if (articulo == null)
                {
                    return new BadRequestObjectResult(new Error("Datos de artículo inválidos"));
                }

                string connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");

                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();
                    using (var transaccion = await conexion.BeginTransactionAsync())
                    {
                        try
                        {
                            var cmd_1 = new MySqlCommand
                            {
                                Connection = conexion,
                                Transaction = transaccion,
                                CommandText = "INSERT INTO articulos(nombre, descripcion, precio, cantidad) VALUES (@nombre, @descripcion, @precio, @cantidad)"
                            };
                            cmd_1.Parameters.AddWithValue("@nombre", articulo.nombre);
                            cmd_1.Parameters.AddWithValue("@descripcion", articulo.descripcion);
                            cmd_1.Parameters.AddWithValue("@precio", articulo.precio);
                            cmd_1.Parameters.AddWithValue("@cantidad", articulo.cantidad);
                            await cmd_1.ExecuteNonQueryAsync();

                            long idArticulo = cmd_1.LastInsertedId;

                            var cmd_2 = new MySqlCommand
                            {
                                Connection = conexion,
                                Transaction = transaccion,
                                CommandText = "INSERT INTO fotos_articulos (foto, id_articulo) VALUES (@foto, @id_articulo)"
                            };
                            cmd_2.Parameters.AddWithValue("@foto", Convert.FromBase64String(articulo.foto));
                            cmd_2.Parameters.AddWithValue("@id_articulo", idArticulo);
                            await cmd_2.ExecuteNonQueryAsync();

                            await transaccion.CommitAsync();
                            return new OkObjectResult("Se dió de alta el artículo");
                        }
                        catch (Exception e)
                        {
                            await transaccion.RollbackAsync();
                            log.LogError($"Error: {e.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new Error(e.Message));
            }
        }
    }
}
