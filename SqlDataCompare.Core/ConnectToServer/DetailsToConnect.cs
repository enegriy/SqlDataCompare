namespace SqlDataCompare.Core
{
	/// <summary>
	/// Детали для подключения к sql server
	/// </summary>
	public class DetailsToConnect
	{
		/// <summary>
		/// Имя сервера
		/// </summary>
		public string ServerName { get; private set; }
		/// <summary>
		/// Имя БД
		/// </summary>
		public string BaseName { get; private set; }
		/// <summary>
		/// Логин
		/// </summary>
		public string Login { get; private set; }
		/// <summary>
		/// Пароль
		/// </summary>
		public string Password { get; private set; }

		/// <summary>
		/// .Ctor
		/// </summary>
		public DetailsToConnect(string serverName, string baseName, string login = "", string password = "")
		{
			ServerName = serverName;
			BaseName = baseName;
			Login = login;
			Password = password;
		}
	}
}
