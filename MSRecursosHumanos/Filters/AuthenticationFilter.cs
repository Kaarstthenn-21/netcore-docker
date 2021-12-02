using Entidades.DTO;
using Entidades.ErrorApi;
using Entidades.Utilitarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSRecursosHumanos.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public int IdServicio { get; set; }


        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            try
            {
                if (!actionContext.HttpContext.Request.Headers.ContainsKey("Authorization"))
                    throw new Exception("HEADER REQUERIDO");

                string authenticationKey = actionContext.HttpContext.Request.Headers["Authorization"];
                var token = authenticationKey.Split(' ');
                if (token.Length < 2) throw new Exception("HEADER INVALIDO");

                PayloadDTO userInfo;
                    var authenticationToken = token[1];                    
                    userInfo = UtilitarioJWT.GetInfoToken(authenticationToken);
                    if (userInfo == null) throw new Exception("ERROR PROCESAR TOKEN");
                

                if (IdServicio != 0 && !userInfo.Servicios.Exists(i => i == IdServicio))
                    throw new Exception("NO AUTORIZADO");

                actionContext.HttpContext.Items.Add("userInfo", userInfo);
                
            }
            catch (SecurityTokenExpiredException ex)
            {
                actionContext.Result = new UnauthorizedObjectResult(
                    new ErrorResponse
                    {
                        IdError = 40150,
                        Mensaje = "Token Expirado",
                        Traza = ex.StackTrace
                    });
            }
            catch (Exception ex)
            {                
                actionContext.Result = new UnauthorizedObjectResult(
                    new ErrorResponse
                    {
                        IdError = 40150,
                        Mensaje = ex.Message,
                        Traza = ex.StackTrace
                    });
            }
        }
    }
}
