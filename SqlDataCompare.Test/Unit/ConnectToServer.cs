using System;
using NUnit.Framework;
using SqlDataCompare.Core;

namespace SqlDataCompare.Test
{
	[TestFixture]
	public class ConnectToServer
	{
		[Test]
		public void ConnectionStringMaker_GetConnectionWinAuth_Test()
		{
			var detailsToConnect = new DetailsToConnect("Test","Test");
			
			var connectionStringMaker = new ConnectionStringMaker(detailsToConnect);
			var connectionString = connectionStringMaker.GetConnectionString();

			Assert.IsTrue(connectionString.IndexOf("User ID", StringComparison.Ordinal) == -1);
		}

		[Test]
		public void ConnectionStringMaker_GetConnectionSqlAuth_Test()
		{
			var detailsToConnect = new DetailsToConnect("Test", "Test", "Login", "Password");

			var connectionStringMaker = new ConnectionStringMaker(detailsToConnect);
			var connectionString = connectionStringMaker.GetConnectionString();

			Assert.IsTrue(connectionString.IndexOf("User ID", StringComparison.Ordinal) != -1);
		}

		[Test]
		public void CheckValidConnectionString_IsSuccess_Test()
		{
			var detailsToConnect = new DetailsToConnect("Test", "Test", "Login", "Password");
			var connectionStringMaker = new ConnectionStringMaker(detailsToConnect);
			var connectionString = connectionStringMaker.GetConnectionString();

			var checkValidConnection = new CheckValidConnectionString();
			var connectionStub = new ConnectionStub();

			Assert.IsTrue(checkValidConnection.Check(connectionStub, connectionString));
		}

		[Test]
		public void CheckValidConnectionString_IsFailConnection_Test()
		{
			var detailsToConnect = new DetailsToConnect("Test", "Test", "Login", "Password");
			var connectionStringMaker = new ConnectionStringMaker(detailsToConnect);
			var connectionString = connectionStringMaker.GetConnectionString();

			var checkValidConnection = new CheckValidConnectionString();
			var connectionStub = new ConnectionStubFail();

			Assert.Catch<OperationCanceledException>(() => 
				checkValidConnection.Check(connectionStub, connectionString), 
				"Ошибка подключения к серверу. Проверьте подключение ");
		}
	}
}
