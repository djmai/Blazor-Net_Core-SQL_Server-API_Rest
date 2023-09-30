using APIAlumnos.Datos;
using Microsoft.Data.SqlClient;
using ModeloClasesAlumnos;
using System;
using System.Data;

namespace APIAlumnos.Repositorio
{
	public class RepositorioCursos : IRepositorioCursos
	{
		private string CadenaConexion;

		public RepositorioCursos(AccesoDatos cadenaConexion)
		{
			CadenaConexion = cadenaConexion.CadenaConexionSQL;
		}
		private SqlConnection conexion()
		{
			return new SqlConnection(CadenaConexion);
		}

		public async Task<IEnumerable<Curso>> DameCursos(int idAlumno)
		{
			List<Curso> listaCursos = new List<Curso>();
			Curso curso = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;
			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.CursoDameCursos";
				Comm.CommandType = System.Data.CommandType.StoredProcedure;
				if (idAlumno != -1)
					Comm.Parameters.Add("@idAlumno", System.Data.SqlDbType.Int).Value = idAlumno;

				reader = await Comm.ExecuteReaderAsync();
				while (reader.Read())
				{
					if(curso == null || curso.Id != Convert.ToInt32(reader["idCurso"]))
					{
						if(curso != null)
							listaCursos.Add(curso);

						curso = new Curso();
						curso.Id = Convert.ToInt32(reader["idCurso"]);
						curso.NombreCurso = reader["NombreCurso"].ToString();
						curso.ListaPrecios = new List<Precio>();
					}

					// Añadimos los posibles precios del curso
					Precio aux = new Precio();
					aux.Id = Convert.ToInt32(reader["idPrecio"]);
					aux.Coste = Convert.ToDouble(reader["Coste"]);
					aux.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
					aux.FechaTermino= Convert.ToDateTime(reader["FechaTermino"]);

					curso.ListaPrecios!.Add(aux);

				}

				if (curso != null)
					listaCursos.Add(curso);

			}
			catch (SqlException ex)
			{
				throw new Exception("Error cargar los datos de los cursos: " + ex.Message);
			}
			finally
			{
				reader?.Close();

				Comm?.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return listaCursos;
		}

		public async Task<Curso> DameCurso(int id)
		{
			Curso curso = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;
			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.CursoDameCursos";
				Comm.CommandType = System.Data.CommandType.StoredProcedure;
				Comm.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;

				reader = await Comm.ExecuteReaderAsync();
				while (reader.Read())
				{
					if (curso == null)
					{
						curso = new Curso();
						curso.Id = Convert.ToInt32(reader["idCurso"]);
						curso.NombreCurso = reader["NombreCurso"].ToString();
						curso.ListaPrecios = new List<Precio>();
					}

					// Añadimos los posibles precios del curso
					Precio aux = new Precio();
					aux.Id = Convert.ToInt32(reader["idPrecio"]);
					aux.Coste = Convert.ToDouble(reader["Coste"]);
					aux.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
					aux.FechaTermino = Convert.ToDateTime(reader["FechaTermino"]);

					curso.ListaPrecios!.Add(aux);
				}

			}
			catch (SqlException ex)
			{
				throw new Exception("Error cargar los datos de los cursos: " + ex.Message);
			}
			finally
			{
				reader?.Close();

				Comm?.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return curso;
		}

		public async Task<Curso> DameCurso(string nombreCurso)
		{
			Curso curso = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;
			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.CursoDameCursos";
				Comm.CommandType = System.Data.CommandType.StoredProcedure;
				Comm.Parameters.Add("@NombreCurso", System.Data.SqlDbType.VarChar, 500).Value = nombreCurso;
				reader = await Comm.ExecuteReaderAsync();
				while (reader.Read())
				{
					if (curso == null)
					{
						curso = new Curso();
						curso.Id = Convert.ToInt32(reader["idCurso"]);
						curso.NombreCurso = reader["NombreCurso"].ToString();
						curso.ListaPrecios = new List<Precio>();
					}

					// Añadimos los posibles precios del curso
					Precio aux = new Precio();
					aux.Id = Convert.ToInt32(reader["idPrecio"]);
					aux.Coste = Convert.ToDouble(reader["Coste"]);
					aux.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
					aux.FechaTermino = Convert.ToDateTime(reader["FechaTermino"]);

					curso.ListaPrecios!.Add(aux);
				}

			}
			catch (SqlException ex)
			{
				throw new Exception("Error cargar los datos de los cursos: " + ex.Message);
			}
			finally
			{
				reader?.Close();

				Comm?.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return curso;
		}

