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
using static Vixark.General;
using System.Drawing.Drawing2D;
using System.Threading;



namespace RTSHelper {



    public partial class MainWindow : Window {



        #region Propiedades y Variables

        public OrdenDeEjecución OrdenDeEjecución { get; set; } = new OrdenDeEjecución();

        private DispatcherTimer? Timer; // Temporizador principal que genera los cambios de pasos.

        private DispatcherTimer? TimerStepEndSound;

        private DispatcherTimer TimerFocus = new DispatcherTimer();

        private DispatcherTimer TimerFlash = new DispatcherTimer();

        private DispatcherTimer TimerBlinkerGameTime = new DispatcherTimer();

        private DispatcherTimer TimerActualizadorUI = new DispatcherTimer();

        private DispatcherTimer TimerActualizadorUIPorCambioTamaño = new DispatcherTimer();

        private DispatcherTimer TimerVerificadorVentanaEsVisible = new DispatcherTimer();

        private DispatcherTimer TimerDetecciónPausa = new DispatcherTimer();

        private DispatcherTimer TimerDetecciónProgreso = new DispatcherTimer();

        private Stopwatch MedidorTimer = new Stopwatch(); // Temporizador medidor del tiempo.

        private bool Inició = false;

        private bool EstablecióTamañoInicial = false;

        private bool CambiandoTxtPasoAutomáticamente = true;

        private bool ActualizarDuraciónPasoEnTimerEnPróximoTick = false;

        private bool EstableciendoTamaño = false;

        public bool EditandoComboBoxEnCódigo = false;

        private double MilisegundosTimerAntesDePausa = 0;

        private double DuraciónPasoParcialPorCambioDuración = 0;

        private bool SilenciadoAlCompletar = false;

        private double ÚltimosSegundosJuego = 0;

        private EEstado Estado { get; set; } = EEstado.Stoped;

        private enum EEstado { Stoped, Running, Paused }

        private bool MostrandoPasoAnterior = false;

        private double MilisegundosJuegoDesface = 0; // Milisegundos de desface acumulado. Se usa principalmente para ajustar el reloj para que no se vea afectado por los desfaces. Esto se hace así por diseño debido a que el reloj se espera que esté sincronizado con el reloj del juego y los desfaces son causados debidos a errores del jugador, por ejemplo si no creó el aldeano a tiempo y tardó 10 segundos con el centro de pueblo desocupado se debería desfazar la ejecución 10 segundos, pero el reloj se mantendría igual.

        FileSystemWatcher SupervisorOrdenDeEjecuciónActual;

        FileSystemWatcher? SupervisorOrdenDeEjecuciónActualEnCódigo; // Para el modo de desarrollo también supervisa la carpeta de las órdenes de ejecución en la carpeta código para permitir que durante el desarrollo se tenga que actualizar únicamente este archivo.

        private int? ÚltimoProgresoLeído = null;

        #endregion Propiedades y Variables>



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
            TimerActualizadorUIPorCambioTamaño.Interval = TimeSpan.FromMilliseconds(1000);
            TimerActualizadorUIPorCambioTamaño.Tick += new EventHandler(TimerActualizadorUIPorCambioTamaño_Tick);
            TimerVerificadorVentanaEsVisible.Interval = TimeSpan.FromMilliseconds(5000);
            TimerVerificadorVentanaEsVisible.Tick += new EventHandler(TimerVerificadorVentanaEsVisible_Tick);
            TimerVerificadorVentanaEsVisible.Start();
            TimerBlinkerGameTime.Interval = TimeSpan.FromMilliseconds(300);
            TimerBlinkerGameTime.Tick += new EventHandler(TimerBlinkerGameTime_Tick);
            TimerDetecciónPausa.Interval = TimeSpan.FromMilliseconds(1000); // Cada ejecución tarda en mi computador alrededor de 50 a 70 ms. 
            TimerDetecciónPausa.Tick += new EventHandler(TimerDetecciónPausa_Tick);

            TimerDetecciónProgreso.Interval = TimeSpan.FromMilliseconds(1000); // Cada ejecución tarda en mi computador alrededor de 50 ms. 
            TimerDetecciónProgreso.Tick += new EventHandler(TimerDetecciónProgreso_Tick);

            LeerPreferencias();
            OrdenDeEjecución.EnCambioNúmeroPaso = () => EnCambioNúmeroPaso();
            OrdenDeEjecución.CargarPasos(Preferencias.BuildOrdersDirectory, Preferencias.CurrentBuildOrder);
            LeerBuildOrders();
            CargarBuildOrder(iniciando: true);

            SupervisorOrdenDeEjecuciónActual = new FileSystemWatcher { NotifyFilter = NotifyFilters.LastWrite };
            ActualizarSupervisorOrdenDeEjecución();
            SupervisorOrdenDeEjecuciónActual.Changed += OrdenDeEjecuciónActual_Changed;
            SupervisorOrdenDeEjecuciónActual.EnableRaisingEvents = true;

            if (ModoDesarrollo) {
                SupervisorOrdenDeEjecuciónActualEnCódigo = new FileSystemWatcher { NotifyFilter = NotifyFilters.LastWrite };
                ActualizarSupervisorOrdenDeEjecuciónEnCódigo();
                SupervisorOrdenDeEjecuciónActualEnCódigo.Changed += OrdenDeEjecuciónActualEnCódigo_Changed;
                SupervisorOrdenDeEjecuciónActualEnCódigo.EnableRaisingEvents = true;
            }

            CrearEntidadesYNombres();
            ActualizarContenidoPaso(númeroPaso: null);

            Inició = true;
            CambiandoTxtPasoAutomáticamente = false;

        } // MainWindow>


        private void OrdenDeEjecuciónActual_Changed(object source, FileSystemEventArgs e) {

            Thread.Sleep(100); // Le da un tiempo para que termine de grabar.
            if (ModoDesarrollo) Thread.Sleep(100); // En modo desarrollo le da más tiempo para que termine de copiar el archivo desde la carpeta de código.
            this.Dispatcher.Invoke(() => CargarBuildOrder());

        } // OrdenDeEjecuciónActual_Changed>


