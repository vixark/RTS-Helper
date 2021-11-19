using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using WForms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.VisualBasic;



namespace RTSHelper {



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {


        public Settings Preferencias;

        private static int NúmeroPaso = 0;

        private static string DirectorioAplicación = AppDomain.CurrentDomain.BaseDirectory;

        public static string RutaPreferencias = System.IO.Path.Combine(DirectorioAplicación, "Settings.json");

        private static string DirectorioBuildOrdersPredeterminado = System.IO.Path.Combine(DirectorioAplicación, "Build Orders");

        private string DirectorioBuildOrdersEfectivo => Preferencias.BuildOrderDirectory ?? DirectorioBuildOrdersPredeterminado;

        private static string[] Pasos = new string[] { };

        private DispatcherTimer Timer;

        private DispatcherTimer TimerFocus = new DispatcherTimer();

        private DispatcherTimer TimerFlash = new DispatcherTimer();

        private Stopwatch MedidorTimer = new Stopwatch();

        private bool Inició = false;

        private bool CambiandoTxtPasoAutomáticamente = true;

        private bool ActualizarDuraciónPasoEnTimerEnPróximoTick = false;

        private bool EstableciendoTamaño = false;

        private bool EditandoComboBoxEnCódigo = false;

        private Color TemporalBackgroundColor;


        #region Eventos

        public MainWindow() {

            InitializeComponent();
            TimerFocus.Interval = TimeSpan.FromMilliseconds(20);
            TimerFocus.Tick += new EventHandler(TimerFocus_Tick);
            TimerFlash.Interval = TimeSpan.FromMilliseconds(500);
            TimerFlash.Tick += new EventHandler(TimerFlash_Tick);
            LeerPreferencias();
            LeerPasos();
            LeerBuildOrders();
            CargarBuildOrder();
            CargarVelocidadEjecución();
            Inició = true;
            CambiandoTxtPasoAutomáticamente = false;
             
        } // MainWindow>


        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        } // Window_MouseDown>


        private void BtnStart_Click(object sender, RoutedEventArgs e) {

            NúmeroPaso = 0;
            ReiniciarTimer();

        } // BtnStart_Click>


