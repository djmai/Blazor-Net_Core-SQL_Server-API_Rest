using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloClasesAlumnos
{
	public class Curso
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "* el campo {0} es obligatorio")]
		public string? NombreCurso { get; set; }

		public List<Precio>? ListaPrecios { get; set; }

        public Error? error { get; set; }
    }
}
