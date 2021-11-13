using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace RTSHelper {



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {


        private static int NúmeroPaso = 0;

        private static double Velocidad = 1.7;

        private static string NombreBuildOrder = "Default";

        private static string DirectorioAplicación = AppDomain.CurrentDomain.BaseDirectory;

        private static string DirectorioBuildOrders = System.IO.Path.Combine(DirectorioAplicación, "Build Orders");

        private static string[] Pasos = new string[] {};

        private DispatcherTimer Timer;

        private DispatcherTimer TimerFocus = new DispatcherTimer();


        public MainWindow() {

            InitializeComponent();
            TimerFocus.Interval = TimeSpan.FromMilliseconds(20);
            TimerFocus.Tick += new EventHandler(TimerFocus_Tick);
            // LeerPasos(); No es necesario. Se ejecuta al iniciar el ComboBox.
            LeerPreferencias();
            LeerBuildOrders();

        } // MainWindow>


        private void LeerPasos() {

            var rutaBuildOrder = System.IO.Path.Combine(DirectorioBuildOrders, $"{NombreBuildOrder}.txt");
            if (!System.IO.Directory.Exists(DirectorioBuildOrders)) System.IO.Directory.CreateDirectory(DirectorioBuildOrders);
            if (!System.IO.File.Exists(rutaBuildOrder)) System.IO.File.WriteAllText(rutaBuildOrder, $@"Edit '\RTS Helper\Build Orders\{NombreBuildOrder}.txt'\nfor adding your build order.");
            Pasos = System.IO.File.ReadAllLines(rutaBuildOrder);

        } // LeerPasos>


        private void LeerBuildOrders() {

            var archivosBuildOrders = System.IO.Directory.GetFiles(DirectorioBuildOrders, "*.txt");
            foreach (var archivoBuildOrder in archivosBuildOrders) {
                var nombreBuildOrder = System.IO.Path.GetFileNameWithoutExtension(archivoBuildOrder);
                if (nombreBuildOrder.ToLower() != "default") CmbBuildOrders.Items.Add(nombreBuildOrder);
            }

        } // LeerBuildOrders>


        private void LeerPreferencias() {

            var rutaSettings = System.IO.Path.Combine(DirectorioAplicación, "Settings.json");
            Settings settings = null;

            if (System.IO.File.Exists(rutaSettings)) {
                settings = Settings.Deserializar(System.IO.File.ReadAllText(rutaSettings));
            } else {
                MessageBox.Show("Settings.json file wasn't found. The default options will be used.");
                settings = new Settings();
                System.IO.File.WriteAllText(rutaSettings, Settings.Serializar(settings));
            }

            this.Top = settings.Top;
            this.Left = settings.Left;
            this.Width = settings.Width;
            this.Height = settings.Height;
            Velocidad = settings.Speed;
            TxbPaso.FontSize = settings.CurrentStepFontSize;
            TxbPaso.Foreground = new SolidColorBrush(settings.CurrentStepFontColor);
            TxbPasoSiguiente.FontSize = settings.NextStepFontSize;
            TxbPasoSiguiente.Foreground = new SolidColorBrush(settings.NextStepFontColor);
            this.Opacity = settings.Opacity;
            this.Background = new SolidColorBrush(settings.BackColor);
            TxbPasoSiguiente.Visibility = settings.ShowNextStep ? Visibility.Visible : Visibility.Collapsed;

        } // LeerPreferencias>


        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        } // Window_MouseDown>


        private void BtnStart_Click(object sender, RoutedEventArgs e) {
            NúmeroPaso = 0;
            ReiniciarTimer();
        } // BtnStart_Click>


        private void ReiniciarTimer() {

            if (!(Timer is null)) Timer.Stop();
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, (int)Math.Round((Velocidad == 1.7 ? 1.02 : 1) * 60 * 1000 / Velocidad , 0)); // En realidad la velocidad 1.7 de AOE2 corresponde aproximadamente a 36 s reales. Lo cual es alrededor de 2% más lento de lo esperado (60/1.7 = 35.29 s).
            Timer.Start();
            ActualizarTexto();

        } // ReiniciarTimer>


        private void ActualizarTexto() {

            if (Timer is null || !Timer.IsEnabled) return;
            if (NúmeroPaso < 0) NúmeroPaso = 0;
            TxtPaso.Text = NúmeroPaso.ToString();
            TxbPaso.Text = Pasos.Length <= NúmeroPaso ? "End" : ProcesarTextoPaso(Pasos[NúmeroPaso]);
            TxbPasoSiguiente.Text = Pasos.Length <= NúmeroPaso + 1 ? (Pasos.Length <= NúmeroPaso ? "" : "End") : $"Next:\n{ProcesarTextoPaso(Pasos[NúmeroPaso + 1])}";

        } // ActualizarTexto>

        private string ProcesarTextoPaso(string textoPaso) => textoPaso.Replace("\\n", "\n");


        private void Timer_Tick(object sender, EventArgs e) {
            NúmeroPaso++;
            ActualizarTexto();
        } // Timer_Tick>


        private void BtnNext_Click(object sender, RoutedEventArgs e) {
            NúmeroPaso++;
            ReiniciarTimer();
        } // BtnNext_Click>


        private void BtnPrev_Click(object sender, RoutedEventArgs e) {     
            NúmeroPaso--;
            ReiniciarTimer();
        } // BtnPrev_Click>


        private void TxtPaso_TextChanged(object sender, TextChangedEventArgs e) {

            if (int.TryParse(TxtPaso.Text, out int intPaso)) {
                NúmeroPaso = intPaso;
                ReiniciarTimer();
            }

        } // TxtPaso_TextChanged>


        private void TimerFocus_Tick(object sender, EventArgs e) {
            TimerFocus.Stop();
            TxtPaso.SelectAll();
        } // TimerFocus_Tick>


        private void CmbBuildOrders_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            switch  (e.AddedItems[0]) {
                case string str:
                    NombreBuildOrder = str;
                    break;
                case ComboBoxItem cbi:
                    NombreBuildOrder = cbi.Content.ToString();
                    break;
                default:
                    break;
            }
            LeerPasos();
            ActualizarTexto();

        } // CmbBuildOrders_SelectionChanged>


        private void TxtPaso_GotFocus(object sender, RoutedEventArgs e) => TimerFocus.Start();

        private void TxtPaso_PreviewMouseDown(object sender, MouseButtonEventArgs e) => TimerFocus.Start();

        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();


    } // MainWindow>



    public class Settings {


        public double Speed { get; set; } = 1.7;

        public int CurrentStepFontSize { get; set; } = 22;

        public int NextStepFontSize { get; set; } = 16;

        public Color BackColor { get; set; } = Color.FromRgb(0, 0, 0);

        public double Opacity { get; set; } = 0.6;

        public Color CurrentStepFontColor { get; set; } = Color.FromRgb(255, 255, 255);

        public Color NextStepFontColor { get; set; } = Color.FromRgb(153, 153, 153);

        public int Width { get; set; } = 620;

        public int Height { get; set; } = 200;

        public int Top { get; set; } = 958;

        public int Left { get; set; } = 945;

        public static Settings Deserializar(string json) => (string.IsNullOrEmpty(json)) ? default : JsonSerializer.Deserialize<Settings>(json);

        public static string Serializar(Settings settings) => JsonSerializer.Serialize(settings);

        public bool ShowNextStep { get; set; } = true;


    } // Settings>



} // RTSHelper>
