using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace RTSHelper {


    public class DiálogoIngresoDato {


        #region Propiedades

        private Window Ventana { get; set; }

        private bool ClicEnAceptar { get; set; }

        private TextBox TextboxDato { get; set; }

        #endregion Propiedades>


        #region Métodos y Funciones


        private DiálogoIngresoDato(string mensaje, string título) {

            var contenedorPrincipal = new StackPanel();
            var tamañoLetra = 14;
            var margen = new Thickness(20);

            Ventana = new Window {
                Height = 200,
                Width = 300,
                //Background = ObtenerBrocha(BrochaTema.Fondo),
                Title = título,
                FontSize = tamañoLetra,
                Content = contenedorPrincipal,
                WindowStyle = WindowStyle.SingleBorderWindow,
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            var cuadroDescripción = new TextBlock {
                Margin = margen,
                TextWrapping = TextWrapping.Wrap,
                //Foreground = ObtenerBrocha(BrochaTema.Texto),
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = mensaje
            };

            contenedorPrincipal.Children.Add(cuadroDescripción);

            TextboxDato = new TextBox { HorizontalAlignment = HorizontalAlignment.Center, Width = 200 };
            TextboxDato.KeyDown += DiálogoIngresoDato_KeyDown;

            contenedorPrincipal.Children.Add(TextboxDato);

            var botónAceptar = new Button { Width = 100, Margin = margen };
            botónAceptar.Click += BotónAceptar_Click;
            botónAceptar.Content = "Ok";

            var contenedorBotónAceptar = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
            contenedorPrincipal.Children.Add(contenedorBotónAceptar);
            contenedorBotónAceptar.Children.Add(botónAceptar);

            TextboxDato.Focus();

        } // CuadroDiálogoContraseña>


        private void DiálogoIngresoDato_KeyDown(object sender, KeyEventArgs e) {

            if (e.Key == Key.Enter && ClicEnAceptar == false) {
                e.Handled = true;
                BotónAceptar_Click(TextboxDato, null);
            }

        } // CuadroContraseña_KeyDown>


        private void BotónAceptar_Click(object sender, RoutedEventArgs? e) {

            ClicEnAceptar = true;
            if (!string.IsNullOrEmpty(TextboxDato.Text)) Ventana.Close();
            ClicEnAceptar = false;

        } // Aceptar_Clic>


        public static string Mostrar(string mensaje, string título) {

            var diálogo = new DiálogoIngresoDato(mensaje, título);
            diálogo.Ventana.ShowDialog();
            return diálogo.TextboxDato.Text;

        } // Mostrar>


        #endregion Métodos y Funciones>


    } // DiálogoIngresoDato>


} // RTSHelper>
