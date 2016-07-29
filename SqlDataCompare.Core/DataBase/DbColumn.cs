using System;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Колонка
	/// </summary>
	public class DbColumn
	{
		/// <summary>
		/// Таблица
		/// </summary>
		public string Table { get; set; }

		/// <summary>
		/// Имя колонки
		/// </summary>
		public string ColumnName { get; set; }
		/// <summary>
		/// Тип колонки
		/// </summary>
		public Type ColumnType { get; set; }


		public DbColumn()
		{}

		public DbColumn(string table,string columnName, Type columnType)
		{
			Table = table;
			ColumnName = columnName;
			ColumnType = columnType;
		}
	}
}
