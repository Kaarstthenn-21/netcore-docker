using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades.ErrorApi
{
    public class ErrorResponse
    {
        public int IdError { get; set; }
        public string Mensaje { get; set; }
        public string Traza { get; set; }
    }
}
