using System;
using System.Globalization;

namespace SqlDataCompare.Core
{
	/// <summary>
	/// Значение из БД
	/// </summary>
	public class DbValue<T>
	{
		/// <summary>
		/// Значение
		/// </summary>
		public T Value { get; set; }
		/// <summary>
		/// Колонка
		/// </summary>
		public DbColumn Column { get; set; }

		public DbValue(T value, DbColumn column)
		{
			Value = value;
			Column = column;
		}

		public override string ToString()
		{
			var type = Column.ColumnType;

			if (type == typeof(string) ||
				type == typeof(Guid))
				return "N'" + Value.ToString() + "'";

			if (type == typeof(int))
				return Value.ToString();

			if (type == typeof (decimal))
				return decimal.Parse(Value.ToString()).ToString(CultureInfo.InvariantCulture);

			return string.Empty;
		}
	}

	public class DbValue : DbValue<Object>
	{
		public DbValue(object value, DbColumn column) : base(value, column)
		{
		}
	}
}
