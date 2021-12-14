using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using static RTSHelper.Global;



namespace RTSHelper {



    public class Paso {


        #region Propiedades

        public double DuraciónEnJuego { get; set; } = 0; // La duración en segundos del juego con la que se ejecutó el paso. Esta duración puede ser diferente en cada paso dependiendo de si el usuario cambió la velocidad de ejecución. Por ejemplo, si empezó con una velocidad de ejeución de 60% en el primer paso este se tardaría 60/0,6 = 100 segundos en el juego y los siguientes se podrían tardar 60 segundos. Este valor se usa para mantener actualizado el reloj de tiempo de juego de la interface.

        public List<Instrucción> Instrucciones { get; set; } = new List<Instrucción>();

        public Comportamiento Comportamiento { get; set; } = new Comportamiento();

        #endregion Propiedades>


        #region Propiedades de Comportamiento



        #endregion Propiedades de Comportamiento>


        #region Constructores

        public Paso(string? texto) => ProcesarPaso(texto);

        #endregion Constructores>


        #region Procedimientos y Funciones


        public static List<Paso> LeerPasos(string directorioBuildOrders, string nombreBuildOrder) {

            var pasos = new List<Paso>();
            var rutaBuildOrder = Path.Combine(directorioBuildOrders, $"{nombreBuildOrder}.txt");
            if (!Directory.Exists(directorioBuildOrders)) Directory.CreateDirectory(directorioBuildOrders);
            if (!File.Exists(rutaBuildOrder))
                File.WriteAllText(rutaBuildOrder, $@"Edit '\RTS Helper\Build Orders\{nombreBuildOrder}.txt' \\n to add your build order.");
            var textosPasos = File.ReadAllLines(rutaBuildOrder);
            foreach (var textoPaso in textosPasos) {
                if (!string.IsNullOrWhiteSpace(textoPaso)) pasos.Add(new Paso(textoPaso)); // No se agrega el paso si es un espacio en la build order. Estos espacios son útiles para tener más orden de edición.
            }
            if (pasos.Count == 0) pasos.Add(new Paso("")); // Para poder almacenar la duración de los pasos, debe haber como mínimo un paso.
            return pasos;

        } // LeerPasos>


        public void ProcesarPaso(string? texto) {

            var textoProcesado = texto?.TrimEnd().TrimStart() ?? "";
            if (textoProcesado.StartsWith("{")) { // Contiene comportamientos propios para el paso.

                var coincidenciasComportamientos = Regex.Match(textoProcesado, "{(.+?)}");
                if (coincidenciasComportamientos.Success) {

                    textoProcesado = textoProcesado.Replace(coincidenciasComportamientos.Groups[0].Value, "").TrimStart();
                    var textoComportamientos = coincidenciasComportamientos.Groups[1].Value;
                    Comportamiento = new Comportamiento(textoComportamientos);

                } 

            }

            textoProcesado = textoProcesado.Replace("   ", NuevaLíneaId).Replace("/n", NuevaLíneaId).Replace("\\n", NuevaLíneaId)
                .Replace($"{NuevaLíneaId}  ", NuevaLíneaId).Replace($"  {NuevaLíneaId}", NuevaLíneaId).Replace($" {NuevaLíneaId}", NuevaLíneaId)
                .Replace($"{NuevaLíneaId} ", NuevaLíneaId).Replace("/f", AlinearInferiormenteId).Replace("\\f", AlinearInferiormenteId)
                .Replace($"{AlinearInferiormenteId}  ", AlinearInferiormenteId).Replace($"  {AlinearInferiormenteId}", AlinearInferiormenteId)
                .Replace($" {AlinearInferiormenteId}", AlinearInferiormenteId).Replace($"{AlinearInferiormenteId} ", AlinearInferiormenteId);

            var instruccionesTexto = textoProcesado.Split(NuevaLíneaId).ToList();
            if (instruccionesTexto.Take(instruccionesTexto.Count - 1).Any(it => it.Contains(AlinearInferiormenteId)))
                MostrarError($"Incorrect use of \\f. It should go only before the last line.");
            var alinearÚltimaInstrucciónInferiormente = instruccionesTexto.Last().Contains(AlinearInferiormenteId);
            if (alinearÚltimaInstrucciónInferiormente) {
                instruccionesTexto.AddRange(instruccionesTexto.Last().Split(AlinearInferiormenteId).ToList());
                instruccionesTexto.RemoveAt(instruccionesTexto.Count - 3);            
            }

            var formatoActual = new Formato(null);
            foreach (var instrucciónTexto in instruccionesTexto) {
                Instrucciones.Add(new Instrucción(instrucciónTexto, formatoActual, out formatoActual));
            }
            Instrucciones.Last().AlinearInferiormente = alinearÚltimaInstrucciónInferiormente;

        } // ProcesarPaso>


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


        #endregion Procedimientos y Funciones>


    } // Paso>



} // RTSHelper>
