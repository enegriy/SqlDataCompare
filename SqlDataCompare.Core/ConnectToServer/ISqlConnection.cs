namespace SqlDataCompare.Core
{
	/// <summary>
	/// Интерфейс для создания подключения к бд
	/// </summary>
	public interface ISqlConnection
	{
		/// <summary>
		/// База данных
		/// </summary>
		string ConnectionString { get; set; }

		/// <summary>
		/// База данных
		/// </summary>
		string Database { get; }

		/// <summary>
		/// Открыть подключение
		/// </summary>
		void Open();

		/// <summary>
		/// Закрыть подключение
		/// </summary>
		void Close();

		/// <summary>
		/// Объект соединения с бд
		/// </summary>
		object Connection { get; }
	}
}
