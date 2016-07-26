using System.Collections.Generic;

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
				$" SELECT TABLE_SCHEMA + '.' + TABLE_NAME FROM [{_sqlConnection.Database}].INFORMATION_SCHEMA.TABLES ";
			return _selectManager.SelectFirstColumn<string>(sqlQueryGetTables);
		}

		public IEnumerable<string> GetColumnsByTable(string table)
		{
			var sqlQueryGetTables =
				$" SELECT COLUMN_NAME FROM [{_sqlConnection.Database}].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA + '.' + TABLE_NAME = '{table}' ";
			return _selectManager.SelectFirstColumn<string>(sqlQueryGetTables);
		}

		public IEnumerable<string> GetKeyColumnsByTable(string table)
		{
			var sqlQueryGetTables =
				$" SELECT COLUMN_NAME FROM [{_sqlConnection.Database}].INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE TABLE_SCHEMA + '.' + TABLE_NAME = '{table}' and CONSTRAINT_NAME like 'PK_%' ";
			return _selectManager.SelectFirstColumn<string>(sqlQueryGetTables);
		}


	}
}
