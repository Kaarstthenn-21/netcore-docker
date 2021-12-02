using Entidades.DTO;
using Entidades.ErrorApi;
using Entidades.Utilitarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using MSRecursosHumanos.Filters;
using MSRecursosHumanos.Repo.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRecursosHumanos.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]    
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly EmployeeService service;
        private readonly IConfiguration config;

        public EmployeeController(EmployeeService employeeService, IConfiguration config)
        {
            this.service = employeeService;
            this.config = config;
        }

        [HttpGet, MapToApiVersion("1.0")]
        [Route("employees")]
        [AuthenticationFilter(IdServicio = 1)]
        public ActionResult<CollectionResponse<EmployeeDTO>> ListarEmpleadosActivosPaginado([FromQuery] int pagina, [FromQuery] int tamanio)
        {
            try
            {
                
                CollectionResponse<EmployeeDTO> response = service.ListarEmpleadosActivos(pagina, tamanio);

                return response;
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new ErrorResponse { IdError = 1, Mensaje = ex.Message, Traza = ex.StackTrace });
            }
        }


        private string GetToken(StringValues requestHeader)
        {
            return !string.IsNullOrEmpty(requestHeader) ? ((string)requestHeader).Split(' ')[1] : null;
        }

        [HttpPost, MapToApiVersion("1.0")]
        [Route("usuarios/login")]
        public ActionResult<TokenDTO> Login()
        {
            try
            {
                var authenticationToken = GetToken(HttpContext.Request.Headers["Authorization"]);
                var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                var credentials = decodedToken.Split(':');
                var login = new LoginDTO
                {
                    Contrasenha = credentials[1],
                    Usuario = credentials[0]
                };
                return service.Login(login);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new ErrorResponse { IdError = 1, Mensaje = ex.Message, Traza = ex.StackTrace });
            }

        }


          
        [HttpPost, MapToApiVersion("1.0")]
        [Route("uploads")]
        public async Task<ActionResult<string>> SubirArchivoAsync(IFormCollection formdata)
        {
            try
            {
                var fileBytes = new List<byte[]>();
                var archivo = formdata.Files.Where(a => a.Name.Equals("archivo")).ToList();
                if ( archivo.Count == 0 || archivo[0].Length == 0) { return new BadRequestObjectResult(new ErrorResponse { IdError = 1, Mensaje = "No hay archivo para subir", Traza = "Controller" }); }


                
                using (var memoryStream = new MemoryStream())
                {
                    await archivo[0].CopyToAsync(memoryStream);
                    fileBytes.Add(memoryStream.ToArray());
                }

                
                string[] extension = archivo[0].FileName.Split(".");
                var nombreTemp = $"{Guid.NewGuid().ToString("N")}.{extension[1]}";

                var fileBase64 = Convert.ToBase64String(fileBytes[0]);

                ArchivoS3 file = new ArchivoS3();
                file.Accesibilidad = ArchivoS3.Accesible.LECTURA_PUBLICA;
                file.Bucket = "dev-contabilidad";
                file.Nombre = nombreTemp;
                file.Base64 = fileBase64;
                file.AccessKey = config["access_key"];
                file.SecretKey = config["secret_key"];
                string response = service.SubirArchivo(file);

                return response;
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new ErrorResponse { IdError = 1, Mensaje = ex.Message, Traza = ex.StackTrace });
            }
        }




    }
}
