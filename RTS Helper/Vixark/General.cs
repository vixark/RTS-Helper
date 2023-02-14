using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;



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


        /// <summary>
        /// Abre una Url en el navegador predeterminado.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool AbrirUrl(string? url) {

            if (url == null) return false;
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            return true;

        } // AbrirUrl>


        /// <summary>
        /// Abre un directorio en el explorador de Windows.
        /// </summary>
        /// <param name="rutaDirectorio"></param>
        public static bool AbrirDirectorio(string? rutaDirectorio) {

            if (rutaDirectorio == null || !Directory.Exists(rutaDirectorio)) return false;
            Process.Start(new ProcessStartInfo(rutaDirectorio) { UseShellExecute = true });
            return true;

        } // AbrirDirectorio>


        public static System.Windows.Media.Color? ObtenerMediaColor(string textoColor) {

            try {
                return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(textoColor);
            } catch {
                return null;
            }

        } // ObtenerMediaColor>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clienteHtml">Se debe proveer un objeto que sea reusado por toda la aplicación para evitar el agotamiento de sockets.</param>
        /// <param name="url"></param>
        /// <param name="rutaArchivo"></param>
        /// <returns></returns>
        public async static Task<bool> DescargarArchivoAsync(HttpClient clienteHtml, string? url, string rutaArchivo) { // Ver https://jonathancrozier.com/blog/how-to-download-files-using-c-sharp.

            if (url is null) return false;

            try {

                using var stream = await clienteHtml.GetStreamAsync(url);
                using var fileStream = new FileStream(rutaArchivo, FileMode.Create);
                await stream.CopyToAsync(fileStream);
                return true;

            } catch (Exception) {
                return false;
            }

        } // DescargarArchivo>


        /// <summary>
        /// Elimina un archivo sin generar excepciones. Devuelve verdadero si fue borrado y falso si no se pudo borrar. Útil en los casos
        /// que se quiere eliminar un archivo, pero si por alguna razón está bloqueado y no se puede eliminar, se puede dejar sin eliminar.
        /// </summary>
        public static bool IntentarEliminar(string rutaArchivo) {

            try {
                File.Delete(rutaArchivo);
                return true;
            } catch (Exception) {
                return false;
            }

        } // IntentarEliminar>


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


        /// <summary>
        /// Encapsulación de rápido acceso de Contains() para IEnumerable para permitir fácilmente comparar ignorando la capitalización.
        /// </summary>
        public static bool Contiene(this IEnumerable<string> lista, string texto, bool ignorarCapitalización = true)
            => lista.Any(x => string.Compare(x, texto, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0);


        /// <summary>
        /// Encapsulación de rápido de Replace() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 sin saturar el código.
        /// </summary>
        public static string Reemplazar(this string texto, string anteriorTexto, string? nuevoTexto, bool ignorarCapitalización = true)
            => texto.Replace(anteriorTexto, nuevoTexto, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);


        /// <summary>
        /// Bajo ciertas circunstancias esta función podría dar mejor rendimiento que foreach (var kv in reemplazos) { if (texto.Contiene(kv.Key)) texto = texto.Reemplazar(kv.Key, kv.Value); }.
        /// Se debe probar el rendimiento en cada caso porque cualquiera de los dos puede ser más rápido.
        /// Si no se necesita alto rendimiento, ReemplazarVarios() es suficientemente rápida, pero tiene inconvenientes con los carácteres
        /// que puede aceptar para los textos a reemplazar, pues estos se usan dentro de una expresión regular.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="reemplazos"></param>
        /// <param name="ignorarCapitalización"></param>
        /// <returns></returns>
        public static string ReemplazarVarios(this string texto, Dictionary<string, string> reemplazos, bool ignorarCapitalización = true) // Ver https://stackoverflow.com/questions/1321331/replace-multiple-string-elements-in-c-sharp.
            => Regex.Replace(texto, "(" + String.Join("|", reemplazos.Keys.ToArray()) + ")", delegate (Match m) { return reemplazos[m.Value]; },
                ignorarCapitalización ? RegexOptions.IgnoreCase : RegexOptions.None);


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


        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BorrarObjeto([In] IntPtr hObject);


        /// <summary>
        /// Devuelve un objeto ImageSource para ser presentado en la interface a partir de un bitmap <paramref name="bmp"/>.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static System.Windows.Media.ImageSource ObtenerImageSource(Bitmap bmp) { // Ver https://stackoverflow.com/a/35274172/8330412. If you get 'dllimport unknown'-, add 'using System.Runtime.InteropServices;'

            var handle = bmp.GetHbitmap();
            try {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            } finally { BorrarObjeto(handle); }

        } // ObtenerImageSource>


        /// <summary>
        /// Encapsulación de acceso rápido para convertir un texto en una enumeración.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static T AEnumeración<T>(this string texto, bool ignorarCapitalización = true) where T : struct, Enum 
            => (T)Enum.Parse(typeof(T), texto, ignorarCapitalización);


        public static double Ancho(this System.Windows.Controls.TextBlock textBlock) { // https://stackoverflow.com/questions/9264398/how-to-calculate-wpf-textblock-width-for-its-known-font-size-and-characters y https://stackoverflow.com/questions/45765980/formattedtext-formttedtext-is-obsolete-use-the-pixelsperdip-override.

            var fuente = new System.Windows.Media.Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
            var textoFormateado = new System.Windows.Media.FormattedText(textBlock.Text, Thread.CurrentThread.CurrentCulture, textBlock.FlowDirection,
                fuente, textBlock.FontSize, textBlock.Foreground, System.Windows.Media.VisualTreeHelper.GetDpi(textBlock).PixelsPerDip);
            var tamaño = new System.Windows.Size(textoFormateado.WidthIncludingTrailingWhitespace, textoFormateado.Height);
            return tamaño.Width;

        } // Ancho>


        /// <summary>
        /// Obtiene la ruta de una carpeta y provee la opción de crearla si no existe. Si se quiere verificar la existencia
        /// de cierta carpeta se puede pasar la ruta en rutaPadre y pasar nombreCarpeta vacío. Funciona correctamente 
        /// si la carpeta que se requiere crear está dentro de una carpeta que tampoco existe, en este caso se crean todas las carpetas 
        /// necesarias para que exista la ruta rutaPadre + nombreCarpeta.
        /// </summary>
        public static string ObtenerRutaCarpeta(string rutaPadre, string nombreCarpeta, bool crearSiNoExiste) {

            var ruta = Path.Combine(rutaPadre, nombreCarpeta);
            if (!Directory.Exists(ruta)) {

                if (crearSiNoExiste) {
                    Directory.CreateDirectory(ruta);
                } else {
                    throw new ArgumentException($"No existe la carpeta {ruta}");
                }

            }
            return ruta;

        } // ObtenerRutaCarpeta>


    } // General>



} // Vixark>
