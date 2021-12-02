using Entidades.DTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Entidades.Utilitarios
{
    public class UtilitarioJWT
    {
        internal static readonly string tokenAcceso = "5m4r7R3450n54cc350";

        internal static readonly string SelfSigned2048_SHA256 =
            @"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";

        internal static readonly string SelfSigned2048_SHA256_Public =
            @"MIIDgzCCAmugAwIBAgIEQhZZATANBgkqhkiG9w0BAQsFADByMQswCQYDVQQGEwJQRTERMA8GA1UECBMIQXJlcXVpcGExETAPBgNVBAcTCEFyZXF1aXBhMRYwFAYDVQQKEw1TbWFydCBSZWFzb25zMQswCQYDVQQLEwJUSTEYMBYGA1UEAxMPUm95IFBlcmV6IFBpbnRvMB4XDTE4MDEyMDA1NTQyNloXDTE5MDEyMDA1NTQyNlowcjELMAkGA1UEBhMCUEUxETAPBgNVBAgTCEFyZXF1aXBhMREwDwYDVQQHEwhBcmVxdWlwYTEWMBQGA1UEChMNU21hcnQgUmVhc29uczELMAkGA1UECxMCVEkxGDAWBgNVBAMTD1JveSBQZXJleiBQaW50bzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAIQsKmVDf9dMipHbELq6cXQOaYiuvWLmDW8nFrptFP2DTnsndsBCeyfhlYowJSAwFcKoxWUH0kyvPGU5gUOKfeBfzdZ6bGJpFqrb+DQsQho+aROC5PqUzZNIawhGf/9aKUwbpGrjOD7itL2mtABrOaBHA+YEJnyHdwvAAM8edYCzuxKlwsEutEIw2vRwrFYWHCi2nDxYAsPU22Y8LDYhfQAZ3e1Su5T2JTtbf3r+ZyOr/WWzCQOUiUzIpD6XuNDfcOdj1nwt5sA33L/UOYQA76vr6MfCsXCVsx8SLn/GCjT+Owm9SXY2aOOE44g7eNuUIOVbmATgI59G6RWxThiVPTcCAwEAAaMhMB8wHQYDVR0OBBYEFH5OLWa/piDHphWoUqCNPSmiycD9MA0GCSqGSIb3DQEBCwUAA4IBAQBuzX24FbpiKq3icjM/3Lld3PLFWHqOzBJLF5EU0XznJQk40V2IZFp4ffxuojJiNNnn3gE8fx/VsdiWA5u47242QZ3F2UJPCBorj+an+Y1XPKM8AgBQLCfEUhEUX6xP0XYFF7PuIUYLfx+a53Gu1lBGIZ1ikYXty+msTcBwLGkXOF8Wf/9Y69oF4Wx1Xabx8ZxZhfWgXxfAC5euslqUfswPDshiST0y8OOT0S/oPmFxi7jGY9oPzuIFnM+IOBdhUP5Li52IlDxEWNUXv4xd+BkrZxIB2LNo12s2L+bsiE/02nwOn9EIFhrpQJvtdrLFJvzuHxlgoN9vJUMIzQBfbDjh";


        public static T Deserialize<T>(string json)
        {
            var obj = Activator.CreateInstance<T>();
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
        public static PayloadDTO GetInfoToken(string authenticationToken)
        {
            try
            {
                var CertSelfSigned2048_SHA256_Public = new X509Certificate2(Convert.FromBase64String(SelfSigned2048_SHA256_Public), tokenAcceso);
                var validateKey = new X509SecurityKey(CertSelfSigned2048_SHA256_Public);
                var handler = new JwtSecurityTokenHandler();
                var validationParameters =
                    new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true
                    };
                validationParameters.IssuerSigningKey = validateKey;
                validationParameters.RequireSignedTokens = true;


                var token = handler.ReadJwtToken(authenticationToken);
                var valuesToken = token.Payload.Values;
                var userInfo = Deserialize<PayloadDTO>(valuesToken.ElementAt(4).ToString());
                SecurityToken validatedSecurityToken = null;

                var cp = handler.ValidateToken(authenticationToken, validationParameters,
                    out validatedSecurityToken);
                var ticksExpira = 0;
                if (!int.TryParse(valuesToken.ElementAt(1).ToString(), out ticksExpira))
                    throw new SecurityTokenExpiredException();
                //long current = DateTimeOffset.UtcNow.UtcDateTime.Ticks;

                var expire = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                expire = expire.AddSeconds(ticksExpira).ToLocalTime();
                if (DateTime.Compare(expire, DateTimeOffset.UtcNow.UtcDateTime) == -1)
                    throw new SecurityTokenExpiredException();
                


                return userInfo;
            }
            catch (SecurityTokenExpiredException ex)
            {
                throw ex;
            }
            catch 
            {
                
                return null;
            }
        }

        public static TokenDTO GenerarAccessToken(PayloadDTO payload, string guid)
        {
            try
            {
                var certSelfSigned2048Sha256 = new X509Certificate2(Convert.FromBase64String(SelfSigned2048_SHA256),
                                                                        tokenAcceso, X509KeyStorageFlags.MachineKeySet);
                var signingKey = new X509SecurityKey(certSelfSigned2048Sha256);
                var tiempoNoAntesMin = 2; //minutos
                var current = DateTimeOffset.UtcNow.UtcDateTime;


                var tiempoExpiraSec = 24 * 30 * 60; //expiracion en un mes
                var expiry = current.AddMinutes(tiempoExpiraSec);
                var notBefore = current.AddMinutes(tiempoNoAntesMin);


                var payloadResult = new JwtPayload("msseguridad", guid, new List<Claim>(), notBefore, expiry)
                {
                    {"scopes", payload}
                };

                var handler = new JwtSecurityTokenHandler();
                // make sure we can still validate with existing logic.
                var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);
                var header = new JwtHeader(signingCredentials);
                var secToken = new JwtSecurityToken(header, payloadResult);

                return new TokenDTO
                {
                    AccessToken = handler.WriteToken(secToken),
                    ExpiresIn = tiempoExpiraSec,
                    RequestAt = current,
                    StateCode = (int)HttpStatusCode.Created
                };
            }
            catch
            {
                throw;
            }
        }

    }
}
