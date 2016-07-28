
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
			DetailsToConnect detailsToConnectSource = new DetailsToConnect("WIN-9KGOLTFA5EP", "Messanger", "sa", "123456");
			DetailsToConnect detailsToConnectTarget = new DetailsToConnect("WIN-9KGOLTFA5EP", "Test", "sa", "123456");

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
