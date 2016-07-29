
using System.Collections.Generic;

namespace SqlDataCompare.Core.Select
{
	/// <summary>
	/// Выборка из БД
	/// </summary>
	public interface ISelectManager
	{
		ISqlConnection Connection { get; set; }
		IEnumerable<T> SelectFirstColumn<T>(string sql);
	}
}
