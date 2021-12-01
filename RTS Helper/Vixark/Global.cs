using System;
using System.Collections.Generic;
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


    } // General>



} // Vixark>
