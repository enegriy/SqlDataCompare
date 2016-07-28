using System;
using System.Collections;
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
			var databaseMetadata = new DatabaseMetadata(_sqlConnectionSource);

			foreach (var table in tableNames)
			{
				var keyColumns = databaseMetadata.GetKeyColumnsByTable(table);
				var dbColumn = GetDbColumn(table, keyColumns);

				if (dbColumn.ColumnType == typeof(string))
				{
					CompareByColumnKey<string>(dbColumn);
				}
				if (dbColumn.ColumnType == typeof(int))
				{
					CompareByColumnKey<int>(dbColumn);
				}
				if (dbColumn.ColumnType == typeof(Guid))
				{
					CompareByColumnKey<Guid>(dbColumn);
				}
			}

			return string.Empty;
		}
		public string CompareByColumnKey<T>(DbColumn dbKeyColumn)
		{
			var sb = new StringBuilder();

			var valuesFromTableSource = GetValuesFromTable<T>(_sqlConnectionSource, dbKeyColumn);
			var valuesFromTableTarget = GetValuesFromTable<T>(_sqlConnectionTarget, dbKeyColumn);

			var valuesToDelete = GetValuesToDelete(valuesFromTableSource, valuesFromTableTarget);
			var valuesToInsert = GetValuesToInsert(valuesFromTableSource, valuesFromTableTarget);
			var valuesToUpdate = GetValuesToUpdate(valuesFromTableSource, valuesFromTableTarget);

			sb.AppendLine(CreateQueryToDelete(valuesToDelete));
			sb.AppendLine(CreateQueryToInsert(valuesToInsert));
			sb.AppendLine(CreateQueryToUpdate(valuesToUpdate));

			return sb.ToString();
		}

		private IEnumerable<DbValue<T>> GetValuesToDelete<T>(
			IEnumerable<DbValue<T>> valuesSource,
			IEnumerable<DbValue<T>> valuesTarget)
		{
			IEnumerable<T> listTarget = valuesTarget.Select(x => x.Value).ToArray();
			IEnumerable<T> listSource = valuesSource.Select(x => x.Value).ToArray();
			var listExcept =  listTarget.Except(listSource);
			return valuesTarget.Where(x => listExcept.Contains(x.Value)).ToArray();
		}

		private IEnumerable<DbValue<T>> GetValuesToInsert<T>(
			IEnumerable<DbValue<T>> valuesSource,
			IEnumerable<DbValue<T>> valuesTarget)
		{
			IEnumerable<T> listTarget = valuesTarget.Select(x => x.Value).ToArray();
			IEnumerable<T> listSource = valuesSource.Select(x => x.Value).ToArray();
			var listExcept = listSource.Except(listTarget);
			return valuesSource.Where(x => listExcept.Contains(x.Value)).ToArray();
		}

		private IEnumerable<DbValue<T>> GetValuesToUpdate<T>(
			IEnumerable<DbValue<T>> valuesSource,
			IEnumerable<DbValue<T>> valuesTarget)
		{
			IEnumerable<T> listTarget = valuesTarget.Select(x => x.Value).ToArray();
			IEnumerable<T> listSource = valuesSource.Select(x => x.Value).ToArray();
			var listExcept = listSource.Intersect(listTarget);
			return valuesSource.Where(x => listExcept.Contains(x.Value)).ToArray();
		}

		private IEnumerable<DbValue<T>> GetValuesFromTable<T>(ISqlConnection connection, DbColumn dbColumn)
		{
			var sql = "select " + dbColumn.ColumnName + " from " + dbColumn.Table;
			var selectManager = new SelectManager(connection);
			var listValues = selectManager.SelectFirstColumn<T>(sql);
			return listValues.Select(x => new DbValue<T>(x, dbColumn));
		}

		private IEnumerable<DbValue> GetRowValues<T>(ISqlConnection connection, DbValue<T> keyValue)
		{
			IList<DbValue> dbValues = new List<DbValue>();

			var dbColumns = GetColumns(keyValue.Column.Table);
			var columnsString = string.Join(", ", dbColumns.Select(x => x.ColumnName));

			connection.Open();
			try
			{
				SqlCommand command = new SqlCommand(
					"select " + columnsString + "from " + keyValue.Column.Table +
					" where " + keyValue.Column.ColumnName + " = " + keyValue.ToString() + ";");

				command.Connection = connection.Connection as SqlConnection;

				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					foreach (var dbColumn in dbColumns)
					{
						var col = dbColumn.ColumnName.Replace("[", "").Replace("]", "");
						dbValues.Add(new DbValue(reader[col], dbColumn));
					}
				}
			}
			finally
			{
				connection.Close();
			}

			return dbValues;
		}

		private string CreateQueryToUpdate<T>(IEnumerable<DbValue<T>> dbKeyValues)
		{
			if (!dbKeyValues.Any())
				return string.Empty;

			var sb = new StringBuilder();

			try
			{
				IList<DbValue> differenceValues = new List<DbValue>();
				foreach (var keyValue in dbKeyValues)
				{
					var valuesSource = GetRowValues(_sqlConnectionSource, keyValue);
					var valuesTarget = GetRowValues(_sqlConnectionTarget, keyValue);

					foreach (var valueSource in valuesSource)
					{
						var valueTarget = valuesTarget.FirstOrDefault(x => x.Column.ColumnName.Equals(valueSource.Column.ColumnName));
						if (valueTarget != null && valueTarget.ToString() != valueSource.ToString())
						{
							differenceValues.Add(valueSource);
						}
					}

					if (differenceValues.Any())
					{
						sb.AppendLine("UPDATE " + keyValue.Column.Table + " SET " +
								  string.Join(",", differenceValues.Select(x => x.Column.ColumnName + "=" + x.ToString())) + " where " +
								  keyValue.Column.ColumnName + "=" + keyValue+";");

						differenceValues.Clear();
					}
				}
			}
			finally
			{
				_sqlConnectionSource.Close();
			}
			return sb.ToString();
		}

		private string CreateQueryToInsert<T>(IEnumerable<DbValue<T>> dbKeyValues)
		{
			if (!dbKeyValues.Any())
				return string.Empty;

			var sb = new StringBuilder();
			try
			{
				var table = dbKeyValues.First().Column.Table;
				var keyColumn = dbKeyValues.First().Column.ColumnName;
				var dbColumns = GetColumns(table);

				var columnsString = string.Join(", ", dbColumns.Select(x => x.ColumnName));

				_sqlConnectionSource.Open();

				SqlCommand command = new SqlCommand(
					"select " + columnsString + "from " + table +
					" where " + keyColumn +
					"in (" + string.Join(",", dbKeyValues.Select(x => x.ToString())) + ");");

				command.Connection = _sqlConnectionSource.Connection as SqlConnection;

				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					IList<DbValue> dbValues = new List<DbValue>();

					foreach (var dbColumn in dbColumns)
					{
						var col = dbColumn.ColumnName.Replace("[", "").Replace("]", "");
						dbValues.Add(new DbValue(reader[col], dbColumn));
					}

					IList<string> listColumn = new List<string>();
					IList<string> listColumnValue = new List<string>();

					foreach (var dbValue in dbValues)
					{
						listColumn.Add(dbValue.Column.ColumnName);
						listColumnValue.Add(dbValue.ToString());
					}

					sb.AppendLine("INSERT INTO " + table + "(" + string.Join(", ", listColumn) + ") VALUES (" + string.Join(", ", listColumnValue) + ");");
				}
			}
			finally
			{
				_sqlConnectionSource.Close();
			}
			return sb.ToString();
		}

		private string CreateQueryToDelete<T>(IEnumerable<DbValue<T>> dbKeyValues)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var dbValue in dbKeyValues)
			{
				sb.AppendLine("delete from " + dbValue.Column.Table + " where " + dbValue.Column.ColumnName + " = " + dbValue + ";");
			}
			return sb.ToString();
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
				dbColumn.Table = table;
				dbColumns.Add(dbColumn);
			}
			return dbColumns;
		}

		private DbColumn GetDbColumn(string table, IEnumerable<string> keyColumns)
		{
			IDatabaseMetadata databaseMetadata = new DatabaseMetadata(_sqlConnectionSource);

			var keyColumnsString = string.Join(" + ", keyColumns);

			Type typeColumn = typeof(string);
			if (keyColumns.Count() == 1)
			{
				typeColumn = databaseMetadata.GetColumnType(table, keyColumns.First());
			}
			var dbKeyColumn = new DbColumn(table, keyColumnsString, typeColumn);
			return dbKeyColumn;
		}
	}
}
