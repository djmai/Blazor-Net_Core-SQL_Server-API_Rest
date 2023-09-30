namespace APIAlumnos.Datos
{
	public class AccesoDatos
	{
		private string cadenaConexionSql;

		public string CadenaConexionSQL { get => cadenaConexionSql; }

		public AccesoDatos(string ConexionSql)
		{
			cadenaConexionSql = ConexionSql;
		}
	}
}
