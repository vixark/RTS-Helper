using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Interop;
using WForms = System.Windows.Forms;



namespace RTSHelper {



    static class Global {


        public static Settings Preferencias;

        public static string DirectorioAplicación = AppDomain.CurrentDomain.BaseDirectory;

        public static string RutaPreferencias = System.IO.Path.Combine(DirectorioAplicación, "Settings.json");

        public static string NoneSoundString = "None";

        public static string DirectorioBuildOrdersPredeterminado = System.IO.Path.Combine(DirectorioAplicación, "Build Orders");

        public static string DirectorioSonidosCortos = System.IO.Path.Combine(System.IO.Path.Combine(DirectorioAplicación, "Sounds"), "Short");

        public static string DirectorioSonidosLargos = System.IO.Path.Combine(System.IO.Path.Combine(DirectorioAplicación, "Sounds"), "Long");

        public static string DirectorioBuildOrdersEfectivo => Preferencias.BuildOrderDirectory ?? DirectorioBuildOrdersPredeterminado;



        public static string ObtenerSeleccionadoEnCombobox(SelectionChangedEventArgs e) {

            var cbi = e.AddedItems[0] as ComboBoxItem;
            return cbi.Content.ToString();

        } // ObtenerSeleccionadoEnCombobox>


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


    } // Global>



} // RTSHelper>
