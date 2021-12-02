using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades.DTO
{
    public class PayloadDTO
    {
        public long IdUsuario { get; set; }
        public string Usuario { get; set; }       
        
        public List<int> Servicios { get; set; }
    }
}
