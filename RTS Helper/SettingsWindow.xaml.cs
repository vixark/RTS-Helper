using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using static RTSHelper.Global;
using System.IO;
using static Vixark.General;
using System.Linq;
using Controles = System.Windows.Controls;
using System.Reflection;
using System.Windows.Threading;
using System.Drawing.Drawing2D;



namespace RTSHelper {



    public partial class SettingsWindow : Window {



        public bool Activado { get; set; } = false;

        public bool ActualizandoUIMods { get; set; } = false;

        public bool CambióResolución { get; set; } = false;

        private MainWindow VentanaPrincipal { get; set; }

        public bool ActualizarDuraciónPasoAlSalir { get; set; } = false;

        public Dictionary<NameType, Dictionary<string, string>>? NombresPersonalizados { get; set; } = null; // Se hace quí para que cada vez que se abra los settings de custom names se lea este archivo, esto permite más flexibilidad para editarlo a mano o reemplazarlo por otro sin reiniciar el programa, si el usuario desea.

        public bool UnNombrePersonalizadoCambiado { get; set; } = false;

        private DispatcherTimer TimerPruebasOCR = new DispatcherTimer();


        public SettingsWindow(bool primerInicio, MainWindow ventanaPrincipal) {

            InitializeComponent();
            VentanaPrincipal = ventanaPrincipal;
            var versión = Assembly.GetEntryAssembly()?.GetName().Version;
            LblVersion.Content = $"{versión?.Major}.{versión?.Minor}{(versión?.Build == 0 ? "" : $".{versión?.Build.ToString()}")}";
            TbCopyright.Text = @$"Copyright© {DateTime.Now.Year} Vixark (vixark@outlook.com, github/vixark)";
            TimerPruebasOCR.Interval = TimeSpan.FromMilliseconds(300); // Se hace un poco más frecuente para que se visualicen los cambios más rápidamente. En el modo Pruebas OCR no es tan importante el rendimiento.
            TimerPruebasOCR.Tick += new EventHandler(TimerDetecciónPruebasOCR_Tick);
            LnkDonate.NavigateUri = new Uri(EnlaceDonación);

            CargarValores(primerInicio);
            if (primerInicio) {
              
                SpnLabels2.Visibility = Visibility.Collapsed;
                SpnSettings2.Visibility = Visibility.Collapsed;
                BtnBackcolor.Visibility = Visibility.Collapsed;
                BtnFontcolor.Visibility = Visibility.Collapsed;
                TxtOpacity.Visibility = Visibility.Collapsed;
                LblBackColor.Visibility = Visibility.Collapsed;
                LblFontColor.Visibility = Visibility.Collapsed;
                LblOpacity.Visibility = Visibility.Collapsed;
                TbiNotifications.Visibility = Visibility.Collapsed;
                TbiImages.Visibility = Visibility.Collapsed;
                TbiDisplayPriority.Visibility = Visibility.Collapsed;
                TbiPersonalization.Visibility = Visibility.Collapsed;
                TbiAbout.Visibility = Visibility.Collapsed;
                TbiCustomNames.Visibility = Visibility.Collapsed;
                TbiOverrides.Visibility = Visibility.Collapsed;
                TbiControl.Visibility = Visibility.Collapsed;
                TbiOCR.Visibility = Visibility.Collapsed;
                Height = 350;
                TbcPreferencias.Height = 240;
                Width = 350;
                LblStepDuration.Visibility = Visibility.Collapsed;
                TxtStepDuration.Visibility = Visibility.Collapsed;
                LblStepDurationSeconds.Visibility = Visibility.Collapsed;
                LblShowProgress.Visibility = Visibility.Collapsed;
                LblShowTime.Visibility = Visibility.Collapsed;
                ChkShowStepProgress.Visibility = Visibility.Collapsed;
                ChkShowTime.Visibility = Visibility.Collapsed;
                CambióResolución = true; // Para que haga la verificación inicial de la resolución después de autodetectarla.

            }

        } // SettingsWindow>


        public void CargarValores(bool primerInicio) {

            CmbResolution.Text = Preferencias.ScreenResolution;
            CmbGame.Text = Preferencias.Game;
            CmbGameSpeed.Text = Preferencias.ObtenerGameSpeedText(Preferencias.Game);
            TxtOpacity.Text = Preferencias.Opacity.ToString();
            TxtStepFontSize.Text = Preferencias.CurrentStepFontSize.ToString();
            TxtNextPreviousStepFontSize.Text = Preferencias.NextPreviousStepFontSize.ToString();
            ChkShowNextStep.IsChecked = Preferencias.ShowNextStep;
            ChkShowPreviousStep.IsChecked = Preferencias.ShowPreviousStep;

            LeerSonidos();
            CmbStartSound.Text = Preferencias.StepStartSound;
            CmbEndSound.Text = Preferencias.StepEndSound;
            if (CmbStartSound.SelectedIndex == -1) CmbStartSound.Text = NoneSoundString;
            if (CmbEndSound.SelectedIndex == -1) CmbEndSound.Text = NoneSoundString;
            ChkUnmuteAtStartup.IsChecked = Preferencias.UnmuteAtStartup;
            SldEndSoundVolume.Value = Preferencias.StepEndSoundVolume;
            SldStartSoundVolume.Value = Preferencias.StepStartSoundVolume;
            
            ChkMinimizeOnComplete.IsChecked = Preferencias.MinimizeOnComplete;
            ChkMuteOnComplete.IsChecked = Preferencias.MuteOnComplete;
            ChkStopFlashingOnComplete.IsChecked = Preferencias.StopFlashingOnComplete;
            ChkFlashOnStepChange.IsChecked = Preferencias.FlashOnStepChange;
            TxtFlashingOpacity.Text = Preferencias.FlashingOpacity.ToString();
            TxtStepDuration.Text = Preferencias.StepDuration.ToString();

            CmbFontName.Text = Preferencias.FontName;
            CmbSecondaryFontName.Text = Preferencias.SecondaryFontName;
            ChkCurrentStepFontBold.IsChecked = Preferencias.CurrentStepFontBold;
            ChkNextPreviousStepFontBold.IsChecked = Preferencias.NextPreviousStepFontBold;

            TxtLineSpacing.Text = Preferencias.LineSpacing.ToString();
            TxtImageSize.Text = Preferencias.ImageSize.ToString();
            TxtImageBackgroundOpacity.Text = Preferencias.ImageBackgroundOpacity.ToString();
            TxtImageHorizontalMargin.Text = Preferencias.EntityHorizontalMargin.ToString();
            TxtImageBackgroundRoundedCornersRadius.Text = Preferencias.ImageBackgroundRoundedCornersRadius.ToString();
            TxtSubscriptAndSuperscriptImagesSize.Text = Preferencias.SubscriptAndSuperscriptImagesSize.ToString();
            CargarPrioridadesNombres();

            ChkOverrideFlashingColor.IsChecked = Preferencias.OverrideFlashingColor;
            ChkOverrideFlashingOpacity.IsChecked = Preferencias.OverrideFlashingOpacity;
            ChkOverrideFlashOnStepChange.IsChecked = Preferencias.OverrideFlashOnStepChange;
            ChkOverrideFontBold.IsChecked = Preferencias.OverrideFontBold;
            ChkOverrideFontColor.IsChecked = Preferencias.OverrideFontColor;
            ChkOverrideFontItalics.IsChecked = Preferencias.OverrideFontItalics;
            ChkOverrideFontName.IsChecked = Preferencias.OverrideFontName;
            ChkOverrideFontPosition.IsChecked = Preferencias.OverrideFontPosition;
            ChkOverrideFontSize.IsChecked = Preferencias.OverrideFontSize;
            ChkOverrideFontUnderline.IsChecked = Preferencias.OverrideFontUnderline;
            ChkOverrideImageSize.IsChecked = Preferencias.OverrideImageSize;
            ChkOverrideShowNextPreviousStep.IsChecked = Preferencias.OverrideShowNextPreviousStep;
            ChkOverrideStepDuration.IsChecked = Preferencias.OverrideStepDuration;
            ChkOverrideStepEndSound.IsChecked = Preferencias.OverrideStepEndSound;
            ChkOverrideStepEndSoundVolume.IsChecked = Preferencias.OverrideStepEndSoundVolume;
            ChkOverrideStepStartSound.IsChecked = Preferencias.OverrideStepStartSound;
            ChkOverrideStepStartSoundVolume.IsChecked = Preferencias.OverrideStepStartSoundVolume;
            ChkShowStepProgress.IsChecked = Preferencias.ShowStepProgress;
            ChkShowTime.IsChecked = Preferencias.ShowTime;
            ChkRandomImageForMultipleImages.IsChecked = Preferencias.RandomImageForMultipleImages;
            ChkCapitalizeNames.IsChecked = Preferencias.CapitalizeNames;

            ChkAutoadjustIdleTime.IsChecked = Preferencias.AutoAdjustIdleTime;
            ChkPauseDetection.IsChecked = Preferencias.PauseDetection;
            ChkShowAddIdleTime.IsChecked = Preferencias.ShowAddIdleTimeButton;
            ChkShowRemoveIdleTime.IsChecked = Preferencias.ShowRemoveIdleTimeButton;
            TxtAutoadjustIdleTimeInterval.Text = Preferencias.AutoAdjustIdleTimeInterval.ToString();
            TxtPauseDetectionInterval.Text = Preferencias.PauseDetectionInterval.ToString();
            TxtAddIdleTimeSeconds.Text = Preferencias.AddIdleTimeSeconds.ToString();
            TxtRemoveIdleTimeSeconds.Text = Preferencias.RemoveIdleTimeSeconds.ToString();
            TxtMinimumDelayToAutoAdjustIdleTime.Text = Preferencias.MinimumDelayToAutoAdjustIdleTime.ToString();
            TxtBackMultipleSteps.Text = Preferencias.BackMultipleSteps.ToString();
            TxtNextMultipleSteps.Text = Preferencias.NextMultipleSteps.ToString();
            TxtFordwardSeconds.Text = Preferencias.ForwardSeconds.ToString();
            TxtBackwardSeconds.Text = Preferencias.BackwardSeconds.ToString();
            ChkShowAlwaysStatsButton.IsChecked = Preferencias.ShowAlwaysStatsButton;
            ChkShowAlternateNextPreviousStepButton.IsChecked = Preferencias.ShowAlternateNextPreviousStepButton;
            ChkOCRTestMode.IsChecked = Preferencias.OCRTestMode;
            ChkShowOptionalInstructions1.IsChecked = Preferencias.ShowOptionalInstructions1;
            ChkShowOptionalInstructions2.IsChecked = Preferencias.ShowOptionalInstructions2;

            AgregarIdiomas(CmbGameLanguage, Preferencias.GameLanguage, IdiomasJuego);
            AgregarUIMods(Preferencias.UIMod);
            CargarVelocidadEjecución();
            CargarEscalaInterface();

            if (!primerInicio) VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en los botones.

        } // CargarValores>


