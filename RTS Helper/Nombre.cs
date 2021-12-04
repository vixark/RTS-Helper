using System;
using System.Collections.Generic;
using System.Text;
using static RTSHelper.Global;



namespace RTSHelper {



    public class Nombre {


        public string ID { get; set; }

        public string Valor { get; set; }

        public NameType Tipo { get; set; }

        public Nombre(string valor, NameType tipo, string id) => (Valor, Tipo, ID) = (valor, tipo, id);


    } // Nombre>



} // RTSHelper>
