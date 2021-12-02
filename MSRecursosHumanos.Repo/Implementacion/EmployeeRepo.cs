using Entidades.DTO;
using Entidades.Interface;
using Entidades.Utilitarios;
using Infraestructura.Contexto;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace MSRecursosHumanos.Repo.Implementacion
{
    public class EmployeeRepo : BaseRepo, IEmployeeRepo
    {

        
        public EmployeeRepo(AdventureWorks2016Context ctx) : base(ctx) { }
        public CollectionResponse<EmployeeDTO> ListarEmpleadosActivos(int pagina, int nroregistros)
        {
            try
            {

                int count = (from e in ctx.Employees
                             where e.CurrentFlag.HasValue && e.CurrentFlag.Value
                             select e).Count();

                List<EmployeeDTO> result = (from e in ctx.Employees
                                            where e.CurrentFlag.HasValue && e.CurrentFlag.Value
                                            select e)
                                                .OrderByDescending(e => e.BusinessEntityId)
                                                .Skip((pagina - 1) * nroregistros)
                                                .Take(nroregistros)
                                                .Select(e => new EmployeeDTO
                                                {
                                                    IdEntidad = e.BusinessEntityId,
                                                    Cargo = e.JobTitle,
                                                    LoginId = e.LoginId,
                                                    Nombre = $"{e.BusinessEntity.FirstName} {e.BusinessEntity.LastName}",
                                                }).ToList();
                return new CollectionResponse<EmployeeDTO>
                {
                    Data = result,
                    Page = pagina,
                    Count = count,
                    First = ""
                };

            }
            catch 
            {
                throw;
            }
        }


        
        public TokenDTO Login(LoginDTO login)
        {
            try
            {
                var guid = Guid.NewGuid().ToString("N");
                if (login.Usuario == "admin" && login.Contrasenha == "12345") 
                {
                    //traer usuario de bd
                    PayloadDTO payload = new PayloadDTO { 
                        Usuario = login.Usuario,
                        IdUsuario = 1,
                        Servicios = new List<int> { 1,2,3}
                    };

                    var token = UtilitarioJWT.GenerarAccessToken(payload, guid);
                    return token;
                }


                throw new Exception("Usuario no encontrado");

                
                
            }
            catch
            {
                throw;
            }
        }

        public string SubirArchivo(ArchivoS3 archivo)
        {
            try
            {
                UtilitariosS3 s3 = new UtilitariosS3();
                s3.Subir(archivo);
                return "ok";
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
