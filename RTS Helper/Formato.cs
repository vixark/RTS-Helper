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
        // Al agregar una nueva se debe agregar en ObtenerFormatoEfectivo() y en CopiarPropiedadesEnNulas().

        public TamañosFuente TamañoFuente { get; set; } = TamañosFuente.Indeterminado;

        public string? NombreFuente { get; set; }

        public Color? Color { get; set; }

        public bool? Negrita { get; set; }

        public bool? Cursiva { get; set; }

        public bool? Subrayado { get; set; }

        public PosiciónTexto Posición { get; set; } = PosiciónTexto.Indeterminado;

        public double? TamañoBaseFuente { get; set; } // Esta propiedad no es personalizable desde los archivos de texto de build orders.

        public double? TamañoImagen { get; set; }

        #endregion Propiedades>



        #region Constructores


        public Formato() { }


        public Formato(string? texto, out Dictionary<string, Formato> clasesLeídas, Dictionary<string, Formato>? clases) {

            clasesLeídas = new Dictionary<string, Formato>();
            if (texto == null) return; // Si el texto es vacío, no contiene ni formatos ni clases.

            var textoMinúscula = texto.ToLowerInvariant();
            var palabrasClave = new List<string>();
            palabrasClave.AddRange(Estilos);
            palabrasClave.AddRange(Posiciones);
            palabrasClave.AddRange(Colores);
            palabrasClave.AddRange(Tamaños);
            palabrasClave.AddRange(Fuentes.Keys);
            var iniciosPalabrasClave = new List<string> { "#", "is" };
       
            var coincidenciasClases = Regex.Matches(textoMinúscula, @"([áéíóúüa-z0-9_-]+)=<(.+?)>"); // Primero extrae las coincidencias de clases de formato y las elimina del texto.
            if (coincidenciasClases.Count > 0) {
                foreach (Match? coincidenciasClase in coincidenciasClases) {
                    if (coincidenciasClase != null && coincidenciasClase.Success) {

                        var nombreClase = coincidenciasClase.Groups[1].Value;
                        var textoFormatosClase = coincidenciasClase.Groups[2].Value;
                        if (palabrasClave.Contains(nombreClase) || iniciosPalabrasClave.Any(i => nombreClase.StartsWith(i))) {
                            MostrarError($"The format class name {nombreClase} is not allowed because it can't have the same name as a keyword.");
                        } else {

                            if (clasesLeídas.ContainsKey(nombreClase)) {
                                MostrarError($"The format class name {nombreClase} is not allowed because it was already added.");
                            } else {
                                clasesLeídas.Add(nombreClase, new Formato(textoFormatosClase, out _, null));
                                textoMinúscula = textoMinúscula.Replace(coincidenciasClase.Groups[0].Value, "");
                            }

                        }

                    }
                }
            }
           
            var valores = textoMinúscula.Split(" ").ToList() ?? new List<string>();
            var formatosDeClases = new List<Formato>();

            foreach (var valor in valores) {

                if (string.IsNullOrEmpty(valor)) continue;

                var valorIdentificado = false;
                if (Estilos.Contains(valor)) {

                    switch (valor) {
                        case "b":
                            if (Preferencias.OverrideFontBold) Negrita = true;  
                            break;
                        case "nb":
                            if (Preferencias.OverrideFontBold) Negrita = false;
                            break;
                        case "i":
                            if (Preferencias.OverrideFontItalics) Cursiva = true;
                            break;
                        case "u":
                            if (Preferencias.OverrideFontUnderline) Subrayado = true;
                            break;
                        default:
                            MostrarError($"To Developer: The value '{valor}' wasn't expected in Formato().");
                            break;
                    }
                    valorIdentificado = true;

                }

                if (Tamaños.Contains(valor)) {
                    if (Preferencias.OverrideFontSize) TamañoFuente = (TamañosFuente)Enum.Parse(typeof(TamañosFuente), valor.ToUpperInvariant());
                    valorIdentificado = true;
                }    

                if (Posiciones.Contains(valor)) {

                    if (Preferencias.OverrideFontPosition) {
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
                    }
                    valorIdentificado = true;

                }

                if (Colores.Contains(valor) || valor.StartsWith("#")) {

                    if (Preferencias.OverrideFontColor) {
                        Color = ObtenerMediaColor(valor);
                        if (Color == null) MostrarError($"Color '{valor}' isn't valid.");
                    }
                    valorIdentificado = true;

                }

                if (Fuentes.Keys.Contains(valor)) {
                    if (Preferencias.OverrideFontName) NombreFuente = Fuentes[valor].Nombre;
                    valorIdentificado = true;
                }

                if (valor.StartsWith("is")) {

                    var coincidencias = Regex.Matches(valor, "is([0-9]+)");
                    if (coincidencias.Count == 1) {

                        if (Preferencias.OverrideImageSize) TamañoImagen = double.Parse(coincidencias[0].Groups[1].Value);
                        valorIdentificado = true;

                    } else {
                        MostrarError($"The format keyword '{valor}' isn't supported.");
                    }

                }

                if (clases != null && clases.ContainsKey(valor)) {
                    formatosDeClases.Add(clases[valor]);
                    valorIdentificado = true;
                }  

                if (!valorIdentificado) MostrarError($"The format keyword '{valor}' isn't supported.");             

            }

            foreach (var formatoDeClase in formatosDeClases) {
                CopiarPropiedadesEnNulas(formatoDeClase, this);
            }

        } // Formato>


        #endregion Constructores>



        #region Funciones y Procedimientos


        public static void CopiarPropiedadesEnNulas(Formato origen, Formato destino) { // Copia las propiedades en formatoDestino están nulas.

            destino.Negrita ??= origen.Negrita;
            destino.Subrayado ??= origen.Subrayado;
            destino.Cursiva ??= origen.Cursiva;
            destino.NombreFuente ??= origen.NombreFuente;
            destino.Color ??= origen.Color;
            if (destino.TamañoFuente == TamañosFuente.Indeterminado) destino.TamañoFuente = origen.TamañoFuente;
            if (destino.Posición == PosiciónTexto.Indeterminado) destino.Posición = origen.Posición;
            destino.TamañoImagen ??= origen.TamañoImagen;
            destino.TamañoBaseFuente ??= origen.TamañoBaseFuente;

        } // CopiarPropiedadesEnNulas>


        public static Formato ObtenerFormatoEfectivo(Formato? formatoHijo, Formato formatoPadre) {

            var formato = new Formato();
            formato.Negrita = (formatoHijo?.Negrita ?? formatoPadre.Negrita) ?? CurrentStepFontBoldPredeterminado;
            formato.Subrayado = (formatoHijo?.Subrayado ?? formatoPadre.Subrayado) ?? false;
            formato.Cursiva = (formatoHijo?.Cursiva ?? formatoPadre.Cursiva) ?? false;
            formato.NombreFuente = (formatoHijo?.NombreFuente ?? formatoPadre.NombreFuente) ?? NombreFuentePredeterminada;
            formato.Color = (formatoHijo?.Color ?? formatoPadre.Color) ?? ObtenerMediaColor(CurrentStepFontColorPredeterminado);
            formato.TamañoFuente = formatoHijo?.TamañoFuente == TamañosFuente.Indeterminado ? formatoPadre.TamañoFuente 
                : (formatoHijo != null ? formatoHijo.TamañoFuente : TamañosFuente.M);
            formato.Posición = formatoHijo?.Posición == PosiciónTexto.Indeterminado ? formatoPadre.Posición 
                : (formatoHijo != null ? formatoHijo.Posición : PosiciónTexto.Normal);
            formato.TamañoImagen = (formatoHijo?.TamañoImagen ?? formatoPadre.TamañoImagen) ?? ImageSizePredeterminado;

            if (formatoHijo?.TamañoBaseFuente != null && formatoPadre.TamañoBaseFuente != null
                && formatoHijo.TamañoBaseFuente != formatoPadre.TamañoBaseFuente)
                MostrarError("To Developer: TamañoBaseFuente can't be different in formatoHijo and formatoPadre.");

            formato.TamañoBaseFuente = formatoPadre.TamañoBaseFuente ?? formatoHijo?.TamañoBaseFuente ?? null; // El TamañoBaseFuente es la única propiedad en la que se le da prioridad al formatoPadre porque es donde usualmente se asigna y el que dicta el tamaño general de todo el texto. Tener varios segmentos con diferentes tamaño fuente base no tiene mucho sentido porque se pierde la funcionalidad de los tamaños relativos.
   
            return formato;

        } // ObtenerFormatoEfectivo>


        public double? TamañoFuenteEfectiva
            => TamañoBaseFuente == null ? null : TamañoFuente switch {
                TamañosFuente.XXXL => 2D * TamañoBaseFuente,
                TamañosFuente.XXL => 1.5D * TamañoBaseFuente,
                TamañosFuente.XL => 1.3D * TamañoBaseFuente,
                TamañosFuente.L => 1.15D * TamañoBaseFuente,
                TamañosFuente.M => 1 * TamañoBaseFuente, 
                TamañosFuente.S => (1D / 1.2) * TamañoBaseFuente,
                TamañosFuente.XS => (1D / 1.5) * TamañoBaseFuente,
                TamañosFuente.XXS => (1D / 2) * TamañoBaseFuente,
                TamañosFuente.XXXS => (1D / 3) * TamañoBaseFuente,
                TamañosFuente.Indeterminado => null
            };


        public string? ColorHexadecimal => Color == null ? null : Color.ToString();

        public double? TamañoTextoEfectivo => TamañoFuenteEfectiva * FactorTamañoTextoAPixeles;

        public double? ObtenerTamañoImagenEfectiva(double imageSize) => TamañoTextoEfectivo * (imageSize / 100);


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
