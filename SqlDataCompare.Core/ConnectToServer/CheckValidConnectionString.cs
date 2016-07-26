
using System;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Проверить валидность строки подключения
	/// </summary>
	public class CheckValidConnectionString
	{
		/// <summary>
		/// Проверить
		/// </summary>
		public bool Check(ISqlConnection sqlConnection ,string connectionString)
		{
			sqlConnection.ConnectionString = connectionString;
			try
			{
				sqlConnection.Open();
			}
			catch (Exception exc)
			{
				throw new OperationCanceledException("Ошибка подключения к серверу. Проверьте подключение "+connectionString, exc);
			}
			finally
			{
				sqlConnection.Close();
			}
			return true;
		}
	}
}
