
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlDataCompare.Core;
using SqlDataCompare.Core.Select;

namespace SqlDataCompare.Test.Intergration
{
	[TestFixture]
	public class CompareTableManager_Test
	{
		[Test]
		public void CompareTable_IsNotEmpty_Test()
		{
			ServerPassport serverPassportSource = new ServerPassport("192.168.0.22", "RI_BankGuarantee", "sa", "456P@ssw0rd");
			ServerPassport serverPassportTarget = new ServerPassport("192.168.0.22", "RI_BankGuarantee_PROD_COPY", "sa", "456P@ssw0rd");

			var connectionStringMakerSource = new ConnectionStringMaker(serverPassportSource);
			var connectionStringMakerTarget = new ConnectionStringMaker(serverPassportTarget);

			ISqlConnection sqlConnectionSource = new SqlServerConnection(connectionStringMakerSource.GetConnectionString());
			ISqlConnection sqlConnectionTarget = new SqlServerConnection(connectionStringMakerTarget.GetConnectionString());

			ISelectManager selectMng = new SelectManager();

			selectMng.Connection = sqlConnectionSource;
			IDbMetadata databaseMetadataSource = new DbMetadata(selectMng);
			selectMng.Connection = sqlConnectionTarget;
			IDbMetadata databaseMetadataTarget = new DbMetadata(selectMng);

			var tablesSource = databaseMetadataSource.GetTables();
			var tablesTarget = databaseMetadataTarget.GetTables();

			var compareTableManager = new CompareTableManager(sqlConnectionSource, sqlConnectionTarget);

			var listTable = compareTableManager.PrepareTableToCompare(tablesSource, tablesTarget);
			var resultCompare = compareTableManager.TablesDataCompare(listTable);

			Assert.IsNotEmpty(listTable);
		}
	}
}