        private void TimerDetecciónPruebasOCR_Tick(object? sender, EventArgs e) {

            if (MostrandoPreferenciasOCR) {

                if (Preferencias.Game == AOE2Name ) {

                    void obtenerBitmapYMostrar(ScreenCaptureText tipoRectángulo) {

                        using var bmp = CapturaDePantalla.ObtenerBitmap(ObtenerRectángulo(tipoRectángulo), false, false, 1, 1, 1, 
                            InterpolationMode.HighQualityBicubic, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        if (RectángulosImágenesPrueba.ContainsKey(tipoRectángulo))
                            RectángulosImágenesPrueba[tipoRectángulo].Source = ObtenerImageSource(bmp);

                    } // obtenerBitmapYMostrar>

                    obtenerBitmapYMostrar(ObtenerTipoPausa());
                    obtenerBitmapYMostrar(ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9);
                    obtenerBitmapYMostrar(ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99);
                    obtenerBitmapYMostrar(ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999);
                    obtenerBitmapYMostrar(ScreenCaptureText.Age_of_Empires_II_Game_Start);

                } else if (Preferencias.Game == AOMName) {

                    void obtenerBitmapYMostrar(ScreenCaptureText tipoRectángulo) {

                        using var bmp = CapturaDePantalla.ObtenerBitmap(ObtenerRectángulo(tipoRectángulo), false, false, 1, 1, 1,
                            InterpolationMode.HighQualityBicubic, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        if (RectángulosImágenesPrueba.ContainsKey(tipoRectángulo))
                            RectángulosImágenesPrueba[tipoRectángulo].Source = ObtenerImageSource(bmp);

                    } // obtenerBitmapYMostrar>

                    obtenerBitmapYMostrar(ObtenerTipoPausa());
                    obtenerBitmapYMostrar(ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9);
                    obtenerBitmapYMostrar(ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99);
                    obtenerBitmapYMostrar(ScreenCaptureText.Age_of_Mythology_Game_Start);

                }
                return;

            }

        } // TimerDetecciónPruebasOCR_Tick>


        private void TbiCustomNames_Selected(object sender, RoutedEventArgs e) => CargarNombresPersonalizados();


        private void TbiPersonalization_Selected(object sender, RoutedEventArgs e) {

            if (Preferencias.Game == AOE2Name || Preferencias.Game == AOE4Name) {

                LblOptionalInstructions.Visibility = Visibility.Visible;
                LblShowOptionalInstructions1.Visibility = Visibility.Visible;
                LblShowOptionalInstructions2.Visibility = Visibility.Visible;
                ChkShowOptionalInstructions1.Visibility = Visibility.Visible;
                ChkShowOptionalInstructions2.Visibility = Visibility.Visible;
                LblShowOptionalInstructions1.Content = "Show Villagers: ";
                LblShowOptionalInstructions2.Content = "Show Houses: ";

            } else { // Para Age of Mythology no es necesario las opciones de esconder instrucciones de nuevas casas y aldeanos.

                LblOptionalInstructions.Visibility = Visibility.Hidden;
                LblShowOptionalInstructions1.Visibility = Visibility.Hidden;
                LblShowOptionalInstructions2.Visibility = Visibility.Hidden;
                ChkShowOptionalInstructions1.Visibility = Visibility.Hidden;
                ChkShowOptionalInstructions2.Visibility = Visibility.Hidden;

            }

        } // TbiPersonalization_Selected>


        private void TbiCustomNames_Unselected(object sender, RoutedEventArgs e) => GuardarNombresPersonalizados(cerrando: false);


        private void TbiOCR_Selected(object sender, RoutedEventArgs e) => EntrarAPreferenciasOCR();


        private void TbiOCR_Unselected(object sender, RoutedEventArgs e) => SalirDePreferenciasOCR(cerrando: false);


        private void SalirDePreferenciasOCR(bool cerrando) {

            TimerPruebasOCR.Stop();
            MostrandoPreferenciasOCR = false;
            RectángulosImágenesPrueba = new Dictionary<ScreenCaptureText, Controles.Image>();
            GuardarRectángulosOCR(cerrando);

        } // SalirDePreferenciasOCR>


        private void EntrarAPreferenciasOCR() {

            TimerPruebasOCR.Start();
            MostrandoPreferenciasOCR = true;
            CargarRectángulosOCR();

        } // EntrarAPreferenciasOCR>


        private void GuardarNombresPersonalizados(bool cerrando) {

            if (NombresPersonalizados != null && UnNombrePersonalizadoCambiado) {

                File.WriteAllText(Preferencias.CustomNamesPath, SerializarNombres(NombresPersonalizados));
                CrearNombresYAgregarAEntidades();
                if (!cerrando) VentanaPrincipal.AplicarPreferencias();
                UnNombrePersonalizadoCambiado = false;

            }

        } // GuardarNombresPersonalizados>


        private void ChkOverride_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            GuardarOverrides();
            VentanaPrincipal.AplicarPreferencias();

        } // ChkOverride_Checked>


        private void GuardarOverrides() {

            Preferencias.OverrideFlashingColor = (bool)ChkOverrideFlashingColor.IsChecked!;
            Preferencias.OverrideFlashingOpacity = (bool)ChkOverrideFlashingOpacity.IsChecked!;
            Preferencias.OverrideFlashOnStepChange = (bool)ChkOverrideFlashOnStepChange.IsChecked!;
            Preferencias.OverrideFontBold = (bool)ChkOverrideFontBold.IsChecked!;
            Preferencias.OverrideFontColor = (bool)ChkOverrideFontColor.IsChecked!;
            Preferencias.OverrideFontItalics = (bool)ChkOverrideFontItalics.IsChecked!;
            Preferencias.OverrideFontName = (bool)ChkOverrideFontName.IsChecked!;
            Preferencias.OverrideFontPosition = (bool)ChkOverrideFontPosition.IsChecked!;
            Preferencias.OverrideFontSize = (bool)ChkOverrideFontSize.IsChecked!;
            Preferencias.OverrideFontUnderline = (bool)ChkOverrideFontUnderline.IsChecked!;
            Preferencias.OverrideImageSize = (bool)ChkOverrideImageSize.IsChecked!;
            Preferencias.OverrideShowNextPreviousStep = (bool)ChkOverrideShowNextPreviousStep.IsChecked!;
            var overrideStepDuration = (bool)ChkOverrideStepDuration.IsChecked!;
            if (!overrideStepDuration) 
                MostrarInformación("If you don't allow build orders to override the default step duration and they have custom steps durations, " + 
                    "their instructions won't be syncronized. It's recommended to leave this option checked.");
            Preferencias.OverrideStepDuration = overrideStepDuration;

            Preferencias.OverrideStepEndSound = (bool)ChkOverrideStepEndSound.IsChecked!;
            Preferencias.OverrideStepEndSoundVolume = (bool)ChkOverrideStepEndSoundVolume.IsChecked!;
            Preferencias.OverrideStepStartSound = (bool)ChkOverrideStepStartSound.IsChecked!;
            Preferencias.OverrideStepStartSoundVolume = (bool)ChkOverrideStepStartSoundVolume.IsChecked!;
            VentanaPrincipal.VerificarModoDesarrolloYCargarBuildOrder();

        } // GuardarOverrides>


        private void CargarNombresPersonalizados() {

            SpnCustomNames.Children.Clear();
            NombresPersonalizados = DeserializarNombres(Preferencias.CustomNamesPath);
            var nombresCompletosConPersonalizado = new List<string>();

            SpnCustomNames.Width = 530;

            var tb = new Controles.TextBlock() { TextWrapping = TextWrapping.Wrap, HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10) };
            var enlaceNombres = new Hyperlink();
            if (Preferencias.Game == AOE2Name) {

                enlaceNombres.NavigateUri = new Uri("https://docs.google.com/spreadsheets/d/1N98XMYNNlUOlA45B3NKTJ54Fhuo53PFYSsnt_F2S7nU");
                enlaceNombres.RequestNavigate += Lnk_Click;
                enlaceNombres.Inlines.Add($"See here the supported names for {Preferencias.Game}.");

            } else if (Preferencias.Game == AOMName) {

                enlaceNombres.NavigateUri = new Uri("https://docs.google.com/spreadsheets/d/1YekY3hWAK08tWPWndnDq7AdAKJfLo-8x6VCeyv8Dp4o");
                enlaceNombres.RequestNavigate += Lnk_Click;
                enlaceNombres.Inlines.Add($"See here the supported names for {Preferencias.Game}.");

            }

            tb.Inlines.Add("Most names that you'd need for your build orders are supported by default. ");
            tb.Inlines.Add(enlaceNombres);
            tb.Inlines.Add(" Custom names are extra names that you can use in your build orders, but they only work in your local computer. If you share your build order using those names with other people, it won't work in their computers.");
            SpnCustomNames.Children.Add(tb);

            foreach (var kv in NombresPersonalizados[NameType.Custom].ToList().OrderBy(kv => kv.Key)) {

                var nombreCompleto = kv.Key;
                var nombrePersonalizado = kv.Value;
                var id = Entidades.FirstOrDefault(e => e.Value.NombreCompleto == nombreCompleto).Key;
                var SpnCustomName = new StackPanel() { Orientation = Controles.Orientation.Horizontal };
                var label = new Controles.Label() { VerticalAlignment = VerticalAlignment.Center, Content = nombreCompleto , Tag = id, 
                    Width = 150, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right, Margin = new Thickness(0, 0, 10, 0), 
                    ToolTip = nombreCompleto };
                var textbox = new Controles.TextBox() { Text = nombrePersonalizado, Width = 70, Tag = nombreCompleto,
                    VerticalAlignment = VerticalAlignment.Center };
                var buttonBorrar = new Controles.Button() { Content = "X", FontWeight = FontWeights.Normal, Tag = nombreCompleto, 
                    Margin = new Thickness(5, 0, 0, 0), Padding = new Thickness(5, 2, 5, 2), VerticalAlignment = VerticalAlignment.Center };

                buttonBorrar.Click += new RoutedEventHandler(this.BtnBorrarNombrePersonalizado_Click);
                textbox.TextChanged += new Controles.TextChangedEventHandler(this.TxtCustomName_TextChanged);
                nombresCompletosConPersonalizado.Add(nombreCompleto.ToLowerInvariant());

                SpnCustomName.Children.Add(label);
                SpnCustomName.Children.Add(textbox);
                SpnCustomName.Children.Add(buttonBorrar);
                SpnCustomNames.Children.Add(SpnCustomName);

            }

            var comboboxNuevo = new Controles.ComboBox() { Width = 150, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0) };
            var textboxNuevo = new Controles.TextBox() { Width = 70, VerticalAlignment = VerticalAlignment.Center };
            var nombresCompletosSinPersonalizado = TodosLosNombres.Where(tupla => tupla.Item2.Tipo == NameType.Complete
                && !nombresCompletosConPersonalizado.Contains(tupla.Item1));
            var buttonNuevo = new Controles.Button() { Content = "Add", Tag = (comboboxNuevo, textboxNuevo), Margin = new Thickness(5, 0, 0, 0), 
                Padding = new Thickness(2), VerticalAlignment = VerticalAlignment.Center };
            buttonNuevo.Click += new RoutedEventHandler(this.BtnNuevoNombrePersonalizado_Click);

