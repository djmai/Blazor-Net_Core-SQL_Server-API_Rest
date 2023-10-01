using APIAlumnos.Repositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModeloClasesAlumnos;
using NLog.Fluent;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace APIAlumnos.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AlumnosController : ControllerBase
	{
		private readonly IRepositorioAlumnos alumnosRepositorio;
        private readonly ILogger<AlumnosController> _log;

        public AlumnosController(IRepositorioAlumnos alumnosRepositorio, ILogger<AlumnosController> logger)
		{
			this.alumnosRepositorio = alumnosRepositorio;
            _log = logger;
            _log.LogDebug(1, "NLog injected into AlumnosController");
        }

		[HttpGet("{idPagina}/{numRegistros}")]
		[Produces("application/json")]
		[SwaggerOperation("Listado de alumnos")]
		[SwaggerResponse(200, "Retorna lista de alumnos", typeof(Alumno[]))]
		[SwaggerResponse(400, "Error al listar alumnos", typeof(Dictionary<string, string>))]
		/*public async Task<ActionResult> DameAlumnos()
        {
			var responseApi = new ResponseAPI<IEnumerable<Alumno>>();
            try
            {
				responseApi.EsCorrecto = true;
				responseApi.Data = await alumnosRepositorio.DameAlumnos();
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
				responseApi.Mensaje = ex.Message;
            }

			return Ok(responseApi);
        }*/
		public async Task<ActionResult> DameAlumnos(int idPagina, int numRegistros)
		{
			try
			{
				return Ok(await alumnosRepositorio.DameAlumnos(idPagina, numRegistros));
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los datos: " + ex.Message);
			}
		}

        /// <summary>
        /// Consultar un alumno por id
        /// </summary>
        [HttpGet("{id:int}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(Alumno), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
		public async Task<ActionResult> DameAlumno(int id)
		{
			try
			{
				var res = await alumnosRepositorio.DameAlumno(id);
				if (res == null)
					return NotFound();

				return Ok(res);
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los datos: " + ex.Message);
			}
		}

		/// <summary>
		/// Consultar un alumno por email
		/// </summary>
		/// <example>cool-service-account@my-cool-org.com</example>
		[HttpGet("{email}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(Alumno), StatusCodes.Status200OK)]
		//		[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
		[SwaggerResponse(404, "El alumno tiene datos invalidos")]
		//[SwaggerResponseAttribute(404, "El alumno tiene datos invalidos", typeof(Dictionary<string, string>), "application/json")]
		public async Task<ActionResult<Alumno>> DameAlumno(string email)
		{
			try
			{
				var res = await alumnosRepositorio.DameAlumno(email);
				if (res == null)
					return NotFound();
				
				return Ok(res);
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los datos: " + ex.Message);
			}
		}

		[HttpPost]
		[Consumes("application/json")]
		[Produces("application/json")]
		// [Authorize]
		[SwaggerOperation(
			Summary = "Agrega un nuevo alumno"
		// Description = "Requires admin privileges",
		// OperationId = "CreateProduct",
		// Tags = new[] { "Purchase", "Products" }
		)]
		[SwaggerResponse(201, "El alumno ha sido creado", typeof(Alumno))]
		[SwaggerResponse(400, "El alumno tiene datos invalidos")]
		public async Task<ActionResult<Alumno>> CrearAlumno(Alumno alumno)
		{
			try
			{
				if (alumno == null)
					return BadRequest();

				var alumnoAux = await alumnosRepositorio.DameAlumno(alumno.Email!);
				if(alumnoAux != null)
				{
					ModelState.AddModelError("email", "El email ya esta en uso");
					return BadRequest(ModelState);
				}

				var nuevoAlumno = await alumnosRepositorio.AltaAlumno(alumno);

				return nuevoAlumno;
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error en el alta del nuevo alumno: " + ex.Message);
			}
		}

		[HttpPut("{id:int}")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Actualizar un alumno"
		// ,Description = "Requires admin privileges"
			,OperationId = "CreateProduct"
		// ,Tags = new[] { "Purchase", "Products" }
		)]
		public async Task<ActionResult<Alumno>> ModificarAlumno(int id, Alumno alumno)
		{
			try
			{
				if (id != alumno.Id)
					return BadRequest("Alumno Id no coincide");

				var alumnoModificar = await alumnosRepositorio.DameAlumno(id);

				if (alumnoModificar == null)
					return NotFound($"Alumno con {id} no encontrado");

				Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(alumno));

                return await alumnosRepositorio.ModificarAlumno(alumno);
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error actualizando datos: " + ex.Message);
			}
		}

		[HttpDelete("{id:int}")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Eliminar un alumno"
			// ,Description = "Requires admin privileges"
			// , OperationId = "CreateProduct"
			// ,Tags = new[] { "Purchase", "Products" }
		)]
		public async Task<ActionResult<Alumno>> BorrarAlumno(int id)
		{
			try
			{
				var alumnoBorrar = await alumnosRepositorio.DameAlumno(id);

				if (alumnoBorrar == null)
					return NotFound($"Alumno con id= {id} no encontrado");

				return await alumnosRepositorio.BorrarAlumno(id);
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al borrar el alumno: " + ex.Message);
			}

		}

		[HttpGet]
		[Route("BuscarAlumnos")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Buscar alumnos por Nombre o Email"
		// ,Description = "Requires admin privileges"
		// , OperationId = "CreateProduct"
		// ,Tags = new[] { "Purchase", "Products" }
		)]
		public async Task<ActionResult> BuscarAlumnos(string texto)
		{
			try
			{
				return Ok(await alumnosRepositorio.BuscarAlumnos(texto));
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los datos");
			}
		}

		[HttpPost]
		[Route("InscribirAlumno/{idCurso}/{idPrecio}")]
		[SwaggerOperation(
			Summary = "Inscribir alumno a cursos"
		// ,Description = "Requires admin privileges"
		// , OperationId = "CreateProduct"
		// ,Tags = new[] { "Purchase", "Products" }
		)]
		public async Task<ActionResult<Alumno>> InscribirAlumnoCurso([FromBody] Alumno alumno, int idCurso, int idPrecio)
		{
			try
			{
				var alumnoValidar = await alumnosRepositorio.DameAlumno(alumno.Id);

				if (alumnoValidar == null)
					return NotFound($"Alumno no encontrado");

				return await alumnosRepositorio.inscribirAlumnoCurso(alumno, idCurso, idPrecio);
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error inscribiendo alumno en curso: " + ex.Message);
			}
		}

		[HttpPost]
		[Route("CursosAlumno/{idAlumno}")]
		[SwaggerOperation(
			Summary = "Buscar alumnos y sus cursos"
		// ,Description = "Requires admin privileges"
		// , OperationId = "CreateProduct"
		// ,Tags = new[] { "Purchase", "Products" }
		)]
		public async Task<ActionResult<Alumno>> AlumnoCursos(int idAlumno)
		{
			try
			{
				Alumno alumnoValidar = await alumnosRepositorio.DameAlumno(idAlumno);

				if (alumnoValidar == null)
					return NotFound($"Alumno no encontrado");

				//return await alumnosRepositorio.AlumnoCursos(idAlumno);

                return await alumnosRepositorio.AlumnoCursos(idAlumno) ?? alumnoValidar;
            }
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo cursos alumno: " + ex.Message);
			}
		}

	}
}
