using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades.Utilitarios
{
    public class CollectionResponse<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int Count { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public string MySelf { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
    }
}
