﻿using System;
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

        public Paso(string? texto, Comportamiento? comportamientoPadre, Formato? formatoPadre, Dictionary<string, Formato>? clasesDeFormatos, 
            Dictionary<string, Comportamiento>? clasesDeComportamientos) 
                => ProcesarPaso(texto, comportamientoPadre ?? new Comportamiento(), formatoPadre ?? new Formato(), clasesDeFormatos, 
                    clasesDeComportamientos);

        #endregion Constructores>


        #region Procedimientos y Funciones


        public void ProcesarPaso(string? texto, Comportamiento comportamientoPadre, Formato formatoPadre, Dictionary<string, Formato>? clasesDeFormatos, 
            Dictionary<string, Comportamiento>? clasesDeComportamientos) {

            var textoProcesado = texto?.TrimEnd().TrimStart() ?? "";
            if (textoProcesado.StartsWith("{")) { // Contiene comportamientos propios para el paso.

                var coincidenciasComportamientos = Regex.Match(textoProcesado, "{(.+?)}");
                if (coincidenciasComportamientos.Success) {

                    textoProcesado = textoProcesado.Replace(coincidenciasComportamientos.Groups[0].Value, "").TrimStart();
                    var textoComportamientos = coincidenciasComportamientos.Groups[1].Value;
                    Comportamiento = Comportamiento.ObtenerComportamientoEfectivo(new Comportamiento(textoComportamientos, out _, 
                        clasesDeComportamientos), comportamientoPadre);

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
                MostrarError($"Incorrect use of \\f. It should go only before the last line.");
            var alinearÚltimaInstrucciónInferiormente = instruccionesTexto.Last().Contains(AlinearInferiormenteId);
            if (alinearÚltimaInstrucciónInferiormente) {
                instruccionesTexto.AddRange(instruccionesTexto.Last().Split(AlinearInferiormenteId).ToList());
                instruccionesTexto.RemoveAt(instruccionesTexto.Count - 3);            
            }

            var formatoActual = new Formato();
            foreach (var instrucciónTexto in instruccionesTexto) {
                Instrucciones.Add(new Instrucción(instrucciónTexto, formatoActual, formatoPadre, clasesDeFormatos, out formatoActual));
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
