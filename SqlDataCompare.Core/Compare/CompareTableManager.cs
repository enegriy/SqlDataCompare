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
		private readonly ISqlConnection _sqlConnectionSource;
		private readonly ISqlConnection _sqlConnectionTarget;

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

		public string CompareTablesData(IEnumerable<string> tableNames)
		{
			IDatabaseMetadata databaseMetadata = new DatabaseMetadata(_sqlConnectionSource);

			foreach (var table in tableNames)
			{
				var keyColumns = databaseMetadata.GetKeyColumnsByTable(table);
				var keyColumnsString = string.Join(" + ", keyColumns);

				Type typeColumn = typeof (string);
				if (keyColumns.Count() == 1)
				{
					typeColumn = databaseMetadata.GetColumnType(table, keyColumns.First());
				}

				if (typeColumn == typeof(string))
				{
					CompareByColumnKey<string>(keyColumnsString, table);
				}
				if (typeColumn == typeof(int))
				{
					CompareByColumnKey<string>(keyColumnsString, table);
				}
				if (typeColumn == typeof(Guid))
				{
					CompareByColumnKey<string>(keyColumnsString, table);
				}
			}

			return string.Empty;
		}

		public string CompareByColumnKey<T>(string key, string table)
		{
			var sb = new StringBuilder();

			var valuesFromTableSource = GetValuesFromTable<T>(key, table, _sqlConnectionSource);
			var valuesFromTableTarget = GetValuesFromTable<T>(key, table, _sqlConnectionTarget);

			var listKeysToUpdate = Enumerable.Intersect(valuesFromTableSource, valuesFromTableTarget);
			var listKeysToInsert = valuesFromTableSource.Except(valuesFromTableTarget);
			var listKeysToDelete = valuesFromTableTarget.Except(valuesFromTableSource);

			sb.AppendLine(CreateDeleteQuery(table, key, listKeysToDelete));
			sb.AppendLine(CreateInsertQuery(_sqlConnectionSource, table, key, listKeysToInsert));

			return String.Empty;
		}

		private IEnumerable<T> GetValuesFromTable<T>(string key, string table,ISqlConnection connection)
		{
			var sql = "select " + key + " from " + table;
			var sm = new SelectManager<T>();
			return sm.Select(connection, sql);
		} 

		private string CreateInsertQuery(
			ISqlConnection sqlConnectionSource, 
			string table, 
			string keyColumnsString, 
			IEnumerable<string> listToInsert)
		{
			StringBuilder sb = new StringBuilder();
			try
			{
				IDatabaseMetadata databaseMetadata = new DatabaseMetadata(sqlConnectionSource);

				var dbColumns = GetColumns(table);
				var columnsString = string.Join(", ", dbColumns.Select(x=>x.ColumnName));

				sqlConnectionSource.Open();

				SqlCommand command = new SqlCommand(
					"select " + columnsString + "from " + table +
					" where " + keyColumnsString + 
					"in (" + string.Join(",", listToInsert.Select(x=>"'"+x+"'").ToArray()) + ");");

				command.Connection = sqlConnectionSource.Connection as SqlConnection;
			}
			finally
			{
				sqlConnectionSource.Close();
			}
			return sb.ToString();
		}

		private string CreateDeleteQuery<T>(
			string table, 
			string keyColumnsString,
			IEnumerable<T>listToDelete)
		{
			StringBuilder sb = new StringBuilder();
			foreach(var keyDelete in listToDelete)
			{
				sb.AppendLine("delete from "+table+" where "+ keyColumnsString +" = '"+keyDelete+"';");
			}
			return sb.ToString();
		}

		private object GetSelectManager(Type typeKeyColumn)
		{
			var genericListType = typeof(SelectManager<>);
			var specificListType = genericListType.MakeGenericType(typeKeyColumn);
			return Activator.CreateInstance(specificListType);
		}

		private IEnumerable<DbColumn> GetColumns(string table)
		{
			IDatabaseMetadata databaseMetadata = new DatabaseMetadata(_sqlConnectionSource);

			var columnsNames = databaseMetadata.GetColumnsByTable(table);
			IList<DbColumn> dbColumns = new List<DbColumn>();
			foreach (var columnName in columnsNames)
			{
				DbColumn dbColumn = new DbColumn();
				dbColumn.ColumnName = columnName;
				dbColumn.ColumnType = databaseMetadata.GetColumnType(table, columnName);
				dbColumns.Add(dbColumn);
			}
			return dbColumns;
		} 
	}
}
