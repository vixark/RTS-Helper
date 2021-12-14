using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using static RTSHelper.Global;
using static Vixark.General;



namespace RTSHelper {



    public class Comportamiento {
        


        #region Propiedades

        public double? Duración { get; set; }

        private string? _Presonido;
        public string? Presonido { get => _Presonido;  
            set { 
                _Presonido = value;
                DuraciónPresonido = ObtenerDuraciónPresonido(_Presonido);
            } 
        }

        public string? Sonido { get; set; }

        public int? VolumenPresonido { get; set; }

        public int? VolumenSonido { get; set; }

        public bool? Flash { get; set; }

        public string? ColorFlash { get; set; }

        public double? OpacidadFlash { get; set; }

        public bool? MostrarSiguientePaso { get; set; }

        public int? DuraciónPresonido { get; set; } // Autocalculada al cambiar el valor de Presonido.

        public static Dictionary<string, int?> DuraciónPresonidos = new Dictionary<string, int?>();

        #endregion Propiedades>


        #region Constructores


        public Comportamiento() { }


        public Comportamiento(string textoComportamientos) {

            var coincidenciasTextos = Regex.Matches(textoComportamientos, @"(s|es|fc|sns)=""(.+?)""", RegexOptions.IgnoreCase);
            if (coincidenciasTextos.Count > 0) {

                foreach (Match? coincidenciaTexto in coincidenciasTextos) {

                    if (coincidenciaTexto != null && coincidenciaTexto.Success) {

                        var comportamientoId = coincidenciaTexto.Groups[1].Value.ToLowerInvariant();
                        var valor = coincidenciaTexto.Groups[2].Value.ToLowerInvariant();
                        switch (comportamientoId) {
                            case "s":
                                Sonido = valor;
                                if (valor != "none" && !File.Exists(Path.Combine(DirectorioSonidosCortos, valor))) {
                                    MostrarError($"{valor} sound file doesn't exists in {DirectorioSonidosCortos}.");
                                    Sonido = null;
                                }
                                break;
                            case "es":
                                Presonido = valor;
                                if (valor != "none" && !File.Exists(Path.Combine(DirectorioSonidosLargos, valor))) {
                                    MostrarError($"{valor} sound file doesn't exists in {DirectorioSonidosLargos}.");
                                    Presonido = null;
                                }
                                break;
                            case "fc":

                                if (valor == "none") {
                                    Flash = false;
                                } else {

                                    Flash = true;
                                    ColorFlash = valor;
                                    var mediaColorFlash = ObtenerMediaColor(ColorFlash);
                                    if (mediaColorFlash == null) {
                                        MessageBox.Show($"The value {valor} for fc is invalid.");
                                        Flash = false;
                                    }

                                }
                                break;

                            case "sns":

                                if (valor == "yes" || valor == "true") {
                                    MostrarSiguientePaso = true;
                                } else if (valor == "no" || valor == "false") {
                                    MostrarSiguientePaso = false;
                                } else {
                                    MessageBox.Show($"The value {valor} for sns is invalid.");
                                }
                                break;

                            default:
                                MessageBox.Show($"The value {comportamientoId} as a behavior wasn't expected.");
                                break;
                        }

                    }

                }

            } // coincidenciasTextos.Count > 0>

            var coincidenciasNúmeros = Regex.Matches(textoComportamientos, @"(t|fco|sv|esv)=([0-9,.]+)", RegexOptions.IgnoreCase);
            if (coincidenciasNúmeros.Count > 0) {

                foreach (Match? coincidenciaNúmero in coincidenciasNúmeros) {

                    if (coincidenciaNúmero != null && coincidenciaNúmero.Success) {

                        var comportamientoId = coincidenciaNúmero.Groups[1].Value.ToLowerInvariant();
                        var valor = coincidenciaNúmero.Groups[2].Value.ToLowerInvariant();
                        switch (comportamientoId) {
                            case "t":

                                int número;
                                if (!int.TryParse(ObtenerTextoNúmeroLocal(valor), out número)) {
                                    MostrarError("t value should be an integer.");
                                } else {

                                    if (número > 0 && número <= 86400) {
                                        Duración = número;
                                    } else {
                                        MostrarError("t value should be between 1 and 86400.");
                                    }

                                }
                                break;

                            case "fco":

                                OpacidadFlash = double.Parse(ObtenerTextoNúmeroLocal(valor));
                                if (OpacidadFlash > 1 || OpacidadFlash < 0) {
                                    OpacidadFlash = null;
                                    MostrarError("fo value should be between 0 and 1.");
                                }
                                break;

                            case "sv":

                                int número2;
                                if (!int.TryParse(ObtenerTextoNúmeroLocal(valor), out número2)) {
                                    MostrarError("sv value should be an integer.");
                                } else {

                                    if (número2 >= 0 && número2 <= 100) {
                                        VolumenSonido = número2;
                                    } else {
                                        MostrarError("sv value should be between 0 and 100.");
                                    }

                                }
                                break;

                            case "esv":

                                int número3;
                                if (!int.TryParse(ObtenerTextoNúmeroLocal(valor), out número3)) {
                                    MostrarError("esv value should be an integer.");
                                } else {

                                    if (número3 >= 0 && número3 <= 100) {
                                        VolumenPresonido = número3;
                                    } else {
                                        MostrarError("esv value should be between 0 and 100.");
                                    }

                                }
                                break;

                            default:
                                MostrarError($"To Developer: The value {comportamientoId} as a behavior wasn't expected.");
                                break;
                        }

                    } // coincidenciaNúmero != null && coincidenciaNúmero.Success>

                }

            } // coincidenciasNúmeros.Count > 0>

        } // Comportamiento>


