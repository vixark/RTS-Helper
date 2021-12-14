using System;
using System.Collections.Generic;
using System.Text;
using static RTSHelper.Global;



namespace RTSHelper {



    public class Fuente {


        #region Propiedades

        public string Nombre { get; set; }

        public TipoFuente Tipo { get; set; }

        public bool SubíndiceLetras { get; set; }

        public bool SubíndiceNúmeros { get; set; }

        public bool SuperíndiceLetras { get; set; }

        public bool SuperíndiceNúmeros { get; set; }

        #endregion Propiedades>


        #region Constructores


        public Fuente(string nombre, TipoFuente tipo, bool superíndiceNúmeros, bool subíndiceNúmeros, bool superíndiceLetras, bool subíndiceLetras) {
            Nombre = nombre;
            Tipo = tipo;
            SubíndiceLetras = subíndiceLetras;
            SubíndiceNúmeros = subíndiceNúmeros;
            SuperíndiceLetras = superíndiceLetras;
            SuperíndiceNúmeros = superíndiceNúmeros;
        } // Fuente>


        #endregion Constructores>


    } // Fuente>



} // RTSHelper>
