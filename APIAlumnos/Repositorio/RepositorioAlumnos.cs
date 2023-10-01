using APIAlumnos.Datos;
using Microsoft.Data.SqlClient;
using ModeloClasesAlumnos;
using System.Data;

namespace APIAlumnos.Repositorio
{
    public class RepositorioAlumnos : IRepositorioAlumnos
    {
        private string CadenaConexion;

        public RepositorioAlumnos(AccesoDatos cadenaConexion)
        {
            CadenaConexion = cadenaConexion.CadenaConexionSQL;
        }

        private SqlConnection conexion()
        {
            return new SqlConnection(CadenaConexion);
        }

        public async Task<Alumno> AltaAlumno(Alumno alumno)
        {
            Alumno? alumnoCreado = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;

            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.UsuarioAltaAlumno";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@Nombre", SqlDbType.VarChar, 500).Value = alumno.Nombre;
                Comm.Parameters.Add("@Email", SqlDbType.VarChar, 500).Value = alumno.Email;
                Comm.Parameters.Add("@Foto", SqlDbType.VarChar, 500).Value = alumno.Foto;
                Comm.Parameters.Add("@FechaAlta", SqlDbType.DateTime).Value = alumno.FechaAlta;

                reader = await Comm.ExecuteReaderAsync();
                if (reader.Read())
                {
                    alumnoCreado = await DameAlumno(Convert.ToInt32(reader["idAlumno"]));

				}

			}
            catch (SqlException ex)
            {
				throw new Exception("Error cargando los datos de nuestro alumno " + ex.Message);
			}
			finally
			{
				if (reader != null)
					reader.Close();

				if (Comm != null)
					Comm.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return alumnoCreado;
		}

