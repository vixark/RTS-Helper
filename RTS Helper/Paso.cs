using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        public string? Información { get; set; } = null; // Esta información se toma del último comentario de la línea.

        public int LíneaCódigo { get; set; } 

        #endregion Propiedades>


        #region Propiedades de Comportamiento



        #endregion Propiedades de Comportamiento>


        #region Constructores

        public Paso(int líneaCódigo, string? código, Comportamiento? comportamientoPadre, Formato? formatoPadre, Dictionary<string, Formato>? clasesDeFormatos, 
            Dictionary<string, Comportamiento>? clasesDeComportamientos, Dictionary<string,string> grupos, out string? errores, int número, 
            string? información) {

            errores = null;
            Número = número;
            Información = información;
            LíneaCódigo = líneaCódigo;

            ProcesarPaso(código, comportamientoPadre ?? new Comportamiento(), formatoPadre ?? new Formato(), clasesDeFormatos,
                    clasesDeComportamientos, grupos, out errores, out string? textoComportamientos);

            #region Adición de comportamiento empty a pasos vacíos.

            var pasoVacío = false;
            pasoVacío = Instrucciones.Count == 0 || (Instrucciones.Count == 1 && Instrucciones[0].AlinearInferiormente); // Se considera que si solo hay un paso alineado inferiormente, el paso tiene contenido vacío.
            if (pasoVacío && clasesDeComportamientos != null && clasesDeComportamientos.Keys.Contains("empty") && 
                (textoComportamientos == null || !textoComportamientos.Contiene("empty"))) {

                Comportamiento = Comportamiento.ObtenerComportamientoEfectivo(new Comportamiento($"{textoComportamientos} empty", out _, 
                    clasesDeComportamientos, out string? erroresInternos, Número), Comportamiento);
                AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

            }

            #endregion Adición de comportamiento empty a pasos vacíos>

        } // Paso>

        #endregion Constructores>


        #region Procedimientos y Funciones


        public void ProcesarPaso(string? código, Comportamiento comportamientoPadre, Formato formatoPadre, Dictionary<string, Formato>? clasesDeFormatos, 
            Dictionary<string, Comportamiento>? clasesDeComportamientos, Dictionary<string, string> grupos, out string? errores, out string? textoComportamientos) {

            errores = null;
            textoComportamientos = null;
            string códigoProcesado = código?.TrimEnd().TrimStart() ?? "";

            foreach (var kv in grupos.Reverse()) { // Se hace el reemplazo en reversa para permitir que se reusen los grupos del principio en grupos posteriores.
                if (códigoProcesado.Contiene(kv.Key)) códigoProcesado = códigoProcesado.Reemplazar($"[{kv.Key}]", kv.Value); // Se hace el reemplazo directo por facilidad y porque no se espera que se generen colisiones. Cuando hay un texto entre [] siempre se estará refiriendo a un nombre de una entidad o de un grupo y se considera primero que sea de grupo reemplazando el el nombre del grupo por el contenido. No se usa la función ReemplazarVarios porque su rendimiento es casi igual (500 esta función vs 300 de ReemplazarVarios) y se restringen los posibles carácteres que se pueden usar para los nombres de los grupos pues la función ReemplazarVarios usaría los nombres de los grupos dentro de una expresión regular.
            }

            if (códigoProcesado.Contiene("<><")) // Después de reemplazar los grupos pueden quedar código como [v]<x>[tree]<><p>[arrow]<>. Este código es consecuencia del uso de un grupo y un superíndice que debe aplicar a ese grupo. Pero ese código es incorrecto. El código correcto si se escribiera sin usar el grupo es [v]<x>[tree]<p>[arrow]<> por lo tanto se debe reemplazar estos códigos por el correcto usando Regex.
                códigoProcesado = Regex.Replace(códigoProcesado, "<><([^<[\\]]+)>", "<$1>");

            if (códigoProcesado.StartsWith("{")) { // Contiene comportamientos propios para el paso.

                var coincidenciasComportamientos = Regex.Match(códigoProcesado, "{(.+?)}");
                if (coincidenciasComportamientos.Success) {

                    códigoProcesado = códigoProcesado.Replace(coincidenciasComportamientos.Groups[0].Value, "").TrimStart();
                    textoComportamientos = coincidenciasComportamientos.Groups[1].Value;
                    Comportamiento = Comportamiento.ObtenerComportamientoEfectivo(new Comportamiento(textoComportamientos, out _, 
                        clasesDeComportamientos, out string? erroresInternos, Número), comportamientoPadre);
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

                } 

            } else {
                Comportamiento = Comportamiento.ObtenerComportamientoEfectivo(new Comportamiento(), comportamientoPadre); // Se asigna de esta manera para que cree un nuevo objeto de comportamientosPadre.
            }

            códigoProcesado = códigoProcesado.Replace("   ", NuevaLíneaId).Replace("/n", NuevaLíneaId).Replace("\\n", NuevaLíneaId)
                .Replace($"{NuevaLíneaId}  ", NuevaLíneaId).Replace($"  {NuevaLíneaId}", NuevaLíneaId).Replace($" {NuevaLíneaId}", NuevaLíneaId)
                .Replace($"{NuevaLíneaId} ", NuevaLíneaId).Replace("/f", AlinearInferiormenteId).Replace("\\f", AlinearInferiormenteId)
                .Replace($"{AlinearInferiormenteId}  ", AlinearInferiormenteId).Replace($"  {AlinearInferiormenteId}", AlinearInferiormenteId)
                .Replace($" {AlinearInferiormenteId}", AlinearInferiormenteId).Replace($"{AlinearInferiormenteId} ", AlinearInferiormenteId);

            var instruccionesTexto = códigoProcesado.Split(NuevaLíneaId).ToList();
            if (instruccionesTexto.Take(instruccionesTexto.Count - 1).Any(it => it.Contains(AlinearInferiormenteId)))
                AgregarErrores(ref errores, $"Incorrect use of \\f. It should go only before the last line.", Número);
            var alinearÚltimaInstrucciónInferiormente = instruccionesTexto.Last().Contains(AlinearInferiormenteId);
            if (alinearÚltimaInstrucciónInferiormente) {
                instruccionesTexto.AddRange(instruccionesTexto.Last().Split(AlinearInferiormenteId).ToList());
                instruccionesTexto.RemoveAt(instruccionesTexto.Count - 3);            
            }

            var formatoActual = new Formato();
            foreach (var instrucciónTexto in instruccionesTexto) {

                string instrucciónTextoLimpia;
                if (Preferencias.Game == AOE2Name || Preferencias.Game == AOE4Name) { // Eliminación de aldeanos y casas no es necesario para Age of Mythology.

                    instrucciónTextoLimpia = instrucciónTexto;

                    if (!Preferencias.ShowOptionalInstructions1 && instrucciónTexto.Contiene("[v]")) { // Aldeanos.

                        if (instrucciónTextoLimpia.Contiene(" or +[v]")) instrucciónTextoLimpia = instrucciónTextoLimpia.Reemplazar(" or +[v]", ""); // Un caso especial que solo sucede en estrategias un poco generales. Se hace así para evitar complejizar la expresión regular.
                        var patrónNuevoAldeano1 = "\\+ ?[0-9]? ?\\[v\\]"; // Patrón sin fuente secundaria. Coincide con + 2[v].
                        var patrónNuevoAldeano2 = "<[a-z]+>\\+<> ?<[a-z]+>[0-9]<> ?\\[v\\]"; // Patrón con fuente secundaria. Concide con <b>+<> <t>4<>[v].
                        var patrónNuevoAldeano3 = "<[a-z]+>\\+<> ?\\[v\\]"; // Patrón con fuente secundaria para un aldeano. Concide con <b>+<>[v].
                        instrucciónTextoLimpia = Regex.Replace(instrucciónTextoLimpia,
                            $"{patrónNuevoAldeano1} ?\\[[0-9]s\\] | \\[[0-9]s\\] ?{patrónNuevoAldeano1}|{patrónNuevoAldeano1}|" +
                            $"{patrónNuevoAldeano2} ?\\[[0-9]s\\] | \\[[0-9]s\\] ?{patrónNuevoAldeano2}|{patrónNuevoAldeano2}|" +
                            $"{patrónNuevoAldeano3} ?\\[[0-9]s\\] | \\[[0-9]s\\] ?{patrónNuevoAldeano3}|{patrónNuevoAldeano3}"
                            , "", RegexOptions.IgnoreCase); // Concide con textos como "patrónNuevoAldeano# [3s] ", " [3s] patrónNuevoAldeano#" y "patrónNuevoAldeano#".

                    }

                    if (!Preferencias.ShowOptionalInstructions2 && instrucciónTexto.Contiene("[h]") && !instrucciónTexto.Contiene("<r>")) { // Casas.
                        instrucciónTextoLimpia = Regex.Replace(instrucciónTextoLimpia,
                            "\\[v\\]<x>\\[h\\]<> → [[0-9] \\[h\\]<x>\\[flag\\]<>$|\\[v\\]<x>\\[h\\]<> → <[a-z]>[[0-9]<> \\[h\\]<x>\\[flag\\]<>$", "", 
                            RegexOptions.IgnoreCase); // Solo se reemplazan las instrucciones de casas con el constructor de casas dedicado. Las construcciones de casa al inicio del juego son importantes y no se deben omitir. Solo se reemplazan instrucciones independientes de casas (por eso se usa $) para evitar reemplazar instrucciones como "[+4h] [->w]".
                    }

                } else {
                    instrucciónTextoLimpia = instrucciónTexto;
                }

                if (!string.IsNullOrWhiteSpace(instrucciónTextoLimpia)) {

                    Instrucciones.Add(new Instrucción(instrucciónTextoLimpia, formatoActual, formatoPadre, clasesDeFormatos, out formatoActual,
                        out string? erroresInternos, Número));
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

                }

            }

            if (Instrucciones.Count > 0) Instrucciones.Last().AlinearInferiormente = alinearÚltimaInstrucciónInferiormente;

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
            return sumaDuración + pasosExceso * Preferencias.StepDuration; // Se corrige a Preferencias.StepDuration que viene siendo el predeterminado del juego actual porque aunque no sea lo más intuitivo si la estrategia actual tiene un tiempo predeterminado es el tiempo que usa el temporizador de duración de paso para los pasos extra, entonces se prefiere no tocar ese código y cambiarlo aquí que me aseguro que solo aplica para efectos del tiempo mostrado. Valor anterior: pasos[pasoFinalSuma].DuraciónEnJuego; Se aproxima que los siguientes pasos después de finalizar los que están en Pasos sean a la misma velocidad de ejeución del último paso, esto puede que no sea exacto si el usuario empieza a mover la velocidad de ejeución después de finalizados todos los pasos, pero es aceptable porque se prefiere no agregar pasos 'ficticios' para poder guardar la duración de estos pasos de exceso porque mucha lógica está condicionada al largo de Pasos.

        } // ObtenerDuraciónPasosAnteriores>


        #endregion Procedimientos y Funciones>


    } // Paso>



} // RTSHelper>
