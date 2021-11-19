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


namespace RTSHelper {


    public partial class SettingsWindow : Window {


        public bool Activado { get; set; } = false;

        public MainWindow VentanaPrincipal { get; set; } = null;


        public SettingsWindow(MainWindow ventanaPrincipal, bool primerInicio) {

            InitializeComponent();
            VentanaPrincipal = ventanaPrincipal;
            CargarValores(ventanaPrincipal.Preferencias, primerInicio);
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
                Height = 250;
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
            ChkPlaySoundEachStep.IsChecked = preferencias.PlaySoundEachStep;
            if (!primerInicio) VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en los botones.

        } // CargarValores>


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
                VentanaPrincipal.Preferencias.BackColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
            }

        } // BtnBackcolor_Click>


        private void BtnFontcolor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnFontcolor.Foreground);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                VentanaPrincipal.Preferencias.FontColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
            }

        } // BtnFontcolor_Click>


        private void BtnStepFontColor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnStepFontColor.Foreground);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                VentanaPrincipal.Preferencias.CurrentStepFontColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
            }

        } // BtnStepFontColor_Click>


        private void BtnNextStepFontColor_Click(object sender, RoutedEventArgs e) {

            var colorDialog = new ColorDialog();
            colorDialog.Color = ObtenerDrawingColor(BtnNextStepFontColor.Foreground);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                VentanaPrincipal.Preferencias.NextStepFontColor = ToHexString(colorDialog.Color);
                VentanaPrincipal.AplicarPreferencias(); // Se requiere aplicarlas para que se haga visible el cambio de color en el botón.
            }

        } // BtnNextStepFontColor_Click>


        private void LnkDavid_Click(object sender, RoutedEventArgs e) =>
            Process.Start(new ProcessStartInfo(((Hyperlink)sender).NavigateUri.ToString()) { UseShellExecute = true });


        private void BtnSave_Click(object sender, RoutedEventArgs e) 
            => this.Close();


        private void Window_Closed(object sender, EventArgs e) {

            VentanaPrincipal.AplicarPreferencias();
            Settings.Guardar(VentanaPrincipal.Preferencias, MainWindow.RutaPreferencias);

        } // Window_Closed>


        private void CmbGame_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            var cbi = e.AddedItems[0] as ComboBoxItem;
            VentanaPrincipal.Preferencias.Game = cbi.Content.ToString(); 
            VentanaPrincipal.Preferencias.EstablecerValoresRecomendados(VentanaPrincipal.Preferencias.ScreenResolution, VentanaPrincipal.Preferencias.Game);
            VentanaPrincipal.AplicarPreferencias();

        } // CmbGame_SelectionChanged>


        private void Window_Activated(object sender, EventArgs e) 
            => Activado = true;


        private void CmbGameSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            var cbi = e.AddedItems[0] as ComboBoxItem;
            VentanaPrincipal.Preferencias.EstablecerGameSpeed(cbi.Content.ToString(), VentanaPrincipal.Preferencias.Game);
            VentanaPrincipal.ActualizarDuraciónPaso();

        } // CmbGameSpeed_SelectionChanged>


        private void CmbResolution_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Activado) return;
            var cbi = e.AddedItems[0] as ComboBoxItem;
            VentanaPrincipal.Preferencias.ScreenResolution = cbi.Content.ToString();
            VentanaPrincipal.Preferencias.EstablecerValoresRecomendados(VentanaPrincipal.Preferencias.ScreenResolution, VentanaPrincipal.Preferencias.Game);
            VentanaPrincipal.AplicarPreferencias();

        } //CmbResolution_SelectionChanged>


        private void TxtOpacity_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtOpacity.Text, out double opacidad)) {
                VentanaPrincipal.Preferencias.Opacity = opacidad;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtOpacity_TextChanged>


        private void TxtStepFontSize_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtStepFontSize.Text, out double stepFontSize)) {
                VentanaPrincipal.Preferencias.CurrentStepFontSize = stepFontSize;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtStepFontSize_TextChanged>


        private void TxtNextStepFontSize_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (double.TryParse(TxtNextStepFontSize.Text, out double nextStepFontSize)) {
                VentanaPrincipal.Preferencias.NextStepFontSize = nextStepFontSize;
                VentanaPrincipal.AplicarPreferencias();
            }

        } // TxtNextStepFontSize_TextChanged>


        private void ChkShowNextStep_CheckedChanged(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            VentanaPrincipal.Preferencias.ShowNextStep = (bool)ChkShowNextStep.IsChecked;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkShowNextStep_CheckedChanged>


        private void ChkPlaySoundEachStep_CheckedChanged(object sender, RoutedEventArgs e) {

            if (!Activado) return;
            VentanaPrincipal.Preferencias.PlaySoundEachStep = (bool)ChkPlaySoundEachStep.IsChecked;
            VentanaPrincipal.AplicarPreferencias();

        } // ChkPlaySoundEachStep_CheckedChanged>


        private void TxtBuildOrderPath_TextChanged(object sender, TextChangedEventArgs e) {

            if (!Activado) return;
            if (System.IO.Directory.Exists(TxtBuildOrderPath.Text)) {
                VentanaPrincipal.Preferencias.BuildOrderDirectory = TxtBuildOrderPath.Text;
                VentanaPrincipal.LeerBuildOrders();
                VentanaPrincipal.CargarBuildOrder();
            } else if (string.IsNullOrEmpty(TxtBuildOrderPath.Text)) { 
                VentanaPrincipal.Preferencias.BuildOrderDirectory = null;
                VentanaPrincipal.LeerBuildOrders();
                VentanaPrincipal.CargarBuildOrder();
            } 

        } // TxtBuildOrderPath_TextChanged>


    } // SettingsWindow>


} // RTSHelper>
