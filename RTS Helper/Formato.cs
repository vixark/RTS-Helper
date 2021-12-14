using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using static RTSHelper.Global;
using static Vixark.General;



namespace RTSHelper {



    public class Formato {


        #region Propiedades

        public TamañosFuente TamañoFuente { get; set; } = TamañosFuente.Indeterminado;

        public string? NombreFuente { get; set; }

        public Color? Color { get; set; }

        public bool? Negrita { get; set; }

        public bool? Cursiva { get; set; }

        public bool? Subrayado { get; set; }

        public PosiciónTexto Posición { get; set; } = PosiciónTexto.Indeterminado;

        public double? TamañoBaseFuente { get; set; }

        public double? ImageSize { get; set; }

        #endregion Propiedades>


        #region Constructores

        public Formato(string? textoFormato) {

            var valores = textoFormato?.ToLowerInvariant()?.Split(" ").ToList() ?? new List<string>();
            foreach (var valor in valores) {

                if (string.IsNullOrEmpty(valor)) continue;

                var valorIdentificado = false;
                if (Estilos.Contains(valor)) {

                    switch (valor) {
                        case "b":
                            Negrita = true;  
                            break;
                        case "nb":
                            Negrita = false;
                            break;
                        case "i":
                            Cursiva = true;
                            break;
                        case "u":
                            Subrayado = true;
                            break;
                        default:
                            MostrarError($"To Developer: The value '{valor}' wasn't expected in Formato().");
                            break;
                    }
                    valorIdentificado = true;

                }

                if (Tamaños.Contains(valor)) {
                    TamañoFuente = (TamañosFuente)Enum.Parse(typeof(TamañosFuente), valor.ToUpperInvariant());
                    valorIdentificado = true;
                }    

                if (Posiciones.Contains(valor)) {

                    switch (valor) {
                        case "sub":
                            Posición = PosiciónTexto.Subíndice;
                            break;
                        case "sup":
                            Posición = PosiciónTexto.Superíndice;
                            break;
                        case "normalpos":
                            Posición = PosiciónTexto.Normal;
                            break;
                        default:
                            break;
                    }
                    valorIdentificado = true;

                }

                if (Colores.Contains(valor) || valor.StartsWith("#")) {

                    Color = ObtenerMediaColor(valor);
                    if (Color == null) MostrarError($"Color '{valor}' isn't valid.");
                    valorIdentificado = true;

                }

                if (Fuentes.Keys.Contains(valor)) {
                    NombreFuente = Fuentes[valor].Nombre;
                    valorIdentificado = true;
                }

                if (valor.StartsWith("is")) {

                    var coincidencias = Regex.Matches(valor, "is([0-9]+)");
                    if (coincidencias.Count == 1) {
                        ImageSize = double.Parse(coincidencias[0].Groups[1].Value);
                        valorIdentificado = true;
                    } else {
                        MostrarError($"The format keyword '{valor}' isn't supported.");
                    }

                }

                if (!valorIdentificado) MostrarError($"The format keyword '{valor}' isn't supported.");             

            }

        } // Formato>

        #endregion Constructores>


        #region Funciones y Procedimientos


        public static Formato ObtenerFormatoEfectivo(Formato? formatoHijo, Formato formatoPadre) {

            var formato = new Formato("");
            formato.Negrita = (formatoHijo?.Negrita ?? formatoPadre.Negrita) ?? CurrentStepFontBoldPredeterminado;
            formato.Subrayado = (formatoHijo?.Subrayado ?? formatoPadre.Subrayado) ?? false;
            formato.Cursiva = (formatoHijo?.Cursiva ?? formatoPadre.Cursiva) ?? false;
            formato.NombreFuente = (formatoHijo?.NombreFuente ?? formatoPadre.NombreFuente) ?? NombreFuentePredeterminada;
            formato.Color = (formatoHijo?.Color ?? formatoPadre.Color) ?? ObtenerMediaColor(CurrentStepFontColorPredeterminado);
            formato.TamañoFuente = formatoHijo?.TamañoFuente == TamañosFuente.Indeterminado ? formatoPadre.TamañoFuente 
                : (formatoHijo != null ? formatoHijo.TamañoFuente : TamañosFuente.M);
            formato.Posición = formatoHijo?.Posición == PosiciónTexto.Indeterminado ? formatoPadre.Posición 
                : (formatoHijo != null ? formatoHijo.Posición : PosiciónTexto.Normal);
            formato.ImageSize = (formatoHijo?.ImageSize ?? formatoPadre.ImageSize) ?? ImageSizePredeterminado;

            if (formatoHijo?.TamañoBaseFuente != null && formatoPadre.TamañoBaseFuente != null
                && formatoHijo.TamañoBaseFuente != formatoPadre.TamañoBaseFuente)
                MostrarError("To Developer: TamañoBaseFuente can't be different in formatoHijo and formatoPadre.");

            formato.TamañoBaseFuente = formatoPadre.TamañoBaseFuente ?? formatoHijo?.TamañoBaseFuente ?? 16; // El TamañoBaseFuente es la única propiedad en la que se le da prioridad al formatoPadre porque es donde usualmente se asigna y el que dicta el tamaño general de todo el texto. Tener varios segmentos con diferentes tamaño fuente base no tiene mucho sentido porque se pierde la funcionalidad de los tamaños relativos.

            return formato;

        } // ObtenerFormatoEfectivo>


