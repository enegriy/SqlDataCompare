
using System.Data.SqlClient;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Подключение к SQL Server
	/// </summary>
	public class SqlServerConnection : ISqlConnection
	{
		private SqlConnection _sqlConnection;

		/// <summary>
		/// .Ctor
		/// </summary>
		public SqlServerConnection(string connectionString)
		{
			_sqlConnection = new SqlConnection(connectionString);
		}

		/// <summary>
		/// .Ctor
		/// </summary>
		public SqlServerConnection()
		{
			_sqlConnection = new SqlConnection();
		}


		public string ConnectionString {
			get { return _sqlConnection.ConnectionString; }
			set { _sqlConnection.ConnectionString = value; }
		}

		/// <summary>
		/// База данных
		/// </summary>
		public string Database {
			get{ return _sqlConnection.Database; }
		}

		/// <summary>
		/// Открыть
		/// </summary>
		public void Open()
		{
			_sqlConnection.Open();
		}

		/// <summary>
		/// Закрыть
		/// </summary>
		public void Close()
		{
			_sqlConnection.Close();
		}

		/// <summary>
		/// Объект соединения с бд
		/// </summary>
		public object Connection {
			get { return _sqlConnection;}
		}
	}
}
