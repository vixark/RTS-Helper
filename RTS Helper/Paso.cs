using System;
using System.Collections.Generic;
using System.IO;
using System.Text;



namespace RTSHelper {



    public class Paso {


        public string? Texto { get; set; } 

        public double DuraciónEnJuego { get; set; } = 0; // La duración en segundos del juego con la que se ejecutó el paso. Esta duración puede ser diferente en cada paso dependiendo de si el usuario cambió la velocidad de ejecución. Por ejemplo, si empezó con una velocidad de ejeución de 60% en el primer paso este se tardaría 60/0,6 = 100 segundos en el juego y los siguientes se podrían tardar 60 segundos. Este valor se usa para mantener actualizado el reloj de tiempo de juego de la interface.

        public Paso(string? texto) => Texto = texto;


        public string? TextoProcesado => Texto?.Replace("   ", "\n").Replace("/n", "\n").Replace("\\n", "\n").Replace("\n  ", "\n")
            .Replace("  \n", "\n").Replace(" \n", "\n").Replace("\n ", "\n"); // No se usa 2 espacios como salto de línea porque considero dos se pueden poner por error o ser necesarios en algunos casos. 3 espacios permiten un distanciamiento más cómodo y visualmente es claramente distinto de un espacios a la hora de revisar el texto.


        public static List<Paso> LeerPasos(string directorioBuildOrders, string nombreBuildOrder) {

            var pasos = new List<Paso>();
            var rutaBuildOrder = Path.Combine(directorioBuildOrders, $"{nombreBuildOrder}.txt");
            if (!Directory.Exists(directorioBuildOrders)) Directory.CreateDirectory(directorioBuildOrders);
            if (!File.Exists(rutaBuildOrder))
                File.WriteAllText(rutaBuildOrder, $@"Edit '\RTS Helper\Build Orders\{nombreBuildOrder}.txt' \n to add your build order.");
            var textosPasos = File.ReadAllLines(rutaBuildOrder);
            foreach (var textoPaso in textosPasos) {
                pasos.Add(new Paso(textoPaso));
            }
            if (pasos.Count == 0) pasos.Add(new Paso("")); // Para poder almacenar la duración de los pasos como mínimo debe haber un paso.
            return pasos;

        } // LeerPasos>


        public static Double ObtenerDuraciónPasosAnteriores(List<Paso> pasos, int pasoActual) {

            if (pasos.Count == 0) return 0;
            var pasoFinalSuma = pasoActual;
            var sumaDuración = (double)0;
            var pasosExceso = 0;
            if (pasoActual > pasos.Count - 1) {
                pasoFinalSuma = pasos.Count - 1;
                pasosExceso = pasoActual - pasoFinalSuma;
            }
            for (int i = 0; i < pasoFinalSuma; i++) {
                sumaDuración += pasos[i].DuraciónEnJuego;
            }
            return sumaDuración + pasosExceso * pasos[pasoFinalSuma].DuraciónEnJuego; // Se aproxima que los siguientes pasos después de finalizar los que están en Pasos sean a la misma velocidad de ejeución del último paso, esto puede que no sea exacto si el usuario empieza a mover la velocidad de ejeución después de finalizados todos los pasos, pero es aceptable porque se prefiere no agregar pasos 'ficticios' para poder guardar la duración de estos pasos de exceso porque mucha lógica está condicionada al largo de Pasos.

        } // ObtenerDuraciónPasosAnteriores>


    } // Paso>



} // RTSHelper>
