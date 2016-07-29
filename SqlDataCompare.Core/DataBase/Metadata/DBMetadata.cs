using System;
using System.Collections.Generic;
using System.Linq;
using SqlDataCompare.Core.Select;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Провайдер для базы данных
	/// </summary>
	public class DbMetadata : IDbMetadata
	{
		private ISqlConnection Connection { get; set; }
		private ISelectManager SelectManager { get; set; }

		/// <summary>
		/// .Ctor
		/// </summary>
		public DbMetadata( ISelectManager selectManager)
		{
			Connection = selectManager.Connection;
			SelectManager = selectManager;
		}

		public IEnumerable<string> GetTables()
		{
			var sqlQueryGetTables =
				$" SELECT '['+TABLE_SCHEMA + '].[' + TABLE_NAME+']' FROM [{Connection.Database}].INFORMATION_SCHEMA.TABLES ";
			return SelectManager.SelectFirstColumn<string>(sqlQueryGetTables);
		}

		public IEnumerable<string> GetColumnsByTable(string table)
		{
			var sqlQueryGetTables =
				$" SELECT '['+COLUMN_NAME+']' FROM [{Connection.Database}].INFORMATION_SCHEMA.COLUMNS WHERE '['+TABLE_SCHEMA + '].[' + TABLE_NAME+']' = '{table}' ";
			return SelectManager.SelectFirstColumn<string>(sqlQueryGetTables);
		}

		public IEnumerable<string> GetKeyColumnsByTable(string table)
		{
			var sqlQueryGetTables =
				$" SELECT '['+COLUMN_NAME+']' FROM [{Connection.Database}].INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE '['+TABLE_SCHEMA + '].[' + TABLE_NAME+']' = '{table}' and CONSTRAINT_NAME like 'PK_%' ";
			return SelectManager.SelectFirstColumn<string>(sqlQueryGetTables);
		}

		/// <summary>
		/// Тип колонки
		/// </summary>
		public Type GetColumnType(string table, string column)
		{
			var sql = $" SELECT DATA_TYPE FROM {Connection.Database}.INFORMATION_SCHEMA.COLUMNS where '['+TABLE_SCHEMA + '].[' + TABLE_NAME+']' = '{table}' AND '['+COLUMN_NAME+']' = '{column}'";
			var column_type = SelectManager.SelectFirstColumn<string>(sql).FirstOrDefault();

			if (column_type.Equals("uniqueidentifier"))
				return typeof (Guid);
			if(column_type.Equals("int"))
				return typeof(int);
			if (column_type.Equals("nvarchar"))
				return typeof(string);
			if (column_type.Equals("decimal"))
				return typeof (decimal);

			return typeof (string);
		}

	}
}
