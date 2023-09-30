using BlazorServer.Pages;
using ModeloClasesAlumnos;

namespace BlazorServer.Servicios
{
    public class ServicioLogin : IServicioLogin
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServicioLogin> _log;

        public ServicioLogin(HttpClient httpClient, ILogger<ServicioLogin> logger)
        {
            _httpClient = httpClient;
            _log = logger;
        }

        public async Task<UsuarioAPI> SolicitudLogin(Login l)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Login", l);
            UsuarioAPI u = await response.Content.ReadFromJsonAsync<UsuarioAPI>();

            if(u.error != null && u.error.mensaje != String.Empty)
            {
                if (u.error.mostrarUsuario)
                {
                    _log.LogError("Error al loguearse: " + u.error.mensaje);
                    throw new Exception(u.error.mensaje);
                }
                else
                {
                    _log.LogError("Error al loguearse" + u.error.mensaje);
                    throw new Exception("Error dando de alta nuestro curso");
                }
            }

            return u;
        }
    }
}
