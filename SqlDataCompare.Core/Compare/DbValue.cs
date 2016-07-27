namespace SqlDataCompare.Core
{
	/// <summary>
	/// Значение из БД
	/// </summary>
	public class DbValue
	{
		/// <summary>
		/// Значение
		/// </summary>
		public object Value { get; set; }
		/// <summary>
		/// Колонка
		/// </summary>
		public DbColumn Column { get; set; }
	}
}
