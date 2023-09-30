using ModeloClasesAlumnos;

namespace BlazorServer.Servicios
{
	public interface IServicioCursos
	{
		Task<IEnumerable<Curso?>?> DameCursos(int idAlumno);

		Task<Curso> AltaCurso(Curso curso);

		Task<Curso> DameCurso(int id, int idPrecio);

		Task<Curso> ModificarCurso(int id, Curso curso);

		Task BorrarCurso(int id);

    }
}