        private void OrdenDeEjecuciónActualEnCódigo_Changed(object source, FileSystemEventArgs e) {

            Thread.Sleep(100); // Le da un tiempo para que termine de grabar.
            var nombreArchivo = $"{Preferencias.CurrentBuildOrder}.txt";
            var rutaOrigen 
                = Path.Combine(Settings.ObtenerDirectorioÓrdenesDeEjecución(Global.DirectorioÓrdenesDeEjecuciónCódigo, Preferencias.Game), nombreArchivo);
            var rutaDestino = Path.Combine(Preferencias.BuildOrdersDirectory, nombreArchivo);
            File.Copy(rutaOrigen, rutaDestino, overwrite: true); // Copia la orden de ejecución recién modificada en D:\Programas\RTS Helper\Código\RTS Helper\RTS Helper\Build Orders\[Juego] a la ruta de compilación donde el cambio del archivo es detectado por OrdenDeEjecuciónActual_Changed();

        } // OrdenDeEjecuciónActual_Changed>


        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        } // Window_MouseDown>


        private void BtnStart_Click(object sender, RoutedEventArgs e) {

            switch (Estado) {
                case EEstado.Stoped: // Start.

                    EstablecerRunningUI(EEstado.Stoped);
                    Estado = EEstado.Running;
                    OrdenDeEjecución.NúmeroPaso = 0;
                    ReiniciarPasoActual();
                    SuspenderBlinkingTiempoJuego();
                    BtnNext.IsEnabled = true;
                    BtnBack.IsEnabled = true;
                    BtnRemoveIdleTime.IsEnabled = true;
                    BtnAddIdleTime.IsEnabled = true;
                    TimerDetecciónPausa.Start();
                    TimerDetecciónProgreso.Start();
                    break;

                case EEstado.Running: // Pause.

                    Pause();
                    break;

                case EEstado.Paused: // Restart.

                    Restart();
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

                    Estado = EEstado.Running;
                    ReiniciarVariables();
                    ReiniciarPasoActual();
                    if (Preferencias.MuteOnComplete && SilenciadoAlCompletar) {
                        Preferencias.Muted = false;
                        SilenciadoAlCompletar = false;
                        AplicarPreferenciasMuted(iniciando: false);
                    }    
                    SuspenderBlinkingTiempoJuego();
                    break;

                case EEstado.Paused: // Stop.

                    Estado = EEstado.Stoped;
                    ReiniciarVariables();
                    ActualizarPaso(stop: true);
                    TxtPaso.Text = "";
                    TxtPaso.IsEnabled = false;
                    Timer?.Stop();
                    TimerFlash.Stop();
                    MedidorTimer.Reset();
                    TimerStepEndSound?.Stop();
                    TimerDetecciónPausa.Stop();
                    TimerDetecciónProgreso.Stop();
                    ActualizarUI(forzar: true);  
                    BtnNext.IsEnabled = false;
                    BtnBack.IsEnabled = false;
                    BtnRemoveIdleTime.IsEnabled = false;
                    BtnAddIdleTime.IsEnabled = false;
                    SuspenderBlinkingTiempoJuego();
                    break;

                default:
                    break;
            }

        } // BtnRestart_Click>


        private void MniRestartStep_Click(object sender, RoutedEventArgs e) {

            var estadoActual = Estado;
            ReiniciarPasoActual();
            if (estadoActual == EEstado.Paused) Pause();

        } // MniRestartStep_Click>


        private void MniStartNextStep_Click(object sender, RoutedEventArgs e) {

            var estadoActual = Estado;
            IniciarSiguientePaso();
            if (estadoActual == EEstado.Paused) Pause();

        } // MniStartNextStep_Click>


        public void Flash() {

            TimerFlash.Start();
            if (!(Preferencias.StopFlashingOnComplete && OrdenDeEjecución.EsDespuésDeÚltimoPaso) && ObtenerFlash(OrdenDeEjecución.NúmeroPaso)) {
                Application.Current.Resources["ColorFondo"] = ObtenerMediaColor(ObtenerColorFlash(OrdenDeEjecución.NúmeroPaso)) ?? Color.FromRgb(0, 0, 0);
                Application.Current.Resources["Opacidad"] = ObtenerOpacidadFlash(OrdenDeEjecución.NúmeroPaso);
            }

        } // Flash>


        private void Timer_Tick(object? sender, EventArgs e) {

            MedidorTimer.Reset();
            OrdenDeEjecución.NúmeroPaso++;
            ActualizarPaso();
            MilisegundosTimerAntesDePausa = 0;
            GuardarDuraciónPaso(OrdenDeEjecución.NúmeroPaso - 1);
            GuardarDesfaceAcumulado(OrdenDeEjecución.NúmeroPaso - 1);
            ActualizarUI();
            Flash();
            var nuevaDuraciónPaso = ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed, OrdenDeEjecución.NúmeroPaso);

            if (!Preferencias.Muted) PlaySonidoInicio();
            if (ActualizarDuraciónPasoEnTimerEnPróximoTick) {
                ActualizarIntervaloTimer(nuevaDuraciónPaso);
                ActualizarDuraciónPasoEnTimerEnPróximoTick = false;
            }
            if (Timer?.Interval != nuevaDuraciónPaso) ActualizarIntervaloTimer(nuevaDuraciónPaso); // Necesario para soportar los comportamientos personalizados por paso.

            if (Preferencias.MinimizeOnComplete && OrdenDeEjecución.EsPasoDespuésDeÚltimo) this.WindowState = WindowState.Minimized; // Se hace con EsPasoDespuésDeÚltimo porque ya se aumentó el valor del paso al siguiente.
            if (Preferencias.MuteOnComplete && (OrdenDeEjecución.EsPasoDespuésDeÚltimo || OrdenDeEjecución.EsDespuésDeÚltimoPaso)) { // Se hace después de reproducir el último sonido por consistencia. Se incluye el condicional EsDespuésDeÚltimoPaso para los casos que el usuario se haya saltado los pasos hasta el final manualmente.
                Preferencias.Muted = true;
                SilenciadoAlCompletar = true;
                AplicarPreferenciasMuted(iniciando: false);
            }

            if (!(Timer is null)) ReiniciarTimerStepEndSound(Timer.Interval);
            MedidorTimer.Start(); // Se reinicia siempre para evitar que se descoordine con el Timer. Si se dejaba correr por su propia cuenta, terminaba desfazándose con el Timer y generando problemas en los momentos entre paso y paso. Pasaba de 19 a 10 y después a 21.

        } // Timer_Tick>


        private void TimerStepEndSound_Tick(object? sender, EventArgs e) {

            TimerStepEndSound?.Stop();
            if (!Preferencias.Muted) PlaySonidoFinal();

        } // TimerStepEndSound_Tick>


        private void TxtPaso_TextChanged(object sender, TextChangedEventArgs e) {

            if (int.TryParse(TxtPaso.Text, out int intPaso)) {

                if (OrdenDeEjecución.NúmeroPaso < intPaso) {
                    for (int i = OrdenDeEjecución.NúmeroPaso; i < intPaso; i++) {
                        GuardarDuraciónPaso(i);
                    }
                }

                OrdenDeEjecución.NúmeroPaso = intPaso;
                if (CambiandoTxtPasoAutomáticamente) {
                    return;
                } else if (Estado == EEstado.Running) {
                    ReiniciarPasoActual();
                } else {
                    ActualizarPaso(siguienteOAnterior: true);
                    ActualizarUI(forzar: true);
                }
   
            }

        } // TxtPaso_TextChanged>


        private void TimerFocus_Tick(object? sender, EventArgs e) {
            TimerFocus.Stop();
            TxtPaso.SelectAll();
        } // TimerFocus_Tick>


        private void TimerFlash_Tick(object? sender, EventArgs e) {

            TimerFlash.Stop();
            if (!(Preferencias.StopFlashingOnComplete && OrdenDeEjecución.EsDespuésDeÚltimoPaso) && ObtenerFlash(OrdenDeEjecución.NúmeroPaso)) {
                RestablecerColor();
            }

        } // TimerFlash_Tick>


        private void TimerActualizadorUIPorCambioTamaño_Tick(object? sender, EventArgs e) {

            ActualizarPaso(cambióTamaño: true);
            TimerActualizadorUIPorCambioTamaño.Stop();

        } // TimerActualizadorUIPorCambioTamaño_Tick>


        private void TimerBlinkerGameTime_Tick(object? sender, EventArgs e) {

            if (Estado == EEstado.Paused) {

                if (SpnIndicadoresDeProgreso.Visibility == Visibility.Visible) {
                    SpnIndicadoresDeProgreso.Visibility = Visibility.Hidden;
                } else {
                    SpnIndicadoresDeProgreso.Visibility = Visibility.Visible;
                }

            }

        } // TimerBlinkerGameTime_Tick>


        private void TimerDetecciónProgreso_Tick(object? sender, EventArgs e) { // En mi computador tarda alrededor de 50 ms. La verificación de aldeanos es casi siempre un solo ensayo.

            if (!Preferencias.AutoAdjustIdleTime) return;
            if (!Jugando()) return;
            if (Estado != EEstado.Running) return;
            if (ModoDesarrolloOCR) {
                var progresoLeído2 = LeerProgreso(50, out float confianza2, rangoValoresEsperados: 0); // No se usa rango de valores esperados para no contaminar las pruebas OCR con un dato de progreso actual. Se debe usar un número cualquiera de una cifra, de dos y de tres para probar el funcionamiento de la extracción de texto en cada uno de los segmentos.
                LblDepuración.Content = $"Progreso Leído: {progresoLeído2.ToString()}{Environment.NewLine}Confianza: {confianza2}";
                return; // En este modo se desactiva el ajuste de progreso automático para facilitar realizar los ensayos.
            }

            var pasoActual = OrdenDeEjecución.NúmeroPaso;
            var progresoActual = (int?)null;
            if (pasoActual <= OrdenDeEjecución.Pasos.Count - 1) progresoActual = OrdenDeEjecución.Pasos[pasoActual].Comportamiento?.Progreso;
            var progresoLeído = (int?)null;
            var confianza = -2f;
            if (progresoActual != null) progresoLeído = LeerProgreso((int)progresoActual, out confianza); 
            if (progresoLeído == ÚltimoProgresoLeído) return; // Cuando el progresoLeído es igual al ÚltimoProgresoLeído, no se realiza ninguna acción. 
            var segundosJuegoPasoActual = ObtenerSegundosJuegoPasoActual();
            
            if (progresoLeído != null) {

                var confiable = confianza > 1 || (confianza > 0 && (progresoLeído == ÚltimoProgresoLeído + 1 || progresoLeído == ÚltimoProgresoLeído - 1));  // Cuando la confianza está entre 0 y 1 es una lectura dudosa que no está en el rango esperado y requiere comprobación con el ÚltimoProgresoLeído para verificar que el último progreso leído era un progreso inmediatamente anterior al progreso actual y confirmar así que la lectura actual es confiable. Esto sucede por ejemplo en el caso de tener 15 aldeanos y en el mismo segundo perder 3 aldeanos (improbable, pero podría suceder), la siguiente lectura serían 11 aldeanos que sería descartada por no estar en el rango esperado 13-14-15-16-17, el RTS Helper pasaría al paso con progreso 16 y la siguente lectura sería 12 que tampoco estaría en el rango 14-15-16-17-18, pero si sería un consecutivo desde el último progreso leído 11, entonces se considerará que es confiable. Se agrega también el consecutivo hacia atrás para considerar el caso de 3 aldeanos muertos seguidos por uno más muerto.      
                if (confianza > 0) {
                    ÚltimoProgresoLeído = progresoLeído;
                } else {
                    ÚltimoProgresoLeído = null;
                }

                if (confiable) {
  
                    var desface = 0D;
                    var direcciónBúsqueda = progresoLeído == progresoActual ? 0 : (progresoLeído > progresoActual ? 1 : -1); // Si se está más adelante, se busca en los pasos posteriores y el desface es negativo. Cuando el progresoLeído es igual al progresoActual, no se necesita revisar otros pasos para encontrar el valor de duraciónDesface.
                    var encontradoPaso = direcciónBúsqueda == 0; 

                    if (!encontradoPaso) { 

                        var paso = pasoActual + direcciónBúsqueda;
                        while (paso >= 0 && paso <= OrdenDeEjecución.Pasos.Count - 1) {
          
                            if (progresoLeído == OrdenDeEjecución.Pasos[paso].Comportamiento?.Progreso) {
                                if (direcciónBúsqueda == -1) desface += -direcciónBúsqueda * ObtenerDuraciónPaso(paso); // Si la búsqueda es hacia atrás, tiene en cuenta el paso final. Por ejemplo, si se está en el inicio del paso de 7 aldeanos y se mueren 3 aldeanos, se debe sumar el largo del paso 6, 5 y 4 (que es el paso en el que se cumple la condición progresoLeído == OrdenDeEjecución.Pasos[paso].Comportamiento?.Progreso). Si la búsqueda es hacia adelante, no se suma el paso en el que se cumple la condición. Por ejemplo, si se tienen 4 aldeanos y se está en el final del paso y se convierten 3 en el mismo momento, se debe sumar la duración del paso de 5 aldeanos y de 6 aldeanos. El de 7 no se suma porque queda al inicio de este. Los ajustes de pasos incompletos para ambos casos se hacen en el condicional siguiente if (encontradoPaso).
                                encontradoPaso = true;
                                break;
                            }
                            desface += -direcciónBúsqueda * ObtenerDuraciónPaso(paso);
                            paso += direcciónBúsqueda;

                        }

                    }

                    if (encontradoPaso) { // Si no se pudo encontrar un paso con el progreso leído, no realiza ninguna acción.
              
                        if (direcciónBúsqueda <= 0) { // Si la dirección de búsqueda es -1 o 0, el desface es positivo y se debe retrasar/delay el RTS Helper una cantidad positiva igual al tiempo en el paso actual.
                            desface += segundosJuegoPasoActual;
                        } else { // Si la duración de búsqueda es 1, el desface es negativo y se debe avanzar/rush el RTS Helper para ponerlo al día con el juego. Esto es causado por ejemplo, por una conversión de un aldeano por un monje.
                            desface -= (ObtenerDuraciónPaso(pasoActual) - segundosJuegoPasoActual);
                        }

                        if (desface > Preferencias.MinimumDelayToAutoAdjustIdleTime || desface < -Preferencias.MinimumDelayToAutoAdjustIdleTime) {

                            LblDepuración.Content = $"Desface: {desface:##.0} s";
                            Desfazar(desface * 1000, desfazarReloj: false);

                        } else {
                            LblDepuración.Content = $"No Desface";
                        }

                    } else {
                        LblDepuración.Content = $"No Encontrado Paso";
                    }

                } else {

                    if (confianza < 0) {
                        LblDepuración.Content = $"No Confiable";
                    } else {
                        LblDepuración.Content = $"No Consecutivo";
                    }
                    
                }

                LblDepuración.Content += $"{Environment.NewLine}Progreso Leído: {(confianza < 0 ? "x" : progresoLeído.ToString())}{Environment.NewLine}Progreso: {progresoActual}";

            } else {
                ÚltimoProgresoLeído = null;
                LblDepuración.Content = $"No Leído{Environment.NewLine}Progreso Leído: {(confianza < 0 ? "x" : progresoLeído.ToString())}{Environment.NewLine}Progreso: {progresoActual}";
            }
    
        } // TimerDetecciónProgreso_Tick>


        private void TimerDetecciónPausa_Tick(object? sender, EventArgs e) { // En mi computador tarda alrededor de 50 ms cuando está en juego  y alrededor de 70 ms cuando está en pausa.

            if (!Preferencias.PauseDetection) return;
            if (!Jugando()) return;
            if (ModoDesarrolloOCR) LblDepuración.Visibility = Visibility.Visible;

            var juegoPausado = false;
            var textoPausa = LeerPausa(out float confianzaPausa);
            if (!string.IsNullOrEmpty(textoPausa)) {

                if (TextosPausa[Preferencias.Game].Exists(tp => textoPausa.ToLower().Contains(tp.ToLower()))) {
                    juegoPausado = true;
                } else {

                    foreach (var tPausa in TextosPausa[Preferencias.Game]) {

                        var distanciaAPausa = ObtenerDistanciaLevenshtein(textoPausa.ToLower(), $"{tPausa} (F3)"); // Se usa F3 como la tecla por defecto.
                        if (distanciaAPausa < 5) {
                            juegoPausado = true;
                            break;
                        }

                    }

                }

                if (!juegoPausado && textoPausa.Contains("(F3)")) juegoPausado = true; // Este es el último intento principalmente útil para los idiomas que no reconoce Tesseract. Aunque se podría argumentar que es preferible hacer esta verificación antes que todas, se prefiere hacer de la manera actual (coincidiendo con el texto de pausa) para evitar falsos positivos.

            } else {
                juegoPausado = false;
            }
  
            if (juegoPausado && Estado == EEstado.Running) Pause(); // Aunque no siempre se pausa en el tiempo exacto, el reinicio tampoco es en el tiempo exacto entonces ambos efectos se tienden a cancelar. No se necesita realizar ningún ajuste adicional.
            if (!juegoPausado && Estado == EEstado.Paused) Restart();
 
        } // TimerDetecciónPausa_Tick>


        private void TimerActualizadorUI_Tick(object? sender, EventArgs e) => ActualizarUI();


        private void CmbBuildOrders_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (!Inició || EditandoComboBoxEnCódigo) return;
            Preferencias.CurrentBuildOrder = ObtenerSeleccionadoEnCombobox(e);
            CargarBuildOrder();
            ActualizarSupervisoresOrdenDeEjecución();

        } // CmbBuildOrders_SelectionChanged>


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
            if (EstablecióTamañoInicial && !TimerActualizadorUIPorCambioTamaño.IsEnabled) TimerActualizadorUIPorCambioTamaño.Start();
            EstablecióTamañoInicial = true;

        } // Window_SizeChanged>


        private void BtnMinize_Click(object sender, RoutedEventArgs e)
            => this.WindowState = WindowState.Minimized;


        private void BtnMute_Click(object sender, RoutedEventArgs e) {

            Preferencias.Muted = !Preferencias.Muted;
            AplicarPreferencias();

        } // BtnMute_Click>


        private void MniLastStep_Click(object sender, RoutedEventArgs e) {

            var pasoInicial = OrdenDeEjecución.NúmeroPaso;
            GuardarDuraciónPaso(pasoInicial);
            OrdenDeEjecución.NúmeroPaso = OrdenDeEjecución.Pasos.Count - 1;
            for (int i = pasoInicial; i < OrdenDeEjecución.NúmeroPaso; i++) {
                OrdenDeEjecución.Pasos[i].DuraciónEnJuego = OrdenDeEjecución.Pasos[pasoInicial].DuraciónEnJuego;
            }

            ActualizarPaso(siguienteOAnterior: true);
            ActualizarUI(forzar:true);

        } // MniLastStep_Click>


        private void MniReloadBuildOrder_Click(object sender, RoutedEventArgs e) {

            if (!Inició || EditandoComboBoxEnCódigo) return;
            CargarBuildOrder();

        } // MniReloadBuildOrder_Click>


        private void BtnAlert_Click(object sender, RoutedEventArgs e) =>
            MostrarInformación((string)Application.Current.Resources["AlertContentMoreHeightThanWindow"]);


        private void BtnAlternarVistaPasoSiguienteAnterior_Click(object sender, RoutedEventArgs e) 
            => ActualizarVistaPasoSiguienteAnterior(!MostrandoPasoAnterior, actualizarPaso: true);

        private void MniBackward_Click(object sender, RoutedEventArgs e) => Backward();
        
        private void MniFordward_Click(object sender, RoutedEventArgs e) => Fordward();

        private void TimerVerificadorVentanaEsVisible_Tick(object? sender, EventArgs e) => VerificarSiVentanaEsVisible();

        private void BtnAddIdleTime_Click(object sender, RoutedEventArgs e) => Delay();

        private void MniAddIdleTime_Click(object sender, RoutedEventArgs e) => Delay();

        private void BtnRemoveIdleTime_Click(object sender, RoutedEventArgs e) => Rush();

        private void MniRemoveIdleTime_Click(object sender, RoutedEventArgs e) => Rush();


        private void MniResetIdleTime_Click(object sender, RoutedEventArgs e) {

            foreach (var paso in OrdenDeEjecución.Pasos) {
                paso.DesfaceAcumulado = null;
            }
            Desfazar(-MilisegundosJuegoDesface, desfazarReloj: false);

        } // MniResetIdleTime_Click>


        private void BtnNext_Click(object sender, RoutedEventArgs e) => Next(proporcional: true);

        private void BtnBack_Click(object sender, RoutedEventArgs e) => Back(proporcional: true);


        private void MniBackMultipleSteps_Click(object sender, RoutedEventArgs e) {

            for (int i = 0; i < Preferencias.BackMultipleSteps; i++) {
                Back(proporcional: true);
            }

        } // MniBackMultipleSteps_Click>


        private void MniNextMultipleSteps_Click(object sender, RoutedEventArgs e) {

            for (int i = 0; i < Preferencias.NextMultipleSteps; i++) {
                Next(proporcional: true);
            }

        } // MniNextMultipleSteps_Click>


        private void BtnStats_Click(object sender, RoutedEventArgs e) {

            var últimoDesfaceAcumulado = 0D;
            var textoEstadísticas = "";
            var cuentaPaso = 0;
            var desfaces = new List<(int, double)>();

            foreach (var paso in OrdenDeEjecución.Pasos) {

                if (paso.DesfaceAcumulado != null && paso.DesfaceAcumulado != últimoDesfaceAcumulado) {
                    desfaces.Add((cuentaPaso - 1, (double)paso.DesfaceAcumulado - últimoDesfaceAcumulado));
                    últimoDesfaceAcumulado = (double)paso.DesfaceAcumulado;
                }
                cuentaPaso++;

            }

            var desfacesOrdenados = desfaces.OrderByDescending(t => t.Item2).ToList();
            textoEstadísticas = $"Total Idle Time: {últimoDesfaceAcumulado:##0} s{Environment.NewLine}{Environment.NewLine}";
            foreach (var kv in desfacesOrdenados) {
                textoEstadísticas += $"{kv.Item2:#00} s - Step {kv.Item1}{Environment.NewLine}";
            }
            MessageBox.Show(textoEstadísticas, "Stats");

        } // BtnStats_Click>


        #endregion Eventos>



        #region Procedimientos y Funciones


        private void ActualizarSupervisoresOrdenDeEjecución() {

            ActualizarSupervisorOrdenDeEjecución();
            if (ModoDesarrollo) ActualizarSupervisorOrdenDeEjecuciónEnCódigo();

        } // ActualizarSupervisoresOrdenDeEjecución>


        private void ActualizarSupervisorOrdenDeEjecución() {

            if (SupervisorOrdenDeEjecuciónActual == null) return;
            SupervisorOrdenDeEjecuciónActual.Path = Preferencias.BuildOrdersDirectory;
            SupervisorOrdenDeEjecuciónActual.Filter = $"{Preferencias.CurrentBuildOrder}.txt";

        } // ActualizarSupervisorOrdenDeEjecución>


        private void ActualizarSupervisorOrdenDeEjecuciónEnCódigo() {

            if (SupervisorOrdenDeEjecuciónActualEnCódigo == null) return;
            SupervisorOrdenDeEjecuciónActualEnCódigo.Path 
                = Settings.ObtenerDirectorioÓrdenesDeEjecución(Global.DirectorioÓrdenesDeEjecuciónCódigo, Preferencias.Game);
            SupervisorOrdenDeEjecuciónActualEnCódigo.Filter = $"{Preferencias.CurrentBuildOrder}.txt";

        } // ActualizarSupervisorOrdenDeEjecuciónEnCódigo>


        private void RestablecerColor() {

            Application.Current.Resources["ColorFondo"] = ObtenerMediaColor(Preferencias.BackColor) ?? Color.FromRgb(0, 0, 0);
            Application.Current.Resources["Opacidad"] = Preferencias.Opacity;

        } // RestablecerColor>


        private void Next(bool proporcional) {

            OrdenDeEjecución.NúmeroPaso++;
            GuardarDuraciónPaso(OrdenDeEjecución.NúmeroPaso - 1);
            if (proporcional) 
                AjustarProgresoPasoProporcional(OrdenDeEjecución.NúmeroPaso - 1, OrdenDeEjecución.NúmeroPaso, ObtenerMilisegundosTimerPasoActual());
            ActualizarPaso(siguienteOAnterior: true);
            ActualizarUI(forzar: true);

        } // SiguientePaso>


        private void Back(bool proporcional) {

            OrdenDeEjecución.NúmeroPaso--;
            if (OrdenDeEjecución.NúmeroPaso == -1 && Estado == EEstado.Running) {
                OrdenDeEjecución.NúmeroPaso = 0;
                ReiniciarPasoActual();
            } else {

                if (proporcional) 
                    AjustarProgresoPasoProporcional(OrdenDeEjecución.NúmeroPaso + 1, OrdenDeEjecución.NúmeroPaso, ObtenerMilisegundosTimerPasoActual());
                ActualizarPaso(siguienteOAnterior: true);
                ActualizarUI(forzar: true);

            }

        } // AnteriorPaso>


        private void AjustarProgresoPasoProporcional(int númeroPasoAnterior, int númeroPasoActual, double msTimerAnterior) {

            var estado = Estado;
            var msTotalTimerPasoAnterior = ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed, númeroPasoAnterior);
            var msTotalTimerPasoActual = ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed, númeroPasoActual);
            if (msTotalTimerPasoAnterior == msTotalTimerPasoActual) return; // Si son de igual duración, no hay que hacer ningún ajuste porque el temporizador mantiene su ubicación correcta al cambiar de paso.
            
            var fracciónPasoAnterior = msTimerAnterior / msTotalTimerPasoAnterior.TotalMilliseconds;
            var msTimerPasoActual = (int)Math.Round(fracciónPasoAnterior * msTotalTimerPasoActual.TotalMilliseconds, 0);
            ReiniciarPasoActual();
            Desfazar(-msTimerPasoActual * ObtenerFactorTimerAJuego(), desfazarReloj: true);
            if (estado == EEstado.Paused) Pause();

        } // AjustarProgresoPasoProporcional>

        
        private void Delay() => Desfazar(Preferencias.AddIdleTimeSeconds * 1000, desfazarReloj: false);


        private void Rush() => Desfazar(-Preferencias.RemoveIdleTimeSeconds * 1000, desfazarReloj: false);


        private void Fordward() => Desfazar(-Preferencias.ForwardSeconds * 1000, desfazarReloj: true);


        private void Backward() => Desfazar(Preferencias.BackwardSeconds * 1000, desfazarReloj: true);


        private void Desfazar(double msJuegoDesface, bool desfazarReloj) {

            if (Math.Abs(msJuegoDesface) < 250) return; // En ocasiones se puede generar error de Stack Overflow cuando el desface es de un valor pequeño. Parece que tiene algo que ver con los tiempos de ejecución de este procedimiento y el temporizador, pero no se tiene muy claro. Se espera que limitando el desface a valores mayores e ignorando realizar desfaces con valores muy pequeños que son imperceptibles para el usuario, este error no suceda más.
            double nuevosMsTimer = 0;
            var estadoInicial = Estado;
            var fMsTimerAJuego = ObtenerFactorTimerAJuego();
            var msTimerDesface = msJuegoDesface * ObtenerFactorJuegoATimer();
            var timerTotalMs = Timer?.Interval.TotalMilliseconds ?? 0;
            var medidorTimerMs = MedidorTimer.ElapsedMilliseconds;

            if (Timer != null) {

                nuevosMsTimer = timerTotalMs - medidorTimerMs + msTimerDesface;
                if (nuevosMsTimer < 0) { // Para que no se sobrepase el paso actual y saque error al intentar poner el timer con tiempo negativo.

                    Desfazar(-fMsTimerAJuego * (timerTotalMs - (medidorTimerMs + 100)), desfazarReloj); // Avanza el paso hasta casi el final.
                    IniciarSiguientePaso();
                    Desfazar(fMsTimerAJuego * nuevosMsTimer, desfazarReloj); // Avanza el resto en el paso siguiente.
                    if (estadoInicial == EEstado.Paused) Pause();
                    return;

                }

            }

            var milisegundosTimerPasoActual = ObtenerMilisegundosTimerPasoActual();
            if (milisegundosTimerPasoActual - msTimerDesface < 0) { // Al desfazar está intentando ir más atrás del paso actual.

                var pasoActual = OrdenDeEjecución.NúmeroPaso;
                if (!desfazarReloj) MilisegundosJuegoDesface += msJuegoDesface; // Se desfaza antes de aplicar los cambios para no enredar con desfaces en los Desfazar() internos.

                Desfazar(fMsTimerAJuego * (milisegundosTimerPasoActual - 100), desfazarReloj: true);
                IniciarAnteriorPaso();
                if (pasoActual == 0) {

                    if (estadoInicial == EEstado.Paused) Pause();
                    return;

                } else {

                    var duraciónAnteriorPaso = ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed, pasoActual - 1);
                    Desfazar(-fMsTimerAJuego * (duraciónAnteriorPaso.TotalMilliseconds - (msTimerDesface - milisegundosTimerPasoActual)), 
                        desfazarReloj: true);
                    if (estadoInicial == EEstado.Paused) Pause();
                    return;

                }

            }
       
            if (!desfazarReloj) MilisegundosJuegoDesface += msJuegoDesface;

            if (Estado == EEstado.Paused) {

                MilisegundosTimerAntesDePausa -= msTimerDesface;
                Restart(msTimerDesface);
                Pause();
                ActualizarUI(forzar: true);

            } else if (Estado == EEstado.Running) {

                Pause();
                MilisegundosTimerAntesDePausa -= msTimerDesface;
                Restart(msTimerDesface);

            }

        } // Desfazar>


        private void EstablecerRunningUI(EEstado estadoAnterior) {

            if (estadoAnterior == EEstado.Stoped) {
                LblTiempoEnJuego.Content = "0:00"; // Para evitar un pequeño retraso en la actualización.
                CpgProgresoPaso.Value = 0;
            }
            BtnStart.Content = "❚❚";
            BtnStart.ToolTip = "Pause";
            BtnRestart.ToolTip = "Restart";
            BtnRestart.Content = "⟳";
            TxtPaso.IsEnabled = true;

        } // EstablecerRunningUI>


        private void Restart(double msTimerDesface = 0) {

            EstablecerRunningUI(EEstado.Paused);
            Estado = EEstado.Running;
            if (Timer != null) {

                MedidorTimer.Start();
                ActualizarDuraciónPasoEnTimerEnPróximoTick = true;
                MilisegundosTimerAntesDePausa += MedidorTimer.ElapsedMilliseconds;

                ActualizarIntervaloTimer(new TimeSpan(0, 0, 0, 0, (int)Math.Round(Timer.Interval.TotalMilliseconds - MedidorTimer.ElapsedMilliseconds 
                    + msTimerDesface, 0)));
                ActualizarUI();

            }
            SuspenderBlinkingTiempoJuego();

        } // Restart>


        private void Pause() {

            Estado = EEstado.Paused;
            BtnStart.Content = "▷";
            BtnStart.ToolTip = "Restart";
            BtnRestart.ToolTip = "Stop";
            BtnRestart.Content = "■";         
            MedidorTimer.Stop(); // Lo suspende, pero mantiene el valor de la duración actual.
            Timer?.Stop(); // Lo suspende completamente.
            TimerFlash.Stop(); // Lo suspende completamente.
            TimerStepEndSound?.Stop(); // Lo suspende completamente.     
            TimerBlinkerGameTime.Start();
            RestablecerColor(); // Para que no se quede el color de flashing activo durante la pausa.

        } // Pause>


        public void EnCambioNúmeroPaso() {

            ActualizarVistaPasoSiguienteAnterior(nuevoMostrandoPasoAnterior: false);
            if (!Preferencias.ShowAlwaysStatsButton) {

                if (OrdenDeEjecución.NúmeroPaso >= OrdenDeEjecución.Pasos.Count - 1) {
                    BtnStats.Visibility = Visibility.Visible;
                } else {
                    BtnStats.Visibility = Visibility.Collapsed;
                }

            }

        } // EnCambioNúmeroPaso>


        public void ReiniciarVariables() {

            MilisegundosJuegoDesface = 0;
            DuraciónPasoParcialPorCambioDuración = 0;
            LblTiempoEnJuego.Content = "0:00"; // Para evitar un pequeño retraso en la actualización.
            CpgProgresoPaso.Value = 0;
            OrdenDeEjecución.NúmeroPaso = 0;
            MilisegundosTimerAntesDePausa = 0;
            foreach (var paso in OrdenDeEjecución.Pasos) {
                paso.DesfaceAcumulado = 0;
            }

        } // ReiniciarVariables>


        public void ActualizarVistaPasoSiguienteAnterior(bool nuevoMostrandoPasoAnterior, bool actualizarPaso = false) {

            if (!nuevoMostrandoPasoAnterior) {
                BtnAlternarPasoSiguienteAnterior.Content = "⮨";
                BtnAlternarPasoSiguienteAnterior.ToolTip = "See Previous Step";
            } else {
                BtnAlternarPasoSiguienteAnterior.Content = "⮩";
                BtnAlternarPasoSiguienteAnterior.ToolTip = "See Next Step";
            }
            MostrandoPasoAnterior = nuevoMostrandoPasoAnterior;
            if (actualizarPaso) ActualizarPaso(alternandoSiguienteAnteriorPaso: true);

        } // ActualizarVistaPasoSiguienteAnterior>


        public void LeerBuildOrders() {

            EditandoComboBoxEnCódigo = true;
            CmbBuildOrders.Items.Clear();
            CmbBuildOrders.Items.Add("Default");
            CmbBuildOrders.SelectedIndex = 0;
            EditandoComboBoxEnCódigo = false;

            var archivosBuildOrders = Directory.GetFiles(Preferencias.BuildOrdersDirectory, "*.txt");
            foreach (var archivoBuildOrder in archivosBuildOrders) {
                var nombreBuildOrder = Path.GetFileNameWithoutExtension(archivoBuildOrder);
                if (nombreBuildOrder.ToLower() != "default") CmbBuildOrders.Items.Add(nombreBuildOrder);
            }

        } // LeerBuildOrders>


        private void ActualizarUI(bool forzar = false) {

            var actualizar = forzar || (!(Timer is null || !Timer.IsEnabled) && MedidorTimer.IsRunning); // Cuando esté en pausa no debe actualizar ni borrar el temporizador.      
            if (!actualizar) return;

            var segundosPasosAnteriores = Paso.ObtenerDuraciónPasosAnteriores(OrdenDeEjecución.Pasos, OrdenDeEjecución.NúmeroPaso);
            var segundosPasoActual = ObtenerSegundosJuegoPasoActual();
            var segundosJuego = segundosPasosAnteriores + segundosPasoActual; // Step duration es en segundos de juego, en cambio el temporizador es en segundos reales, por eso solo se ajusta este último valor a los segundos en el juego que son los que finalmente se muestran.
            var segundos = segundosJuego % 60;
            if (segundos < ÚltimosSegundosJuego && !forzar) { // En algunas ocasiones en el límite del entre dos pasos el MedidorTimer se adelanta al Tick del Timer y por lo tanto sucede que por ejemplo tenga 60.04 segundos (dando 0.04 en el nuevo paso) mientras que el Timer aún no ha hecho el evento Tick y por lo tanto aún no se ha aumentado el paso. Esto produce error en la presentación porque pasa de 29 a 15 y después a 31 rápidamente. Para evitarlo sin complejizar mucho el código simplemente no se actualizará la UI en estos casos.
                //var msmedidortimer = MedidorTimer.Elapsed.TotalSeconds;
                //var mstimer = Timer.Interval.TotalSeconds;
            } else {

                CpgProgresoPaso.Value 
                    = (segundosPasoActual / (ObtenerDuraciónPaso(OrdenDeEjecución.NúmeroPaso) / Preferencias.ExecutionSpeed)) * 100;
                var segundosJuegoAMostrar = segundosJuego + MilisegundosJuegoDesface / 1000;
                var segundosAMostrar = segundosJuegoAMostrar % 60;
                LblTiempoEnJuego.Content = Math.Floor(segundosJuegoAMostrar / 60).ToString() + ":" 
                    + (segundosAMostrar > 59 ? 59 : Math.Round(segundosAMostrar)).ToString("00");

            }
            ÚltimosSegundosJuego = segundos;

        } // ActualizarUI>


        private double ObtenerSegundosJuegoPasoActual() => (ObtenerMilisegundosTimerPasoActual()) * ObtenerFactorTimerAJuego() / 1000;


        private void GuardarDuraciónPaso(int númeroPaso) {
            if (númeroPaso <= OrdenDeEjecución.Pasos.Count - 1) 
                OrdenDeEjecución.Pasos[númeroPaso].DuraciónEnJuego = ObtenerDuraciónPaso(númeroPaso) / Preferencias.ExecutionSpeed;
        } // GuardarDuraciónPaso>


        private void GuardarDesfaceAcumulado(int númeroPaso) {
            if (númeroPaso <= OrdenDeEjecución.Pasos.Count - 1)
                OrdenDeEjecución.Pasos[númeroPaso].DesfaceAcumulado = MilisegundosJuegoDesface / 1000;
        } // GuardarDesfaceAcumulado>


        private T ObtenerPropiedadDePaso<T>(int númeroPaso, Func<Comportamiento, T?> propiedad, T valorPreferencias) where T : struct {

            if (númeroPaso >= 0 && númeroPaso <= OrdenDeEjecución.Pasos.Count - 1) {
                return propiedad(OrdenDeEjecución.Pasos[númeroPaso].Comportamiento) ?? valorPreferencias;
            } else {
                return valorPreferencias;
            }

        } // ObtenerPropiedadDePaso>


        private T ObtenerPropiedadDePasoClase<T>(int númeroPaso, Func<Comportamiento, T?> propiedad, T valorPreferencias) where T : class {

            if (númeroPaso >= 0 && númeroPaso <= OrdenDeEjecución.Pasos.Count - 1) {
                return propiedad(OrdenDeEjecución.Pasos[númeroPaso].Comportamiento) ?? valorPreferencias;
            } else {
                return valorPreferencias;
            }

        } // ObtenerPropiedadDePaso>


        private double ObtenerDuraciónPaso(int númeroPaso) 
            => ObtenerPropiedadDePaso(númeroPaso, c => c.Duración, Preferencias.StepDuration);


        private bool ObtenerMostrarSiguientePaso(int númeroPaso) 
            => ObtenerPropiedadDePaso(númeroPaso, c => c.MostrarSiguientePaso, Preferencias.ShowNextStep);


        private string ObtenerSonido(int númeroPaso) 
            => ObtenerPropiedadDePasoClase(númeroPaso, c => c.Sonido, Preferencias.StepStartSound);


        private string ObtenerPresonido(int númeroPaso) 
            => ObtenerPropiedadDePasoClase(númeroPaso, c => c.Presonido, Preferencias.StepEndSound);


        private int ObtenerVolumenSonido(int númeroPaso) 
            => ObtenerPropiedadDePaso(númeroPaso, c => c.VolumenSonido, Preferencias.StepStartSoundVolume);


        private int ObtenerVolumenPresonido(int númeroPaso) 
            => ObtenerPropiedadDePaso(númeroPaso, c => c.VolumenPresonido, Preferencias.StepEndSoundVolume);


        private int ObtenerDuraciónPresonido(int númeroPaso)
            => ObtenerPropiedadDePaso(númeroPaso, c => c.DuraciónPresonido, Preferencias.StepEndSoundDuration);


        private bool ObtenerFlash(int númeroPaso)
            => ObtenerPropiedadDePaso(númeroPaso, c => c.Flash, Preferencias.FlashOnStepChange);


        private string ObtenerColorFlash(int númeroPaso)
            => ObtenerPropiedadDePasoClase(númeroPaso, c => c.ColorFlash, Preferencias.FlashingColor);


        private double ObtenerOpacidadFlash(int númeroPaso)
            => ObtenerPropiedadDePaso(númeroPaso, c => c.OpacidadFlash, Preferencias.FlashingOpacity);


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
            Application.Current.Resources["ColorFondo"] = ObtenerMediaColor(Preferencias.BackColor) ?? Color.FromRgb(0, 0, 0);
            Application.Current.Resources["BrushPaso"] = (SolidColorBrush)new BrushConverter().ConvertFrom(Preferencias.CurrentStepFontColor);
            Application.Current.Resources["BrushPasoSiguiente"] = (SolidColorBrush)new BrushConverter().ConvertFrom(Preferencias.NextStepFontColor);
            Application.Current.Resources["Opacidad"] = Preferencias.Opacity;
            Application.Current.Resources["MargenPaso"] = new Thickness(Preferencias.LeftMarginCurrentStep, Preferencias.TopMarginCurrentStep, 0, 0);
            Application.Current.Resources["MargenPasoSiguiente"] = new Thickness(0, Preferencias.TopMarginNextStep, Preferencias.RightMarginNextStep, 0);
            Application.Current.Resources["Alto"] = Preferencias.Height;
            Application.Current.Resources["Ancho"] = Preferencias.Width;
            Application.Current.Resources["PosiciónY"] = Preferencias.Top;
            Application.Current.Resources["PosiciónX"] = Preferencias.Left;
            Application.Current.Resources["VisibilidadPasoSiguiente"] = ObtenerMostrarSiguientePaso(OrdenDeEjecución.NúmeroPaso) 
                ? Visibility.Visible : Visibility.Collapsed;
            Application.Current.Resources["AnchoSelectorBuildOrder"] = Preferencias.BuildOrderSelectorWidth;
            Application.Current.Resources["AnchoSelectorVelocidadEjecución"] = Preferencias.ExecutionSpeedSelectorWidth;
            Application.Current.Resources["BrushFlashingColor"] = (SolidColorBrush)new BrushConverter().ConvertFrom(ObtenerColorFlash(-1)); // Solo se establece para que sea efectivo en la ventana de preferencias.
            Application.Current.Resources["BrushImageBackgroundColor"] 
                = (SolidColorBrush)new BrushConverter().ConvertFrom(Preferencias.ImageBackgroundColor);
            Application.Current.Resources["GrosorBarraProgresoCircularDelPaso"] = Preferencias.ThicknessCircularProgressBar;
            Application.Current.Resources["MargenBarraProgresoCircularDelPaso"] = new Thickness(0, 0, Preferencias.RightMarginCircularProgressBar, 0);
            Application.Current.Resources["VisibilidadProgresoPaso"] = Preferencias.ShowStepProgress ? Visibility.Visible : Visibility.Collapsed;
            Application.Current.Resources["VisibilidadTiempoEnJuego"] = Preferencias.ShowTime ? Visibility.Visible : Visibility.Collapsed;
            Application.Current.Resources["VisibilidadBotónRemoveIdleTime"] = Preferencias.ShowRemoveIdleTimeButton ? Visibility.Visible : Visibility.Collapsed;
            Application.Current.Resources["VisibilidadBotónAddIdleTime"] = Preferencias.ShowAddIdleTimeButton ? Visibility.Visible : Visibility.Collapsed;
            TimerDetecciónPausa.Interval = new TimeSpan(0, 0, Preferencias.PauseDetectionInterval);
            TimerDetecciónProgreso.Interval = new TimeSpan(0, 0, Preferencias.AutoAdjustIdleTimeInterval);
            Application.Current.Resources["VisibilidadBotónStats"] = Preferencias.ShowAlwaysStatsButton ? Visibility.Visible : Visibility.Collapsed;
            Application.Current.Resources["VisibilidadBotónVerPasoAnterior"] 
                = Preferencias.ShowPreviousStepButton && Preferencias.ShowNextStep ? Visibility.Visible : Visibility.Collapsed;

            AplicarPreferenciasMuted(iniciando);

            this.Width = Preferencias.Width; // Se deben establecer manualmente porque no funciona el DynamicResource.
            this.Left = Preferencias.Left;
            this.Height = Preferencias.Height;
            this.Top = Preferencias.Top;
            MniBackMultipleSteps.Header = $"|◁      Go Back {Preferencias.BackMultipleSteps} Steps";
            MniNextMultipleSteps.Header = $"▷|      Advance {Preferencias.NextMultipleSteps} Steps";
            MniBackward.Header = $"◁◁    Backward {Preferencias.BackwardSeconds} Seconds";
            MniFordward.Header = $"▷▷    Fordward {Preferencias.ForwardSeconds} Seconds";

            EstableciendoTamaño = false;
            ActualizarSupervisoresOrdenDeEjecución();

            if (!iniciando) ActualizarPaso(stop: false, aplicandoPreferencias: true);

        } // AplicarPreferencias>
     
        
        public void AplicarPreferenciasMuted(bool iniciando) {

            if (iniciando && Preferencias.UnmuteAtStartup) Preferencias.Muted = false;
            BtnMute.Content = !Preferencias.Muted ? "🔈" : "🔇";
            BtnMute.ToolTip = !Preferencias.Muted ? "Mute" : "Unmute";
            BtnMute.Visibility = Visibility.Visible; // Se prefiere siempre dejarlo visible para poder silenciar sonidos en el markup de comportamiento sin estar ocultando y mostrando este botón según el paso actual tenga o no sonidos.
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
            if (Preferencias.StepEndSoundDuration == 0) Preferencias.StepEndSoundDuration = ObtenerDuraciónEndStepSound(ObtenerPresonido(-1));

            AplicarPreferencias(iniciando: true);
            CrearOCompletarScreenCaptureRectangles(); // Se debe hacer siempre después de finalizar la lectura de preferencias para agregar los nuevos rectángulos generales (los que no tiene el usuario).

        } // LeerPreferencias>


        private void IniciarSiguientePaso() {

            Next(proporcional: false);
            ReiniciarPasoActual();

        } // IniciarSiguientePaso>


        private void IniciarAnteriorPaso() {

            Back(proporcional: false);
            ReiniciarPasoActual();

        } // IniciarAnteriorPaso>


        private void ReiniciarPasoActual() {

            if (!(Timer is null)) Timer.Stop();
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(Timer_Tick);
            MilisegundosTimerAntesDePausa = 0;

            if (!(TimerStepEndSound is null)) TimerStepEndSound.Stop();
            TimerStepEndSound = new DispatcherTimer();
            TimerStepEndSound.Tick += new EventHandler(TimerStepEndSound_Tick);
      
            ActualizarIntervaloTimer(ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed, OrdenDeEjecución.NúmeroPaso));
            ActualizarPaso();
            if (Estado == EEstado.Paused) ActualizarUI(forzar: true);

        } // ReiniciarPasoActual>


        private double ObtenerVelocidadJuegoEfectiva(double velocidadJuego) => velocidadJuego / (velocidadJuego == 1.7 ? 1.02 : 1); // En realidad la velocidad 1.7 de AOE2 corresponde aproximadamente a 36 s reales. Lo cual es 2% más lento de lo esperado (60/1.7 = 35.29 s).


        private double ObtenerFactorTimerAJuego() => ObtenerVelocidadJuegoEfectiva(Preferencias.GameSpeed);


        private double ObtenerFactorJuegoATimer() => 1 / ObtenerFactorTimerAJuego();


        private TimeSpan ObtenerDuraciónPaso(double velocidadJuego, double velocidadEjecución, int númeroPaso) => new TimeSpan(0, 0, 0, 0,
            (int)Math.Round(ObtenerDuraciónPaso(númeroPaso) * 1000 / (ObtenerVelocidadJuegoEfectiva(velocidadJuego) * velocidadEjecución), 0)); 


        private void ActualizarIntervaloTimer(TimeSpan duración) {

            if (duración.TotalSeconds < 0) {
                if (ModoDesarrollo) MessageBox.Show("Error. Revisar caso. duración.TotalSeconds < 0 && ModoDesarrollo @ ActualizarIntervaloTimer().");  // Puede pasar en modo desarrollo. En modo de producción nunca debería pasar y si sucede debe ser un error.
                return;
            }

            if (Timer is null) return; // No debería pasar.
            Timer.Interval = duración;
            Timer.Start(); // Se reinicia aquí porque de todas maneras al cambiar el invervalo el timer se reinicia siempre.
            ReiniciarTimerStepEndSound(duración);
            MedidorTimer.Restart(); // Se reinicia después de Timer.Start() para que siempre esté un poco por delante de este y se disminuyan los casos en los que pasa de 29 a 15 y después a 31.

        } // ActualizarIntervaloTimer>


        private void ReiniciarTimerStepEndSound(TimeSpan duraciónTimerPaso) {

            if (TimerStepEndSound is null) return; // No debería pasar.
            var duraciónPresonido = ObtenerDuraciónPresonido(OrdenDeEjecución.NúmeroPaso);
            if (duraciónTimerPaso.TotalMilliseconds > duraciónPresonido) {
                TimerStepEndSound.Interval = duraciónTimerPaso.Add(new TimeSpan(0, 0, 0, 0, -duraciónPresonido));
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
            return new TimeSpan(0, 0, 0, 0, (int)Math.Round(milisegundosPasoActual + MilisegundosTimerAntesDePausa 
                + DuraciónPasoParcialPorCambioDuración, 0));

        } // ObtenerTiempoPasoActual>


        public double ObtenerMilisegundosTimerPasoActual() => ObtenerTiempoPasoActual().TotalMilliseconds;


        public void ActualizarDuraciónPaso() {

            if (Timer is null || !Timer.IsEnabled) return;

            // Al actualizar la duración de Timer.Interval se reinicia. Para evitar este problema se establece la nueva duración del paso en dos pasos.
            // Primero establece un intervalo parcial del tiempo que falta para finalizar el paso actual modificado y después establece la nueva duración del paso completa.
            var nuevaDuraciónPasoCompleto = ObtenerDuraciónPaso(Preferencias.GameSpeed, Preferencias.ExecutionSpeed, OrdenDeEjecución.NúmeroPaso);
            var tiempoPasoActual = ObtenerTiempoPasoActual();
            TimeSpan duraciónPasoParcial;

            if (nuevaDuraciónPasoCompleto < tiempoPasoActual) { // Con la nueva velocidad el paso actual ya se habría terminado. Se debe incrementar el número de paso y hacer un paso parcial más pequeño.

                var pasosSaltados = tiempoPasoActual.TotalMilliseconds / nuevaDuraciónPasoCompleto.TotalMilliseconds; // Resta uno porque es el actual.
                var pasosSaltadosEnteros = (int)Math.Floor(pasosSaltados);
                var fracciónPasoSaltado = pasosSaltados - pasosSaltadosEnteros;
                for (int i = OrdenDeEjecución.NúmeroPaso; i < OrdenDeEjecución.NúmeroPaso + pasosSaltadosEnteros; i++) {
                    OrdenDeEjecución.Pasos[i].DuraciónEnJuego = ObtenerDuraciónPaso(i) / Preferencias.ExecutionSpeed;
                }
                OrdenDeEjecución.NúmeroPaso = OrdenDeEjecución.NúmeroPaso + pasosSaltadosEnteros;
                ActualizarPaso();
                var duraciónTranscurridaPasoActual = tiempoPasoActual.Add(-pasosSaltadosEnteros * nuevaDuraciónPasoCompleto);
                duraciónPasoParcial = nuevaDuraciónPasoCompleto - duraciónTranscurridaPasoActual; // Es la duración con la que se debe hacer un paso parcial. Es la duración del nuevo paso menos el tiempo que ya ha transcurrido de este.
                DuraciónPasoParcialPorCambioDuración = duraciónTranscurridaPasoActual.TotalMilliseconds;

            } else { // Con la nueva velocidad el paso actual aún no habría terminado. Se hace un paso parcial para terminarlo.
                duraciónPasoParcial = nuevaDuraciónPasoCompleto.Add(-tiempoPasoActual);
                DuraciónPasoParcialPorCambioDuración = tiempoPasoActual.TotalMilliseconds;
            }

            ActualizarDuraciónPasoEnTimerEnPróximoTick = true;
            ActualizarIntervaloTimer(duraciónPasoParcial);

        } // ActualizarDuraciónPaso>


        private void ActualizarPaso(bool stop = false, bool aplicandoPreferencias = false, bool cargandoBuildOrder = false, 
            bool siguienteOAnterior = false, bool cambióTamaño = false, bool alternandoSiguienteAnteriorPaso = false) {

            if (stop) {
                ActualizarContenidoPaso(númeroPaso: null);
            } else {

                if ((Timer is null || !Timer.IsEnabled) 
                    && !aplicandoPreferencias && !cargandoBuildOrder && !siguienteOAnterior && !cambióTamaño && !alternandoSiguienteAnteriorPaso) return; // Evita que se actualice el texto si no se ha dado clic en Start.
                
                if (!aplicandoPreferencias && Estado != EEstado.Stoped) {

                    if (OrdenDeEjecución.NúmeroPaso < 0) OrdenDeEjecución.NúmeroPaso = 0;
                    CambiandoTxtPasoAutomáticamente = true;
                    TxtPaso.Text = OrdenDeEjecución.NúmeroPaso.ToString();
                    CambiandoTxtPasoAutomáticamente = false;

                }

                ActualizarContenidoPaso(Estado == EEstado.Stoped ? (int?)null : OrdenDeEjecución.NúmeroPaso);

            }

        } // ActualizarPaso>


        private void ActualizarContenidoPaso(int? númeroPaso) {

            SpnPaso.Children.Clear();
            SpnPasoSiguiente.Children.Clear();
            SpnPasoAnterior.Children.Clear();

            var formatoPaso = new Formato($"{Preferencias.CurrentStepFontColor} {(Preferencias.CurrentStepFontBold ? "b" : "")} " +
                $"{Preferencias.FontName.Replace(" ", "").ToLowerInvariant()} normalpos M", out _, null) { TamañoBaseFuente = Preferencias.CurrentStepFontSize,
                TamañoImagen = Preferencias.ImageSize };
            var formatoSiguientePaso = new Formato($"{Preferencias.NextStepFontColor} {(Preferencias.NextStepFontBold ? "b" : "")} " +
                $"{Preferencias.FontName.Replace(" ", "").ToLowerInvariant()} normalpos M", out _, null) { TamañoBaseFuente = Preferencias.NextStepFontSize,
                TamañoImagen = Preferencias.ImageSize
            };
    
            OrdenDeEjecución.MostrarPaso(númeroPaso, formatoPaso, SpnPaso, mostrarSiempreÚltimoPaso: true, 
                this.Height - Preferencias.BottomMargenSteps - Preferencias.TopMarginCurrentStep, HorizontalAlignment.Left,
                Preferencias.BottomMargenSteps, out bool superóAltoPasoActual);

            var superóAltoSiguientePaso = false;
            if (númeroPaso != null && ObtenerMostrarSiguientePaso((int)númeroPaso)) {

                OrdenDeEjecución.MostrarPaso(númeroPaso + 1, formatoSiguientePaso, SpnPasoSiguiente, mostrarSiempreÚltimoPaso: false,
                    this.Height - (SpnInferior.ActualHeight == 0 ? 42 : SpnInferior.ActualHeight) - Preferencias.BottomMargenSteps 
                    - Preferencias.TopMarginNextStep, HorizontalAlignment.Right, Preferencias.BottomMargenSteps, out superóAltoSiguientePaso); // ActualHeight es cero al iniciar antes de cargar la interface, entonces se usa un valor fijo aproximado de 42.
                Application.Current.Resources["VisibilidadPasoSiguiente"] = Visibility.Visible;

            } else {
                Application.Current.Resources["VisibilidadPasoSiguiente"] = Visibility.Collapsed;
            }

            var superóAltoAnteriorPaso = false;
            if (númeroPaso != null && númeroPaso > 0 && MostrandoPasoAnterior) {

                OrdenDeEjecución.MostrarPaso(númeroPaso - 1, formatoSiguientePaso, SpnPasoAnterior, mostrarSiempreÚltimoPaso: false,
                    this.Height - (SpnInferior.ActualHeight == 0 ? 42 : SpnInferior.ActualHeight) - Preferencias.BottomMargenSteps
                    - Preferencias.TopMarginNextStep, HorizontalAlignment.Right, Preferencias.BottomMargenSteps, out superóAltoAnteriorPaso);
                Application.Current.Resources["VisibilidadPasoAnterior"] = Visibility.Visible;
                Application.Current.Resources["VisibilidadPasoSiguiente"] = Visibility.Collapsed;

            } else {
                Application.Current.Resources["VisibilidadPasoAnterior"] = Visibility.Collapsed;
            }

            if (this.WindowState == WindowState.Normal) 
                BtnAlert.Visibility = (superóAltoPasoActual || superóAltoSiguientePaso || superóAltoAnteriorPaso) ? Visibility.Visible : Visibility.Collapsed;

        } // ActualizarContenidoPaso>


        public void CargarBuildOrder(bool iniciando = false) {

            if (CmbBuildOrders.Text != Preferencias.CurrentBuildOrder) {
                EditandoComboBoxEnCódigo = true;
                CmbBuildOrders.Text = Preferencias.CurrentBuildOrder;
                EditandoComboBoxEnCódigo = false;
            }

            OrdenDeEjecución.CargarPasos(Preferencias.BuildOrdersDirectory, Preferencias.CurrentBuildOrder);
            if (!iniciando) ActualizarPaso(stop: Estado == EEstado.Stoped, cargandoBuildOrder: true);

        } // CargarBuildOrder>


        private void SuspenderBlinkingTiempoJuego() {

            SpnIndicadoresDeProgreso.Visibility = Visibility.Visible;
            TimerBlinkerGameTime.Stop();

        } // SuspenderBlinkingTiempoJuego>


        public void PlaySonidoInicio()
            => MediaPlayer.PlayFile(Path.Combine(DirectorioSonidosCortos, 
                ObtenerSonido(OrdenDeEjecución.NúmeroPaso)), ObtenerVolumenSonido(OrdenDeEjecución.NúmeroPaso));


        public void PlaySonidoFinal()
            => MediaPlayer.PlayFile(Path.Combine(DirectorioSonidosLargos, 
                ObtenerPresonido(OrdenDeEjecución.NúmeroPaso)), ObtenerVolumenPresonido(OrdenDeEjecución.NúmeroPaso));


        private void VerificarSiVentanaEsVisible() {

            var rectánguloPantallaActual = ObtenerRectánguloPantallaActual(ajustadoEscala: true);
            if (this.Top > SystemParameters.PrimaryScreenHeight || this.Left > SystemParameters.PrimaryScreenWidth) {

                Preferencias.ScreenResolution = ObtenerResoluciónRecomendada();
                Preferencias.EstablecerValoresRecomendados(Preferencias.ScreenResolution, Preferencias.Game);
                AplicarPreferencias();

            }

        } // VerificarSiVentanaEsVisible>


        #endregion Procedimientos y Funciones>


    } // MainWindow>



} // RTSHelper>
