using BlazorServer.Pages;
using ModeloClasesAlumnos;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BlazorServer.Servicios
{
    public class ServicioCursos : IServicioCursos
	{
		private readonly HttpClient _httpClient;
        private readonly ILogger<ServicioCursos> _log;

        public ServicioCursos(HttpClient httpClient, ILogger<ServicioCursos> logger)
		{
			_httpClient = httpClient;
			_log = logger;
		}

		public async Task<IEnumerable<Curso?>?> DameCursos(int idAlumno)
		{
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			//List<Alumno> alu = await _httpClient.GetFromJsonAsync<List<Alumno?>?>("api/Alumnos");

            var response = await _httpClient.PostAsJsonAsync<Curso[]?>("api/AlumnosCursos/" + idAlumno.ToString(), null);
            Curso[] c = await response.Content.ReadFromJsonAsync<Curso[]>();

            if (c != null && c[0].error != null && c[0].error.mensaje != String.Empty)
            {
                if (c[0].error.mostrarUsuario)
                {
                    _log.LogError("Error obteniendo alumnos: " + c[0].error.mensaje);
                    throw new Exception(c[0].error.mensaje);
                }
                else
                {
                    _log.LogError("Error obteniendo alumnos: " + c[0].error.mensaje);
                    throw new Exception("Error obteniendo alumnos");
                }
            }

            return c;

            //var response = await _httpClient.PostAsJsonAsync<Curso[]?>("api/AlumnosCursos/" + idAlumno.ToString(), null);
            //return await response.Content.ReadFromJsonAsync<Curso[]>();
        }
		public async Task<Curso?> AltaCurso(Curso curso)
		{
			//var response = await _httpClient.PostAsJsonAsync<Curso>("/api/Cursos/", curso);
			//return await response.Content.ReadFromJsonAsync<Curso>();

			var response = await _httpClient.PostAsJsonAsync("/api/Cursos/", curso);
			Curso c = await response.Content.ReadFromJsonAsync<Curso>();

			if(c.error != null && c.error.mensaje != String.Empty){
				if(c.error.mostrarUsuario)
				{
					_log.LogError("Error dando de alta el nuevo curso: " + c.error.mensaje);
					throw new Exception(c.error.mensaje);
				}
				else
				{
					_log.LogError("Error dando de alta nuestro curso: " + c.error.mensaje);
					throw new Exception("Error dando de alta nuestro curso");
				}
			}

			return c;
        }

		public async Task<Curso> DameCurso(int idCurso, int idPrecio)
		{
			return await _httpClient.GetFromJsonAsync<Curso>($"/api/Cursos/{idCurso!.ToString()}/{idPrecio!.ToString()}");
		}

		public async Task<Curso> ModificarCurso(int id, Curso curso)
		{
			var res = await _httpClient.PutAsJsonAsync("/api/Cursos/" + id.ToString(), curso);
			return await res.Content.ReadFromJsonAsync<Curso>();
		}

        public async Task BorrarCurso(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/Cursos/{id}");
			if (!response.IsSuccessStatusCode)
			{
				if(response.StatusCode == HttpStatusCode.SeeOther)
				{
					string error = await response.Content.ReadAsStringAsync();
					_log.LogError("Error borrando nuestro curso: " + error);
					throw new Exception(error);
				}
				else
				{
					_log.LogError("Error borrando nuestro curso: " + response.ReasonPhrase);
					throw new Exception("Se produjo un error borrando el curso");
				}
			}
        }
    }
}
