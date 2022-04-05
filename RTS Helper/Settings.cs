using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.IO;
using static RTSHelper.Global;
using static Vixark.General;
using System.Linq;



namespace RTSHelper {



    public class Settings {



        #region Generales

        public string Game { get; set; } = AOE2Name; // Es una pseudopreferencia porque no se usa por si sola si no para establecer otros valores.

        public string ScreenResolution { get; set; } = "1920x1080"; // Es una pseudopreferencia porque no se usa por si sola si no para establecer otros valores.

        public float GameInterfaceScale { get; set; } = 100;

        public double GameSpeed { get; set; } = 1;

        public double ExecutionSpeed { get; set; } = 1; // 1 es la máxima velocidad. En esta velocidad cada conjunto de comandos corresponde a un minuto en el juego.

        public double StepDuration { get; set; } = StepDurationPredeterminado;

        public string CurrentBuildOrder { get; set; } = "Tutorial";

        public bool ShowNextStep { get; set; } = ShowNextStepPredeterminado;

        public bool ShowPreviousStep { get; set; } = ShowPreviousStepPredeterminado;

        public string? BuildOrderCustomDirectory { get; set; }

        public bool MinimizeOnComplete { get; set; } = false;

        public double LineSpacing { get; set; } = 20;

        public bool ShowStepProgress { get; set; } = true;

        public bool ShowTime { get; set; } = false;

        public bool ShowAlternateNextPreviousStepButton { get; set; } = true;

        public bool ShowAlwaysStatsButton { get; set; } = true; // Si es falso, solo se muestra al final.

        #endregion Generales>



        #region Sonidos

        public string StepStartSound { get; set; } = StepStartSoundPredeterminado;

        public string StepEndSound { get; set; } = StepEndSoundPredeterminado;

        public int StepStartSoundVolume { get; set; } = StepStartSoundVolumePredeterminado;

        public int StepEndSoundVolume { get; set; } = StepEndSoundVolumePredeterminado;

        public int StepEndSoundDuration { get; set; } = 0; // Se establece automáticamente al cambiar el sonido.

        public bool UnmuteAtStartup { get; set; } = false; // Permite que no se mantenga muted si la última vez que se usó la aplicación se cerró muted.

        public bool Muted { get; set; } = false; // Se establece desde la interface principal.

        public bool MuteOnComplete { get; set; } = true;

        #endregion Sonidos>



        #region Colores

        public string BackColor { get; set; } = BackColorPredeterminado;

        public string FontColor { get; set; } = FontColorPredeterminado;

        public double Opacity { get; set; } = OpacityPredeterminado;

        public string CurrentStepFontColor { get; set; } = CurrentStepFontColorPredeterminado;

        public string NextPreviousStepFontColor { get; set; } = NextPreviousStepFontColorPredeterminado;

        public bool FlashOnStepChange { get; set; } = FlashOnStepChangePredeterminado;

        public string FlashingColor { get; set; } = FlashingColorPredeterminado;

        public double FlashingOpacity { get; set; } = FlashingOpacityPredeterminado;

        public bool StopFlashingOnComplete { get; set; } = StopFlashingOnCompletePredeterminado;

        #endregion Colores>



        #region Fuentes

        public string FontName { get; set; } = NombreFuentePredeterminada;

        public bool CurrentStepFontBold { get; set; } = CurrentStepFontBoldPredeterminado;

        public bool NextPreviousStepFontBold { get; set; } = false;

        #endregion Fuentes>



        #region Tamaños
        // Son establecidos automáticamente al cambiar el juego y la resolución.

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public double CurrentStepFontSize { get; set; }

        public double NextPreviousStepFontSize { get; set; }

        public double ButtonsSize { get; set; }

        public double ButtonsMargin { get; set; }

        public double ButtonsPadding { get; set; }

        public double LargeFontSize { get; set; }

        public double MediumFontSize { get; set; }

        public double LeftMarginCurrentStep { get; set; }

        public double TopMarginCurrentStep { get; set; }

        public double RightMarginNextPreviousStep { get; set; }

        public double TopMarginNextPreviousStep { get; set; }

        public double BottomMargenSteps { get; set; }

