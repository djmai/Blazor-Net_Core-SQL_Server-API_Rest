using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloClasesAlumnos
{
    public class ResponseAPI<T>
    {
        public bool EsCorrecto { get; set; }

        public T? Data { get; set; }

        public string? Mensaje { get; set; }
    }
}
