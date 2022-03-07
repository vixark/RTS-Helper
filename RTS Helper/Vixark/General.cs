using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;



// Funciones copiadas textualmente desde Vixark.cs.
namespace Vixark {



    static class General {



        [Flags] public enum Serialización { EnumeraciónEnTexto = 1, DiccionarioClaveEnumeración = 2, UTF8 = 3 } // Se puede establecer una o varias serializaciones especiales con el operador |.

        public static IEnumerable<T> ObtenerValores<T>() where T : struct, Enum => Enum.GetValues(typeof(T)).Cast<T>();

        public static string[] ExtensionesImágenes = new string[] { ".jpg", ".png", ".jpeg" }; // Este vector es redundante con los dos anteriores, pero se prefiere disponer de él por rendimiento para no tener que generarlo al vuelo.


        public static JsonSerializerOptions ObtenerOpcionesSerialización(Serialización serializacionesEspeciales) {

            var opcionesSerialización = new JsonSerializerOptions();
            opcionesSerialización.WriteIndented = true;
            if (serializacionesEspeciales.HasFlag(Serialización.DiccionarioClaveEnumeración)) {
                opcionesSerialización.Converters.Add(new FábricaConvertidorDiccionarioClaveEnumeración());
            }
            if (serializacionesEspeciales.HasFlag(Serialización.EnumeraciónEnTexto)) {
                opcionesSerialización.Converters.Add(new JsonStringEnumConverter());
            }
            if (serializacionesEspeciales.HasFlag(Serialización.UTF8)) {
                opcionesSerialización.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            }
            return opcionesSerialización;

        } // ObtenerOpcionesSerialización>


        /// <summary>
        /// Agrega a un diccionario de manera limpia. Se crea el diccionario si es nulo y se actualiza el valor si ya existe en la clave. 
        /// Para que funcione correctamente la creación si es nulo, se debe llamar asignándolo a si mismo así diccionario = diccionario.Agregar(clave, valor).
        /// </summary>
        public static Dictionary<K, V> Agregar<K, V>(this Dictionary<K, V>? diccionario, K clave, V valor, bool sobreescribir = true) where K : notnull {

            diccionario ??= new Dictionary<K, V>();
            if (diccionario.ContainsKey(clave)) {

                if (sobreescribir) {
                    diccionario[clave] = valor; // Por rendimiento de manera predeterminada se sobreescribe. No hay claridad del rendimiento genérico de la función Equals.
                } else {
                    if (!Equals(diccionario[clave], valor)) diccionario[clave] = valor; // Se evita sobreescribir si no es necesario.
                }

            } else {
                diccionario.Add(clave, valor);
            }
            return diccionario;

        } // Agregar>

        public static string? ObtenerExtensión(string archivo) => Path.GetExtension(archivo).AMinúscula();

        /// <summary>
        /// Encapsulación de rápido acceso de ToLowerInvariant(). Es útil para omitir la advertencia CA1308 sin saturar el código.
        /// No se puede crear un método con el mismo nombre para string (sin ?) porque el compilador no lo permite. Si se necesitara, 
        /// se podría hacer otro método con otro nombre. Una solución fácil es usar este método con un string y poner ! después de () 
        /// para informarle al compilador que se asegura que el resultado no será nulo.
        /// </summary>
        public static string? AMinúscula(this string? texto) => texto?.ToLowerInvariant();


        /// <summary>
        /// Abre un archivo en Windows.
        /// </summary>
        /// <param name="rutaArchivo"></param>
        public static bool AbrirArchivo(string? rutaArchivo) {

            if (rutaArchivo == null || !File.Exists(rutaArchivo)) return false;
            Process.Start(new ProcessStartInfo(rutaArchivo) { UseShellExecute = true });
            return true;

        } // AbrirArchivo>


        public static System.Windows.Media.Color? ObtenerMediaColor(string textoColor) {

            try {
                return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(textoColor);
            } catch {
                return null;
            }

        } // ObtenerMediaColor>