        public double BuildOrderSelectorWidth { get; set; }

        public double ExecutionSpeedSelectorWidth { get; set; }

        public double ThicknessCircularProgressBar { get; set; }

        public double RightMarginCircularProgressBar { get; set; }

        #endregion Tamaños>



        #region Imágenes y Entidades

        public double ImageSize { get; set; } = ImageSizePredeterminado;

        public string ImageBackgroundColor { get; set; } = Color.FromRgb(0, 0, 0).ToString();

        public double ImageBackgroundOpacity { get; set; } = 0.3;

        public double EntityHorizontalMargin { get; set; } = 20; // Espaciado para las entidades: imágenes y textos entre []. Porcentaje del alto de la entidad  cada lado.

        public double ImageBackgroundRoundedCornersRadius { get; set; } = 10;

        public double SubscriptAndSuperscriptImagesSize { get; set; } = SubscriptAndSuperscriptImagesSizePredeterminado;

        public bool RandomImageForMultipleImages { get; set; } = true; // Si es falso, se usa la primera.

        public bool CapitalizeNames { get; set; } = true;

        public string GameLanguage { get; set; } = "EN";

        #endregion Imágenes y Entidades>



        #region Anulación de Opciones

        public bool OverrideFontSize { get; set; } = true;

        public bool OverrideFontName { get; set; } = true;

        public bool OverrideFontColor { get; set; } = true; // Aunque el color no debería tenerse que cambiar porque se pueden dar situaciones de usuarios que usen otros fondos de color y sea imposible de leer, para que esto suceda debe darse que el usuario use un fondo de color especial y además cargue una build order con un color no compatible. Se considera que esto es un caso poco frecuente que se soluciona desactivando manualmente esta opción, entonces se permite el cambio.

        public bool OverrideFontBold { get; set; } = true;

        public bool OverrideFontItalics { get; set; } = true;

        public bool OverrideFontUnderline { get; set; } = true;

        public bool OverrideFontPosition { get; set; } = true;

        public bool OverrideImageSize { get; set; } = true;

        public bool OverrideStepEndSound { get; set; } = true;

        public bool OverrideStepStartSound { get; set; } = true;

        public bool OverrideStepEndSoundVolume { get; set; } = false; // El volumen es una preferencia muy personal y adaptada al equipo de cada usuario, no debería tenerse que modificar desde la build order.

        public bool OverrideStepStartSoundVolume { get; set; } = false; // El volumen es una preferencia muy personal y adaptada al equipo de cada usuario, no debería tenerse que modificar desde la build order.

        public bool OverrideFlashOnStepChange { get; set; } = true;

        public bool OverrideFlashingColor { get; set; } = true;

        public bool OverrideFlashingOpacity { get; set; } = true;

        public bool OverrideShowNextPreviousStep { get; set; } = true;

        public bool OverrideStepDuration { get; set; } = true;

        #endregion Anulación de Opciones>


        #region Control

        public bool AutoAdjustIdleTime { get; set; } = true;

        public bool PauseDetection { get; set; } = true;

        public bool ShowAddIdleTimeButton { get; set; } = false;

        public bool ShowRemoveIdleTimeButton { get; set; } = false;

        public int AutoAdjustIdleTimeInterval { get; set; } = 1; // En segundos.

        public int PauseDetectionInterval { get; set; } = 1; // En segundos.

        public int AddIdleTimeSeconds { get; set; } = 10;

        public int RemoveIdleTimeSeconds { get; set; } = 10;

        public int MinimumDelayToAutoAdjustIdleTime { get; set; } = 3;

        public int ForwardSeconds { get; set; } = 10;

        public int BackwardSeconds { get; set; } = 10;

        public int NextMultipleSteps { get; set; } = 5;

        public int BackMultipleSteps { get; set; } = 5;

        #endregion Control>


        #region Órdenes de Ejecución Favoritas

        public Dictionary<string, List<string>> FavoriteBuildOrders { get; set; } = new Dictionary<string, List<string>>(); // La clave es el nombre del juego y

        public bool ShowOnlyFavoriteBuildOrders { get; set; } = false;

        #endregion Órdenes de Ejecución Favoritas>