        #endregion Constructores>



        #region Procedimientos y Funciones


        public static int? ObtenerDuraciónPresonido(string? presonido) {

            if (presonido == null || presonido.ToLowerInvariant() == NoneSoundString.ToLowerInvariant()) return null;
            if (DuraciónPresonidos.ContainsKey(presonido)) return DuraciónPresonidos[presonido];
            int? duración = ObtenerDuraciónEndStepSound(presonido);
            if (duración == 0) duración = null;
            DuraciónPresonidos.Add(presonido, duración);
            return duración;

        } // ObtenerDuraciónPresonido>


        public static Comportamiento ObtenerComportamientoEfectivo(Comportamiento? comportamientoHijo, Comportamiento comportamientoPadre) {

            var comportamiento = new Comportamiento();
            comportamiento.ColorFlash = (comportamientoHijo?.ColorFlash ?? comportamientoPadre.ColorFlash) ?? FlashingColorPredeterminado;
            comportamiento.Duración = (comportamientoHijo?.Duración ?? comportamientoPadre.Duración) ?? StepDurationPredeterminado;
            comportamiento.Flash = (comportamientoHijo?.Flash ?? comportamientoPadre.Flash) ?? FlashOnStepChangePredeterminado;
            comportamiento.OpacidadFlash = (comportamientoHijo?.OpacidadFlash ?? comportamientoPadre.OpacidadFlash) ?? FlashingOpacityPredeterminado;
            comportamiento.Presonido = (comportamientoHijo?.Presonido ?? comportamientoPadre.Presonido) ?? StepEndSoundPredeterminado;
            comportamiento.Sonido = (comportamientoHijo?.Sonido ?? comportamientoPadre.Sonido) ?? StepStartSoundPredeterminado;
            comportamiento.VolumenPresonido = (comportamientoHijo?.VolumenPresonido ?? comportamientoPadre.VolumenPresonido)
                ?? StepEndSoundVolumePredeterminado;
            comportamiento.VolumenSonido = (comportamientoHijo?.VolumenSonido ?? comportamientoPadre.VolumenSonido) ?? StepStartSoundVolumePredeterminado;
            comportamiento.MostrarSiguientePaso = (comportamientoHijo?.MostrarSiguientePaso ?? comportamientoPadre.MostrarSiguientePaso) ?? ShowNextStepPredeterminado;
            return comportamiento;

        } // ObtenerComportamientoEfectivo>


        #endregion Procedimientos y Funciones>



    } // Comportamiento>



} // RTSHelper>
