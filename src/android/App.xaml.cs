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
		private List<string> incrementTypesNames = [];

		// Цветовая схема
		private Color solutionMasterBackColor = Color.FromArgb ("#e7e7ff");
		private Color solutionFieldBackColor = Color.FromArgb ("#dedeff");
		private Color aboutMasterBackColor = Color.FromArgb ("#F0FFF0");
		private Color aboutFieldBackColor = Color.FromArgb ("#D0FFD0");

		#endregion

		#region Переменные страниц

		private ContentPage solutionPage, aboutPage;

		private Label aboutFontSizeField;
		private Label resultLabel, resultNames;

		private Button incrementTypeButton, languageButton;
		private List<Button> incrementButtons = [];

		private DatePicker firstDate, secondDate;
		private TimePicker firstTime, secondTime;
		private DateTime minimumDate, maximumDate;

		private StackLayout masterLayout;

		#endregion

		#region Запуск и настройка

		/// <summary>
		/// Конструктор. Точка входа приложения
		/// </summary>
		public App ()
			{
			// Инициализация
			InitializeComponent ();
			}

		// Замена определению MainPage = new MasterPage ()
		protected override Window CreateWindow (IActivationState activationState)
			{
			return new Window (AppShell ());
			}

		// Инициализация страниц приложения
		private Page AppShell ()
			{
			Page mainPage = new MasterPage ();
			flags = RDGenerics.GetAppStartupFlags (RDAppStartupFlags.DisableXPUN);

			// Общая конструкция страниц приложения
			solutionPage = RDInterface.ApplyPageSettings (new SolutionPage (),
				RDLocale.GetText ("SolutionPage"), solutionMasterBackColor);
			aboutPage = RDInterface.ApplyPageSettings (new AboutPage (),
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				aboutMasterBackColor);

			RDInterface.SetMasterPage (mainPage, solutionPage, solutionMasterBackColor);

			#region Основная страница

			// Поля дат
			RDInterface.ApplyLabelSettings (solutionPage, "InputLabel",
				RDLocale.GetText ("InputLabel"), RDLabelTypes.HeaderLeft);

			RDInterface.ApplyLabelSettings (solutionPage, "FirstDateLabel",
				RDLocale.GetText ("FirstDateLabel"), RDLabelTypes.DefaultLeft);
			firstDate = RDInterface.ApplyDatePickerSettings (solutionPage, "FirstDatePicker",
				solutionFieldBackColor, DateSelectionMethod);
			firstTime = RDInterface.ApplyTimePickerSettings (solutionPage, "FirstTimePicker",
				solutionFieldBackColor, DateSelectionMethod);
			firstTime.Margin = Thickness.Zero;
			incrementButtons.Add (RDInterface.ApplyButtonSettings (solutionPage, "FirstDateIncrement",
				RDDefaultButtons.Increase, solutionFieldBackColor, DateIncrementMethod));
			incrementButtons.Add (RDInterface.ApplyButtonSettings (solutionPage, "FirstDateDecrement",
				RDDefaultButtons.Decrease, solutionFieldBackColor, DateIncrementMethod));

			RDInterface.ApplyLabelSettings (solutionPage, "SecondDateLabel",
				RDLocale.GetText ("SecondDateLabel"), RDLabelTypes.DefaultLeft);
			secondDate = RDInterface.ApplyDatePickerSettings (solutionPage, "SecondDatePicker",
				solutionFieldBackColor, DateSelectionMethod);
			secondTime = RDInterface.ApplyTimePickerSettings (solutionPage, "SecondTimePicker",
				solutionFieldBackColor, DateSelectionMethod);
			secondTime.Margin = Thickness.Zero;
			incrementButtons.Add (RDInterface.ApplyButtonSettings (solutionPage, "SecondDateIncrement",
				RDDefaultButtons.Increase, solutionFieldBackColor, DateIncrementMethod));
			incrementButtons.Add (RDInterface.ApplyButtonSettings (solutionPage, "SecondDateDecrement",
				RDDefaultButtons.Decrease, solutionFieldBackColor, DateIncrementMethod));

			incrementButtons.Add (RDInterface.ApplyButtonSettings (solutionPage, "FirstDateNow",
				RDDefaultButtons.Create, solutionFieldBackColor, DateIncrementMethod));
			incrementButtons.Add (RDInterface.ApplyButtonSettings (solutionPage, "SecondDateNow",
				RDDefaultButtons.Create, solutionFieldBackColor, DateIncrementMethod));

			firstDate.MinimumDate = secondDate.MinimumDate = minimumDate = new DateTime (1900, 1, 1, 0, 0, 0);
			firstDate.MaximumDate = secondDate.MaximumDate = maximumDate = new DateTime (2100, 1, 1, 0, 0, 0);

			// Поле типа инкремента
			RDInterface.ApplyLabelSettings (solutionPage, "IncrementLabel",
				RDLocale.GetText ("IncrementLabel"), RDLabelTypes.DefaultLeft);
			incrementTypesNames.AddRange (DDMath.IncrementNames);
			incrementTypeButton = RDInterface.ApplyButtonSettings (solutionPage, "IncrementButton",
				" ", solutionFieldBackColor, SelectIncrementMethod, false);

			SelectIncrementMethod (null, null);

			// Поля результатов
			RDInterface.ApplyLabelSettings (solutionPage, "OutputLabel",
				RDLocale.GetText ("OutputLabel"), RDLabelTypes.HeaderLeft);

			resultNames = RDInterface.ApplyLabelSettings (solutionPage, "ResultNamesLabel",
				RDLocale.GetText ("ResultNamesLabel").Replace ("\r", ""), RDLabelTypes.DefaultLeft);
			resultNames.FontAttributes = FontAttributes.Bold;
			resultNames.FontFamily = RDGenerics.MonospaceFont;
			resultNames.HorizontalTextAlignment = TextAlignment.End;

			resultLabel = RDInterface.ApplyLabelSettings (solutionPage, "ResultLabel",
				" ", RDLabelTypes.DefaultLeft);
			resultLabel.FontFamily = RDGenerics.MonospaceFont;

			// Ориентация
			masterLayout = (StackLayout)solutionPage.FindByName ("MasterLayout");
			DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;

			// Вызов меню
			Button mnu = RDInterface.ApplyButtonSettings (solutionPage, "MenuButton",
				RDDefaultButtons.Menu, solutionFieldBackColor, AboutButton_Clicked);
			Button cpy = RDInterface.ApplyButtonSettings (solutionPage, "CopyResultButton",
				RDLocale.GetText ("CopyResultButton"), solutionFieldBackColor, CopyResult_Clicked, false);
			incrementTypeButton.HeightRequest = cpy.HeightRequest = mnu.HeightRequest;

			// Загрузка сохранённых значений
			FirstDateFull = DDMath.FirstSavedDate;
			SecondDateFull = DDMath.SecondSavedDate;

			// Обновление состояния
			DateSelectionMethod (null, null);

			#endregion

			#region Страница "О программе"

			RDInterface.ApplyLabelSettings (aboutPage, "AboutLabel",
				RDGenerics.AppAboutLabelText, RDLabelTypes.AppAbout);

			RDInterface.ApplyButtonSettings (aboutPage, "ManualsButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_ReferenceMaterials),
				aboutFieldBackColor, ReferenceButton_Click, false);
			RDInterface.ApplyButtonSettings (aboutPage, "HelpButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_HelpSupport),
				aboutFieldBackColor, HelpButton_Click, false);
			RDInterface.ApplyLabelSettings (aboutPage, "GenericSettingsLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_GenericSettings),
				RDLabelTypes.HeaderLeft);

			RDInterface.ApplyLabelSettings (aboutPage, "RestartTipLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Message_RestartRequired),
				RDLabelTypes.TipCenter);

			RDInterface.ApplyLabelSettings (aboutPage, "LanguageLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceLanguage),
				RDLabelTypes.DefaultLeft);
			languageButton = RDInterface.ApplyButtonSettings (aboutPage, "LanguageSelector",
				RDLocale.LanguagesNames[(int)RDLocale.CurrentLanguage],
				aboutFieldBackColor, SelectLanguage_Clicked, false);

			RDInterface.ApplyLabelSettings (aboutPage, "FontSizeLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceFontSize),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyButtonSettings (aboutPage, "FontSizeInc",
				RDDefaultButtons.Increase, aboutFieldBackColor, FontSizeButton_Clicked);
			RDInterface.ApplyButtonSettings (aboutPage, "FontSizeDec",
				RDDefaultButtons.Decrease, aboutFieldBackColor, FontSizeButton_Clicked);
			aboutFontSizeField = RDInterface.ApplyLabelSettings (aboutPage, "FontSizeField",
				" ", RDLabelTypes.DefaultCenter);

			RDInterface.ApplyLabelSettings (aboutPage, "HelpHeaderLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				RDLabelTypes.HeaderLeft);
			Label htl = RDInterface.ApplyLabelSettings (aboutPage, "HelpTextLabel",
				RDGenerics.GetAppHelpText (), RDLabelTypes.SmallLeft);
			htl.TextType = TextType.Html;

			FontSizeButton_Clicked (null, null);

			#endregion

			// Отображение подсказок первого старта
			ShowStartupTips ();
			return mainPage;
			}

		// Изменение ориентации экрана
		private async void Current_MainDisplayInfoChanged (object sender, DisplayInfoChangedEventArgs e)
			{
			await Task.Delay (500);
			bool portrait = Windows[0].Width < Windows[0].Height;
			masterLayout.Orientation = (portrait ? StackOrientation.Vertical : StackOrientation.Horizontal);
			}

		protected override void OnStart ()
			{
			Current_MainDisplayInfoChanged (null, null);
			base.OnStart ();
			}

		protected override void OnResume ()
			{
			Current_MainDisplayInfoChanged (null, null);
			base.OnResume ();
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
				await RDInterface.XPUNLoop ();

			// Требование принятия Политики
			await RDInterface.PolicyLoop ();
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

		#endregion

		#region О приложении

		// Выбор языка приложения
		private async void SelectLanguage_Clicked (object sender, EventArgs e)
			{
			languageButton.Text = await RDInterface.CallLanguageSelector ();
			}

		// Вызов справочных материалов
		private async void ReferenceButton_Click (object sender, EventArgs e)
			{
			await RDInterface.CallHelpMaterials (RDHelpMaterials.ReferenceMaterials);
			}

		private async void HelpButton_Click (object sender, EventArgs e)
			{
			await RDInterface.CallHelpMaterials (RDHelpMaterials.HelpAndSupport);
			}

		// Изменение размера шрифта интерфейса
		private void FontSizeButton_Clicked (object sender, EventArgs e)
			{
			if (sender != null)
				{
				Button b = (Button)sender;
				if (RDInterface.IsNameDefault (b.Text, RDDefaultButtons.Increase))
					RDInterface.MasterFontSize += 0.5;
				else if (RDInterface.IsNameDefault (b.Text, RDDefaultButtons.Decrease))
					RDInterface.MasterFontSize -= 0.5;
				}

			aboutFontSizeField.Text = RDInterface.MasterFontSize.ToString ("F1");
			aboutFontSizeField.FontSize = RDInterface.MasterFontSize;
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
				RDInterface.ShowBalloon (RDLocale.GetText ("DateTruncated"), true);

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
				res = await RDInterface.ShowList (RDLocale.GetText ("IncrementLabel"),
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
			RDInterface.SetCurrentPage (aboutPage, aboutMasterBackColor);
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