        public static Color? ObtenerColor(string textoColor) {

            var mediaColor = ObtenerMediaColor(textoColor);
            if (mediaColor == null) return null;
            var notNullMediaColor = (System.Windows.Media.Color)mediaColor;
            return Color.FromArgb(notNullMediaColor.A, notNullMediaColor.R, notNullMediaColor.G, notNullMediaColor.B);

        } // ObtenerColor>


        public static Color ObtenerColorPromedio(List<Color> colores) {

            double R = 0;
            double G = 0;
            double B = 0;
            foreach (var color in colores) {
                R += color.R;
                G += color.G;
                B += color.B;
            }
            var cantidadColores = colores.Count;
            return Color.FromArgb(RedondearAEntero(R / cantidadColores), RedondearAEntero(G / cantidadColores), RedondearAEntero(B / cantidadColores));

        } // ObtenerColorPromedio>


        public static int RedondearAEntero(double d) => (int)Math.Round(d, 0);


        /// <summary>
        /// Obtiene el valor del atributo Display del valor de la enumeración. Es útil para usar un texto personalizado para cada elemento en una enumeración.
        /// </summary>
        public static string ATexto<T>(this T enumeración) where T : struct, Enum {

            var tipo = enumeración.GetType();
            var textoDirecto = enumeración.ToString();

            string obtenerTexto(string textoDirecto)
                => tipo.GetMember(textoDirecto).Where(x => x.MemberType == MemberTypes.Field && ((FieldInfo)x).FieldType == tipo)
                       .First().GetCustomAttribute<DisplayAttribute>()?.Name ?? textoDirecto;

            if (textoDirecto.Contiene(", ")) {

                var texto = new StringBuilder();
                foreach (var textoDirectoAux in textoDirecto.Split(", ")) {
                    texto.Append($"{obtenerTexto(textoDirectoAux)}, ");
                }
                return texto.ToString()[0..^2];

            } else {
                return obtenerTexto(textoDirecto);
            }

        } // ATexto>


        /// <summary>
        /// Encapsulación de rápido acceso de Contains() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 sin saturar el código.
        /// </summary>
        public static bool Contiene(this string texto, string textoContenido, bool ignorarCapitalización = true)
            => texto.Contains(textoContenido, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);


        public static int ObtenerDistanciaLevenshtein(string texto1, string texto2) {

            int[,] d = new int[texto1.Length + 1, texto2.Length + 1];
            int i;
            int j;
            int costo;
            char[] str1 = texto1.ToCharArray();
            char[] str2 = texto2.ToCharArray();

            for (i = 0; i <= str1.Length; i++) {
                d[i, 0] = i;
            }
                
            for (j = 0; j <= str2.Length; j++) {
                d[0, j] = j;
            }
                
            for (i = 1; i <= str1.Length; i++) {
                for (j = 1; j <= str2.Length; j++) {

                    costo = str1[i - 1] == str2[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(d[i - 1, j] + 1, Math.Min(d[i, j - 1] + 1, d[i - 1, j - 1] + costo)); // Eliminación e inserción.
                    if ((i > 1) && (j > 1) && (str1[i - 1] == str2[j - 2]) && (str1[i - 2] == str2[j - 1]))
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + costo); // Sustitución.

                }
            }

            return d[str1.Length, str2.Length];

        } // ObtenerDistanciaLevenshtein>


