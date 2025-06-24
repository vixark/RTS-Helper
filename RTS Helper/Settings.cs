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
using System.Text.RegularExpressions;



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

        public bool MinimizeOnComplete { get; set; } = false;

        public double LineSpacing { get; set; } = 35; // Se da prioridad a mostrar 3 líneas de instrucciones lo más amplias posibles y una línea de detalles adicionales inferior de fuente más pequeña.

        public bool ShowStepProgress { get; set; } = true;

        public bool ShowTime { get; set; } = false;

        public bool ShowAlternateNextPreviousStepButton { get; set; } = true;

        public bool ShowAlwaysStatsButton { get; set; } = true; // Si es falso, solo se muestra al final.

        public DateTime LastDateDonationSuggestion { get; set; } = DateTime.MinValue;

        public bool ShowOptionalInstructions1 { get; set; } = true; // Para los juegos de Age of Empires es la creación de aldeanos.

        public bool ShowOptionalInstructions2 { get; set; } = true; // Para los juegos de Age of Empires es la construcción de casas.

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

        public string SecondaryFontName { get; set; } = NombreFuenteSecundariaPredeterminada; // Principalmente usada para números y símbolos.

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

        public int MinimumDelayToAutoAdjustIdleTime { get; set; } = 2; // 3 me parecía un poco alto. En algunas ocasiones quería que se sincronizara automáticamente y sentía que el desface que quedaba sin autoajuste era más grande de lo que quisiera.

        public int ForwardSeconds { get; set; } = 10;

        public int BackwardSeconds { get; set; } = 10;

        public int NextMultipleSteps { get; set; } = 10; // 10 pasos es para avanzar una cantidad considerable para 3 clics. Antes lo tenía en 5 y no lo usaba porque solo ahorraba 2 clics comparado con usar el botón de siguiente 5 veces.

        public int BackMultipleSteps { get; set; } = 10;

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

        public string UIMod { get; set; } = Global.UIMod.No_Mod.ToString();

        #endregion OCR>


        #region Actualizaciones

        public int ImagesVersion { get; set; } = 5; // 3. Primeras imágenes de AOM. 4. Imagen de gallinas y drop resources para AOE2. 5. Imágenes de China de AOM.

        public int SoundsVersion { get; set; } = 1; // 1. Primeros sonidos.

        public int BuildOrdersVersion { get; set; } = 8; // 4. Primeras estrategias de AOM. 5. Una estrategia con gallinas para AOE2. 6. Estrategia de con gallinas para Malian MAA para AOE2. 7. Estrategias para AOM de DoD Clan. 8. Estrategia de AOM para China y cambio de tiempos de creación de aldeanos en el resto de las estrategias.

        public string UpdatesBaseUrl = "https://vixark.b-cdn.net/rts-helper"; // Es cambiada cuando se lee desde el Json. Este valor solo sirve para la ejecución inicial. Debería ser cambiado cuando se cambie la ruta del CDN para que los usuarios que lo instalen por primera vez puedan acceder a las actualizaciones automáticas.

        #endregion Actualizaciones>


        #region Propiedades Autocalculadas

        public string BuildOrdersDirectory { get => ObtenerDirectorioEstrategias(DirectorioEstrategias, Game); }

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

        public int WidthScreenResolution => int.Parse(Regex.Match(Preferencias.ScreenResolution, "([0-9]+)x").Groups[1].Value);

        public int HeightScreenResolution => int.Parse(Regex.Match(Preferencias.ScreenResolution, "x([0-9]+)").Groups[1].Value);

        #endregion Propiedades Autocalculadas>


        public string ObtenerGameSpeedText(string game) {

            if (game == AOE2Name) {
                return (GameSpeed == 1.7 ? "Normal" : (GameSpeed == 1 ? "Slow" : (GameSpeed == 1.5 ? "Casual" : (GameSpeed == 2 ? "Fast" : "Other"))));
            } else if (game == AOMName) {
                return (GameSpeed == AOMNormalSpeed ? "Normal" : 
                    (GameSpeed == AOMNormalSpeed * 0.5 * (599D / 600) ? "Slow" : // Este valor no se pudo conseguir de manera muy consistente. Sospecho que hay causas que hacen que esto sea algo aleatoria y dependiendo de cierto 'lag'. Se usa un factor relativamente burdo de 1 segundo de extra velocidad de RTS Helper por 10 minutos de juego.
                    (GameSpeed == 2 ? "Fast" : 
                    (GameSpeed == 0.3333 * 1.020408 ? "Consider the internet" 
                    : (GameSpeed == 2.5 ? "Lets go! now!" : "Other")))));
            } else {
                return "Normal";
            }

        } // ObtenerGameSpeedText>


        public Dictionary<NameType, int> ObtenerDisplayPriorityOrdenadas()
            => DisplayPriority.OrderBy(kv => kv.Value).ToDictionary(g => g.Key, g => g.Value);


        public void EstablecerGameSpeed(string gameSpeedText, string game) {

            if (game == AOE2Name) {
                GameSpeed = (gameSpeedText == "Normal" ? 1.7 : (gameSpeedText == "Slow" ? 1 : (gameSpeedText == "Casual" ? 1.5 :
                    (gameSpeedText == "Fast" ? 2 : 1.7))));
            } else if (game == AOMName) {
                GameSpeed = (gameSpeedText == "Normal" ? AOMNormalSpeed :
                    (gameSpeedText == "Slow" ? AOMNormalSpeed * 0.5 * (599D / 600) : 
                    (gameSpeedText == "Consider the internet" ? 0.3333 * 1.020408 : // 1200 segundos del juego dieron 1176 segundos dieron RTS Helper. El juego estaba más rapido por 1,020408163265306.
                    (gameSpeedText == "Fast" ? 2 : 
                    (gameSpeedText == "Lets go! now!" ? 2.5 : 1)))));
            } else {
                if (gameSpeedText != "Normal")
                    MostrarInformación($"The game {game} doesn't have speed options other than normal. Normal speed will be used.");
                GameSpeed = 1;
            }
            
        } // EstablecerGameSpeed>


        public static string ObtenerDirectorioEstrategias(string directorioPadre, string juego) {
     
            var directorioÓrdenesDeEjecución = Path.Combine(directorioPadre, juego);
            if (!Directory.Exists(directorioÓrdenesDeEjecución)) Directory.CreateDirectory(directorioÓrdenesDeEjecución);
            return directorioÓrdenesDeEjecución;

        } // ObtenerDirectorioÓrdenesDeEjecución>


        public void EstablecerValoresRecomendadosAOE2(string resolución, bool cambióJuego) {

            if (cambióJuego) {
                ShowNextStep = false;
                ShowPreviousStep = true;
                GameSpeed = 1.7;
                ShowAlwaysStatsButton = true;
                StepDuration = 25; // El tiempo de creación de 1 aldeano.
            }

            switch (resolución) {
                case "1920x1080":
                case "1680x1050":
                case "1920x1200":

                    Height = 146 * CorrecciónEscala;
                    Width = 495 * CorrecciónEscala;
                    Top = 718 * CorrecciónEscala;
                    Left = 722 * CorrecciónEscala;
                    if (resolución == "1680x1050") {
                        Top = 695 * CorrecciónEscala;
                        Left = 600 * CorrecciónEscala;
                    }
                    if (resolución == "1920x1200") {
                        Top = 815 * CorrecciónEscala;
                        Left = 722 * CorrecciónEscala;
                    }
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

                    Height = 200 * CorrecciónEscala;
                    Width = 662 * CorrecciónEscala; // No cambiar. Normalmente uso el minimapa 25% más grande y el valor mi valor sería 635.
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
                    BottomMargenSteps = 3 * CorrecciónEscala;
                    RightMarginNextPreviousStep = 90 * CorrecciónEscala;
                    BuildOrderSelectorWidth = 200 * CorrecciónEscala;
                    ExecutionSpeedSelectorWidth = 75 * CorrecciónEscala;
                    ThicknessCircularProgressBar = 8 * CorrecciónEscala;
                    RightMarginCircularProgressBar = 7 * CorrecciónEscala;

                    if (resolución == "3840x2160") {

                        var factorIncrementoVs1440p = 1.5;
                        var factorIncrementoUIVs1440p = 1.25; // Los elementos de interface no es necesario incrementarlos en la misma proporción, pueden ser más pequeños.
                        Height*= factorIncrementoVs1440p;
                        Width *= factorIncrementoVs1440p; // No cambiar. Normalmente uso el minimapa 25% más grande y el valor mi valor sería 635.
                        Top *= factorIncrementoVs1440p;
                        Left *= factorIncrementoVs1440p;
                        CurrentStepFontSize *= factorIncrementoVs1440p;
                        NextPreviousStepFontSize *= factorIncrementoVs1440p;
                        ButtonsSize *= factorIncrementoUIVs1440p;
                        ButtonsMargin *= factorIncrementoUIVs1440p;
                        ButtonsPadding *= factorIncrementoUIVs1440p;
                        LargeFontSize *= factorIncrementoUIVs1440p;
                        MediumFontSize *= factorIncrementoUIVs1440p;
                        LeftMarginCurrentStep *= factorIncrementoVs1440p;
                        TopMarginCurrentStep *= factorIncrementoVs1440p;
                        TopMarginNextPreviousStep *= factorIncrementoUIVs1440p;
                        BottomMargenSteps *= factorIncrementoUIVs1440p;
                        RightMarginNextPreviousStep *= factorIncrementoUIVs1440p;
                        BuildOrderSelectorWidth *= factorIncrementoUIVs1440p;
                        ExecutionSpeedSelectorWidth *= factorIncrementoUIVs1440p;
                        ThicknessCircularProgressBar *= factorIncrementoUIVs1440p;
                        RightMarginCircularProgressBar *= factorIncrementoUIVs1440p;

                    }

                    break;

                case "1366x768":
                case "1280x720":
                case "1360x768":
                case "1280x800":

                    Height = 105 * CorrecciónEscala;
                    Width = 354 * CorrecciónEscala;
                    Top = 510 * CorrecciónEscala;
                    Left = 513 * CorrecciónEscala;
                    if (resolución == "1280x720") {
                        Top = 472 * CorrecciónEscala;
                        Left = 470 * CorrecciónEscala;
                    }
                    if (resolución == "1280x800") {
                        Top = 538 * CorrecciónEscala;
                        Left = 475 * CorrecciónEscala;
                    }
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

                case "1600x900":
                case "1440x900":

                    Height = 125 * CorrecciónEscala;
                    Width = 424 * CorrecciónEscala;
                    Top = 595 * CorrecciónEscala;
                    Left = 595 * CorrecciónEscala;
                    if (resolución == "1440x900") {
                        Top = 595 * CorrecciónEscala;
                        Left = 510 * CorrecciónEscala;
                    }
                    CurrentStepFontSize = 16 * CorrecciónEscala;
                    NextPreviousStepFontSize = 10 * CorrecciónEscala;
                    ButtonsSize = 20.5 * CorrecciónEscala;
                    ButtonsMargin = 1.5 * CorrecciónEscala;
                    ButtonsPadding = 1.5 * CorrecciónEscala;
                    LargeFontSize = 14 * CorrecciónEscala;
                    MediumFontSize = 12 * CorrecciónEscala;
                    LeftMarginCurrentStep = 11.5 * CorrecciónEscala;
                    TopMarginCurrentStep = 2.5 * CorrecciónEscala;
                    TopMarginNextPreviousStep = 21 * CorrecciónEscala;
                    BottomMargenSteps = 2.5 * CorrecciónEscala;
                    RightMarginNextPreviousStep = 56 * CorrecciónEscala;
                    BuildOrderSelectorWidth = 128 * CorrecciónEscala;
                    ExecutionSpeedSelectorWidth = 47.5 * CorrecciónEscala;
                    ThicknessCircularProgressBar = 5 * CorrecciónEscala;
                    RightMarginCircularProgressBar = 5 * CorrecciónEscala;
                    break;

                default:
                    break;
            }

        } // EstablecerValoresRecomendadosAOE2>


        public void EstablecerValoresRecomendados(string resolución, string juego, bool cambióResolución, bool cambióUIMod, bool cambióJuego) {

            Game = juego;
            ScreenResolution = resolución;

            if (juego == AOE2Name) {

                EstablecerValoresRecomendadosAOE2(resolución, cambióJuego);
                if (cambióResolución || cambióUIMod) CrearOCompletarScreenCaptureRectangles(cambióResolución: true, cambióUIMod);

            } else if (juego == AOE4Name) {

                EstablecerValoresRecomendadosAOE2(resolución, cambióJuego);
                
                if (cambióJuego) {
                    GameSpeed = 1;
                    ShowNextStep = false;
                    ShowPreviousStep = false;
                    ShowAlwaysStatsButton = true;
                    StepDuration = 60; // 60 es el tiempo de creación de 3 aldeanos.
                }

                switch (resolución) {
                    case "1920x1080":
                    case "1680x1050":
                    case "1920x1200":

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
                    case "1280x720":
                    case "1360x768":
                    case "1280x800":

                        BottomMargenSteps += 26 * CorrecciónEscala;
                        Height = 105 * CorrecciónEscala;
                        Width = 188 * CorrecciónEscala;
                        Top = 510 * CorrecciónEscala;
                        Left = 743 * CorrecciónEscala;
                        CurrentStepFontSize = 10 * CorrecciónEscala;
                        BuildOrderSelectorWidth = 97 * CorrecciónEscala;
                        break;

                    case "1600x900":
                    case "1440x900":

                        BottomMargenSteps += 29 * CorrecciónEscala;
                        Height = 125 * CorrecciónEscala;
                        Width = 226 * CorrecciónEscala;
                        Top = 614 * CorrecciónEscala;
                        Left = 894 * CorrecciónEscala;
                        CurrentStepFontSize = 12.3 * CorrecciónEscala;
                        BuildOrderSelectorWidth = 119 * CorrecciónEscala;
                        break;

                    default:
                        break;
                }

            } else if (juego == AOMName) {

                EstablecerValoresRecomendadosAOE2(resolución, cambióJuego); // Para evitar enredos con los tamaños y posiciones, se usa casi lo mismo que en AOE2, solo se incrementa ligeramente el ancho.
                var factorAnchoAOE2aAOM = 1; // Arbitrario. Inicialmente se quería poner un valor mayor a uno para aprovechar que hay más espacio en AOM, pero se prefirió dejar los tamaños iguales por consistencia.
                Width = Width * factorAnchoAOE2aAOM;

                var factorIzquierdaAOE2aAOM = 1.172; // Establecido iterando usando mi pantalla para que quede justo al lado izquierdo del mapa partiendo del punto optimo de Age of Empires II. Este factor depende de ambos juegos. Si cambia la interface de uno de ellos, se debe cambiar.
                Left = Left * factorIzquierdaAOE2aAOM;

                if (cambióJuego) {
                    GameSpeed = AOMNormalSpeed;
                    ShowNextStep = false;
                    ShowPreviousStep = true;
                    ShowAlwaysStatsButton = false;
                    StepDuration = 15; // 15 es el tiempo de creación de 1 aldeano de los griegos.
                    BuildOrderSelectorWidth = 250 * CorrecciónEscala;
                }

            } else if (juego == OtherName) {

                EstablecerValoresRecomendadosAOE2(resolución, cambióJuego);
                if (cambióJuego) {
                    GameSpeed = 1;
                    StepDuration = 60;
                }

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