            foreach (var tupla in nombresCompletosSinPersonalizado.OrderBy(n => n.Item1)) {

                var nombreCompletoCapitalizaciónCorrecta 
                    = Nombres.FirstOrDefault(t => t.Value.ID == tupla.Item2.ID && t.Value.Tipo == NameType.Complete).Value?.Valor;
                if (!string.IsNullOrEmpty(nombreCompletoCapitalizaciónCorrecta)) comboboxNuevo.Items.Add(nombreCompletoCapitalizaciónCorrecta);

            }

            var SpnCustomNameNuevo = new StackPanel() { Orientation = Controles.Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            SpnCustomNameNuevo.Children.Add(comboboxNuevo);
            SpnCustomNameNuevo.Children.Add(textboxNuevo);
            SpnCustomNameNuevo.Children.Add(buttonNuevo);
            SpnCustomNames.Children.Add(SpnCustomNameNuevo);

            SpnCustomNames.Children.Add(new Controles.TextBlock() { Margin = new Thickness(0, 10, 0, 10), TextWrapping = TextWrapping.Wrap,
                Text = "To display them instead of images, go to 'Display Priority' section and move 'Custom Name' to the top. While displaying custom names, you can add special custom names between squared brackets and instead of a custom name it will display the name from that type. Supported values are: [Image], [Abbreviation], [Acronym], [Common], [Complete], [AbbreviationPlural], [AcronymPlural] and [CommonPlural]."
            });

        } // CargarNombresPersonalizados>


