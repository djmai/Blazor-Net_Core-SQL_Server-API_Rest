using BlazorServer.Pages;
using ModeloClasesAlumnos;
using System.Net.Http.Headers;

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
            UsuarioAPI? u = await response.Content.ReadFromJsonAsync<UsuarioAPI>();

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
		
        public async Task<UsuarioLogin> CrearUsuario(UsuarioLogin usuarioLogin)
		{
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var response = await _httpClient.PostAsJsonAsync<UsuarioLogin?>("api/Login/CrearUsuario", usuarioLogin);
			UsuarioLogin? usuario = await response.Content.ReadFromJsonAsync<UsuarioLogin>();

			if (usuario.error != null && usuario.error.mensaje != String.Empty)
			{
				if (usuario.error.mostrarUsuario)
				{
					_log.LogError("Error creando nuestro usuario: " + usuario.error.mensaje);
					throw new Exception(usuario.error.mensaje);
				}
				else
				{
					_log.LogError("Error obteniendo usuario: " + usuario.error.mensaje);
					throw new Exception("Error obteniendo usuario");
				}
			}

            return usuario;
		}

        public async Task<UsuarioLogin> ValidarUsuario(UsuarioLogin usuarioLogin)
        {
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync<UsuarioLogin?>("api/Login/ValidarUsuario", usuarioLogin);
            UsuarioLogin? usuario = await response.Content.ReadFromJsonAsync<UsuarioLogin>();

            if (usuario.error != null && usuario.error.mensaje != String.Empty)
            {
                if (usuario.error.mostrarUsuario)
                {
                    _log.LogError("Error validando usuario: " + usuario.error.mensaje);
                    throw new Exception(usuario.error.mensaje);
                }
                else
                {
                    _log.LogError("Error validando usuario: " + usuario.error.mensaje);
                    throw new Exception("Error obteniendo usuario");
                }
            }

            return usuario;
        }
    }
}
