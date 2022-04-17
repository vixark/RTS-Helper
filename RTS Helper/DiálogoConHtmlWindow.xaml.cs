using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Vixark.General;



namespace RTSHelper {



    public partial class DiálogoConHtmlWindow : Window {


        public MessageBoxResult Respuesta { get; set; } = MessageBoxResult.None;


        public DiálogoConHtmlWindow(string mensaje, string título, string html, string textoBotónYes = "Update", bool mostrarBotónNo = true, 
            int altoVentana = 500) {

            InitializeComponent();
            WbCambios.NavigateToString(html);
            TbMensaje.Text = mensaje;
            if (string.IsNullOrEmpty(mensaje)) TbMensaje.Visibility = Visibility.Collapsed;
            this.Title = título;
            BtnYes.Content = textoBotónYes;
            BtnNo.Visibility = mostrarBotónNo ? Visibility.Visible : Visibility.Collapsed;
            this.Height = altoVentana;
            WbCambios.Height = altoVentana - 200 + (TbMensaje.Visibility == Visibility.Collapsed ? 70 : 0);

        } // DiálogoConHtmlWindow>


        private void BtnYes_Click(object sender, RoutedEventArgs e) {
            Respuesta = MessageBoxResult.Yes;
            this.Close();
        } // BtnYes_Click>


        private void BtnNo_Click(object sender, RoutedEventArgs e) {
            Respuesta = MessageBoxResult.No;
            this.Close();
        } // BtnNo_Click>


        public static MessageBoxResult MostrarDiálogo(string mensaje, string título, string html, string textoBotónYes = "Update", bool mostrarBotónNo = true,
            int altoVentana = 500) {

            var dlg = new DiálogoConHtmlWindow(mensaje, título, html, textoBotónYes, mostrarBotónNo, altoVentana);
            dlg.ShowDialog();
            return dlg.Respuesta;

        } // MostrarDiálogo>


        private void WbCambios_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e) {

            if (e.Uri == null) return;
            e.Cancel = true;
            AbrirUrl(e.Uri.AbsoluteUri);

        } // WbCambios_Navigating>


    } // DiálogoConHtmlWindow>



} // RTSHelper>