		public async Task<Curso> AltaCurso(Curso curso)
		{
			Curso cursoNuevo = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;
			int idCursoCreado = -1;
			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.CursoAltaCurso";
				Comm.CommandType = System.Data.CommandType.StoredProcedure;

				int cont = 0;
				while (cont < curso.ListaPrecios.Count)
				{
					if (curso.ListaPrecios == null || curso.ListaPrecios[cont] == null ||
						curso.ListaPrecios[cont].FechaInicio == null || curso.ListaPrecios[cont].FechaTermino == null)
						throw new Exception("Para dar de alta un curso debes enviar precio, fecha de inicio y fecha termino");
					
					Comm.Parameters.Clear();
					Comm.Parameters.Add("@NombreCurso", System.Data.SqlDbType.VarChar, 500).Value = curso.NombreCurso;
					Comm.Parameters.Add("@Coste", System.Data.SqlDbType.Float).Value = curso.ListaPrecios[cont].Coste;
					Comm.Parameters.Add("@FechaInicio", System.Data.SqlDbType.DateTime).Value = curso.ListaPrecios[cont].FechaInicio;
					Comm.Parameters.Add("@FechaTermino", System.Data.SqlDbType.DateTime).Value = curso.ListaPrecios[cont].FechaTermino;
					idCursoCreado = Convert.ToInt32(await Comm.ExecuteScalarAsync());
					cont++;
				}

				if(idCursoCreado != -1)
				{
					cursoNuevo = await DameCurso(idCursoCreado);
				}

			}
			catch (SqlException ex)
			{
				throw new Exception("Error cargar los datos de los cursos: " + ex.Message);
			}
			finally
			{
				reader?.Close();

				Comm?.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return cursoNuevo;
		}

		public async Task<Curso> BorrarCurso(int id)
		{
			Curso cursoBorrado = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand Comm = null;
			SqlDataReader reader = null;
			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.CursoBorrarCurso";
				Comm.CommandType = CommandType.StoredProcedure;
				Comm.Parameters.Add("@idCurso", SqlDbType.Int).Value = id;
				cursoBorrado = await DameCurso(id);
				await Comm.ExecuteNonQueryAsync();
			}
			finally
			{
				reader?.Close();
				Comm?.Dispose();
				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return cursoBorrado;

		}

		public async Task<Curso> ModificarCurso(Curso curso)
		{
			Curso cursoModificado = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;
			try
			{
				sqlConexion.Open();
				int cont = 0;
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.CursoModificarCurso";
				Comm.CommandType = System.Data.CommandType.StoredProcedure;

				while(cont < curso.ListaPrecios.Count)
				{
					Comm.Parameters.Clear();
					Comm.Parameters.Add("@idCurso", System.Data.SqlDbType.Int).Value = curso.Id;
					if (curso.ListaPrecios[cont].Id > 0)
						Comm.Parameters.Add("@idPrecio", System.Data.SqlDbType.Float).Value = curso.ListaPrecios[cont].Id;
					Comm.Parameters.Add("@NombreCurso", System.Data.SqlDbType.VarChar, 500).Value = curso.NombreCurso;
					Comm.Parameters.Add("@Coste", System.Data.SqlDbType.Float).Value = curso.ListaPrecios[cont].Coste;
					Comm.Parameters.Add("@FechaInicio", System.Data.SqlDbType.DateTime).Value = curso.ListaPrecios[cont].FechaInicio;
					Comm.Parameters.Add("@FechaTermino", System.Data.SqlDbType.DateTime).Value = curso.ListaPrecios[cont].FechaTermino;
					await Comm.ExecuteNonQueryAsync();
					cont++;
				}

				cursoModificado = await DameCurso(curso.Id);

			}
			catch (SqlException ex)
			{
				throw new Exception("Error al modificar curso: " + ex.Message);
			}
			finally
			{
				reader?.Close();

				Comm?.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return cursoModificado;
		}

		public async Task<Curso> DameCurso(int id, int idPrecio)
		{
			Curso curso = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand? Comm = null;
			SqlDataReader? reader = null;
			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.CursoDameCursos";
				Comm.CommandType = System.Data.CommandType.StoredProcedure;
				Comm.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
				Comm.Parameters.Add("@idPrecio", System.Data.SqlDbType.Int).Value = idPrecio;

				reader = await Comm.ExecuteReaderAsync();
				while (reader.Read())
				{
					if (curso == null)
					{
						curso = new Curso();
						curso.Id = Convert.ToInt32(reader["idCurso"]);
						curso.NombreCurso = reader["NombreCurso"].ToString();
						curso.ListaPrecios = new List<Precio>();
					}

					// Añadimos los posibles precios del curso
					Precio aux = new Precio();
					aux.Id = Convert.ToInt32(reader["idPrecio"]);
					aux.Coste = Convert.ToDouble(reader["Coste"]);
					aux.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
					aux.FechaTermino = Convert.ToDateTime(reader["FechaTermino"]);

					curso.ListaPrecios!.Add(aux);
				}

			}
			catch (SqlException ex)
			{
				throw new Exception("Error cargar los datos de los cursos: " + ex.Message);
			}
			finally
			{
				reader?.Close();

				Comm?.Dispose();

				sqlConexion.Close();
				sqlConexion.Dispose();
			}

			return curso;
		}
	}
}
