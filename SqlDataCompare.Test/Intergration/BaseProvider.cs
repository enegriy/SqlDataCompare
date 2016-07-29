using NUnit.Framework;
using SqlDataCompare.Core;
using SqlDataCompare.Core.Select;

namespace SqlDataCompare.Test.Intergration
{
	[TestFixture]
	class BaseProvider
	{
		[Test]
		public void GetTables_IsNotEmpty_Test()
		{
			ServerPassport serverPassport = new ServerPassport("192.168.0.22", "RI_BankGuarantee_PROD_COPY","sa","456P@ssw0rd");
			var connectionStringMaker = new ConnectionStringMaker(serverPassport);

			ISqlConnection sqlConnection = new SqlServerConnection(connectionStringMaker.GetConnectionString());
			ISelectManager selectMng = new SelectManager(sqlConnection);
			IDbMetadata databaseMetadata = new DbMetadata(selectMng);

			Assert.IsNotEmpty(databaseMetadata.GetTables());
		}
	}
}
