using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;



// Algunas funciones copiadas textualmente desde Vixark.cs.
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


    } // General>



} // Vixark>
