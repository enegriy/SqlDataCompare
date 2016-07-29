namespace SqlDataCompare.Core
{
	/// <summary>
	/// Сборщик строки подключения
	/// </summary>
	public class ConnectionStringMaker
	{
		private readonly ServerPassport _serverPassport;

		private string _connectionStringWinAuthTemplate =
			@"Persist Security Info=False;Integrated Security=true;Initial Catalog={1};Server={0}";

		private string _connectionStringSqlAuthTemplate =
			@"Persist Security Info=False;User ID={2};Password={3};Initial Catalog={1};Server={0}";

		/// <summary>
		/// .Ctor
		/// </summary>
		public ConnectionStringMaker(ServerPassport serverPassport)
		{
			_serverPassport = serverPassport;
		}

		/// <summary>
		/// Получить строку подключения
		/// </summary>
		public string GetConnectionString()
		{
			string connectionString;
			if (string.IsNullOrEmpty(_serverPassport.Login))
			{
				connectionString = string.Format(
					_connectionStringWinAuthTemplate, _serverPassport.ServerName, _serverPassport.BaseName);
			}
			else
			{
				connectionString = string.Format(
					_connectionStringSqlAuthTemplate, _serverPassport.ServerName, _serverPassport.BaseName,
					_serverPassport.Login,
					_serverPassport.Password);
			}
			return connectionString;
		}
	}
}