        #region Nombres

        public Dictionary<NameType, int> DisplayPriority { get; set; } = new Dictionary<NameType, int> {
            { NameType.Image, 0 }, { NameType.Abbreviation, 1 }, { NameType.Acronym, 2 }, { NameType.Common, 3 }, { NameType.Complete, 4 },
            { NameType.Custom, 5 }, { NameType.AbbreviationPlural, 6 }, { NameType.AcronymPlural, 7 }, { NameType.CommonPlural, 8 }, { NameType.BR, 9 },
            { NameType.DE, 10 }, { NameType.ES, 11 }, { NameType.FR, 12 }, { NameType.HI, 13 }, { NameType.IT, 14 }, { NameType.JP, 15 },
            { NameType.KO, 16 }, { NameType.MS, 17 }, { NameType.MX, 18 }, { NameType.PL, 19 }, { NameType.RU, 20 }, { NameType.TR, 21 },
            { NameType.TW, 22 }, { NameType.VI, 23 }, { NameType.ZH, 24 }
        };

        #endregion Nombres>


        #region OCR

        public Dictionary<ScreenCaptureText, System.Drawing.RectangleF>? ScreenCaptureRectangles { get; set; } = null;

        public bool OCRTestMode { get; set; } = false;

        #endregion OCR>


        #region Propiedades Autocalculadas

        public string BuildOrdersDirectory {
            get {

                if (Preferencias.BuildOrderCustomDirectory != null) return Preferencias.BuildOrderCustomDirectory;
                return ObtenerDirectorioÓrdenesDeEjecución(DirectorioÓrdenesDeEjecución, Game);

            }
        }

        public string NamesDirectory {
            get {

                var directorioNombresJuego = Path.Combine(DirectorioNombres, Game);
                if (!Directory.Exists(directorioNombresJuego)) Directory.CreateDirectory(directorioNombresJuego);
                return directorioNombresJuego;

            }
        }

        public string ImagesDirectory {
            get {

                var directorioImágenesJuego = Path.Combine(DirectorioImágenes, Game);
                if (!Directory.Exists(directorioImágenesJuego)) Directory.CreateDirectory(directorioImágenesJuego);
                return directorioImágenesJuego;

            }
        }

        public string NamesPath => Path.Combine(NamesDirectory, "Names.json");

        public string CustomNamesDefaultPath => Path.Combine(NamesDirectory, "Custom Names.Default.json"); // Este archivo es el que se lee si no se encuentra CustomNamesPath. Se manejan por separado para que cualquier personalización realizada por el usuario a Names.Custom.json no se pierda al copiar una nueva versión de CustomNamesDefaultPath.

        public string CustomNamesPath => Path.Combine(NamesDirectory, "Custom Names.json");

        public string CustomImagesDefaultPath => Path.Combine(NamesDirectory, "Custom Images.Default.json");

        public string CustomImagesPath => Path.Combine(NamesDirectory, "Custom Images.json");

        public string TypesDefaultPath => Path.Combine(NamesDirectory, "Types.Default.json");

        public string TypesPath => Path.Combine(NamesDirectory, "Types.json");

        public string EnglishNamesPath => Path.Combine(NamesDirectory, "English Names.json"); // Es temporal, se escribe usando información de la hoja de excel para crear el Names.json que contiene los otros idiomas.

        #endregion Propiedades Autocalculadas>


        public string ObtenerGameSpeedText(string game) {

            if (game == AOE2Name) {
                return (GameSpeed == 1.7 ? "Normal" : (GameSpeed == 1 ? "Slow" : (GameSpeed == 1.5 ? "Casual" : (GameSpeed == 2 ? "Fast" : "Other"))));
            } else {
                return "Normal";
            }

        } // ObtenerGameSpeedText>


        public Dictionary<NameType, int> ObtenerDisplayPriorityOrdenadas()
            => DisplayPriority.OrderBy(kv => kv.Value).ToDictionary(g => g.Key, g => g.Value);


