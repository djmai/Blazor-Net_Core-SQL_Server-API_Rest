using BlazorServer.Pages;
using ModeloClasesAlumnos;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorServer.Servicios
{
	public class ServicioAlumnos : IServicioAlumnos
	{
		private readonly HttpClient _httpClient;
        private readonly ILogger<ServicioAlumnos> _log;

        public ServicioAlumnos(HttpClient httpClient, ILogger<ServicioAlumnos> logger)
		{
			_httpClient = httpClient;
			_log = logger;
		}

		public async Task<IEnumerable<Alumno?>?> DameAlumnos(int idPagina, int numRegistros)
		{
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            List<Alumno> alu = await _httpClient.GetFromJsonAsync<List<Alumno?>?>($"api/Alumnos/{idPagina.ToString()}/{numRegistros.ToString()}");

            if(alu != null && alu[0].error != null && alu[0].error.mensaje != String.Empty)
            {
                if (alu[0].error.mostrarUsuario)
                {
                    _log.LogError("Error obteniendo alumnos: " + alu[0].error.mensaje);
                    throw new Exception(alu[0].error.mensaje);
                }
                else
                {
                    _log.LogError("Error obteniendo alumnos: " + alu[0].error.mensaje);
                    throw new Exception("Error obteniendo alumnos");
                }
            }

            return alu;

            /*var res = await _httpClient.GetFromJsonAsync<ResponseAPI<Alumno[]>>("api/Alumnos");

            if (!res.EsCorrecto)
                throw new Exception(res.Mensaje);
            
			return res.Data;*/

        }

        public async Task<Alumno?> DameAlumno(int id)
        {
            return await _httpClient.GetFromJsonAsync<Alumno>("api/Alumnos/" + id.ToString());
        }

		public async Task<Alumno?> CrearAlumno(Alumno alumno)
		{
			// var response = await _httpClient.PostAsJsonAsync("api/Alumnos", alumno);
			// return await response.Content.ReadFromJsonAsync<Alumno>();

            var response = await _httpClient.PostAsJsonAsync("api/Alumnos", alumno);
            Alumno a = await response.Content.ReadFromJsonAsync<Alumno>();

            if (a.error != null && a.error.mensaje != String.Empty)
            {
                if (a.error.mostrarUsuario)
                {
                    _log.LogError("Error dando de alta el nuevo curso: " + a.error.mensaje);
                    throw new Exception(a.error.mensaje);
                }
                else
                {
                    _log.LogError("Error dando de alta nuestro curso: " + a.error.mensaje);
                    throw new Exception("Error dando de alta nuestro curso");
                }
            }

            return a;
        }

		public async Task<Alumno?> ModificarAlumno(int id, Alumno alumno)
		{
            var res = await _httpClient.PutAsJsonAsync("api/Alumnos/"+id, alumno);
            return await res.Content.ReadFromJsonAsync<Alumno>();
        }

		public async Task EliminarAlumno(int id)
		{
			//await _httpClient.DeleteAsync($"api/Alumnos/{id}");
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/Alumnos/{id}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.SeeOther)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    _log.LogError("Error borrando nuestro alumno: " + error);
                    throw new Exception(error);
                }
                else
                {
                    _log.LogError("Error borrando nuestro curso: " + response.ReasonPhrase);
                    throw new Exception("Se produjo un error borrando el alumno");
                }
            }
        }

		public async Task<Alumno?> CursosInscritorAlumno(int id)
		{
            /*var response = await _httpClient.PostAsJsonAsync<Alumno>("api/Alumnos/CursosAlumno/" + id.ToString());
			return await response.Content.ReadFromJsonAsync<Alumno>();*/

            var response = await _httpClient.PostAsJsonAsync("api/Alumnos/CursosAlumno/" + id.ToString(), id.ToString());

            return await response.Content.ReadFromJsonAsync<Alumno>();
        }

		public async Task<Alumno?> CursosInscribirAlumno(Alumno alumno, int idCurso, int idPrecio)
		{
			var response = await _httpClient.PostAsJsonAsync("/api/Alumnos/InscribirAlumno/" + idCurso.ToString() + "/" + idPrecio.ToString(), alumno);
			return await response.Content.ReadFromJsonAsync<Alumno>();
		}

	}
}