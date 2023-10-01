using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloClasesAlumnos
{
	public class Alumno
	{
		public int Id { get; set; }
		
		[Required(ErrorMessage = "* El campo {0} es obligatorio")]
		public string? Nombre { get; set; }

		[Required(ErrorMessage = "* El campo {0} es obligatorio")]
		[EmailAddress(ErrorMessage = "* Formato de {0} incorrecto")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "* El campo {0} es obligatorio")]
		public string? Foto { get; set; }

		public List<Curso>? Listacursos { get; set; }

		[Required(ErrorMessage = "* El campo {0} es obligatorio")]
		public DateTime FechaAlta { get; set; }

		public DateTime? FechaBaja { get; set; }

		public Error? error { get; set; }

		public Paginacion paginacion { get; set; }
	}
}
