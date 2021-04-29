using System;
using System.Collections.Generic;
using System.Linq;

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

			var useLogInTest = false;

			if (TestNumbersConvertation(useLog: useLogInTest) && TestProcessingMethod(useLog: useLogInTest))
				_ = GetLuckyTicketsCount(usingDigits: Number13.Digits, maxRank: maxRank, checkingRanks: checkingRanks, useLog: true);
			else
				Console.WriteLine("Провален один или более тестов. Включите логирование для отображения подробностей.");

			Console.WriteLine();
			Console.WriteLine("Нажмите ENTER для закрытия приложения.");
			Console.ReadLine();
		}

		/// <summary>
		/// Метод решения задачи. Находит количество красивых чисел в заданной системе счисления, у которых сумма первых [checkingRanks] цифр слева равна сумме первых [checkingRanks] цифр справа.
		/// </summary>
		/// <param name="usingDigits">Цифры, используемые в системе счисления.</param>
		/// <param name="maxRank">Количество цифр в проверяемых числах.</param>
		/// <param name="checkingRanks">Количество проверяемых 'разрядов' (цифр) слева и справа.</param>
		/// <param name="useLog">Использовать ли вывод в консоль для логов.</param>
		/// <returns>Результат решения задачи.</returns>
		private static long GetLuckyTicketsCount(IReadOnlyList<char> usingDigits, byte maxRank, byte checkingRanks, bool useLog)
		{
			var numberBase = usingDigits.Count;

			if (useLog)
			{
				Console.WriteLine();
				Console.WriteLine("========================================");
				Console.WriteLine($"Начинаем решать задачу поиска красивых чисел для {maxRank}-значных чисел в {numberBase}-ичной системе счисления с проверкой по {checkingRanks} первых цифр слева и справа:");
			}

			var notCheckingRanks = maxRank - checkingRanks * 2;
			if (notCheckingRanks < 0)
			{
				Console.WriteLine($"Ошибка: Аргумент '{nameof(maxRank)}' должен быть минимум в 2 раза больше, чем аргумент '{nameof(checkingRanks)}'.");
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
				Console.WriteLine($"Т.к. по заданию у нас все числа содержат {maxRank} цифр (включая ведущие нули) и мы рассматриваем все без исключения такие числа, для каждой суммы первых {checkingRanks} цифр 'слева' гарантированно будет один или несколько подходящих вариантов расположения первых {checkingRanks} цифр 'справа' с такой же суммой цифр.");

			var minNumber10 = 0;
			var minNumberX = new NumberX(minNumber10, usingDigits);
			var maxNumberX = new NumberX(new string(usingDigits[numberBase - 1], checkingRanks), usingDigits);
			var maxNumber10 = maxNumberX.ToInteger();

			var differentSumsCount = maxNumberX.GetDigitsSum() + 1; // не забываем про ноль.

			if (useLog)
			{
				Console.WriteLine($"Получается, что для решения задачи достаточно перебрать {numberBase}-ичные числа от '{minNumberX}' до '{maxNumberX}', сосчитать количество вариантов N для каждой суммы цифр 'слева' - для каждого из этих вариантов будет N вариантов искомых цифр 'справа', т.е. чтобы получить результирующее количество вариантов - нужно полученное значение возвести в квадрат.");
				Console.WriteLine($"Определили, что различных сумм цифр у нас будет {differentSumsCount}.");
			}

			var sums = new int[differentSumsCount];

			for (var number10 = minNumber10; number10 <= maxNumber10; number10++)
			{
				var numberX = new NumberX(number10, usingDigits);
				var digitsSum = numberX.GetDigitsSum();
				sums[digitsSum]++;
			}

			long result = 0;

			if (useLog)
				Console.WriteLine("Сосчитали количество сочтаний цифр для каждой из сумм цифр:");

			for (var sum = 0; sum < differentSumsCount; sum++)
			{
				var variantsCount = sums[sum];
				result += variantsCount * variantsCount;

				if (useLog)
					Console.WriteLine($"- {sum}: {variantsCount}");
			}

			if (useLog)
				Console.WriteLine($"Итоговое количество искомых проверяемых комбинаций: {result}");

			if (notCheckingMultiplier != 1)
			{
				result *= notCheckingMultiplier;
				if (useLog)
					Console.WriteLine($"Множитель дополнительной оптимизации, описанной выше: {notCheckingMultiplier}.");
			}

			if (useLog)
			{
				Console.WriteLine("----------------------------------------");
				Console.WriteLine($"Результат выполнения задачи: {result}");
				Console.WriteLine("----------------------------------------");
				Console.WriteLine();
			}

			return result;
		}

		/// <summary>
		/// Метод тестирования разрабатываемого метода решения задачи. 
		/// Да, тесты надо писать по-другому. 
		/// Да, в этом методе я не следую принципу DRY.
		/// </summary>
		/// <returns>Успешно ли прошёл тест.</returns>
		private static bool TestProcessingMethod(bool useLog)
		{
			var count1 = TestGetLuckyTicketsCount(usingDigits: new List<char>() { '0', '1' }, maxRank: 7, checkingRanks1: new[] { 0, 1 }, checkingRanks2: new[] { 2, 3 }, useLog: useLog);
			if (useLog)
				Console.WriteLine();

			var count2 = GetLuckyTicketsCount(usingDigits: new List<char>() { '0', '1' }, maxRank: 7, checkingRanks: 2, useLog: useLog);
			if (useLog)
				Console.WriteLine();

			if (count1 != count2)
			{
				if (useLog)
					Console.WriteLine("Тест №1 разрабатываемого метода ПРОВАЛЕН.");

				return false;
			}

			count1 = TestGetLuckyTicketsCount(usingDigits: new List<char>() { '0', '1', '2' }, maxRank: 7, checkingRanks1: new[] { 0, 1, 2 }, checkingRanks2: new[] { 4, 5, 6 }, useLog: useLog);
			if (useLog)
				Console.WriteLine();

			count2 = GetLuckyTicketsCount(usingDigits: new List<char>() { '0', '1', '2' }, maxRank: 7, checkingRanks: 3, useLog: useLog);
			if (useLog)
				Console.WriteLine();

			if (count1 != count2)
			{
				if (useLog)
					Console.WriteLine("Тест №2 разрабатываемого метода ПРОВАЛЕН.");

				return false;
			}

			if (useLog)
				Console.WriteLine("Тесты разрабатываемого метода пройдены успешно.");

			return true;
		}

		/// <summary>
		/// Метод поиска решения задачи перебором. Используется для тестирования разрабатываемого метода. 
		/// Да, тесты надо писать по-другому. 
		/// </summary>
		/// <param name="usingDigits">Используемые цифры в системе счисления.</param>
		/// <param name="maxRank">Количество цифр в проверяемых числах.</param>
		/// <param name="checkingRanks1">Массив проверяемых 'порядков' (цифр) №1.</param>
		/// <param name="checkingRanks2">Массив проверяемых 'порядков' (цифр) №2.</param>
		/// <param name="useLog">Использовать ли вывод в консоль для логов.</param>
		/// <returns>Результат решения задачи.</returns>
		private static long TestGetLuckyTicketsCount(IReadOnlyList<char> usingDigits, int maxRank, int[] checkingRanks1, int[] checkingRanks2, bool useLog)
		{
			var minNumberIncl = 0;
			var maxNumberNotIncl = new NumberX(new string(usingDigits[usingDigits.Count - 1], maxRank), usingDigits).ToInteger() + 1;
			NumberX baseXNumber;
			var luckyTicketCount = 0;

			var luckyTickets = new List<string>();

			for (var base10Number = minNumberIncl; base10Number < maxNumberNotIncl; base10Number++)
			{
				baseXNumber = new NumberX(base10Number, usingDigits);
				if (checkingRanks1.Sum(rank => baseXNumber.ToInteger(rank)) == checkingRanks2.Sum(rank => baseXNumber.ToInteger(rank)))
				{
					var baseXNumberStr = baseXNumber.ToString();
					luckyTickets.Add(baseXNumberStr);
					//Console.WriteLine($"Найдено красивое число: {baseXNumberStr}");
					luckyTicketCount++;
				}
			}

			if (useLog)
			{
				Console.WriteLine($"[Тестовый метод] Всего найдено красивых {maxRank}-тизначных чисел в {usingDigits.Count}-ричной системе счисления: {luckyTicketCount}");
				Console.WriteLine();
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
	}
}
