using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Провайдер для базы данных
	/// </summary>
	public class DatabaseMetadata : IDatabaseMetadata
	{
		private readonly ISqlConnection _sqlConnection;
		private readonly SelectManager _selectManager;

		/// <summary>
		/// .Ctor
		/// </summary>
		public DatabaseMetadata(ISqlConnection sqlConnection)
		{
			_sqlConnection = sqlConnection;
			_selectManager = new SelectManager(_sqlConnection);
		}

		public IEnumerable<string> GetTables()
		{
			var sqlQueryGetTables =
				$" SELECT '['+TABLE_SCHEMA + '].[' + TABLE_NAME+']' FROM [{_sqlConnection.Database}].INFORMATION_SCHEMA.TABLES ";
			return _selectManager.SelectFirstColumn<string>(sqlQueryGetTables);
		}

		public IEnumerable<string> GetColumnsByTable(string table)
		{
			var sqlQueryGetTables =
				$" SELECT '['+COLUMN_NAME+']' FROM [{_sqlConnection.Database}].INFORMATION_SCHEMA.COLUMNS WHERE '['+TABLE_SCHEMA + '].[' + TABLE_NAME+']' = '{table}' ";
			return _selectManager.SelectFirstColumn<string>(sqlQueryGetTables);
		}

		public IEnumerable<string> GetKeyColumnsByTable(string table)
		{
			var sqlQueryGetTables =
				$" SELECT '['+COLUMN_NAME+']' FROM [{_sqlConnection.Database}].INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE '['+TABLE_SCHEMA + '].[' + TABLE_NAME+']' = '{table}' and CONSTRAINT_NAME like 'PK_%' ";
			return _selectManager.SelectFirstColumn<string>(sqlQueryGetTables);
		}

		/// <summary>
		/// Тип колонки
		/// </summary>
		public Type GetColumnType(string table, string column)
		{
			var sql =
				$" SELECT DATA_TYPE FROM [RI_BankGuarantee].INFORMATION_SCHEMA.COLUMNS where '['+TABLE_SCHEMA + '].[' + TABLE_NAME+']' = '{table}' AND '['+COLUMN_NAME+']' = '{column}'";
			var column_type =  _selectManager.SelectFirstColumn<string>(sql).FirstOrDefault();

			if (column_type.Equals("uniqueidentifier"))
				return typeof (Guid);
			if(column_type.Equals("int"))
				return typeof(int);
			if (column_type.Equals("nvarchar"))
				return typeof(string);

			return typeof (string);
		}

	}
}
