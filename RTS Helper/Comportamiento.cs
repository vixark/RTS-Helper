using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using static RTSHelper.Global;
using static Vixark.General;



namespace RTSHelper {



    public class Comportamiento {



        #region Propiedades
        // Al agregar una nueva se debe agregar en ObtenerComportamientoEfectivo() y en CopiarPropiedadesEnNulas().

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

        public bool? MostrarAnteriorPaso { get; set; }

        public int? DuraciónPresonido { get; set; } // Autocalculada al cambiar el valor de Presonido.

        public static Dictionary<string, int?> DuraciónPresonidos = new Dictionary<string, int?>();

        public int? Progreso { get; set; } // Progreso de la orden de ejecución. Normalmente es la cantidad de unidades recolectoras económicas (aldeanos).

        #endregion Propiedades>


        #region Constructores


        public Comportamiento() { }


        public Comportamiento(string texto, out Dictionary<string, Comportamiento> clasesLeídas, Dictionary<string, Comportamiento>? clases) {

            clasesLeídas = new Dictionary<string, Comportamiento>();
            if (string.IsNullOrEmpty(texto)) return; // Si el texto es vacío, no contiene ni comportamientos ni clases.

            var palabrasClaveTextos = "s|es|fc|sns|sps";
            var palabrasClaveNúmeros = "t|fco|sv|esv|p";
            var palabrasClave = $"{palabrasClaveTextos}|{palabrasClaveNúmeros}".Split("|").ToList();
            var textoMinúscula = texto.ToLowerInvariant();

            var coincidenciasClases = Regex.Matches(textoMinúscula, @"([áéíóúüa-z0-9_-]+)={(.+?)}"); // Primero extrae las coincidencias de clases de formato y las elimina del texto.
            if (coincidenciasClases.Count > 0) {
                foreach (Match? coincidenciasClase in coincidenciasClases) {
                    if (coincidenciasClase != null && coincidenciasClase.Success) {

                        var nombreClase = coincidenciasClase.Groups[1].Value;
                        var textoComportamientosClase = coincidenciasClase.Groups[2].Value;
                        if (palabrasClave.Contains(nombreClase)) {
                            MostrarError($"The behavior class name {nombreClase} is not allowed because it can't have the same name as a keyword.");
                        } else {

                            if (clasesLeídas.ContainsKey(nombreClase)) {
                                MostrarError($"The behavior class name {nombreClase} is not allowed because it was already added.");
                            } else {
                                clasesLeídas.Add(nombreClase, new Comportamiento(textoComportamientosClase, out _, null));
                                textoMinúscula = textoMinúscula.Replace(coincidenciasClase.Groups[0].Value, "");
                            }

                        }

                    }
                }
            }

            var coincidenciasTextos = Regex.Matches(textoMinúscula, $@"({palabrasClaveTextos})=""(.+?)""");
            if (coincidenciasTextos.Count > 0) {

                foreach (Match? coincidenciaTexto in coincidenciasTextos) {

                    if (coincidenciaTexto != null && coincidenciaTexto.Success) {

                        var comportamientoId = coincidenciaTexto.Groups[1].Value;
                        var valor = coincidenciaTexto.Groups[2].Value;
                        switch (comportamientoId) {
                            case "s":

                                if (Preferencias.OverrideStepStartSound) {
                                    Sonido = valor;
                                    if (Sonido == "default") Sonido = Preferencias.StepStartSound;
                                    if (Sonido.ToLower() != NoneSoundString.ToLower() && !File.Exists(Path.Combine(DirectorioSonidosCortos, Sonido))) {
                                        MostrarError($"{Sonido} sound file doesn't exists in {DirectorioSonidosCortos}.");
                                        Sonido = null;
                                    }
                                }
                                break;

                            case "es":

                                if (Preferencias.OverrideStepEndSound) {
                                    Presonido = valor;
                                    if (Presonido == "default") Presonido = Preferencias.StepEndSound;
                                    if (Presonido.ToLower() != NoneSoundString.ToLower() && !File.Exists(Path.Combine(DirectorioSonidosLargos, Presonido))) {
                                        MostrarError($"{Presonido} sound file doesn't exists in {DirectorioSonidosLargos}.");
                                        Presonido = null;
                                    }
                                }
                                break;

                            case "fc":

                                if (valor.ToLower() == NoneSoundString.ToLower()) {
                                    if (Preferencias.OverrideFlashOnStepChange) Flash = false;
                                } else {

                                    if (Preferencias.OverrideFlashOnStepChange) Flash = true;
                                    if (Preferencias.OverrideFlashingColor) {

                                        ColorFlash = valor;
                                        var mediaColorFlash = ObtenerMediaColor(ColorFlash);
                                        if (mediaColorFlash == null) {
                                            MessageBox.Show($"The value {valor} for fc is invalid.");
                                            if (Preferencias.OverrideFlashOnStepChange) Flash = false;
                                        }

                                    }

                                }
                                break;

                            case "sns":

                                if (Preferencias.OverrideShowNextPreviousStep) {

                                    if (valor == "yes" || valor == "true") {
                                        MostrarSiguientePaso = true;
                                    } else if (valor == "no" || valor == "false") {
                                        MostrarSiguientePaso = false;
                                    } else {
                                        MessageBox.Show($"The value {valor} for sns is invalid.");
                                    }

                                }
                                break;

                            case "sps":

                                if (Preferencias.OverrideShowNextPreviousStep) {

                                    if (valor == "yes" || valor == "true") {
                                        MostrarAnteriorPaso = true;
                                    } else if (valor == "no" || valor == "false") {
                                        MostrarAnteriorPaso = false;
                                    } else {
                                        MessageBox.Show($"The value {valor} for sps is invalid.");
                                    }

                                }
                                break;

                            default:
                                MessageBox.Show($"The value {comportamientoId} as a behavior wasn't expected.");
                                break;
                        }

                    }

                }

            } // coincidenciasTextos.Count > 0>

            var coincidenciasNúmeros = Regex.Matches(textoMinúscula, $@"({palabrasClaveNúmeros})=([0-9,.]+)");
            if (coincidenciasNúmeros.Count > 0) {

                foreach (Match? coincidenciaNúmero in coincidenciasNúmeros) {

                    if (coincidenciaNúmero != null && coincidenciaNúmero.Success) {

                        var comportamientoId = coincidenciaNúmero.Groups[1].Value;
                        var valor = coincidenciaNúmero.Groups[2].Value;
                        switch (comportamientoId) {
                            case "t":

                                if (Preferencias.OverrideStepDuration) {

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

                                }
                                break;

                            case "fco":

                                if (Preferencias.OverrideFlashingOpacity) {

                                    OpacidadFlash = double.Parse(ObtenerTextoNúmeroLocal(valor));
                                    if (OpacidadFlash > 1 || OpacidadFlash < 0) {
                                        OpacidadFlash = null;
                                        MostrarError("fo value should be between 0 and 1.");
                                    }

                                }
                                break;

                            case "sv":

                                if (Preferencias.OverrideStepStartSoundVolume) {

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

                                }
                                break;

                            case "esv":

                                if (Preferencias.OverrideStepEndSoundVolume) {

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

                                }
                                break;

                            case "p":

                                int número4;
                                if (!int.TryParse(ObtenerTextoNúmeroLocal(valor), out número4)) {
                                    MostrarError("p value should be an integer.");
                                } else {
                                    Progreso = número4;
                                }
                                break;

                            default:
                                MostrarError($"To Developer: The value {comportamientoId} as a behavior wasn't expected.");
                                break;
                        }

                    } // coincidenciaNúmero != null && coincidenciaNúmero.Success>

                }

            } // coincidenciasNúmeros.Count > 0>

            var valores = textoMinúscula.Split(" ").ToList() ?? new List<string>();
            var comportamientosDeClases = new List<Comportamiento>();
            foreach (var valor in valores) {
                if (string.IsNullOrEmpty(valor)) continue;
                if (clases != null && clases.ContainsKey(valor)) comportamientosDeClases.Add(clases[valor]);
            }

            foreach (var comportamientoDeClase in comportamientosDeClases) {
                CopiarPropiedadesEnNulas(comportamientoDeClase, this);
            }

        } // Comportamiento>


