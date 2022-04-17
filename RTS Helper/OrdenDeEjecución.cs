using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static RTSHelper.Global;
using System.Windows.Media;
using static Vixark.General;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Diagnostics;
using System.IO;



namespace RTSHelper {



    public class OrdenDeEjecución {



        #region Propiedades

        public List<Paso> Pasos { get; set; } = new List<Paso>();

        public Paso? Introducción { get; set; } = null;

        private int _NúmeroPaso = 0;
        public int NúmeroPaso {

            get { return _NúmeroPaso; }
            set {

                var hacerCambioNúmeroPaso = _NúmeroPaso != value;
                _NúmeroPaso = value;
                if (hacerCambioNúmeroPaso && EnCambioNúmeroPaso != null) EnCambioNúmeroPaso(); // Se hace aquí para que los procedimientos de la acción ya tengan del nuevo valor del paso.

            } 

        }

        public Dictionary<string, Comportamiento> ClasesDeComportamientos { get; set; } = new Dictionary<string, Comportamiento>();

        public Dictionary<string, Formato> ClasesDeFormatos { get; set; } = new Dictionary<string, Formato>();

        public Action? EnCambioNúmeroPaso;

        #endregion Propiedades>



        #region Propiedades Autocalculadas

        public bool EsÚltimoPaso => NúmeroPaso == Pasos.Count - 1;

        public bool EsPasoDespuésDeÚltimo => NúmeroPaso - 1 == Pasos.Count - 1;

        public bool EsDespuésDeÚltimoPaso => NúmeroPaso > Pasos.Count - 1;

        #endregion Propiedades Autocalculadas>



        #region Funciones y Procedimientos


