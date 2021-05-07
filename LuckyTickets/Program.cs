using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyTickets
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			byte maxRank = 13;
			try
			{
				maxRank = byte.Parse(args[0]);
			}
			catch { }

			byte checkingRanks = 6;
			try
			{
				checkingRanks = byte.Parse(args[1]);
			}
			catch { }

			var useLogInTest = true;
			try
			{
				useLogInTest = byte.Parse(args[2]) == 1;
			}
			catch { }

			var useMultiThreadTest = true;
			try
			{
				useMultiThreadTest = byte.Parse(args[3]) == 1;
			}
			catch { }

			var useSingleThreadTest = false;
			try
			{
				useSingleThreadTest = byte.Parse(args[4]) == 1;
			}
			catch { }

			if
			(
				TestNumbersConvertation(useLog: useLogInTest)
				&& TestDigitsSum(useLog: useLogInTest)
				&& TestProcessingMethod(useMultiThreadTest: useMultiThreadTest, useSingleThreadTest: useSingleThreadTest, useLog: useLogInTest)
			)
			{
				_ = GetLuckyTicketsCount(usingDigits: Number13.Digits, digitsCount: maxRank, checkingRanks: checkingRanks, useLog: true);
			}
			else
			{
				Console.WriteLine("Провален один или более тестов. Включите вывод логов в консоль для отображения подробностей.");
			}

			Console.WriteLine();
			Console.WriteLine("Нажмите ENTER для закрытия приложения.");
			Console.ReadLine();
		}

		/// <summary>
		/// Метод решения задачи. Находит количество красивых чисел в заданной системе счисления, у которых сумма первых [checkingRanks] цифр слева равна сумме первых [checkingRanks] цифр справа.
		/// </summary>
		/// <param name="usingDigits">Цифры, используемые в системе счисления.</param>
		/// <param name="digitsCount">Количество цифр в проверяемых числах.</param>
		/// <param name="checkingRanks">Количество проверяемых 'разрядов' (цифр) слева и справа.</param>
		/// <param name="useLog">Использовать ли вывод в консоль для логов.</param>
		/// <returns>Результат решения задачи.</returns>
		private static BigInteger GetLuckyTicketsCount(IReadOnlyList<char> usingDigits, byte digitsCount, byte checkingRanks, bool useLog)
		{
			var numberBase = usingDigits.Count;

			if (useLog)
			{
				Console.WriteLine();
				Console.WriteLine("========================================");
				Console.WriteLine();
				Console.WriteLine($"Начинаем решать задачу поиска красивых чисел для {digitsCount}-значных чисел в {numberBase}-ичной системе счисления с проверкой по {checkingRanks} первых цифр слева и справа:");
			}

			var notCheckingRanks = digitsCount - checkingRanks * 2;
			if (notCheckingRanks < 0)
			{
				Console.WriteLine($"Ошибка: Аргумент '{nameof(digitsCount)}' должен быть минимум в 2 раза больше, чем аргумент '{nameof(checkingRanks)}'.");
				return -1;
			}

			var notCheckingMultiplier = 1;

			if (notCheckingRanks == 0)
			{
				if (useLog)
					Console.WriteLine("Обнаружено, что все цифры в числах будут проверяться. Дополнительная оптимизация неактуальна.");
			}
			else
			{
				notCheckingMultiplier = (int)Math.Pow(numberBase, notCheckingRanks);
				if (useLog)
					Console.WriteLine($"Обнаружено, что {notCheckingRanks} цифр(ы) в числах не будут проверяться. Следовательно, на каждую искомую проверяемую комбинацию у нас будет по {numberBase}^{notCheckingRanks} подходящих вариантов.");
			}

			if (useLog)
				Console.WriteLine($"Т.к. по заданию у нас все числа содержат {digitsCount} цифр (включая ведущие нули) и мы рассматриваем все без исключения такие числа, для каждой суммы первых {checkingRanks} цифр 'слева' гарантированно будет один или несколько подходящих вариантов расположения первых {checkingRanks} цифр 'справа' с такой же суммой цифр.");

			var numberX = new NumberX(0, usingDigits);
			var maxNumberX = new NumberX(new string(usingDigits[numberBase - 1], checkingRanks), usingDigits);

			var differentSumsCount = maxNumberX.GetDigitsSum() + 1; // не забываем про ноль.

			if (useLog)
			{
				Console.WriteLine($"Получается, что для решения задачи достаточно перебрать {numberBase}-ичные числа от '{numberX}' до '{maxNumberX}', сосчитать количество вариантов N для каждой суммы цифр 'слева' - для каждого из этих вариантов будет N вариантов искомых цифр 'справа', т.е. чтобы получить результирующее количество вариантов - нужно полученное значение возвести в квадрат.");
				Console.WriteLine($"Определили, что различных сумм цифр у нас будет {differentSumsCount}.");
			}

			var sums = new int[differentSumsCount];

			maxNumberX.IncrementAbs(); // чтобы последняя проверка с самим макс. числом тоже прошла
			while (numberX.ToString() != maxNumberX.ToString())
			{
				var digitsSum = numberX.GetDigitsSum();
				sums[digitsSum]++;
				numberX.IncrementAbs();
			}

			var result = new BigInteger(0);

			if (useLog)
				Console.WriteLine("Сосчитали количество сочетаний цифр для каждой из сумм цифр:");

			for (var sum = 0; sum < differentSumsCount; sum++)
			{
				var variantsCount = sums[sum];
				result += new BigInteger(variantsCount) * new BigInteger(variantsCount);

				if (useLog)
					Console.WriteLine($"- {sum}: {variantsCount}");
			}

			if (useLog)
				Console.WriteLine($"Итоговое количество искомых проверяемых комбинаций: {result}");

			if (notCheckingMultiplier != 1)
			{
				result *= new BigInteger(notCheckingMultiplier);
				if (useLog)
					Console.WriteLine($"Множитель дополнительной оптимизации, описанной выше: {notCheckingMultiplier}.");
			}

			if (useLog)
			{
				Console.WriteLine("----------------------------------------");
				Console.WriteLine($"Результат выполнения задачи: {result}");
				Console.WriteLine("----------------------------------------");
				Console.WriteLine();
				Console.WriteLine("========================================");
				Console.WriteLine();
			}

			return result;
		}

		/// <summary>
		/// Метод тестирования разрабатываемого метода решения задачи. 
		/// Да, тесты надо писать по-другому.
		/// </summary>
		/// <returns>Успешно ли прошёл тест.</returns>
		private static bool TestProcessingMethod(bool useMultiThreadTest, bool useSingleThreadTest, bool useLog)
		{
			if (!useMultiThreadTest && !useSingleThreadTest)
			{
				if (useLog)
					Console.WriteLine("Для тестирования требуется включить хотя бы один из методов проверки.");

				return false;
			}

			bool test(IReadOnlyList<char> usingDigits, byte digitsCount, byte checkingRanks)
			{
				if (useLog)
					Console.WriteLine($"Тестируем разрабатываемый метод с параметрами ({usingDigits.Count}, {digitsCount}, {checkingRanks}):");

				var myMethodResult = GetLuckyTicketsCount(usingDigits, digitsCount, checkingRanks, useLog: false);
				if (useLog)
					Console.WriteLine($"Разрабатываемый метод вернул результат: {myMethodResult}");

				var testMultiThreadMethodResult = useMultiThreadTest ? TestMultiThreadGetLuckyTicketsCount(usingDigits, digitsCount, checkingRanks, useLog) : - 1L;
				
				var testSingleThreadMethodResult = useSingleThreadTest ? TestSingleThreadGetLuckyTicketsCount(usingDigits, digitsCount, checkingRanks, useLog) : -1L;

				if ((useMultiThreadTest && testMultiThreadMethodResult != myMethodResult) || (useMultiThreadTest && useSingleThreadTest && testMultiThreadMethodResult != testSingleThreadMethodResult) || (useSingleThreadTest && myMethodResult != testSingleThreadMethodResult))
				{
					if (useLog)
						Console.WriteLine($"Тест ({usingDigits.Count}, {digitsCount}, {checkingRanks}) разрабатываемого метода ПРОВАЛЕН. [Тестируемый метод: {myMethodResult} | Многопоточный перебор: {testMultiThreadMethodResult} | Однопоточный перебор: {testSingleThreadMethodResult}]");

					return false;
				}

				if (useLog)
				{
					Console.WriteLine($"Тест ({usingDigits.Count}, {digitsCount}, {checkingRanks}) разрабатываемого метода успешно пройден. [Результат: {myMethodResult}]");
					Console.WriteLine();
				}

				return true;
			}

			return
				test(usingDigits: new List<char>() { '0', '1' }, digitsCount: 9, checkingRanks: 4) &&
				test(usingDigits: new List<char>() { '0', '1', '2' }, digitsCount: 7, checkingRanks: 3) &&
				test(usingDigits: new List<char>() { '0', '1', '2' }, digitsCount: 9, checkingRanks: 4) &&
				test(usingDigits: new List<char>() { '0', '1', '2' }, digitsCount: 13, checkingRanks: 6) &&
				test(usingDigits: Number13.Digits, digitsCount: 5, checkingRanks: 2);// &&
				//test(usingDigits: Number13.Digits, digitsCount: 7, checkingRanks: 3);
		}

		/// <summary>
		/// Метод поиска решения задачи перебором. Используется для тестирования разрабатываемого метода. 
		/// Да, тесты надо писать по-другому. 
		/// </summary>
		/// <param name="usingDigits">Используемые цифры в системе счисления.</param>
		/// <param name="digitsCount">Количество цифр в проверяемых числах.</param>
		/// <param name="checkingRanks">Количество проверяемых 'разрядов' (цифр) слева и справа.</param>
		/// <param name="useLog">Использовать ли вывод в консоль для логов.</param>
		/// <returns>Результат решения задачи.</returns>
		private static BigInteger TestSingleThreadGetLuckyTicketsCount(IReadOnlyList<char> usingDigits, int digitsCount, byte checkingRanks, bool useLog)
		{
			if (useLog)
				Console.WriteLine($"Тестируем однопоточным перебором с параметрами ({usingDigits.Count}, {digitsCount}, {checkingRanks})...");

			var luckyTicketCount = new BigInteger(0);

			var numberX = new NumberX(0, usingDigits);
			var maxNumberX = new NumberX(new string(usingDigits[usingDigits.Count - 1], digitsCount), usingDigits);
			maxNumberX.IncrementAbs();
			while (numberX != maxNumberX)
			{
				var delta = 0;
				for (var rank = 0; rank < checkingRanks; rank++)
				{
					delta += numberX.ToInteger(rank) - numberX.ToInteger(digitsCount - 1 - rank);
				}

				if (delta == 0)
				{
					luckyTicketCount++;
				}

				numberX.IncrementAbs();
			}

			if (useLog)
			{
				Console.WriteLine($"[Тестовый однопоточный перебор] Результат: {luckyTicketCount}");
			}

			return luckyTicketCount;
		}

		/// <summary>
		/// Метод поиска решения задачи перебором. Используется для тестирования разрабатываемого метода. 
		/// Да, тесты надо писать по-другому. 
		/// </summary>
		/// <param name="usingDigits">Используемые цифры в системе счисления.</param>
		/// <param name="digitsCount">Количество цифр в проверяемых числах.</param>
		/// <param name="checkingRanks">Количество проверяемых 'разрядов' (цифр) слева и справа.</param>
		/// <param name="useLog">Использовать ли вывод в консоль для логов.</param>
		/// <returns>Результат решения задачи.</returns>
		private static BigInteger TestMultiThreadGetLuckyTicketsCount(IReadOnlyList<char> usingDigits, int digitsCount, byte checkingRanks, bool useLog)
		{
			if (useLog)
				Console.WriteLine($"Тестируем многопоточным перебором с параметрами ({usingDigits.Count}, {digitsCount}, {checkingRanks})...");

			var minNumberIncl = 0L;
			var maxNumberNotIncl = new NumberX(new string(usingDigits[usingDigits.Count - 1], digitsCount), usingDigits).ToInteger() + 1;
			if (maxNumberNotIncl < 0)
				throw new Exception();

			var luckyTicketCount = new BigInteger(0);
			var resultLocker = new object();

			var task = Parallel.For(minNumberIncl, maxNumberNotIncl, base10Number =>
			{
				var baseXNumber = new NumberX(base10Number, usingDigits);
				var delta = 0;
				for (var rank = 0; rank < checkingRanks; rank++)
				{
					delta += baseXNumber.ToInteger(rank) - baseXNumber.ToInteger(digitsCount - 1 - rank);
				}

				if (delta == 0)
				{
					lock (resultLocker)
					{
						luckyTicketCount++;
					}
				}
			});

			if (useLog)
			{
				Console.WriteLine($"[Тестовый многопоточный перебор] Результат: {luckyTicketCount}");
			}

			return luckyTicketCount;
		}

		/// <summary>
		/// Метод проверки корректности конвертации различных чисел в 13-ричную систему счисления и обратно. 
		/// Да, тесты надо писать по-другому. 
		/// </summary>
		/// <param name="useLog">Использовать ли вывод в консоль для логов.</param>
		/// <returns>Успешно ли прошёл тест.</returns>
		private static bool TestNumbersConvertation(bool useLog)
		{
			var base10numbers = new int[] { 0, 9, 10, 12, 13, 14, 100, 200, 1000, -100, -200 };

			if (useLog)
				Console.WriteLine($"Тестируем перевод {base10numbers.Length} десятичных чисел в 13-ричную систему и обратно:");

			foreach (var base10Number in base10numbers)
			{
				var base13Number = new Number13(base10Number);
				var convertedBase10Number = base13Number.ToInteger();

				if (useLog)
					Console.WriteLine($"Тест: {base10Number} >>> {base13Number} >>> {convertedBase10Number}");

				if (base10Number != convertedBase10Number)
				{
					if (useLog)
					{
						Console.WriteLine("Тест конвертации чисел ПРОВАЛЕН.");
						Console.WriteLine();
					}
					return false;
				}
			}

			if (useLog)
			{
				Console.WriteLine("Тест конвертации чисел успешно пройден.");
				Console.WriteLine();
			}

			return true;
		}

		/// <summary>
		/// Метод проверки корректности вычисления суммы цифр чисел в 13-ричной системе счисления. 
		/// Да, тесты надо писать по-другому. 
		/// </summary>
		/// <param name="useLog">Использовать ли вывод в консоль для логов.</param>
		/// <returns>Успешно ли прошёл тест.</returns>
		private static bool TestDigitsSum(bool useLog)
		{
			if 
			( 
				new Number13("0").GetDigitsSum() != 0 
				|| new Number13("A").GetDigitsSum() != 10
				|| new Number13("C").GetDigitsSum() != 12
				|| new Number13("10").GetDigitsSum() != 1
				|| new Number13("1A").GetDigitsSum() != 11
				|| new Number13("AA").GetDigitsSum() != 20
				|| new Number13("CC").GetDigitsSum() != 24
				|| new Number13("CCC").GetDigitsSum() != 36
				|| new Number13("CCC1").GetDigitsSum() != 37
				|| new Number13("1CCC1").GetDigitsSum() != 38
			)
			{
				if (useLog)
					Console.WriteLine($"Тест вычисления суммы цифр ПРОВАЛЕН.");

				return false;
			}

			if (useLog)
			{
				Console.WriteLine("Тест суммы цифр успешно пройден.");
				Console.WriteLine();
			}

			return true;
		}
	}
}
