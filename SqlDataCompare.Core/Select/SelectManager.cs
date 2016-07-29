using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SqlDataCompare.Core.Select;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Выборка из бд
	/// </summary>
	public class SelectManager : ISelectManager
	{
		public ISqlConnection Connection { get; set; }

		public SelectManager(ISqlConnection connection)
		{
			Connection = connection;
		}

		public SelectManager()
		{
		}

		public IEnumerable<T> SelectFirstColumn<T>(string sql)
		{
			IList<T> list = new List<T>();

			Connection.Open();
			try
			{
				var command = new SqlCommand(
					string.Format(sql, Connection.Database));

				command.Connection = Connection.Connection as SqlConnection;

				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					list.Add((T)reader[0]);
				}
			}
			finally
			{
				Connection.Close();
			}
			return list;
		}
	}
}
