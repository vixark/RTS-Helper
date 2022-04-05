using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using static Vixark.General;
using System.Drawing.Drawing2D;



namespace RTSHelper {



    public class CapturaDePantalla { 


        public static void Guardar(string ruta, RectangleF rectánguloFraccional, ParámetrosExtracción parámetros) {

            var bmp = ObtenerBitmap(rectánguloFraccional, parámetros.Negativo, parámetros.BlancoYNegro, parámetros.Escala, parámetros.Luminosidad, 
                parámetros.Contraste, parámetros.ModoInterpolación, parámetros.FormatoPixeles);
            bmp.Save(ruta, ImageFormat.Bmp);

        } // Guardar>


        public static Bitmap ObtenerBitmap(RectangleF rectánguloFraccional, bool negativo, bool blancoYNegro, double escala, float luminosidad, 
            float contraste, InterpolationMode modoInterpolación, PixelFormat formatoPíxeles) {

            int redondear(float f) => (int)Math.Round(f, 0);
            
            var límites = Screen.GetBounds(Point.Empty);
            var límitesAplicable = new Rectangle(redondear(límites.Width * rectánguloFraccional.X), redondear(límites.Height * rectánguloFraccional.Y),
                redondear(límites.Width * rectánguloFraccional.Width), redondear(límites.Height * rectánguloFraccional.Height));

            var bmp = new Bitmap(límitesAplicable.Width, límitesAplicable.Height);
            try {
                using (var g = Graphics.FromImage(bmp)) {
                    g.CopyFromScreen(new Point(límitesAplicable.X, límitesAplicable.Y), Point.Empty, límitesAplicable.Size);
                }
            } catch (Exception) {
                return bmp; // En casos de errores de Windows (usualmente cuando está en la pantalla de Ctrl+Alt+Supr), devuelve el mapa de bits vacío.
                throw;
            }

            Bitmap convertirA8Bits(Bitmap b) {

                Bitmap bmp8Bits;
                if (formatoPíxeles == PixelFormat.Format8bppIndexed) {

                    bmp8Bits = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.Format8bppIndexed);
                    b.Dispose();
                    return bmp8Bits;

                } else {
                    return b;
                }

            } // convertirA8Bits

            if (negativo || blancoYNegro || escala != 1) {

                var bmpTransformado = Transformar(bmp, negativo, blancoYNegro, escala, luminosidad, contraste, modoInterpolación,
                    formatoPíxeles == PixelFormat.Format8bppIndexed ? PixelFormat.Format32bppArgb : formatoPíxeles); // El formato de pixeles Format8bppIndexed no es compatible con los objetos graphics.
                bmp.Dispose();
                return convertirA8Bits(bmpTransformado);

            } else {
                return convertirA8Bits(bmp);
            }

        } // ObtenerBitmap>



    } // CapturaDePantalla>



} // RTSHelper>