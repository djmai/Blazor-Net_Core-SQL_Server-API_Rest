using ModeloClasesAlumnos;

namespace APIAlumnos.Repositorio
{
	public interface IRepositorioUsuarios
	{
		Task<UsuarioLogin> AltaUsuario(UsuarioLogin Alumno);

		Task<UsuarioLogin> DameUsuario(int id);

		Task<UsuarioLogin> DameUsuario(string email);
	}
}
