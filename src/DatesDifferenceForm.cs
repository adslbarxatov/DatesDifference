using System;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает главную форму приложения
	/// </summary>
	public partial class DatesDifferenceForm: Form
		{
		/// <summary>
		/// Конструктор. Запускает главную форму
		/// </summary>
		public DatesDifferenceForm ()
			{
			// Инициализация
			InitializeComponent ();
			this.Text = ProgramDescription.AssemblyTitle;
			RDGenerics.LoadWindowDimensions (this);

			LanguageCombo.Items.AddRange (RDLocale.LanguagesNames);
			try
				{
				LanguageCombo.SelectedIndex = (int)RDLocale.CurrentLanguage;
				}
			catch
				{
				LanguageCombo.SelectedIndex = 0;
				}

			/*DateNow_Click (StartDateNow, null);
			DateNow_Click (EndDateNow, null);*/
			StartDate.Value = DDMath.FirstSavedDate;
			EndDate.Value = DDMath.SecondSavedDate;
			}

		// Локализация формы
		private void LanguageCombo_SelectedIndexChanged (object sender, EventArgs e)
			{
			// Сохранение языка
			RDLocale.CurrentLanguage = (RDLanguages)LanguageCombo.SelectedIndex;

			// Локализация
			RDLocale.SetControlsText (this);
			BExit.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Exit);
			AboutButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout);

			StartDateAdd.Text = EndDateAdd.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Add);
			EndDateAdd.Text = EndDateAdd.Text.Replace ("&", "").Insert (2, "&");

			int currentItem = (AdditionalItem.SelectedIndex < 0) ? (int)DDIncrements.Days :
				AdditionalItem.SelectedIndex;
			AdditionalItem.Items.Clear ();
			AdditionalItem.Items.AddRange (DDMath.IncrementNames);
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
			DDMath.FirstSavedDate = StartDate.Value;
			DDMath.SecondSavedDate = EndDate.Value;
			RDGenerics.SaveWindowDimensions (this);
			}

		// Задание значений в полях дат
		private void DateNow_Click (object sender, EventArgs e)
			{
			try
				{
				Button b = (Button)sender;
				if (b.Name.Contains ("Start"))
					StartDate.Value = DDMath.NowSeconds;
				else
					EndDate.Value = DDMath.NowSeconds;
				}
			catch { }
			}

		// Вычисление
		private void Date_ValueChanged (object sender, EventArgs e)
			{
			string[] res = DDMath.GetDifferencePresentation (StartDate.Value, EndDate.Value);

			ResultLabel1.Text = res[0];
			ResultLabel2.Text = res[1];
			ResultLabel3.Text = res[2];
			ResultLabel4.Text = res[3];
			}

		// Добавление значений
		private void StartDateAdd_Click (object sender, EventArgs e)
			{
			DateTimePicker field = (((Button)sender).Name == "StartDateAdd") ? StartDate : EndDate;
			DateTime newTime = DDMath.AddTime (field.Value, (int)AdditionalValue.Value,
				(DDIncrements)AdditionalItem.SelectedIndex);

			if ((newTime < field.MinDate) || (newTime > field.MaxDate))
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "DateTruncated", 750);

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
