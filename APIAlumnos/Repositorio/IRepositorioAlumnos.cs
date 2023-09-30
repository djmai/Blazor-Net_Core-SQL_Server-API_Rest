using ModeloClasesAlumnos;

namespace APIAlumnos.Repositorio
{
	public interface IRepositorioAlumnos
	{
		Task<Alumno> AltaAlumno(Alumno Alumno);

		Task<IEnumerable<Alumno>> DameAlumnos();

		Task<Alumno> DameAlumno(int id);

		Task<Alumno> DameAlumno(string email);

		Task<Alumno> ModificarAlumno(Alumno Alumno);

		Task<Alumno> BorrarAlumno(int id);

		Task<IEnumerable<Alumno>> BuscarAlumnos(string texto);

		//Inscribir Alumnos en curso
		Task<Alumno?> inscribirAlumnoCurso(Alumno Alumno, int idCurso, int idPrecio);

		//Devuelve los datos de un alumno y todos sus cursos
		Task<Alumno> AlumnoCursos(int idAlumno);
	}
}
