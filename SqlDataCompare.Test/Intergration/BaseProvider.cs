using NUnit.Framework;
using SqlDataCompare.Core;

namespace SqlDataCompare.Test.Intergration
{
	[TestFixture]
	class BaseProvider
	{
		[Test]
		public void GetTables_IsNotEmpty_Test()
		{
			DetailsToConnect detailsToConnect = new DetailsToConnect("192.168.0.22", "RI_BankGuarantee_PROD_COPY","sa","456P@ssw0rd");
			var connectionStringMaker = new ConnectionStringMaker(detailsToConnect);

			ISqlConnection sqlConnection = new SqlServerConnection(connectionStringMaker.GetConnectionString());
			IDatabaseMetadata databaseMetadata = new DatabaseMetadata(sqlConnection);

			Assert.IsNotEmpty(databaseMetadata.GetTables());
		}
	}
}