        public bool CargarPasos(string directorioBuildOrders, string nombreBuildOrder, out string? errores, out string? rutaBuildOrder) {

            errores = null;
            var pasos = new List<Paso>();
            rutaBuildOrder = Path.Combine(directorioBuildOrders, $"{nombreBuildOrder}.txt");
            if (!Directory.Exists(directorioBuildOrders)) Directory.CreateDirectory(directorioBuildOrders);
            if (!File.Exists(rutaBuildOrder)) return false;
            if (!ObtenerArchivoLibre(rutaBuildOrder)) return true; // Si pasado el tiempo máximo permitido el archivo no está libre, no carga los pasos.

            var textosPasos = File.ReadAllLines(rutaBuildOrder);
            Formato? formatoGlobal = null;
            Comportamiento? comportamientoGlobal = null;
            var grupos = new Dictionary<string, string>(); // Almacena el nombre de un nombre grupo y su contenido. El contenido será reemplazado directamente en el lugar que se use el nombre del grupo.
            var númeroPaso = 0;
            
            foreach (var textoPaso in textosPasos) {

                var textoPasoSinComentarios = EliminarComentarios(textoPaso) ?? "";
                var textoPasoTrimmed = textoPasoSinComentarios.Trim();

                if (textoPasoTrimmed.StartsWith("<<") && textoPasoTrimmed.EndsWith(">>")) { // Es necesario hacerlo así y no con Regex por facilidad y para que no entre en conflicto con las clases de comportamientos que van entre <>.

                    formatoGlobal = new Formato(textoPasoTrimmed[2..^2], out Dictionary<string, Formato> clasesLeídas, null, out string? erroresInternos
                        , -4);
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
                    ClasesDeFormatos = clasesLeídas;

                } else if (textoPasoTrimmed.StartsWith("{{") && textoPasoTrimmed.EndsWith("}}")) { // Es necesario hacerlo así y no con Regex por facilidad y para que no entre en conflicto con las clases de comportamientos que van entre {}.

                    comportamientoGlobal = new Comportamiento(textoPasoTrimmed[2..^2], out Dictionary<string, Comportamiento> clasesLeídas, null, 
                        out string? erroresInternos, -3);
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
                    ClasesDeComportamientos = clasesLeídas;

                } else if (textoPasoTrimmed.StartsWith("((") && textoPasoTrimmed.EndsWith("))")) { // Es necesario hacerlo así y no con Regex por facilidad.

                    Introducción = new Paso(textoPasoTrimmed[2..^2], comportamientoGlobal, formatoGlobal, ClasesDeFormatos, ClasesDeComportamientos,
                        grupos, out string ? erroresInternos, -2);
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

                } else if (textoPasoTrimmed.StartsWith("[[") && textoPasoTrimmed.EndsWith("]]")) { // Es necesario hacerlo así y no con Regex por facilidad y para que no entre en conflicto con las clases de comportamientos que van entre [].

                    var textoGrupos = textoPasoTrimmed[2..^1]; // No recorta el último corchete porque este se ignorará cuando se agregue el contenido del último grupo.
                    var estado = "n"; // Puede ser n: en nombre, c: contenido.
                    var profundidadCorchetes = 0;
                    var nombreGrupoActual = new StringBuilder();
                    var contenidoGrupoActual = new StringBuilder();
                    var cuentaCarácteres = 0;

                    foreach (var carácter in textoGrupos) { // Se hace de esta manera y no con regex porque regex no es capaz de controlar adecuadamente los múltiples [] dentro de estos textos.

                        if (carácter == '=') {
                            estado = "c";
                        } else if ((carácter == ' ' && profundidadCorchetes == 0) || cuentaCarácteres == textoGrupos.Length - 1) {

                            estado = "n";
                            var nombre = nombreGrupoActual.ToString();
                            if (!string.IsNullOrWhiteSpace(nombre)) {

                                if (ObtenerEntidad(nombre) != null) {
                                    AgregarErrores(ref errores, $"{nombre} can't be used as a group name because it's already an entity name.", -1);
                                } else {

                                    if (grupos.ContainsKey(nombre)) {
                                        AgregarErrores(ref errores, $"You can't add two groups with the same name: {nombre}.", -1);
                                    } else {
                                        grupos.Add(nombre, contenidoGrupoActual.ToString());
                                    }
                                
                                }

                            }

                            nombreGrupoActual = new StringBuilder();
                            contenidoGrupoActual = new StringBuilder();

                        } else {

                            var agregarContenido = true; // Evita que se agreguen los corchetes de inicio y del final.
                            if (carácter == '[') {
                                agregarContenido = profundidadCorchetes != 0;
                                profundidadCorchetes++;
                            } else if (carácter == ']') {
                                profundidadCorchetes--;
                                agregarContenido = profundidadCorchetes != 0;
                            }

                            if (estado == "c" && agregarContenido) {
                                contenidoGrupoActual.Append(carácter);
                            } else if (estado == "n") {
                                nombreGrupoActual.Append(carácter);
                            }

                        }
                        cuentaCarácteres++;

                    }

                } else if (!string.IsNullOrWhiteSpace(textoPasoTrimmed)) { // No se agrega el paso si es un espacio en la build order. Estos espacios son útiles para tener más orden de edición.

                    pasos.Add(new Paso(textoPasoTrimmed, comportamientoGlobal, formatoGlobal, ClasesDeFormatos, ClasesDeComportamientos, grupos, 
                        out string? erroresInternos, númeroPaso));
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
                    númeroPaso++;

                }

            }

            if (pasos.Count == 0) {
                pasos.Add(new Paso("", null, null, null, null, new Dictionary<string, string>(), out string? erroresInternos, 0)); // Para poder almacenar la duración de los pasos, debe haber como mínimo un paso.
                AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
            }
                
            if (NúmeroPaso > 0) { // Si se carga una build order en la mitad de la ejecución, debe copiar las duraciones de los pasos de la ejecución actual.
                for (int i = 0; i < NúmeroPaso; i++) {
                    if (i <= pasos.Count - 1 && i <= Pasos.Count - 1) pasos[i].DuraciónEnJuego = Pasos[i].DuraciónEnJuego;
                }
            }
            Pasos = pasos;

            return true;

        } // CargarPasos>