        private void BtnBorrarNombrePersonalizado_Click(object sender, RoutedEventArgs e) {

            var nombreCompleto = (string)((Controles.Button)e.Source).Tag;
            if (!string.IsNullOrEmpty(nombreCompleto) && NombresPersonalizados != null
                && NombresPersonalizados[NameType.Custom].ContainsKey(nombreCompleto)) {

                NombresPersonalizados[NameType.Custom].Remove(nombreCompleto);
                UnNombrePersonalizadoCambiado = true;
                GuardarNombresPersonalizados(cerrando: false);
                CargarNombresPersonalizados();

            }

        } // BtnBorrarNombrePersonalizado_Click>


        private void BtnNuevoNombrePersonalizado_Click(object sender, RoutedEventArgs e) {

            var tupla = ((ValueTuple<Controles.ComboBox, Controles.TextBox>)((Controles.Button)e.Source).Tag);
            var comboboxNuevo = tupla.Item1;
            var nombrePersonalizado = tupla.Item2?.Text;
            if (comboboxNuevo.SelectedItem == null) return; 
            var nuevoNombre = comboboxNuevo.SelectedItem.ToString();
            if (NombresPersonalizados != null && nuevoNombre != null && !string.IsNullOrEmpty(nombrePersonalizado) 
                && !NombresPersonalizados[NameType.Custom].ContainsKey(nuevoNombre)) {

                if (nombrePersonalizado.StartsWith("[") || !NombresPersonalizados[NameType.Custom].Values.Contiene(nombrePersonalizado)) {

                    NombresPersonalizados[NameType.Custom].Add(nuevoNombre, nombrePersonalizado);
                    UnNombrePersonalizadoCambiado = true;
                    GuardarNombresPersonalizados(cerrando: false);
                    CargarNombresPersonalizados();

                } else {
                    MostrarError($"Custom name {nombrePersonalizado} already exists.");
                }

            }

        } // BtnNuevoNombrePersonalizado_Click>


        private void TxtCustomName_TextChanged(object sender, TextChangedEventArgs e) {

            var textbox = (Controles.TextBox)sender;
            var nombreCompleto = textbox.Tag?.ToString();
            if (NombresPersonalizados != null && nombreCompleto != null) NombresPersonalizados[NameType.Custom][nombreCompleto] = textbox.Text;
            UnNombrePersonalizadoCambiado = true;

        } // TxtCustomName_TextChanged>


        private void CargarRectángulosOCR() {
  
            RectángulosImágenesPrueba.Clear();

            void agregarControlesRectángulo(ScreenCaptureText rectángulo, string nombreRectángulo) {

                if (Preferencias.ScreenCaptureRectangles == null) return;

                var spnRect = new StackPanel() { Orientation = Controles.Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0), Tag = rectángulo };
                var lblTítulo = new Controles.Label() { Content = $"{nombreRectángulo}: ", FontWeight = FontWeights.Bold, Width = 125,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var lblX = new Controles.Label() { Content = "X:", VerticalAlignment = VerticalAlignment.Center };
                var txtX = new Controles.TextBox() { Text = Preferencias.ScreenCaptureRectangles[rectángulo].X.ToString(), Width = 39, Height = 25,
                    VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0), 
                    VerticalAlignment = VerticalAlignment.Center, Tag = "X"
                };
                var lblY = new Controles.Label() { Content = "Y:", VerticalAlignment = VerticalAlignment.Center };
                var txtY = new Controles.TextBox() { Text = Preferencias.ScreenCaptureRectangles[rectángulo].Y.ToString(), Width = 39, Height = 25,
                    VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0), 
                    VerticalAlignment = VerticalAlignment.Center, Tag = "Y"
                };
                var lblWidth = new Controles.Label() { Content = "W:", VerticalAlignment = VerticalAlignment.Center };
                var txtWidth = new Controles.TextBox() { Text = Preferencias.ScreenCaptureRectangles[rectángulo].Width.ToString(), Width = 39, 
                    Height = 25, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0), 
                    VerticalAlignment = VerticalAlignment.Center, Tag = "Width"
                };
                var lblHeight = new Controles.Label() { Content = "H:", VerticalAlignment = VerticalAlignment.Center };
                var txtHeight = new Controles.TextBox() {
                    Text = Preferencias.ScreenCaptureRectangles[rectángulo].Height.ToString(), Width = 39,
                    Height = 25, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center, Tag = "Height"
                };

