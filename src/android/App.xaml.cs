using Microsoft.Maui.Controls;
using System.ComponentModel;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает функционал приложения
	/// </summary>
	public partial class App: Application
		{
		#region Общие переменные и константы

		// Прочие параметры
		private RDAppStartupFlags flags;
		private List<string> incrementTypesNames = new List<string> ();

		// Цветовая схема
		private readonly Color
			solutionMasterBackColor = Color.FromArgb ("#e7e7ff"),
			solutionFieldBackColor = Color.FromArgb ("#dedeff"),

			aboutMasterBackColor = Color.FromArgb ("#F0FFF0"),
			aboutFieldBackColor = Color.FromArgb ("#D0FFD0");

		#endregion

		#region Переменные страниц

		private ContentPage solutionPage, aboutPage;

		private Label aboutFontSizeField;
		private Label resultLabel, resultNames;

		private Button incrementTypeButton, languageButton;
		private List<Button> incrementButtons = new List<Button> ();

		private DatePicker firstDate, secondDate;
		private TimePicker firstTime, secondTime;
		private DateTime minimumDate, maximumDate;

		#endregion

		#region Запуск и настройка

		/// <summary>
		/// Конструктор. Точка входа приложения
		/// </summary>
		public App ()
			{
			// Инициализация
			InitializeComponent ();
			flags = AndroidSupport.GetAppStartupFlags (RDAppStartupFlags.DisableXPUN);

			// Общая конструкция страниц приложения
			MainPage = new MasterPage ();

			solutionPage = AndroidSupport.ApplyPageSettings (new SolutionPage (), "SolutionPage",
				RDLocale.GetText ("SolutionPage"), solutionMasterBackColor);
			aboutPage = AndroidSupport.ApplyPageSettings (new AboutPage (), "AboutPage",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				aboutMasterBackColor);

			AndroidSupport.SetMasterPage (MainPage, solutionPage, solutionMasterBackColor);

			#region Основная страница

			// Поля дат
			AndroidSupport.ApplyLabelSettings (solutionPage, "InputLabel",
				RDLocale.GetText ("InputLabel"), RDLabelTypes.HeaderLeft);

			AndroidSupport.ApplyLabelSettings (solutionPage, "FirstDateLabel",
				RDLocale.GetText ("FirstDateLabel"), RDLabelTypes.DefaultLeft);
			firstDate = AndroidSupport.ApplyDatePickerSettings (solutionPage, "FirstDatePicker",
				solutionFieldBackColor, DateSelectionMethod);
			firstTime = AndroidSupport.ApplyTimePickerSettings (solutionPage, "FirstTimePicker",
				solutionFieldBackColor, DateSelectionMethod);
			incrementButtons.Add (AndroidSupport.ApplyButtonSettings (solutionPage, "FirstDateIncrement",
				RDDefaultButtons.Increase, solutionFieldBackColor, DateIncrementMethod));
			incrementButtons.Add (AndroidSupport.ApplyButtonSettings (solutionPage, "FirstDateDecrement",
				RDDefaultButtons.Decrease, solutionFieldBackColor, DateIncrementMethod));

			AndroidSupport.ApplyLabelSettings (solutionPage, "SecondDateLabel",
				RDLocale.GetText ("SecondDateLabel"), RDLabelTypes.DefaultLeft);
			secondDate = AndroidSupport.ApplyDatePickerSettings (solutionPage, "SecondDatePicker",
				solutionFieldBackColor, DateSelectionMethod);
			secondTime = AndroidSupport.ApplyTimePickerSettings (solutionPage, "SecondTimePicker",
				solutionFieldBackColor, DateSelectionMethod);
			incrementButtons.Add (AndroidSupport.ApplyButtonSettings (solutionPage, "SecondDateIncrement",
				RDDefaultButtons.Increase, solutionFieldBackColor, DateIncrementMethod));
			incrementButtons.Add (AndroidSupport.ApplyButtonSettings (solutionPage, "SecondDateDecrement",
				RDDefaultButtons.Decrease, solutionFieldBackColor, DateIncrementMethod));

			incrementButtons.Add (AndroidSupport.ApplyButtonSettings (solutionPage, "FirstDateNow",
				RDDefaultButtons.Create, solutionFieldBackColor, DateIncrementMethod));
			incrementButtons.Add (AndroidSupport.ApplyButtonSettings (solutionPage, "SecondDateNow",
				RDDefaultButtons.Create, solutionFieldBackColor, DateIncrementMethod));

			firstDate.MinimumDate = secondDate.MinimumDate = minimumDate = new DateTime (1900, 1, 1, 0, 0, 0);
			firstDate.MaximumDate = secondDate.MaximumDate = maximumDate = new DateTime (2100, 1, 1, 0, 0, 0);

			// Поле типа инкремента
			AndroidSupport.ApplyLabelSettings (solutionPage, "IncrementLabel",
				RDLocale.GetText ("IncrementLabel"), RDLabelTypes.DefaultLeft);
			incrementTypesNames.AddRange (DDMath.IncrementNames);
			incrementTypeButton = AndroidSupport.ApplyButtonSettings (solutionPage, "IncrementButton",
				" ", solutionFieldBackColor, SelectIncrementMethod, false);

			SelectIncrementMethod (null, null);

			// Поля результатов
			AndroidSupport.ApplyLabelSettings (solutionPage, "OutputLabel",
				RDLocale.GetText ("OutputLabel"), RDLabelTypes.HeaderLeft);

			resultNames = AndroidSupport.ApplyLabelSettings (solutionPage, "ResultNamesLabel",
				RDLocale.GetText ("ResultNamesLabel").Replace ("\r", ""), RDLabelTypes.DefaultLeft);
			resultNames.FontAttributes = FontAttributes.Bold;
			resultNames.FontFamily = AndroidSupport.MonospaceFont;
			resultNames.HorizontalTextAlignment = TextAlignment.End;

			resultLabel = AndroidSupport.ApplyLabelSettings (solutionPage, "ResultLabel",
				" ", RDLabelTypes.DefaultLeft);
			resultLabel.FontFamily = AndroidSupport.MonospaceFont;

			// Вызов меню
			AndroidSupport.ApplyButtonSettings (solutionPage, "MenuButton",
				RDDefaultButtons.Menu, solutionFieldBackColor, AboutButton_Clicked);
			AndroidSupport.ApplyButtonSettings (solutionPage, "CopyResultButton",
				RDLocale.GetText ("CopyResultButton"), solutionFieldBackColor, CopyResult_Clicked, false);

			// Загрузка сохранённых значений
			FirstDateFull = DDMath.FirstSavedDate;
			SecondDateFull = DDMath.SecondSavedDate;

			// Обновление состояния
			DateSelectionMethod (null, null);

			#endregion

			#region Страница "О программе"

			AndroidSupport.ApplyLabelSettings (aboutPage, "AboutLabel",
				RDGenerics.AppAboutLabelText, RDLabelTypes.AppAbout);

			AndroidSupport.ApplyButtonSettings (aboutPage, "ManualsButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_ReferenceMaterials),
				aboutFieldBackColor, ReferenceButton_Click, false);
			AndroidSupport.ApplyButtonSettings (aboutPage, "HelpButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_HelpSupport),
				aboutFieldBackColor, HelpButton_Click, false);
			AndroidSupport.ApplyLabelSettings (aboutPage, "GenericSettingsLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_GenericSettings),
				RDLabelTypes.HeaderLeft);

			AndroidSupport.ApplyLabelSettings (aboutPage, "RestartTipLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Message_RestartRequired),
				RDLabelTypes.TipCenter);

			AndroidSupport.ApplyLabelSettings (aboutPage, "LanguageLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceLanguage),
				RDLabelTypes.DefaultLeft);
			languageButton = AndroidSupport.ApplyButtonSettings (aboutPage, "LanguageSelector",
				RDLocale.LanguagesNames[(int)RDLocale.CurrentLanguage],
				aboutFieldBackColor, SelectLanguage_Clicked, false);

			AndroidSupport.ApplyLabelSettings (aboutPage, "FontSizeLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceFontSize),
				RDLabelTypes.DefaultLeft);
			AndroidSupport.ApplyButtonSettings (aboutPage, "FontSizeInc",
				RDDefaultButtons.Increase, aboutFieldBackColor, FontSizeButton_Clicked);
			AndroidSupport.ApplyButtonSettings (aboutPage, "FontSizeDec",
				RDDefaultButtons.Decrease, aboutFieldBackColor, FontSizeButton_Clicked);
			aboutFontSizeField = AndroidSupport.ApplyLabelSettings (aboutPage, "FontSizeField",
				" ", RDLabelTypes.DefaultCenter);

			AndroidSupport.ApplyLabelSettings (aboutPage, "HelpHeaderLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				RDLabelTypes.HeaderLeft);
			Label htl = AndroidSupport.ApplyLabelSettings (aboutPage, "HelpTextLabel",
				AndroidSupport.GetAppHelpText (), RDLabelTypes.SmallLeft);
			htl.TextType = TextType.Html;

			FontSizeButton_Clicked (null, null);

			#endregion

			// Отображение подсказок первого старта
			ShowStartupTips ();
			}

		// Свойства-ретрансляторы
		private DateTime FirstDateFull
			{
			get
				{
				return firstDate.Date.Add (firstTime.Time);
				}
			set
				{
				firstDate.Date = value.Date;
				firstTime.Time = value.TimeOfDay;
				}
			}

		private DateTime SecondDateFull
			{
			get
				{
				return secondDate.Date.Add (secondTime.Time);
				}
			set
				{
				secondDate.Date = value.Date;
				secondTime.Time = value.TimeOfDay;
				}
			}

		// Метод отображает подсказки при первом запуске
		private async void ShowStartupTips ()
			{
			// Контроль XPUN
			if (!flags.HasFlag (RDAppStartupFlags.DisableXPUN))
				await AndroidSupport.XPUNLoop ();

			// Требование принятия Политики
			if (TipsState.HasFlag (TipTypes.PolicyTip))
				return;

			await AndroidSupport.PolicyLoop ();
			TipsState |= TipTypes.PolicyTip;
			}

		/// <summary>
		/// Сохранение настроек программы
		/// </summary>
		protected override void OnSleep ()
			{
			// Сохранение значений
			DDMath.FirstSavedDate = FirstDateFull;
			DDMath.SecondSavedDate = SecondDateFull;
			}

		/// <summary>
		/// Возвращает или задаёт состав флагов просмотра справочных сведений
		/// </summary>
		public static TipTypes TipsState
			{
			get
				{
				return (TipTypes)RDGenerics.GetSettings (tipsStatePar, 0);
				}
			set
				{
				RDGenerics.SetSettings (tipsStatePar, (uint)value);
				}
			}
		private const string tipsStatePar = "TipsState";

		/// <summary>
		/// Доступные типы уведомлений
		/// </summary>
		public enum TipTypes
			{
			/// <summary>
			/// Принятие Политики и первая подсказка
			/// </summary>
			PolicyTip = 0x0001,
			}

		#endregion

		#region О приложении

		// Выбор языка приложения
		private async void SelectLanguage_Clicked (object sender, EventArgs e)
			{
			languageButton.Text = await AndroidSupport.CallLanguageSelector ();
			}

		// Вызов справочных материалов
		private async void ReferenceButton_Click (object sender, EventArgs e)
			{
			await AndroidSupport.CallHelpMaterials (RDHelpMaterials.ReferenceMaterials);
			}

		private async void HelpButton_Click (object sender, EventArgs e)
			{
			await AndroidSupport.CallHelpMaterials (RDHelpMaterials.HelpAndSupport);
			}

		// Изменение размера шрифта интерфейса
		private void FontSizeButton_Clicked (object sender, EventArgs e)
			{
			if (sender != null)
				{
				Button b = (Button)sender;
				if (AndroidSupport.IsNameDefault (b.Text, RDDefaultButtons.Increase))
					AndroidSupport.MasterFontSize += 0.5;
				else if (AndroidSupport.IsNameDefault (b.Text, RDDefaultButtons.Decrease))
					AndroidSupport.MasterFontSize -= 0.5;
				}

			aboutFontSizeField.Text = AndroidSupport.MasterFontSize.ToString ("F1");
			aboutFontSizeField.FontSize = AndroidSupport.MasterFontSize;
			}

		#endregion

		#region Рабочая зона

		// Выбор или изменение даты
		private void DateSelectionMethod (object sender, PropertyChangedEventArgs e)
			{
			if ((e != null) && (e.PropertyName != "Date") && (e.PropertyName != "Time"))
				return;

			// Обновление состояния
			resultLabel.Text = DDMath.GetDifferencePresentation (FirstDateFull, SecondDateFull);
			}

		// Приращение даты
		private void DateIncrementMethod (object sender, EventArgs e)
			{
			int idx = incrementButtons.IndexOf ((Button)sender);
			int increment = ((idx % 2 == 0) ? 1 : -1);

			// Обработка сбросов
			if (idx >= 4)
				{
				if (idx % 2 == 0)
					FirstDateFull = DDMath.NowSeconds;
				else
					SecondDateFull = DDMath.NowSeconds;

				return;
				}

			DateTime value;
			if (idx < 2)
				value = DDMath.AddTime (FirstDateFull, increment, DDMath.IncrementType);
			else
				value = DDMath.AddTime (SecondDateFull, increment, DDMath.IncrementType);

			if ((value < minimumDate) || (value > maximumDate))
				{
				AndroidSupport.ShowBalloon (RDLocale.GetText ("DateTruncated"), true);

				if (value < minimumDate)
					value = minimumDate;
				else
					value = maximumDate;
				}

			if (idx < 2)
				FirstDateFull = value;
			else
				SecondDateFull = value;
			}

		// Выбор варианта приращения
		private async void SelectIncrementMethod (object sender, EventArgs e)
			{
			int res;
			if (sender == null)
				{
				res = (int)DDMath.IncrementType;
				}
			else
				{
				res = await AndroidSupport.ShowList (RDLocale.GetText ("IncrementLabel"),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), incrementTypesNames);
				if (res < 0)
					return;

				DDMath.IncrementType = (DDIncrements)res;
				}

			incrementTypeButton.Text = incrementTypesNames[res];
			}

		// Метод открывает страницу О программе
		private void AboutButton_Clicked (object sender, EventArgs e)
			{
			AndroidSupport.SetCurrentPage (aboutPage, aboutMasterBackColor);
			}

		// Копирование результата
		private void CopyResult_Clicked (object sender, EventArgs e)
			{
			RDGenerics.SendToClipboard (DDMath.BuildResult (FirstDateFull, SecondDateFull,
				resultNames.Text, resultLabel.Text), true);
			}

		#endregion
		}
	}
