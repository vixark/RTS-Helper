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

        public double GameSpeed { get; set; } = 1;

        public double ExecutionSpeed { get; set; } = 1; // 1 es la máxima velocidad. En esta velocidad cada conjunto de comandos corresponde a un minuto en el juego.

        public double StepDuration { get; set; } = StepDurationPredeterminado; 

        public string CurrentBuildOrder { get; set; } = "Default";

        public bool ShowNextStep { get; set; } = ShowNextStepPredeterminado;

        public string? BuildOrderDirectory { get; set; }

        public bool MinimizeOnComplete { get; set; } = true;

        public double LineSpacing { get; set; } = 20;

        #endregion Generales>


        #region Sonidos

        public string StepStartSound { get; set; } = StepStartSoundPredeterminado;

        public string StepEndSound { get; set; } = StepEndSoundPredeterminado;

        public int StepStartSoundVolume { get; set; } = StepStartSoundVolumePredeterminado;

        public int StepEndSoundVolume { get; set; } = StepEndSoundVolumePredeterminado;

        public int StepEndSoundDuration { get; set; } = 0; // Se establece automáticamente al cambiar el sonido.

        public bool UnmuteAtStartup { get; set; } = true; // Permite que no se mantenga muted si la última vez que se usó la aplicación se cerró muted.

        public bool Muted { get; set; } = false; // Se establece desde la interface principal.

        public bool MuteOnComplete { get; set; } = true;

        #endregion Sonidos>


        #region Colores

        public string BackColor { get; set; } = BackColorPredeterminado;

        public string FontColor { get; set; } = FontColorPredeterminado;

        public double Opacity { get; set; } = OpacityPredeterminado;

        public string CurrentStepFontColor { get; set; } = CurrentStepFontColorPredeterminado;

        public string NextStepFontColor { get; set; } = NextStepFontColorPredeterminado;

        public bool FlashOnStepChange { get; set; } = FlashOnStepChangePredeterminado;

        public string FlashingColor { get; set; } = FlashingColorPredeterminado;

        public double FlashingOpacity { get; set; } = FlashingOpacityPredeterminado;

        public bool StopFlashingOnComplete { get; set; } = StopFlashingOnCompletePredeterminado;

        #endregion Colores>


        #region Fuentes

        public string FontName { get; set; } = NombreFuentePredeterminada;

        public bool CurrentStepFontBold { get; set; } = CurrentStepFontBoldPredeterminado;

        public bool NextStepFontBold { get; set; } = false;

        #endregion Fuentes>


        #region Tamaños
        // Son establecidos automáticamente al cambiar el juego y la resolución.

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public double CurrentStepFontSize { get; set; }

        public double NextStepFontSize { get; set; }

        public double ButtonsSize { get; set; }

        public double ButtonsMargin { get; set; }

        public double ButtonsPadding { get; set; }

        public double LargeFontSize { get; set; }

        public double MediumFontSize { get; set; }

        public double LeftMarginCurrentStep { get; set; }

        public double TopMarginCurrentStep { get; set; }

        public double RightMarginNextStep { get; set; }

        public double TopMarginNextStep { get; set; }

        public double BottomMargenSteps { get; set; }

        public double BuildOrderSelectorWidth { get; set; }

        public double ExecutionSpeedSelectorWidth { get; set; }

        #endregion Tamaños>


        #region Imágenes

        public double ImageSize { get; set; } = ImageSizePredeterminado;

        public string ImageBackgroundColor { get; set; } = Color.FromRgb(0, 0, 0).ToString();

        public double ImageBackgroundOpacity { get; set; } = 0.3;

        public double EntityHorizontalMargin { get; set; } = 20; // Espaciado para las entidades: imágenes y textos entre []. Porcentaje del alto de la entidad  cada lado.

        public double ImageBackgroundRoundedCornersRadius { get; set; } = 10;

        public double SubscriptAndSuperscriptImagesSize { get; set; } = SubscriptAndSuperscriptImagesSizePredeterminado;

        #endregion Imágenes>


        #region Nombres

        public Dictionary<NameType, int> DisplayPriority { get; set; } = new Dictionary<NameType, int> {
            { NameType.Image, 0 }, { NameType.Abbreviation, 1 }, { NameType.Acronym, 2 }, { NameType.Common, 3 }, { NameType.Complete, 4 },
            { NameType.Custom, 5 }, { NameType.AbbreviationPlural, 6 }, { NameType.AcronymPlural, 7 }, { NameType.CommonPlural, 8 }, { NameType.BR, 9 },
            { NameType.DE, 10 }, { NameType.ES, 11 }, { NameType.FR, 12 }, { NameType.HI, 13 }, { NameType.IT, 14 }, { NameType.JP, 15 },
            { NameType.KO, 16 }, { NameType.MS, 17 }, { NameType.MX, 18 }, { NameType.PL, 19 }, { NameType.RU, 20 }, { NameType.TR, 21 },
            { NameType.TW, 22 }, { NameType.VI, 23 }, { NameType.ZH, 24 }
        };

        #endregion Nombres>


        #region Propiedades Autocalculadas

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


        public void EstablecerValoresRecomendadosAOE2(string resolución) {

            ShowNextStep = true;
            GameSpeed = 1.7;
            StepDuration = 50; // El tiempo de creación de 2 aldeanos.

            switch (resolución) {
                case "1920x1080":

                    Height = 146;
                    Width = 495;
                    Top = 718;
                    Left = 722;
                    CurrentStepFontSize = 19;
                    NextStepFontSize = 11.5;
                    ButtonsSize = 24;
                    ButtonsMargin = 2;
                    ButtonsPadding = 2;
                    LargeFontSize = 16;
                    MediumFontSize = 14;
                    LeftMarginCurrentStep = 15;
                    TopMarginCurrentStep = 3;
                    TopMarginNextStep = 27;
                    BottomMargenSteps = 3;
                    RightMarginNextStep = 67;
                    BuildOrderSelectorWidth = 145;
                    ExecutionSpeedSelectorWidth = 55;
                    break;

                case "2560x1440":

                    Height = 195;
                    Width = 662; // No cambiar, normalmente uso el minimapa 25% más grande y el valor mi valor sería 635.
                    Top = 957;
                    Left = 962;
                    CurrentStepFontSize = 25;
                    NextStepFontSize = 16;
                    ButtonsSize = 33;
                    ButtonsMargin = 3;
                    ButtonsPadding = 2;
                    LargeFontSize = 24;
                    MediumFontSize = 18;
                    LeftMarginCurrentStep = 17;
                    TopMarginCurrentStep = 5;
                    TopMarginNextStep = 31;
                    BottomMargenSteps = 5;
                    RightMarginNextStep = 90;
                    BuildOrderSelectorWidth = 200;
                    ExecutionSpeedSelectorWidth = 75;
                    break;

                case "1366x768":

                    Height = 105;
                    Width = 354;
                    Top = 510;
                    Left = 513;
                    CurrentStepFontSize = 13.5;
                    NextStepFontSize = 8.3;
                    ButtonsSize = 17;
                    ButtonsMargin = 1;
                    ButtonsPadding = 1;
                    LargeFontSize = 12;
                    MediumFontSize = 10;
                    LeftMarginCurrentStep = 8;
                    TopMarginCurrentStep = 2;
                    TopMarginNextStep = 15;
                    BottomMargenSteps = 2;
                    RightMarginNextStep = 45;
                    BuildOrderSelectorWidth = 110;
                    ExecutionSpeedSelectorWidth = 40;
                    break;

                default:
                    break;
            }

        } // EstablecerValoresRecomendadosAOE2>


        public void EstablecerValoresRecomendados(string resolución, string juego) {

            Game = juego;
            ScreenResolution = resolución;

            if (juego == AOE2Name) {

                EstablecerValoresRecomendadosAOE2(resolución);

            } else if (juego == AOE4Name) {

                EstablecerValoresRecomendadosAOE2(resolución);
                
                GameSpeed = 1;
                ShowNextStep = false;
                StepDuration = 60; // 60 es el tiempo de creación de 3 aldeanos.

                switch (resolución) {
                    case "1920x1080":

                        BottomMargenSteps += 32;
                        Height = 146;
                        Width = 263;
                        Top = 718;
                        Left = 1044;
                        CurrentStepFontSize = 14.5;
                        BuildOrderSelectorWidth = 140;
                        break;

                    case "2560x1440":

                        BottomMargenSteps += 40;
                        Width = 355;
                        Left = 1389;
                        Height = 195;
                        Top = 957;
                        CurrentStepFontSize = 19.5;
                        BuildOrderSelectorWidth = 185;
                        break;

                    case "1366x768":

                        BottomMargenSteps += 26;
                        Height = 105;
                        Width = 188;
                        Top = 510;
                        Left = 743;
                        CurrentStepFontSize = 10;
                        BuildOrderSelectorWidth = 97;
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
