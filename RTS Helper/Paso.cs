using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using static RTSHelper.Global;
using static Vixark.General;



namespace RTSHelper {



    public class Paso {


        #region Propiedades

        public double DuraciónEnJuego { get; set; } = 0; // La duración en segundos del juego con la que se ejecutó el paso. Esta duración puede ser diferente en cada paso dependiendo de si el usuario cambió la velocidad de ejecución. Por ejemplo, si empezó con una velocidad de ejeución de 60% en el primer paso este se tardaría 60/0,6 = 100 segundos en el juego y los siguientes se podrían tardar 60 segundos. Este valor se usa para mantener actualizado el reloj de tiempo de juego de la interface.

        public List<Instrucción> Instrucciones { get; set; } = new List<Instrucción>();

        public Comportamiento Comportamiento { get; set; } = new Comportamiento();

        public double? DesfaceAcumulado { get; set; } = null; // El desface acumulado generado hasta este paso. En el caso de Age of Empires II equivale al tiempo desocupado que estuvo el centro de pueblo durante desde el inico del juego hasta el final de este paso.

        public int Número { get; set; } = 0;

        #endregion Propiedades>


        #region Propiedades de Comportamiento



        #endregion Propiedades de Comportamiento>


        #region Constructores

        public Paso(string? texto, Comportamiento? comportamientoPadre, Formato? formatoPadre, Dictionary<string, Formato>? clasesDeFormatos, 
            Dictionary<string, Comportamiento>? clasesDeComportamientos, Dictionary<string,string> grupos, out string? errores, int número) {

            errores = null;
            Número = número;
            ProcesarPaso(texto, comportamientoPadre ?? new Comportamiento(), formatoPadre ?? new Formato(), clasesDeFormatos,
                    clasesDeComportamientos, grupos, out errores);

        } // Paso>

        #endregion Constructores>


        #region Procedimientos y Funciones


        public void ProcesarPaso(string? texto, Comportamiento comportamientoPadre, Formato formatoPadre, Dictionary<string, Formato>? clasesDeFormatos, 
            Dictionary<string, Comportamiento>? clasesDeComportamientos, Dictionary<string, string> grupos, out string? errores) {

            errores = null;
            string textoProcesado = texto?.TrimEnd().TrimStart() ?? "";

            foreach (var kv in grupos) {
                if (textoProcesado.Contiene(kv.Key)) textoProcesado = textoProcesado.Reemplazar($"[{kv.Key}]", kv.Value); // Se hace el reemplazo directo por facilidad y porque no se espera que se generen colisiones. Cuando hay un texto entre [] siempre se estará refiriendo a un nombre de una entidad o de un grupo y se considera primero que sea de grupo reemplazando el el nombre del grupo por el contenido. No se usa la función ReemplazarVarios porque su rendimiento es casi igual (500 esta función vs 300 de ReemplazarVarios) y se restringen los posibles carácteres que se pueden usar para los nombres de los grupos pues la función ReemplazarVarios usaría los nombres de los grupos dentro de una expresión regular.
            }

            if (textoProcesado.StartsWith("{")) { // Contiene comportamientos propios para el paso.

                var coincidenciasComportamientos = Regex.Match(textoProcesado, "{(.+?)}");
                if (coincidenciasComportamientos.Success) {

                    textoProcesado = textoProcesado.Replace(coincidenciasComportamientos.Groups[0].Value, "").TrimStart();
                    var textoComportamientos = coincidenciasComportamientos.Groups[1].Value;
                    Comportamiento = Comportamiento.ObtenerComportamientoEfectivo(new Comportamiento(textoComportamientos, out _, 
                        clasesDeComportamientos, out string? erroresInternos, Número), comportamientoPadre);
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

                } 

            } else {
                Comportamiento = Comportamiento.ObtenerComportamientoEfectivo(new Comportamiento(), comportamientoPadre); // Se asigna de esta manera para que cree un nuevo objeto de comportamientosPadre.
            }

            textoProcesado = textoProcesado.Replace("   ", NuevaLíneaId).Replace("/n", NuevaLíneaId).Replace("\\n", NuevaLíneaId)
                .Replace($"{NuevaLíneaId}  ", NuevaLíneaId).Replace($"  {NuevaLíneaId}", NuevaLíneaId).Replace($" {NuevaLíneaId}", NuevaLíneaId)
                .Replace($"{NuevaLíneaId} ", NuevaLíneaId).Replace("/f", AlinearInferiormenteId).Replace("\\f", AlinearInferiormenteId)
                .Replace($"{AlinearInferiormenteId}  ", AlinearInferiormenteId).Replace($"  {AlinearInferiormenteId}", AlinearInferiormenteId)
                .Replace($" {AlinearInferiormenteId}", AlinearInferiormenteId).Replace($"{AlinearInferiormenteId} ", AlinearInferiormenteId);

            var instruccionesTexto = textoProcesado.Split(NuevaLíneaId).ToList();
            if (instruccionesTexto.Take(instruccionesTexto.Count - 1).Any(it => it.Contains(AlinearInferiormenteId)))
                AgregarErrores(ref errores, $"Incorrect use of \\f. It should go only before the last line.", Número);
            var alinearÚltimaInstrucciónInferiormente = instruccionesTexto.Last().Contains(AlinearInferiormenteId);
            if (alinearÚltimaInstrucciónInferiormente) {
                instruccionesTexto.AddRange(instruccionesTexto.Last().Split(AlinearInferiormenteId).ToList());
                instruccionesTexto.RemoveAt(instruccionesTexto.Count - 3);            
            }

            var formatoActual = new Formato();
            foreach (var instrucciónTexto in instruccionesTexto) {

                Instrucciones.Add(new Instrucción(instrucciónTexto, formatoActual, formatoPadre, clasesDeFormatos, out formatoActual, 
                    out string? erroresInternos, Número));
                AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

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
