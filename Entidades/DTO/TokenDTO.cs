using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades.DTO
{
    public class TokenDTO
    {

        public int StateCode { get; set; }
        public DateTime RequestAt { get; set; }
        public int ExpiresIn { get; set; }
        public string AccessToken { get; set; }
    }
}
