﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using static RTSHelper.Global;



namespace RTSHelper {



    public class Instrucción {


        #region Propiedades

        public List<Segmento> Segmentos { get; set; } = new List<Segmento>();

        public bool AlinearInferiormente { get; set; } = false;

        #endregion Propiedades>


        #region Constructores

        public Instrucción(string textoInstrucción, Formato? formatoInicial, out Formato? últimoFormato) { // últimoFormato es el último formato que fue aplicado en la instrucción. Este formato se usa para el inicio de la próxima instrucción. El formato se mantiene para todo el paso.

            var finalizaConEtiquetaFormato = textoInstrucción.EndsWith(">");
            var coincidencias = Regex.Matches(textoInstrucción, @"<(.*?)>([^<]+)", RegexOptions.IgnoreCase);

            if (coincidencias.Count == 0) {

                if (finalizaConEtiquetaFormato) {

                    últimoFormato = null; // Se establece al final del procedimiento.
                    if (textoInstrucción.Contains("<")) {
                        Segmentos.AddRange(ExtraerSegmentos(textoInstrucción[0..(textoInstrucción.IndexOf("<"))], formatoInicial));
                    } // Si no contiene "<", es un formato mal formado, entonces no se agrega el texto.{
                    
                } else {
                    Segmentos.AddRange(ExtraerSegmentos(textoInstrucción, formatoInicial));
                    últimoFormato = formatoInicial;
                }

            } else { // coincidencias.Count > 0.

                últimoFormato = null;
                var índicePrimerFormato = coincidencias[0].Index;
                if (coincidencias[0].Index > 0) { // La instrucción no empieza por una etiqueta de formato <. Al primer segmento se le aplica el formato inicial.

                    var texto = textoInstrucción.Substring(0, índicePrimerFormato);
                    Segmentos.AddRange(ExtraerSegmentos(texto, formatoInicial));

                }

                foreach (Match? coincidencia in coincidencias) {

                    if (coincidencia != null && coincidencia.Success) {

                        var textoFormato = coincidencia.Groups[1].Value.ToLowerInvariant();
                        var texto = coincidencia.Groups[2].Value;
                        var formato = new Formato(textoFormato);
                        Segmentos.AddRange(ExtraerSegmentos(texto, formato));
                        últimoFormato = formato;
       
                    }

                }

            }

            if (finalizaConEtiquetaFormato) { // La instrucción finaliza con una etiqueta de formato. Esta no afecta ningún texto de la instrucción, pero si establece un formato inicial para la siguiente instrucción. Su uso más común es para cerrar un formato asignado a la instrucción actual con <>..
                var índiceInicioFormato = textoInstrucción.LastIndexOf("<");
                var textoFormato = textoInstrucción[(índiceInicioFormato + 1)..^1];
                últimoFormato = new Formato(textoFormato);
            }

        } // Instrucción>


        public static List<Segmento> ExtraerSegmentos(string textoPresegmento, Formato? formato) { // Separa los segmentos de imágenes de los de texto.

            var segmentos = new List<Segmento>();
            var tipoActual = TipoSegmento.Texto;
            var textoActual = new StringBuilder();

            foreach (var c in textoPresegmento) {

                if (c == '[' || c == ']') {

                    var texto = textoActual.ToString();
                    if (!string.IsNullOrEmpty(texto)) segmentos.Add(new Segmento(texto, formato, tipoActual));
                    textoActual.Clear();

                }

                switch (c) {
                    case '[':
                        tipoActual = TipoSegmento.Entidad;
                        break;
                    case ']':
                        tipoActual = TipoSegmento.Texto;
                        break;
                    default:
                        textoActual.Append(c);
                        break;
                }

            }

            var textoFinal = textoActual.ToString();
            if (!string.IsNullOrEmpty(textoFinal)) segmentos.Add(new Segmento(textoFinal, formato, tipoActual));
            if (textoPresegmento == "") segmentos.Add(new Segmento("", formato, TipoSegmento.Texto)); // Para poder soportar múltiples líneas.

            return segmentos;

        } // ExtraerSegmentos>


        #endregion Constructores>


    } // Instrucción>



} // RTSHelper>
