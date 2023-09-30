using APIAlumnos.Repositorio;
using Microsoft.AspNetCore.Mvc;
using ModeloClasesAlumnos;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace APIAlumnos.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CursosController : ControllerBase
	{
		private readonly IRepositorioCursos cursosRepositorio;
        private readonly ILogger<CursosController> _log;

        public CursosController(IRepositorioCursos cursosRepositorio, ILogger<CursosController> logger)
		{
			this.cursosRepositorio = cursosRepositorio;
            _log = logger;
            _log.LogDebug(1, "NLog injected into CursosController");
        }

		[HttpGet]
		[Produces("application/json")]
		[ProducesResponseType(typeof(Curso[]), StatusCodes.Status200OK)]
		[SwaggerOperation("Listado de cursos")]
		public async Task<ActionResult> DameCursos()
		{
			try
			{
				return Ok(await cursosRepositorio.DameCursos(-1));
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los datos: " + ex.Message);
			}
		}

		[HttpGet("{id:int}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(Curso), StatusCodes.Status200OK)]
		[SwaggerOperation("Consultar curso por id")]
		public async Task<ActionResult<Curso>> DameCurso(int id)
		{
			try
			{
				var curso = await cursosRepositorio.DameCurso(id);

				if (curso == null)
					return NotFound();

				return Ok(curso);
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los datos del curso: " + ex.Message);
			}
		}

		[HttpGet("{nombreCurso}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(Curso), StatusCodes.Status200OK)]
		[SwaggerOperation("Consultar curso por nombre")]
		public async Task<ActionResult<Curso>> DameCurso(string nombreCurso)
		{
			try
			{
				var curso = await cursosRepositorio.DameCurso(nombreCurso);

				if (curso == null)
					return NotFound();

				return Ok(curso);
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los datos del curso: " + ex.Message);
			}
		}

		[HttpPost]
		[Consumes("application/json")]
		[Produces("application/json")]
		[SwaggerOperation("Agregar un nuevo curso")]
		public async Task<ActionResult<Curso>> AltaCurso(Curso curso)
		{
			try
			{
				if (curso == null || curso.ListaPrecios == null)
					return BadRequest();

				var cursoAux = await cursosRepositorio.DameCurso(curso.NombreCurso!);

				if(cursoAux != null)
				{
					ModelState.AddModelError("NombreCurso", "El cuso ya esta dado de alta");
					return BadRequest(ModelState);
				}

				var nuevoCurso = await cursosRepositorio.AltaCurso(curso);
				return nuevoCurso;
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al dar del alta nuevo curso: " + ex.Message);
			}
		}

		[HttpPut("{id:int}")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[SwaggerOperation("Modificar curso por id")]
		public async Task<ActionResult<Curso>> ModificarCurso(int id, Curso curso)
		{
			try
			{
				if (id != curso.Id)
					return BadRequest("Curso Id no coincide");

				var cursoModificar= await cursosRepositorio.DameCurso(id);

                if (cursoModificar == null)
					return BadRequest($"Curso no id= {id} no encontrado");

                return await cursosRepositorio.ModificarCurso(curso);

			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error actualizando datos " + ex.Message);
			}

		}

		[HttpDelete("{id:int}")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[SwaggerOperation("Borrar curso por id")]
		public async Task<ActionResult<Curso>> BorrarCurso(int id)
		{
			try
			{
				var cursoBorrar = await cursosRepositorio.DameCurso(id);

				if (cursoBorrar == null)
					return NotFound($"Curso con id= {id} no encontrado");

				return await cursosRepositorio.BorrarCurso(id);
			}
			catch (SqlException ex) {
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status303SeeOther, ex.Message);
            }
			catch (Exception ex)
			{
				_log.LogDebug(1, ex.ToString());
				return StatusCode(StatusCodes.Status500InternalServerError, "Error al borrar el curso: " + ex.Message);
			}
		}

		[HttpGet("{id:int}/{idPrecio:int}")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[SwaggerOperation("Borrar curso por id")]
		public async Task<ActionResult<Curso>> DameCurso(int id, int idPrecio)
		{
			try
			{
				var resultado = await cursosRepositorio.DameCurso(id, idPrecio);
				if (resultado == null)
					return NotFound();

				return resultado;
			}catch(Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obtenido los datos: " + ex.Message);
			}
		}

		[HttpPost]
		[Route("~/api/AlumnosCursos/{idAlumno}")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Inscribir alumno a cursos"
			,Tags = new[] { "Alumnos" }
		)]
		public async Task<ActionResult> DameCursos(int idAlumno)
		{
			try
			{
				return Ok(await cursosRepositorio.DameCursos(idAlumno));
			}
			catch (Exception ex)
			{
                _log.LogDebug(1, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obtenido los datos: " + ex.Message);
			}
		}
	}
}
