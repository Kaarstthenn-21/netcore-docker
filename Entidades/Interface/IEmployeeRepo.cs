using Entidades.DTO;
using Entidades.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades.Interface
{
    public interface IEmployeeRepo
    {
        CollectionResponse<EmployeeDTO> ListarEmpleadosActivos(int pagina, int nroregistros);
        string SubirArchivo(ArchivoS3 archivo);
        TokenDTO Login(LoginDTO login);
    }
}
