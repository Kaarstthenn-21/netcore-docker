using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Entidades.Utilitarios.ArchivoS3;

namespace Entidades.Utilitarios
{
    public class ArchivoS3
    {
        public enum Accesible
        {
            PRIVADO,
            LECTURA_PUBLICA,
            LECTURA_ESCRITURA_PUBLICA
        }
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }

        public string Nombre { get; set; }
        public string Base64 { get; set; }
        public string Bucket { get; set; }
        public Accesible Accesibilidad { get; set; } = Accesible.PRIVADO;
    }
    public class UtilitariosS3
    {
        
        private S3CannedACL GetAccesibilidad(Accesible accesible)
        {
            switch (accesible)
            {
                case Accesible.LECTURA_PUBLICA:
                    return S3CannedACL.PublicRead;
                case Accesible.LECTURA_ESCRITURA_PUBLICA:
                    return S3CannedACL.PublicReadWrite;
                case Accesible.PRIVADO:
                default:
                    return S3CannedACL.NoACL;
            }
        }


        public void Subir(ArchivoS3 archivo)
        {            

            var bytes = Convert.FromBase64String(archivo.Base64);
            var contents = new MemoryStream(bytes);

            using (var amazonClient = new AmazonS3Client(
                archivo.AccessKey,
                archivo.SecretKey,
                RegionEndpoint.USWest2))
            {
                var createFileRequest = new PutObjectRequest
                {
                    CannedACL = GetAccesibilidad(archivo.Accesibilidad)
                };
                createFileRequest.Key = archivo.Nombre;
                createFileRequest.BucketName = archivo.Bucket;
                createFileRequest.InputStream = contents;
                amazonClient.PutObjectAsync(createFileRequest);
            }
        }


        public void Borrar(ArchivoS3 archivo)
        {
            

            using (var amazonClient = new AmazonS3Client(
                archivo.AccessKey,
                archivo.SecretKey,
                RegionEndpoint.USWest2))
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = archivo.Bucket,
                    Key = archivo.Nombre
                };
                amazonClient.DeleteObjectAsync(deleteObjectRequest);
            }
        }

        public ArchivoS3 Bajar(ArchivoS3 archivo)
        {
            

            using (var amazonClient = new AmazonS3Client(
                archivo.AccessKey,
                archivo.SecretKey,
                RegionEndpoint.USWest2
            ))
            {
                var getFileRequest = new GetObjectRequest
                {
                    BucketName = archivo.Bucket,
                    Key = archivo.Nombre
                };
                var fileResponse = amazonClient.GetObjectAsync(getFileRequest).Result;
                var streamResponse = fileResponse.ResponseStream;
                using (var ms = new MemoryStream())
                {
                    streamResponse.CopyTo(ms);
                    var byteResponse = ms.ToArray();
                    var stringResponse = Convert.ToBase64String(byteResponse);
                    archivo.Base64 = stringResponse;
                }
            }

            return archivo;
        }

    }
}
