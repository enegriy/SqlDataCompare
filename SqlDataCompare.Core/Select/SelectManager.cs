using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Выборка
	/// </summary>
	public class SelectManager
	{
		readonly ISqlConnection _sqlConnection;

		public SelectManager(ISqlConnection sqlConnection)
		{
			_sqlConnection = sqlConnection;
		}

		public IEnumerable<T> SelectFirstColumn<T>(string sql)
		{
			IList<T> list = new List<T>();

			_sqlConnection.Open();
			try
			{
				var command = new SqlCommand(
					string.Format(sql, _sqlConnection.Database));

				command.Connection = _sqlConnection.Connection as SqlConnection;

				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					list.Add((T)reader[0]);
				}
			}
			finally
			{
				_sqlConnection.Close();
			}
			return list;
		}
	}

	/// <summary>
	///  Выборка
	/// </summary>
	public class SelectManager<T>
	{
		public IEnumerable<T> Select(ISqlConnection sqlConnection, string sql)
		{
			return new SelectManager(sqlConnection).SelectFirstColumn<T>(sql);
		}
	}
}
