
using System;
using SqlDataCompare.Core;

namespace SqlDataCompare.Test
{
	public class ConnectionStubFail : ISqlConnection
	{
		public string ConnectionString { get; set; }
		public string Database { get; }
		public void Open()
		{
			throw new Exception("Сервер не доступен");
		}

		public void Close()
		{
		}

		public object Connection { get; }
	}
}
