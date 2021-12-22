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



namespace RTSHelper {


    public partial class SettingsWindow : Window {


        public bool Activado { get; set; } = false;

        private MainWindow VentanaPrincipal { get; set; }

        public bool ActualizarDuraciónPasoAlSalir { get; set; } = false;

        public Dictionary<NameType, Dictionary<string, string>>? NombresPersonalizados { get; set; } = null; // Se hace quí para que cada vez que se abra los settings de custom names se lea este archivo, esto permite más flexibilidad para editarlo a mano o reemplazarlo por otro sin reiniciar el programa, si el usuario desea.

        public bool UnNombrePersonalizadoCambiado { get; set; } = false;


        public SettingsWindow(bool primerInicio, MainWindow ventanaPrincipal) {

            InitializeComponent();
            VentanaPrincipal = ventanaPrincipal;
            var versión = Assembly.GetEntryAssembly()?.GetName().Version;
            LblVersion.Content = $"{versión?.Major}.{versión?.Minor}{(versión?.Build == 0 ? "" : $".{versión?.Build.ToString()}")}";

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
                SpnBuildOrderPath.Visibility = Visibility.Collapsed;
                TbiNotifications.Visibility = Visibility.Collapsed;
                TbiImages.Visibility = Visibility.Collapsed;
                TbiDisplayPriority.Visibility = Visibility.Collapsed;
                TbiPersonalization.Visibility = Visibility.Collapsed;
                TbiAbout.Visibility = Visibility.Collapsed;
                TbiCustomNames.Visibility = Visibility.Collapsed;
                TbiOverrides.Visibility = Visibility.Collapsed;
                Height = 260;
                TbcPreferencias.Height = 150;
                Width = 350;
                LblStepDuration.Visibility = Visibility.Collapsed;
                TxtStepDuration.Visibility = Visibility.Collapsed;
                LblStepDurationSeconds.Visibility = Visibility.Collapsed;
                LblShowProgress.Visibility = Visibility.Collapsed;
                LblShowTime.Visibility = Visibility.Collapsed;
                ChkShowStepProgress.Visibility = Visibility.Collapsed;
                ChkShowTime.Visibility = Visibility.Collapsed;

            }

        } // SettingsWindow>


        public void CargarValores(bool primerInicio) {

            CmbResolution.Text = Preferencias.ScreenResolution;
            CmbGame.Text = Preferencias.Game;
            CmbGameSpeed.Text = Preferencias.ObtenerGameSpeedText(Preferencias.Game);
            TxtOpacity.Text = Preferencias.Opacity.ToString();
            TxtBuildOrderPath.Text = Preferencias.BuildOrderCustomDirectory;    
            TxtStepFontSize.Text = Preferencias.CurrentStepFontSize.ToString();
            TxtNextStepFontSize.Text = Preferencias.NextStepFontSize.ToString();
            ChkShowNextStep.IsChecked = Preferencias.ShowNextStep;
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
            ChkCurrentStepFontBold.IsChecked = Preferencias.CurrentStepFontBold;
            ChkNextStepFontBold.IsChecked = Preferencias.NextStepFontBold;

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
            ChkOverrideShowNextStep.IsChecked = Preferencias.OverrideShowNextStep;
            ChkOverrideStepDuration.IsChecked = Preferencias.OverrideStepDuration;
            ChkOverrideStepEndSound.IsChecked = Preferencias.OverrideStepEndSound;
            ChkOverrideStepEndSoundVolume.IsChecked = Preferencias.OverrideStepEndSoundVolume;
            ChkOverrideStepStartSound.IsChecked = Preferencias.OverrideStepStartSound;
            ChkOverrideStepStartSoundVolume.IsChecked = Preferencias.OverrideStepStartSoundVolume;
            ChkShowStepProgress.IsChecked = Preferencias.ShowStepProgress;
            ChkShowTime.IsChecked = Preferencias.ShowTime;
            ChkRandomImageForMultipleImages.IsChecked = Preferencias.RandomImageForMultipleImages;
            ChkCapitalizeNames.IsChecked = Preferencias.CapitalizeNames;

            if (!primerInicio) VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en los botones.

        } // CargarValores>


        private void TbiCustomNames_Selected(object sender, RoutedEventArgs e) => CargarNombresPersonalizados();