        private void Timer_Tick(object sender, EventArgs e) {

            NúmeroPaso++;
            ActualizarTexto();

            TimerFlash.Start();
            TemporalBackgroundColor = ((SolidColorBrush)this.Background).Color;
            Application.Current.Resources["ColorFondo"] = (Color)System.Windows.Media.ColorConverter.ConvertFromString(Preferencias.CurrentStepFontColor);
            Application.Current.Resources["Opacidad"] = (double)1;

            if (Preferencias.PlaySoundEachStep) Console.Beep(400, 200);
            if (ActualizarDuraciónPasoEnTimerEnPróximoTick) {
                ActualizarIntervaloTimer(ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed));
                ActualizarDuraciónPasoEnTimerEnPróximoTick = false;
            }

        } // Timer_Tick>


        private void BtnNext_Click(object sender, RoutedEventArgs e) {
            NúmeroPaso++;
            ActualizarTexto();
        } // BtnNext_Click>


        private void BtnPrev_Click(object sender, RoutedEventArgs e) {
            NúmeroPaso--;
            ActualizarTexto();
        } // BtnPrev_Click>


        private void TxtPaso_TextChanged(object sender, TextChangedEventArgs e) {

            if (int.TryParse(TxtPaso.Text, out int intPaso)) {
                NúmeroPaso = intPaso;
                if (CambiandoTxtPasoAutomáticamente) return;
                ReiniciarTimer();
            }

        } // TxtPaso_TextChanged>


        private void TimerFocus_Tick(object sender, EventArgs e) {
            TimerFocus.Stop();
            TxtPaso.SelectAll();
        } // TimerFocus_Tick>


        private void TimerFlash_Tick(object sender, EventArgs e) {
            TimerFlash.Stop();
            Application.Current.Resources["ColorFondo"] = (Color)System.Windows.Media.ColorConverter.ConvertFromString(Preferencias.BackColor);
            Application.Current.Resources["Opacidad"] = Preferencias.Opacity;
        } // TimerFlash_Tick>


        private void CmbBuildOrders_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Inició || EditandoComboBoxEnCódigo) return;
            var cbi = e.AddedItems[0] as ComboBoxItem;
            if (cbi != null) {
                Preferencias.CurrentBuildOrder = cbi.Content.ToString(); ;
            } else {
                var str = e.AddedItems[0] as string;
                Preferencias.CurrentBuildOrder = str.ToString();
            }
            CargarBuildOrder();

        } // CmbBuildOrders_SelectionChanged>


        private void CmbVelocidadEjecución_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Inició) return;
            var cbi = e.AddedItems[0] as ComboBoxItem;
            Preferencias.ExecutionSpeed = Convert.ToDouble(cbi.Tag) / 100;
            ActualizarDuraciónPaso();

        } // CmbVelocidadJugador_SelectionChanged>


        private void TxtPaso_GotFocus(object sender, RoutedEventArgs e)
            => TimerFocus.Start();


        private void TxtPaso_PreviewMouseDown(object sender, MouseButtonEventArgs e)
            => TimerFocus.Start();


        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => this.Close();


        private void BtnSettings_Click(object sender, RoutedEventArgs e) {
            var winSettings = new SettingsWindow(this, primerInicio: false);
            winSettings.Topmost = true;
            winSettings.ShowDialog();
        } // BtnSettings_Click>


        private void Window_Closed(object sender, EventArgs e)
            => Settings.Guardar(Preferencias, RutaPreferencias);


        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {

            if (!Inició) return;
            Preferencias.Top = this.Top;
            Preferencias.Left = this.Left;

        } // Window_MouseUp>


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {

            if (!Inició || EstableciendoTamaño) return;
            Preferencias.Width = this.Width;
            Preferencias.Height = this.Height;

        } // Window_SizeChanged>


        private void BtnMinize_Click(object sender, RoutedEventArgs e)
            => this.WindowState = WindowState.Minimized;


        private void BtnMute_Click(object sender, RoutedEventArgs e) {

        }


        private void BtnRestart_Click(object sender, RoutedEventArgs e) {

        }


        #endregion Eventos>


        #region Procedimientos y Funciones


        private void LeerPasos() {

            var rutaBuildOrder = System.IO.Path.Combine(DirectorioBuildOrdersEfectivo, $"{Preferencias.CurrentBuildOrder}.txt");
            if (!System.IO.Directory.Exists(DirectorioBuildOrdersEfectivo)) System.IO.Directory.CreateDirectory(DirectorioBuildOrdersEfectivo);
            if (!System.IO.File.Exists(rutaBuildOrder))
                System.IO.File.WriteAllText(rutaBuildOrder, $@"Edit '\RTS Helper\Build Orders\{Preferencias.CurrentBuildOrder}.txt' \n to add your build order.");
            Pasos = System.IO.File.ReadAllLines(rutaBuildOrder);

        } // LeerPasos>


        public void LeerBuildOrders() {

            EditandoComboBoxEnCódigo = true;
            CmbBuildOrders.Items.Clear();
            CmbBuildOrders.Items.Add("Default");
            CmbBuildOrders.SelectedIndex = 0;
            EditandoComboBoxEnCódigo = false;

            var archivosBuildOrders = System.IO.Directory.GetFiles(DirectorioBuildOrdersEfectivo, "*.txt");
            foreach (var archivoBuildOrder in archivosBuildOrders) {
                var nombreBuildOrder = System.IO.Path.GetFileNameWithoutExtension(archivoBuildOrder);
                if (nombreBuildOrder.ToLower() != "default") CmbBuildOrders.Items.Add(nombreBuildOrder);
            }

        } // LeerBuildOrders>


        public static string ObtenerResoluciónRecomendada() {

            var interopHelper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            var pantallaActual = WForms.Screen.FromHandle(interopHelper.Handle);
            var anchoPantalla = pantallaActual.Bounds.Width;
            var altoPantalla = pantallaActual.Bounds.Height;
            var resolución = "1920x1080";
            if (anchoPantalla == 1920 && altoPantalla == 1080) {
                resolución = "1920x1080";
            } else if (anchoPantalla == 2560 && altoPantalla == 1440) {
                resolución = "2560x1440";
            } else if (anchoPantalla == 1366 && altoPantalla == 768) {
                resolución = "1366x768";
            } else if (altoPantalla >= 1440) {
                resolución = "2560x1440";
            } else if (altoPantalla >= 1080) {
                resolución = "1920x1080";
            } else {
                resolución = "1366x768";
            }
            return resolución;

        } // ObtenerResoluciónRecomendada>


        public void AplicarPreferencias() {

            EstableciendoTamaño = true;
            Application.Current.Resources["FuenteNormal"] = Preferencias.MediumFontSize;
            Application.Current.Resources["FuenteGrande"] = Preferencias.LargeFontSize;
            Application.Current.Resources["FuentePaso"] = Preferencias.CurrentStepFontSize;
            Application.Current.Resources["FuenteSiguientePaso"] = Preferencias.NextStepFontSize;
            Application.Current.Resources["MargenBotones"] = new Thickness(Preferencias.ButtonsMargin);
            Application.Current.Resources["PaddingBotones"] = new Thickness(Preferencias.ButtonsPadding);
            Application.Current.Resources["TamañoBotones"] = Preferencias.ButtonsSize;
            Application.Current.Resources["BrushFuente"] = (SolidColorBrush)new BrushConverter().ConvertFrom(Preferencias.FontColor);
            Application.Current.Resources["BrushFondo"] = (SolidColorBrush)new BrushConverter().ConvertFrom(Preferencias.BackColor);
            Application.Current.Resources["ColorFondo"] = (Color)System.Windows.Media.ColorConverter.ConvertFromString(Preferencias.BackColor);
            Application.Current.Resources["BrushPaso"] = (SolidColorBrush)new BrushConverter().ConvertFrom(Preferencias.CurrentStepFontColor);
            Application.Current.Resources["BrushPasoSiguiente"] = (SolidColorBrush)new BrushConverter().ConvertFrom(Preferencias.NextStepFontColor);
            Application.Current.Resources["Opacidad"] = Preferencias.Opacity;
            Application.Current.Resources["MargenPaso"] = new Thickness(Preferencias.LeftMarginCurrentStep, Preferencias.TopMarginCurrentStep, 0, 0);
            Application.Current.Resources["MargenPasoSiguiente"] = new Thickness(0, Preferencias.TopMarginNextStep, Preferencias.RightMarginNextStep, 0);
            Application.Current.Resources["Alto"] = Preferencias.Height;
            Application.Current.Resources["Ancho"] = Preferencias.Width;
            Application.Current.Resources["PosiciónY"] = Preferencias.Top;
            Application.Current.Resources["PosiciónX"] = Preferencias.Left;
            Application.Current.Resources["VisibilidadPasoSiguiente"] = Preferencias.ShowNextStep ? Visibility.Visible : Visibility.Collapsed;
            Application.Current.Resources["AnchoSelectorBuildOrder"] = Preferencias.BuildOrderSelectorWidth;
            Application.Current.Resources["AnchoSelectorVelocidadEjecución"] = Preferencias.ExecutionSpeedSelectorWidth;

            this.Width = Preferencias.Width; // Se deben establecer manualmente porque no funciona el DynamicResource.
            this.Left = Preferencias.Left;
            this.Height = Preferencias.Height;
            this.Top = Preferencias.Top;
            EstableciendoTamaño = false;

        } // AplicarPreferencias>


        private void LeerPreferencias() {

            if (System.IO.File.Exists(RutaPreferencias)) {
                Preferencias = Settings.Deserializar(System.IO.File.ReadAllText(RutaPreferencias));
            } else {

                Preferencias = new Settings();
                var resoluciónRecomendada = ObtenerResoluciónRecomendada();
                var juegoRecomendado = Settings.AOE2Name;
                Preferencias.EstablecerValoresRecomendados(resoluciónRecomendada, juegoRecomendado);
                var winSettings = new SettingsWindow(this, primerInicio: true);
                winSettings.ShowDialog();

            }

            AplicarPreferencias();

        } // LeerPreferencias>


        private void ReiniciarTimer() {

            if (!(Timer is null)) Timer.Stop();
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(Timer_Tick);
            ActualizarIntervaloTimer(ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed));
            ActualizarTexto();

        } // ReiniciarTimer>


        private TimeSpan ObtenerDuraciónPaso(double velocidadJuego, double velocidadEjecución)
            => new TimeSpan(0, 0, 0, 0, (int)Math.Round((velocidadJuego == 1.7 ? 1.02 : 1) * 60 * 1000 / (velocidadJuego * velocidadEjecución), 0)); // En realidad la velocidad 1.7 de AOE2 corresponde aproximadamente a 36 s reales. Lo cual es alrededor de 2% más lento de lo esperado (60/1.7 = 35.29 s).


        private void ActualizarIntervaloTimer(TimeSpan duración) {

            Timer.Interval = duración;
            Timer.Start(); // Se reinicia aquí porque de todas maneras al cambiar el invervalo el timer se reinicia siempre.
            MedidorTimer.Restart();

        } // ActualizarIntervaloTimer>


        public void ActualizarDuraciónPaso() {

            if (Timer is null || !Timer.IsEnabled) return;

            // Al actualizar la duración de Timer.Interval se reinicia. Para evitar este problema se establece la nueva duración del paso en dos pasos.
            // Primero establece un intervalo parcial del tiempo que falta para finalizar el paso actual modificado y después establece la nueva duración del paso completa.
            var nuevaDuraciónPasoCompleto = ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed);
            var elapsedTotal = MedidorTimer.Elapsed;
            var elapsedPasoActual = new TimeSpan(0, 0, 0, 0, (int)(Math.Round(elapsedTotal.TotalMilliseconds % Timer.Interval.TotalMilliseconds, 0)));
            TimeSpan duraciónPasoParcial;

            if (nuevaDuraciónPasoCompleto < elapsedPasoActual) { // Con la nueva velocidad el paso actual ya se habría terminado. Se debe incrementar el número de paso y hacer un paso parcial más pequeño.

                var pasosSaltados = elapsedPasoActual.TotalMilliseconds / nuevaDuraciónPasoCompleto.TotalMilliseconds; // Resta uno porque es el actual.
                var pasosSaltadosEnteros = (int)Math.Floor(pasosSaltados);
                var fracciónPasoSaltado = pasosSaltados - pasosSaltadosEnteros;
                NúmeroPaso = NúmeroPaso + pasosSaltadosEnteros;
                ActualizarTexto();
                duraciónPasoParcial = elapsedPasoActual.Add(-pasosSaltadosEnteros * nuevaDuraciónPasoCompleto);

            } else { // Con la nueva velocidad el paso actual aún no habría terminado. Se hace un paso parcial para terminarlo.
                duraciónPasoParcial = nuevaDuraciónPasoCompleto.Add(-elapsedPasoActual);
            }

            ActualizarDuraciónPasoEnTimerEnPróximoTick = true;
            ActualizarIntervaloTimer(duraciónPasoParcial);

        } // ActualizarDuraciónPaso>


        private void ActualizarTexto() {

            if (Timer is null || !Timer.IsEnabled) return; // Evita que se actualice el texto si no se ha dado clic en Start.
            if (NúmeroPaso < 0) NúmeroPaso = 0;
            CambiandoTxtPasoAutomáticamente = true;
            TxtPaso.Text = NúmeroPaso.ToString();
            CambiandoTxtPasoAutomáticamente = false;
            TxbPaso.Text = Pasos.Length <= NúmeroPaso ? "End" : ProcesarTextoPaso(Pasos[NúmeroPaso]);
            TxbPasoSiguiente.Text = Pasos.Length <= NúmeroPaso + 1 ? (Pasos.Length <= NúmeroPaso ? "" : "End")
                : $"{ProcesarTextoPaso(Pasos[NúmeroPaso + 1])}";

        } // ActualizarTexto>


        private string ProcesarTextoPaso(string textoPaso)
            => textoPaso.Replace(" \\n\\n ", "\n\n").Replace(" \\n ", "\n").Replace("\\n", "\n").Replace("     ", "\n");


        public void CargarBuildOrder() {

            if (CmbBuildOrders.Text != Preferencias.CurrentBuildOrder) {
                EditandoComboBoxEnCódigo = true;
                CmbBuildOrders.Text = Preferencias.CurrentBuildOrder;
                EditandoComboBoxEnCódigo = false;
            }
            LeerPasos();
            ActualizarTexto();

        } // CargarBuildOrder>


        private void CargarVelocidadEjecución() {

            var textoVelocidadEjecución = (Preferencias.ExecutionSpeed * 100).ToString() + "%";
            if (CmbVelocidadEjecución.Text != textoVelocidadEjecución) {
                EditandoComboBoxEnCódigo = true;
                CmbVelocidadEjecución.Text = textoVelocidadEjecución;
                EditandoComboBoxEnCódigo = false;
            }

        } // CargarVelocidadEjecución>


        #endregion Procedimientos y Funciones>


    } // MainWindow>



} // RTSHelper>
