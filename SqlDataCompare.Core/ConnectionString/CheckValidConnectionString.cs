using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

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
				throw new BadConnectionStringException("Ошибка подключения к серверу. Проверьте подключение.", exc);
			}
			finally
			{
				sqlConnection.Close();
			}
			return true;
		}
	}
}
