using System;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает главную форму приложения
	/// </summary>
	public partial class DatesDifferenceForm: Form
		{
		// Переменные
		private char[] splitter = new char[] { '\n' };

		/// <summary>
		/// Конструктор. Запускает главную форму
		/// </summary>
		public DatesDifferenceForm ()
			{
			// Инициализация
			InitializeComponent ();
			this.Text = ProgramDescription.AssemblyTitle;
			RDGenerics.LoadWindowDimensions (this);

			LanguageCombo.Items.AddRange (Localization.LanguagesNames);
			try
				{
				LanguageCombo.SelectedIndex = (int)Localization.CurrentLanguage;
				}
			catch
				{
				LanguageCombo.SelectedIndex = 0;
				}

			DateNow_Click (StartDateNow, null);
			DateNow_Click (EndDateNow, null);
			}

		// Локализация формы
		private void LanguageCombo_SelectedIndexChanged (object sender, EventArgs e)
			{
			// Сохранение языка
			Localization.CurrentLanguage = (SupportedLanguages)LanguageCombo.SelectedIndex;

			// Локализация
			Localization.SetControlsText (this);
			BExit.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Exit);
			AboutButton.Text = Localization.GetDefaultText (LzDefaultTextValues.Control_AppAbout);

			StartDateAdd.Text = EndDateAdd.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Add);
			EndDateAdd.Text = EndDateAdd.Text.Replace ("&", "").Insert (2, "&");

			string[] values = Localization.GetText ("AdditionalItems").Split (splitter,
				StringSplitOptions.RemoveEmptyEntries);

			int currentItem = (AdditionalItem.SelectedIndex < 0) ? 3 : AdditionalItem.SelectedIndex;    // Дни
			AdditionalItem.Items.Clear ();
			AdditionalItem.Items.AddRange (values);
			AdditionalItem.SelectedIndex = currentItem;

			Date_ValueChanged (null, null);
			}

		// Запрос справки
		private void AboutButton_Clicked (object sender, EventArgs e)
			{
			RDGenerics.ShowAbout (false);
			}

		// Закрытие окна
		private void BExit_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		private void DatesDifferenceForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			RDGenerics.SaveWindowDimensions (this);
			}

		// Задание значений в полях дат
		private void DateNow_Click (object sender, EventArgs e)
			{
			try
				{
				DateTime res = new DateTime (DateTime.Now.Year, DateTime.Now.Month,
					DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

				Button b = (Button)sender;
				if (b.Name.Contains ("Start"))
					StartDate.Value = res;
				else
					EndDate.Value = res;
				}
			catch { }
			}

		// Вычисление
		private void Date_ValueChanged (object sender, EventArgs e)
			{
			// Инициализация
			DateTime start = StartDate.Value, end = EndDate.Value;
			if (StartDate.Value > EndDate.Value)
				{
				end = StartDate.Value;
				start = EndDate.Value;
				}

			// Вычисление
			TimeSpan diff = end - start;
			double seconds = (((ulong)diff.Days * 24 + (ulong)diff.Hours) * 60 + (ulong)diff.Minutes) * 60 +
				(ulong)diff.Seconds;

			ResultLabel.Text = seconds.ToString ("#,#0") + Localization.RN;
			seconds /= 60.0;
			ResultLabel.Text += seconds.ToString ("#,#0.###") + Localization.RN;
			seconds /= 60.0;
			ResultLabel.Text += seconds.ToString ("#,#0.#####") + Localization.RN;
			seconds /= 24.0;
			ResultLabel.Text += seconds.ToString ("#,#0.######") + Localization.RN;
			seconds /= 7.0;
			ResultLabel.Text += seconds.ToString ("#,#0.#######") + Localization.RN;

			ulong startMonthOffset = (ulong)(((start.Day * 24 + start.Hour) * 60 + start.Minute) * 60 + start.Second);
			ulong endMonthOffset = (ulong)(((end.Day * 24 + end.Hour) * 60 + end.Minute) * 60 + end.Second);

			ulong months = (ulong)(end.Year - start.Year) * 12 + (ulong)(end.Month - start.Month);
			if ((months > 0) && (endMonthOffset < startMonthOffset))
				months--;

			ResultLabel.Text += months.ToString () +
				"@(" + (months / 12).ToString () + "@×@12@+@" + (months % 12).ToString () + ")" + Localization.RN;
			ResultLabel.Text += (months / 12).ToString ();

			ResultLabel.Text = string.Format (Localization.GetText ("FullFormat"), diff.Days, diff.Hours,
				diff.Minutes, diff.Seconds) + Localization.RN +
				ResultLabel.Text.Replace (' ', '\'').Replace ('\xA0', '\'').Replace ("@", " ");
			}

		// Добавление значений
		private DateTime AddTime (DateTime OldTime)
			{
			switch (AdditionalItem.SelectedIndex)
				{
				// Секунды
				case 0:
					return OldTime.AddSeconds ((double)AdditionalValue.Value);

				// Минуты
				case 1:
					return OldTime.AddMinutes ((double)AdditionalValue.Value);

				// Часы
				case 2:
					return OldTime.AddHours ((double)AdditionalValue.Value);

				// Дни
				case 3:
					return OldTime.AddDays ((double)AdditionalValue.Value);

				// Недели
				case 4:
					return OldTime.AddDays ((double)AdditionalValue.Value * 7.0);

				default:
					return OldTime;
				}
			}

		private void StartDateAdd_Click (object sender, EventArgs e)
			{
			DateTimePicker field = (((Button)sender).Name == "StartDateAdd") ? StartDate : EndDate;
			DateTime newTime = AddTime (field.Value);

			if ((newTime < field.MinDate) || (newTime > field.MaxDate))
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "DateTruncated");

				if (newTime < StartDate.MinDate)
					field.Value = field.MinDate;
				else
					field.Value = field.MaxDate;

				return;
				}

			field.Value = newTime;
			}
		}
	}
