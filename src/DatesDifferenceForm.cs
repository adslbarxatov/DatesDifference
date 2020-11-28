using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает главную форму приложения
	/// </summary>
	public partial class DatesDifferenceForm: Form
		{
		// Переменные
		private SupportedLanguages al = Localization.CurrentLanguage;

		/// <summary>
		/// Конструктор. Запускает главную форму
		/// </summary>
		public DatesDifferenceForm ()
			{
			// Инициализация
			InitializeComponent ();
			this.Text = ProgramDescription.AssemblyTitle;

			LanguageCombo.Items.AddRange (Localization.LanguagesNames);
			try
				{
				LanguageCombo.SelectedIndex = (int)al;
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
			Localization.CurrentLanguage = al = (SupportedLanguages)LanguageCombo.SelectedIndex;

			// Локализация
			Localization.SetControlsText (this, al);
			Date_ValueChanged (null, null);
			}

		// Запрос справки
		private void DatesDifferenceForm_HelpButtonClicked (object sender, CancelEventArgs e)
			{
			e.Cancel = true;

			ProgramDescription.ShowAbout (false);
			}

		// Закрытие окна
		private void BExit_Click (object sender, EventArgs e)
			{
			this.Close ();
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
			double seconds = (((ulong)diff.Days * 24 + (ulong)diff.Hours) * 60 + (ulong)diff.Minutes) * 60 + (ulong)diff.Seconds;

			ResultLabel.Text = seconds.ToString ("#,#0") + "\n";
			seconds /= 60.0;
			ResultLabel.Text += seconds.ToString ("#,#0.###") + "\n";
			seconds /= 60.0;
			ResultLabel.Text += seconds.ToString ("#,#0.#####") + "\n";
			seconds /= 24.0;
			ResultLabel.Text += seconds.ToString ("#,#0.######") + "\n";
			seconds /= 7.0;
			ResultLabel.Text += seconds.ToString ("#,#0.#######") + "\n";

			ulong startMonthOffset = (ulong)(((start.Day * 24 + start.Hour) * 60 + start.Minute) * 60 + start.Second);
			ulong endMonthOffset = (ulong)(((end.Day * 24 + end.Hour) * 60 + end.Minute) * 60 + end.Second);

			ulong months = (ulong)(end.Year - start.Year) * 12 + (ulong)(end.Month - start.Month);
			if ((months > 0) && (endMonthOffset < startMonthOffset))
				months--;

			ResultLabel.Text += months.ToString () + "\n" + (months / 12).ToString ();

			ResultLabel.Text = string.Format (Localization.GetText ("FullFormat", al), diff.Days, diff.Hours,
				diff.Minutes, diff.Seconds) + "\n" + ResultLabel.Text.Replace (' ', '\'').Replace ('\xA0', '\'');
			}
		}
	}
