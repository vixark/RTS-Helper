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
using System.Windows.Threading;
using Microsoft.VisualBasic;
using static RTSHelper.Global;
using System.IO;



namespace RTSHelper {



    public partial class MainWindow : Window {


        public List<Paso> Pasos = new List<Paso>();

        public int NúmeroPaso = 0;

        public bool EsÚltimoPaso => NúmeroPaso == Pasos.Count - 1;

        public bool EsDespuésDeÚltimoPaso => NúmeroPaso > Pasos.Count - 1;

        private DispatcherTimer? Timer;

        private DispatcherTimer? TimerStepEndSound;

        private DispatcherTimer TimerFocus = new DispatcherTimer();

        private DispatcherTimer TimerFlash = new DispatcherTimer();

        private DispatcherTimer TimerBlinkerGameTime = new DispatcherTimer();

        private DispatcherTimer TimerActualizadorUI = new DispatcherTimer();

        private Stopwatch MedidorTimer = new Stopwatch();

        private bool Inició = false;

        private bool CambiandoTxtPasoAutomáticamente = true;

        private bool ActualizarDuraciónPasoEnTimerEnPróximoTick = false;

        private bool EstableciendoTamaño = false;

        private bool EditandoComboBoxEnCódigo = false;

        private double DuraciónPasoParcialAnterior = 0;

        private bool SilenciadoAlCompletar = false;

        private double ÚltimosSegundosJuego = 0;

        private EEstado Estado { get; set; } = EEstado.Stoped;

        private enum EEstado { Stoped, Running, Paused }


        #region Eventos

        public MainWindow() {

            InitializeComponent();
            TimerFocus.Interval = TimeSpan.FromMilliseconds(20);
            TimerFocus.Tick += new EventHandler(TimerFocus_Tick);
            TimerFlash.Interval = TimeSpan.FromMilliseconds(500);
            TimerFlash.Tick += new EventHandler(TimerFlash_Tick);
            TimerActualizadorUI.Interval = TimeSpan.FromMilliseconds(200);
            TimerActualizadorUI.Tick += new EventHandler(TimerActualizadorUI_Tick);
            TimerActualizadorUI.Start();
            TimerBlinkerGameTime.Interval = TimeSpan.FromMilliseconds(300);
            TimerBlinkerGameTime.Tick += new EventHandler(TimerBlinkerGameTime_Tick);

            LeerPreferencias();
            Pasos = Paso.LeerPasos(DirectorioBuildOrdersEfectivo, Preferencias.CurrentBuildOrder);
            LeerBuildOrders();
            CargarBuildOrder();
            CargarVelocidadEjecución();
            CrearEntidadesYNombres();

            Inició = true;
            CambiandoTxtPasoAutomáticamente = false;
            
        } // MainWindow>


        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        } // Window_MouseDown>


        private void BtnStart_Click(object sender, RoutedEventArgs e) {

            if (Estado == EEstado.Stoped || Estado == EEstado.Paused) {
                LblTiempoEnJuego.Content = "0:00"; // Para evitar un pequeño retraso en la actualización.
                BtnStart.Content = "❚❚";
                BtnStart.ToolTip = "Pause";
                BtnRestart.ToolTip = "Restart";
                BtnRestart.Content = "⟳";
                BtnNext.IsEnabled = true;
                BtnPrevious.IsEnabled = true;
                TxtPaso.IsEnabled = true;
                CmbVelocidadEjecución.IsEnabled = true;
            }

            switch (Estado) {
                case EEstado.Stoped: // Start.

                    NúmeroPaso = 0;
                    ReiniciarPasoActual();
                    Estado = EEstado.Running;
                    SuspenderBlinkingTiempoJuego();
                    break;

                case EEstado.Running: // Pause.

                    BtnStart.Content = "▷";
                    BtnStart.ToolTip = "Restart";
                    BtnRestart.ToolTip = "Stop";
                    BtnRestart.Content = "■";
                    BtnNext.IsEnabled = false;
                    BtnPrevious.IsEnabled = false;
                    TxtPaso.IsEnabled = false;
                    CmbVelocidadEjecución.IsEnabled = false; // Para cambiar la velocidad de ejeución se requiere el MedidorTimer ejecutando.            
                    MedidorTimer.Stop(); // Lo suspende, pero almacena la duración actual.
                    Timer?.Stop(); // Lo suspende completamente.
                    TimerFlash.Stop(); // Lo suspende completamente.
                    TimerStepEndSound?.Stop(); // Lo suspende completamente.
                    Estado = EEstado.Paused;
                    TimerBlinkerGameTime.Start();
                    break;

                case EEstado.Paused: // Restart.

                    if (Timer != null) {

                        MedidorTimer.Start();
                        ActualizarDuraciónPasoEnTimerEnPróximoTick = true;
                        DuraciónPasoParcialAnterior += MedidorTimer.ElapsedMilliseconds;
                        ActualizarIntervaloTimer(new TimeSpan(0, 0, 0, 0, (int)Math.Round(Timer.Interval.TotalMilliseconds - MedidorTimer.ElapsedMilliseconds, 0)));
                        ActualizarUI();

                    }
                    Estado = EEstado.Running;
                    SuspenderBlinkingTiempoJuego();
                    break;

                default:
                    break;
            }

        } // BtnStart_Click>