        public void EstablecerGameSpeed(string gameSpeedText, string game) {

            if (game == AOE2Name) {
                GameSpeed = (gameSpeedText == "Normal" ? 1.7 : (gameSpeedText == "Slow" ? 1 : (gameSpeedText == "Casual" ? 1.5 : (gameSpeedText == "Fast" ? 2 : 1))));
            } else {
                if (gameSpeedText != "Normal")
                    MostrarInformación($"The game {game} doesn't have speed options other than normal, normal speed will be used.");
                GameSpeed = 1;
            }

        } // EstablecerGameSpeed>


        public static string ObtenerDirectorioÓrdenesDeEjecución(string directorioPadre, string juego) {
     
            var directorioÓrdenesDeEjecución = Path.Combine(directorioPadre, juego);
            if (!Directory.Exists(directorioÓrdenesDeEjecución)) Directory.CreateDirectory(directorioÓrdenesDeEjecución);
            return directorioÓrdenesDeEjecución;

        } // ObtenerDirectorioÓrdenesDeEjecución>


        public void EstablecerValoresRecomendadosAOE2(string resolución) {

            ShowNextStep = false;
            ShowPreviousStep = true;
            GameSpeed = 1.7;
            StepDuration = 25; // El tiempo de creación de 1 aldeano.

            switch (resolución) {
                case "1920x1080":

                    Height = 146 * CorrecciónEscala;
                    Width = 495 * CorrecciónEscala;
                    Top = 718 * CorrecciónEscala;
                    Left = 722 * CorrecciónEscala;
                    CurrentStepFontSize = 18.5 * CorrecciónEscala;
                    NextPreviousStepFontSize = 11.5 * CorrecciónEscala;
                    ButtonsSize = 24 * CorrecciónEscala;
                    ButtonsMargin = 2 * CorrecciónEscala;
                    ButtonsPadding = 2 * CorrecciónEscala;
                    LargeFontSize = 16 * CorrecciónEscala;
                    MediumFontSize = 14 * CorrecciónEscala;
                    LeftMarginCurrentStep = 15 * CorrecciónEscala;
                    TopMarginCurrentStep = 3 * CorrecciónEscala;
                    TopMarginNextPreviousStep = 27 * CorrecciónEscala;
                    BottomMargenSteps = 3 * CorrecciónEscala;
                    RightMarginNextPreviousStep = 67 * CorrecciónEscala;
                    BuildOrderSelectorWidth = 145 * CorrecciónEscala;
                    ExecutionSpeedSelectorWidth = 55 * CorrecciónEscala;
                    ThicknessCircularProgressBar = 6 * CorrecciónEscala;
                    RightMarginCircularProgressBar = 6 * CorrecciónEscala;
                    break;

                case "2560x1440":
                case "3840x2160":

                    Height = 195 * CorrecciónEscala;
                    Width = 662 * CorrecciónEscala; // No cambiar, normalmente uso el minimapa 25% más grande y el valor mi valor sería 635.
                    Top = 957 * CorrecciónEscala;
                    Left = 962 * CorrecciónEscala;
                    CurrentStepFontSize = 24.7 * CorrecciónEscala;
                    NextPreviousStepFontSize = 16 * CorrecciónEscala;
                    ButtonsSize = 33 * CorrecciónEscala;
                    ButtonsMargin = 3 * CorrecciónEscala;
                    ButtonsPadding = 2 * CorrecciónEscala;
                    LargeFontSize = 24 * CorrecciónEscala;
                    MediumFontSize = 18 * CorrecciónEscala;
                    LeftMarginCurrentStep = 17 * CorrecciónEscala;
                    TopMarginCurrentStep = 5 * CorrecciónEscala;
                    TopMarginNextPreviousStep = 31 * CorrecciónEscala;
                    BottomMargenSteps = 5 * CorrecciónEscala;
                    RightMarginNextPreviousStep = 90 * CorrecciónEscala;
                    BuildOrderSelectorWidth = 200 * CorrecciónEscala;
                    ExecutionSpeedSelectorWidth = 75 * CorrecciónEscala;
                    ThicknessCircularProgressBar = 8 * CorrecciónEscala;
                    RightMarginCircularProgressBar = 7 * CorrecciónEscala;
                    break;

                case "1366x768":

                    Height = 105 * CorrecciónEscala;
                    Width = 354 * CorrecciónEscala;
                    Top = 510 * CorrecciónEscala;
                    Left = 513 * CorrecciónEscala;
                    CurrentStepFontSize = 13.5 * CorrecciónEscala;
                    NextPreviousStepFontSize = 8.3 * CorrecciónEscala;
                    ButtonsSize = 17 * CorrecciónEscala;
                    ButtonsMargin = 1 * CorrecciónEscala;
                    ButtonsPadding = 1 * CorrecciónEscala;
                    LargeFontSize = 12 * CorrecciónEscala;
                    MediumFontSize = 10 * CorrecciónEscala;
                    LeftMarginCurrentStep = 8 * CorrecciónEscala;
                    TopMarginCurrentStep = 2 * CorrecciónEscala;
                    TopMarginNextPreviousStep = 15 * CorrecciónEscala;
                    BottomMargenSteps = 2 * CorrecciónEscala;
                    RightMarginNextPreviousStep = 45 * CorrecciónEscala;
                    BuildOrderSelectorWidth = 110 * CorrecciónEscala;
                    ExecutionSpeedSelectorWidth = 40 * CorrecciónEscala;
                    ThicknessCircularProgressBar = 4 * CorrecciónEscala;
                    RightMarginCircularProgressBar = 4 * CorrecciónEscala;
                    break;

                default:
                    break;
            }

        } // EstablecerValoresRecomendadosAOE2>


