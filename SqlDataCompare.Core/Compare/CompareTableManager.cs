using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

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


		public string CompareTableData(IEnumerable<string> tableNames)
		{
			var sb = new StringBuilder();

			var selectManagerSource = new SelectManager(_sqlConnectionSource);
			var selectManagerTarget = new SelectManager(_sqlConnectionTarget);

			IDatabaseMetadata databaseMetadata = new DatabaseMetadata(_sqlConnectionSource);

			foreach (var table in tableNames)
			{
				var keyColumns = databaseMetadata.GetKeyColumnsByTable(table);
				var keyColumnsString = string.Join(" + ", keyColumns);

				var sql = "select " + keyColumnsString + " from [" + table +"]";

				var listValuesSource = selectManagerSource.SelectFirstColumn<string>(sql);
				var listValuesTarget = selectManagerTarget.SelectFirstColumn<string>(sql);

				var listToUpdate = Enumerable.Intersect(listValuesSource, listValuesTarget);
				var listToInsert = listValuesSource.Except(listValuesTarget);
				var listToDelete = listValuesTarget.Except(listValuesSource);

				sb.AppendLine(CreateDeleteQuery(table, keyColumnsString, listToDelete));
				sb.AppendLine(CreateInsertQuery(_sqlConnectionSource, table, keyColumnsString, listToInsert));
			}

			return sb.ToString();
		}

		private string CreateInsertQuery(
			ISqlConnection sqlConnectionSource, 
			string table, 
			string keyColumnsString, 
			IEnumerable<string> listToInsert)
		{
			StringBuilder sb = new StringBuilder();
			sqlConnectionSource.Open();
			try
			{
				IDatabaseMetadata databaseMetadata = new DatabaseMetadata(sqlConnectionSource);
				var columns = databaseMetadata.GetColumnsByTable(table);
				var columnsString = string.Join(", ", columns);

				SqlCommand command = new SqlCommand("select " + columnsString + "from " + table + " where " + keyColumnsString + "in (" + string.Join(",", listToInsert) + ");");
				command.Connection = sqlConnectionSource.Connection as SqlConnection;
				var reader = command.ExecuteReader();
				while(reader.Read())
				{
					var values = new List<string>();

					foreach(var column in columns)
					{
						values.Add(reader[column].ToString());
					}

					sb.AppendLine("INSERT INTO " + table + "(" + columns + ") VALUES (" + string.Join(",", values) + ");");
				}
			}
			finally
			{
				sqlConnectionSource.Close();
			}
			return sb.ToString();
		}

		private string CreateDeleteQuery(
			string table, 
			string keyColumnsString,
			IEnumerable<string >listToDelete)
		{
			StringBuilder sb = new StringBuilder();
			foreach(var keyDelete in listToDelete)
			{
				sb.AppendLine("delete from "+table+" where "+ keyColumnsString +" = '"+keyDelete+"';");
			}
			return sb.ToString();
		}
	}
}
