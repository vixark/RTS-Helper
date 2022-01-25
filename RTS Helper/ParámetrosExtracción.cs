using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;



namespace RTSHelper {



    public class ParámetrosExtracción {



        #region Propiedades

        public bool SoloNúmeros { get; set; }

        public bool PermitirUnCarácter { get; set; }

        public bool Negativo { get; set; }

        public bool BlancoYNegro { get; set; }

        public double Escala { get; set; }

        public float Luminosidad { get; set; }

        public float Contraste { get; set; }

        public InterpolationMode ModoInterpolación { get; set; }

        public PixelFormat FormatoPixeles { get; set; }

        #endregion Propiedades>



        #region Constructores


        public ParámetrosExtracción(bool soloNúmeros, bool permitirUnCarácter, bool negativo, bool blancoYNegro, double escala, 
            float luminosidad, float contraste, InterpolationMode modoInterpolación, PixelFormat formatoPixeles) {

            SoloNúmeros = soloNúmeros;
            PermitirUnCarácter = permitirUnCarácter;
            Negativo = negativo;
            BlancoYNegro = blancoYNegro;
            Escala = escala;
            Luminosidad = luminosidad;
            Contraste = contraste;
            ModoInterpolación = modoInterpolación;
            FormatoPixeles = formatoPixeles;

        } // ParámetrosExtracción>


        #endregion Constructores>



    } // ParámetrosExtracción>



} // RTSHelper>
