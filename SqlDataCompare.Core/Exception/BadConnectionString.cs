namespace SqlDataCompare.Core
{
	/// <summary>
	/// Плохая строка подключения
	/// </summary>
	public class BadConnectionStringException : System.Exception
	{
		public BadConnectionStringException(): base() { }

		public BadConnectionStringException(string message): base(message) { }

		public BadConnectionStringException(string format, params object[] args): base(string.Format(format, args)) { }

		public BadConnectionStringException(string message, System.Exception innerException): base(message, innerException) { }

		public BadConnectionStringException(string format, System.Exception innerException, params object[] args): base(string.Format(format, args), innerException) { }
	}
}
