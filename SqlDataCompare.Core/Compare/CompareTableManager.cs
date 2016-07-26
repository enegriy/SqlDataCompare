using System.Collections.Generic;
using System.Linq;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Сравнить таблицы
	/// </summary>
	public class CompareTableManager
	{
		private ISqlConnection _sqlConnectionSource;
		private ISqlConnection _sqlConnectionTarget;

		public CompareTableManager(ISqlConnection sqlConnectionSource, ISqlConnection sqlConnectionTarget)
		{
			_sqlConnectionSource = sqlConnectionSource;
			_sqlConnectionTarget = sqlConnectionTarget;
		}

		/// <summary>
		/// Список таблиц соответствующие источнику и приемнику
		/// </summary>
		public IEnumerable<string> CompareTable(IEnumerable<string> sourceTableNames, IEnumerable<string> targetTableNames)
		{
			var tables = sourceTableNames.Intersect(targetTableNames).ToArray();
			return CompareTableStruct(tables).ToArray();
		}

		public IEnumerable<string> CompareTableStruct(IEnumerable<string> tableNames)
		{
			var sql = " SELECT TABLE_SCHEMA+TABLE_NAME+COLUMN_NAME+DATA_TYPE from {0}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA + '.' + TABLE_NAME = '{1}'; ";

			var selectManagerSource = new SelectManager(_sqlConnectionSource);
			var selectManagerTarget = new SelectManager(_sqlConnectionTarget);

			foreach (var tableName in tableNames)
			{
				var sqlSource = string.Format(sql, _sqlConnectionSource.Database, tableName);
				var sqlTarget = string.Format(sql, _sqlConnectionTarget.Database, tableName);

				var listStructSource = selectManagerSource.SelectFirstColumn<string>(sqlSource).OrderBy(x => x);
				var listStructTarget = selectManagerTarget.SelectFirstColumn<string>(sqlTarget).OrderBy(x => x);

				if (listStructSource.Except(listStructTarget).Any())
					continue;
				else
					yield return tableName;
			}
			
		} 

	}
}
