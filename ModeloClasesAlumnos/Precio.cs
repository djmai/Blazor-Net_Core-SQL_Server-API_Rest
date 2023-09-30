using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloClasesAlumnos
{
	public class Precio
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "* el campo {0} es obligatorio")]
		public double Coste { get; set; }

		[Required(ErrorMessage = "* el campo {0} es obligatorio")]
		public DateTime FechaInicio { get; set; }

		[Required(ErrorMessage = "* el campo {0} es obligatorio")]
		public DateTime FechaTermino { get; set; }
	}
}