        private void BtnRestart_Click(object sender, RoutedEventArgs e) {

            switch (Estado) {
                case EEstado.Stoped: // Stop otra vez. No pasa nada.
                    break;
                case EEstado.Running: // Restart.

                    LblTiempoEnJuego.Content = "0:00"; // Para evitar un pequeño retraso en la actualización.
                    NúmeroPaso = 0;
                    ReiniciarPasoActual();
                    if (Preferencias.MuteOnComplete && SilenciadoAlCompletar) {
                        Preferencias.Muted = false;
                        SilenciadoAlCompletar = false;
                        AplicarPreferenciasMuted(iniciando: false);
                    }
                    Estado = EEstado.Running;
                    SuspenderBlinkingTiempoJuego();
                    break;

                case EEstado.Paused: // Stop.

                    LblTiempoEnJuego.Content = "0:00";
                    NúmeroPaso = 0;
                    TxbPaso.Text = "";
                    TxbPasoSiguiente.Text = "";
                    CmbVelocidadEjecución.IsEnabled = true; // Cuando está parado completamente si se puede cambiar la velocidad de ejecución.
                    Timer?.Stop();
                    TimerFlash.Stop();
                    MedidorTimer.Reset();
                    TimerStepEndSound?.Stop();
                    ActualizarUI();
                    Estado = EEstado.Stoped;
                    SuspenderBlinkingTiempoJuego();
                    break;

                default:
                    break;
            }

        } // BtnRestart_Click>


        private void MniRestartStep_Click(object sender, RoutedEventArgs e) 
            => ReiniciarPasoActual();


        public void Flash() {

            TimerFlash.Start();
            if (!(Preferencias.StopFlashingOnComplete && EsDespuésDeÚltimoPaso) && Preferencias.FlashOnStepChange) {
                Application.Current.Resources["ColorFondo"] = (Color)System.Windows.Media.ColorConverter.ConvertFromString(Preferencias.FlashingColor);
                Application.Current.Resources["Opacidad"] = Preferencias.FlashingOpacity;
            }

        } // Flash>


        private void Timer_Tick(object? sender, EventArgs e) {

            MedidorTimer.Reset();
            NúmeroPaso++;
            ActualizarTexto();
            DuraciónPasoParcialAnterior = 0;
            GuardarDuraciónPaso(NúmeroPaso - 1);
            ActualizarUI();
            Flash();

            if (!Preferencias.Muted)
                MediaPlayer.PlaySonidoInicio();
            if (ActualizarDuraciónPasoEnTimerEnPróximoTick) {
                ActualizarIntervaloTimer(ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed));
                ActualizarDuraciónPasoEnTimerEnPróximoTick = false;
            }

            if (Preferencias.MinimizeOnComplete && EsÚltimoPaso) this.WindowState = WindowState.Minimized;
            if (Preferencias.MuteOnComplete && EsÚltimoPaso) { // Se hace después de reproducir el último sonido por consistencia.
                Preferencias.Muted = true;
                SilenciadoAlCompletar = true;
                AplicarPreferenciasMuted(iniciando: false);
            }

