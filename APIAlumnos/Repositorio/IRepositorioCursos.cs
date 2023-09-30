using ModeloClasesAlumnos;

namespace APIAlumnos.Repositorio
{
	public interface IRepositorioCursos
	{

		// Método para obtener todos los cursos
		// Task<IEnumerable<Curso>> DameCursos();

		Task<IEnumerable<Curso>> DameCursos(int idAlumno);

		// Método para obtener un curso por su ID
		Task<Curso> DameCurso(int id);

		// Devolvera los datos de un curso con un precio buscado por id
		Task<Curso> DameCurso(int id, int idPrecio);

		// Método para obtener los datos de un curso con sus precios buscando por nombre
		Task<Curso> DameCurso(string nombreCurso);

		// Método para dar de alta un nuevo curso
		Task<Curso> AltaCurso(Curso curso);

		// Método para modificar un curso existente
		Task<Curso> ModificarCurso(Curso curso);

		// Método para borrar un curso por su ID
		Task<Curso> BorrarCurso(int id);

	}
}
