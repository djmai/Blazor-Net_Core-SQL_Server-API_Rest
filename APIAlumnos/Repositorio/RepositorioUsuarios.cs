using APIAlumnos.Datos;
using Microsoft.Data.SqlClient;
using ModeloClasesAlumnos;
using System.Data;

namespace APIAlumnos.Repositorio
{
	public class RepositorioUsuarios : IRepositorioUsuarios
	{
		private string CadenaConexion;

		public RepositorioUsuarios(AccesoDatos cadenaConexion)
		{
			CadenaConexion = cadenaConexion.CadenaConexionSQL;
		}

		private SqlConnection conexion()
		{
			return new SqlConnection(CadenaConexion);
		}

		public async Task<UsuarioLogin> AltaUsuario(UsuarioLogin usuario)
		{
			UsuarioLogin usuarioCreado = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand Comm = null;
			SqlDataReader reader = null;

			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.UsuarioAplicacionCrear";
				Comm.CommandType = CommandType.StoredProcedure;
				Comm.Parameters.Add("@Email", SqlDbType.VarChar, 500).Value = usuario.EmailLogin;
				Comm.Parameters.Add("@Pass", SqlDbType.VarChar, 500).Value = usuario.Password;

				reader = await Comm.ExecuteReaderAsync();
				if (reader.Read())
				{
					usuarioCreado = await DameUsuario(Convert.ToInt32(reader["idUsuario"]));
				}
			}
			finally {
                if (reader != null)
                    reader.Close();
                Comm!.Dispose();
				sqlConexion!.Close();
				sqlConexion!.Dispose();
			}

			return usuarioCreado;
		}

		public async Task<UsuarioLogin> DameUsuario(int id)
		{
			UsuarioLogin usuario = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand Comm = null;
			SqlDataReader reader = null;

			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.UsuarioDameUsuario";
				Comm.CommandType = CommandType.StoredProcedure;
				Comm.Parameters.Add("@id", SqlDbType.Int).Value = id;
				reader = await Comm.ExecuteReaderAsync();

				if (reader.Read())
				{
					usuario = new UsuarioLogin();
					usuario.EmailLogin = reader["Email"].ToString();
					usuario.Password = reader["Pass"].ToString();
				}
			}
			finally
			{
				reader!.Close();
				Comm!.Dispose();
				sqlConexion!.Close();
				sqlConexion!.Dispose();
			}

			return usuario;
		}

		public async Task<UsuarioLogin> DameUsuario(string email)
		{
			UsuarioLogin usuario = null;
			SqlConnection sqlConexion = conexion();
			SqlCommand Comm = null;
			SqlDataReader reader = null;

			try
			{
				sqlConexion.Open();
				Comm = sqlConexion.CreateCommand();
				Comm.CommandText = "dbo.UsuarioDameUsuario";
				Comm.CommandType = CommandType.StoredProcedure;
				Comm.Parameters.Add("@email", SqlDbType.Int).Value = email;
				reader = await Comm.ExecuteReaderAsync();

				if (reader.Read())
				{
					usuario = new UsuarioLogin();
					usuario.EmailLogin = reader["Email"].ToString();
					usuario.Password = reader["Pass"].ToString();
				}
			}
			finally
			{
				reader!.Close();
				Comm!.Dispose();
				sqlConexion!.Close();
				sqlConexion!.Dispose();
			}

			return usuario;
		}
	}
}