            if (!(Timer is null)) ReiniciarTimerStepEndSound(Timer.Interval);
            MedidorTimer.Start(); // Se reinicia siempre para evitar que se descoordine con el Timer. Si se dejaba correr por su propia cuenta terminaba desfazándose con el Timer y generando problemas en los momentos entre paso y paso. Pasaba de 19 a 10 y después a 21.

        } // Timer_Tick>


        private void TimerStepEndSound_Tick(object? sender, EventArgs e) {

            TimerStepEndSound?.Stop();
            if (!Preferencias.Muted) MediaPlayer.PlaySonidoFinal();

        } // TimerStepEndSound_Tick>


        private void BtnNext_Click(object sender, RoutedEventArgs e) {

            NúmeroPaso++;
            GuardarDuraciónPaso(NúmeroPaso - 1);
            ActualizarTexto();
            ActualizarUI();

        } // BtnNext_Click>


        private void BtnPrevious_Click(object sender, RoutedEventArgs e) {

            NúmeroPaso--;
            if (NúmeroPaso == -1) {
                ReiniciarPasoActual();
            } else {
                ActualizarTexto();
                ActualizarUI();
            }

        } // BtnPrevious_Click>


        private void TxtPaso_TextChanged(object sender, TextChangedEventArgs e) {

            if (int.TryParse(TxtPaso.Text, out int intPaso)) {
                NúmeroPaso = intPaso;
                if (CambiandoTxtPasoAutomáticamente) return;
                ReiniciarPasoActual();
            }

        } // TxtPaso_TextChanged>


        private void TimerFocus_Tick(object? sender, EventArgs e) {
            TimerFocus.Stop();
            TxtPaso.SelectAll();
        } // TimerFocus_Tick>


        private void TimerFlash_Tick(object? sender, EventArgs e) {

            TimerFlash.Stop();
            if (!(Preferencias.StopFlashingOnComplete && EsDespuésDeÚltimoPaso) && Preferencias.FlashOnStepChange) {
                Application.Current.Resources["ColorFondo"] = (Color)System.Windows.Media.ColorConverter.ConvertFromString(Preferencias.BackColor);
                Application.Current.Resources["Opacidad"] = Preferencias.Opacity;
            }

        } // TimerFlash_Tick>


        private void TimerBlinkerGameTime_Tick(object? sender, EventArgs e) {

            if (Estado == EEstado.Paused) {

                if (LblTiempoEnJuego.Visibility == Visibility.Visible) {
                    LblTiempoEnJuego.Visibility = Visibility.Hidden;
                } else {
                    LblTiempoEnJuego.Visibility = Visibility.Visible;
                }

            }

        } // TimerBlinkerGameTime_Tick>


        private void TimerActualizadorUI_Tick(object? sender, EventArgs e) => ActualizarUI();


        private void CmbBuildOrders_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Inició || EditandoComboBoxEnCódigo) return;
            Preferencias.CurrentBuildOrder = ObtenerSeleccionadoEnCombobox(e);
            CargarBuildOrder();

        } // CmbBuildOrders_SelectionChanged>


        private void CmbVelocidadEjecución_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Inició) return;
            var cbi = e.AddedItems[0] as ComboBoxItem;
            if (cbi is null) return;
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

            var winSettings = new SettingsWindow(primerInicio: false, this);
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

            Preferencias.Muted = !Preferencias.Muted;
            AplicarPreferencias();

        } // BtnMute_Click>


        private void MniLastStep_Click(object sender, RoutedEventArgs e) {

            var pasoInicial = NúmeroPaso;
            GuardarDuraciónPaso(pasoInicial);
            NúmeroPaso = Pasos.Count - 1;
            for (int i = pasoInicial; i < NúmeroPaso; i++) {
                Pasos[i].DuraciónEnJuego = Pasos[pasoInicial].DuraciónEnJuego;
            }
            ActualizarTexto();
            ActualizarUI();

        } // MniLastStep_Click>


        #endregion Eventos>


        #region Procedimientos y Funciones

        public void LeerBuildOrders() {

            EditandoComboBoxEnCódigo = true;
            CmbBuildOrders.Items.Clear();
            CmbBuildOrders.Items.Add("Default");
            CmbBuildOrders.SelectedIndex = 0;
            EditandoComboBoxEnCódigo = false;

            var archivosBuildOrders = Directory.GetFiles(DirectorioBuildOrdersEfectivo, "*.txt");
            foreach (var archivoBuildOrder in archivosBuildOrders) {
                var nombreBuildOrder = Path.GetFileNameWithoutExtension(archivoBuildOrder);
                if (nombreBuildOrder.ToLower() != "default") CmbBuildOrders.Items.Add(nombreBuildOrder);
            }

        } // LeerBuildOrders>


        private void ActualizarUI() {

            if (Timer is null || !Timer.IsEnabled) {
                // Cuando esté en pausa no debe actualizar ni borrar el temporizador.
            } else {

                if (MedidorTimer.IsRunning) {

                    var velocidadJuegoEfectiva = ObtenerVelocidadJuegoEfectiva(Preferencias.GameSpeed);
                    var segundosJuego = Paso.ObtenerDuraciónPasosAnteriores(Pasos, NúmeroPaso)
                        + (ObtenerTiempoPasoActual().TotalMilliseconds + DuraciónPasoParcialAnterior) * velocidadJuegoEfectiva / 1000; // Step duration es en segundos de juego, en cambio el temporizador es en segundos reales, por eso solo se ajusta este último valor a los segundos en el juego que son los que finalmente se muestran.
                    var segundos = segundosJuego % 60;
                    if (segundos < ÚltimosSegundosJuego) { // En algunas ocasiones en el límite del entre dos pasos el MedidorTimer se adelanta al Tick del Timer y por lo tanto sucede que por ejemplo tenga 60.04 segundos (dando 0.04 en el nuevo paso) mientras que el Timer aún no ha hecho el evento Tick y por lo tanto aún no se ha aumentado el paso. Esto produce error en la presentación porque pasa de 29 a 15 y después a 31 rápidamente. Para evitarlo sin complejizar mucho el código simplemente no se actualizará la UI en estos casos.
                        //var msmedidortimer = MedidorTimer.Elapsed.TotalSeconds;
                        //var mstimer = Timer.Interval.TotalSeconds;
                    } else {
                        LblTiempoEnJuego.Content = Math.Floor(segundosJuego / 60).ToString() + ":" + (segundos > 59 ? 59 : Math.Round(segundos)).ToString("00");
                    }
                    ÚltimosSegundosJuego = segundos;

                }

            }

        } // ActualizarUI>


        private void GuardarDuraciónPaso(int númeroPaso) {
            if (númeroPaso <= Pasos.Count - 1) Pasos[númeroPaso].DuraciónEnJuego = Preferencias.StepDuration / Preferencias.ExecutionSpeed;
        } // GuardarDuraciónPasoAnterior>


        public void AplicarPreferencias(bool iniciando = false) {

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
            Application.Current.Resources["BrushFlashingColor"] = (SolidColorBrush)new BrushConverter().ConvertFrom(Preferencias.FlashingColor);
            AplicarPreferenciasMuted(iniciando);

            this.Width = Preferencias.Width; // Se deben establecer manualmente porque no funciona el DynamicResource.
            this.Left = Preferencias.Left;
            this.Height = Preferencias.Height;
            this.Top = Preferencias.Top;
            EstableciendoTamaño = false;

        } // AplicarPreferencias>
     
        
        public void AplicarPreferenciasMuted(bool iniciando) {

            BtnMute.Content = !Preferencias.Muted || (iniciando && Preferencias.UnmuteAtStartup) ? "🔈" : "🔇";
            BtnMute.ToolTip = !Preferencias.Muted || (iniciando && Preferencias.UnmuteAtStartup) ? "Mute" : "Unmute";
            BtnMute.Visibility = Preferencias.StepStartSound == NoneSoundString && Preferencias.StepEndSound == NoneSoundString 
                ? Visibility.Collapsed : Visibility.Visible;
            if (Preferencias.Muted) MediaPlayer.Player?.controls.stop();

        } // AplicarPreferenciasMuted>


        private void LeerPreferencias() {

            var crearNuevas = false;
            if (File.Exists(RutaPreferencias)) {

                var preferencias = Settings.Deserializar(File.ReadAllText(RutaPreferencias));     
                if (preferencias == null) {
                    MessageBox.Show($"The settings couldn't be read from {RutaPreferencias}. Default settings will be loaded.");
                    File.Copy(RutaPreferencias, Path.Combine(Path.GetDirectoryName(RutaPreferencias)!, 
                        $"Settings.json.bak.{DateTime.Now.ToString("yyyy-MM-dd-hh-mm")}"));
                    crearNuevas = true;
                } else {
                    Preferencias = preferencias;
                }
               
            } else {
                crearNuevas = true;
            }

            if (crearNuevas) {

                Preferencias = new Settings();
                var resoluciónRecomendada = ObtenerResoluciónRecomendada();
                var juegoRecomendado = AOE2Name;
                Preferencias.EstablecerValoresRecomendados(resoluciónRecomendada, juegoRecomendado);
                var winSettings = new SettingsWindow(primerInicio: true, this);
                winSettings.ShowDialog();

            }
            if (Preferencias.StepEndSoundDuration == 0) Preferencias.StepEndSoundDuration = ObtenerDuraciónEndStepSound();

            AplicarPreferencias(iniciando: true);

        } // LeerPreferencias>


        private void ReiniciarPasoActual() {

            if (!(Timer is null)) Timer.Stop();
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(Timer_Tick);

            if (!(TimerStepEndSound is null)) TimerStepEndSound.Stop();
            TimerStepEndSound = new DispatcherTimer();
            TimerStepEndSound.Tick += new EventHandler(TimerStepEndSound_Tick);

            ActualizarIntervaloTimer(ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed));
            ActualizarTexto();

        } // Reiniciar>


        private double ObtenerVelocidadJuegoEfectiva(double velocidadJuego) => velocidadJuego / (velocidadJuego == 1.7 ? 1.02 : 1); // En realidad la velocidad 1.7 de AOE2 corresponde aproximadamente a 36 s reales. Lo cual es 2% más lento de lo esperado (60/1.7 = 35.29 s).


        private TimeSpan ObtenerDuraciónPaso(double velocidadJuego, double velocidadEjecución) => new TimeSpan(0, 0, 0, 0,
            (int)Math.Round(Preferencias.StepDuration * 1000 / (ObtenerVelocidadJuegoEfectiva(velocidadJuego) * velocidadEjecución), 0)); 


        private void ActualizarIntervaloTimer(TimeSpan duración) {

            if (Timer is null) return; // No debería pasar.
            Timer.Interval = duración;
            Timer.Start(); // Se reinicia aquí porque de todas maneras al cambiar el invervalo el timer se reinicia siempre.
            ReiniciarTimerStepEndSound(duración);
            MedidorTimer.Restart(); // Se reinicia después de Timer.Start() para que siempre esté un poco por delante de este y se disminuyan los casos en los que pasa de 29 a 15 y después a 31.

        } // ActualizarIntervaloTimer>


        private void ReiniciarTimerStepEndSound(TimeSpan duraciónTimerPaso) {

            if (TimerStepEndSound is null) return; // No debería pasar.
            if (duraciónTimerPaso.TotalMilliseconds > Preferencias.StepEndSoundDuration) {
                TimerStepEndSound.Interval = duraciónTimerPaso.Add(new TimeSpan(0, 0, 0, 0, -Preferencias.StepEndSoundDuration));
                TimerStepEndSound.Start();
            } else {
                // Sucede cuando el audio es más largo que el paso y en algunas ocasiones al cambiar la duración del paso. No se reproduce nada.
            }

        } // ReiniciarTimerStepEndSound>


        public TimeSpan ObtenerTiempoPasoActual() {

            if (Timer == null) return new TimeSpan(0);
            var milisegundosMedidorTimer = MedidorTimer.Elapsed.TotalMilliseconds;
            var milisegundosPasoActual = (int)(Math.Round(milisegundosMedidorTimer % Timer.Interval.TotalMilliseconds, 0));
            //var milisegundosPasoActual2 = (int)(Math.Round(((milisegundosMedidorTimer / Timer.Interval.TotalMilliseconds) - NúmeroPaso) * Timer.Interval.TotalMilliseconds, 0)); // Aunque esta fórmula permite evitar el problema de que en algunos pasos llegaba primero en el MedidorTimer a 60.4 antes que el Timer hiciera el evento Tick (por lo tanto generando un valor incorrecto mostrado), tenía el inconveniente que complejiza el funcionamiento para los botones siguiente y después, entonces se prefirió mantener el problema en el caso límite y más bien no se actualiza la interface cuando eso sucede. Esto se maneja en el temporizador de Actualización UI.
            //if (milisegundosPasoActual != milisegundosPasoActual2) Debugger.Break();
            return new TimeSpan(0, 0, 0, 0, milisegundosPasoActual);

        } // ObtenerTiempoPasoActual>


        public void ActualizarDuraciónPaso() {

            if (Timer is null || !Timer.IsEnabled) return;

            // Al actualizar la duración de Timer.Interval se reinicia. Para evitar este problema se establece la nueva duración del paso en dos pasos.
            // Primero establece un intervalo parcial del tiempo que falta para finalizar el paso actual modificado y después establece la nueva duración del paso completa.
            var nuevaDuraciónPasoCompleto = ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed);
            var tiempoPasoActual = ObtenerTiempoPasoActual();
            TimeSpan duraciónPasoParcial;

            if (nuevaDuraciónPasoCompleto < tiempoPasoActual) { // Con la nueva velocidad el paso actual ya se habría terminado. Se debe incrementar el número de paso y hacer un paso parcial más pequeño.

                var pasosSaltados = tiempoPasoActual.TotalMilliseconds / nuevaDuraciónPasoCompleto.TotalMilliseconds; // Resta uno porque es el actual.
                var pasosSaltadosEnteros = (int)Math.Floor(pasosSaltados);
                var fracciónPasoSaltado = pasosSaltados - pasosSaltadosEnteros;
                for (int i = NúmeroPaso; i < NúmeroPaso + pasosSaltadosEnteros; i++) {
                    Pasos[i].DuraciónEnJuego = Preferencias.StepDuration / Preferencias.ExecutionSpeed;
                }
                NúmeroPaso = NúmeroPaso + pasosSaltadosEnteros;
                ActualizarTexto();
                var duraciónTranscurridaPasoActual = tiempoPasoActual.Add(-pasosSaltadosEnteros * nuevaDuraciónPasoCompleto);
                duraciónPasoParcial = nuevaDuraciónPasoCompleto - duraciónTranscurridaPasoActual; // Es la duración con la que se debe hacer un paso parcial. Es la duración del nuevo paso menos el tiempo que ya ha transcurrido de este.
                DuraciónPasoParcialAnterior = duraciónTranscurridaPasoActual.TotalMilliseconds;

            } else { // Con la nueva velocidad el paso actual aún no habría terminado. Se hace un paso parcial para terminarlo.
                duraciónPasoParcial = nuevaDuraciónPasoCompleto.Add(-tiempoPasoActual);
                DuraciónPasoParcialAnterior = tiempoPasoActual.TotalMilliseconds;
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
            TxbPaso.Text = Pasos.Count <= NúmeroPaso ? (Pasos.Count > 0 ? Pasos[Pasos.Count - 1].TextoProcesado : "") : Pasos[NúmeroPaso].TextoProcesado;
            TxbPasoSiguiente.Text = Pasos.Count <= NúmeroPaso + 1 ? "" : $"{Pasos[NúmeroPaso + 1].TextoProcesado}";

        } // ActualizarTexto>


        public void CargarBuildOrder() {

            if (CmbBuildOrders.Text != Preferencias.CurrentBuildOrder) {
                EditandoComboBoxEnCódigo = true;
                CmbBuildOrders.Text = Preferencias.CurrentBuildOrder;
                EditandoComboBoxEnCódigo = false;
            }
            var nuevosPasos = Paso.LeerPasos(DirectorioBuildOrdersEfectivo, Preferencias.CurrentBuildOrder);
            if (NúmeroPaso > 0) { // Si se carga una build order en la mitad de la ejecución, debe copiar las duraciones de los pasos de la ejecución actual.
                for (int i = 0; i < NúmeroPaso; i++) {
                    if (i <= nuevosPasos.Count - 1 && i <= Pasos.Count - 1) nuevosPasos[i].DuraciónEnJuego = Pasos[i].DuraciónEnJuego;
                }
            }
            Pasos = nuevosPasos;
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


        private void SuspenderBlinkingTiempoJuego() {

            LblTiempoEnJuego.Visibility = Visibility.Visible;
            TimerBlinkerGameTime.Stop();

        } // SuspenderBlinkingTiempoJuego>


        #endregion Procedimientos y Funciones>


    } // MainWindow>



} // RTSHelper>