        #endregion Constructores>



        #region Procedimientos y Funciones


        public static void CopiarPropiedadesEnNulas(Comportamiento origen, Comportamiento destino) { // Copia las propiedades en formatoDestino están nulas.

            destino.ColorFlash ??= origen.ColorFlash;
            destino.Duración ??= origen.Duración;
            destino.Flash ??= origen.Flash;
            destino.OpacidadFlash ??= origen.OpacidadFlash;
            destino.Presonido ??= origen.Presonido;
            destino.Sonido ??= origen.Sonido;
            destino.VolumenPresonido ??= origen.VolumenPresonido;
            destino.VolumenSonido ??= origen.VolumenSonido;
            destino.MostrarSiguientePaso ??= origen.MostrarSiguientePaso;
            destino.MostrarAnteriorPaso ??= origen.MostrarAnteriorPaso;
            destino.Progreso ??= origen.Progreso;

        } // CopiarPropiedadesEnNulas>


        public static int? ObtenerDuraciónPresonido(string? presonido) {

            if (presonido == null || presonido.ToLowerInvariant() == NoneSoundString.ToLowerInvariant()) return null;
            if (DuraciónPresonidos.ContainsKey(presonido)) return DuraciónPresonidos[presonido];
            int? duración = ObtenerDuraciónEndStepSound(presonido);
            if (duración == 0) duración = null;
            DuraciónPresonidos.Add(presonido, duración);
            return duración;

        } // ObtenerDuraciónPresonido>


