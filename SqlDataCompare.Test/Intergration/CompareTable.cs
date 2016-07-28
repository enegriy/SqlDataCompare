
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlDataCompare.Core;

namespace SqlDataCompare.Test.Intergration
{
	[TestFixture]
	public class CompareTableManager_Test
	{
		[Test]
		public void CompareTable_IsNotEmpty_Test()
		{
			DetailsToConnect detailsToConnectSource = new DetailsToConnect("192.168.0.22", "RI_BankGuarantee", "sa", "456P@ssw0rd");
			DetailsToConnect detailsToConnectTarget = new DetailsToConnect("192.168.0.22", "RI_BankGuarantee_PROD_COPY", "sa", "456P@ssw0rd");

			var connectionStringMakerSource = new ConnectionStringMaker(detailsToConnectSource);
			var connectionStringMakerTarget = new ConnectionStringMaker(detailsToConnectTarget);

			ISqlConnection sqlConnectionSource = new SqlServerConnection(connectionStringMakerSource.GetConnectionString());
			ISqlConnection sqlConnectionTarget = new SqlServerConnection(connectionStringMakerTarget.GetConnectionString());

			IDatabaseMetadata databaseMetadataSource = new DatabaseMetadata(sqlConnectionSource);
			IDatabaseMetadata databaseMetadataTarget = new DatabaseMetadata(sqlConnectionTarget);

			var tablesSource = databaseMetadataSource.GetTables();
			var tablesTarget = databaseMetadataTarget.GetTables();

			var compareTableManager = new CompareTableManager(sqlConnectionSource, sqlConnectionTarget);

			var listTable = compareTableManager.CompareTable(tablesSource, tablesTarget);
			var resultCompare = compareTableManager.CompareTablesData(listTable);

			Assert.IsNotEmpty(listTable);
		}
	}
}
