using Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSRecursosHumanos.Repo.Implementacion
{
    public class BaseRepo
    {
        protected readonly AdventureWorks2016Context ctx;

        public BaseRepo(AdventureWorks2016Context ctx)
        {
            this.ctx = ctx;
        }
    }
}
