
using SqlDataCompare.Core;

namespace SqlDataCompare.Test
{
	public class ConnectionStub : ISqlConnection
	{
		public string ConnectionString { get; set; }
		public string Database { get; }

		public void Open()
		{
			
		}

		public void Close()
		{
			
		}

		public object Connection { get; }
	}
}
