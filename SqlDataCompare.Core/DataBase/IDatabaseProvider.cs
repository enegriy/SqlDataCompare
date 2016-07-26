
using System.Collections.Generic;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Интерфейс для провайдера доступа к бд
	/// </summary>
	public interface IDatabaseProvider
	{
		/// <summary>
		/// Список таблиц
		/// </summary>
		IEnumerable<string> GetTables();
		/// <summary>
		/// Список колонок в таблице
		/// </summary>
		IEnumerable<string> GetColumnsByTable(string table);
		/// <summary>
		/// Ключевые колонки
		/// </summary>
		IEnumerable<string> GetKeyColumnsByTable(string table);
	}
}