                var img = new Controles.Image() { Height = 60, Width = 120, MaxHeight = 60, MaxWidth = 120, MinHeight = 60, MinWidth = 120 };
                spnRect.Children.Add(lblTítulo);
                spnRect.Children.Add(lblX);
                spnRect.Children.Add(txtX);
                spnRect.Children.Add(lblY);
                spnRect.Children.Add(txtY);
                spnRect.Children.Add(lblHeight);
                spnRect.Children.Add(txtHeight);
                spnRect.Children.Add(lblWidth);
                spnRect.Children.Add(txtWidth);
                spnRect.Children.Add(img);
                SpnRectangles.Children.Add(spnRect);
                RectángulosImágenesPrueba.Add(rectángulo, img);
                var manejadorEvento = new System.Windows.Controls.TextChangedEventHandler(CambióRectángulo);
                txtHeight.TextChanged += manejadorEvento;
                txtX.TextChanged += manejadorEvento;
                txtY.TextChanged += manejadorEvento;
                txtWidth.TextChanged += manejadorEvento;

            } // agregarControlesRectángulo>

            SpnRectangles.Children.Clear();
            foreach (var kv in ObtenerRectángulosActivos(Preferencias.Game)) {
                agregarControlesRectángulo(kv.Key, kv.Value.Reemplazar("_", " ").Reemplazar(Preferencias.Game, "").Trim());
            }

        } // CargarRectángulosOCR>


        private bool RectángulosSonLosRecomendados() {

            foreach (var kv in ObtenerRectángulosActivos(Preferencias.Game)) {

                var tipo = kv.Key;
                var rectánguloRecomendado = ObtenerRectánguloRecomendado(tipo, Preferencias.ScreenResolution, Preferencias.UIMod.ToString());
                var rectángulo = ObtenerRectánguloSinAjustes(tipo);
                if (rectánguloRecomendado != rectángulo && rectángulo != null) return false;

            }

            return true;

        } // VerificarSiRectángulosSonLosRecomendados>


        private Dictionary<ScreenCaptureText, string> ObtenerRectángulosActivos(string juego) {

            var rectángulosActivos = new Dictionary<ScreenCaptureText, string>();
            var rectánguloPausa = ObtenerTipoPausa(); // En el ciclo siguiente se ignoran todos los que tengan Pause en el nombre del tipo.
            rectángulosActivos.Add(rectánguloPausa, "Pause");

            foreach (var rectángulo in RectángulosActivos) {

                var nombreRectángulo = rectángulo.ToString();
                if (!nombreRectángulo.Contains("Pause") && nombreRectángulo.Contains(juego.Reemplazar(" ", "_"))) {
                    rectángulosActivos.Add(rectángulo, nombreRectángulo);
                }

            }
            return rectángulosActivos;

        } // ObtenerRectángulosActivos>


        private void CambióRectángulo(object sender, TextChangedEventArgs e) 
            => GuardarRectángulosOCR(cerrando: true);


        private void GuardarRectángulosOCR(bool cerrando) {

            if (Preferencias.ScreenCaptureRectangles == null) return;

            foreach (var control in SpnRectangles.Children) {

                var spnRect = control as StackPanel;
                if (spnRect != null) {

                    var tipoRectángulo = (ScreenCaptureText)spnRect.Tag;
                    var rectángulo = Preferencias.ScreenCaptureRectangles[tipoRectángulo];
                    foreach (var control2 in spnRect.Children) {

                        var txt = control2 as Controles.TextBox;
                        if (txt != null) {

                            switch (txt.Tag.ToString()) {
                                case "X":
                                    if (float.TryParse(ObtenerTextoNúmeroLocal(txt.Text), out float x) && x >= 0 && x <= 1) rectángulo.X = x;
                                    break;
                                case "Y":
                                    if (float.TryParse(ObtenerTextoNúmeroLocal(txt.Text), out float y) && y >= 0 && y <= 1) rectángulo.Y = y;
                                    break;
                                case "Height":
                                    if (float.TryParse(ObtenerTextoNúmeroLocal(txt.Text), out float height) && height > 0 && height <= 1) 
                                        rectángulo.Height = height;
                                    break;
                                case "Width":
                                    if (float.TryParse(ObtenerTextoNúmeroLocal(txt.Text), out float width) && width > 0 && width <= 1) 
                                        rectángulo.Width = width;
                                    break;
                                default:
                                    MostrarError("Unexpected value in GuardarRectángulosOCR()");
                                    break;
                            }

                        }

                    } // foreach (var control2 in spnRect.Children).

                    Preferencias.ScreenCaptureRectangles[tipoRectángulo] = rectángulo;

                }

            } // foreach (var control in SpnRectangles.Children.

            if (!cerrando) VentanaPrincipal.AplicarPreferencias();

        } // GuardarRectángulosOCR>


        private void CargarPrioridadesNombres() {

            SpnDisplayPriority.Children.Clear();
            var encontradoPrimerIdioma = false;
            var idiomaEncontrado = "";
            var cuenta = 0;
            var totalNombresSinIdiomas = Preferencias.DisplayPriority.Count - IdiomasNombres.Count + 1; // Se suma 1 porque siempre se muestra el idioma de más prioridad.
            var displayPriorityOrdenadas = Preferencias.ObtenerDisplayPriorityOrdenadas();
            
            foreach (var displayPriority in displayPriorityOrdenadas) {

                var esIdioma = false;
                var textoNombre = displayPriority.Key.ToString();
                if (IdiomasNombres.ContainsKey(textoNombre)) {

                    if (encontradoPrimerIdioma) continue;
                    idiomaEncontrado = textoNombre;
                    encontradoPrimerIdioma = true;
                    esIdioma = true;

                }
                var spnName = new StackPanel() { Orientation = Controles.Orientation.Horizontal };
                var label = new Controles.Label() {
                    VerticalAlignment = VerticalAlignment.Center,
                    Content = esIdioma ? "Other Language" : displayPriority.Key.ATexto(), Tag = displayPriority
                };
                var btnUp = new Controles.Button() { Margin = new Thickness(4), Height = 22, Width = 22, Content = "↑", Tag = (-1, label) };
                var btnDown = new Controles.Button() { Margin = new Thickness(4), Height = 22, Width = 22, Content = "↓", Tag = (1, label) };
                btnUp.Click += new System.Windows.RoutedEventHandler(this.BtnMovePriority_Click);
                btnDown.Click += new System.Windows.RoutedEventHandler(this.BtnMovePriority_Click);

                if (cuenta <= 0) btnUp.IsEnabled = false;
                if (cuenta >= totalNombresSinIdiomas - 1) btnDown.IsEnabled = false;

                spnName.Children.Add(btnUp);
                spnName.Children.Add(btnDown);
                spnName.Children.Add(label);

                if (esIdioma) { // Solo pasa por aquí cuando encuentra el primero.

                    var cmbIdioma = new Controles.ComboBox() { VerticalAlignment = VerticalAlignment.Center };
                    AgregarIdiomas(cmbIdioma, idiomaEncontrado, IdiomasNombres);
                    cmbIdioma.SelectionChanged += new Controles.SelectionChangedEventHandler(this.CmbOtherLanguage_SelectionChanged);
                    spnName.Children.Add(cmbIdioma);

                }
                SpnDisplayPriority.Children.Add(spnName);
                cuenta++;

            }

        } // CargarPrioridadesNombres>


        private void AgregarUIMods(string anteriorUIMod) {

            ActualizandoUIMods = true;
            CmbUIMod.Items.Clear();

            if (UIMods.ContainsKey(Preferencias.Game)) {

                ComboBoxItem? selectedCbi = null;
                ComboBoxItem? noModCbi = null;
                foreach (var mod in UIMods[Preferencias.Game]) {

                    var cmi = new ComboBoxItem() { Content = mod.ToString().Replace("_", " "), Tag = mod };
                    CmbUIMod.Items.Add(cmi);
                    if (mod.ToString().ToLower() == Preferencias.UIMod.ToLower()) selectedCbi = cmi;
                    if (mod.ToString() == UIMod.No_Mod.ToString()) noModCbi = cmi;

                }

                if (selectedCbi != null) {
                    CmbUIMod.SelectedItem = selectedCbi;
                } else {
                    CmbUIMod.SelectedItem = noModCbi;
                }

            } else {

                var cbi = new ComboBoxItem() { Content = UIMod.No_Mod.ToString().Replace("_", " "), Tag = UIMod.No_Mod };
                CmbUIMod.Items.Add(cbi);
                CmbUIMod.SelectedItem = cbi;

            }

            if (CmbUIMod.SelectedItem is ComboBoxItem cbi2) Preferencias.UIMod = cbi2.Tag?.ToString() ?? UIMod.No_Mod.ToString();

            if (anteriorUIMod != Preferencias.UIMod) 
                Preferencias.EstablecerValoresRecomendados(Preferencias.ScreenResolution, Preferencias.Game, cambióResolución: false, cambióUIMod: true, 
                    cambióJuego: false);

            ActualizandoUIMods = false;

        } // AgregarUIMods>


        private void AgregarIdiomas(Controles.ComboBox cmbIdioma, string idiomaSeleccionado, Dictionary<string, string> idiomas) {

            foreach (var kv in idiomas) {

                var idioma = kv.Key;
                var nombreIdioma = kv.Value;
                var cmi = new ComboBoxItem() { Content = nombreIdioma, Tag = idioma };
                if (idioma == idiomaSeleccionado) cmi.IsSelected = true;
                cmbIdioma.Items.Add(cmi);

            }

        } // AgregarIdiomas>


        private void CmbOtherLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            var nombreIdiomaTexto = "";
            var cbi = e.AddedItems[0] as ComboBoxItem;
            if (cbi != null) {
                nombreIdiomaTexto = cbi.Tag.ToString() ?? "";
            } else {
                return;
            }

            foreach (var displayPriority in Preferencias.ObtenerDisplayPriorityOrdenadas()) { // Busca el idioma actualmente en el top 10 y lo reemplaza por el seleccionado.

                var prioridad = displayPriority.Value;
                var nombre = displayPriority.Key;
                var textoNombre = nombre.ToString();
                if (IdiomasNombres.ContainsKey(textoNombre)) {

                    if (textoNombre != nombreIdiomaTexto) { // Si es el mismo actualmente seleccionado, no pasa nada.

                        var idioma = (NameType)Enum.Parse(typeof(NameType), nombreIdiomaTexto); 
                        var prioridadAnterior = Preferencias.DisplayPriority[idioma];
                        Preferencias.DisplayPriority[idioma] = prioridad;
                        Preferencias.DisplayPriority[nombre] = prioridadAnterior;

                    }
                    break;

                }

            }

            CargarPrioridadesNombres();
            VentanaPrincipal.AplicarPreferencias();

        } // CmbStartSound_SelectionChanged>


        private void BtnMovePriority_Click(object sender, RoutedEventArgs e) {

            var información = (ValueTuple<int, Controles.Label>)((Controles.Button)e.Source).Tag;
            var cantidadAMover = información.Item1;
            var label = información.Item2;
            var kv = (KeyValuePair<NameType, int>)label.Tag;
            var prioridadActual = kv.Value;
            var nombreActual = kv.Key;
            var encontradoPrimerIdioma = false;
            var desfacePrioridad = 0;

            foreach (var displayPriority in Preferencias.ObtenerDisplayPriorityOrdenadas()) {

                var prioridad = displayPriority.Value;
                var nombre = displayPriority.Key;
                var textoNombre = nombre.ToString();

                if (IdiomasNombres.ContainsKey(textoNombre)) {

                    if (encontradoPrimerIdioma) {
                        desfacePrioridad++;
                        continue;   
                    }
                    encontradoPrimerIdioma = true;

                }

                if (prioridad == prioridadActual) {
                    Preferencias.DisplayPriority[nombre] += cantidadAMover;
                } else if (cantidadAMover == -1 && prioridad - desfacePrioridad == prioridadActual - 1) {
                    Preferencias.DisplayPriority[nombre] += -cantidadAMover;
                } else if (cantidadAMover == 1 && prioridad - desfacePrioridad == prioridadActual + 1) {
                    Preferencias.DisplayPriority[nombre] += -cantidadAMover;
                }

            }
            
            CargarPrioridadesNombres();
            VentanaPrincipal.AplicarPreferencias();

        } // BtnMovePriority_Click>


        public void LeerSonidos() {

            CmbStartSound.Items.Clear();
            CmbEndSound.Items.Clear();
            CmbStartSound.Items.Add(NoneSoundString);
            CmbEndSound.Items.Add(NoneSoundString);
            CmbStartSound.Text = NoneSoundString;
            CmbEndSound.Text = NoneSoundString;
            var extensionesAudio = new List<string> { ".mp3", ".wav", ".wma", ".ogg", ".acc", ".flac" };

            var rutasSonidosCortos = Directory.GetFiles(DirectorioSonidosCortos); // Si saca error aquí, posiblemente es que se olvidó reactivar ModoDesarrollo.
            foreach (var rutaSonidoCorto in rutasSonidosCortos) {
                var extensión = Path.GetExtension(rutaSonidoCorto).ToLower();
                if (extensionesAudio.Contains(extensión) || string.IsNullOrEmpty(extensión)) 
                    CmbStartSound.Items.Add(Path.GetFileName(rutaSonidoCorto));
            }

            var rutasSonidosLargos = Directory.GetFiles(DirectorioSonidosLargos);
            foreach (var rutaSonidoLargo in rutasSonidosLargos) {
                if (extensionesAudio.Contains(Path.GetExtension(rutaSonidoLargo).ToLower())) 
                    CmbEndSound.Items.Add(Path.GetFileName(rutaSonidoLargo));
            }

        } // LeerSonidos>


        public static string ToHexString(System.Drawing.Color c) 
            => $"#{c.R:X2}{c.G:X2}{c.B:X2}";


        public static System.Drawing.Color ObtenerDrawingColor(System.Windows.Media.Brush brush) {
            var color = ((SolidColorBrush)brush).Color;
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        } // ObtenerDrawingColor>


        private void BtnBackcolor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnBackcolor.Background);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Preferencias.BackColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
            }

        } // BtnBackcolor_Click>


        private void BtnFontcolor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnFontcolor.Foreground);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Preferencias.FontColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
            }

        } // BtnFontcolor_Click>


        private void BtnStepFontColor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnStepFontColor.Foreground);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Preferencias.CurrentStepFontColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
            }

        } // BtnStepFontColor_Click>


        private void BtnNextPreviousStepFontColor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnNextPreviousStepFontColor.Foreground);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Preferencias.NextPreviousStepFontColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
            }

        } // BtnNextPreviousStepFontColor_Click>


        private void BtnSave_Click(object sender, RoutedEventArgs e) => this.Close();


        private void Window_Closed(object sender, EventArgs e) {

            if (CambióResolución) VerificarResolución(); 
            GuardarNombresPersonalizados(cerrando: true);
            SalirDePreferenciasOCR(cerrando: true);
            VentanaPrincipal.AplicarPreferencias();
            Settings.Guardar(Preferencias, RutaPreferencias);
            if (ActualizarDuraciónPasoAlSalir) VentanaPrincipal.ActualizarDuraciónPaso(); // Se aplica solo al salir para no reiniciar el timer cada vez que se haga un cambio en los controles.

        } // Window_Closed>


        private void CmbGame_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.Game = ObtenerSeleccionadoEnCombobox(e);
            AgregarUIMods(Preferencias.UIMod);
            Preferencias.EstablecerValoresRecomendados(Preferencias.ScreenResolution, Preferencias.Game, cambióResolución: false, cambióUIMod: false, 
                cambióJuego: true); // Esta instrucción se repite dentro de AgregarUIMods() con cambióUIMod = true. Solo genera un poco de menos rendimiento, nada grave.

            Activado = false; // Se desactiva esta variable para permitir reflejar en la interface los cambios realizados en Preferencias.EstablecerValoresRecomendados() al cambiar el juego sin generar nuevamente eventos de cambio en estos controles.
            TxtStepDuration.Text = Preferencias.StepDuration.ToString();
            CmbGameSpeed.Text = Preferencias.ObtenerGameSpeedText(Preferencias.Game);
            ChkShowNextStep.IsChecked = Preferencias.ShowNextStep;
            ChkShowPreviousStep.IsChecked = Preferencias.ShowPreviousStep;
            Activado = true;

            VentanaPrincipal.AplicarPreferencias();
            CrearEntidadesYNombres();
            Preferencias.CurrentBuildOrder = "Tutorial";
            VentanaPrincipal.CmbBuildOrders.SelectedValue = "Tutorial";
            VentanaPrincipal.RecargarEstrategia();

            ActualizarDuraciónPasoAlSalir = true;

        } // CmbGame_SelectionChanged>


        private void Window_Activated(object sender, EventArgs e) 
            => Activado = true;


        private void CmbGameSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.EstablecerGameSpeed(ObtenerSeleccionadoEnCombobox(e), Preferencias.Game);
            ActualizarDuraciónPasoAlSalir = true;

        } // CmbGameSpeed_SelectionChanged>


        private void VerificarResolución() {

            if (Preferencias.Game == AOE2Name && Preferencias.UIMod == UIMod.No_Mod.ToString()) {

                switch (Preferencias.ScreenResolution) {
                    case "1366x768":
                    case "1280x720":
                    case "1600x900":
                    case "1360x768":
                    case "1280x800":
                    case "1440x900":

                        MostrarInformación($"Your screen resolution is low.{Environment.NewLine}{Environment.NewLine}" +
                            $"For RTS Helper to work correctly for Age of Empires II, install one of the 'Anne HK Better Resource Panel' mods and select " +
                            $"it in 'RTS Helper > Settings > General > Game Interface Mod' and/or in game settings " +
                            "increase the 'Interface > In-game HUD Scale' value and select the same value in 'RTS Helper > Settings > General > " +
                            "Game Interface Scale'."); 
                        break;

                    default:
                        break;
                }

            }

        } // VerificarResolución>


        private void CmbResolution_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            if (!ValidarCambioRectángulos(CmbResolution, Preferencias.ScreenResolution)) return;

            CambióResolución = true;
            Preferencias.ScreenResolution = ObtenerSeleccionadoEnCombobox(e);
            Preferencias.EstablecerValoresRecomendados(Preferencias.ScreenResolution, Preferencias.Game, cambióResolución: true, cambióUIMod: false, 
                cambióJuego: false);
            VentanaPrincipal.AplicarPreferencias();

        } //CmbResolution_SelectionChanged>


        private void TxtOpacity_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(ObtenerTextoNúmeroLocal(TxtOpacity.Text), out double opacidad)) {
                Preferencias.Opacity = opacidad;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtOpacity_TextChanged>


        private void TxtStepFontSize_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtStepFontSize.Text, out double stepFontSize)) {
                Preferencias.CurrentStepFontSize = stepFontSize;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtStepFontSize_TextChanged>


        private void TxtNextPreviousStepFontSize_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtNextPreviousStepFontSize.Text, out double nextStepFontSize)) {
                Preferencias.NextPreviousStepFontSize = nextStepFontSize;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtNextPreviousStepFontSize_TextChanged>


        private void ChkShowNextStep_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowNextStep = (bool)ChkShowNextStep.IsChecked!;
            if (Preferencias.ShowNextStep && Preferencias.ShowPreviousStep) ChkShowPreviousStep.IsChecked = false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowNextStep_Checked>


        private void ChkShowPreviousStep_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowPreviousStep = (bool)ChkShowPreviousStep.IsChecked!;
            if (Preferencias.ShowNextStep && Preferencias.ShowPreviousStep) ChkShowNextStep.IsChecked = false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowPreviousStep_Checked>


        private void CmbEndSound_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.StepEndSound = ObtenerSeleccionadoEnCombobox(e);
            Preferencias.StepEndSoundDuration = ObtenerDuraciónEndStepSound(Preferencias.StepEndSound);
            MediaPlayer.PlaySonidoFinal();
            if (Preferencias.StepEndSound.ToLower().StartsWith("windows beep")) {
                SldEndSoundVolume.Visibility = Visibility.Collapsed;
                LblEndSoundVolume.Visibility = Visibility.Collapsed;
            } else {
                SldEndSoundVolume.Visibility = Visibility.Visible;
                LblEndSoundVolume.Visibility = Visibility.Visible;
            }

        } // CmbEndSound_SelectionChanged>


        private void CmbStartSound_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.StepStartSound = ObtenerSeleccionadoEnCombobox(e);
            MediaPlayer.PlaySonidoInicio();
            if (Preferencias.StepStartSound.ToLower().StartsWith("windows beep")) {
                SldStartSoundVolume.Visibility = Visibility.Collapsed;
                LblStartSoundVolume.Visibility = Visibility.Collapsed;
            } else {
                SldStartSoundVolume.Visibility = Visibility.Visible;
                LblStartSoundVolume.Visibility = Visibility.Visible;
            }

        } // CmbStartSound_SelectionChanged>


        private void SldStartSoundVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {

            if (!Activado) return;
            Preferencias.StepStartSoundVolume = (int)SldStartSoundVolume.Value;
            MediaPlayer.PlaySonidoInicio();

        } // SldStartSoundVolume_ValueChanged>


        private void SldEndSoundVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {

            if (!Activado) return;
            Preferencias.StepEndSoundVolume = (int)SldEndSoundVolume.Value;
            MediaPlayer.PlaySonidoFinal();

        } // SldEndSoundVolume_ValueChanged>


        private void ChkUnmuteAtStartup_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.UnmuteAtStartup = (bool)ChkUnmuteAtStartup.IsChecked!;

        } // ChkUnmuteAtStartup_Checked>


        private void ChkMinimizeOnComplete_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.MinimizeOnComplete = (bool)ChkMinimizeOnComplete.IsChecked!;

        } // ChkMinimizeOnComplete_Checked>


        private void ChkMuteOnComplete_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.MuteOnComplete = (bool)ChkMuteOnComplete.IsChecked!;

        } // ChkMuteOnComplete_Checked>


        private void ChkStopFlashingOnComplete_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.StopFlashingOnComplete = (bool)ChkStopFlashingOnComplete.IsChecked!;

        } // ChkStopFlashingOnComplete_Checked>


        private void ChkFlashOnStepChange_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.FlashOnStepChange = (bool)ChkFlashOnStepChange.IsChecked!;

        } // ChkFlashOnStepChange_Checked>


        private void BtnFlashingColor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnFlashingColor.Background);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Preferencias.FlashingColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
                VentanaPrincipal.Flash();
            }

        } // BtnFlashingColor_Click>


        private void TxtFlashingOpacity_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(ObtenerTextoNúmeroLocal(TxtFlashingOpacity.Text), out double opacidad)) {
                Preferencias.FlashingOpacity = opacidad;
                VentanaPrincipal.AplicarPreferencias();
                VentanaPrincipal.Flash();
            }

        } // TxtFlashingOpacity_TextChanged>


        private void TxtStepDuration_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtStepDuration.Text, out double duración)) Preferencias.StepDuration = duración;
            ActualizarDuraciónPasoAlSalir = true;

        } // TxtStepDuration_TextChanged>


        private void Lnk_Click(object sender, RoutedEventArgs e) => AbrirUrl(((Hyperlink)sender).NavigateUri.ToString());


        private void ChkCurrentStepFontBold_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.CurrentStepFontBold = ChkCurrentStepFontBold.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkCurrentStepFontBold_Checked>


        private void ChkNextPreviousStepFontBold_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.NextPreviousStepFontBold = ChkNextPreviousStepFontBold.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkNextPreviousStepFontBold_Checked>


        private void CmbFontName_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.FontName = ObtenerSeleccionadoEnCombobox(e);
            VentanaPrincipal.AplicarPreferencias();

        } // CmbFontName_SelectionChanged>


        private void CmbSecondaryFontName_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.SecondaryFontName = ObtenerSeleccionadoEnCombobox(e);
            VentanaPrincipal.AplicarPreferencias();

        } // CmbSecondaryFontName_SelectionChanged>


        private void TxtLineSpacing_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtLineSpacing.Text, out double espaciadoLíneas)) {
                Preferencias.LineSpacing = espaciadoLíneas;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtLineSpacing_TextChanged>


        private void TxtImageSize_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtImageSize.Text, out double espaciadoLíneas)) {
                Preferencias.ImageSize = espaciadoLíneas;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtImageSize_TextChanged>


        private void BtnImageBackgroundColor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnImageBackgroundColor.Background);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Preferencias.ImageBackgroundColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias();
            }

        } // BtnImageBackgroundColor_Click>


        private void TxtImageBackgroundOpacity_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(ObtenerTextoNúmeroLocal(TxtImageBackgroundOpacity.Text), out double opacidad)) {
                Preferencias.ImageBackgroundOpacity = opacidad;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtImageBackgroundOpacity_TextChanged>


        private void TxtImageHorizontalMargin_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtImageHorizontalMargin.Text, out double margenHorizontal)) {
                Preferencias.EntityHorizontalMargin = margenHorizontal; // Esta preferencia se pone en imágenes porque tiene más sentido ahí y es donde más se usa, pero también afecta a las entidades que se muestran como texto en reemplazo de las imágenes. Si fuera necesario se podrían crear dos preferencias diferentes.
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtImageHorizontalMargin_TextChanged>


        private void TxtImageBackgroundRoundedCornersRadius_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtImageBackgroundRoundedCornersRadius.Text, out double radioEsquinas)) {
                Preferencias.ImageBackgroundRoundedCornersRadius = radioEsquinas;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtImageBackgroundRoundedCornersRadius_TextChanged>


        private void TxtSubscriptAndSuperscriptImagesSize_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtSubscriptAndSuperscriptImagesSize.Text, out double subSuperSize)) {
                Preferencias.SubscriptAndSuperscriptImagesSize = subSuperSize;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtSubscriptAndSuperscriptImagesSize_TextChanged>


        private void ChkRandomImageForMultipleImages_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.RandomImageForMultipleImages = ChkRandomImageForMultipleImages.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkRandomImageForMultipleImages_Checked>


        private void ChkCapitalizeNames_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.CapitalizeNames = ChkCapitalizeNames.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkCapitalizeNames_Checked>


        private void ChkShowStepProgress_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowStepProgress = ChkShowStepProgress.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowStepProgress_Checked>


        private void ChkShowTime_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowTime = ChkShowTime.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowTime_Checked>


        private void CmbGameLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.GameLanguage = ObtenerSeleccionadoEnCombobox(e, tag: true);
            if (Preferencias.Game == AOMName && Preferencias.GameLanguage != "EN") {
                MostrarInformación("Currently only english is supported for pauses detection. You can still use the app, only that it won't " +
                    "automatically pause when you pause the game. Alternatively, you can change your game to english if you want to try the auto pause " +
                    "detection feature.");
            }

        } // CmbGameLanguage_SelectionChanged>


        private bool ValidarCambioRectángulos(Controles.ComboBox cmb, string valorAnterior) {

            if (!RectángulosSonLosRecomendados()) {

                if (System.Windows.MessageBox.Show($@"You have modified manually the OCR values. If you make this change, you will lose your custom " +
                    $@"values.{Environment.NewLine}{Environment.NewLine}Do you want to continue?", 
                    "OCR Rectangles", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {

                    return true; // Cambio aceptado.

                } else {

                    Activado = false;
                    var cbi = cmb.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Content.ToString() == valorAnterior);
                    cmb.SelectedIndex = cmb.Items.IndexOf(cbi);
                    Activado = true;
                    return false; // Cambio no aceptado.

                }

            } else {
                return true; // Cambio aceptado.
            }

        } // ValidarCambioRectángulos>


        private void CmbUIMod_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado || ActualizandoUIMods) return;
            if (!ValidarCambioRectángulos(CmbUIMod, Preferencias.UIMod.Replace("_", " "))) return;

            var anteriorUIMod = Preferencias.UIMod;
            Preferencias.UIMod = ObtenerSeleccionadoEnCombobox(e, tag: true);
            AgregarUIMods(anteriorUIMod);

        } // CmbGameLanguage_SelectionChanged>


        private void CmbVelocidadEjecución_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            var cbi = e.AddedItems[0] as ComboBoxItem;
            if (cbi is null) return;
            Preferencias.ExecutionSpeed = Convert.ToDouble(cbi.Tag) / 100;
            VentanaPrincipal.ActualizarDuraciónPaso();

        } // CmbVelocidadJugador_SelectionChanged>


        private void CargarVelocidadEjecución() {

            var textoVelocidadEjecución = (Preferencias.ExecutionSpeed * 100).ToString() + "%";
            if (CmbVelocidadEjecución.Text != textoVelocidadEjecución) {
                VentanaPrincipal.EditandoComboBoxEnCódigo = true;
                CmbVelocidadEjecución.Text = textoVelocidadEjecución;
                VentanaPrincipal.EditandoComboBoxEnCódigo = false;
            }

        } // CargarVelocidadEjecución>


        private void CargarEscalaInterface() {

            var textoEscalaInterface = Preferencias.GameInterfaceScale.ToString() + "%";
            if (CmbGameInterfaceScale.Text != textoEscalaInterface) {
                VentanaPrincipal.EditandoComboBoxEnCódigo = true;
                CmbGameInterfaceScale.Text = textoEscalaInterface;
                VentanaPrincipal.EditandoComboBoxEnCódigo = false;
            }

        } // CargarEscalaInterface>


        private void ChkAutoadjustIdleTime_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.AutoAdjustIdleTime = ChkAutoadjustIdleTime.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkAutoadjustIdleTime_Checked>


        private void TxtAutoadjustIdleTimeInterval_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (int.TryParse(TxtAutoadjustIdleTimeInterval.Text, out int valor) && valor > 0) {
                Preferencias.AutoAdjustIdleTimeInterval = valor;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtAutoadjustIdleTimeInterval_TextChanged>


        private void ChkPauseDetection_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.PauseDetection = ChkPauseDetection.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkPauseDetection_Checked>


        private void TxtPauseDetectionInterval_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (int.TryParse(TxtPauseDetectionInterval.Text, out int valor) && valor > 0) {
                Preferencias.PauseDetectionInterval = valor;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtPauseDetectionInterval_TextChanged>


        private void ChkShowAddIdleTime_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowAddIdleTimeButton = ChkShowAddIdleTime.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowAddIdleTime_Checked>


        private void TxtAddIdleTimeSeconds_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (int.TryParse(TxtAddIdleTimeSeconds.Text, out int valor) && valor > 0) {
                Preferencias.AddIdleTimeSeconds = valor;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtAddIdleTimeSeconds_TextChanged>


        private void ChkShowRemoveIdleTime_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowRemoveIdleTimeButton = ChkShowRemoveIdleTime.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowRemoveIdleTime_Checked>


        private void TxtRemoveIdleTimeSeconds_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (int.TryParse(TxtRemoveIdleTimeSeconds.Text, out int valor) && valor > 0) {
                Preferencias.RemoveIdleTimeSeconds = valor;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtRemoveIdleTimeSeconds_TextChanged>


        private void TxtMinimumDelayToAutoAdjustIdleTime_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (int.TryParse(TxtMinimumDelayToAutoAdjustIdleTime.Text, out int valor) && valor > 0) {

                if (valor <= Preferencias.AutoAdjustIdleTimeInterval) {
                    System.Windows.MessageBox.Show("MinimumDelayToAutoAdjustIdleTime should be at least 1 second more than AutoAdjustIdleTimeInterval.", 
                        "Error");
                    TxtMinimumDelayToAutoAdjustIdleTime.Text = (Preferencias.AutoAdjustIdleTimeInterval + 1).ToString();
                } else {
                    Preferencias.MinimumDelayToAutoAdjustIdleTime = valor;
                    VentanaPrincipal.AplicarPreferencias();
                }

            }

        } // TxtMinimumDelayToAutoAdjustIdleTime_TextChanged>


        private void TxtBackwardSeconds_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (int.TryParse(TxtBackwardSeconds.Text, out int valor) && valor > 0) {
                Preferencias.BackwardSeconds = valor;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtBackwardSeconds_TextChanged>


        private void TxtFordwardSeconds_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (int.TryParse(TxtFordwardSeconds.Text, out int valor) && valor > 0) {
                Preferencias.ForwardSeconds = valor;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtFordwardSeconds_TextChanged>


        private void TxtBackMultipleSteps_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (int.TryParse(TxtBackMultipleSteps.Text, out int valor) && valor > 0) {
                Preferencias.BackMultipleSteps = valor;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtBackMultipleSteps_TextChanged>


        private void TxtNextMultipleSteps_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (int.TryParse(TxtNextMultipleSteps.Text, out int valor) && valor > 0) {
                Preferencias.NextMultipleSteps = valor;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtNextMultipleSteps_TextChanged>


        private void ChkShowAlwaysStatsButton_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowAlwaysStatsButton = ChkShowAlwaysStatsButton.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowAlwaysStatsButton_Checked>


        private void ChkShowAlternateNextPreviousStepButton_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowAlternateNextPreviousStepButton = ChkShowAlternateNextPreviousStepButton.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowAlternateNextPreviousStepButton_Checked>


        private void ChkShowOptionalInstructions1_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowOptionalInstructions1 = ChkShowOptionalInstructions1.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();
            VentanaPrincipal.RecargarEstrategia();

        } // ChkShowOptionalInstructions1_Checked>


        private void ChkShowOptionalInstructions2_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowOptionalInstructions2 = ChkShowOptionalInstructions2.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();
            VentanaPrincipal.RecargarEstrategia();

        } // ChkShowOptionalInstructions2_Checked>


        private void CmbGameInterfaceScale_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            var cbi = e.AddedItems[0] as ComboBoxItem;
            if (cbi is null) return;
            Preferencias.GameInterfaceScale = Convert.ToInt32(cbi.Tag);

        } // CmbGameInterfaceScale_SelectionChanged>


        private void ChkOCRTestMode_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.OCRTestMode = ChkOCRTestMode.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkOCRTestMode_Checked>


        private void BtnRestoreDefaultRectangles_Click(object sender, RoutedEventArgs e) {

            Preferencias.ScreenCaptureRectangles?.Clear();
            Preferencias.ScreenCaptureRectangles = null;
            CrearOCompletarScreenCaptureRectangles(cambióResolución: false, cambióUIMod: false);
            CargarRectángulosOCR();

        } // BtnRestoreDefaultRectangles_Click>


    } // SettingsWindow>


} // RTSHelper>
