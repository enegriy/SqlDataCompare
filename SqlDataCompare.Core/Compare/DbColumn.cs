using System;

namespace SqlDataCompare.Core
{
	public class DbColumn
	{
		/// <summary>
		/// Имя колонки
		/// </summary>
		public string ColumnName { get; set; }
		/// <summary>
		/// Тип колонки
		/// </summary>
		public Type ColumnType { get; set; }
	}
}