        public string? EliminarComentarios(string? texto) {

            if (texto is null) return null;
            if (!texto.Contains("||")) return texto;
            var textoSinComentarios = new StringBuilder();
            var índiceActual = 0;
            var índiceAnterior = 0;
            var esComentario = texto.StartsWith("||");
            var inicioTexto = true;

            do {

                índiceAnterior = índiceActual;
                índiceActual = texto.IndexOf("||", índiceAnterior + 2);
                var índiceInicial = inicioTexto ? 0 : índiceAnterior + 2;
                if (!esComentario)
                    textoSinComentarios.Append(índiceActual == -1 ? texto[índiceInicial..^0] : texto[índiceInicial..índiceActual]);
                esComentario = !esComentario;
                inicioTexto = false;

            } while (índiceActual != -1);

            return textoSinComentarios.ToString();

        } // EliminarComentarios>


        public void MostrarPaso(int? númeroPaso, Formato formatoPredeterminado, StackPanel contenedor, bool mostrarSiempreÚltimoPaso, double altoMáximo,
            HorizontalAlignment alineaciónHorizontal, double margenInferior, out bool superóAlto, out string? errores) {

            superóAlto = false;
            var númeroPasoAMostrar = númeroPaso;
            errores = null;
            if (númeroPaso >= Pasos.Count) {

                if (mostrarSiempreÚltimoPaso) {
                    númeroPasoAMostrar = Pasos.Count > 0 ? Pasos.Count - 1 : (int?)null;
                } else {
                    númeroPasoAMostrar = null;
                }

            }

            Paso? paso;
            if (númeroPaso == null || númeroPaso < 0) {
                paso = Introducción;
            } else if (númeroPasoAMostrar == null) {
                paso = null; // No muestra nada. Es el el siguiente paso después del último.     
            } else {
                paso = Pasos[(int)númeroPasoAMostrar];
            }

            if (paso != null) {

                var sumaAlto = 0D;
                foreach (var instrucción in paso.Instrucciones) {

                    var mayorEspaciadoVertical = 0D;
                    var altoInstrucción = 0D;
                    var spnHorizontal = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = alineaciónHorizontal };

                    foreach (var segmento in instrucción.Segmentos) {

                        var formato = Formato.ObtenerFormatoEfectivo(segmento.Formato, formatoPredeterminado, out string? erroresInternos, númeroPaso, 
                            forzarValores: true);
                        AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

                        if (formato.Negrita == null || formato.Cursiva == null || formato.NombreFuente == null || formato.Subrayado == null
                            || formato.TamañoFuenteEfectiva == null || formato.Color == null || formato.ColorHexadecimal == null ||
                            formato.TamañoImagen == null || formato.ObtenerTamañoImagenEfectiva((double)formato.TamañoImagen) == null) {
                            AgregarErrores(ref errores, "To Developer: Unespected null value in formato in MostrarPaso().", númeroPaso);
                            continue;
                        }

                        var altoSegmentoTexto = (double)formato.TamañoFuenteEfectiva * FactorTamañoTextoAPixeles;
                        var altoSegmentoImagen = 0D;

                        var espaciadoVertical = altoSegmentoTexto * (Preferencias.LineSpacing / 100);
                        if (espaciadoVertical > mayorEspaciadoVertical) mayorEspaciadoVertical = espaciadoVertical;

                        Segmento segmentoEfectivo;
                        var margenEnTexto = false;
                        if (segmento.Tipo == TipoSegmento.Entidad) {

                            var entidad = ObtenerEntidad(segmento.Texto);
                            if (entidad == null) {

                                AgregarErrores(ref errores, $"The name {segmento.Texto} wasn't found.", númeroPaso);
                                segmentoEfectivo = segmento.Clonar(out string? erroresInternos2, númeroPaso);
                                AgregarErrores(ref errores, erroresInternos2, númeroPaso: null);
                                segmentoEfectivo.Tipo = TipoSegmento.Texto;
                                margenEnTexto = false;

                            } else {

                                segmentoEfectivo = ObtenerSegmentoEfectivo(entidad, out string? erroresInternos2, númeroPaso);
                                margenEnTexto = segmentoEfectivo.Tipo != TipoSegmento.Imagen; // Cuando se usa el marcado con corchetes cuadrados por ejemplo, [attack][town center] se espera que las entidades no sean adjuntas si están en texto, es decir, se espera que no se muestre ATTKTC, si no ATT TC. Por esto se debe agregar este margen adicional cuando la entidad no se vaya a mostrar como imagen.
                                AgregarErrores(ref errores, erroresInternos2, númeroPaso: null);

                            }

                        } else {

                            segmentoEfectivo = segmento.Clonar(out string? erroresInternos2, númeroPaso);
                            AgregarErrores(ref errores, erroresInternos2, númeroPaso: null);
                            margenEnTexto = false; // El texto normal no lleva margen.

                        }

                        if (segmentoEfectivo.Tipo == TipoSegmento.Texto) {

                            var textBlock = ObtenerTextBlock(segmentoEfectivo.Texto);
                            textBlock.FontStyle = (bool)formato.Cursiva ? FontStyles.Italic : FontStyles.Normal;
                            textBlock.FontWeight = (bool)formato.Negrita ? FontWeights.Bold : FontWeights.Normal;
                            textBlock.FontFamily = new FontFamily((string)formato.NombreFuente);
                            textBlock.FontSize = (double)formato.TamañoFuenteEfectiva;
                            textBlock.VerticalAlignment = VerticalAlignment.Center; // Es preferible el alineamiento central para facilitar el posicionamiento de las imágenes. Además, el alineamiento inferior no coincide exactamente con la línea base de los textos cuando se usan diferentes tamaños de fuente, entonces tampoco es muy útil. 
                            textBlock.Foreground = ObtenerBrocha(formato.ColorHexadecimal);

                            var margenTexto = (margenEnTexto ? formato.TamañoTextoEfectivo * (Preferencias.EntityHorizontalMargin / 200) : 0) ?? 0;
                            textBlock.Margin = new Thickness(margenTexto, 0, margenTexto, 0);

                            if ((bool)formato.Subrayado) textBlock.TextDecorations = TextDecorations.Underline;

                            if (formato.Posición != PosiciónTexto.Normal) {

                                textBlock.FontFamily = Formato.ObtenerFuentePosiciónEspecial(formato.Posición, segmentoEfectivo.Texto,
                                    formato.NombreFuente, out string? erroresInternos2, númeroPaso);
                                if (formato.Posición == PosiciónTexto.Superíndice) textBlock.Typography.Variants = FontVariants.Superscript;
                                if (formato.Posición == PosiciónTexto.Subíndice) textBlock.Typography.Variants = FontVariants.Subscript;
                                AgregarErrores(ref errores, erroresInternos2, númeroPaso: null);

                            }

                            spnHorizontal.Children.Add(textBlock);

                        } else if (segmentoEfectivo.Tipo == TipoSegmento.Imagen) {

                            var tamaño = (double)formato.ObtenerTamañoImagenEfectiva((double)formato.TamañoImagen)!;
                            altoSegmentoImagen = tamaño;
                            var margen = tamaño * (Preferencias.EntityHorizontalMargin / 200); // Se divide por 200 porque el margen se aplica a ambos lados.
                            var margenDerecha = margen;
                            var margenIzquierda = margen;
                            var alineación = VerticalAlignment.Center;
                            var radioEsquinas = tamaño * (Preferencias.ImageBackgroundRoundedCornersRadius / 100);
                            var radioEsquinasIzquierdas = radioEsquinas;
                            var radioEsquinasDerechas = radioEsquinas;

                            if (formato.Posición != PosiciónTexto.Normal) {

                                margenIzquierda = -margen;
                                margenDerecha = margen;
                                tamaño = (Preferencias.SubscriptAndSuperscriptImagesSize / 100) * tamaño;
                                radioEsquinasIzquierdas = 0;

                                if (formato.Posición == PosiciónTexto.Subíndice) {
                                    alineación = VerticalAlignment.Bottom;
                                } else if (formato.Posición == PosiciónTexto.Superíndice) {
                                    alineación = VerticalAlignment.Top;
                                }

                                if (spnHorizontal.Children.Count > 0 && spnHorizontal.Children[spnHorizontal.Children.Count - 1] is Grid gridAnterior
                                    && gridAnterior.Children[0] is Border borderAnterior) { // Ajusta las esquinas redondeadas de la imagen anterior para que sea integrada con la imagen subíndice o superíndice.

                                    var ambosÍndices = false;

                                    var radioAnterior = borderAnterior.CornerRadius.TopLeft;
                                    switch (gridAnterior.VerticalAlignment) {
                                        case VerticalAlignment.Top:

                                            if (formato.Posición == PosiciónTexto.Superíndice)
                                                borderAnterior.CornerRadius = new CornerRadius(radioAnterior, 0, 0, radioAnterior);
                                            if (formato.Posición == PosiciónTexto.Subíndice) ambosÍndices = true;

                                            break;

                                        case VerticalAlignment.Bottom:

                                            if (formato.Posición == PosiciónTexto.Subíndice)
                                                borderAnterior.CornerRadius = new CornerRadius(radioAnterior, 0, 0, radioAnterior);
                                            if (formato.Posición == PosiciónTexto.Superíndice) ambosÍndices = true;
                                            break;

                                        case VerticalAlignment.Center:
                                        case VerticalAlignment.Stretch:

                                            if (formato.Posición == PosiciónTexto.Subíndice) {
                                                borderAnterior.CornerRadius = new CornerRadius(radioAnterior, radioAnterior, 0, radioAnterior);
                                            } else if (formato.Posición == PosiciónTexto.Superíndice) {
                                                borderAnterior.CornerRadius = new CornerRadius(radioAnterior, 0, radioAnterior, radioAnterior);
                                            }
                                            break;

                                    }

                                    if (ambosÍndices) { // Se está usando tanto el superíndice como el subíndice. Se debe verificar un objeto aún más atrás para corregir los bordes del original.

                                        margenIzquierda = -margen - tamaño; // Para que quede justo abajo/arriba del otro super/subíndice.
                                        if (spnHorizontal.Children.Count > 1
                                            && spnHorizontal.Children[spnHorizontal.Children.Count - 2] is Grid gridAnterior2
                                            && gridAnterior2.Children[0] is Border borderAnterior2 &&
                                            gridAnterior2.VerticalAlignment == VerticalAlignment.Center) { // Solo se analiza el caso de imagen grande anterior a la anterior. Los otros no son casos usuales.

                                            var radioAnterior2 = borderAnterior2.CornerRadius.TopLeft;
                                            borderAnterior2.CornerRadius = new CornerRadius(radioAnterior2, 0, 0, radioAnterior2);

                                        }

                                    }

                                }

                            } // formato.Posición != PosiciónTexto.Normal>

                            var grid = new Grid { Margin = new Thickness(0), VerticalAlignment = alineación };
                            var border = new Border {
                                Height = tamaño, CornerRadius = new CornerRadius(radioEsquinasIzquierdas,
                                radioEsquinasDerechas, radioEsquinasDerechas, radioEsquinasIzquierdas),
                                Opacity = Preferencias.ImageBackgroundOpacity, Background = ObtenerBrocha(Preferencias.ImageBackgroundColor),
                                Margin = new Thickness(margenIzquierda, 0, margenDerecha, 0)
                            };
                            var image = new Image {
                                Source = ObtenerImagen(segmentoEfectivo.Texto), Height = tamaño,
                                Margin = new Thickness(margenIzquierda, 0, margenDerecha, 0)
                            }; // El texto del segmentoEfectivo es el nombre de la imagen.
                            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
                            grid.Children.Add(border);
                            grid.Children.Add(image);
                            spnHorizontal.Children.Add(grid);

                        } // segmentoEfectivo.Tipo == TipoSegmento.Imagen>

                        var altoSegmento = altoSegmentoTexto > altoSegmentoImagen ? altoSegmentoTexto : altoSegmentoImagen;
                        if (altoSegmento > altoInstrucción) altoInstrucción = altoSegmento;

                    } // foreach segmento>

                    altoInstrucción += mayorEspaciadoVertical;
                    sumaAlto += altoInstrucción;
                    if (sumaAlto > (altoMáximo + margenInferior)) superóAlto = true; // Para efectos de la alerta por contenido muy largo se tiene en cuenta el margen para que quede la alerta justo en el límite de ocultarse la primera parte inferior del último paso.
                    if (superóAlto) { foreach (var child in spnHorizontal.Children) { } } //if (child is TextBlock textblock) textblock.Foreground = ObtenerBrocha("#F00");  No borrar. Se usa para el desarrollo.
                    spnHorizontal.Margin = new Thickness(0, 0, 0, mayorEspaciadoVertical);
                    if (instrucción.AlinearInferiormente) {

                        var altoRestante = altoMáximo - sumaAlto;
                        if (altoRestante > 0) {
                            var spnRelleno = new StackPanel() { Height = altoRestante };
                            contenedor.Children.Add(spnRelleno);
                        }

                    }
                    contenedor.Children.Add(spnHorizontal);

                } // foreach instrucción>

            } // paso != null>

        } // MostrarPaso>