        public static Bitmap Transformar(Bitmap bmp, bool negativo, bool blancoYNegro, double escala, float luminosidad, float contraste,
            InterpolationMode modoInterpolación = InterpolationMode.HighQualityBicubic, PixelFormat formatoPíxeles = PixelFormat.Format32bppArgb) { // Ver https://mariusbancila.ro/blog/2009/11/13/using-colormatrix-for-creating-negative-image/.

            var nuevoAncho = (int)Math.Round(bmp.Width * escala, 0);
            var nuevoAlto = (int)Math.Round(bmp.Height * escala, 0);
            var nuevoBmp = new Bitmap(nuevoAncho, nuevoAlto, formatoPíxeles);
            var g = Graphics.FromImage(nuevoBmp);

            var matrizNegativo = new float[][] { new float[] {-1, 0, 0, 0, 0}, new float[] {0, -1, 0, 0, 0}, new float[] {0, 0, -1, 0, 0},
                  new float[] {0, 0, 0, 1, 0}, new float[] {1, 1, 1, 0, 1} };
            var matrizBlancoYNegro = new float[][] { new float[] {0.299F, 0.299F, 0.299F, 0, 0}, new float[] { 0.587F, 0.587F, 0.587F, 0, 0}, 
                new float[] { 0.114F, 0.114F, 0.114F, 0, 0}, new float[] {0, 0, 0, 1, 0}, new float[] { 0, 0, 0, 0, 1 } };
            var matrizLuminosidadYContraste = new float[][] { new float[] { contraste, 0, 0, 0, 0}, new float[] {0, contraste, 0, 0, 0}, new float[] 
                {0, 0, contraste, 0, 0}, new float[] {0, 0, 0, 1.0f, 0}, new float[] { 1 - luminosidad, 1 - luminosidad, 1 - luminosidad, 0, 1}};
            var matriz = new float[][] { new float[] {1, 0, 0, 0, 0}, new float[] {0, 1, 0, 0, 0}, new float[] {0, 0, 1, 0, 0},
                  new float[] {0, 0, 0, 1, 0}, new float[] {0, 0, 0, 0, 1} };

            if (negativo) matriz = MultiplicarMatrices(matriz, matrizNegativo, 5);
            if (blancoYNegro) matriz = MultiplicarMatrices(matriz, matrizBlancoYNegro, 5);
            if (luminosidad != 1 || contraste != 1) matriz = MultiplicarMatrices(matriz, matrizLuminosidadYContraste, 5);

            var attributes = new ImageAttributes();
            attributes.SetColorMatrix(new ColorMatrix(matriz));
            
            if (escala != 1) g.InterpolationMode = modoInterpolación;
            g.DrawImage(bmp, new Rectangle(0, 0, nuevoAncho, nuevoAlto), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();

            return nuevoBmp;

        } // Transformar>


        private static float[][] MultiplicarMatrices(float[][] f1, float[][] f2, int longitud) { // Ver https://www.codeproject.com/Articles/7836/Multiple-Matrices-With-ColorMatrix-in-C.
            
            var x = new float[longitud][];
            for (int d = 0; d < longitud; d++) {
                x[d] = new float[longitud];
            }    
            
            var size = longitud;
            var column = new float[longitud];
            for (int j = 0; j < longitud; j++) {

                for (int k = 0; k < longitud; k++) {
                    column[k] = f1[k][j];
                }

                for (int i = 0; i < longitud; i++) {

                    var row = f2[i];
                    var s = 0f;
                    for (int k = 0; k < size; k++) {
                        s += row[k] * column[k];
                    }
                    x[i][j] = s;

                }
            }

            return x;

        } // MultiplicarMatrices>


        public static string LimpiarNombreArchivo(string? nombreArchivo, string carácterSustitución = " ") {

            if (nombreArchivo == null) return carácterSustitución;
            var carácteresInválidos = new Regex(@"[\\/:*?""<>|]");
            return carácteresInválidos.Replace(nombreArchivo, carácterSustitución);

        } // LimpiarNombreArchivo>


        /// <summary>
        /// Obtiene un archivo que puede estar siendo usado por otro proceso, pero que se espera que pronto sea liberado.
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="máximosIntentos"></param>
        /// <returns></returns>
        public static bool ObtenerArchivoLibre(string ruta, int máximosIntentos = 20) { // Ver https://stackoverflow.com/a/21053032/8330412.

            var estáLibre = false;
            var intentos = 0;
            while (intentos <= máximosIntentos && !estáLibre) {
                try {
                    using (File.Open(ruta, FileMode.Open, FileAccess.ReadWrite)) {
                        estáLibre = true;
                    }
                } catch {
                    intentos++;
                    Thread.Sleep(100);
                }
            }

            return estáLibre;

        } // ObtenerArchivoLibre>


    } // General>



} // Vixark>