        public async Task<Alumno> BorrarAlumno(int id)
        {
            Alumno alumnoBorrado = null;
            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;
            SqlDataReader reader = null;
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.UsuarioMarcarBaja";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@IdAlumno", SqlDbType.Int).Value = id;
                reader = await Comm.ExecuteReaderAsync();
                if (reader.Read())
                    alumnoBorrado = await DameAlumno(Convert.ToInt32(reader["idAlumno"]));

            }
			finally
			{
				reader?.Close();
				Comm?.Dispose();
				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return alumnoBorrado;
		}

        public async Task<Alumno> DameAlumno(int id)
        {
            Alumno? alu = null;
            SqlConnection sqlConexion = conexion();
            SqlCommand? Comm = null;
            SqlDataReader? reader = null;
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.UsuarioDameAlumnos";
                Comm.CommandType = System.Data.CommandType.StoredProcedure;
                Comm.Parameters.Add("@id", SqlDbType.Int).Value = id;
                reader = await Comm.ExecuteReaderAsync();
                if (reader.Read())
                {
                    alu = new Alumno();
                    alu.Id = Convert.ToInt32(reader["Id"]);
                    alu.Nombre = reader["Nombre"].ToString()!;
                    alu.Email = reader["Email"].ToString()!;
                    alu.Foto = reader["Foto"].ToString()!;
                    alu.FechaAlta = Convert.ToDateTime(reader["FechaAlta"].ToString());

                    if (reader["FechaBaja"] != System.DBNull.Value)
                        alu.FechaBaja = Convert.ToDateTime(reader["FechaBaja"].ToString());
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error cargando los datos de nuestro alumno " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (Comm != null)
                    Comm.Dispose();

                sqlConexion.Close();
                sqlConexion.Dispose();
            }

            return alu!;

        }

		public async Task<Alumno> DameAlumno(string email)
		{
			Alumno? alu = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;
			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.UsuarioDameAlumnos";
				Comm.CommandType = System.Data.CommandType.StoredProcedure;
				Comm.Parameters.Add("@email", SqlDbType.VarChar, 500).Value = email;
				reader = await Comm.ExecuteReaderAsync();
				if (reader.Read())
				{
					alu = new Alumno();
					alu.Id = Convert.ToInt32(reader["Id"]);
					alu.Nombre = reader["Nombre"].ToString()!;
					alu.Email = reader["Email"].ToString()!;
					alu.Foto = reader["Foto"].ToString()!;
					alu.FechaAlta = Convert.ToDateTime(reader["FechaAlta"].ToString());

					if (reader["FechaBaja"] != System.DBNull.Value)
						alu.FechaBaja = Convert.ToDateTime(reader["FechaBaja"].ToString());
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error cargando los datos de nuestro alumno " + ex.Message);
			}
			finally
			{
				if (reader != null)
					reader.Close();

				if (Comm != null)
					Comm.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return alu!;

		}

		public async Task<IEnumerable<Alumno>> DameAlumnos(int idPagina, int numRegistros)
        {
            List<Alumno> lista = new List<Alumno>();
            SqlConnection sqlConexion = conexion();
            SqlCommand? Comm = null;
            SqlDataReader? reader = null;
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.UsuarioDameAlumnos";
                Comm.CommandType = System.Data.CommandType.StoredProcedure;
				Comm.Parameters.Add("@numpagina", SqlDbType.Int).Value = idPagina;
				Comm.Parameters.Add("@numregistros", SqlDbType.Int).Value = numRegistros;
				reader = await Comm.ExecuteReaderAsync();
                while (reader.Read())
                {
                    Alumno alu = new Alumno();
                    alu.Id = Convert.ToInt32(reader["Id"]);
                    alu.Nombre = reader["Nombre"].ToString()!;
                    alu.Email = reader["Email"].ToString()!;
                    alu.Foto = reader["Foto"].ToString()!;
                    alu.FechaAlta = Convert.ToDateTime(reader["FechaAlta"].ToString());

                    if (reader["FechaBaja"] != System.DBNull.Value)
                        alu.FechaBaja = Convert.ToDateTime(reader["FechaBaja"].ToString());

					alu.paginacion = new Paginacion();
					alu.paginacion.totalPaginas = Convert.ToInt32(reader["totalPaginas"]);

                    lista.Add(alu);
                }
                reader.Close();
            }
            catch (SqlException ex)
            {
                throw new Exception("Error cargando los datos de nuestros alumnos " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (Comm != null)
                    Comm.Dispose();

                sqlConexion.Close();
                sqlConexion.Dispose();
            }

            return lista;
        }

        public async Task<Alumno> ModificarAlumno(Alumno alumno)
        {
            Alumno? alumnoModiifcado = null;
            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;
            SqlDataReader reader = null;
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.UsuarioAltaAlumno";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@IdAlumno", SqlDbType.Int).Value = alumno.Id;
                Comm.Parameters.Add("@Nombre", SqlDbType.VarChar, 500).Value = alumno.Nombre;
                Comm.Parameters.Add("@Email", SqlDbType.VarChar, 500).Value = alumno.Email;
                Comm.Parameters.Add("@Foto",SqlDbType.VarChar, 500).Value = alumno.Foto;

                if (alumno.FechaBaja != null)
                    Comm.Parameters.Add("@FechaBaja", SqlDbType.DateTime).Value = alumno.FechaBaja;

                reader = await Comm.ExecuteReaderAsync();
                if (reader.Read())
                    alumnoModiifcado = await DameAlumno(Convert.ToInt32(reader["idAlumno"]));
			}
            catch (SqlException ex)
            {
                throw new Exception("Error modificando el alumno" + ex.Message);
            }
			finally
			{
				if (reader != null)
					reader.Close();

				if (Comm != null)
					Comm.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return alumnoModiifcado;
		}

		public async Task<IEnumerable<Alumno>> BuscarAlumnos(string texto)
        {
            List<Alumno> alumnosEncontrados = new List<Alumno>();
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;
			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.UsuarioBuscarAlumnos";
				Comm.CommandType = System.Data.CommandType.StoredProcedure;
				Comm.Parameters.Add("@texto", SqlDbType.VarChar, 500).Value = texto;
				reader = await Comm.ExecuteReaderAsync();
				while (reader.Read())
				{
					Alumno alu = new Alumno();
					alu.Id = Convert.ToInt32(reader["Id"]);
					alu.Nombre = reader["Nombre"].ToString()!;
					alu.Email = reader["Email"].ToString()!;
					alu.Foto = reader["Foto"].ToString()!;
					alu.FechaAlta = Convert.ToDateTime(reader["FechaAlta"].ToString());

					if (reader["FechaBaja"] != System.DBNull.Value)
						alu.FechaBaja = Convert.ToDateTime(reader["FechaBaja"].ToString());

                    alumnosEncontrados.Add(alu);
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("Error al buscar alumnos: " + ex.Message);
			}
			finally
			{
				if (reader != null)
					reader.Close();

				if (Comm != null)
					Comm.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return alumnosEncontrados;
		}

		public async Task<Alumno> inscribirAlumnoCurso(Alumno Alumno, int idCurso, int idPrecio)
		{
			Alumno alumnoInscrito = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand Comm = null;

			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.UsuarioInscribirCurso";
				Comm.CommandType = CommandType.StoredProcedure;
				Comm.Parameters.Add("@idAlumno", SqlDbType.Int).Value = Alumno.Id;
				Comm.Parameters.Add("@idCurso", SqlDbType.Int).Value = idCurso;
				Comm.Parameters.Add("@idPrecio", SqlDbType.Int).Value = idPrecio;

				await Comm.ExecuteNonQueryAsync();

				alumnoInscrito = await DameAlumno(Alumno.Id);

			}
			catch (SqlException ex)
			{
				throw new Exception("Error inscribiendo alumno en curso" + ex.Message);
			}
			finally
			{
				Comm?.Dispose();
				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return alumnoInscrito;
		}

		public async Task<Alumno?> AlumnoCursos(int idAlumno)
		{
			Alumno? alumno = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;
			int idCursoAux = -1;
            Curso? c = null;
            try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.UsuarioInscritoCursos";
				Comm.CommandType = CommandType.StoredProcedure;
				Comm.Parameters.Add("@idAlumno", SqlDbType.Int).Value = idAlumno;
				reader = await Comm.ExecuteReaderAsync();
				while (reader.Read())
				{
					if (alumno == null)
					{
						alumno = new Alumno();
						alumno.Id = Convert.ToInt32(reader["Id"]);
						alumno.Nombre = reader["Nombre"].ToString();
						alumno.Email = reader["Email"].ToString();
						alumno.Foto = reader["Foto"].ToString();
						alumno.Listacursos = new List<Curso>();
					}

					if(idCursoAux == -1 || idCursoAux != Convert.ToInt32(reader["idCurso"]))
					{
						if (c != null)
							alumno.Listacursos!.Add(c);

						c = new Curso();
						c.Id = Convert.ToInt32(reader["idCurso"]);
						c.NombreCurso = reader["NombreCurso"].ToString();
						c.ListaPrecios = new List<Precio>();
					}

					Precio p = new Precio();
					p.Coste = Convert.ToDouble(reader["Coste"]);
					p.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
					p.FechaTermino = Convert.ToDateTime(reader["FechaTermino"]);
					c.ListaPrecios!.Add(p);

					idCursoAux = Convert.ToInt32(reader["idCurso"]);

				}
				if(c != null)
                    alumno!.Listacursos!.Add(c);

			}
			catch (SqlException ex)
			{
				throw new Exception("Error cargando los datos de nuestro alumno " + ex.Message);
			}
			finally
			{
				reader?.Close();
				Comm?.Dispose();
				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return alumno;
		}

	}

}
