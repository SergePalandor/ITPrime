using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuckyTickets
{
	/// <summary>
	/// Класс, позволяющий работать с числами в различных системах счисления.
	/// </summary>
	public class NumberX
	{
		/// <summary>
		/// Хранимое значение.
		/// </summary>
		private readonly string _value;

		/// <summary>
		/// Количество цифр в числе.
		/// </summary>
		public int DigitsCount { get; }

		/// <summary>
		/// Максимальный порядок в числе (0 - 'единицы', 1 - 'десятки', 2 - 'сотни' и т.д.).
		/// </summary>
		public int MaxRank { get; }

		/// <summary>
		/// Является ли число отрицательным.
		/// </summary>
		public bool IsNegative { get; }

		/// <summary>
		/// База системы счисления.
		/// </summary>
		protected int NumberBase { get; }

		/// <summary>
		/// Цифры, используемые в заданной системе счисления.
		/// </summary>
		public IReadOnlyList<char> UsingDigits { get; }

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="number">Число в десятичной системе счисления.</param>
		public NumberX(int number, IReadOnlyList<char> usingDigits)
		{
			if (usingDigits == null)
				throw new ArgumentNullException(nameof(usingDigits));

			if (usingDigits.Count < 1)
				throw new ArgumentException("Для определения базы системы счисления необходимо передать список символов, используемых в качестве цифр.", nameof(usingDigits));

			this.UsingDigits = usingDigits;
			this.NumberBase = this.UsingDigits.Count;

			if (number == 0)
			{
				_value = usingDigits[0].ToString();
			}
			else
			{
				var numberBase = this.NumberBase;
				var sb = new StringBuilder(32);

				if (number < 0)
				{
					this.IsNegative = true;
					number = -number;
				}

				while (number > 0)
				{
					var base10DigitValue = number % numberBase;
					if (usingDigits.Count <= base10DigitValue)
						throw new InvalidCastException($"В системе счисления с базой '{this.NumberBase}' не задана цифра, описывающая десятичное значение '{base10DigitValue}'.");

					sb.Insert(0, usingDigits[base10DigitValue]);
					number /= numberBase;
				}

				_value = sb.ToString();
			}

			this.DigitsCount = _value.Length;
			this.MaxRank = this.DigitsCount - 1;
		}

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="value">Число в заданной системе счисления.</param>
		public NumberX(string value, IReadOnlyList<char> usingDigits)
		{
			if (string.IsNullOrWhiteSpace(value) || value == "-")
				throw new ArgumentException(message: "Требуется задать корректное значение числа.", nameof(value));

			if (usingDigits == null)
				throw new ArgumentNullException(nameof(usingDigits));

			if (usingDigits.Count < 1)
				throw new ArgumentException("Для определения базы системы счисления необходимо передать список символов, используемых в качестве цифр.", nameof(usingDigits));

			this.UsingDigits = usingDigits;
			this.NumberBase = this.UsingDigits.Count;

			if (value[0] == '-')
			{
				this.IsNegative = true;
				value = value[1..];
			}

			foreach (var digit in value)
			{
				if (!usingDigits.Contains(digit))
					throw new ArgumentException($"В системе счисления с базой '{this.NumberBase}' не используется цифра '{digit}'.");
			}

			_value = value;
			this.DigitsCount = _value.Length;
			this.MaxRank = this.DigitsCount - 1;
		}

		/// <summary>
		/// Метод получения цифры по порядку.
		/// </summary>
		/// <param name="rank">Порядок, где 0 - 'единицы', 1 - 'десятки', 2 - 'сотни' и т.д.</param>
		/// <returns>Искомая цифра.</returns>
		public char GetDigit(int rank) => rank > this.MaxRank ? this.UsingDigits[0] : _value[this.MaxRank - rank];

		/// <summary>
		/// Метод перевода числа в десятичную систему счисления.
		/// </summary>
		public long ToInteger()
		{
			var result = 0L;
			var multiplier = 1;
			for (var rank = 0; rank <= this.MaxRank; rank++)
			{
				var base10DigitValue = this.ToInteger(rank);
				result += base10DigitValue * multiplier;
				multiplier *= this.NumberBase;
			}

			if (this.IsNegative)
				result = -result;

			return result;
		}

		/// <summary>
		/// Метод перевода цифры заданного порядка (0 - 'единицы', 1 - 'десятки' и т.д.) в десятичную систему счисления.
		/// </summary>
		public int ToInteger(int rank)
		{
			var digit = this.GetDigit(rank);
			var base10DigitValue = -1;
			for (var i = 0; i < this.NumberBase; i++) // IndexOf - наше всё :)
			{
				if (this.UsingDigits[i] == digit)
				{
					base10DigitValue = i;
					break;
				}
			}

			if (base10DigitValue < 0)
				throw new InvalidCastException($"Цифра '{digit}' не используется в системе счисления с базой '{this.NumberBase}'.");

			return base10DigitValue;
		}

		/// <summary>
		/// Метод представления числа в виде строки.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{(this.IsNegative ? "-" : string.Empty)}{_value}";

		/// <summary>
		/// Метод получения суммы цифр в числе.
		/// </summary>
		public int GetDigitsSum()
		{
			var result = 0;

			for (var rank = 0; rank <= this.MaxRank; rank++)
			{
				var base10DigitValue = this.ToInteger(rank);
				result += base10DigitValue;
			}

			return result;
		}
	}
}