        public SolidColorBrush ObtenerBrocha(string colorHexadecimal) {

            if (!BrochasDeColorSólido.ContainsKey(colorHexadecimal))
                BrochasDeColorSólido.Add(colorHexadecimal, new SolidColorBrush((Color)ObtenerMediaColor(colorHexadecimal)!)); // Se almacena en diccionario para no duplicar el objeto en memoria.
            return BrochasDeColorSólido[colorHexadecimal];

        } // ObtenerBrocha>


        public BitmapImage? ObtenerImagen(string nombreImagen) {

            if (!Imágenes.ContainsKey(nombreImagen))
                return null;

            if (!Bitmaps.ContainsKey(nombreImagen))
                Bitmaps.Add(nombreImagen, new BitmapImage(new Uri(Imágenes[nombreImagen]))); // Se almacena en diccionario para no duplicar el objeto en memoria y para no generarlo cada vez pues se puede demorar alrededor de 50 ms.
            return Bitmaps[nombreImagen];

        } // ObtenerImagen>


        public TextBlock ObtenerTextBlock(string texto) {

            if (texto.Contains("http", StringComparison.InvariantCultureIgnoreCase)) {

                var textBlock = new TextBlock();
                var matches = Regex.Matches(texto, @"(https?:\/\/[^ :]+):*([^ :]*)");
                var índiceActual = 0;
                foreach (Match? match in matches) {
                    if (match != null) {

                        var índiceMatch = match.Index;
                        var enlace = new Hyperlink();
                        var url = match.Groups[1].Value;

                        if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)) {

                            enlace.NavigateUri = new Uri(url);
                            var textoEnlace = match.Groups[2].Value;
                            enlace.Inlines.Add(string.IsNullOrWhiteSpace(textoEnlace) ? url : textoEnlace);
                            enlace.RequestNavigate += Enlace_RequestNavigate;
                            var textoPlano = texto.Substring(índiceActual, índiceMatch - índiceActual);
                            textBlock.Inlines.Add(new Run(textoPlano));
                            textBlock.Inlines.Add(enlace);
                            índiceActual = índiceMatch + match.Value.Length;

                        }

                    }
                }
                if (índiceActual <= texto.Length) textBlock.Inlines.Add(new Run(texto.Substring(índiceActual)));
                return textBlock;

            } else {
                return new TextBlock { Text = texto };
            }

        } // ObtenerTextBlock>


        static void Enlace_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {

            try {
                Process.Start(new ProcessStartInfo(e.Uri.ToString()) { UseShellExecute = true });
            } catch { }

        } // Enlace_RequestNavigate>


        #endregion Funciones y Procedimientos>


    } // OrdenDeEjecución>



} // RTSHelper>
