using ModeloClasesAlumnos;

namespace BlazorServer.Servicios
{
    public interface IServicioLogin
    {
        Task<UsuarioAPI> SolicitudLogin(Login l);
    }
}