        public static Comportamiento ObtenerComportamientoEfectivo(Comportamiento? comportamientoHijo, Comportamiento comportamientoPadre) {

            var comportamiento = new Comportamiento(); // Se permiten valores nulos del comportamiento efectivo porque al ser usados se reemplazan por el valor de preferencias.
            comportamiento.ColorFlash = comportamientoHijo?.ColorFlash ?? comportamientoPadre.ColorFlash;
            comportamiento.Duración = comportamientoHijo?.Duración ?? comportamientoPadre.Duración;
            comportamiento.Flash = comportamientoHijo?.Flash ?? comportamientoPadre.Flash;
            comportamiento.OpacidadFlash = comportamientoHijo?.OpacidadFlash ?? comportamientoPadre.OpacidadFlash;
            comportamiento.Presonido = comportamientoHijo?.Presonido ?? comportamientoPadre.Presonido;
            comportamiento.Sonido = comportamientoHijo?.Sonido ?? comportamientoPadre.Sonido;
            comportamiento.VolumenPresonido = comportamientoHijo?.VolumenPresonido ?? comportamientoPadre.VolumenPresonido;
            comportamiento.VolumenSonido = comportamientoHijo?.VolumenSonido ?? comportamientoPadre.VolumenSonido;
            comportamiento.MostrarSiguientePaso = comportamientoHijo?.MostrarSiguientePaso ?? comportamientoPadre.MostrarSiguientePaso;
            comportamiento.MostrarAnteriorPaso = comportamientoHijo?.MostrarAnteriorPaso ?? comportamientoPadre.MostrarAnteriorPaso;
            comportamiento.Progreso = comportamientoHijo?.Progreso ?? comportamientoPadre.Progreso;
            return comportamiento;

        } // ObtenerComportamientoEfectivo>


        #endregion Procedimientos y Funciones>



    } // Comportamiento>



} // RTSHelper>
