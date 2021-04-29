using System.Collections.Generic;

namespace LuckyTickets
{
	/// <summary>
	/// Класс, позволяющий работать с числами в различных системах счисления.
	/// </summary>
	public sealed class Number13 : NumberX
	{
		/// <summary>
		/// Цифры, используемые в заданной системе счисления.
		/// </summary>
		/// <remarks>
		/// Можно было бы взять за стандарт использование классических цифр (0-9, A, B, C, ...), 
		/// но задавая их вручную мы можем написать любую свою систему счисления, использующую 
		/// любые символы для записи цифр.
		/// </remarks>
		public static readonly IReadOnlyList<char> Digits = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C' };

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="number">Число в десятичной системе счисления.</param>
		public Number13(int number) : base(number, Digits)
		{
		}

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="value">Число в 13-ричной системе счисления.</param>
		public Number13(string value) : base(value, Digits)
		{
		}
	}
}
