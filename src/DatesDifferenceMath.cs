using System;
using System.Collections.Generic;

namespace RD_AAOW
	{
	/// <summary>
	/// Варианты приращения даты
	/// </summary>
	public enum DDIncrements
		{
		/// <summary>
		/// Секунды
		/// </summary>
		Seconds = 0,

		/// <summary>
		/// Минуты
		/// </summary>
		Minutes = 1,

		/// <summary>
		/// Часы
		/// </summary>
		Hours = 2,

		/// <summary>
		/// Дни
		/// </summary>
		Days = 3,

		/// <summary>
		/// Недели
		/// </summary>
		Weeks = 4,
		}

	/// <summary>
	/// Класс описывает математику приложения
	/// </summary>
	public static class DDMath
		{
		/// <summary>
		/// Метод формирует представление разницы между датами в текстовом виде
		/// </summary>
		/// <param name="First">Первая дата</param>
		/// <param name="Second">Вторая дата</param>
		/// <returns>Текстовое представление разницы в виде восьми (8) строк в представлениях:
		/// 1. Полное
		/// 2. В секундах
		/// 3. В минутах
		/// 4. В часах
		/// 5. В днях
		/// 6. В неделях
		/// 7. В целых месяцах
		/// 8. В целых годах</returns>
		public static string GetDifferencePresentation (DateTime First, DateTime Second)
			{
			// Инициализация
			DateTime start, end;
			if (First > Second)
				{
				end = First;
				start = Second;
				}
			else
				{
				end = Second;
				start = First;
				}

			// Вычисление
			TimeSpan diff = end - start;
			double seconds = (((ulong)diff.Days * 24 + (ulong)diff.Hours) * 60 + (ulong)diff.Minutes) * 60 +
				(ulong)diff.Seconds;

			// Секунды, минуты, часы, дни, недели
			/*string[] resV37 = new string[] { "", "", "", "", "", "", "", "" };*/

			List<List<string>> notAlignedFractions = [];
			for (int i = 0; i < fractionsDividers.Length; i++)
				{
				notAlignedFractions.Add ([]);
				notAlignedFractions[i].Add (((long)seconds).ToString ("#,#0"));

				if (i == 0)
					{
					notAlignedFractions[i].Add ("");
					}
				else
					{
					notAlignedFractions[i][0] = notAlignedFractions[i][0].PadLeft (notAlignedFractions[0][0].Length);
					notAlignedFractions[i].Add ((seconds - (long)seconds).ToString (fractionsFormats[i]));
					}

				seconds /= fractionsDividers[i];
				notAlignedFractions[i][0] = notAlignedFractions[i][0].Replace ('\xA0', ' ');
				}

			// Месяцы, годы
			ulong startMonthOffset = (ulong)(((start.Day * 24 + start.Hour) * 60 + start.Minute) * 60 + start.Second);
			ulong endMonthOffset = (ulong)(((end.Day * 24 + end.Hour) * 60 + end.Minute) * 60 + end.Second);

			ulong months = (ulong)(end.Year - start.Year) * 12 + (ulong)(end.Month - start.Month);
			if ((months > 0) && (endMonthOffset < startMonthOffset))
				months--;

			string fullMonthYears = months.ToString () +
				" (" + (months / 12).ToString () + " × 12 + " + (months % 12).ToString () + ")" + RDLocale.RN;
			fullMonthYears += (months / 12).ToString ();

			// Полный формат
			string res = string.Format (RDLocale.GetText ("FullFormat"), diff.Days, diff.Hours,
				diff.Minutes, diff.Seconds) + RDLocale.RN;

			for (int i = 0; i < notAlignedFractions.Count; i++)
				res += (notAlignedFractions[i][0] + notAlignedFractions[i][1] + RDLocale.RN);

			res += fullMonthYears;

			// Завершено
			return res;
			}
		private static string[] fractionsFormats = [
			"",
			"#.0##",
			"#.0####",
			"#.0#####",
			"#.0######",
			];
		private static double[] fractionsDividers = [
			60.0,
			60.0,
			24.0,
			7.0,
			1.0,
			];

		/// <summary>
		/// Метод добавляет указанное значение заданного типа к дате
		/// </summary>
		/// <param name="OldTime">Старое значение даты</param>
		/// <param name="Type">Тип приращения</param>
		/// <param name="Value">Величина приращения</param>
		/// <returns>Обновлённое значение даты</returns>
		public static DateTime AddTime (DateTime OldTime, int Value, DDIncrements Type)
			{
			switch (Type)
				{
				// Секунды
				case DDIncrements.Seconds:
					return OldTime.AddSeconds (Value);

				// Минуты
				case DDIncrements.Minutes:
					return OldTime.AddMinutes (Value);

				// Часы
				case DDIncrements.Hours:
					return OldTime.AddHours (Value);

				// Дни
				case DDIncrements.Days:
					return OldTime.AddDays (Value);

				// Недели
				case DDIncrements.Weeks:
					return OldTime.AddDays (Value * 7);

				default:
					return OldTime;
				}
			}

		/// <summary>
		/// Возвращает текущую дату с точностью до секунд
		/// </summary>
		public static DateTime NowSeconds
			{
			get
				{
				DateTime n = DateTime.Now;
				return new DateTime (n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
				}
			}

		/// <summary>
		/// Возвращает названия величин приращения для текущего языка
		/// </summary>
		public static string[] IncrementNames
			{
			get
				{
				return RDLocale.GetText ("AdditionalItems").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				}
			}
		private static char[] splitter = ['\n'];

		/// <summary>
		/// Сохраняет или загружает первую дату
		/// </summary>
		public static DateTime FirstSavedDate
			{
			get
				{
				string date = RDGenerics.GetSettings (firstDatePar, "");
				DateTime d;
				try
					{
					d = DateTime.Parse (date, RDLocale.GetCulture ());
					}
				catch
					{
					d = NowSeconds;
					}

				return d;
				}
			set
				{
				RDGenerics.SetSettings (firstDatePar, value.ToString (RDLocale.GetCulture ()));
				}
			}
		private const string firstDatePar = "FirstDate";

		/// <summary>
		/// Сохраняет или загружает вторую дату
		/// </summary>
		public static DateTime SecondSavedDate
			{
			get
				{
				string date = RDGenerics.GetSettings (secondDatePar, "");
				DateTime d;
				try
					{
					d = DateTime.Parse (date, RDLocale.GetCulture ());
					}
				catch
					{
					d = NowSeconds;
					}

				return d;
				}
			set
				{
				RDGenerics.SetSettings (secondDatePar, value.ToString (RDLocale.GetCulture ()));
				}
			}
		private const string secondDatePar = "SecondDate";

		/// <summary>
		/// Сохраняет или загружает тип приращения значений
		/// </summary>
		public static DDIncrements IncrementType
			{
			get
				{
				return (DDIncrements)RDGenerics.GetSettings (incrementTypePar,
					(uint)DDIncrements.Days);
				}
			set
				{
				RDGenerics.SetSettings (incrementTypePar, (uint)value);
				}
			}
		private const string incrementTypePar = "IncrementType";

		/// <summary>
		/// Метод оформляет результат в пригодную для копирования форму
		/// </summary>
		/// <param name="FirstDate">Первая дата</param>
		/// <param name="SecondDate">Вторая дата</param>
		/// <param name="LineNames">Названия строк форматов</param>
		/// <param name="ResultLines">Строки форматов</param>
		/// <returns></returns>
		public static string BuildResult (DateTime FirstDate, DateTime SecondDate,
			string LineNames, string ResultLines)
			{
			string res = string.Format (RDLocale.GetText ("DifferenceResultFmt"),
				FirstDate.ToString ("dd.MM.yyyy HH:mm:ss"), SecondDate.ToString ("dd.MM.yyyy HH:mm:ss"));

			string[] names = LineNames.Split (brSplitters, StringSplitOptions.RemoveEmptyEntries);
			string[] lines = ResultLines.Split (brSplitters, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < names.Length; i++)
				{
				res += RDLocale.RN + "• ";
				res += names[i] + " " + lines[i].Trim ();
				}

			return res;
			}
		private static char[] brSplitters = ['\r', '\n'];
		}
	}