        public void EstablecerValoresRecomendados(string resolución, string juego, bool cambióResolución) {

            Game = juego;
            ScreenResolution = resolución;

            if (juego == AOE2Name) {

                EstablecerValoresRecomendadosAOE2(resolución);
                if (cambióResolución) CrearOCompletarScreenCaptureRectangles(cambióResolución: true);

            } else if (juego == AOE4Name) {

                EstablecerValoresRecomendadosAOE2(resolución);
                
                GameSpeed = 1;
                ShowNextStep = false;
                ShowPreviousStep = false;
                StepDuration = 60; // 60 es el tiempo de creación de 3 aldeanos.

                switch (resolución) {
                    case "1920x1080":

                        BottomMargenSteps += 32 * CorrecciónEscala;
                        Height = 146 * CorrecciónEscala;
                        Width = 263 * CorrecciónEscala;
                        Top = 718 * CorrecciónEscala;
                        Left = 1044 * CorrecciónEscala;
                        CurrentStepFontSize = 14.5 * CorrecciónEscala;
                        BuildOrderSelectorWidth = 140 * CorrecciónEscala;
                        break;

                    case "2560x1440":
                    case "3840x2160":

                        BottomMargenSteps += 40 * CorrecciónEscala;
                        Width = 355 * CorrecciónEscala;
                        Left = 1389 * CorrecciónEscala;
                        Height = 195 * CorrecciónEscala;
                        Top = 957 * CorrecciónEscala;
                        CurrentStepFontSize = 19.5 * CorrecciónEscala;
                        BuildOrderSelectorWidth = 185 * CorrecciónEscala;
                        break;

                    case "1366x768":

                        BottomMargenSteps += 26 * CorrecciónEscala;
                        Height = 105 * CorrecciónEscala;
                        Width = 188 * CorrecciónEscala;
                        Top = 510 * CorrecciónEscala;
                        Left = 743 * CorrecciónEscala;
                        CurrentStepFontSize = 10 * CorrecciónEscala;
                        BuildOrderSelectorWidth = 97 * CorrecciónEscala;
                        break;

                    default:
                        break;
                }

            } else if (juego == OtherName) {

                EstablecerValoresRecomendadosAOE2(resolución);
                GameSpeed = 1;
                StepDuration = 60;

            }

        } // EstablecerValoresRecomendados>


        public static Settings? Deserializar(string json) => (string.IsNullOrEmpty(json)) ? default 
            : JsonSerializer.Deserialize<Settings>(json, ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración));


        public static string Serializar(Settings settings) 
            => JsonSerializer.Serialize(settings, ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración));


        public static void Guardar(Settings settings, string ruta) 
            => File.WriteAllText(ruta, Settings.Serializar(settings));


    } // Settings>


} // RTSHelper>
