using Entidades.DTO;
using Entidades.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSRecursosHumanos.Repo.Services
{
    public class EmployeeService
    {
        private readonly EmployeeUnit unit;
        public EmployeeService(EmployeeUnit unit)
        {
            this.unit = unit;
        }

        public CollectionResponse<EmployeeDTO> ListarEmpleadosActivos(int pagina, int nroregistros)
        {
            try
            {
                return this.unit.EmployeeRepository.ListarEmpleadosActivos(pagina, nroregistros);
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
                return this.unit.EmployeeRepository.SubirArchivo(archivo);
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
                return this.unit.EmployeeRepository.Login(login);
            }
            catch
            {
                throw;
            }
        }
    }
}
