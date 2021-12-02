using Entidades.Interface;
using Infraestructura.Contexto;
using MSRecursosHumanos.Repo.Implementacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSRecursosHumanos.Repo
{
    public class EmployeeUnit
    {
        private readonly AdventureWorks2016Context ctx;
        private IEmployeeRepo employeeRepository;
        public EmployeeUnit(AdventureWorks2016Context ctx)
        {
            this.ctx = ctx;
        }
        public IEmployeeRepo EmployeeRepository
        {
            get
            {
                return employeeRepository = employeeRepository ?? new EmployeeRepo(ctx);
            }
        }
    }
}
