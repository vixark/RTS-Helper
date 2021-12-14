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



namespace RTSHelper {



    public class OrdenDeEjecución {


        #region Propiedades

        public List<Paso> Pasos = new List<Paso>();

        public int NúmeroPaso = 0;

        public bool EsÚltimoPaso => NúmeroPaso == Pasos.Count - 1;

        public bool EsDespuésDeÚltimoPaso => NúmeroPaso > Pasos.Count - 1;

        public Comportamiento Comportamiento { get; set; } = new Comportamiento();

        #endregion Propiedades>


        #region Funciones y Procedimientos


        public void MostrarPaso(int? númeroPaso, Formato formatoPredeterminado, StackPanel contenedor, bool mostrarSiempreÚltimoPaso, double altoMáximo,
            HorizontalAlignment alineaciónHorizontal, double márgenInferior, out bool superóAlto) {

            superóAlto = false;
            var númeroPasoAMostrar = númeroPaso;
            var errores = "";
            if (númeroPaso >= Pasos.Count) {

                if (mostrarSiempreÚltimoPaso) {
                    númeroPasoAMostrar = Pasos.Count > 0 ? Pasos.Count - 1 : (int?)null;
                } else {
                    númeroPasoAMostrar = null;
                }

            }

            if (númeroPasoAMostrar == null) {

                var textBlock = new TextBlock { Text = "Pendiente implementar para mostrar información general de la build order." };
                contenedor.Children.Add(textBlock);

            } else {

                var paso = Pasos[(int)númeroPasoAMostrar];
                var sumaAlto = 0D;

                if (paso != null) {
     
                    foreach (var instrucción in paso.Instrucciones) {

                        var mayorEspaciadoVertical = 0D;
                        var altoInstrucción = 0D;
                        var spnHorizontal = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = alineaciónHorizontal };

                        foreach (var segmento in instrucción.Segmentos) {

                            var formato = Formato.ObtenerFormatoEfectivo(segmento.Formato, formatoPredeterminado);
                            if (formato.Negrita == null || formato.Cursiva == null || formato.NombreFuente == null || formato.Subrayado == null
                                || formato.TamañoFuenteEfectiva == null || formato.Color == null || formato.ColorHexadecimal == null ||
                                formato.ImageSize == null || formato.ObtenerTamañoImagenEfectiva((double)formato.ImageSize) == null) {
                                errores += "To Developer: Unespected null value in formato in MostrarPaso()." + Environment.NewLine;
                                continue;
                            }

                            var altoSegmentoTexto = (double)formato.TamañoFuenteEfectiva * FactorTamañoTextoAPixeles;
                            var altoSegmentoImagen = 0D;

                            var espaciadoVertical = altoSegmentoTexto * (Preferencias.LineSpacing / 100);
                            if (espaciadoVertical > mayorEspaciadoVertical) mayorEspaciadoVertical = espaciadoVertical;
                                
                            Segmento segmentoEfectivo;
                            if (segmento.Tipo == TipoSegmento.Entidad) {

                                var entidad = ObtenerEntidad(segmento.Texto);
                                if (entidad == null) {

                                    errores += $"The name {segmento.Texto} wasn't found." + Environment.NewLine;
                                    segmentoEfectivo = segmento.Clonar();
                                    segmentoEfectivo.Tipo = TipoSegmento.Texto;

                                } else {
                                    segmentoEfectivo = ObtenerSegmentoEfectivo(entidad);
                                }
                                    
                            } else {
                                segmentoEfectivo = segmento.Clonar();
                            }

                            if (segmentoEfectivo.Tipo == TipoSegmento.Texto) {

                                var textBlock = new TextBlock {
                                    Text = segmentoEfectivo.Texto,
                                    FontStyle = (bool)formato.Cursiva ? FontStyles.Italic : FontStyles.Normal,
                                    FontWeight = (bool)formato.Negrita ? FontWeights.Bold : FontWeights.Normal,
                                    FontFamily = new FontFamily((string)formato.NombreFuente),
                                    FontSize = (double)formato.TamañoFuenteEfectiva,
                                    VerticalAlignment = VerticalAlignment.Center, // Es preferible el alineamiento central para facilitar el posicionamiento de las imágenes. Además, el alineamiento inferior no coincide exactamente con la línea base de los textos cuando se usan diferentes tamaños de fuente, entonces tampoco es muy útil. 
                                };

                                textBlock.Foreground = ObtenerBrocha(formato.ColorHexadecimal);
                              
                                if ((bool)formato.Subrayado) textBlock.TextDecorations = TextDecorations.Underline;

                                if (formato.Posición != PosiciónTexto.Normal) {
                                    textBlock.FontFamily = Formato.ObtenerFuentePosiciónEspecial(formato.Posición, segmentoEfectivo.Texto, 
                                        formato.NombreFuente);
                                    if (formato.Posición == PosiciónTexto.Superíndice) textBlock.Typography.Variants = FontVariants.Superscript;
                                    if (formato.Posición == PosiciónTexto.Subíndice) textBlock.Typography.Variants = FontVariants.Subscript;
                                }

                                spnHorizontal.Children.Add(textBlock);

                            } else if (segmentoEfectivo.Tipo == TipoSegmento.Imagen) {

                                var tamaño = (double)formato.ObtenerTamañoImagenEfectiva((double)formato.ImageSize)!;
                                altoSegmentoImagen = tamaño;
                                var margen = tamaño * (Preferencias.ImageHorizontalMargin / 200); // Se divide por 200 porque el márgen se aplica a ambos lados.
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
                                var border = new Border { Height = tamaño, CornerRadius = new CornerRadius(radioEsquinasIzquierdas, 
                                    radioEsquinasDerechas, radioEsquinasDerechas, radioEsquinasIzquierdas), 
                                    Opacity = Preferencias.ImageBackgroundOpacity, Background = ObtenerBrocha(Preferencias.ImageBackgroundColor),
                                    Margin = new Thickness(margenIzquierda, 0, margenDerecha, 0) 
                                };
                                var image = new Image { Source = ObtenerImagen(segmentoEfectivo.Texto), Height = tamaño,
                                    Margin = new Thickness(margenIzquierda, 0, margenDerecha, 0) }; // El texto del segmentoEfectivo es el nombre de la imagen.
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
                        if (sumaAlto > (altoMáximo + márgenInferior)) superóAlto = true; // Para efectos de la alerta por contenido muy largo se tiene en cuenta el margen para que quede la alerta justo en el límite de ocultarse la primera parte inferior del último paso.
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

            } // númeroPasoAMostrar != null>

            if (!string.IsNullOrEmpty(errores)) MostrarError(errores);

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


        #endregion Funciones y Procedimientos>


    } // OrdenDeEjecución>



} // RTSHelper>
