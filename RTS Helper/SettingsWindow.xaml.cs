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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using static RTSHelper.Global;



namespace RTSHelper {


    public partial class SettingsWindow : Window {


        public bool Activado { get; set; } = false;

        private MainWindow VentanaPrincipal { get; set; }


        public SettingsWindow(bool primerInicio, MainWindow ventanaPrincipal) {

            InitializeComponent();
            VentanaPrincipal = ventanaPrincipal;

            CargarValores(Preferencias, primerInicio);
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
                TbiBehavior.Visibility = Visibility.Collapsed;
                TbiCustomStyles.Visibility = Visibility.Collapsed;
                TbiStyle.Visibility = Visibility.Collapsed;
                Height = 250;
                TbcPreferencias.Height = 150;
                Width = 350;

            }

        } // SettingsWindow>


        public void CargarValores(Settings preferencias, bool primerInicio) {

            CmbResolution.Text = preferencias.ScreenResolution;
            CmbGame.Text = preferencias.Game;
            CmbGameSpeed.Text = preferencias.ObtenerGameSpeedText(preferencias.Game);
            TxtOpacity.Text = preferencias.Opacity.ToString();
            TxtBuildOrderPath.Text = preferencias.BuildOrderDirectory;    
            TxtStepFontSize.Text = preferencias.CurrentStepFontSize.ToString();
            TxtNextStepFontSize.Text = preferencias.NextStepFontSize.ToString();
            ChkShowNextStep.IsChecked = preferencias.ShowNextStep;
            LeerSonidos();
            CmbStartSound.Text = preferencias.StepStartSound;
            CmbEndSound.Text = preferencias.StepEndSound;
            if (CmbStartSound.SelectedIndex == -1) CmbStartSound.Text = NoneSoundString;
            if (CmbEndSound.SelectedIndex == -1) CmbEndSound.Text = NoneSoundString;
            ChkUnmuteAtStartup.IsChecked = preferencias.UnmuteAtStartup;
            SldEndSoundVolume.Value = preferencias.StepEndSoundVolume;
            SldStartSoundVolume.Value = preferencias.StepStartSoundVolume;
            if (!primerInicio) VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en los botones.

        } // CargarValores>


        public void LeerSonidos() {

            CmbStartSound.Items.Clear();
            CmbEndSound.Items.Clear();
            CmbStartSound.Items.Add(NoneSoundString);
            CmbEndSound.Items.Add(NoneSoundString);
            CmbStartSound.Text = NoneSoundString;
            CmbEndSound.Text = NoneSoundString;
            var extensionesAudio = new List<string> { ".mp3", ".wav", ".wma", ".ogg", ".acc", ".flac" };

            var rutasSonidosCortos = System.IO.Directory.GetFiles(DirectorioSonidosCortos);
            foreach (var rutaSonidoCorto in rutasSonidosCortos) {
                var extensión = System.IO.Path.GetExtension(rutaSonidoCorto).ToLower();
                if (extensionesAudio.Contains(extensión) || string.IsNullOrEmpty(extensión)) 
                    CmbStartSound.Items.Add(System.IO.Path.GetFileName(rutaSonidoCorto));
            }

            var rutasSonidosLargos = System.IO.Directory.GetFiles(DirectorioSonidosLargos);
            foreach (var rutaSonidoLargo in rutasSonidosLargos) {
                if (extensionesAudio.Contains(System.IO.Path.GetExtension(rutaSonidoLargo).ToLower())) 
                    CmbEndSound.Items.Add(System.IO.Path.GetFileName(rutaSonidoLargo));
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


        private void LnkDavid_Click(object sender, RoutedEventArgs e) =>
            Process.Start(new ProcessStartInfo(((Hyperlink)sender).NavigateUri.ToString()) { UseShellExecute = true });


        private void BtnSave_Click(object sender, RoutedEventArgs e) 
            => this.Close();


        private void Window_Closed(object sender, EventArgs e) {

            VentanaPrincipal.AplicarPreferencias();
            Settings.Guardar(Preferencias, RutaPreferencias);

        } // Window_Closed>


        private void CmbGame_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.Game = ObtenerSeleccionadoEnCombobox(e); 
            Preferencias.EstablecerValoresRecomendados(Preferencias.ScreenResolution, Preferencias.Game);
            VentanaPrincipal.AplicarPreferencias();

        } // CmbGame_SelectionChanged>


        private void Window_Activated(object sender, EventArgs e) 
            => Activado = true;


        private void CmbGameSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.EstablecerGameSpeed(ObtenerSeleccionadoEnCombobox(e), Preferencias.Game);
            VentanaPrincipal.ActualizarDuraciónPaso();

        } // CmbGameSpeed_SelectionChanged>


        private void CmbResolution_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.ScreenResolution = ObtenerSeleccionadoEnCombobox(e);
            Preferencias.EstablecerValoresRecomendados(Preferencias.ScreenResolution, Preferencias.Game);
            VentanaPrincipal.AplicarPreferencias();

        } //CmbResolution_SelectionChanged>


        private void TxtOpacity_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtOpacity.Text, out double opacidad)) {
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
            Preferencias.ShowNextStep = (bool)ChkShowNextStep.IsChecked;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowNextStep_CheckedChanged>


        private void TxtBuildOrderPath_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (System.IO.Directory.Exists(TxtBuildOrderPath.Text)) {
                Preferencias.BuildOrderDirectory = TxtBuildOrderPath.Text;
                VentanaPrincipal.LeerBuildOrders();
                VentanaPrincipal.CargarBuildOrder();
            } else if (string.IsNullOrEmpty(TxtBuildOrderPath.Text)) { 
                Preferencias.BuildOrderDirectory = null;
                VentanaPrincipal.LeerBuildOrders();
                VentanaPrincipal.CargarBuildOrder();
            } 

        } // TxtBuildOrderPath_TextChanged>


        private void CmbEndSound_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            Preferencias.StepEndSound = ObtenerSeleccionadoEnCombobox(e);

        } // CmbEndSound_SelectionChanged>


    } // SettingsWindow>


} // RTSHelper>
