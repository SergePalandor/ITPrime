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
		private string _value;

		/// <summary>
		/// Количество цифр в числе.
		/// </summary>
		public int DigitsCount { get; private set; }

		/// <summary>
		/// Максимальный разряд в числе (0 - 'единицы', 1 - 'десятки', 2 - 'сотни' и т.д.).
		/// </summary>
		public int MaxRank { get; private set; }

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
		public NumberX(long number, IReadOnlyList<char> usingDigits)
		{
			this.UsingDigits = new List<char>(usingDigits);

			if (this.UsingDigits == null)
				throw new ArgumentNullException(nameof(usingDigits));

			if (this.UsingDigits.Count < 1)
				throw new ArgumentException("Для определения базы системы счисления необходимо передать список символов, используемых в качестве цифр.", nameof(usingDigits));

			this.NumberBase = this.UsingDigits.Count;

			if (number == 0)
			{
				_value = this.UsingDigits[0].ToString();
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
					var base10DigitValue = (int)(number % numberBase);
					if (this.UsingDigits.Count <= base10DigitValue)
						throw new InvalidCastException($"В системе счисления с базой '{this.NumberBase}' не задана цифра, описывающая десятичное значение '{base10DigitValue}'.");

					sb.Insert(0, this.UsingDigits[base10DigitValue]);
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

			this.UsingDigits = new List<char>(usingDigits);

			if (this.UsingDigits == null)
				throw new ArgumentNullException(nameof(usingDigits));

			if (this.UsingDigits.Count < 1)
				throw new ArgumentException("Для определения базы системы счисления необходимо передать список символов, используемых в качестве цифр.", nameof(usingDigits));

			this.NumberBase = this.UsingDigits.Count;

			if (value[0] == '-')
			{
				this.IsNegative = true;
				value = value[1..];
			}

			foreach (var digit in value)
			{
				if (!this.UsingDigits.Contains(digit))
					throw new ArgumentException($"В системе счисления с базой '{this.NumberBase}' не используется цифра '{digit}'.");
			}

			_value = value;
			this.DigitsCount = _value.Length;
			this.MaxRank = this.DigitsCount - 1;
		}

		/// <summary>
		/// Метод получения цифры по её разряду.
		/// </summary>
		/// <param name="rank">Разряд, где 0 - 'единицы', 1 - 'десятки', 2 - 'сотни' и т.д.</param>
		/// <returns>Искомая цифра.</returns>
		public char GetDigit(int rank) => rank > this.MaxRank ? this.UsingDigits[0] : _value[this.MaxRank - rank];

		/// <summary>
		/// Метод перевода числа в десятичную систему счисления.
		/// </summary>
		public long ToInteger()
		{
			var result = 0L;
			var multiplier = 1L;
			for (var rank = 0; rank <= this.MaxRank; rank++)
			{
				var base10DigitValue = this.ToInteger(rank);
				var val = base10DigitValue * multiplier;
				if (long.MaxValue - result < val)
					throw new OverflowException($"Число '{this}' в {this.NumberBase}-ичной системе счисления нельзя привести к long, т.к. оно больше long.MaxValue.");

				result += val;
				multiplier *= this.NumberBase;
			}

			if (this.IsNegative)
				result = -result;

			return result;
		}

		/// <summary>
		/// Метод перевода цифры заданного разряда (0 - 'единицы', 1 - 'десятки' и т.д.) в десятичную систему счисления.
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

		public void IncrementAbs()
		{
			for (var rank = 0; rank <= this.MaxRank; rank++)
			{
				var base10DigitValue = this.ToInteger(rank) + 1;
				var arr = _value.Reverse().ToArray();
				if (base10DigitValue == this.NumberBase)
				{
					arr[rank] = this.UsingDigits[0];
					arr = arr.Reverse().ToArray();
					if (rank == this.MaxRank)
					{
						_value = this.UsingDigits[1].ToString() + new string(arr);
						this.MaxRank++;
						this.DigitsCount++;
						break;
					}
					else
					{
						_value = new string(arr);
					}
				}
				else
				{
					arr[rank] = this.UsingDigits[base10DigitValue];
					arr = arr.Reverse().ToArray();
					_value = new string(arr);
					break;
				}
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is NumberX numberX)
				return this == numberX;

			if (obj is sbyte int8)
				return this == new NumberX(int8, this.UsingDigits);
			
			if (obj is byte uint8)
				return this == new NumberX(uint8, this.UsingDigits);
			
			if (obj is short int16)
				return this == new NumberX(int16, this.UsingDigits);

			if (obj is ushort uint16)
				return this == new NumberX(uint16, this.UsingDigits);

			if (obj is int int32)
				return this == new NumberX(int32, this.UsingDigits);

			if (obj is uint uint32)
				return this == new NumberX(uint32, this.UsingDigits);

			if (obj is long int64)
				return this == new NumberX(int64, this.UsingDigits);

			if(obj is ulong uint64 && uint64 <= long.MaxValue)
				return this == new NumberX((long)uint64, this.UsingDigits);

			return false;
		}

		public override int GetHashCode() => $"{this.NumberBase}_{this}".GetHashCode();

		public static bool operator ==(NumberX a, NumberX b) => a.NumberBase == b.NumberBase && a.IsNegative == b.IsNegative && a._value == b._value;

		public static bool operator !=(NumberX a, NumberX b) => !(a == b);
	}
}
