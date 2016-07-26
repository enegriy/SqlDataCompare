namespace SqlDataCompare.Core
{
	/// <summary>
	/// Сборщик строки подключения
	/// </summary>
	public class ConnectionStringMaker
	{
		private readonly DetailsToConnect _detailsToConnect;

		private string _connectionStringWinAuthTemplate =
			@"Persist Security Info=False;Integrated Security=true;Initial Catalog={1};Server={0}";

		private string _connectionStringSqlAuthTemplate =
			@"Persist Security Info=False;User ID={2};Password={3};Initial Catalog={1};Server={0}";

		/// <summary>
		/// .Ctor
		/// </summary>
		public ConnectionStringMaker(DetailsToConnect detailsToConnect)
		{
			_detailsToConnect = detailsToConnect;
		}

		/// <summary>
		/// Получить строку подключения
		/// </summary>
		public string GetConnectionString()
		{
			string connectionString;
			if (string.IsNullOrEmpty(_detailsToConnect.Login))
			{
				connectionString = string.Format(
					_connectionStringWinAuthTemplate,_detailsToConnect.ServerName,_detailsToConnect.BaseName);
			}
			else
			{
				connectionString = string.Format(
					_connectionStringSqlAuthTemplate,_detailsToConnect.ServerName,_detailsToConnect.BaseName,
					_detailsToConnect.Login,
					_detailsToConnect.Password);
			}
			return connectionString;
		}
	}
}
