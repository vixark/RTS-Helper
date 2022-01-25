using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using Tesseract;



namespace RTSHelper {



    public class OCR {


        public static string? ObtenerTexto(string ruta, bool soloNúmeros, bool unCarácter, out float confianza) 
            => ObtenerTexto(Pix.LoadFromFile(ruta), soloNúmeros, unCarácter, out confianza);


        public static string? ObtenerTexto(Bitmap bmp, bool soloNúmeros, bool permitirUnCarácter, out float confianza) 
            => ObtenerTexto(PixConverter.ToPix(bmp), soloNúmeros, permitirUnCarácter, out confianza);


        public static string? ObtenerTexto(Pix pix, bool soloNúmeros, bool permitirUnCarácter, out float confianza) { // Prueba tomada de https://github.com/charlesw/tesseract-samples/blob/master/src/Tesseract.ConsoleDemo/Program.cs.

            var textoExtraído = (string?)null;
            confianza = 0;

            try {

                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {

                    if (soloNúmeros) engine.SetVariable("tessedit_char_whitelist", "0123456789");
                    if (permitirUnCarácter) engine.DefaultPageSegMode = PageSegMode.SingleChar; // En ocasiones se debe activar esta opción para mejorar el reconocimiento de números de 2 o 3 cifras.

                    using (var img = pix) {
                        using (var página = engine.Process(img)) {

                            textoExtraído = página.GetText();
                            confianza = página.GetMeanConfidence();

                        }
                    }
                }

            } catch (Exception) { }

            return textoExtraído?.Trim().Replace("\n", " ");

        } // ObtenerTexto>


    } // OCR>



} // RTSHelper>