        public double? TamañoFuenteEfectiva
            => TamañoBaseFuente == null ? null : TamañoFuente switch {
                TamañosFuente.XXXL => 3D * TamañoBaseFuente,
                TamañosFuente.XXL => 2D * TamañoBaseFuente,
                TamañosFuente.XL => 1.5D * TamañoBaseFuente,
                TamañosFuente.L => 1.3D * TamañoBaseFuente,
                TamañosFuente.M => 1 * TamañoBaseFuente, 
                TamañosFuente.S => (1D / 1.3) * TamañoBaseFuente,
                TamañosFuente.XS => (1D / 1.5) * TamañoBaseFuente,
                TamañosFuente.XXS => (1D / 2) * TamañoBaseFuente,
                TamañosFuente.XXXS => (1D / 3) * TamañoBaseFuente,
                TamañosFuente.Indeterminado => null
            };


        public string? ColorHexadecimal => Color == null ? null : Color.ToString();


        public double? ObtenerTamañoImagenEfectiva(double imageSize) => TamañoFuenteEfectiva * FactorTamañoTextoAPixeles * (imageSize / 100);


        public static FontFamily ObtenerFuentePosiciónEspecial(PosiciónTexto posición, string texto, string nombreFuente) {

            var tipo = Regex.IsMatch(texto, "^[0-9 ]+$") ? "num" : "alfanum"; // En Global.cs se buscó false, .*, true, .* y no hubo resultados. Esto significa que no hay fuentes que tengan superíndice letras y que no tengan superíndice números. Es decir el superíndice es alfanumérico o solo numérico. Igual para el subíndice.
            var palatinoLinotype = new FontFamily("Palatino Linotype");
            var fuente = Fuentes.FirstOrDefault(f => f.Value.Nombre == nombreFuente).Value;
            if (fuente == null) return palatinoLinotype;
            var fuenteCumpleRequerimiento = false;
            var requerimiento = "";

            switch (posición) {
                case PosiciónTexto.Indeterminado:
                    MostrarError("To Developer: Indeterminado is unexpected in ObtenerFuentePosiciónEspecial()");
                    break;
                case PosiciónTexto.Normal:
                    MostrarError("To Developer: Normal is unexpected in ObtenerFuentePosiciónEspecial()");
                    break;
                case PosiciónTexto.Subíndice:

                    requerimiento = $"sub-{tipo}";
                    if (tipo == "num" && fuente.SubíndiceNúmeros) {
                        fuenteCumpleRequerimiento = true;
                    } else if (tipo == "alfanum" && fuente.SubíndiceLetras) {
                        fuenteCumpleRequerimiento = true;
                    }
                    break;

                case PosiciónTexto.Superíndice:

                    requerimiento = $"sup-{tipo}";
                    if (tipo == "num" && fuente.SuperíndiceNúmeros) {
                        fuenteCumpleRequerimiento = true;
                    } else if (tipo == "alfanum" && fuente.SuperíndiceLetras) {
                        fuenteCumpleRequerimiento = true;
                    }
                    break;

                default:
                    break;
            }

            if (fuenteCumpleRequerimiento) return new FontFamily(fuente.Nombre);
            if (requerimiento == "sub-alfanum") return palatinoLinotype;

            switch (fuente.Tipo) {
                case TipoFuente.Sans:

                    switch (requerimiento) {
                        case "sup-num":       
                        case "sup-alfanum":
                        case "sub-num":
                            return new FontFamily("Calibri");
                    }
                    break;

                case TipoFuente.SansNegrita:

                    switch (requerimiento) {
                        case "sup-num":
                            return new FontFamily("Impact");
                        case "sup-alfanum":
                        case "sub-num":
                            return new FontFamily("Calibri");
                    }
                    break;

                case TipoFuente.Caligráfica:

                    switch (requerimiento) {
                        case "sup-num":
                        case "sub-num":
                            return new FontFamily("Gabriola");
                        case "sup-alfanum":
                            return new FontFamily("Calibri");
                    }
                    break;

                case TipoFuente.SerifCuadrada: 
                case TipoFuente.Símbolos:
                case TipoFuente.Serif:
                    return palatinoLinotype;
                default:
                    MostrarError("To Developer: default is unexpected in ObtenerFuentePosiciónEspecial()");
                    break;
            }

            return palatinoLinotype;

        } // ObtenerFuentePosiciónEspecial>


        #endregion Funciones y Procedimientos>


    } // Formato>



} // RTSHelper>
