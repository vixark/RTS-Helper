using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Interop;
using WForms = System.Windows.Forms;
using System.IO;



namespace RTSHelper {



    static class Global {


        public static Settings Preferencias = new Settings();

        public static string DirectorioAplicación = AppDomain.CurrentDomain.BaseDirectory ?? @"C:\"; // En realidad no veo en que situaciuón podría ser null BaseDirectory.

        public static string RutaPreferencias = Path.Combine(DirectorioAplicación, "Settings.json");

        public static string NoneSoundString = "None";

        public static string DirectorioBuildOrdersPredeterminado = Path.Combine(DirectorioAplicación, "Build Orders");

        public static string DirectorioSonidosCortos = Path.Combine(Path.Combine(DirectorioAplicación, "Sounds"), "Short");

        public static string DirectorioSonidosLargos = Path.Combine(Path.Combine(DirectorioAplicación, "Sounds"), "Long");

        public static string DirectorioBuildOrdersEfectivo => Preferencias.BuildOrderDirectory ?? DirectorioBuildOrdersPredeterminado;



        public static string ObtenerSeleccionadoEnCombobox(SelectionChangedEventArgs e) {

            var cbi = e.AddedItems[0] as ComboBoxItem;
            if (cbi != null) {
                if (cbi.Content is null) {
                    throw new Exception("No se encontró el elemento seleccionado en cbi.Content."); // Nunca debería pasar.
                } else {
                    return cbi.Content?.ToString() ?? throw new Exception("No se encontró el elemento seleccionado en cbi.Content?.ToString().");
                }              
            } else {
                var str = e.AddedItems[0] as string;
                return str ?? throw new Exception("No se encontró el elemento seleccionado en str.");
            }

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


        public static int ObtenerDuraciónEndStepSound() 
            => (int)Math.Round(1000 * MediaPlayer.GetDuration(Path.Combine(DirectorioSonidosLargos, Preferencias.StepEndSound)), 0);
       

    } // Global>



} // RTSHelper>
