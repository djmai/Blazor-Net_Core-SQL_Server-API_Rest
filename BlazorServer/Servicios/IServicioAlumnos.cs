using ModeloClasesAlumnos;

namespace BlazorServer.Servicios
{
	public interface IServicioAlumnos
	{
		Task<IEnumerable<Alumno?>> DameAlumnos();

        Task<Alumno?> DameAlumno(int id);

		Task<Alumno?> CrearAlumno(Alumno alumno);

        Task<Alumno?> ModificarAlumno(int id, Alumno alumno);

		Task EliminarAlumno(int id);

		Task<Alumno?> CursosInscritorAlumno(int id);

		Task<Alumno?> CursosInscribirAlumno(Alumno alumno, int idCurso, int idPrecio);
	}
}
