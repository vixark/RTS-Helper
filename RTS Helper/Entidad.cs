using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RTSHelper.Global;



namespace RTSHelper {



    public class Entidad {


        #region Propiedades

        public string ID { get; set; } // Único. Usualmente es un número, pero se prefiere manejar como string para dar mayor flexibilidad.

        public string NombreCompleto { get; set; } // Debe ser único porque se usa para relacionar las imágenes personalizadas, los nombres personalizados y los tipos. Se relacionan con el nombre completo y no con el ID para facilitar la edición manual de parte del usuario de los archivos Custom Images.json, Custom Names.json y Types.json.

        public string Tipo { get; set; } // Tipo de la entidad. Por ejemplo, Edificio, Tecnología, Recurso, etc.

        public Dictionary<NameType, string> Nombres { get; set; } = new Dictionary<NameType, string>();

        public string? ImágenesPersonalizadas { get; set; } // Es plural porque pueden ser varias unidas con |.

        public NameType TipoNombre { get; set; }

        #endregion Propiedades>


        #region Propiedades Autocalculadas

        public List<string> ImágenesEfectivas => ($"{(ImágenesPersonalizadas ?? NombreCompleto).ToLowerInvariant()}").Split("|").ToList(); // Es plural porque pueden ser varias unidas con |.

        #endregion Propiedades Autocalculadas>


        #region Constructores

        public Entidad(string id, string nombreCompleto, string tipo, string? imagenPersonalizada = null) 
            => (ID, NombreCompleto, Tipo, ImágenesPersonalizadas) = (id, nombreCompleto, tipo, imagenPersonalizada);

        #endregion Constructores>


        #region Funciones y Procedimientos


        public string ObtenerImagenEfectiva(bool aletatoria = false) {

            var imágenesEfectivas = ImágenesEfectivas;
            if (aletatoria) {

                var random = new Random(DateTime.Now.Second);
                var índice = random.Next(0, imágenesEfectivas.Count);
                return imágenesEfectivas[índice];

            } else {
                return imágenesEfectivas.First();
            }

        } // ObtenerImágenEfectiva>


        #endregion Funciones y Procedimientos>


    } // Entidad>



} // RTSHelper>
