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



namespace RTSHelper {



    public partial class ProgresoWindow : Window {


        public static ProgresoWindow? ProgressWindowObj { get; set; }


        public ProgresoWindow(string mensaje, int máximo, int valorInicial = 0) {

            InitializeComponent();
            TbMensaje.Text = mensaje;
            PbProgreso.Maximum = máximo;
            PbProgreso.Value = valorInicial;

        } // ProgresoWindow>


        public static void Aumentar() {
            if (ProgressWindowObj != null) ProgressWindowObj.PbProgreso.Value++;
        } // Aumentar>


        public static void Iniciar(string mensaje, int máximo, int valorInicial = 0) {

            ProgressWindowObj = new ProgresoWindow(mensaje, máximo, valorInicial);
            ProgressWindowObj.Show();

        } // Iniciar>


        public static void Finalizar() {
            if (ProgressWindowObj != null) ProgressWindowObj.Close();
        } // Finalizar>


    } // ProgresoWindow>



} // RTSHelper>
