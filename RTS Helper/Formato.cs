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

        public double? DesfaceHorizontal { get; set; }

        public double? DesfaceVertical { get; set; }

        public double? OpacidadFondoImagen { get; set; }

        #endregion Propiedades>



        #region Constructores


        public Formato() { }


        public Formato(string? código, out Dictionary<string, Formato> clasesLeídas, Dictionary<string, Formato>? clases, out string? errores, int? númeroPaso) {

            errores = null;
            clasesLeídas = new Dictionary<string, Formato>();
            if (código == null) return; // Si el texto es vacío, no contiene ni formatos ni clases.

            var códigoMinúscula = código.ToLowerInvariant();
            var palabrasClave = new List<string>();
            palabrasClave.AddRange(Estilos);
            palabrasClave.AddRange(Posiciones);
            palabrasClave.AddRange(Colores);
            palabrasClave.AddRange(Tamaños);
            palabrasClave.AddRange(Fuentes.Keys);
            palabrasClave.AddRange(new List<string> { "font1", "font2" });

            var iniciosPalabrasClave = new List<string> { "#", "is", "ho", "vo", "bo" };
       
            var coincidenciasClases = Regex.Matches(códigoMinúscula, @"([áéíóúüa-z0-9_-]+)=<(.+?)>"); // Primero extrae las coincidencias de clases de formato y las elimina del texto.
            if (coincidenciasClases.Count > 0) {
                foreach (Match? coincidenciasClase in coincidenciasClases) {
                    if (coincidenciasClase != null && coincidenciasClase.Success) {

                        var nombreClase = coincidenciasClase.Groups[1].Value;
                        var textoFormatosClase = coincidenciasClase.Groups[2].Value;
                        if (palabrasClave.Contains(nombreClase) || iniciosPalabrasClave.Any(i => nombreClase.StartsWith(i))) {
                            AgregarErrores(ref errores, $"The format class name {nombreClase} is not allowed because it can't have the same name as a " +
                                $"keyword.", númeroPaso);
                        } else {

                            if (clasesLeídas.ContainsKey(nombreClase)) {
                                AgregarErrores(ref errores, $"The format class name {nombreClase} is not allowed because it was already added.", númeroPaso);
                            } else {

                                clasesLeídas.Add(nombreClase, new Formato(textoFormatosClase, out _, null, out string? erroresInternos, númeroPaso));
                                códigoMinúscula = códigoMinúscula.Replace(coincidenciasClase.Groups[0].Value, "");
                                AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

                            }

                        }

                    }
                }
            }
           
            var valores = códigoMinúscula.Split(" ").ToList() ?? new List<string>();
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
                            AgregarErrores(ref errores, $"To Developer: The value '{valor}' wasn't expected in Formato().", númeroPaso);
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
                        if (Color == null) AgregarErrores(ref errores, $"Color '{valor}' isn't valid.", númeroPaso);
                    }
                    valorIdentificado = true;

                }

                if (Fuentes.Keys.Contains(valor) || valor == "font1" || valor == "font2") {

                    var valorAplicable = valor switch {
                        "font1" => NombreFuentePredeterminada.Replace(" ", "").ToLowerInvariant(),
                        "font2" => NombreFuenteSecundariaPredeterminada.Replace(" ", "").ToLowerInvariant(),
                        _ => valor };
                    if (Preferencias.OverrideFontName) NombreFuente = Fuentes[valorAplicable].Nombre;
                    valorIdentificado = true;

                }

                if (valor.StartsWith("is")) {

                    var coincidencias = Regex.Matches(valor, "is([0-9]+)");
                    if (coincidencias.Count == 1) {

                        if (Preferencias.OverrideImageSize) TamañoImagen = double.Parse(coincidencias[0].Groups[1].Value);
                        valorIdentificado = true;

                    } else {
                        AgregarErrores(ref errores, $"The format keyword '{valor}' isn't supported.", númeroPaso);
                    }

                }

                if (valor.StartsWith("ho")) {

                    var coincidencias = Regex.Matches(valor, "ho(-?[0-9]+)");
                    if (coincidencias.Count == 1) {

                        DesfaceHorizontal = double.Parse(coincidencias[0].Groups[1].Value);
                        valorIdentificado = true;

                    } else {
                        AgregarErrores(ref errores, $"The format keyword '{valor}' isn't supported.", númeroPaso);
                    }

                }

                if (valor.StartsWith("vo")) {

                    var coincidencias = Regex.Matches(valor, "vo(-?[0-9]+)");
                    if (coincidencias.Count == 1) {

                        DesfaceVertical = double.Parse(coincidencias[0].Groups[1].Value);
                        valorIdentificado = true;

                    } else {
                        AgregarErrores(ref errores, $"The format keyword '{valor}' isn't supported.", númeroPaso);
                    }

                }

                if (valor.StartsWith("bo")) {

                    var coincidencias = Regex.Matches(valor, "bo([0-9]+)");
                    if (coincidencias.Count == 1) {

                        OpacidadFondoImagen = double.Parse(coincidencias[0].Groups[1].Value)/100;
                        valorIdentificado = true;

                    } else {
                        AgregarErrores(ref errores, $"The format keyword '{valor}' isn't supported.", númeroPaso);
                    }

                }

                if (clases != null && clases.ContainsKey(valor)) {
                    formatosDeClases.Add(clases[valor]);
                    valorIdentificado = true;
                }  

                if (!valorIdentificado) AgregarErrores(ref errores, $"The format keyword '{valor}' isn't supported.", númeroPaso);

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
            destino.DesfaceHorizontal ??= origen.DesfaceHorizontal;
            destino.DesfaceVertical ??= origen.DesfaceVertical;
            destino.OpacidadFondoImagen ??= origen.OpacidadFondoImagen;

        } // CopiarPropiedadesEnNulas>


        public static Formato ObtenerFormatoEfectivo(Formato? formatoHijo, Formato formatoPadre, out string? errores, int? númeroPaso, bool forzarValores) {

            errores = null;
            var formato = new Formato();

            if (forzarValores) {

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
                formato.DesfaceHorizontal = (formatoHijo?.DesfaceHorizontal ?? formatoPadre.DesfaceHorizontal) ?? 0;
                formato.DesfaceVertical = (formatoHijo?.DesfaceVertical ?? formatoPadre.DesfaceVertical) ?? 0;
                formato.OpacidadFondoImagen = (formatoHijo?.OpacidadFondoImagen ?? formatoPadre.OpacidadFondoImagen) ?? Preferencias.ImageBackgroundOpacity;
                
                if (formatoHijo?.TamañoBaseFuente != null && formatoPadre.TamañoBaseFuente != null
                    && formatoHijo.TamañoBaseFuente != formatoPadre.TamañoBaseFuente)
                    AgregarErrores(ref errores, "To Developer: TamañoBaseFuente can't be different in formatoHijo and formatoPadre.", númeroPaso);

            } else {

                formato.Negrita = formatoHijo?.Negrita ?? formatoPadre.Negrita;
                formato.Subrayado = formatoHijo?.Subrayado ?? formatoPadre.Subrayado;
                formato.Cursiva = formatoHijo?.Cursiva ?? formatoPadre.Cursiva;
                formato.NombreFuente = formatoHijo?.NombreFuente ?? formatoPadre.NombreFuente;
                formato.Color = formatoHijo?.Color ?? formatoPadre.Color;
                formato.TamañoFuente = formatoHijo?.TamañoFuente == TamañosFuente.Indeterminado ? formatoPadre.TamañoFuente
                    : (formatoHijo != null ? formatoHijo.TamañoFuente : TamañosFuente.Indeterminado);
                formato.Posición = formatoHijo?.Posición == PosiciónTexto.Indeterminado ? formatoPadre.Posición
                    : (formatoHijo != null ? formatoHijo.Posición : PosiciónTexto.Indeterminado);
                formato.TamañoImagen = formatoHijo?.TamañoImagen ?? formatoPadre.TamañoImagen;
                formato.DesfaceHorizontal = formatoHijo?.DesfaceHorizontal ?? formatoPadre.DesfaceHorizontal;
                formato.DesfaceVertical = formatoHijo?.DesfaceVertical ?? formatoPadre.DesfaceVertical;
                formato.OpacidadFondoImagen = formatoHijo?.OpacidadFondoImagen ?? formatoPadre.OpacidadFondoImagen;

                if (formatoHijo?.TamañoBaseFuente != null && formatoPadre.TamañoBaseFuente != null
                    && formatoHijo.TamañoBaseFuente != formatoPadre.TamañoBaseFuente)
                    AgregarErrores(ref errores, "To Developer: TamañoBaseFuente can't be different in formatoHijo and formatoPadre.", númeroPaso);

            }

            formato.TamañoBaseFuente = formatoPadre.TamañoBaseFuente ?? formatoHijo?.TamañoBaseFuente ?? null; // El TamañoBaseFuente es la única propiedad en la que se le da prioridad al formatoPadre porque es donde usualmente se asigna y el que dicta el tamaño general de todo el texto. Tener varios segmentos con diferentes tamaño fuente base no tiene mucho sentido porque se pierde la funcionalidad de los tamaños relativos.

            return formato;

        } // ObtenerFormatoEfectivo>


        public double? TamañoFuenteEfectiva => TamañoBaseFuente == null ? null : ObtenerFactorTamañoFuente(TamañoFuente) * TamañoBaseFuente;

        public string? ColorHexadecimal => Color == null ? null : Color.ToString();

        public double? TamañoTextoEfectivo => TamañoFuenteEfectiva * FactorTamañoTextoAPixelesFuentePredeterminada;

        public double? ObtenerTamañoImagenEfectiva(double tamañoImagen) => TamañoTextoEfectivo * (tamañoImagen / 100);


        public static FontFamily ObtenerFuentePosiciónEspecial(PosiciónTexto posición, string texto, string nombreFuente, out string? errores, 
            int? númeroPaso) {

            errores = null;
            var tipo = Regex.IsMatch(texto, "^[0-9 ]+$") ? "num" : "alfanum"; // En Global.cs se buscó false, .*, true, .* y no hubo resultados. Esto significa que no hay fuentes que tengan superíndice letras y que no tengan superíndice números. Es decir el superíndice es alfanumérico o solo numérico. Igual para el subíndice.
            var palatinoLinotype = new FontFamily("Palatino Linotype");
            var fuente = Fuentes.FirstOrDefault(f => f.Value.Nombre == nombreFuente).Value;
            if (fuente == null) return palatinoLinotype;
            var fuenteCumpleRequerimiento = false;
            var requerimiento = "";

            switch (posición) {
                case PosiciónTexto.Indeterminado:
                    AgregarErrores(ref errores, "To Developer: Indeterminado is unexpected in ObtenerFuentePosiciónEspecial()", númeroPaso);
                    break;
                case PosiciónTexto.Normal:
                    return new FontFamily(fuente.Nombre);
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
                    AgregarErrores(ref errores, "To Developer: default is unexpected in ObtenerFuentePosiciónEspecial()", númeroPaso);
                    break;
            }

            return palatinoLinotype;

        } // ObtenerFuentePosiciónEspecial>


        #endregion Funciones y Procedimientos>



    } // Formato>



} // RTSHelper>
