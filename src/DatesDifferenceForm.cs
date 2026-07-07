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
			this.Text = RDGenerics.DefaultAssemblyVisibleName;
			RDGenerics.LoadWindowDimensions (this);

			LocalizeForm_Click (null, null);

			StartDate.Value = DDMath.FirstSavedDate;
			EndDate.Value = DDMath.SecondSavedDate;
			}

		// Локализация формы
		private void LocalizeForm_Click (object sender, EventArgs e)
			{
			// Выбор языка
			if ((sender != null) && !RDInterface.MessageBox ())
				return;

			// Локализация
			RDLocale.SetControlText (this.Name, CopyResultButton);
			RDLocale.SetControlText (this.Name, EndDateLabel);
			RDLocale.SetControlText (this.Name, EndDateNow);
			RDLocale.SetControlText (this.Name, LinesLabel);
			RDLocale.SetControlText (this.Name, StartDateLabel);
			RDLocale.SetControlText (this.Name, StartDateNow);
			RDLocale.SetDefaultControlText (BExit, RDLDefaultTexts.Button_Exit);
			RDLocale.SetDefaultControlText (LanguageButton, RDLDefaultTexts.Control_InterfaceLanguage);
			RDLocale.SetDefaultControlText (AboutButton, RDLDefaultTexts.Control_AppAbout);

			int currentItem = (AdditionalItem.SelectedIndex < 0) ? (int)DDMath.IncrementType :
				AdditionalItem.SelectedIndex;
			AdditionalItem.Items.Clear ();
			AdditionalItem.Items.AddRange (DDMath.IncrementNames);
			AdditionalItem.SelectedIndex = currentItem;

			Date_ValueChanged (null, null);
			}

		// Запрос справки
		private void AboutButton_Clicked (object sender, EventArgs e)
			{
			RDInterface.ShowAbout (false);
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
			DDMath.IncrementType = (DDIncrements)AdditionalItem.SelectedIndex;

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
			ResultLabel.Text = DDMath.GetDifferencePresentation (StartDate.Value, EndDate.Value);
			}

		// Добавление значений
		private void StartDateAdd_Click (object sender, EventArgs e)
			{
			string name = ((Button)sender).Name;
			bool start = name.Contains ("Start");
			bool add = name.Contains ("Add");

			DateTimePicker field = start ? StartDate : EndDate;
			DateTime newTime = DDMath.AddTime (field.Value, add ? 1 : -1,
				(DDIncrements)AdditionalItem.SelectedIndex);

			if ((newTime < field.MinDate) || (newTime > field.MaxDate))
				{
				RDInterface.LocalizedMessageBox (RDMessageFlags.Warning | RDMessageFlags.CenterText,
					"DateTruncated", 750);

				if (newTime < StartDate.MinDate)
					field.Value = field.MinDate;
				else
					field.Value = field.MaxDate;

				return;
				}

			field.Value = newTime;
			}

		// Копирование результата
		private void Result_Click (object sender, EventArgs e)
			{
			RDGenerics.SendToClipboard (DDMath.BuildResult (StartDate.Value, EndDate.Value,
				LinesLabel.Text, ResultLabel.Text), true);
			}
		}
	}