        private void TbiCustomNames_Unselected(object sender, RoutedEventArgs e) => GuardarNombresPersonalizados(cerrando: false);


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
            Preferencias.OverrideShowNextStep = (bool)ChkOverrideShowNextStep.IsChecked!;
            var overrideStepDuration = (bool)ChkOverrideStepDuration.IsChecked!;
            if (!overrideStepDuration) 
                MostrarInformación("If you don't allow build orders to override the default step duration and they have custom steps durations, " + 
                    "their instructions won't be syncronized. It's recommended to leave this option checked.");
            Preferencias.OverrideStepDuration = overrideStepDuration;

            Preferencias.OverrideStepEndSound = (bool)ChkOverrideStepEndSound.IsChecked!;
            Preferencias.OverrideStepEndSoundVolume = (bool)ChkOverrideStepEndSoundVolume.IsChecked!;
            Preferencias.OverrideStepStartSound = (bool)ChkOverrideStepStartSound.IsChecked!;
            Preferencias.OverrideStepStartSoundVolume = (bool)ChkOverrideStepStartSoundVolume.IsChecked!;
            VentanaPrincipal.CargarBuildOrder();

        } // GuardarOverrides>


        private void CargarNombresPersonalizados() {

            SpnCustomNames.Children.Clear();
            NombresPersonalizados = DeserializarNombres(Preferencias.CustomNamesPath);
            var nombresCompletosConPersonalizado = new List<string>();

            foreach (var kv in NombresPersonalizados[NameType.Custom]) {

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

            SpnCustomNames.Children.Add(new Controles.Label() { Margin = new Thickness(0, 10, 0, 10),
                Content = "To use them, in 'Display Priority' section move 'Custom Name' to the top."
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

                NombresPersonalizados[NameType.Custom].Add(nuevoNombre, nombrePersonalizado);
                UnNombrePersonalizadoCambiado = true;
                GuardarNombresPersonalizados(cerrando: false);
                CargarNombresPersonalizados();

            }

        } // BtnNuevoNombrePersonalizado_Click>


        private void TxtCustomName_TextChanged(object sender, TextChangedEventArgs e) {

            var textbox = (Controles.TextBox)sender;
            var nombreCompleto = textbox.Tag?.ToString();
            if (NombresPersonalizados != null && nombreCompleto != null) NombresPersonalizados[NameType.Custom][nombreCompleto] = textbox.Text;
            UnNombrePersonalizadoCambiado = true;

        } // TxtCustomName_TextChanged>


        private void CargarPrioridadesNombres() {

            SpnDisplayPriority.Children.Clear();
            var encontradoPrimerIdioma = false;
            var idiomaEncontrado = "";
            var cuenta = 0;
            var totalNombresSinIdiomas = Preferencias.DisplayPriority.Count - Idiomas.Count + 1; // Se suma 1 porque siempre se muestra el idioma de más prioridad.
            var displayPriorityOrdenadas = Preferencias.ObtenerDisplayPriorityOrdenadas();
            
            foreach (var displayPriority in displayPriorityOrdenadas) {

                var esIdioma = false;
                var textoNombre = displayPriority.Key.ToString();
                if (Idiomas.ContainsKey(textoNombre)) {

                    if (encontradoPrimerIdioma) continue;
                    idiomaEncontrado = textoNombre;
                    encontradoPrimerIdioma = true;
                    esIdioma = true;

                }
                var SpnName = new StackPanel() { Orientation = Controles.Orientation.Horizontal };
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

                SpnName.Children.Add(btnUp);
                SpnName.Children.Add(btnDown);
                SpnName.Children.Add(label);

                if (esIdioma) { // Solo pasa por aquí cuando encuentra el primero.

                    var cmbIdioma = new Controles.ComboBox() { VerticalAlignment = VerticalAlignment.Center };
                    foreach (var kv in Idiomas) {

                        var idioma = kv.Key;
                        var nombreIdioma = kv.Value;
                        var cmi = new ComboBoxItem() { Content = nombreIdioma, Tag = idioma };
                        if (idioma == idiomaEncontrado) cmi.IsSelected = true;
                        cmbIdioma.Items.Add(cmi);

                    }
                    cmbIdioma.SelectionChanged += new Controles.SelectionChangedEventHandler(this.CmbOtherLanguage_SelectionChanged);
                    SpnName.Children.Add(cmbIdioma);

                }
                SpnDisplayPriority.Children.Add(SpnName);
                cuenta++;

            }

        } // CargarPrioridadesNombres>


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
                if (Idiomas.ContainsKey(textoNombre)) {

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

                if (Idiomas.ContainsKey(textoNombre)) {

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

            var rutasSonidosCortos = Directory.GetFiles(DirectorioSonidosCortos);
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


        private void BtnNextStepFontColor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnNextStepFontColor.Foreground);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Preferencias.NextStepFontColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
            }

        } // BtnNextStepFontColor_Click>


        private void BtnSave_Click(object sender, RoutedEventArgs e) 
            => this.Close();


        private void Window_Closed(object sender, EventArgs e) {

            GuardarNombresPersonalizados(cerrando: true);
            VentanaPrincipal.AplicarPreferencias();
            Settings.Guardar(Preferencias, RutaPreferencias);
            if (ActualizarDuraciónPasoAlSalir) VentanaPrincipal.ActualizarDuraciónPaso(); // Se aplica solo al salir para no reiniciar el timer cada vez que se haga un cambio en los controles.

        } // Window_Closed>


        private void CmbGame_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.Game = ObtenerSeleccionadoEnCombobox(e); 
            Preferencias.EstablecerValoresRecomendados(Preferencias.ScreenResolution, Preferencias.Game);
            TxtStepDuration.Text = Preferencias.StepDuration.ToString(); // Es el único valor que se actualiza en la interface de preferencias después de haber cambiado el juego. No es lo más óptimo, pero de todas maneras lo ideal es que cada archivo de txt de build orders traiga su propio StepDuration.
            VentanaPrincipal.AplicarPreferencias();
            CrearEntidadesYNombres();
            VentanaPrincipal.LeerBuildOrders();
            VentanaPrincipal.CargarBuildOrder();
            ActualizarDuraciónPasoAlSalir = true;

        } // CmbGame_SelectionChanged>


        private void Window_Activated(object sender, EventArgs e) 
            => Activado = true;


        private void CmbGameSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.EstablecerGameSpeed(ObtenerSeleccionadoEnCombobox(e), Preferencias.Game);
            ActualizarDuraciónPasoAlSalir = true;

        } // CmbGameSpeed_SelectionChanged>


        private void CmbResolution_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.ScreenResolution = ObtenerSeleccionadoEnCombobox(e);
            Preferencias.EstablecerValoresRecomendados(Preferencias.ScreenResolution, Preferencias.Game);
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


        private void TxtNextStepFontSize_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtNextStepFontSize.Text, out double nextStepFontSize)) {
                Preferencias.NextStepFontSize = nextStepFontSize;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtNextStepFontSize_TextChanged>


        private void ChkShowNextStep_CheckedChanged(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.ShowNextStep = (bool)ChkShowNextStep.IsChecked!;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowNextStep_CheckedChanged>


        private void TxtBuildOrderPath_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (Directory.Exists(TxtBuildOrderPath.Text)) {

                Preferencias.BuildOrderCustomDirectory = TxtBuildOrderPath.Text;
                VentanaPrincipal.LeerBuildOrders();
                VentanaPrincipal.CargarBuildOrder();

            } else if (string.IsNullOrEmpty(TxtBuildOrderPath.Text)) { 

                Preferencias.BuildOrderCustomDirectory = null;
                VentanaPrincipal.LeerBuildOrders();
                VentanaPrincipal.CargarBuildOrder();

            } 

        } // TxtBuildOrderPath_TextChanged>


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


        private void BtnBuildOrderPath_Click(object sender, RoutedEventArgs e) {

            var folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = TxtBuildOrderPath.Text;
            if (string.IsNullOrEmpty(folderDialog.SelectedPath)) folderDialog.SelectedPath = Preferencias.BuildOrdersDirectory;
            var respuesta = folderDialog.ShowDialog();
            if (respuesta != System.Windows.Forms.DialogResult.Cancel) TxtBuildOrderPath.Text = folderDialog.SelectedPath;

        } // BtnBuildOrderPath_Click>


        private void Lnk_Click(object sender, RoutedEventArgs e) 
            => Process.Start(new ProcessStartInfo(((Hyperlink)sender).NavigateUri.ToString()) { UseShellExecute = true });


        private void ChkCurrentStepFontBold_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.CurrentStepFontBold = ChkCurrentStepFontBold.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkCurrentStepFontBold_Checked>


        private void ChkNextStepFontBold_Checked(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            Preferencias.NextStepFontBold = ChkNextStepFontBold.IsChecked ?? false;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkNextStepFontBold_Checked>


        private void CmbFontName_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.FontName = ObtenerSeleccionadoEnCombobox(e);
            VentanaPrincipal.AplicarPreferencias();

        } // CmbFontName_SelectionChanged>


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


    } // SettingsWindow>


} // RTSHelper>
